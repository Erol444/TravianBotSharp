using System.Windows.Forms;
using TbsCore.Helpers;

namespace TravBotSharp.Forms
{
    public partial class TroopsSelectorUc : UserControl
    {
        public int[] Troops
        {
            get
            {
                int[] ret = new int[10];
                for (int i = 0; i < numerics.Length; i++)
                {
                    ret[i] = (int)numerics[i].Value;
                }
                return ret;
            }
            set
            {
                if (value != null)
                {
                    for (int i = 0; i < value.Length; i++)
                    {
                        numerics[i].Value = value[i];
                    }
                }
            }
        }

        public bool Hero
        {
            get { return checkBox1.Checked; }
            set { checkBox1.Checked = value; }
        }

        public bool HeroEditable
        {
            set { checkBox1.Enabled = value; }
        }

        public bool TroopsEditable
        {
            set
            {
                foreach (var numeric in numerics) numeric.Enabled = value;
            }
        }

        private NumericUpDown[] numerics;

        public TroopsSelectorUc()
        {
            InitializeComponent();

            numerics = new NumericUpDown[]
            {
                numericUpDown1,
                numericUpDown2,
                numericUpDown3,
                numericUpDown4,
                numericUpDown5,
                numericUpDown6,
                numericUpDown7,
                numericUpDown8,
                numericUpDown9,
                numericUpDown10,
            };
        }

        public void Init(Classificator.TribeEnum? tribe)
        {
            troopsDisplayUc1.Init(tribe ?? Classificator.TribeEnum.Nature);
        }

        public void Init(Classificator.TribeEnum tribe)
        {
            troopsDisplayUc1.Init(tribe);
        }
    }
}