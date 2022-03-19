using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Chrome.ChromeDriverExtensions;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TbsBrowser.ChromeExtension;

namespace TbsBrowser
{
    public partial class MainForm : Form
    {
        private ChromeDriver Driver;
        private readonly ChromeDriverService chromeService;
        private readonly Proxy Proxy;

        public MainForm()
        {
            InitializeComponent();
            Useragent.Instance.Load();

            Proxy = new();
            chromeService = ChromeDriverService.CreateDefaultService();
            chromeService.HideCommandPromptWindow = true;
            button2.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;

            ChromeOptions options = new();
            options.AddExtension(DisableWebRTCLeak.GetPath());
            options.AddExtensions(FingerPrintDefender.GetPath());

            options.AddHttpProxy(Proxy.Host, Proxy.Port, Proxy.Username, Proxy.Password);

            options.AddExcludedArgument("enable-automation");
            options.AddAdditionalOption("useAutomationExtension", false);
            options.AddArgument("--disable-blink-features=AutomationControlled");
            options.AddArgument("--disable-features=UserAgentClientHint");
            options.AddArgument("--disable-logging");

            options.AddArgument($"--user-agent={Useragent.Instance.GetUserAgent()}");

            // Mute audio because of the Ads
            options.AddArgument("--mute-audio");
            options.AddArgument("--no-sandbox");

            Driver = new ChromeDriver(chromeService, options);
            Driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(60);
            button2.Enabled = true;
            try
            {
                Driver.Navigate().GoToUrl(textBox2.Text);
            }
            catch { }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Proxy.Init(textBox1.Text);
            richTextBox1.Text = $"{Proxy.Host} {Proxy.Port} {Proxy.Username} {Proxy.Password}";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            Driver.Close();
            button1.Enabled = true;
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            button3.Enabled = false;
            var client = new RestClient("https://api.ipify.org/");
            ICredentials credentials = new NetworkCredential(Proxy.Username, Proxy.Password);
            client.Proxy = new WebProxy($"{Proxy.Host}:{Proxy.Port}", false, null, credentials);
            client.Timeout = 5000;
            var response = await client.ExecuteAsync(new RestRequest()
            {
                Method = Method.GET,
            });

            richTextBox2.Text = response.Content.Equals(Proxy.Host) ? $"SUCCESS {response.Content}" : "FAIL";
            button3.Enabled = true;
        }
    }
}