using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using TTWarsCore;
using TTWarsCore.Models;
using WPFUI.Views;

namespace WPFUI.ViewModels
{
    public class AccountViewModel : ReactiveObject
    {
        public AccountViewModel()
        {
            _contextFactory = SetupService.GetService<IDbContextFactory<AppDbContext>>();

            TestCommand = ReactiveCommand.CreateFromTask(TestTask);
            TestAllCommand = ReactiveCommand.CreateFromTask(TestAllTask);
            SaveCommand = ReactiveCommand.CreateFromTask(SaveTask);
            CancelCommand = ReactiveCommand.CreateFromTask(CancelTask);
        }

        private async Task TestTask()
        {
            await Task.Delay(599);
        }

        private async Task TestAllTask()
        {
            await Task.Delay(599);
        }

        private async Task SaveTask()
        {
            await Task.Run(() =>
            {
                var context = _contextFactory.CreateDbContext();

                if (IsNewAccount)
                {
                    var account = new Account()
                    {
                        Username = Username,
                        Server = Server,
                    };

                    context.Add(account);
                    context.SaveChanges();
                }

                Clean();
            });
        }

        private async Task CancelTask()
        {
            await Task.Run(Clean);
        }

        private void Clean()
        {
            var accountWindow = SetupService.GetService<AccountWindow>();

            if (!accountWindow.Dispatcher.CheckAccess())
            {
                accountWindow.Dispatcher.Invoke(() =>
                {
                    accountWindow.Hide();
                    Server = "";
                    Username = "";
                    Access.Clear();
                });
            }
            else
            {
                accountWindow.Hide();
                Server = "";
                Username = "";
                Access.Clear();
            }
        }

        private string _server;
        private string _username;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public string Server
        {
            get => _server;
            set => this.RaiseAndSetIfChanged(ref _server, value);
        }

        public string Username
        {
            get => _username;
            set => this.RaiseAndSetIfChanged(ref _username, value);
        }

        public ObservableCollection<Models.Access> Access { get; } = new();

        public bool IsNewAccount { get; set; }

        public ReactiveCommand<Unit, Unit> TestCommand { get; }
        public ReactiveCommand<Unit, Unit> TestAllCommand { get; }
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }
    }
}