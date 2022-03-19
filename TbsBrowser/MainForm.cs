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
        private readonly List<Proxy> Proxies;
        private int selected = 0;
        private const string username = "vinaghost";
        private const string password = "0938682566";

        public MainForm()
        {
            InitializeComponent();
            Useragent.Instance.Load();

            Proxies = new();
            chromeService = ChromeDriverService.CreateDefaultService();
            chromeService.HideCommandPromptWindow = true;
            button2.Enabled = false;
        }

        private void UpdateListView()
        {
            listView1.Items.Clear();
            foreach (var proxy in Proxies)
            {
                var item = new ListViewItem();
                item.SubItems[0].Text = proxy.Host;
                item.SubItems.Add(proxy.Port.ToString());
                item.SubItems.Add(proxy.Check ? "✔" : "❌");
                listView1.Items.Add(item);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;

            ChromeOptions options = new();
            options.AddExtension(DisableWebRTCLeak.GetPath());
            options.AddExtensions(FingerPrintDefender.GetPath());

            options.AddHttpProxy(Proxies[selected].Host, Proxies[selected].Port, username, password);

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

        private void button2_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            try
            {
                Driver.Close();
            }
            catch { }
            button1.Enabled = true;
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            button3.Enabled = false;
            richTextBox1.Enabled = false;
            List<Task> tasks = new();
            ICredentials credentials = new NetworkCredential(username, password);

            foreach (var proxy in Proxies)
            {
                tasks.Add(Task.Run(async () =>
                {
                    var client = new RestClient("https://api.ipify.org/")
                    {
                        Proxy = new WebProxy($"{proxy.Host}:{proxy.Port}", false, null, credentials),
                        Timeout = 5000
                    };
                    var response = await client.ExecuteAsync(new RestRequest()
                    {
                        Method = Method.GET,
                    });

                    proxy.Check = response.Content.Equals(proxy.Host);
                }));
            }

            await Task.WhenAll(tasks);
            UpdateListView();
            button3.Enabled = true;
            richTextBox1.Enabled = true;
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            var strArr = richTextBox1.Text.Split('\n');
            label3.Text = $"Loaded {strArr.Length}";
            Proxies.Clear();
            foreach (var str in strArr)
            {
                var proxy = Proxy.Init(str.Trim());
                if (proxy is not null)
                {
                    Proxies.Add(proxy);
                }
            }
            UpdateListView();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices.Count < 1) return;
            selected = listView1.SelectedIndices[0];
            var proxy = Proxies[selected];
            var check = proxy.Check ? "✔" : "❌";
            textBox1.Text = $"{proxy.Host} {proxy.Port} {check}";
        }
    }
}