using ReactiveUI;

namespace UI.Models
{
    public class BuildingModel : ReactiveObject
    {
        public BuildingModel(int id)
        {
            Id = id;
        }

        public int Id { get; }
        private string _color;

        public string Color
        {
            get => _color;
            set => this.RaiseAndSetIfChanged(ref _color, value);
        }

        private string _content;

        public string Content
        {
            get => _content;
            set => this.RaiseAndSetIfChanged(ref _content, value);
        }
    }
}