using CSCore.CoreAudioAPI;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

// The program hooks windows
// EVENT_SYSTEM_FOREGROUND event(foreground window change)
// Mute all other background audio source by Windos Audio API
// when event procs, and use a list to customize.

// The rendering process may be not the audio process,
// chrome is a great example.
// So here use process name rather than process id to
// identify list and foreground process.

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

        enum WorkMode
        {
            Blacklist,
            Whitelist
        }

        WorkMode workMode;

        bool isRunning;

        // This is a lock for the global list. So as to make it possible
        // to modify the list while listening.
        // Actually this rwLock is very much the same as a mutex,
        // I can hardly come up with such a situation in which multiple
        // threads are reading the list at the same time.
        // If so, the event handler will be running too slow.
        ReaderWriterLock rwLock = new ReaderWriterLock();
        List<string> globalList;

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
            var list = new HashSet<string>(globalList);
            var mode = workMode;
            rwLock.ReleaseReaderLock();

            if (mode == WorkMode.Blacklist) list.Remove(getForegroundWindowProcessName());
            if (mode == WorkMode.Whitelist) list.Add(getForegroundWindowProcessName());

            // Pretty sure bool is atomic, no lock for it
            Mute(list, mode, isRunning);
        }

        /// <summary>
        /// Mute processes by given conditions.
        /// </summary>
        /// <param name="list">A list for processes that should
        /// never get muted if whitelisted, or those will be muted if blacklisted</param>
        /// <param name="enabled">A global switch. If set to false all processes will be unmuted</param>
        /// <returns></returns>
        private int Mute(HashSet<string> list, WorkMode mode, bool enabled)
        {
            var sessionEnumerator = AudioSession.GetAudioSessionEnumerator();
            foreach (var session in sessionEnumerator)
            {
                using (var simpleVolume = session.QueryInterface<SimpleAudioVolume>())
                using (var sessionControl = session.QueryInterface<AudioSessionControl2>())
                {
                    Debug.WriteLine("process id:" + sessionControl.ProcessID);
                    Debug.WriteLine("process name:" + sessionControl.Process.ProcessName);
                    if (workMode == WorkMode.Blacklist)
                    {
                        simpleVolume.IsMuted = list.Contains(sessionControl.Process.ProcessName) && enabled;
                    }
                    if (workMode == WorkMode.Whitelist)
                    {
                        simpleVolume.IsMuted = !list.Contains(sessionControl.Process.ProcessName) && enabled;
                    }
                }
            }
            return 0;
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            var settings =
            ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            listBoxList.Items.Clear();
            rwLock.AcquireWriterLock(int.MaxValue);
            globalList = new List<string>(settings.AppSettings.Settings["list"]?.Value.Split(new char[] { ';' }) ?? new string[] { });
            globalList = globalList.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();
            listBoxList.Items.AddRange(globalList.ToArray());
            rwLock.ReleaseWriterLock();
            this.comboBoxMode.SelectedIndex = 0;
            workMode = WorkMode.Blacklist;
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

        // Mute process should be triggered instantly after list
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
                    if (!globalList.Contains(name))
                    {
                        globalList.Add(name);
                        listBoxList.Items.Add(name);
                        saveConfig();
                    }
                    rwLock.ReleaseWriterLock();
                }
            }

            updateMuteTarget();
        }

        private void buttonDeleteItem_Click(object sender, EventArgs e)
        {
            if (listBoxList.SelectedIndex == -1) return;
            rwLock.AcquireWriterLock(int.MaxValue);
            globalList.RemoveAt(listBoxList.SelectedIndex);
            listBoxList.Items.RemoveAt(listBoxList.SelectedIndex);
            saveConfig();
            rwLock.ReleaseWriterLock();
            updateMuteTarget();
        }

        private void saveConfig()
        {
            var settings =
            ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            rwLock.AcquireReaderLock(int.MaxValue);
            settings.AppSettings.Settings.Remove("list");
            settings.AppSettings.Settings.Add("list", string.Join(';', globalList));
            settings.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
            rwLock.ReleaseReaderLock();
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            isRunning = false;
            updateMuteTarget();
        }

        private void Reveal()
        {
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
            this.Activate();
        }
        private void Conceal()
        {
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
        }

        private void toolStripMenuItemShow_Click(object sender, EventArgs e)
        {
            this.Reveal();
        }

        private void toolStripMenuItemExit_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Conceal();
        }

        private void comboBoxMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            rwLock.AcquireWriterLock(int.MaxValue);
            workMode = (WorkMode)comboBoxMode.SelectedIndex;
            rwLock.ReleaseWriterLock();
        }
    }
}