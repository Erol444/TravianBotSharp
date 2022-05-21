using System.Collections.ObjectModel;
using TbsCrossPlatform.Models.UI;

namespace TbsCrossPlatform.ViewModels
{
    public class SidebarViewModel : ViewModelBase
    {
        private readonly ObservableCollection<Account> _accounts;

        public ObservableCollection<Account> Accounts
        {
            get => _accounts;
        }

        public SidebarViewModel()
        {
            _accounts = new()
            {
                new Account()
                {
                    Id = "1asd",
                    ServerUrl = "ts2.internal",
                    Username = "abc",
                },
                new Account()
                {
                    Id = "20d",
                    ServerUrl = "ts2.internal",
                    Username = "wsd",
                },
                new Account()
                {
                    Id = "3asd",
                    ServerUrl = "ts2.internal",
                    Username = "vca",
                }
            };
        }
    }
}