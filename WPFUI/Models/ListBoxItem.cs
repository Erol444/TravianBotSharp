using ReactiveUI;
using System;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace WPFUI.Models
{
    public class ListBoxItem : ReactiveObject
    {
        public ListBoxItem()
        { }

        public ListBoxItem(int id) => Id = id;

        public ListBoxItem(int id, string content, Color color) : this(id)
        {
            Content = content;
            Color = color;
        }

        public ListBoxItem(int id, string username, string server) : this(id)
        {
            var serverUrl = new Uri(server);
            Content = $"{username}{Environment.NewLine}({serverUrl.Host})";
            Color = Color.FromRgb(0, 0, 0);
        }

        public ListBoxItem(int id, string villageName, int x, int y) : this(id)
        {
            Content = $"{villageName}{Environment.NewLine}({x}|{y})";
            Color = Color.FromRgb(0, 0, 0);
        }

        public ListBoxItem(int id, string content, System.Drawing.Color color) : this(id, content, color.ToMediaColor())
        {
        }

        public int Id { get; set; }
        private string _content;

        public string Content
        {
            get => _content;
            set => this.RaiseAndSetIfChanged(ref _content, value);
        }

        private Color _color;

        public Color Color
        {
            get => _color;
            set => this.RaiseAndSetIfChanged(ref _color, value);
        }
    }

    public static class ColorExtension
    {
        /// <summary>
        /// Convert Media Color (WPF) to Drawing Color (WinForm)
        /// </summary>
        /// <param name="mediaColor"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Drawing.Color ToDrawingColor(this System.Windows.Media.Color mediaColor)
        {
            return System.Drawing.Color.FromArgb(mediaColor.A, mediaColor.R, mediaColor.G, mediaColor.B);
        }

        /// <summary>
        /// Convert Drawing Color (WinForm) to Media Color (WPF)
        /// </summary>
        /// <param name="drawingColor"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static System.Windows.Media.Color ToMediaColor(this System.Drawing.Color drawingColor)
        {
            return System.Windows.Media.Color.FromArgb(drawingColor.A, drawingColor.R, drawingColor.G, drawingColor.B);
        }
    }
}