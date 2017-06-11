using Microsoft.Practices.Unity;
using Prism.Logging;
using Prism.Modularity;
using Prism.Regions;

namespace Wayne.Payment.Products.iXConfigurator.Infrastructure.Modularity
{
    public abstract class ModuleBase : IModule
    {
        protected ModuleBase(IUnityContainer container, IRegionManager regionManager, ILoggerFacade logger)
        {
            Container = container;
            RegionManager = regionManager;
            Logger = logger;
        }

        protected ILoggerFacade Logger { get; private set; }

        protected IUnityContainer Container { get; private set; }

        protected IRegionManager RegionManager { get; private set; }

        public abstract void Initialize();
    }
}
