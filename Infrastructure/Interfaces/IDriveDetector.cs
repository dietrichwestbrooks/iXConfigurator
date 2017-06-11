using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Wayne.Payment.Products.iXConfigurator.Infrastructure.Interfaces
{
    public interface IDriveDetector
    {
        /// <summary>
        /// Scans for all attached removable devices
        /// </summary>
        Task<IEnumerable<InstalledMountedDriveInfo>>  ScanAttachedDrivesAsync();

        /// <summary>
        /// Registers a window to receive notifications when USB devices are plugged or unplugged.
        /// </summary>
        /// <param name="hWnd">Handle to the window receiving notifications.</param>
        bool RegisterUsbDeviceNotification(IntPtr hWnd);

        /// <summary>
        /// Unregisters the window for USB device notifications
        /// </summary>
        bool UnregisterUsbDeviceNotification();
    }
}
