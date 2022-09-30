using ReactiveUI;
using System;

namespace WPFUI.ViewModels.Abstract
{
    public abstract class VillageTabBaseViewModel : TabBaseViewModel
    {
        public VillageTabBaseViewModel() : base()
        {
            this.WhenAnyValue(x => x.CurrentVillage).Subscribe((x) =>
            {
                if (x is not null)
                {
                    LoadData(x.Id);
                }
            });
        }

        protected abstract void LoadData(int index);

        private MainCore.Models.Database.Account _currentAccount;

        public MainCore.Models.Database.Account CurrentAccount
        {
            get => _currentAccount;
            set => this.RaiseAndSetIfChanged(ref _currentAccount, value);
        }

        private Models.Village _currentVillage;

        public Models.Village CurrentVillage
        {
            get => _currentVillage;
            set => this.RaiseAndSetIfChanged(ref _currentVillage, value);
        }
    }
}