using MainCore.Services;
using ReactiveUI;
using System.Reactive.Disposables;
using WPFUI.Interfaces;
using WPFUI.ViewModels.Tabs.Villages;

namespace WPFUI.Views.Tabs.Villages
{
    /// <summary>
    /// Interaction logic for InfoPage.xaml
    /// </summary>
    public partial class InfoPage : ReactivePage<InfoViewModel>
    {
        public InfoPage()
        {
            ViewModel = new();
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.BindCommand(ViewModel,
                    vm => vm.BothDorfCommand,
                    v => v.UpdateDorfButton)
                .DisposeWith(d);
                this.BindCommand(ViewModel,
                    vm => vm.Dorf1Command,
                    v => v.UpdateDorf1Button)
                .DisposeWith(d);
                this.BindCommand(ViewModel,
                    vm => vm.Dorf2Command,
                    v => v.UpdateDorf2Button)
                .DisposeWith(d);

                this.OneWayBind(ViewModel,
                    vm => vm.AccountId,
                    v => v.AccountIdText.Text)
                .DisposeWith(d);
                this.OneWayBind(ViewModel,
                    vm => vm.VillageId,
                    v => v.VillageIdText.Text)
                .DisposeWith(d);
                ViewModel.OnActived();
            });
        }
    }
}