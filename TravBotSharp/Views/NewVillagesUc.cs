using System;
using System.Windows.Forms;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models;
using TravBotSharp.Files.Models.AccModels;

namespace TravBotSharp.Views
{
    public partial class NewVillagesUc : TbsBaseUc
    {
        public NewVillagesUc()
        {
            InitializeComponent();
        }
        public void UpdateTab()
        {
            var acc = GetSelectedAcc();
            if (acc == null) return;

            BuildTasksLocationTextBox.Text = acc.NewVillages.BuildingTasksLocationNewVillage;

            //new villages coords list
            NewVillList.Items.Clear();
            if (acc.NewVillages.Locations.Count > 0)
            {
                foreach (var newvill in acc.NewVillages.Locations)
                {
                    var item = new ListViewItem();
                    item.SubItems[0].Text = newvill.coordinates.x.ToString();
                    item.SubItems.Add(newvill.coordinates.y.ToString());
                    item.SubItems.Add(newvill.Name);
                    NewVillList.Items.Add(item);
                }
            }

            //new village types to find
            villTypeView.Items.Clear();
            if (acc.NewVillages.Types.Count > 0)
            {
                foreach (var type in acc.NewVillages.Types)
                {
                    villTypeView.Items.Add(new ListViewItem(type.ToString().Replace("_", "")));
                }
            }

            valleyType.SelectedIndex = 0;
            checkBox3.Checked = acc.NewVillages.AutoSettleNewVillages;
            autoNewVillagesToSettle.Checked = acc.NewVillages.AutoFindVillages;
            villName.Text = acc.NewVillages.NameTemplate;
        }

        private void removeNewVill_Click(object sender, EventArgs e)
        {
            var indicies = NewVillList.SelectedIndices;
            if (indicies.Count > 0)
            {
                var i = indicies[0];
                NewVillList.Items.RemoveAt(i);
                GetSelectedAcc().NewVillages.Locations.RemoveAt(i);
            }
        }

        private void confirmNewVill_Click(object sender, EventArgs e)
        {
            Coordinates c = new Coordinates();
            c.x = (int)XNewVill.Value;
            c.y = (int)YNewVill.Value;
            GetSelectedAcc().NewVillages.Locations.Add(new Files.Models.VillageModels.NewVillage() { coordinates = c, Name = NewVillName.Text });
            //clear values
            XNewVill.Value = 0;
            YNewVill.Value = 0;
            NewVillName.Text = "";
            UpdateTab();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var acc = GetSelectedAcc();
            var selected = (string)valleyType.SelectedItem;
            selected = "_" + selected;
            var type = (Classificator.VillTypeEnum)Enum.Parse(typeof(Classificator.VillTypeEnum), selected);
            acc.NewVillages.Types.Add(type);
            UpdateTab();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            GetSelectedAcc().NewVillages.Types.Clear();
            UpdateTab();
        }

        private void autoNewVillagesToSettle_CheckedChanged(object sender, EventArgs e)
        {
            GetSelectedAcc().NewVillages.AutoFindVillages = autoNewVillagesToSettle.Checked;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            GetSelectedAcc().NewVillages.Locations.Clear();
            UpdateTab();
        }

        private void villName_TextChanged(object sender, EventArgs e)
        {
            GetSelectedAcc().NewVillages.NameTemplate = villName.Text;
        }

        private void Button25_Click(object sender, EventArgs e) //select building tasks for new village
        {
            string location = IoHelperForms.PromptUserForBuidTasksLocation();
            GetSelectedAcc().NewVillages.BuildingTasksLocationNewVillage = location;
            UpdateTab();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            GetSelectedAcc().NewVillages.AutoSettleNewVillages = checkBox3.Checked;
        }
    }
}
