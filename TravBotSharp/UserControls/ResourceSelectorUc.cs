using System.Windows.Forms;

using TbsCore.Models.ResourceModels;

namespace TravBotSharp.UserControls
{
    public partial class ResourceSelectorUc : UserControl
    {
        public ResourceSelectorUc()
        {
            InitializeComponent();
        }

        public Resources resources
        {
            get
            {
                Resources res = new Resources
                {
                    Wood = (long)numericUpDown1.Value,
                    Clay = (long)numericUpDown2.Value,
                    Iron = (long)numericUpDown3.Value,
                    Crop = (long)numericUpDown4.Value,
                };
                return res;
            }
            set
            {
                if (value != null)
                {
                    numericUpDown1.Value = value.Wood;
                    numericUpDown2.Value = value.Clay;
                    numericUpDown3.Value = value.Iron;
                    numericUpDown4.Value = value.Crop;
                }
            }
        }
    }
}