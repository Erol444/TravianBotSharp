using ReactiveUI;
using WPFUI.ViewModels;

namespace WPFUI.Views
{
    /// <summary>
    /// Interaction logic for VersionWindow.xaml
    /// </summary>
    public partial class VersionWindow : ReactiveWindow<VersionViewModel>
    {
        public VersionWindow()
        {
            ViewModel = new();
            InitializeComponent();

            #region command

            this.BindCommand(ViewModel,
                vm => vm.DiscordCommand,
                v => v.DiscordButton);
            this.BindCommand(ViewModel,
                vm => vm.LatestVersionCommand,
                v => v.LastVersionButton);
            this.BindCommand(ViewModel,
                vm => vm.LatestBuildCommand,
                v => v.LastBuildButton);
            this.BindCommand(ViewModel,
                vm => vm.CloseCommand,
                v => v.CloseButton);

            #endregion command

            #region data

            this.OneWayBind(ViewModel,
                vm => vm.Message,
                v => v.StatusLabel.Text);
            this.OneWayBind(ViewModel,
                 vm => vm.CurrentVersion,
                 v => v.CurrentLabel.Text);
            this.OneWayBind(ViewModel,
                 vm => vm.LatestVersion,
                 v => v.LastVersionLabel.Text);
            this.OneWayBind(ViewModel,
                 vm => vm.LatestBuild,
                 v => v.LastBuildLabel.Text);

            #endregion data
        }
    }
}