using System.Threading;
using System.Windows.Forms;

namespace TbsWinformNet6.Forms
{
    public partial class Splash : Form
    {
        public Splash()
        {
            InitializeComponent();
        }

        private async void Splash_Shown(object sender, EventArgs e)
        {
            await Task.Delay(500); // random delay ._.
            var mainForm = new ControlPanel();
            mainForm.Show();
            Hide();
        }
    }
}