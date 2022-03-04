using CSCore.CoreAudioAPI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace silencer
{
    internal class AudioSession
    {
        // Lock for manager
        private static ReaderWriterLock rwLock = new ReaderWriterLock();
        
        // The calling sequence ensure manager is not null
        private static AudioSessionManager2 manager;
        public static AudioSessionEnumerator GetAudioSessionEnumerator()
        {
            // Windows Audio API is troublesome. Some of the code in CSCore
            // must be executed out of the UI Thread.
            // Create a new task to avoid any exception.
            Task.Run(() => GetDefaultAudioSessionManager2(DataFlow.Render)).Wait();
            rwLock.AcquireReaderLock(int.MaxValue);
            var enumerator = manager.GetSessionEnumerator();
            rwLock.ReleaseReaderLock();
            return enumerator;
        }

        private static void GetDefaultAudioSessionManager2(DataFlow dataFlow)
        {
            var enumerator = new MMDeviceEnumerator();
            var device = enumerator.GetDefaultAudioEndpoint(dataFlow, Role.Multimedia);
            Debug.WriteLine("DefaultDevice: " + device.FriendlyName);
            rwLock.AcquireWriterLock(int.MaxValue);
            manager = AudioSessionManager2.FromMMDevice(device);
            rwLock.ReleaseWriterLock();
        }
    }
}
