using Avalonia.Threading;
using MainCore.Services.Interface;
using MessageBox.Avalonia;
using ReactiveUI;
using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace UI.ViewModels
{
    public class VersionViewModel : ActivatableViewModelBase
    {
        private readonly string discordUrl = "https://discord.gg/DVPV4gesCz";

        private readonly string[] message = {
            "You're up to date",
            "New version is available"
        };

        public VersionViewModel(IGithubService githubService) : base()
        {
            _githubService = githubService;

            DiscordCommand = ReactiveCommand.CreateFromTask(DiscordTask);
            LatestVersionCommand = ReactiveCommand.CreateFromTask(LatestVersionTask);
        }

        protected override void OnActived(CompositeDisposable disposable)
        {
            base.OnActived(disposable);
            var thread = new Thread(async () =>
            {
                Thread.Sleep(500);
                await LoadTask();
            });
            thread.Start();
        }

        private async Task LoadTask()
        {
            await Dispatcher.UIThread.InvokeAsync(async () =>
            {
                var currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
                currentVersion = new Version(currentVersion.Major, currentVersion.Minor, currentVersion.Build);
                CurrentVersion = currentVersion.ToString();

                var result = await _githubService.GetLatestVersion();
                if (result is null)
                {
                    var messageBoxStandardWindow = MessageBoxManager.GetMessageBoxStandardWindow("Error", "It seems like you press this button too much. Github denied your request. You still press the button to get the latest");
                    await messageBoxStandardWindow.Show();
                    return;
                }

                var isNew = await _githubService.IsNewVersion(currentVersion);

                LatestVersion = result.ToString();
                Message = isNew ? message[1] : message[0];
            });
        }

        private async Task DiscordTask()
        {
            await Avalonia.Application.Current.Clipboard.SetTextAsync(discordUrl);
            var messageBoxStandardWindow = MessageBoxManager.GetMessageBoxStandardWindow("Information", "Copied url to clipboard");
            await messageBoxStandardWindow.Show();
        }

        private async Task LatestVersionTask()
        {
            await Avalonia.Application.Current.Clipboard.SetTextAsync(_githubService.GetLink(_latestVersion));
            var messageBoxStandardWindow = MessageBoxManager.GetMessageBoxStandardWindow("Information", "Copied url to clipboard");
            await messageBoxStandardWindow.Show();
        }

        private string _currentVersion;

        public string CurrentVersion
        {
            get => $"Current version: {_currentVersion}";
            set => this.RaiseAndSetIfChanged(ref _currentVersion, value);
        }

        private string _latestVersion = "0.0.0";

        public string LatestVersion
        {
            get => $"Latest version: {_latestVersion}";
            set => this.RaiseAndSetIfChanged(ref _latestVersion, value);
        }

        private string _message;

        public string Message
        {
            get => _message;
            set => this.RaiseAndSetIfChanged(ref _message, value);
        }

        public ReactiveCommand<Unit, Unit> DiscordCommand { get; }
        public ReactiveCommand<Unit, Unit> LatestVersionCommand { get; }
        private readonly IGithubService _githubService;
    }
}