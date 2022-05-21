using System.Collections.ObjectModel;
using TbsCrossPlatform.Models.UI;

namespace TbsCrossPlatform.ViewModels
{
    public class SidebarVillageViewModel : ViewModelBase
    {
        private readonly ObservableCollection<Village> _villages;

        public ObservableCollection<Village> Villages
        {
            get => _villages;
        }

        public SidebarVillageViewModel()
        {
            _villages = new()
            {
                new Village()
                {
                    Id = 124,
                    Name = "a",
                    Coordinates = new () {X = 1, Y = 1},
                },
                new Village()
                {
                    Id = 123,
                    Name = "c",
                    Coordinates = new () {X = 2, Y = 1},
                },
                new Village()
                {
                    Id = 126,
                    Name = "d",
                    Coordinates = new () {X = 1, Y = -2},
                },
            };
        }
    }
}