using System;
using System.Threading;

namespace HidLibrary
{
    internal class HidDeviceEventMonitor
    {
        private readonly HidDevice _device;

        public HidDeviceEventMonitor(HidDevice device)
        {
            _device = device;
        }

        public void Init()
        {
            var eventMonitor = new Action(DeviceEventMonitor);
            eventMonitor.BeginInvoke(DisposeDeviceEventMonitor, eventMonitor);
        }

        private void DeviceEventMonitor()
        {
            _device.RefreshConnectedFlag();
            Thread.Sleep(500);
            if (_device.MonitorDeviceEvents) Init();
        }

        private static void DisposeDeviceEventMonitor(IAsyncResult ar)
        {
            ((Action)ar.AsyncState).EndInvoke(ar);
        }
    }
}
