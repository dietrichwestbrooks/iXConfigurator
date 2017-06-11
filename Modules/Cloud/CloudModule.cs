using Microsoft.Practices.Unity;
using Prism.Logging;
using Prism.Modularity;
using Prism.Regions;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Interfaces;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Modularity;
using Wayne.Payment.Products.iXConfigurator.Modules.Cloud.Services;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Cloud
{
    [Module(ModuleName = "Cloud", OnDemand = false)]
    [ModuleDependency("Core")]
    public class CloudModule : ModuleBase
    {
        public CloudModule(IUnityContainer container, IRegionManager regionManager, ILoggerFacade logger) 
            : base(container, regionManager, logger)
        {
        }

        public override void Initialize()
        {
            // Register types
            Container.RegisterType<ILibraryService, CloudLibraryService>(new HierarchicalLifetimeManager());
        }
    }
}
