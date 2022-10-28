using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Splat;
using UI.ViewModels;
using UI.Views;

namespace UI
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            AppBootstrapper.Register(Locator.CurrentMutable, Locator.Current);

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var viewModel = Locator.Current.GetService<MainWindowViewModel>();

                desktop.MainWindow = new MainWindow()
                {
                    ViewModel = viewModel,
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}