using ReactiveUI;
using System.Collections.ObjectModel;
using WPFUI.Models;

namespace WPFUI.ViewModels.Uc
{
    public class TroopsViewModel : ReactiveObject
    {
        public ObservableCollection<TroopInfo> Troops { get; };
    }
}