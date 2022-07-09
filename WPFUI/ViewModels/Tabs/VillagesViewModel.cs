using ReactiveUI;
using System.Collections.ObjectModel;
using WPFUI.Models;

namespace WPFUI.ViewModels.Tabs
{
    public class VillagesViewModel : ReactiveObject
    {
        public VillagesViewModel()
        {
        }

        public ObservableCollection<VillageInfo> Accounts { get; } = new();
    }
}