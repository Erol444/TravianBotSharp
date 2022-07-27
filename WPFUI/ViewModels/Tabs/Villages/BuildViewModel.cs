using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using WPFUI.Models;

namespace WPFUI.ViewModels.Tabs.Villages
{
    public class BuildViewModel : ReactiveObject
    {
        public ReactiveCommand<Unit, Unit> SingleBuildCommand { get; }
        public ReactiveCommand<Unit, Unit> ResourceBuildCommand { get; }
        public ReactiveCommand<Unit, Unit> PreBuildCommand { get; }

        public ReactiveCommand<Unit, Unit> TopCommand { get; }
        public ReactiveCommand<Unit, Unit> BottomCommand { get; }
        public ReactiveCommand<Unit, Unit> UpCommand { get; }
        public ReactiveCommand<Unit, Unit> DownCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteCommand { get; }
        public ReactiveCommand<Unit, Unit> ClearCommand { get; }
        public ReactiveCommand<Unit, Unit> ImportCommand { get; }
        public ReactiveCommand<Unit, Unit> ExportCommand { get; }

        public ObservableCollection<BuildingInfo> Buildings { get; } = new();
        public ObservableCollection<CurrentlyBuildingInfo> CurrentlyBuildings { get; } = new();
        public ObservableCollection<QueueBuildingInfo> QueueBuildings { get; } = new();

        private Building _singleBuilding;

        public Building SingleBuilding
        {
            get => _singleBuilding;
            set => this.RaiseAndSetIfChanged(ref _singleBuilding, value);
        }

        private
    }
}