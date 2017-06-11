using Microsoft.Practices.Unity;
using Prism.Logging;
using Prism.Modularity;
using Prism.Regions;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Modularity;

namespace Wayne.Payment.Products.iXConfigurator.Modules.TeamCity
{
    [Module(ModuleName = "TeamCity", OnDemand = true)]
    [ModuleDependency("Core")]
    public class TeamCityModule : ModuleBase
    {
        public TeamCityModule(IUnityContainer container, IRegionManager regionManager, ILoggerFacade logger) 
            : base(container, regionManager, logger)
        {
        }

        public override void Initialize()
        {
        }
    }
}
