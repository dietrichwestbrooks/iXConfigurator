using Wayne.Payment.Products.iXConfigurator.Infrastructure.Interfaces;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Mvvm;

namespace Wayne.Payment.Products.iXConfigurator.Modules.Core.Views
{
    public class DriveViewModel : ViewModelBase
    {
        private InstalledMountedDriveInfo _driveInfo;
        private string _name;
        private string _description;

        public DriveViewModel(InstalledMountedDriveInfo driveInfo)
        {
            _driveInfo = driveInfo;

            _name = driveInfo.DriveName;
            _description = driveInfo.Description;
        }

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }

        public bool IsFatFormat => _driveInfo.DriveFormat == "FAT" || _driveInfo.DriveFormat == "FAT32";
    }
}
