using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using Prism.Events;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Events;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Interfaces;
using Wayne.Payment.Products.iXConfigurator.Infrastructure.Win32;

namespace Wayne.Payment.Products.iXConfigurator.Infrastructure.Services
{
    public class DriveDetector : IDriveDetector
    {
        private IApplicationLogger _logger;
        private IEventAggregator _events;

        private IntPtr _notificationHandle = IntPtr.Zero;

        private const int DbtDevtypDeviceInterface = 5;
        private static readonly Guid GuidDevinterfaceUsbDevice = new Guid("A5DCBF10-6530-11D2-901F-00C04FB951ED"); // USB devices

        public DriveDetector(IApplicationLogger logger, IEventAggregator events)
        {
            _logger = logger;
            _events = events;
        }

        /// <summary>
        /// Scans for all attached removable devices
        /// </summary>
        public Task<IEnumerable<InstalledMountedDriveInfo>> ScanAttachedDrivesAsync()
        {
            var installedMountedDrives = new SortedList<string, InstalledMountedDriveInfo>();

            try
            {
                var environmentDrives = new Hashtable();
                var mountedDrives = new Hashtable();

                foreach (var drive in Environment.GetLogicalDrives())
                {
                    environmentDrives.Add(drive, new DriveInfo(drive));
                }

                var mountedDevices = Registry.LocalMachine.OpenSubKey("SYSTEM\\MountedDevices");

                if (mountedDevices?.ValueCount > 0)
                {
                    var mountedDeviceNames = mountedDevices.GetValueNames()
                        .Where(name => name.StartsWith("\\DosDevices\\"));

                    foreach (var mountedDeviceName in mountedDeviceNames)
                    {
                        var driveLetter = new StringBuilder(mountedDeviceName.Substring(12));
                        driveLetter.Append("\\");

                        if (!environmentDrives.ContainsKey(driveLetter.ToString()))
                            continue;

                        var thisDriveInfo = (DriveInfo)environmentDrives[driveLetter.ToString()];

                        if (thisDriveInfo.DriveType != DriveType.Removable)
                            continue;

                        var bytes = (byte[])mountedDevices.GetValue(mountedDeviceName);
                        var cleaned = new byte[bytes.Length - 1];

                        var pointer = 0;
                        var firstSlashFound = false;
                        var secondSlashFound = false;
                        var questionMarkFound = false;

                        foreach (var character in (byte[])mountedDevices.GetValue(mountedDeviceName))
                        {
                            if (character == 0)
                                continue;

                            if (firstSlashFound)
                            {
                                if (questionMarkFound)
                                {
                                    if (secondSlashFound)
                                    {
                                        cleaned[pointer] = character;
                                        pointer++;
                                    }
                                    else
                                    {
                                        if (character == 92)
                                        {
                                            secondSlashFound = true;
                                        }
                                    }
                                }
                                else
                                {
                                    if (character == 63)
                                    {
                                        questionMarkFound = true;
                                    }
                                }
                            }
                            else
                            {
                                if (character == 92)
                                {
                                    firstSlashFound = true;
                                }
                            }
                        }

                        var finished = new byte[pointer];
                        for (var i = 0; i < pointer; i++)
                        {
                            finished[i] = cleaned[i];
                        }

                        mountedDrives.Add(driveLetter,
                            new MountedDriveInfo(thisDriveInfo, Encoding.Default.GetString(finished).ToUpper()));
                    }
                }

                mountedDevices?.Close();

                var classGuid = new Guid(UnsafeNativeMethods.GUID_DEVINTERFACE_VOLUME);
                var deviceInfoSet = UnsafeNativeMethods.SetupDiGetClassDevs(ref classGuid, 0, IntPtr.Zero,
                    UnsafeNativeMethods.DIGCF_DEVICEINTERFACE | UnsafeNativeMethods.DIGCF_PRESENT);

                if (deviceInfoSet.ToInt32() == UnsafeNativeMethods.INVALID_HANDLE_VALUE)
                    return Task.FromResult(Enumerable.Empty<InstalledMountedDriveInfo>());

                var index = 0;

                while (true)
                {
                    var size = 0;

                    var interfaceData = new UnsafeNativeMethods.SP_DEVICE_INTERFACE_DATA();
                    var devData = new UnsafeNativeMethods.SP_DEVINFO_DATA();
                    var detailData = new UnsafeNativeMethods.SP_DEVICE_INTERFACE_DETAIL_DATA();

                    if (
                        !UnsafeNativeMethods.SetupDiEnumDeviceInterfaces(deviceInfoSet, null, ref classGuid, index,
                            interfaceData))
                    {
                        break;
                    }

                    UnsafeNativeMethods.SetupDiGetDeviceInterfaceDetail(deviceInfoSet, interfaceData, IntPtr.Zero, 0,
                        ref size, devData);

                    var buffer = Marshal.AllocHGlobal(size);

                    // for 64 bit system, the size needs to be 8, for 32 bit system, the size needs to be 5
                    if (IntPtr.Size == 8)
                    {
                        detailData.cbSize = 8;
                    }
                    else
                    {
                        detailData.cbSize = Marshal.SizeOf(typeof(UnsafeNativeMethods.SP_DEVICE_INTERFACE_DETAIL_DATA));
                    }

                    Marshal.StructureToPtr(detailData, buffer, false);

                    if (!UnsafeNativeMethods.SetupDiGetDeviceInterfaceDetail(deviceInfoSet, interfaceData,
                        buffer, size, ref size, devData))
                    {
                        Marshal.FreeHGlobal(buffer);
                    }

                    var pDevicePath = (IntPtr)((int)buffer + Marshal.SizeOf(typeof(int)));
                    var devicePath = Marshal.PtrToStringAuto(pDevicePath);

                    if (devicePath != null)
                    {
                        devicePath = devicePath.ToUpper();
                        Marshal.FreeHGlobal(buffer);

                        foreach (MountedDriveInfo mountedDrive in mountedDrives.Values)
                        {
                            if (!devicePath.Contains(mountedDrive.RegistryInfo))
                                continue;

                            var thisMountedDrive = new InstalledMountedDriveInfo(mountedDrive.DriveInfo,
                                devData.devInst, (UnsafeNativeMethods.DeviceCapabilities)
                                    UnsafeNativeMethods.GetProperty(deviceInfoSet, devData,
                                        UnsafeNativeMethods.SPDRP_CAPABILITIES, 0),
                                UnsafeNativeMethods.GetProperty(deviceInfoSet, devData,
                                    UnsafeNativeMethods.SPDRP_PHYSICAL_DEVICE_OBJECT_NAME, "bad name"));

                            try
                            {
                                // Check attempts to add duplicate drives when the SetupDiEnumDeviceInterfaces call does not break
                                // on first call for a particular drive (multiple device interfaces).

                                if (installedMountedDrives.ContainsKey(thisMountedDrive.Description) == false)
                                {
                                    installedMountedDrives.Add(thisMountedDrive.Description, thisMountedDrive);
                                }
                            }
                            catch (Exception)
                            {
                                // Ignore unspecified exceptions and continue building mounted drives
                            }
                        }
                    }

                    index++;
                }
            }
            catch (Exception ex)
            {
                _logger.Log(ex);
            }

            _events.GetEvent<ScanDrivesCompletedEvent>().Publish(installedMountedDrives.Values.AsEnumerable());

            return Task.FromResult(installedMountedDrives.Values.AsEnumerable());
        }

