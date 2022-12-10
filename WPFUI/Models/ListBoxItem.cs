using ReactiveUI;
using System;
using System.Windows.Media;

namespace WPFUI.Models
{
    public class ListBoxItem : ReactiveObject
    {
        public ListBoxItem(int id) => Id = id;

        public ListBoxItem(int id, string username, string server) : this(id)
        {
            var serverUrl = new Uri(server);
            Content = $"{username}{Environment.NewLine}({serverUrl.Host})";
            Color = Color.FromRgb(0, 0, 0);
        }

        public ListBoxItem(int id, string villageName, int x, int y) : this(id)
        {
            Content = $"{villageName}{Environment.NewLine}({x}|{y})";
        }

        public int Id { get; set; }
        public string Content { get; set; }

        private Color _color;

        public Color Color
        {
            get => _color;
            set => this.RaiseAndSetIfChanged(ref _color, value);
        }
    }
}