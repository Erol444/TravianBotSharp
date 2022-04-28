using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using TbsCore.Database;
using TbsCore.Helpers;
using TbsCore.Models.Access;
using TbsCore.Models.AccModels;

namespace TravBotSharp.Forms
{
    public partial class AddAccounts : Form
    {
        private List<AccountInfo> accounts = new List<AccountInfo>();

        public AddAccounts()
        {
            InitializeComponent();
        }

        private void AddAccounts_Load(object sender, EventArgs e)
        {
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            var strArr = richTextBox1.Text.Split('\n');
            richTextBox2.Text = "";

            accounts.Clear();
            foreach (var str in strArr)
            {
                var account = ParserAccout(str);
                if (account != null)
                {
                    accounts.Add(account);
                    if (!string.IsNullOrEmpty(account.Proxy.Proxy))
                    {
                        if (!string.IsNullOrEmpty(account.Proxy.ProxyUsername))
                        {
                            richTextBox2.AppendText($"[{account.Server}] [{account.Username}] [{account.Proxy.Password}] [{account.Proxy.Proxy}] [{account.Proxy.ProxyPort}] [{account.Proxy.ProxyUsername}] [{account.Proxy.ProxyPassword}]\n");

                        }
                        else
                        {
                            richTextBox2.AppendText($"[{account.Server}] [{account.Username}] [{account.Proxy.Password}] [{account.Proxy.Proxy}] [{account.Proxy.ProxyPort}]\n");
                        }
                    }
                    else
                    {
                        richTextBox2.AppendText($"[{account.Server}] [{account.Username}] [{account.Proxy.Password}]\n");
                    }
                }
            }
        }

        private static AccountInfo ParserAccout(string accountStr)
        {
            var strArr = accountStr.Trim().Split(' ');

            AccountInfo account = null;

            int port;
            if (strArr.Length < 2) return null;

            string server = strArr[0].Replace("https://", "").Replace("http://", "");
            if (server.Contains("/"))
            {
                var str = server.Split('/');
                server = str[0];
            }
            server = $"https://{server}";

            switch (strArr.Length)
            {
                // only username & password
                case 3:
                    account = new AccountInfo()
                    {
                        Server = server,
                        Username = strArr[1],
                        Proxy = GetAccessRaw(strArr[2]),
                    };
                    break;
                // username & password & proxy without autheciation
                case 5:
                    if (!int.TryParse(strArr[4], out port)) return null;
                    account = new AccountInfo()
                    {
                        Server = server,
                        Username = strArr[1],
                        Proxy = GetAccessRaw(strArr[2], strArr[3], port),
                    };
                    break;
                // full info
                case 7:
                    if (!int.TryParse(strArr[4], out port)) return null;
                    account = new AccountInfo()
                    {
                        Server = server,
                        Username = strArr[1],
                        Proxy = GetAccessRaw(strArr[2], strArr[3], port, strArr[5], strArr[6]),
                    };
                    break;
            }
            return account;
        }

        public static AccessRaw GetAccessRaw(string password, string proxyHost = null, int proxyPort = 0, string proxyUsername = null, string proxyPassword = null)
        {
            var useragent = UseragentDatabase.Instance.GetUserAgent();

            using (var hash = SHA256.Create())
            {
                var byteArray = hash.ComputeHash(Encoding.UTF8.GetBytes(useragent));
                AccessRaw accessRaw = new AccessRaw
                {
                    Password = password,
                    Proxy = proxyHost,
                    ProxyPort = proxyPort,
                    ProxyUsername = proxyUsername,
                    ProxyPassword = proxyPassword,
                    Useragent = useragent,
                    UseragentHash = BitConverter.ToString(byteArray).ToLower(),
                };
                return accessRaw;
            }
        }

        public List<Account> GetAccounts()
        {
            var result = new List<Account>();
            foreach (var item in accounts)
            {
                var acc = new Account();
                acc.Init();
                acc.AccInfo.ServerUrl = item.Server;
                acc.AccInfo.Nickname = item.Username;
                acc.Access.AddNewAccess(item.Proxy);
                IoHelperCore.CreateUserData(acc.AccInfo.Nickname, IoHelperCore.UrlRemoveHttp(acc.AccInfo.ServerUrl));

                result.Add(acc);
            }
            return result;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog1 = new OpenFileDialog())
            {
                DialogResult result = openFileDialog1.ShowDialog();
                if (result == DialogResult.OK)
                {
                    string filename = openFileDialog1.FileName;
                    try
                    {
                        var lines = File.ReadAllText(filename);
                        button1.Text = $"Loaded {Path.GetFileName(filename)}";
                        richTextBox1.Text = lines;
                    }
                    catch { }
                }
            }
        }

        public class AccountInfo
        {
            public string Server { get; set; }
            public string Username { get; set; }
            public AccessRaw Proxy { get; set; }
        }
    }
}