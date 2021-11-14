using System;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using TravBotSharp.Interfaces;
using TbsCore.Models.VillageModels;
using TbsCore.Models.ResourceModels;
using TbsCore.Models.MapModels;
using TbsCore.Models.Settings;
using static TbsCore.Helpers.MarketHelper;
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
            UpdateTradeRouteView(vill);

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

        private void UpdateTradeRouteView(Village vill)
        {
            var traderoute = vill.Market.TradeRoute;
            if (traderoute == null) return;
            var vills = GetSelectedAcc().Villages;

            routeTradeList.Items.Clear();
            foreach (var route in traderoute.TradeRoutes)
            {
                ListViewItem item = new ListViewItem();

                item.SubItems[0].Text = (routeTradeList.Items.Count + 1).ToString();
                item.SubItems.Add(vills.FirstOrDefault(x => x.Coordinates.Equals(route.Location))?.Name ?? route.Location.ToString());
                item.SubItems.Add(route.Resource.Wood.ToString());
                item.SubItems.Add(route.Resource.Clay.ToString());
                item.SubItems.Add(route.Resource.Iron.ToString());
                item.SubItems.Add(route.Resource.Crop.ToString());
                item.SubItems.Add(route.Time.ToString());
                item.SubItems.Add(route.Last.ToString());
                item.SubItems.Add(route.Active ? "✔" : "✘");

                item.ForeColor = Color.White;
                routeTradeList.Items.Add(item);
            }
        }

        #region Target Village radiobutton

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            this.VillageComboBox.Enabled = radioButton1.Checked;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            this.coordinatesUc1.Enabled = radioButton2.Checked;
        }

        #endregion Target Village radiobutton

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
                new SendResource(
                    GetSelectedVillage(),
                    _res,
                    coord,
                    DateTime.Now.AddHours(-10)));
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

        #region Resource numeric group

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

        #endregion Resource numeric group

        #region Route trade button group

        private void button3_Click(object sender, EventArgs e)
        {
            Coordinates coord;
            if (radioButton1.Checked)
            {
                if (VillageComboBox.SelectedIndex == -1)
                {
                    MessageBox.Show("Missing coordinate");
                    return;
                }
                coord = GetSelectedAcc().Villages.FirstOrDefault(x => x.Name == VillageComboBox.Items[VillageComboBox.SelectedIndex].ToString()).Coordinates;
            }
            else
            {
                coord = coordinatesUc1.Coords;
            }

            var route = new TradeRoute
            {
                Location = coord,
                Resource = _res,
                Time = (int)numericUpDown1.Value,
                TimeDelay = (int)numericUpDown2.Value,
                Active = checkBox1.Checked,
            };

            var vill = GetSelectedVillage(GetSelectedAcc());
            AddTradeRoute(vill, route);
            UpdateTradeRouteView(vill);

            GetSelectedAcc().Tasks.Add(new SendRoute(vill, DateTime.Now));
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (routeTradeList.FocusedItem == null) return;

            Coordinates coord;
            if (radioButton1.Checked)
            {
                if (VillageComboBox.SelectedIndex == -1)
                {
                    MessageBox.Show("Missing coordinate");
                    return;
                }
                coord = GetSelectedAcc().Villages.FirstOrDefault(x => x.Name == VillageComboBox.Items[VillageComboBox.SelectedIndex].ToString()).Coordinates;
            }
            else
            {
                coord = coordinatesUc1.Coords;
            }

            var route = new TradeRoute
            {
                Location = coord,
                Resource = _res,
                Time = (int)numericUpDown1.Value,
                TimeDelay = (int)numericUpDown2.Value,
                Active = checkBox1.Checked,
            };
            var vill = GetSelectedVillage(GetSelectedAcc());
            UpdateTradeRoute(vill, route, routeTradeList.FocusedItem.Index);
            UpdateTradeRouteView(vill);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (routeTradeList.FocusedItem == null) return;

            var vill = GetSelectedVillage(GetSelectedAcc());
            RemoveTradeRoute(vill, routeTradeList.FocusedItem.Index);
            UpdateTradeRouteView(vill);
        }

        #endregion Route trade button group

        private void routeTradeList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (routeTradeList.FocusedItem == null) return;
            var vill = GetSelectedVillage(GetSelectedAcc());
            var route = vill.Market.TradeRoute.TradeRoutes[routeTradeList.FocusedItem.Index];

            radioButton2.Checked = true;
            coordinatesUc1.Coords = route.Location;

            numericUpDown9.Value = route.Resource.Wood;
            numericUpDown8.Value = route.Resource.Clay;
            numericUpDown7.Value = route.Resource.Iron;
            numericUpDown6.Value = route.Resource.Crop;

            numericUpDown2.Value = route.TimeDelay;
            numericUpDown1.Value = route.Time;
            checkBox1.Checked = route.Active;
        }
    }
}