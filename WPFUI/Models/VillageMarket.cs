using MainCore.Helper;
using ReactiveUI;
using System.Windows;
using System.Diagnostics;

namespace WPFUI.Models
{
    public class VillageMarket : ReactiveObject
    {
        public void CopyFrom(MainCore.Models.Database.VillageMarket settings)
        {
            IsSendExcessResources = settings.IsSendExcessResources;
            SendExcessWood = settings.SendExcessWood.ToString();
            SendExcessClay = settings.SendExcessClay.ToString();
            SendExcessIron = settings.SendExcessIron.ToString();
            SendExcessCrop = settings.SendExcessCrop.ToString();
        }

        public void CopyTo(MainCore.Models.Database.VillageMarket settings)
        {
            settings.IsSendExcessResources = IsSendExcessResources;
            settings.SendExcessWood = int.Parse(SendExcessWood);
            settings.SendExcessClay = int.Parse(SendExcessClay);
            settings.SendExcessIron = int.Parse(SendExcessIron);
            settings.SendExcessCrop = int.Parse(SendExcessCrop);
        }

        public bool IsValidate()
        {
            Debug.WriteLine("Hello world");
            Debug.WriteLine(SendExcessWood);
            Debug.WriteLine(SendExcessClay);
            Debug.WriteLine(SendExcessCrop);
            if (!SendExcessWood.IsNumeric())
            {
                MessageBox.Show("Auto NPC wood is not a number.", "Warning");
                return false;
            }
            if (!SendExcessClay.IsNumeric())
            {
                MessageBox.Show("Auto NPC clay is not a number.", "Warning");
                return false;
            }
            if (!SendExcessIron.IsNumeric())
            {
                MessageBox.Show("Auto NPC iron is not a number.", "Warning");
                return false;
            }
            if (!SendExcessCrop.IsNumeric())
            {
                MessageBox.Show("Auto NPC crop is not a number.", "Warning");
                return false;
            }

            return true;
        }

        private bool _isSendExcessResources;

        public bool IsSendExcessResources
        {
            get => _isSendExcessResources;
            set => this.RaiseAndSetIfChanged(ref _isSendExcessResources, value);
        }

        private string _sendExcessWood;

        public string SendExcessWood
        {
            get => _sendExcessWood;
            set => this.RaiseAndSetIfChanged(ref _sendExcessWood, value);
        }

        private string _sendExcessClay;

        public string SendExcessClay
        {
            get => _sendExcessClay;
            set => this.RaiseAndSetIfChanged(ref _sendExcessClay, value);
        }

        private string _sendExcessIron;

        public string SendExcessIron
        {
            get => _sendExcessIron;
            set => this.RaiseAndSetIfChanged(ref _sendExcessIron, value);
        }

        private string _sendExcessCrop;

        public string SendExcessCrop
        {
            get => _sendExcessCrop;
            set => this.RaiseAndSetIfChanged(ref _sendExcessCrop, value);
        }
    }
}