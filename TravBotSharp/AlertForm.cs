using System;
using System.Media;
using System.Windows.Forms;

namespace TravBotSharp
{
    public partial class AlertForm : Form
    {
        SoundPlayer simpleSound;
        public AlertForm(string msg)
        {
            InitializeComponent();
            label2.Text = msg;
            simpleSound = new SoundPlayer("data/alert.wav");
            simpleSound.Play();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            simpleSound.Stop();
        }
    }
}
