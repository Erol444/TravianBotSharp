using System;
using System.Windows.Forms;

namespace TravBotSharp.Forms
{
    public partial class Helper : Form
    {
        private const string bugReport = "https://discord.gg/y6n6FwARcR";

        private const string kofi = "https://ko-fi.com/vinaghost";

        public Helper()

        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(kofi);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(bugReport);
        }
    }
}