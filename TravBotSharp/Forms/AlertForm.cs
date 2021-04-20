using System;
using System.Media;
using System.Windows.Forms;

namespace TravBotSharp
{
    public partial class AlertForm : Form
    {
        private readonly SoundPlayer alert;

        public AlertForm(string msg)
        {
            InitializeComponent();
            richTextBox1.Text = msg;
            alert = new SoundPlayer(FormsResources.alert);
            alert.Play();
            BringToTop();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AlertStop();
        }

        private void AlertForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            AlertStop();
        }

        private void AlertStop()
        {
            alert.Stop();
            alert.Dispose();
        }

        public void BringToTop()
        {
            //Checks if the method is called from UI thread or not
            if (InvokeRequired)
            {
                Invoke(new Action(BringToTop));
            }
            else
            {
                if (WindowState == FormWindowState.Minimized) WindowState = FormWindowState.Normal;
                //Keeps the current topmost status of form
                var top = TopMost;
                //Brings the form to top
                TopMost = true;
                //Set form's topmost status back to whatever it was
                TopMost = top;
            }
        }
    }
}