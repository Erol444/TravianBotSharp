using MainCore.Helper.Interface;
using ReactiveUI;
using System;
using System.Diagnostics;
using System.Reactive;
using System.Reflection;
using System.Threading.Tasks;
using WPFUI.ViewModels.Abstract;

namespace WPFUI.ViewModels.Uc
{
    public class VersionOverlayViewModel : ViewModelBase
    {
        public ReactiveCommand<Unit, Unit> DiscordCommand { get; }
        public ReactiveCommand<Unit, Unit> LatestVersionCommand { get; }
        public ReactiveCommand<Unit, Unit> OpenCommand { get; }
        public ReactiveCommand<Unit, Unit> CloseCommand { get; }

        private readonly IGithubHelper _githubHelper;

        public VersionOverlayViewModel(IGithubHelper githubHelper)
        {
            _githubHelper = githubHelper;
            var currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
            CurrentVersion = new Version(currentVersion.Major, currentVersion.Minor, currentVersion.Build);

            DiscordCommand = ReactiveCommand.Create(DiscordTask);
            LatestVersionCommand = ReactiveCommand.Create(LatestVersionTask);
            OpenCommand = ReactiveCommand.Create(OpenTask);
            CloseCommand = ReactiveCommand.Create(CloseTask);
        }

        public async Task Load()
        {
            var result = await _githubHelper.GetLatestVersion();
            result ??= new Version(30, 4, 1975);
            LatestVersion = result;
            if (LatestVersion > CurrentVersion)
            {
                Message = message[1];
                OpenCommand.Execute().Subscribe();
            }
            else
            {
                Message = message[0];
            }
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
                FileName = _githubHelper.GetLink($"{LatestVersion}"),
                UseShellExecute = true
            });
        }

        private void OpenTask() => IsOpen = true;

        private void CloseTask() => IsOpen = false;

        private bool _isOpen = false;

        public bool IsOpen
        {
            get => _isOpen;
            set => this.RaiseAndSetIfChanged(ref _isOpen, value);
        }

        private Version _currentVersion;

        public Version CurrentVersion
        {
            get => _currentVersion;
            set => this.RaiseAndSetIfChanged(ref _currentVersion, value);
        }

        private Version _latestVersion;

        public Version LatestVersion
        {
            get => _latestVersion;
            set => this.RaiseAndSetIfChanged(ref _latestVersion, value);
        }

        private string _message;

        public string Message
        {
            get => _message;
            set => this.RaiseAndSetIfChanged(ref _message, value);
        }

        private const string discordUrl = "https://discord.gg/DVPV4gesCz";

        private readonly string[] message = {
            "You're up to date",
            "New version is available"
        };
    }
}