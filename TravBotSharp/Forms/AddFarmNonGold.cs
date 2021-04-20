using System;
using System.Windows.Forms;
using TbsCore.Models.VillageModels;
using TravBotSharp.Files.Helpers;

namespace TravBotSharp.Forms
{
    public partial class AddFarmNonGold : Form
    {
        public AddFarmNonGold(Classificator.TribeEnum? tribeEnum)
        {
            InitializeComponent();

            troopsSelectorUc1.HeroEditable = false;
            troopsSelectorUc1.Init(tribeEnum ?? Classificator.TribeEnum.Nature);
        }

        public Farm Farm
        {
            get
            {
                var coords = coordinatesUc1.Coords;
                var troops = troopsSelectorUc1.Troops;
                return new Farm(troops, coords);
            }
            set
            {
                coordinatesUc1.Coords = value.Coords;
                troopsSelectorUc1.Troops = value.Troops;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}