using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;
using TbsCore.Models.VillageModels;
using TbsCore.Tasks.LowLevel;
using TravBotSharp.Interfaces;

namespace TravBotSharp.Views
{
    public partial class VillagesUc : TbsBaseUc, ITbsUc
    {
        private int villSelected = 0;

        private ITbsUc[] Ucs;

        public VillagesUc()
        {
            InitializeComponent();

            // Be sure to have these in correct order!
            Ucs = new ITbsUc[]
            {
                buildUc1,
                marketUc1,
                troopsUc1,
                attackUc1,
                farmingNonGoldUc1,
                infoUc1,
            };

            // Initialize all the views
            foreach (var uc in Ucs) uc.Init(this);
        }

        // To satisfy Interface
        public void UpdateUc() => UpdateUc(true);

        private void UpdateUc(bool updateVillList = true)
        {
            var acc = GetSelectedAcc();

            if (villSelected < 0 || acc.Villages.Count <= villSelected)
                villSelected = acc.Villages.Count - 1;

            if (updateVillList)
            {
                VillagesListView.Items.Clear();
                for (int i = 0; i < acc.Villages.Count; i++) // Update villages list
                {
                    var item = new ListViewItem();
                    item.SubItems[0].Text = acc.Villages[i].Name;
                    item.SubItems[0].ForeColor = Color.FromName(villSelected == i ? "DodgerBlue" : "Black");
                    item.SubItems.Add(acc.Villages[i].Coordinates.x + "/" + acc.Villages[i].Coordinates.y); //coords
                    item.SubItems.Add(VillageHelper.VillageType(acc.Villages[i])); //type (resource)
                    item.SubItems.Add(VillageHelper.ResourceIndicator(acc.Villages[i])); //resources count
                    VillagesListView.Items.Add(item);
                }
            }

            // Don't update village view if there is no village selected!
            if (GetSelectedVillage() == null) return;
            Ucs.ElementAtOrDefault(villageTabController.SelectedIndex)?.UpdateUc();
        }

        public Village GetSelectedVillage(Account acc = null)
        {
            if (acc == null) acc = GetSelectedAcc();
            // Some error. Refresh acc list view, maybe this will help.
            if (villSelected >= acc.Villages.Count)
            {
                main.RefreshAccView();
                return null;
            }
            return acc.Villages.ElementAtOrDefault(villSelected);
        }

        private void VillagesListView_SelectedIndexChanged(object sender, EventArgs e) //update building tab if its selected
        {
            var indicies = VillagesListView.SelectedIndices;
            if (indicies.Count > 0)
                villSelected = indicies[0];
            UpdateUc(true);
        }

        private void RefreshVill_Click(object sender, EventArgs e) // Refresh selected village
        {
            var acc = GetSelectedAcc();
            var vill = GetSelectedVillage(acc);
            RefreshVillage(acc, vill);
        }

        private void RefreshAllVills_Click(object sender, EventArgs e) // Refresh all villages
        {
            var acc = GetSelectedAcc();
            acc.Villages.ForEach(x => RefreshVillage(acc, x));
        }

        // villageTabController tab changed event
        private void villageTabController_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateUc();
        }

        private void RefreshVillage(Account acc, Village vill) // Refresh village
        {
            acc.Tasks.Add(new UpdateVillage()
            {
                ExecuteAt = DateTime.Now.AddHours(-1),
                Vill = vill,
                ImportTasks = false
            });
        }
    }
}