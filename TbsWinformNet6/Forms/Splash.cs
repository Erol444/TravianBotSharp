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

        //Delegate for cross thread call to close
        private delegate void CloseDelegate();

        //The type of form to be displayed as the splash screen.
        private static Splash splashForm;

        public static void ShowSplashScreen()
        {
            // Make sure it is only launched once.
            if (splashForm != null) return;
            splashForm = new Splash();
            Thread thread = new Thread(new ThreadStart(ShowForm))
            {
                IsBackground = true
            };
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        private static void ShowForm()
        {
            if (splashForm != null) Application.Run(splashForm);
        }

        public static void CloseForm()
        {
            splashForm?.Invoke(new CloseDelegate(Splash.CloseFormInternal));
        }

        private static void CloseFormInternal()
        {
            if (splashForm != null)
            {
                splashForm.Close();
                splashForm = null;
            };
        }
    }
}