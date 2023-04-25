using MainCore.Helper.Interface;
using ReactiveUI;
using Splat;
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
            _githubHelper = Locator.Current.GetService<IGithubHelper>();
        }

        public async Task Load()
        {
            var result = await _githubHelper.GetLatestVersion();
            if (result is null) result = new Version(30, 4, 1975);
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
                FileName = _githubHelper.GetLink(_latestVersion),
                UseShellExecute = true
            });
            Close();
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
        private readonly IGithubHelper _githubHelper;

        public Action Close;
        public Action Show;
    }
}