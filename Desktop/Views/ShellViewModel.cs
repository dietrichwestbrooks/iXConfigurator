using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Microsoft.Practices.ServiceLocation;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Commands;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Constants;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Events;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Interfaces;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Mvvm;

namespace Wayne.Payment.Products.iXConfigurator.Desktop.Views
{
    public class ShellViewModel : ViewModelBase
    {
        private IEventAggregator _events;
        private IRegionManager _regionManager;
        private string _selectedTemplate;
        private bool _isAdminMode;
        private bool _isEditorOpen;
        private string _libraryPath;

        public ShellViewModel(IEventAggregator events, IApplicationCommands commands, IApplicationSettings settings, 
            IRegionManager regionManager)
        {
            _events = events;
            _regionManager = regionManager;

            Title = "iXConfigurator";

            _isAdminMode = settings.IsAdminMode;

            _events.GetEvent<TemplateAddedEvent>().Subscribe(OnTemplateAdded);
            _events.GetEvent<SelectTemplateEvent>().Subscribe(OnSelectTemplate);
            _events.GetEvent<ModulesInitializedEvent>().Subscribe(OnModulesInitialized);

            OpenEditorCommand = new DelegateCommand(OnOpenEditor, () => true);
            CloseEditorCommand = new DelegateCommand(OnCloseEditor, () => true);

            commands.OpenEditorCommand.RegisterCommand(OpenEditorCommand);
            commands.CloseEditorCommand.RegisterCommand(CloseEditorCommand);

            _libraryPath = settings.LocalLibraryPath;
        }

        public ICommand OpenEditorCommand { get; }

        public ICommand CloseEditorCommand { get; }

        public string SelectedTemplate
        {
            get { return _selectedTemplate; }
            set { SetProperty(ref _selectedTemplate, value, () => OnSelectedTemplateChanged(value)); }
        }

        public bool IsAdminMode
        {
            get { return _isAdminMode; }
            set { SetProperty(ref _isAdminMode, value); }
        }

        public string LibraryPath
        {
            get { return _libraryPath; }
            set { SetProperty(ref _libraryPath, value); }
        }

        public bool IsEditorOpen
        {
            get { return _isEditorOpen; }
            set { SetProperty(ref _isEditorOpen, value); }
        }

        public ObservableCollection<string> Templates { get; } = new ObservableCollection<string>();

        private async void OnModulesInitialized()
        {
            _regionManager.RequestNavigate(RegionNames.MainRegion, ViewNames.EditorView);
            _regionManager.RequestNavigate(RegionNames.MainRegion, ViewNames.ConfiguratorView);

            _regionManager.RequestNavigate(RegionNames.FlyoutsRegion, ViewNames.DebugView);
            _regionManager.RequestNavigate(RegionNames.FlyoutsRegion, ViewNames.SnippetsView);
            
            _regionManager.RequestNavigate(RegionNames.DebugRegion, ViewNames.VariablesView);
            _regionManager.RequestNavigate(RegionNames.DebugRegion, ViewNames.OutputView);

            var templateManager = ServiceLocator.Current.GetInstance<ITemplateManager>();
            await templateManager.InitializeAsync();

            SelectedTemplate = Templates.FirstOrDefault();
        }

        private void OnSelectTemplate(string key)
        {
            SelectedTemplate = Templates.First(t => t == key);
        }

        private void OnTemplateAdded(string key)
        {
            Templates.Add(key);
        }

        private void OnSelectedTemplateChanged(string key)
        {
            _events.GetEvent<TemplateSelectedEvent>().Publish(key);
        }


        private void OnOpenEditor()
        {
            _regionManager.RequestNavigate(RegionNames.MainRegion, ViewNames.EditorView, result =>
            {
                IsEditorOpen = result.Result.GetValueOrDefault();
            });
        }

        private void OnCloseEditor()
        {
            _regionManager.RequestNavigate(RegionNames.MainRegion, ViewNames.ConfiguratorView, result =>
            {
                IsEditorOpen = !result.Result.GetValueOrDefault();
            });

        }
    }
}
