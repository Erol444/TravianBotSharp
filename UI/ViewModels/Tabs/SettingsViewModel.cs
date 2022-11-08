using Avalonia.Controls;
using MainCore;
using MainCore.Models.Database;
using MessageBox.Avalonia;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using Splat;
using System;
using System.IO;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using UI.Models;
using UI.ViewModels.UserControls;
using UI.Views;

namespace UI.ViewModels.Tabs
{
    public class SettingsViewModel : ActivatableViewModelBase
    {
        public SettingsViewModel(IDbContextFactory<AppDbContext> contextFactory, AccountViewModel accountViewModel, LoadingOverlayViewModel loadingOverlayViewModel) : base()
        {
            _contextFactory = contextFactory;
            _accountViewModel = accountViewModel;
            _loadingOverlayViewModel = loadingOverlayViewModel;
            SaveCommand = ReactiveCommand.CreateFromTask(SaveTask);
            ExportCommand = ReactiveCommand.CreateFromTask(ExportTask);
            ImportCommand = ReactiveCommand.CreateFromTask(ImportTask);
        }

        protected override void OnActived(CompositeDisposable disposable)
        {
            base.OnActived(disposable);
            RxApp.MainThreadScheduler.Schedule(async () => await LoadData(_cancellationTokenSource.Token));
        }

        protected override void OnDeactived()
        {
            base.OnDeactived();
            _loadingOverlayViewModel.Unload();
        }

        private async Task LoadData(CancellationToken token)
        {
            _loadingOverlayViewModel.Load();
            _loadingOverlayViewModel.LoadingText = "Loading settings ...";
            using var context = await _contextFactory.CreateDbContextAsync(token);
            if (token.IsCancellationRequested) return;
            var settings = await context.AccountsSettings.FindAsync(new object[] { _accountViewModel.Account.Id }, cancellationToken: token);
            if (token.IsCancellationRequested) return;
            Settings.CopyFrom(settings);
            _loadingOverlayViewModel.Unload();
        }

        private async Task SaveTask()
        {
            var result = Settings.IsValid();
            if (result.IsFailed)
            {
                var messageBoxStandardWindow = MessageBoxManager.GetMessageBoxStandardWindow("Error", result.Reasons[0].Message);
                await messageBoxStandardWindow.Show();
                return;
            }
            _loadingOverlayViewModel.Load();
            _loadingOverlayViewModel.LoadingText = "Saving settings ...";
            var token = _cancellationTokenSource.Token;

            using var context = await _contextFactory.CreateDbContextAsync(token);
            if (token.IsCancellationRequested) return;
            var settings = await context.AccountsSettings.FindAsync(new object[] { _accountViewModel.Account.Id }, cancellationToken: token);
            if (token.IsCancellationRequested) return;
            Settings.CopyTo(settings);
            context.Update(settings);
            await context.SaveChangesAsync();
            _loadingOverlayViewModel.Unload();

            var success = MessageBoxManager.GetMessageBoxStandardWindow("Success", "Account's settings are saved");
            await success.Show();
        }

        private async Task ExportTask()
        {
            var token = _cancellationTokenSource.Token;

            using var context = await _contextFactory.CreateDbContextAsync(token);
            if (token.IsCancellationRequested) return;
            var account = await context.Accounts.FindAsync(new object[] { _accountViewModel.Account.Id }, cancellationToken: token);
            if (token.IsCancellationRequested) return;

            var sfd = new SaveFileDialog
            {
                Directory = AppContext.BaseDirectory,
                InitialFileName = $"{account.Username}_settings.tbs",
            };

            var path = await sfd.ShowAsync(Locator.Current.GetService<MainWindow>());
            if (path is null) return;

            var settings = await context.AccountsSettings.FindAsync(new object[] { _accountViewModel.Account.Id }, cancellationToken: token);
            if (token.IsCancellationRequested) return;
            var jsonString = JsonSerializer.Serialize(settings);
            File.WriteAllText(path, jsonString);
        }

        private async Task ImportTask()
        {
            var token = _cancellationTokenSource.Token;

            using var context = await _contextFactory.CreateDbContextAsync(token);
            if (token.IsCancellationRequested) return;
            var account = await context.Accounts.FindAsync(new object[] { _accountViewModel.Account.Id }, cancellationToken: token);
            if (token.IsCancellationRequested) return;

            var ofd = new OpenFileDialog
            {
                Directory = AppContext.BaseDirectory,
                AllowMultiple = false,
            };

            var path = await ofd.ShowAsync(Locator.Current.GetService<MainWindow>());
            if (path is null) return;

            var jsonString = File.ReadAllText(path[0]);

            try
            {
                var settings = JsonSerializer.Deserialize<AccountSetting>(jsonString);

                settings.AccountId = _accountViewModel.Account.Id;
                context.Update(settings);
                await context.SaveChangesAsync(token);
            }
            catch
            {
                var error = MessageBoxManager.GetMessageBoxStandardWindow("Error", "Invalid settings file");
                await error.Show();
            }
        }

        public AccountSettingsModel Settings { get; } = new();
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly AccountViewModel _accountViewModel;
        private readonly LoadingOverlayViewModel _loadingOverlayViewModel;

        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> ExportCommand { get; }
        public ReactiveCommand<Unit, Unit> ImportCommand { get; }
    }
}