using MainCore;
using ReactiveUI;
using System;
using System.Reactive.Linq;
using System.Windows;

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
            SendExcessToX = settings.SendExcessToX.ToString();
            SendExcessToY = settings.SendExcessToY.ToString();

            IsGetMissingResources = settings.IsGetMissingResources;
            GetMissingWood = settings.GetMissingWood.ToString();
            GetMissingClay = settings.GetMissingClay.ToString();
            GetMissingIron = settings.GetMissingIron.ToString();
            GetMissingCrop = settings.GetMissingCrop.ToString();
            SendFromX = settings.SendFromX.ToString();
            SendFromY = settings.SendFromY.ToString();

        }

        public void CopyTo(MainCore.Models.Database.VillageMarket settings)
        {
            settings.IsSendExcessResources = IsSendExcessResources;
            settings.SendExcessWood = int.Parse(SendExcessWood);
            settings.SendExcessClay = int.Parse(SendExcessClay);
            settings.SendExcessIron = int.Parse(SendExcessIron);
            settings.SendExcessCrop = int.Parse(SendExcessCrop);
            settings.SendExcessToX = int.Parse(SendExcessToX);
            settings.SendExcessToY = int.Parse(SendExcessToY);

            settings.IsGetMissingResources = IsGetMissingResources;
            settings.GetMissingWood = int.Parse(GetMissingWood);
            settings.GetMissingClay = int.Parse(GetMissingClay);
            settings.GetMissingIron = int.Parse(GetMissingIron);
            settings.GetMissingCrop = int.Parse(GetMissingCrop);
            settings.SendFromX = int.Parse(SendFromX);
            settings.SendFromY = int.Parse(SendFromY);

        }

        public bool IsValidate()
        {
            if (!SendExcessWood.IsNumeric())
            {
                MessageBox.Show("Auto SendOutResources wood is not a number.", "Warning");
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
            if (SendExcessToX[0] == '-')
            {
                var positiveString = SendExcessToX.Remove(0, 1);
                if (!positiveString.IsNumeric())
                {
                    MessageBox.Show("X coorinate is not a number.", "Warning");
                    return false;
                }
            }
            else
            {
                if (!SendExcessToX.IsNumeric())
                {
                    MessageBox.Show("X coorinate is not a number.", "Warning");
                    return false;
                }
            }
            if (SendExcessToY[0] == '-')
            {
                var positiveString = SendExcessToY.Remove(0, 1);
                if (!positiveString.IsNumeric())
                {
                    MessageBox.Show("Y coorinate is not a number.", "Warning");
                    return false;
                }
            }
            else
            {
                if (!SendExcessToY.IsNumeric())
                {
                    MessageBox.Show("Y coorinate is not a number.", "Warning");
                    return false;
                }
            }
            if (SendFromX[0] == '-')
            {
                var positiveString = SendFromX.Remove(0, 1);
                if (!positiveString.IsNumeric())
                {
                    MessageBox.Show("X coorinate is not a number.", "Warning");
                    return false;
                }
            }
            else
            {
                if (!SendFromX.IsNumeric())
                {
                    MessageBox.Show("X coorinate is not a number.", "Warning");
                    return false;
                }
            }
            if (SendFromY[0] == '-')
            {
                var positiveString = SendFromY.Remove(0, 1);
                if (!positiveString.IsNumeric())
                {
                    MessageBox.Show("Y coorinate is not a number.", "Warning");
                    return false;
                }
            }
            else
            {
                if (!SendFromY.IsNumeric())
                {
                    MessageBox.Show("Y coorinate is not a number.", "Warning");
                    return false;
                }
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
        private string _sendExcessToX;

        public string SendExcessToX
        {
            get => _sendExcessToX;
            set => this.RaiseAndSetIfChanged(ref _sendExcessToX, value);
        }

        private string _sendExcessToY;

        public string SendExcessToY
        {
            get => _sendExcessToY;
            set => this.RaiseAndSetIfChanged(ref _sendExcessToY, value);
        }
        private bool _isGetMissingResources;

        public bool IsGetMissingResources
        {
            get => _isGetMissingResources;
            set => this.RaiseAndSetIfChanged(ref _isGetMissingResources, value);
        }

        private string _getMissingWood;

        public string GetMissingWood
        {
            get => _getMissingWood;
            set => this.RaiseAndSetIfChanged(ref _getMissingWood, value);
        }

        private string _getMissingClay;

        public string GetMissingClay
        {
            get => _getMissingClay;
            set => this.RaiseAndSetIfChanged(ref _getMissingClay, value);
        }

        private string _getMissingIron;

        public string GetMissingIron
        {
            get => _getMissingIron;
            set => this.RaiseAndSetIfChanged(ref _getMissingIron, value);
        }

        private string _getMissingCrop;

        public string GetMissingCrop
        {
            get => _getMissingCrop;
            set => this.RaiseAndSetIfChanged(ref _getMissingCrop, value);
        }
        private string _sendFromX;

        public string SendFromX
        {
            get => _sendFromX;
            set => this.RaiseAndSetIfChanged(ref _sendFromX, value);
        }

        private string _sendFromY;

        public string SendFromY
        {
            get => _sendFromY;
            set => this.RaiseAndSetIfChanged(ref _sendFromY, value);
        }
    }
}