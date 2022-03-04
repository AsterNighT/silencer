using CSCore.CoreAudioAPI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace silencer
{
    public partial class FormBrowser : Form
    {
        public string selection = "";
        public FormBrowser()
        {
            InitializeComponent();
        }

        private void FormBrowser_Load(object sender, EventArgs e)
        {
            listBoxProcessName.Items.Clear();
            var sessionEnumerator = AudioSession.GetAudioSessionEnumerator();
            foreach (var session in sessionEnumerator)
            {
                using (var sessionControl = session.QueryInterface<AudioSessionControl2>())
                {
                    // Do not know what is this
                    if (sessionControl.Process.ProcessName != "Idle")
                    {
                        listBoxProcessName.Items.Add(sessionControl.Process.ProcessName);
                    }
                }
            }
        }

        private void buttonConfirm_Click(object sender, EventArgs e)
        {
            selection = listBoxProcessName.Items[listBoxProcessName.SelectedIndex].ToString()??"";
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
