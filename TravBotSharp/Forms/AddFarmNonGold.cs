﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TbsCore.Models.VillageModels;
using TbsCore.Helpers;

namespace TravBotSharp.Forms
{
    public partial class AddFarmNonGold : Form
    {
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

        public AddFarmNonGold(Classificator.TribeEnum? tribeEnum)
        {
            InitializeComponent();

            troopsSelectorUc1.HeroEditable = false;
            troopsSelectorUc1.Init(tribeEnum ?? Classificator.TribeEnum.Nature);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}