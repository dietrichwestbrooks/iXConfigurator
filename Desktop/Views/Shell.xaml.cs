using System;
using System.Windows;
using System.Windows.Interop;
using Prism.Regions;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Constants;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Interfaces;

namespace Wayne.Payment.Products.iXConfigurator.Desktop.Views
{
    /// <summary>
    /// Interaction logic for Shell.xaml
    /// </summary>
    public partial class Shell
    {
        private IDriveDetector _driveDetector;

        public Shell(IRegionManager regionManager, IDriveDetector driveDetector)
        {
            _driveDetector = driveDetector;

            InitializeComponent();

            if (regionManager != null)
            {
                SetRegionManager(regionManager, LeftWindowCommandsRegion, RegionNames.LeftWindowCommandsRegion);
                SetRegionManager(regionManager, RightWindowCommandsRegion, RegionNames.RightWindowCommandsRegion);
                SetRegionManager(regionManager, FlyoutsRegion, RegionNames.FlyoutsRegion);
            }

            Loaded += (sender, args) =>
            {
                var hWnd = new WindowInteropHelper(this).Handle;

                _driveDetector.RegisterUsbDeviceNotification(hWnd);

                var source = HwndSource.FromHwnd(hWnd);
                source?.AddHook(WindowsMessageHandler);
            };

            Unloaded += (sender, args) =>
            {
                _driveDetector.UnregisterUsbDeviceNotification();
            };
        }

        private void SetRegionManager(IRegionManager regionManager, DependencyObject regionTarget, string regionName)
        {
            RegionManager.SetRegionName(regionTarget, regionName);
            RegionManager.SetRegionManager(regionTarget, regionManager);
        }

        private IntPtr WindowsMessageHandler(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case WindowsMessages.WM_DEVICECHANGE:
                {
                        switch ((int)wParam)
                        {
                            case WindowsMessages.DBT_DEVICEARRIVAL:
                            case WindowsMessages.DBT_DEVICEREMOVECOMPLETE:
                            {
                                _driveDetector.ScanAttachedDrivesAsync();
                                break;
                            }
                        }
                        break;
                }
            }

            return IntPtr.Zero;
        }
    }
}
