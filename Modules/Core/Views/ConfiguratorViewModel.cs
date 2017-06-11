using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Practices.ServiceLocation;
using Prism;
using Prism.Commands;
using Prism.Events;
using Prism.Logging;
using Prism.Regions;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Commands;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Constants;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Events;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Interfaces;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Mvvm;
using Wayne.Payment.Products.iXConfigurator.Modules.Core.ElementViewModels;
using Task = System.Threading.Tasks.Task;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.Views
{
    public class ConfiguratorViewModel : ViewModelBase, IConfirmNavigationRequest, IActiveAware
    {
        private IApplicationLogger _logger;
        private ITemplateManager _templateManager;
        private IDriveDetector _driveDetector;
        private IMessageDisplayService _messageService;
        private ElementViewModelFactory _elementFactory;
        private IConfiguratorService _configurator;
        private IVariableStore _variables;
        private ConfigurationViewModel _configuration;
        private PageViewModel _selectedPage;
        private DriveViewModel _selectedDrive;
        private bool _isBuilding;
        private bool _isScanning;
        private bool _isLastPage;
        private string _key;
        private bool _isActive;

        public ConfiguratorViewModel(IApplicationLogger logger, IEventAggregator events, IApplicationCommands commands,
            ITemplateManager templateManager, IDriveDetector driveDetector, IMessageDisplayService messageService,
            ElementViewModelFactory elementFactory, IConfiguratorService configurator, IVariableStore variables)
        {
            _logger = logger;
            _templateManager = templateManager;
            _driveDetector = driveDetector;
            _messageService = messageService;
            _elementFactory = elementFactory;
            _configurator = configurator;
            _variables = variables;

            Title = "Configurator";

            events.GetEvent<TemplateSelectedEvent>().Subscribe(OnTemplateSelected);
            events.GetEvent<ScanDrivesCompletedEvent>().Subscribe(OnScanDrivesCompleted);

            ImportCommand = new DelegateCommand(OnImport, () => true);
            commands.ImportCommand.RegisterCommand(ImportCommand);

            PrevPageCommand = new DelegateCommand<PageViewModel>(OnPrevPage, CanPrevPage);
            NextPageCommand = new DelegateCommand<PageViewModel>(OnNextPage, page => true);
            RescanCommand = new DelegateCommand(OnScanDrives, () => !IsScanning);

            RescanCommand.Execute();
        }

        public DelegateCommand RescanCommand { get; }

        public bool IsScanning
        {
            get { return _isScanning; }
            set { SetProperty(ref _isScanning, value, () => RescanCommand.RaiseCanExecuteChanged()); }
        }

        public bool IsBuilding
        {
            get { return _isBuilding; }
            set { SetProperty(ref _isBuilding, value); }
        }

        private async void OnScanDrives()
        {
            try
            {
                IsScanning = true;

                // Give time to show indicator to user 
                await Task.Delay(2000);

                await _driveDetector.ScanAttachedDrivesAsync();
            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message, Category.Exception, Priority.High);
            }
        }

        private void OnScanDrivesCompleted(IEnumerable<InstalledMountedDriveInfo> drives)
        {
            Drives.Clear();

            Drives.AddRange(drives.Select(drive => new DriveViewModel(drive)));

            SelectedDrive = Drives.FirstOrDefault();

            IsScanning = false;
        }

        private void OnNextPage(PageViewModel page)
        {
            if (!page.Validate())
            {
                return;
            }

            var index = Configuration.Pages.IndexOf(page);

            if (index == Configuration.Pages.Count - 1)
            {
                StartBuildAsync();
                return;
            }

            MovePage(index + 1);
        }

        private bool CanPrevPage(PageViewModel page)
        {
            var index = Configuration.Pages.IndexOf(page);
            return index > 0;
        }

        private void OnPrevPage(PageViewModel page)
        {
            var index = Configuration.Pages.IndexOf(page);

            if (index == 0)
            {
                return;
            }

            MovePage(index - 1);
        }

        private void MovePage(int index)
        {
            SelectedPage = Configuration.Pages.ElementAt(index);
        }

        public bool IsLastPage
        {
            get { return _isLastPage; }
            set { SetProperty(ref _isLastPage, value); }
        }

        private void OnSelectedPageChanged()
        {
            try
            {
                var index = Configuration.Pages.IndexOf(SelectedPage);

                for (int i = 0; i < index; i++)
                {
                    var page = Configuration.Pages.ElementAt(i);

                    if (!page.Validate())
                    {
                        Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(() => SelectedPage = page));
                        return;
                    }
                }

                IsLastPage = index == Configuration.Pages.Count - 1;
            }
            finally
            {
                NextPageCommand.RaiseCanExecuteChanged();
                PrevPageCommand.RaiseCanExecuteChanged();
            }
        }

        private async void StartBuildAsync()
        {
            ProgressDialogController progress = null;

            try
            {
                IsBuilding = true;

                foreach (var page in Configuration.Pages)
                {
                    if (!page.Validate())
                    {
                        SelectedPage = page;
                        return;
                    }
                }

                if (SelectedDrive == null)
                {
                    await
                        _messageService.ShowMessageAsync("No Drive Selected",
                            "Please select a USB drive to copy install files.");
                    return;
                }

                if (!SelectedDrive.IsFatFormat)
                {
                    await
                        _messageService.ShowMessageAsync("The selected drive is not formatted for FAT (FAT16 or FAT32) and cannot " +
                                                         "be used for this purpose.Please format the drive and try again.",
                                                         "Invalid Drive Format");
                    return;
                }

                var settings = new MetroDialogSettings
                {
                    AffirmativeButtonText = "Continue",
                    NegativeButtonText = "Stop",
                    AnimateShow = true,
                    AnimateHide = false
                };

                var result = await _messageService.ShowMessageAsync("Build Warning!",
                    "All files on the destination drive are about to be erased.  Do you wish to continue?",
                    MessageDialogStyle.AffirmativeAndNegative, settings);

                if (result == MessageDialogResult.Negative)
                {
                    return;
                }

                var mainWindow = (MetroWindow) ServiceLocator.Current.GetInstance<Window>(ViewNames.MainWindow);

                progress = await mainWindow.ShowProgressAsync("Building...", "Preparing build...");

                await _configurator.BuildAsync(_key, SelectedDrive.Name, (title, message, value) =>
                {
                    progress.SetTitle(title);
                    progress.SetMessage(message);
                    progress.SetProgress(value);
                });
            }
            catch (Exception ex)
            {
                await _messageService.ShowMessageAsync("Build Failed", "Please check the logs for error details");
                _logger.Log(ex);
            }
            finally
            {
                if (progress != null)
                {
                    await progress.CloseAsync();
                }

                IsBuilding = false;
            }
        }

        public DriveViewModel SelectedDrive
        {
            get { return _selectedDrive; }
            set { SetProperty(ref _selectedDrive, value); }
        }

        public ObservableCollection<DriveViewModel> Drives { get; } = new ObservableCollection<DriveViewModel>();
         
        public ICommand ImportCommand { get; }

        public DelegateCommand<PageViewModel> PrevPageCommand { get; }

        public DelegateCommand<PageViewModel> NextPageCommand { get; }

        public ConfigurationViewModel Configuration
        {
            get { return _configuration; }
            set { SetProperty(ref _configuration, value); }
        }

        public PageViewModel SelectedPage
        {
            get { return _selectedPage; }
            set { SetProperty(ref _selectedPage, value, OnSelectedPageChanged); }
        }

        private async void OnTemplateSelected(string key)
        {
            _key = key;

            await LoadConfigurationAsync();
        }

        private async Task LoadConfigurationAsync()
        {
            try
            {
                _variables.ClearVariables();

                var template = await _templateManager.GetTemplateAsync(_key);

                Configuration?.Dispose();

                Configuration = _elementFactory.CreateConfiguration(_key, template);

                Configuration.Initialize();

                SelectedPage = Configuration.Pages.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.Log(ex.Message, Category.Exception, Priority.High);
            }
        }

        private void OnImport()
        {
        }

        public async void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (string.IsNullOrWhiteSpace(_key))
                return;

            await LoadConfigurationAsync();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public void ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback)
        {
            continuationCallback(true);
        }

        public event EventHandler IsActiveChanged;

        public bool IsActive
        {
            get { return _isActive; }
            set { SetProperty(ref _isActive, value, OnActiveChanged); }
        }

        private void OnActiveChanged()
        {
            RaisActiveChanged();
        }

        private void RaisActiveChanged()
        {
            IsActiveChanged?.Invoke(this, new EventArgs());
        }
    }
}
