using System;
using System.Net;
using System.Linq;
using System.Drawing;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows.Forms;

using RestSharp;

using TbsCore.Models;
using TbsCore.Models.VillageModels;
using TbsCore.Models.AccModels;

using TravBotSharp.Files.Parsers;
using TravBotSharp.Files.Helpers.InactiveFinder;

namespace TravBotSharp.Forms
{
    public partial class InactiveFinder : Form
    {
        private ListViewColumnSorter lvwColumnSorter;

        public List<Farm> InactiveFarms
        {
            get
            {
                List<Farm> result = new List<Farm>();
                foreach (ListViewItem item in InactiveList.SelectedItems)
                {
                    result.Add(new Farm()
                    {
                        Coords = MapParser.GetCoordinates(item.SubItems[2].Text),
                        Troops = troopsSelectorUc1.Troops
                    });
                }
                return result;
            }
        }

        private Account acc;

        public InactiveFinder(Account acc, string label)
        {
            InitializeComponent();

            // list view sorter
            lvwColumnSorter = new ListViewColumnSorter();
            this.InactiveList.ListViewItemSorter = lvwColumnSorter;

            this.acc = acc;
            this.flName.Text = label;

            troopsSelectorUc1.HeroEditable = false;
            troopsSelectorUc1.Init(acc.AccInfo.Tribe);

            tool.SelectedIndex = 0;

            // UI
            foreach (var vill in acc.Villages)
            {
                comboBoxVillages.Items.Add(vill.Name);
            }
            comboBoxVillages.SelectedIndex = 0;
        }

        private async void button2_Click(object sender, System.EventArgs e)
        {
            button2.Text = "Waiting ...";
            button2.Enabled = false;
            var Inactives = new List<InactiveFarm>();
            InactiveList.Items.Clear();
            switch (tool.SelectedIndex)
            {
                case 0:
                    Inactives = await TravianStatsDe.GetFarms(acc, coordinatesUc1.Coords, (int)Distance.Value);
                    break;

                case 1:
                    Inactives = await InactiveSearchIt.GetFarms(acc, coordinatesUc1.Coords, (int)Distance.Value);
                    break;
            }

            if (Inactives == null || Inactives.Count < 1)
            {
                button2.Text = "Search";
                button2.Enabled = true;

                MessageBox.Show("Maybe nothing in your input range or tool doesn't support your server", "Not found any inactive village");
                return;
            }

            for (int i = 0; i < Inactives.Count; i++)
            {
                var Inactive = Inactives[i];
                var item = new ListViewItem();

                item.SubItems[0].Text = (i + 1).ToString();
                item.SubItems.Add(Inactive.distance.ToString());
                item.SubItems.Add(Inactive.coord.ToString());
                item.SubItems.Add(Inactive.namePlayer);
                item.SubItems.Add(Inactive.nameAlly);
                item.SubItems.Add(Inactive.nameVill);
                item.SubItems.Add(Inactive.population.ToString());
                item.ForeColor = Color.White;

                InactiveList.Items.Add(item);
            }

            button2.Text = "Search";
            button2.Enabled = true;
        }

        private void comboBoxVillages_SelectedIndexChanged(object sender, EventArgs e)
        {
            var vill = acc.Villages[comboBoxVillages.SelectedIndex];
            if (vill == null) return;
            coordinatesUc1.Coords = vill.Coordinates;
        }

        private void InactiveList_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            //found at https://docs.microsoft.com/en-US/troubleshoot/dotnet/csharp/sort-listview-by-column

            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == lvwColumnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (lvwColumnSorter.Order == SortOrder.Ascending)
                {
                    lvwColumnSorter.Order = SortOrder.Descending;
                }
                else
                {
                    lvwColumnSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            this.InactiveList.Sort();
        }

        private void InactiveList_SelectedIndexChanged(object sender, EventArgs e)
        {
            countFarmChose.Text = InactiveList.SelectedItems.Count.ToString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}