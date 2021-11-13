using System;
using System.Windows.Forms;

namespace TravBotSharp.Forms
{
    public partial class Close : Form
    {
        public Close()
        {
            InitializeComponent();
        }

        private void Close_Load(object sender, EventArgs e)
        {
            this.CenterToParent();
        }
    }
}