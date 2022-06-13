using TbsWinformNet6.Forms;

namespace TbsWinformNet6
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new Splash());
        }
    }
}