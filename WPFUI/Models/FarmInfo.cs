using ReactiveUI;

namespace WPFUI.Models
{
    public class FarmInfo : ReactiveObject
    {
        public int Id { get; set; }
        public string Name { get; set; }

        private string _color;

        public string Color
        {
            get => _color;
            set => this.RaiseAndSetIfChanged(ref _color, value);
        }
    }
}