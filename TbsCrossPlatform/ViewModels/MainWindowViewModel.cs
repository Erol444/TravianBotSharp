using Avalonia.Interactivity;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using ReactiveUI;
using TbsCrossPlatform.Database;
using TbsCrossPlatform.Helper;
using TbsCrossPlatform.Views;
using static System.Net.Mime.MediaTypeNames;

namespace TbsCrossPlatform.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private bool browser = false;
        private LoadingWindow loadingWindow;

        private bool Browser
        {
            set
            {
                browser = value;
                this.RaisePropertyChanged(nameof(ContentButton));
            }
            get => browser;
        }

        public string ContentButton => Browser ? "Close chrome" : "Open chrome";

        public void OnClickCommand()
        {
            if (!browser)
            {
                loadingWindow = new();
                loadingWindow.ViewModel.Message = "Please wait . . .";
                using var context = new AccountContext();
                loadingWindow.Show();
                Browser = true;
            }
            else
            {
                loadingWindow.ViewModel.CanClose = true;
                loadingWindow.Close();
                Browser = false;
            }
        }
    }
}