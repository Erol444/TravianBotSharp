using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using TravBotSharp.Forms;
using TravBotSharp.Helpers;

namespace TravBotSharp
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static async Task Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Splash.ShowSplashScreen();
            try
            {
                // await ChromeDriverInstaller.Install();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return;
            }
            var mainForm = new ControlPanel();
            Splash.CloseForm();
            Application.Run(mainForm);
        }
    }
}