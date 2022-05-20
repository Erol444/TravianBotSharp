using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Microsoft.Extensions.DependencyInjection;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using ReactiveUI;
using System;
using System.Threading.Tasks;
using TbsCrossPlatform.Database;
using TbsCrossPlatform.Helper;
using TbsCrossPlatform.Services;
using TbsCrossPlatform.Views;
using static System.Net.Mime.MediaTypeNames;

namespace TbsCrossPlatform.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IWaitingService _waitingService;

        public MainWindowViewModel()
        {
            _waitingService = Program.GetService<IWaitingService>();
        }

        public static string ContentButton => "Test";

        public async void OnClickCommand()
        {
            var task = _waitingService.Show();
            using var context = new AccountContext();
            _waitingService.Close();
            await task;
        }
    }
}