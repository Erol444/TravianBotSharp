using MainCore.Enums;
using System.Collections.ObjectModel;
using WPFUI.Interfaces;
using WPFUI.Models;
using WPFUI.ViewModels.Abstract;

namespace WPFUI.ViewModels.Tabs.Villages
{
    public class TroopsViewModel : VillageTabBaseViewModel, ITabPage
    {
        public TroopsViewModel() : base()
        {
        }

        protected override void LoadData(int index)
        {
            Level.Clear();
            for (var i = TroopEnums.Legionnaire; i < TroopEnums.RomanSettler; i++)
            {
                Level.Add(new TroopInfo
                {
                    Troop = i,
                    Num = "0"
                });
            }
        }

        public void OnActived()
        {
            IsActive = true;
            if (CurrentVillage is null) return;
            LoadData(CurrentVillage.Id);
        }

        public void OnDeactived()
        {
            IsActive = false;
        }

        public ObservableCollection<TroopInfo> Level { get; } = new();
        public bool IsActive { get; set; }
    }
}