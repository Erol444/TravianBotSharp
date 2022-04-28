using System;
using System.Windows.Forms;
using TravBotSharp.Forms;

namespace TravBotSharp
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Splash.ShowSplashScreen();
            var mainForm = new ControlPanel();
            Splash.CloseForm();
            Application.Run(mainForm);
        }
    }
}