using ReactiveUI;
using System.Drawing;

namespace WPFUI.Models
{
    public class TroopInfo : ReactiveObject
    {
        public int Id { get; set; }
        private Bitmap _image;

        public Bitmap Image
        {
            get => _image;
            set => this.RaiseAndSetIfChanged(ref _image, value);
        }

        private string _num;

        public string Num
        {
            get => _num;
            set => this.RaiseAndSetIfChanged(ref _num, value);
        }
    }