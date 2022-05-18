using Avalonia.Interactivity;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using ReactiveUI;

namespace TbsCrossPlatform.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private bool browser = false;
        private IWebDriver driver;

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
                ChromeDriverService chromeService = ChromeDriverService.CreateDefaultService();
                chromeService.HideCommandPromptWindow = true;
                driver = new ChromeDriver(chromeService);
                driver.Navigate().GoToUrl("https://github.com/Erol444/TravianBotSharp");
                Browser = true;
            }
            else
            {
                driver.Quit();
                Browser = false;
            }
        }
    }
}