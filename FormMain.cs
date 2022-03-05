using CSCore.CoreAudioAPI;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

// The program hooks windows
// EVENT_SYSTEM_FOREGROUND event(foreground window change)
// Mute all other background audio source by Windos Audio API
// when event procs, and use a whitelist to customize.

// The rendering process may be not the audio process,
// chrome is a great example.
// So here use process name rather than process id to
// identify whitelist and foreground process.

namespace silencer
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
            isRunning = false;
            buttonBegin.Enabled = !isRunning;
            buttonEnd.Enabled = isRunning;
        }

        bool isRunning;

        // This is a lock for the global whitelist. So as to make it possible
        // to modify the whitelist while listening.
        // Actually this rwLock is very much the same as a mutex,
        // I can hardly come up with such a situation in which multiple
        // threads are reading the whitelist at the same time.
        // If so, the event handler will be running too slow.
        ReaderWriterLock rwLock = new ReaderWriterLock();
        List<string> globalWhitelist;

        delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        [DllImport("user32.dll")]
        static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);
        [DllImport("user32.dll")]
        static extern bool UnhookWinEvent(IntPtr hWinEventHook);
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);


        private WinEventDelegate dele;
        private IntPtr hWinEventHook;
        private const uint WINEVENT_OUTOFCONTEXT = 0;
        private const uint EVENT_SYSTEM_FOREGROUND = 3;

        private uint getForegroundWindowPID()
        {
            uint pid;
            IntPtr handle = GetForegroundWindow();

            GetWindowThreadProcessId(handle, out pid);
            return pid;
        }

        private string getForegroundWindowProcessName()
        {
            uint pid = getForegroundWindowPID();
            return Process.GetProcessById((int)pid).ProcessName;
        }

        private void onForegroundWindowChange(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            updateMuteTarget();
        }

        private void updateMuteTarget()
        {
            var foregroundName = getForegroundWindowProcessName();
            Debug.WriteLine("Foreground name:" + foregroundName);

            // Do not care about the waiting time ,just make it INF;
            rwLock.AcquireReaderLock(int.MaxValue);
            var whitelist = new HashSet<string>(globalWhitelist);
            rwLock.ReleaseReaderLock();

            whitelist.Add(getForegroundWindowProcessName());

            // Pretty sure bool is atomic, no lock for it
            Mute(whitelist, isRunning);
        }

        /// <summary>
        /// Mute processes by given conditions.
        /// </summary>
        /// <param name="whitelist">A whitelist for processes that should
        /// never get muted</param>
        /// <param name="enabled">A global switch. If set to false all processes will be unmuted</param>
        /// <returns></returns>
        private int Mute(HashSet<string> whitelist, bool enabled)
        {
            var sessionEnumerator = AudioSession.GetAudioSessionEnumerator();
            foreach (var session in sessionEnumerator)
            {
                using (var simpleVolume = session.QueryInterface<SimpleAudioVolume>())
                using (var sessionControl = session.QueryInterface<AudioSessionControl2>())
                {
                    Debug.WriteLine("process id:" + sessionControl.ProcessID);
                    Debug.WriteLine("process name:" + sessionControl.Process.ProcessName);
                    simpleVolume.IsMuted = !whitelist.Contains(sessionControl.Process.ProcessName) && enabled;
                }
            }
            return 0;
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            var settings =
            ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            listBoxWhitelist.Items.Clear();
            rwLock.AcquireWriterLock(int.MaxValue);
            globalWhitelist = new List<string>(settings.AppSettings.Settings["whitelist"]?.Value.Split(new char[] { ';' }) ?? new string[] { });
            globalWhitelist = globalWhitelist.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();
            listBoxWhitelist.Items.AddRange(globalWhitelist.ToArray());
            rwLock.ReleaseWriterLock();
        }

        private void beginListening(object sender, EventArgs e)
        {
            isRunning = true;
            dele = new WinEventDelegate(onForegroundWindowChange);
            hWinEventHook = SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, dele, 0, 0, WINEVENT_OUTOFCONTEXT);
            buttonBegin.Enabled = !isRunning;
            buttonEnd.Enabled = isRunning;
            updateMuteTarget();
        }

        private void endListening(object sender, EventArgs e)
        {
            isRunning = false;
            UnhookWinEvent(hWinEventHook);
            buttonBegin.Enabled = !isRunning;
            buttonEnd.Enabled = isRunning;
            updateMuteTarget();
        }

        // Mute process should be triggered instantly after whitelist
        // is modified, so as to reflect the change.

        private void buttonAddItem_Click(object sender, EventArgs e)
        {
            using (var form = new FormBrowser())
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    string name = form.selection;            //values preserved after close
                    if (name == "") return;
                    rwLock.AcquireWriterLock(int.MaxValue);
                    if (!globalWhitelist.Contains(name))
                    {
                        globalWhitelist.Add(name);
                        listBoxWhitelist.Items.Add(name);
                        saveConfig();
                    }
                    rwLock.ReleaseWriterLock();
                }
            }

            updateMuteTarget();
        }

        private void buttonDeleteItem_Click(object sender, EventArgs e)
        {
            if (listBoxWhitelist.SelectedIndex == -1) return;
            rwLock.AcquireWriterLock(int.MaxValue);
            globalWhitelist.RemoveAt(listBoxWhitelist.SelectedIndex);
            listBoxWhitelist.Items.RemoveAt(listBoxWhitelist.SelectedIndex);
            saveConfig();
            rwLock.ReleaseWriterLock();
            updateMuteTarget();
        }

        private void saveConfig()
        {
            var settings =
            ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            rwLock.AcquireReaderLock(int.MaxValue);
            settings.AppSettings.Settings.Remove("whitelist");
            settings.AppSettings.Settings.Add("whitelist", string.Join(';', globalWhitelist));
            settings.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
            rwLock.ReleaseReaderLock();
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            isRunning = false;
            updateMuteTarget();
        }
    }
}