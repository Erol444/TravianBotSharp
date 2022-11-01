using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using ReactiveUI;
using Splat.Microsoft.Extensions.DependencyInjection;
using System;
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
            Container.UseMicrosoftDependencyResolver();

            RxApp.MainThreadScheduler = AvaloniaScheduler.Instance;

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow();
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}