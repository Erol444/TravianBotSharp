using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;
using TbsCrossPlatform.Models.UI;
using TbsCrossPlatform.Services;

namespace TbsCrossPlatform.ViewModels
{
    public class AddAccountsViewModel : ViewModelBase
    {
        private string _lines;
        private string _formatedLines;
        private readonly List<AccountInput> _accounts;
        private readonly IUseragentService _useragentService;
        public ReactiveCommand<Unit, Unit> AddAccountCommand { get; }

        public AddAccountsViewModel()
        {
            _accounts = new();
            _useragentService = Program.GetService<IUseragentService>();
        }

        public string Lines
        {
            get => _lines;
            set
            {
                this.RaiseAndSetIfChanged(ref _lines, value);
                UpdateLines(_lines);
            }
        }

        public string FormatedLines
        {
            get => _formatedLines;
            set => this.RaiseAndSetIfChanged(ref _formatedLines, value);
        }

        public List<AccountInput> GetAccounts()
        {
            foreach (var item in _accounts)
            {
                item.Id = $"{item.Username}{item.ServerUrl}";
                item.Useragent = _useragentService.Get();
            }
            return _accounts;
        }

        public void UpdateLines(string lines)
        {
            var strArr = lines.Split(Environment.NewLine);
            var tmp = "";
            _accounts.Clear();
            foreach (var str in strArr)
            {
                var account = ParserAccout(str);
                if (account != null)
                {
                    _accounts.Add(account);
                    if (!string.IsNullOrEmpty(account.ProxyHost))
                    {
                        if (!string.IsNullOrEmpty(account.ProxyUsername))
                        {
                            tmp = $"{tmp}[{account.ServerUrl}] [{account.Username}] [{account.Password}] [{account.ProxyHost}] [{account.ProxyPort}] [{account.ProxyUsername}] [{account.ProxyPassword}]\n";
                        }
                        else
                        {
                            tmp = $"{tmp}[{account.ServerUrl}] [{account.Username}] [{account.Password}] [{account.ProxyHost}] [{account.ProxyPort}]\n";
                        }
                    }
                    else
                    {
                        tmp = $"{tmp}[{account.ServerUrl}] [{account.Username}] [{account.Password}]\n";
                    }
                }
            }
            FormatedLines = tmp;
        }

        private static AccountInput ParserAccout(string accountStr)
        {
            var strArr = accountStr.Trim().Split(' ');

            AccountInput account = null;

            int port;
            if (strArr.Length < 2) return null;

            string server = strArr[0].Replace("https://", "").Replace("http://", "");
            if (server.Contains('/'))
            {
                var str = server.Split('/');
                server = str[0];
            }

            switch (strArr.Length)
            {
                // only username & password
                case 3:
                    account = new AccountInput()
                    {
                        ServerUrl = server,
                        Username = strArr[1],
                        Password = strArr[2],
                    };
                    break;
                // username & password & proxy without autheciation
                case 5:
                    if (!int.TryParse(strArr[4], out port)) return null;
                    account = new AccountInput()
                    {
                        ServerUrl = server,
                        Username = strArr[1],
                        Password = strArr[2],
                        ProxyHost = strArr[3],
                        ProxyPort = port,
                    };
                    break;
                // full info
                case 7:
                    if (!int.TryParse(strArr[4], out port)) return null;
                    account = new AccountInput()
                    {
                        ServerUrl = server,
                        Username = strArr[1],
                        Password = strArr[2],
                        ProxyHost = strArr[3],
                        ProxyPort = port,
                        ProxyUsername = strArr[5],
                        ProxyPassword = strArr[6],
                    };
                    break;
            }

            return account;
        }
    }
}