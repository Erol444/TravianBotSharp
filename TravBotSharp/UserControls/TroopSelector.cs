using System.Windows.Forms;
using TbsCore.Helpers;

namespace TravBotSharp.UserControls
{
    public partial class TroopSelector : UserControl
    {
        public Classificator.TroopsEnum? SelectedTroop
        {
            get
            {
                if (_tribe == Classificator.TribeEnum.Any) return null;
                int troopsEnum = ((int)_tribe - 1) * 10;
                var troopSelected = troopsEnum + comboBox.SelectedIndex + 1;
                var troop = (Classificator.TroopsEnum)troopSelected;
                return troop;
            }
            set
            {
                if (value == null) return;
                var troopNum = (int)value % 10 - 1;
                if (9 <= comboBox.Items.Count) comboBox.SelectedIndex = troopNum;
            }
        }

        public TroopSelector()
        {
            InitializeComponent();
        }

        private Classificator.TribeEnum _tribe;

        public void Init(Classificator.TribeEnum? tribe)
        {
            _tribe = tribe ?? Classificator.TribeEnum.Any;

            comboBox.Items.Clear();
            int troopsEnum = ((int)_tribe - 1) * 10;
            for (var i = troopsEnum + 1; i < troopsEnum + 11; i++)
            {
                Classificator.TroopsEnum troop = (Classificator.TroopsEnum)i;
                comboBox.Items.Add(VillageHelper.EnumStrToString(troop.ToString()));
            }
            if (0 < comboBox.Items.Count) comboBox.SelectedIndex = 0;
        }
    }
}
