using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using System;
using UI.ViewModels;
using UI.Views;

namespace UI
{
    public partial class App : Application
    {
        public static IServiceProvider Container { get; private set; }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            Container = AppBootstrapper.Init();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var viewModel = Container.GetRequiredService<MainWindowViewModel>();

                desktop.MainWindow = new MainWindow()
                {
                    ViewModel = viewModel,
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}