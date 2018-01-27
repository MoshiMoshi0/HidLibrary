using System.Threading.Tasks;

namespace HidLibrary
{
    public class HidFastReadDevice : HidDevice
    {
        internal HidFastReadDevice(string devicePath, string description = null)
            : base(devicePath, description) { }

        // FastRead assumes that the device is connected,
        // which could cause stability issues if hardware is
        // disconnected during a read
        public HidDeviceData FastRead()
        {
            return FastRead(0);
        }

        public HidDeviceData FastRead(int timeout)
        {
            try
            {
                return ReadData(timeout);
            }
            catch
            {
                return new HidDeviceData(HidDeviceData.ReadStatus.ReadError);
            }
        }

        public void FastRead(ReadCallback callback)
        {
            FastRead(callback, 0);
        }

        public void FastRead(ReadCallback callback, int timeout)
        {
            var readDelegate = new ReadDelegate(FastRead);
            var asyncState = new HidAsyncState(readDelegate, callback);
            readDelegate.BeginInvoke(timeout, EndRead, asyncState);
        }


        public HidReport FastReadReport()
        {
            return FastReadReport(0);
        }

        public HidReport FastReadReport(int timeout)
        {
            return new HidReport(Capabilities.InputReportByteLength, FastRead(timeout));
        }

        public void FastReadReport(ReadReportCallback callback)
        {
            FastReadReport(callback, 0);
        }

        public void FastReadReport(ReadReportCallback callback, int timeout)
        {
            var readReportDelegate = new ReadReportDelegate(FastReadReport);
            var asyncState = new HidAsyncState(readReportDelegate, callback);
            readReportDelegate.BeginInvoke(timeout, EndReadReport, asyncState);
        }

    }
}
