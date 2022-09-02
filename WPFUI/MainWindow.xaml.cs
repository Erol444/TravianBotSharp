using ReactiveUI;
using System;
using System.Windows;

namespace WPFUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            ViewModel = new();

            InitializeComponent();



            #region Events

            this.Events().Closing.InvokeCommand(this, x => x.ViewModel.ClosingCommand);

            #endregion Events

            #region Data

            this.Bind(ViewModel,
                vm => vm.IsAccountSelected,
                v => v.AccountTable.ViewModel.IsAccountSelected);

            this.OneWayBind(ViewModel,
                vm => vm.IsAccountNotSelected,
                v => v.NoAccountTab.Visibility);

            this.OneWayBind(ViewModel,
                vm => vm.IsAccountSelected,
                v => v.GeneralTab.Visibility);
            this.OneWayBind(ViewModel,
                vm => vm.IsAccountSelected,
                v => v.OverviewTab.Visibility);

            this.OneWayBind(ViewModel,
                vm => vm.IsAccountSelected,
                v => v.HeroTab.Visibility);
            this.OneWayBind(ViewModel,
                vm => vm.IsAccountSelected,
                v => v.VillagesTab.Visibility);

            this.OneWayBind(ViewModel,
                vm => vm.IsAccountSelected,
                v => v.DebugTab.Visibility);

            this.OneWayBind(ViewModel,
                vm => vm.CurrentAccount.Id,
                v => v.DebugPage.ViewModel.AccountId);

            this.OneWayBind(ViewModel,
                vm => vm.CurrentAccount.Id,
                v => v.HeroPage.ViewModel.AccountId);
            this.OneWayBind(ViewModel,
                vm => vm.CurrentAccount.Id,
                v => v.OverviewPage.ViewModel.AccountId);
            this.OneWayBind(ViewModel,
                vm => vm.CurrentAccount.Id,
                v => v.VillagesPage.ViewModel.AccountId);
            this.OneWayBind(ViewModel,
                vm => vm.CurrentAccount.Id,
                v => v.GeneralPage.ViewModel.AccountId);

            #endregion Data
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Application.Current.Shutdown();
        }

        private void NoAccountTab_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var value = (bool)e.NewValue;
            if (!value)
            {
                DebugTab.IsSelected = true;
            }
        }

        private void OverviewTab_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var value = (bool)e.NewValue;
            if (!value)
            {
                NoAccountTab.IsSelected = true;
            }
        }
    }
}