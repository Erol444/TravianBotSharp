using Avalonia.ReactiveUI;
using ReactiveUI;
using UI.ViewModels.UserControls;

namespace UI.Views.UserControls
{
    public partial class ButtonsPanelUc : ReactiveUserControl<ButtonsPanelViewModel>
    {
        public ButtonsPanelUc()
        {
            InitializeComponent();
            this.WhenActivated(d =>
            {
            });
        }
    }
}