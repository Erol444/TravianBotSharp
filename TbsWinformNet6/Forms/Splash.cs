using System.Threading;
using System.Windows.Forms;
using TbsWinformNet6.Helpers;

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
            await ChromeDriverInstaller.Install();
            var mainForm = new ControlPanel();
            mainForm.Show();
            Hide();
        }
    }
}