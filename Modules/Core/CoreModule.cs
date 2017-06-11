using System;
using Microsoft.Practices.Unity;
using Prism.Logging;
using Prism.Modularity;
using Prism.Regions;
using Prism.Unity;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Constants;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Interfaces;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Modularity;
using Wayne.Payment.Products.iXConfigurator.Modules.Core.Services;
using Wayne.Payment.Products.iXConfigurator.Modules.Core.ElementViewModels;
using Wayne.Payment.Products.iXConfigurator.Modules.Core.Commands;
using Wayne.Payment.Products.iXConfigurator.Modules.Core.Views;
using Wayne.Payment.Products.iXConfigurator.Modules.Core.EditorViewModels;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core
{
    [Module(ModuleName = "Core", OnDemand = false)]
    public class CoreModule : ModuleBase
    {
        public CoreModule(IUnityContainer container, IRegionManager regionManager, ILoggerFacade logger)
            : base(container, regionManager, logger)
        {
        }

        public override void Initialize()
        {
            try
            {
                // Register types
                Container.RegisterType<IConfiguratorService, ConfiguratorService>(new HierarchicalLifetimeManager());
                Container.RegisterType<IElementViewModelFactory, ElementViewModelFactory>(new HierarchicalLifetimeManager());
                Container.RegisterType<ITaskCommandFactory, TaskCommandFactory>(new HierarchicalLifetimeManager());
                Container.RegisterType<IEditorViewModelFactory, EditorViewModelFactory>(new HierarchicalLifetimeManager());

                // Register view model types
                Container.RegisterType<ConfiguratorViewModel>(new HierarchicalLifetimeManager());
                Container.RegisterType<EditorViewModel>(new HierarchicalLifetimeManager());

                // Register views types
                Container.RegisterTypeForNavigation<EditorView>(ViewNames.EditorView);
                Container.RegisterTypeForNavigation<ConfiguratorView>(ViewNames.ConfiguratorView);
                Container.RegisterTypeForNavigation<DebugFlyoutView>(ViewNames.DebugView);
                Container.RegisterTypeForNavigation<SnippetsFlyoutView>(ViewNames.SnippetsView);
                Container.RegisterTypeForNavigation<VariablesView>(ViewNames.VariablesView);
                Container.RegisterTypeForNavigation<OutputView>(ViewNames.OutputView);
                Container.RegisterTypeForNavigation<ExpressionBuilderView>(PopupNames.ExpressionBuilderPopup);
                Container.RegisterTypeForNavigation<ExpandableBuilderView>(PopupNames.ExpandableBuilderPopup);
                Container.RegisterTypeForNavigation<DataTableView>(PopupNames.DataTablePopup);
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message, Category.Exception, Priority.High);
            }
        }
    }
}
