using System.IO;
using System.Text;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Win32;

namespace Wayne.Payment.Products.iXConfigurator.Infrastructure.Interfaces
{
    /// <summary>
    /// Public class of exposed drive information
    /// </summary>
    public class InstalledMountedDriveInfo
    {
        public const string NoMedia = "<NO MEDIA INSERTED>";

        public string DeviceCapabilities { get; }

        public string Description { get; private set; }

        public int DeviceInstanceHandle { get; }

        public string DeviceName { get; }

        public string DriveFormat { get; private set; }

        public DriveInfo DriveInfo => new DriveInfo(DriveName);

        public string DriveName { get; }

        public bool MediaInserted { get; private set; }

        /// <summary>
        /// Instantion object of this type
        /// </summary>
        /// <param name="thisDriveInfo">DriveInfo object</param>
        /// <param name="instanceHandle"></param>
        /// <param name="thisDeviceCapabilities">Device Capabilities</param>
        /// <param name="thisDeviceName">Device Name</param>
        public InstalledMountedDriveInfo(DriveInfo thisDriveInfo, int instanceHandle,
            UnsafeNativeMethods.DeviceCapabilities thisDeviceCapabilities, string thisDeviceName)
        {
            DriveName = thisDriveInfo.Name;
            DeviceInstanceHandle = instanceHandle;
            DeviceCapabilities = thisDeviceCapabilities.ToString();
            DeviceName = thisDeviceName;

            Initialize();
        }

        /// <summary>
        /// Sets the description and media inserted flags
        /// </summary>
        public bool Initialize()
        {
            var driveDescription = new StringBuilder(DriveInfo.ToString());

            var mediaChanged = false;

            try
            {
                var thisDriveInfo = new DriveInfo(DriveName);

                // ReSharper disable once PossibleLossOfFraction
                float capacity = (thisDriveInfo.TotalSize / 1024) / 1024;
                DriveFormat = thisDriveInfo.DriveFormat;

                if (DriveInfo.VolumeLabel != string.Empty)
                {
                    driveDescription.Append(" (");
                    driveDescription.Append(thisDriveInfo.VolumeLabel);
                    driveDescription.Append(")");
                }

                driveDescription.Append(" [Capacity: ");
                driveDescription.Append(capacity.ToString("00.0"));
                driveDescription.Append(" MB] Type: ");
                driveDescription.Append(thisDriveInfo.DriveFormat);

                Description = driveDescription.ToString();

                if (MediaInserted != true)
                {
                    MediaInserted = true;
                    mediaChanged = true;
                }
            }
            catch
            {
                driveDescription.Append(NoMedia);
                Description = driveDescription.ToString();
                DriveFormat = NoMedia;

                if (MediaInserted)
                {
                    MediaInserted = false;
                    mediaChanged = true;
                }
            }

            return mediaChanged;
        }
    }
}
