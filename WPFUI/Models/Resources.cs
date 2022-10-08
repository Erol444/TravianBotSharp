using MainCore.Helper;
using ReactiveUI;
using System.Windows;

namespace WPFUI.Models
{
    public class Resources : ReactiveObject
    {
        public MainCore.Models.Runtime.Resources GetResources()
        {
            return new()
            {
                Wood = int.Parse(Wood),
                Clay = int.Parse(Clay),
                Iron = int.Parse(Iron),
                Crop = int.Parse(Crop)
            };
        }

        public bool IsVaild()
        {
            if (!Wood.IsNumeric())
            {
                MessageBox.Show("Wood is not a number");
                return false;
            }
            if (!Clay.IsNumeric())
            {
                MessageBox.Show("Clay is not a number");
                return false;
            }
            if (!Iron.IsNumeric())
            {
                MessageBox.Show("Iron is not a number");
                return false;
            }
            if (!Crop.IsNumeric())
            {
                MessageBox.Show("Crop is not a number");
                return false;
            }
            return true;
        }

        private string _wood;

        public string Wood
        {
            get => _wood;
            set => this.RaiseAndSetIfChanged(ref _wood, value);
        }

        private string _clay;

        public string Clay
        {
            get => _clay;
            set => this.RaiseAndSetIfChanged(ref _clay, value);
        }

        private string _iron;

        public string Iron
        {
            get => _iron;
            set => this.RaiseAndSetIfChanged(ref _iron, value);
        }

        private string _crop;

        public string Crop
        {
            get => _crop;
            set => this.RaiseAndSetIfChanged(ref _crop, value);
        }
    }
}