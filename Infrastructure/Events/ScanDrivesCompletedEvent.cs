using System.Collections.Generic;
using Prism.Events;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Interfaces;

namespace Wayne.Payment.Products.iXConfigurator.Infrastructure.Events
{
    public class ScanDrivesCompletedEvent : PubSubEvent<IEnumerable<InstalledMountedDriveInfo>>
    {
         
    }
}
