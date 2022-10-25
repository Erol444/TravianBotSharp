using MainCore.Helper;
using ReactiveUI;
using System;
using System.Diagnostics;
using System.Reactive;
using System.Reflection;
using System.Threading.Tasks;

namespace WPFUI.ViewModels
{
    public class VersionViewModel : ReactiveObject
    {
        public VersionViewModel()
        {
            var currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
            currentVersion = new Version(currentVersion.Major, currentVersion.Minor, currentVersion.Build);
            CurrentVersion = currentVersion.ToString();

            DiscordCommand = ReactiveCommand.Create(DiscordTask);
            LatestVersionCommand = ReactiveCommand.Create(LatestVersionTask, this.WhenAnyValue(x => x.IsNewVersion));
            CloseCommand = ReactiveCommand.Create(CloseTask);
        }

        public async Task Load()
        {
            var result = await GithubHelper.CheckGitHubLatestVersion();

            LatestVersion = result.ToString();
        }

        private void DiscordTask()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = discordUrl,
                UseShellExecute = true
            });
        }

        private void LatestVersionTask()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = GithubHelper.GetLink(_latestVersion),
                UseShellExecute = true
            });
            CloseTask();
        }

        private void CloseTask()
        {
            CloseWindow?.Invoke();
        }

        private string _currentVersion = "0.0.0";

        public string CurrentVersion
        {
            get => $"Current version: {_currentVersion}";
            set => this.RaiseAndSetIfChanged(ref _currentVersion, value);
        }

        private string _latestVersion = "0.0.0";

        public string LatestVersion
        {
            get => $"Latest version: {_latestVersion}";
            set
            {
                this.RaiseAndSetIfChanged(ref _latestVersion, value);
                this.RaisePropertyChanged(nameof(IsNewVersion));
                this.RaisePropertyChanged(nameof(Message));
            }
        }

        public bool IsNewVersion
        {
            get
            {
                var current = new Version($"{_currentVersion}.0");
                var last = new Version($"{_latestVersion}.0");
                return current.CompareTo(last) < 0;
            }
        }

        public string Message
        {
            get => message[IsNewVersion ? 1 : 0];
        }

        private readonly string discordUrl = "https://discord.gg/DVPV4gesCz";

        private readonly string[] message = {
            "You're up to date",
            "New version is available"
        };

        public ReactiveCommand<Unit, Unit> DiscordCommand { get; }
        public ReactiveCommand<Unit, Unit> LatestVersionCommand { get; }
        public ReactiveCommand<Unit, Unit> CloseCommand { get; }

        public event Action CloseWindow;
    }
}