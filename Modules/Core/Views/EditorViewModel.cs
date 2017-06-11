using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Practices.ServiceLocation;
using Prism;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using Wayne.Payment.Products.iXConfigurator.Template;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Constants;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Events;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Extensions;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Interfaces;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Models;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Mvvm;
using Wayne.Payment.Products.iXConfigurator.Modules.Core.Commands;
using Task = System.Threading.Tasks.Task;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.Views
{
    public class EditorViewModel : ViewModelBase, IConfirmNavigationRequest, IActiveAware
    {
        private IApplicationLogger _logger;
        private IEventAggregator _events;
        private ITemplateManager _templateManager;
        private IMessageDisplayService _messageService;
        private IEditorView _view;
        private ILibraryService _librarian;
        private MetroWindow _mainWindow;
        private string _text;
        private string _revertText;
        private bool _isTextChanged;
        private bool _isSaving;
        private string _key;
        private bool _isActive;

        public EditorViewModel(IApplicationLogger logger, IEventAggregator events, ITemplateManager templateManager, 
            IMessageDisplayService messageService, IView view, ILibraryService librarian)
        {
            _logger = logger;
            _events = events;
            _templateManager = templateManager;
            _messageService = messageService;
            _view = (IEditorView) view;
            _librarian = librarian;

            _mainWindow = (MetroWindow) ServiceLocator.Current.GetInstance<Window>(ViewNames.MainWindow);

            Title = "Editor";

            events.GetEvent<TemplateSelectedEvent>().Subscribe(OnTemplateSelected);

            SaveCommand = new DelegateCommand(OnSave, () => IsTextChanged);
            FormatCommand = new DelegateCommand(() => Text = Text.FormattedXml(), () => !string.IsNullOrWhiteSpace(Text));

            NewConfigurationCommand = new DelegateCommand<BaseMetroDialog>(NewConfiguration, dialog => true);
            CopyConfigurationCommand = new DelegateCommand<BaseMetroDialog>(CopyConfiguration, dialog => true);
        }

        public DelegateCommand SaveCommand { get; }

        public DelegateCommand FormatCommand { get; }

        public DelegateCommand<BaseMetroDialog> NewConfigurationCommand { get; }

        public DelegateCommand<BaseMetroDialog> CopyConfigurationCommand { get; }

        private async void NewConfiguration(BaseMetroDialog dialog)
        {
            dialog.CommandBindings.Clear();

            dialog.CommandBindings.Add(new CommandBinding(RoutedCommands.AffirmConfigurationDialogCommand,
                AffirmNewConfigurationDialog, CanAffirmConfigurationDialog));

            dialog.CommandBindings.Add(new CommandBinding(RoutedCommands.CancelConfigurationDialogCommand,
                CancelConfigurationDialog));

            dialog.Title = "New Configuration";

            var configuration = new NewConfiguration();

            dialog.DataContext = configuration;

            await _mainWindow.ShowMetroDialogAsync(dialog);
        }

        private async void CopyConfiguration(BaseMetroDialog dialog)
        {
            dialog.CommandBindings.Clear();

            dialog.CommandBindings.Add(new CommandBinding(RoutedCommands.AffirmConfigurationDialogCommand,
                AffirmCopyConfigurationDialog, CanAffirmConfigurationDialog));

            dialog.CommandBindings.Add(new CommandBinding(RoutedCommands.CancelConfigurationDialogCommand,
                CancelConfigurationDialog));

            dialog.Title = "Copy Configuration";

            var template = await _templateManager.GetTemplateAsync(_key);

            var configuration = new NewConfiguration
                {
                    Name = template.Product.Name,
                    Version = template.Product.Version,
                    Description = template.Product.Description,
                    FolderName = $"{_key} (2)"
                };

            dialog.DataContext = configuration;

            await _mainWindow.ShowMetroDialogAsync(dialog);
        }

        private async void CancelConfigurationDialog(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = sender as CustomDialog;
            Debug.Assert(dialog != null);

            await _mainWindow.HideMetroDialogAsync(dialog);
        }

        private void CanAffirmConfigurationDialog(object sender, CanExecuteRoutedEventArgs e)
        {
            var dialog = sender as CustomDialog;
            Debug.Assert(dialog != null);

            var configuration = dialog.DataContext as NewConfiguration;
            Debug.Assert(configuration != null);

            e.CanExecute = !string.IsNullOrWhiteSpace(configuration.Name) &&
                           !string.IsNullOrWhiteSpace(configuration.Version) && 
                           !configuration.HasErrors;
            e.Handled = true;
        }

        private async void AffirmNewConfigurationDialog(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                var dialog = sender as CustomDialog;
                Debug.Assert(dialog != null);

                var configuration = dialog.DataContext as NewConfiguration;
                Debug.Assert(configuration != null);

                await NewConfigurationAsync(configuration);

                await _mainWindow.HideMetroDialogAsync(dialog);
            }
            catch (Exception ex)
            {
                _logger.Log(ex);
                await _messageService.ShowMessageAsync("New Product Configuration",
                        "Error occured creating new configuration");
            }
        }

        private async void AffirmCopyConfigurationDialog(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                var dialog = sender as CustomDialog;
                Debug.Assert(dialog != null);

                var configuration = dialog.DataContext as NewConfiguration;
                Debug.Assert(configuration != null);

                if (configuration.HasErrors)
                {
                    return;
                }

                await CopyConfigurationAsync(configuration);

                await _mainWindow.HideMetroDialogAsync(dialog);
            }
            catch (Exception ex)
            {
                _logger.Log(ex);
                await _messageService.ShowMessageAsync("New Product Configuration",
                        "Error occured creating new configuration");
            }
        }

        private async Task NewConfigurationAsync(NewConfiguration configuration)
        {
            var folderName = configuration.FolderName;

            if (string.IsNullOrWhiteSpace(folderName))
            {
                folderName = $"{configuration.Name} {configuration.Version}";
            }

            var libraryPath = await _librarian.CreateLibraryPathAsync(folderName);
            var templatePath = await _librarian.CreateTemplatePathAsync(folderName);

            var template = new Configuration(libraryPath, templatePath)
                {
                    Product = new Product(configuration.Name, configuration.Version, configuration.Description)
                };

            var page = new Page("General Configuration", "true", "[Page description goes here]", "[Page summary goes here]");

            template.Pages.Add(page);

            var section = new Section("Language Configuration", "true", "[Section description goes here]");

            page.Sections.Add(section);

            var option = new Option("languageCode", "Language:", "true");

            section.Options.Add(option);

            var combo = new ComboControl("true", "true");

            option.Control = combo;

            var englishItem = new Item("en", "English");

            combo.Items.Add(englishItem);

            var frenchItem = new Item("fr", "French");

            combo.Items.Add(frenchItem);

            _key = await _templateManager.AddTemplateAsync(template);

            _events.GetEvent<SelectTemplateEvent>().Publish(_key);
        }

        private async Task CopyConfigurationAsync(NewConfiguration configuration)
        {
            var folderName = configuration.FolderName;

            if (string.IsNullOrWhiteSpace(folderName))
            {
                folderName = $"{configuration.Name} {configuration.Version}";
            }

            var libraryPath = await _librarian.CreateLibraryPathAsync(folderName);
            var templatePath = await _librarian.CreateTemplatePathAsync(folderName);

            var template = await _templateManager.GetTemplateAsync(_key);

            template.LibraryPath = libraryPath;
            template.TemplatePath = templatePath;

            template.Product.Name = configuration.Name;
            template.Product.Version = configuration.Version;
            template.Product.Description = configuration.Description;

            _key = await _templateManager.AddTemplateAsync(template);

            _events.GetEvent<SelectTemplateEvent>().Publish(_key);
        }

        private async void OnSave()
        {
            await Save();
        }

        private async Task<bool> Save()
        {
            try
            {
                IsSaving = true;

                if (!_view.Validate())
                    return false;

                await _templateManager.SaveTemplateAsync(_key, Text);

                _revertText = Text;

                IsTextChanged = false;
            }
            catch (Exception ex)
            {
                _logger.Log(ex);
                await _messageService.ShowMessageAsync("Save File", "Error occured while saving template");
                return false;
            }
            finally
            {
                IsSaving = false;
            }

            return true;
        }

        public bool IsSaving
        {
            get { return _isSaving; }
            set { SetProperty(ref _isSaving, value); }
        }

        public string Text
        {
            get { return _text; }
            set { SetProperty(ref _text, value, () => IsTextChanged = true); }
        }

        public bool IsTextChanged
        {
            get { return _isTextChanged; }
            set
            {
                SetProperty(ref _isTextChanged, value, () =>
                {
                    SaveCommand.RaiseCanExecuteChanged();
                    FormatCommand.RaiseCanExecuteChanged();
                });
            }
        }

        private async void OnTemplateSelected(string key)
        {
            _key = key;

            await LoadTextAsync();
        }

        private async Task LoadTextAsync()
        {
            try
            {
                Text = await _templateManager.GetTemplateTextAsync(_key);

                _revertText = Text;

                IsTextChanged = false;
            }
            catch (Exception ex)
            {
                _logger.Log(ex);
                await _messageService.ShowMessageAsync("Read Template", $"An error occured reading template for {_key}");
            }
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public async void ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback)
        {
            var confirm = true;

            if (IsTextChanged)
            {
                var settings = new MetroDialogSettings
                {
                    AffirmativeButtonText = "Discard",
                    NegativeButtonText = "Cancel",
                    DefaultButtonFocus = MessageDialogResult.Negative,
                    AnimateShow = true,
                    AnimateHide = false
                };

                var result = await _messageService.ShowMessageAsync("Discard Changes",
                    "If you continue your changes will be lost. Do you want to discard changes?",
                    MessageDialogStyle.AffirmativeAndNegative, settings);

                if (result == MessageDialogResult.Negative)
                { 
                        confirm = false;
                }
                else if (result == MessageDialogResult.Affirmative)
                {
                    Text = _revertText;
                }
            }

            continuationCallback(confirm);
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
