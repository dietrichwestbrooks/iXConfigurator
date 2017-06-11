using System.Linq;
using System.Windows.Input;
using MahApps.Metro.Controls;
using Prism.Commands;
using Prism.Regions;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Commands;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Constants;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Interfaces;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Mvvm;

namespace Wayne.Payment.Products.iXConfigurator.Infrastructure.Services
{
    public class FlyoutService : IFlyoutService
    {
        IRegionManager _regionManager;

        public ICommand ShowFlyoutCommand { get; }

        public FlyoutService(IRegionManager regionManager, IApplicationCommands applicationCommands)
        {
            _regionManager = regionManager;

            ShowFlyoutCommand = new DelegateCommand<string>(ShowFlyout, CanShowFlyout);
            applicationCommands.ShowFlyoutCommand.RegisterCommand(ShowFlyoutCommand);
        }

        public void ShowFlyout(string flyoutName)
        {
            var region = _regionManager.Regions[RegionNames.FlyoutsRegion];

            var flyout = region?.Views.FirstOrDefault(v => v is IFlyoutView && ((IFlyoutView)v).FlyoutName.Equals(flyoutName)) as Flyout;

            if (flyout != null)
            {
                flyout.IsOpen = !flyout.IsOpen;
            }
        }

        public bool CanShowFlyout(string flyoutName)
        {
            return true;
        }
    }
}
