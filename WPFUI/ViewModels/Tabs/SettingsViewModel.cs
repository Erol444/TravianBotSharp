using ReactiveUI;
using WPFUI.Interfaces;

namespace WPFUI.ViewModels.Tabs
{
    public class SettingsViewModel : ReactiveObject, IMainTabPage
    {
        private int _accountId;

        public int AccountId
        {
            get => _accountId;
            set => this.RaiseAndSetIfChanged(ref _accountId, value);
        }

        public void OnActived()
        {
            throw new System.NotImplementedException();
        }
    }
}