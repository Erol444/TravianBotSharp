using ReactiveUI;
using System;

namespace WPFUI.ViewModels.Abstract
{
    public abstract class VillageTabBaseViewModel : TabBaseViewModel
    {
        public VillageTabBaseViewModel() : base()
        {
            this.WhenAnyValue(x => x.VillageId).Subscribe(LoadData);
        }

        protected abstract void LoadData(int index);

        private int _accountId;

        public int AccountId
        {
            get => _accountId;
            set => this.RaiseAndSetIfChanged(ref _accountId, value);
        }

        private int _villageId;

        public int VillageId
        {
            get => _villageId;
            set => this.RaiseAndSetIfChanged(ref _villageId, value);
        }
    }
}