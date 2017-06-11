using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows;
using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Logging;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Unity;
using Wayne.Payment.Products.iXConfigurator.Template;
using Wayne.Payment.Products.iXConfigurator.Desktop.Logging;
using Wayne.Payment.Products.iXConfigurator.Desktop.Views;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Commands;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Constants;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Events;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Interfaces;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Mvvm;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Services;

namespace Wayne.Payment.Products.iXConfigurator.Desktop
{
    public class WindowsBootstrapper : UnityBootstrapper
    {
        public WindowsBootstrapper()
        {
            //AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
            //AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += OnAssemblyResolve;
        }

        protected override DependencyObject CreateShell()
        {
            Container.RegisterInstance(typeof(Window), ViewNames.MainWindow, Container.Resolve<Shell>(), new ContainerControlledLifetimeManager());
            return Container.Resolve<Window>(ViewNames.MainWindow);
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();

            // Needs to be instantiated after shell window is created
            Container.RegisterInstance<IMessageDisplayService>(Container.Resolve<MessageDisplayService>(), new HierarchicalLifetimeManager());

            RegisterViews();

            Application.Current.MainWindow.Show();
        }

        private void RegisterViews()
        {
            var regionManager = Container.Resolve<IRegionManager>();

            // Add right windows commands
            regionManager.RegisterViewWithRegion(RegionNames.RightWindowCommandsRegion, typeof(RightTitlebarCommands));
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            Container.RegisterInstance(Logger as IApplicationLogger, new HierarchicalLifetimeManager());

            Container.RegisterType<IApplicationCommands, ApplicationCommandsProxy>(new HierarchicalLifetimeManager());

            Container.RegisterInstance<IFlyoutService>(Container.Resolve<FlyoutService>(), new HierarchicalLifetimeManager());

            Container.RegisterInstance<IApplicationService>(Container.Resolve<ApplicationService>(), new HierarchicalLifetimeManager());

            Container.RegisterType<IApplicationSettings, ApplicationSettings>(new HierarchicalLifetimeManager());

            Container.RegisterType<IFileService, FileService>(new HierarchicalLifetimeManager());

            Container.RegisterType<ITextMarkerService, TextMarkerService>();

            Container.RegisterType<ShellViewModel>(new HierarchicalLifetimeManager());

            Container.RegisterType<IDriveDetector, DriveDetector>(new HierarchicalLifetimeManager());

            Container.RegisterType<RightTitlebarCommands>(new InjectionConstructor(typeof(ShellViewModel)));

            Container.RegisterType<ITemplateSerializer, TemplateSerializer>(new HierarchicalLifetimeManager());

            Container.RegisterType<ITemplateManager, TemplateManager>(new HierarchicalLifetimeManager());

            Container.RegisterType<ILibraryService, LocalLibraryService>(new HierarchicalLifetimeManager());

            Container.RegisterType<IVariableStore, VariableStore>(new HierarchicalLifetimeManager());
        }

        protected override IModuleCatalog CreateModuleCatalog()
        {
            return new DirectoryModuleCatalog { ModulePath = @".\Modules" };
        }

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();

            ViewModelLocationProvider.SetDefaultViewModelFactory(
                (view, type) => Container.Resolve(type, new DependencyOverride(typeof(IView), view)));

            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver(viewType =>
            {
                var viewName = viewType.FullName;
                var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
                var suffix = viewName.EndsWith("View") ? "Model" : "ViewModel";
                var viewModelName = string.Format(CultureInfo.InvariantCulture, "{0}{1}, {2}", viewName, suffix,
                    viewAssemblyName);
                return Type.GetType(viewModelName);
            });
        }

        protected override void InitializeModules()
        {
            base.InitializeModules();

            var events = Container.Resolve<IEventAggregator>();
            events.GetEvent<ModulesInitializedEvent>().Publish();
        }

        protected override ILoggerFacade CreateLogger()
        {
            return new Log4NetLogger();
        }

        private static Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            var folderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (folderPath == null)
                return null;

            var assemblyPath = Path.Combine(folderPath, new AssemblyName(args.Name).Name + ".dll");

            if (!File.Exists(assemblyPath))
                return null;

            var assembly = Assembly.LoadFrom(assemblyPath);

            return assembly;
        }
    }
}
