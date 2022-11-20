using Avalonia.ReactiveUI;
using ReactiveUI;
using Splat;
using System.Reactive.Disposables;
using UI.ViewModels.UserControls;

namespace UI.Views.UserControls
{
    public partial class ResourcesBuildUc : ReactiveUserControl<ResourcesBuildViewModel>
    {
        public ResourcesBuildUc()
        {
            ViewModel = Locator.Current.GetService<ResourcesBuildViewModel>();
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.BindCommand(ViewModel, vm => vm.BuildCommand, v => v.Build).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.ComboResTypes, v => v.Type.Items).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.ComboStrategy, v => v.Strategy.Items).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Level, v => v.Level.Value).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.MinLevel, v => v.Level.Minimum).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.MaxLevel, v => v.Level.Maximum).DisposeWith(d);
            });
        }
    }
}