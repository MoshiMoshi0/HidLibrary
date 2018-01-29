using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace HidLibrary
{
    internal class DeviceChangeNotifier : Form
    {
        public delegate void DeviceNotifyDelegate(IEnumerable<HidDevices.DeviceInfo> devices);
        public static event DeviceNotifyDelegate DeviceNotify;

        private static readonly DeviceChangeNotifier singletonInstance;
        private static object startLock = new object();
        private static List<HidDevices.DeviceInfo> cachedDeviceList;

        public static IEnumerable<HidDevices.DeviceInfo> CachedDevices => cachedDeviceList;

        static DeviceChangeNotifier()
        {
            //No start/stop mechanism because it's easier ;-)
            //(and not needed atm)
            singletonInstance = new DeviceChangeNotifier();
            var t = new Thread(() =>
            {
                Application.Run(singletonInstance);
            });
            t.SetApartmentState(ApartmentState.STA);
            t.IsBackground = true;
            t.Start();
        }

        protected override void SetVisibleCore(bool value)
        {
            // Prevent window getting visible
            CreateHandle();
            value = false;
            base.SetVisibleCore(value);
        }

        protected override void WndProc(ref Message m)
        {
            // Trap WM_DEVICECHANGE
            if (m.Msg == NativeMethods.WM_DEVICECHANGE)
            {
                var newList = GetCurrentDeviceList();
                cachedDeviceList = newList;
                DeviceNotify?.Invoke(newList);
            }
            base.WndProc(ref m);
        }

        private static List<HidDevices.DeviceInfo> GetCurrentDeviceList()
        {
            return HidDevices.EnumerateDevices().ToList();
        }
    }
}