        /// <summary>
        /// Registers a window to receive notifications when USB devices are plugged or unplugged.
        /// </summary>
        /// <param name="hWnd">Handle to the window receiving notifications.</param>
        public bool RegisterUsbDeviceNotification(IntPtr hWnd)
        {
            if (_notificationHandle != IntPtr.Zero)
            {
                return false;
            }

            try
            {
                var dbi = new UnsafeNativeMethods.DevBroadcastDeviceInterface
                {
                    DeviceType = DbtDevtypDeviceInterface,
                    Reserved = 0,
                    ClassGuid = GuidDevinterfaceUsbDevice,
                    Name = 0
                };

                dbi.Size = Marshal.SizeOf(dbi);
                var buffer = Marshal.AllocHGlobal(dbi.Size);
                Marshal.StructureToPtr(dbi, buffer, true);

                _notificationHandle = UnsafeNativeMethods.RegisterDeviceNotification(hWnd, buffer, 0);
            }
            catch (Exception ex)
            {
                _logger.Log(ex);
            }

            return _notificationHandle != IntPtr.Zero;
        }

        /// <summary>
        /// Unregisters the window for USB device notifications
        /// </summary>
        public bool UnregisterUsbDeviceNotification()
        {
            if (_notificationHandle == IntPtr.Zero)
            {
                return true;
            }

            try
            {
                UnsafeNativeMethods.UnregisterDeviceNotification(_notificationHandle);
            }
            catch (Exception ex)
            {
                _logger.Log(ex);
                return false;
            }

            return true;
        }

        class MountedDriveInfo
        {
            public DriveInfo DriveInfo { get; }

            public string RegistryInfo { get; }

            public MountedDriveInfo(DriveInfo newDriveInfo, string newRegistryInfo)
            {
                DriveInfo = newDriveInfo;
                RegistryInfo = newRegistryInfo;
            }
        }
    }
}
