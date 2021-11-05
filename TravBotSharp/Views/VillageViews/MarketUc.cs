using System;
using System.Linq;
using TravBotSharp.Interfaces;
using TbsCore.Models.VillageModels;
using TbsCore.Models.ResourceModels;
using TbsCore.Models.MapModels;
using TbsCore.Tasks.LowLevel;
using TbsCore.Tasks.SecondLevel;

namespace TravBotSharp.Views
{
    public partial class MarketUc : BaseVillageUc, ITbsUc
    {
        public Resources _res;

        public MarketUc()
        {
            InitializeComponent();
            _res = new Resources();
            numericUpDown6.Maximum = long.MaxValue;
            numericUpDown7.Maximum = long.MaxValue;
            numericUpDown8.Maximum = long.MaxValue;
            numericUpDown9.Maximum = long.MaxValue;
        }

        public void UpdateUc()
        {
            var vill = GetSelectedVillage();

            var acc = GetSelectedAcc();
            if (acc == null) return;

            VillageComboBox.Items.Clear();
            foreach (var _vill in acc.Villages)
            {
                if (_vill.Id == vill.Id) continue;
                VillageComboBox.Items.Add(_vill.Name);
            }
            if (VillageComboBox.Items.Count > 0)
            {
                VillageComboBox.SelectedIndex = 0;
            }

            richTextBox1.Text = vill.Res.Stored.Resources.ToString();
            //vill.Market.Merchant = vill.Market.Merchant ?? new MerchantInfo();
            //MerchantInfo.Text = $"Merchant: {vill.Market.Merchant.Free}/ {vill.Market.Merchant.Number} ({vill.Market.Merchant.Capacity})";
        }

        private void UpdateMerchantNeed(Village vill)
        {
            //var merchant = vill.Market.Merchant;
            //if (merchant.Capacity > 0)
            //{
            //    var reminder = _res.Sum() % merchant.Capacity;
            //    if (reminder > 0)
            //    {
            //        label19.Text = $"Need {_res.Sum() / merchant.Capacity + 1} merchant";
            //    }
            //    else
            //    {
            //        label19.Text = $"Need {_res.Sum() / merchant.Capacity} merchant";
            //    }
            //}
        }

        #region Target Village Callback

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            this.VillageComboBox.Enabled = radioButton1.Checked;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            this.coordinatesUc1.Enabled = radioButton2.Checked;
        }

        #endregion Target Village Callback

        //private void button2_Click(object sender, EventArgs e) =>
        //    GetSelectedAcc().Tasks.Add(
        //        new UpdateMarket
        //        {
        //            ExecuteAt = DateTime.Now.AddHours(-10),
        //            Vill = GetSelectedVillage(),
        //        },
        //        true,
        //        GetSelectedVillage());

        private void button1_Click(object sender, EventArgs e)
        {
            Coordinates coord;
            if (radioButton1.Checked)
            {
                if (VillageComboBox.SelectedIndex == -1) return;
                coord = GetSelectedAcc().Villages.FirstOrDefault(x => x.Name == VillageComboBox.Items[VillageComboBox.SelectedIndex].ToString()).Coordinates;
            }
            else
            {
                coord = coordinatesUc1.Coords;
            }
            GetSelectedAcc().Tasks.Add(
                new SendResources
                {
                    ExecuteAt = DateTime.Now.AddHours(-10),
                    Vill = GetSelectedVillage(),
                    Resources = _res,
                    Coordinates = coord,
                    RunTimes = 1,
                },
                true,
                GetSelectedVillage());
        }

        private void numericUpDown9_ValueChanged(object sender, EventArgs e)
        {
            _res.Wood = (long)numericUpDown9.Value;
            UpdateMerchantNeed(GetSelectedVillage());
        }

        private void numericUpDown8_ValueChanged(object sender, EventArgs e)
        {
            _res.Clay = (long)numericUpDown8.Value;
            UpdateMerchantNeed(GetSelectedVillage());
        }

        private void numericUpDown7_ValueChanged(object sender, EventArgs e)
        {
            _res.Iron = (long)numericUpDown7.Value;
            UpdateMerchantNeed(GetSelectedVillage());
        }

        private void numericUpDown6_ValueChanged(object sender, EventArgs e)
        {
            _res.Crop = (long)numericUpDown6.Value;
            UpdateMerchantNeed(GetSelectedVillage());
        }

        private void button7_Click(object sender, EventArgs e)
        {
            var acc = GetSelectedAcc();
            var vill = GetSelectedVillage();
            acc.Tasks.Remove(typeof(NPC), vill);
            acc.Tasks.Add(
                new NPC()
                {
                    ExecuteAt = DateTime.Now,
                    Vill = vill,
                });
        }
    }
}