using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TbsCrossPlatform.Models.UI;

namespace TbsCrossPlatform.ViewModels
{
    public class EditAccountViewModel : ViewModelBase
    {
        private string _username = "hello";

        public string Username
        {
            get => _username;
            set => this.RaiseAndSetIfChanged(ref _username, value);
        }

        private string _server = "travian.com";

        public string Server
        {
            get => _server;
            set => this.RaiseAndSetIfChanged(ref _server, value);
        }

        public ObservableCollection<Access> Accesses { get; private set; } = new();

        public void Opened(object sender, EventArgs e)
        {
            Accesses.Add(new Access
            {
                AccountId = 1,
                Id = 2,
                Password = "abc",
                ProxyHost = "11.11.11.11",
                ProxyPort = 23,
                ProxyUsername = "hm",
                ProxyPassword = "sadasd",
            });
            Accesses.Add(new Access
            {
                AccountId = 1,
                Id = 3,
                Password = "erfssc",
            });
            Accesses.Add(new Access
            {
                AccountId = 1,
                Id = 4,
                Password = "sadsaw",
                ProxyHost = "11.11.11.11",
                ProxyPort = 23,
            });
            Accesses.Add(new Access
            {
                AccountId = 1,
                Id = 2,
                Password = "eqwewc",
                ProxyHost = "11.11.11.11",
                ProxyPort = 341,
                ProxyUsername = "hm",
                ProxyPassword = "sad213w",
            });
        }
    }
}