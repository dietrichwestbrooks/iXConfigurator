// ReSharper disable InconsistentNaming
using System;
using System.Runtime.InteropServices;

namespace Wayne.Payment.Products.iXConfigurator.Infrastructure.Win32
{
    public static class UnsafeNativeMethods
    {
        // Win32 Constants

        public const int DIGCF_DEVICEINTERFACE = (0x00000010);
        public const int DIGCF_PRESENT = (0x00000002);
        public const int INVALID_HANDLE_VALUE = -1;
        public const string GUID_DEVINTERFACE_VOLUME = "53f5630d-b6bf-11d0-94f2-00a0c91efb8b";
        public const int SPDRP_CAPABILITIES = 0x0000000F;
        public const int SPDRP_PHYSICAL_DEVICE_OBJECT_NAME = 0x0000000E;

        /// <summary>
        /// Contains constants for determining devices capabilities.
        /// This enumeration has a FlagsAttribute attribute that allows a bitwise combination of its member values.
        /// </summary>
        [Flags]
        public enum DeviceCapabilities      // matches cfmgr32.h CM_DEVCAP_* definitions
        {
            Unknown = 0x00000000,
            LockSupported = 0x00000001,
            EjectSupported = 0x00000002,
            Removable = 0x00000004,
            DockDevice = 0x00000008,
            UniqueId = 0x00000010,
            SilentInstall = 0x00000020,
            RawDeviceOk = 0x00000040,
            SurpriseRemovalOk = 0x00000080,
            HardwareDisabled = 0x00000100,
            NonDynamic = 0x00000200,
        }

        #region /// SETUPAPI Functions & Declarations

        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        public struct SP_DEVICE_INTERFACE_DETAIL_DATA
        {
            public int cbSize;
            public short devicePath;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class SP_DEVINFO_DATA
        {
            public int cbSize = Marshal.SizeOf(typeof(SP_DEVINFO_DATA));
            public Guid classGuid = Guid.Empty;               // temp
            public int devInst = 0;                           // dummy
            public UIntPtr reserved = UIntPtr.Zero;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class SP_DEVICE_INTERFACE_DATA
        {
            public int cbSize = Marshal.SizeOf(typeof(SP_DEVICE_INTERFACE_DATA));
            public Guid interfaceClassGuid = Guid.Empty;      // temp
            public int flags = 0;
            public UIntPtr reserved = UIntPtr.Zero;
        }

        [DllImport("setupapi.dll")]
        public static extern IntPtr SetupDiGetClassDevs(
                ref Guid classGuid,
                int enumerator,
                IntPtr hwndParent,
                int flags);

        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetupDiGetDeviceRegistryProperty(
                IntPtr deviceInfoSet,
                SP_DEVINFO_DATA deviceInfoData,
                int property,
                out int propertyRegDataType,
                IntPtr propertyBuffer,
                int propertyBufferSize,
                out int requiredSize);

        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool SetupDiGetDeviceInterfaceDetail(
                IntPtr deviceInfoSet,
                SP_DEVICE_INTERFACE_DATA deviceInterfaceData,
                IntPtr deviceInterfaceDetailData,
                int deviceInterfaceDetailDataSize,
                ref int requiredSize,
                SP_DEVINFO_DATA deviceInfoData);

        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool SetupDiEnumDeviceInterfaces(
                IntPtr deviceInfoSet,
                SP_DEVINFO_DATA deviceInfoData,
                ref Guid interfaceClassGuid,
                int memberIndex,
                SP_DEVICE_INTERFACE_DATA deviceInterfaceData);

        #endregion

        #region /// Overloaded functions to retrieve device registry property
        /// <summary>
        /// Returns a string property
        /// </summary>
        /// <param name="deviceInfoSet"></param>
        /// <param name="devData"></param>
        /// <param name="property"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string GetProperty(IntPtr deviceInfoSet, SP_DEVINFO_DATA devData, int property, string defaultValue)
        {
            if (devData == null) { return defaultValue; }

            int propertyRegDataType;
            int requiredSize;
            var propertyBufferSize = 1024;

            var propertyBuffer = Marshal.AllocHGlobal(propertyBufferSize);
            if (!SetupDiGetDeviceRegistryProperty(
                    deviceInfoSet,
                    devData,
                    property,
                    out propertyRegDataType,
                    propertyBuffer,
                    propertyBufferSize,
                    out requiredSize))
            {
                Marshal.FreeHGlobal(propertyBuffer);
                return defaultValue;
            }

            var value = Marshal.PtrToStringAuto(propertyBuffer);
            Marshal.FreeHGlobal(propertyBuffer);
            return value;
        }

        /// <summary>
        /// Returns an integer property
        /// </summary>
        /// <param name="deviceInfoSet"></param>
        /// <param name="devData"></param>
        /// <param name="property"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int GetProperty(IntPtr deviceInfoSet, SP_DEVINFO_DATA devData, int property, int defaultValue)
        {
            if (devData == null) { return defaultValue; }

            int propertyRegDataType;
            int requiredSize;
            var propertyBufferSize = Marshal.SizeOf(typeof(int));

            var propertyBuffer = Marshal.AllocHGlobal(propertyBufferSize);
            if (!SetupDiGetDeviceRegistryProperty(
                    deviceInfoSet,
                    devData,
                    property,
                    out propertyRegDataType,
                    propertyBuffer,
                    propertyBufferSize,
                    out requiredSize))
            {
                Marshal.FreeHGlobal(propertyBuffer);
                return defaultValue;
            }

            var value = (int)Marshal.PtrToStructure(propertyBuffer, typeof(int));
            Marshal.FreeHGlobal(propertyBuffer);
            return value;
        }

        /// <summary>
        /// Returns a GUID property
        /// </summary>
        /// <param name="deviceInfoSet"></param>
        /// <param name="devData"></param>
        /// <param name="property"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static Guid GetProperty(IntPtr deviceInfoSet, SP_DEVINFO_DATA devData, int property, Guid defaultValue)
        {
            if (devData == null) { return defaultValue; }

            int propertyRegDataType;
            int requiredSize;
            var propertyBufferSize = Marshal.SizeOf(typeof(Guid));

            var propertyBuffer = Marshal.AllocHGlobal(propertyBufferSize);
            if (!SetupDiGetDeviceRegistryProperty(
                    deviceInfoSet,
                    devData,
                    property,
                    out propertyRegDataType,
                    propertyBuffer,
                    propertyBufferSize,
                    out requiredSize))
            {
                Marshal.FreeHGlobal(propertyBuffer);
                return defaultValue;
            }

            var value = (Guid)Marshal.PtrToStructure(propertyBuffer, typeof(Guid));
            Marshal.FreeHGlobal(propertyBuffer);
            return value;
        }
        #endregion

        #region USB Device Notifications

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr RegisterDeviceNotification(IntPtr recipient, IntPtr notificationFilter, int flags);

        [DllImport("user32.dll")]
        public static extern bool UnregisterDeviceNotification(IntPtr handle);

        [StructLayout(LayoutKind.Sequential)]
        public struct DevBroadcastDeviceInterface
        {
            internal int Size;
            internal int DeviceType;
            internal int Reserved;
            internal Guid ClassGuid;
            internal short Name;
        }

        #endregion
    }
}
