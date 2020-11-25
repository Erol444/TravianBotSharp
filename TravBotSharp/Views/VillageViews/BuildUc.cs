using Newtonsoft.Json;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.AccModels;
using TravBotSharp.Files.Models.ResourceModels;
using TravBotSharp.Files.Tasks;
using TravBotSharp.Files.Tasks.Building;
using TravBotSharp.Files.Tasks.LowLevel;
using TravBotSharp.Interfaces;

namespace TravBotSharp.Views
{
    public partial class BuildUc : BaseVillageUc, ITbsUc
    {
        Building selectedBuilding;
        public BuildUc()
        {
            InitializeComponent();
        }
        public void UpdateUc()
        {
            Account acc = GetSelectedAcc();
            var vill = GetSelectedVillage();

            if (vill == null) return;

            RefreshBuildingsList(vill);

            //Building Tasks ListView
            UpdateBuildTasks(vill);

            foreach (var task in vill.Build.DemolishTasks)
            {
                var item = new ListViewItem();
                //building
                item.SubItems[0].Text = "Demolish " + VillageHelper.BuildingTypeToString(vill.Build.Buildings.FirstOrDefault(x => x.Id == task.BuildingId).Type);

                item.SubItems.Add(task.Level.ToString()); //lvl
                item.SubItems.Add(task.BuildingId.ToString()); //buildingId
                buildListView.Items.Add(item);
            }

            //Currently building ListView
            currentlyBuildinglistView.Items.Clear();
            foreach (var building in vill.Build.CurrentlyBuilding)
            {
                var item = new ListViewItem();
                item.SubItems[0].Text = building.Building.ToString();
                item.SubItems.Add(building.Level.ToString()); //lvl
                item.SubItems.Add(building.Duration == DateTime.MinValue ? "/" : building.Duration.ToString()); //execute at
                currentlyBuildinglistView.Items.Add(item);
            }
            autoBuildResType.SelectedIndex = 0;
            autoBuildResStrat.SelectedIndex = 0;
            autoBuildResLevel.Value = 10;

            AutoBuildBonusBuildings.Checked = vill.Build.AutoBuildResourceBonusBuildings;
            buildTypeComboBox.Enabled = false;

            buildRadioButton.Checked = true;
            instaUpgradeUpDown.Enabled = vill.Build.InstaBuild;
            instaUpgradeUpDown.Value = vill.Build.InstaBuildHours;
        }

        private void UpdateBuildTasks(Village vill)
        {
            buildListView.Items.Clear();

            for (int i = 0; i < vill.Build.Tasks.Count; i++)
            {
                var task = vill.Build.Tasks[i];
                var item = new ListViewItem();
                //building
                if (task.TaskType == BuildingHelper.BuildingType.AutoUpgradeResFields)
                {
                    item.SubItems[0].Text = AutoBuildResFieldsStr(task);
                }
                else item.SubItems[0].Text = VillageHelper.BuildingTypeToString(task.Building);

                item.ForeColor = Color.FromName(oldSelected == i ? "DodgerBlue" : "Black");

                item.SubItems.Add(task.Level.ToString()); //lvl
                item.SubItems.Add(task.BuildingId.ToString()); //buildingId

                //var upgradeTask = acc.Tasks?.FirstOrDefault(x =>
                //    x.GetType() == typeof(UpgradeBuilding)
                //    && ((UpgradeBuilding)x).Task == task
                //    );

                //item.SubItems.Add(upgradeTask == null ? "" : upgradeTask.ExecuteAt.ToString()); //execute at
                buildListView.Items.Add(item);
            }
        }

        private string AutoBuildResFieldsStr(BuildingTask task)
        {
            var str = "";
            switch (task.ResourceType)
            {
                case ResTypeEnum.AllResources:
                    str += "All fields";
                    break;
                case ResTypeEnum.ExcludeCrop:
                    str += "Exclude crop";
                    break;
                case ResTypeEnum.OnlyCrop:
                    str += "Only crop";
                    break;
            }
            str += "-";
            switch (task.BuildingStrategy)
            {
                case BuildingStrategyEnum.BasedOnLevel:
                    str += "Based on level";
                    break;
                case BuildingStrategyEnum.BasedOnProduction:
                    str += "Based on production";
                    break;
                case BuildingStrategyEnum.BasedOnRes:
                    str += "Based on storage";
                    break;
            }
            return str;
        }

        private void RefreshBuildingsList(Village vill)
        {
            //update buildings list view
            buildingsList.Items.Clear();
            //19, 38);
            for (int i = 0; i <= 39; i++)
            {
                var building = vill.Build.Buildings[i];
                var item = new ListViewItem();
                var id = building.Id;

                item.SubItems[0].Text = id.ToString();//id
                string buildingName = VillageHelper.BuildingTypeToString(building.Type);
                //if there is a task for upgrading/construction the building on this site
                string upgradeLvl = "";

                var upgradeBuilding = vill.Build.Tasks.LastOrDefault(x => x.BuildingId == id);
                if (upgradeBuilding != null)
                {
                    upgradeLvl = " -> " + upgradeBuilding.Level;
                    if (buildingName == "Site") buildingName = VillageHelper.BuildingTypeToString(upgradeBuilding.Building);
                }
                item.SubItems.Add(buildingName); //building

                item.SubItems.Add(building.Level.ToString() + upgradeLvl); //level
                //set color
                switch(building.Type)
                {
                    case Classificator.BuildingEnum.Woodcutter:
                        item.ForeColor = Color.LightGreen;
                        break;
                    case Classificator.BuildingEnum.ClayPit:
                        item.ForeColor = Color.Orange;
                        break;
                    case Classificator.BuildingEnum.IronMine:
                        item.ForeColor = Color.Gray;
                        break;
                    case Classificator.BuildingEnum.Cropland:
                        item.ForeColor = Color.Yellow;
                        break;
                    case Classificator.BuildingEnum.Site:
                        item.ForeColor = (buildingName == "Site" ? Color.White : Color.LightBlue);
                        break;
                    default:
                        item.ForeColor = Color.GreenYellow;
                        break;
                }
                buildingsList.Items.Add(item);
            }
        }

        private int oldSelected = 0;
        private BuildingTask GetSelectedBuildingTask()
        {
            var vill = GetSelectedVillage();
            var tasks = vill.Build.Tasks;
            if (tasks.Count == 0) return null;

            return (oldSelected >= 0 && oldSelected < tasks.Count) ? tasks[oldSelected] : tasks[0];
        }

        private void buildButton_Click(object sender, EventArgs e) //build button
        {
            var acc = GetSelectedAcc();
            var vill = GetSelectedVillage(acc);

            var indicies = buildingsList.SelectedIndices;
            if (indicies.Count > 0)
                selectedBuilding = vill.Build.Buildings[indicies[0]];
            else return; //no building selected :(
            if (buildRadioButton.Checked)
            {
                var task = new BuildingTask
                {
                    TaskType = BuildingHelper.BuildingType.General,
                    Level = (int)buildLevelUpDown.Value,
                    BuildingId = selectedBuilding.Id
                };

                //Create building task, construct new building
                if (selectedBuilding.Type == Classificator.BuildingEnum.Site)
                {
                    Enum.TryParse(buildTypeComboBox.SelectedItem.ToString(), out Classificator.BuildingEnum building);
                    task.Building = building;
                    task.ConstructNew = true;
                }
                else //upgrade existing building
                {
                    task.Building = selectedBuilding.Type;
                }
                BuildingHelper.AddBuildingTask(acc, vill, task);
            }
            else if (demolishRadioButton.Checked)
            {
                DemolishTask dt = new DemolishTask
                {
                    Building = selectedBuilding.Type,
                    BuildingId = selectedBuilding.Id,
                    Level = (int)buildLevelUpDown.Value
                };
                vill.Build.DemolishTasks.Add(dt);
                //TODO: ReStartDemolish
                TaskExecutor.AddTaskIfNotExistInVillage(acc, vill, new DemolishBuilding() { ExecuteAt = DateTime.Now, Vill = vill });
            }
            UpdateUc();
        }

        private void button23_Click(object sender, EventArgs e) //move top build task
        {
            var vill = GetSelectedVillage();
            var task = GetSelectedBuildingTask();
            var index = vill.Build.Tasks.IndexOf(task);
            if (index == 0) return;
            vill.Build.Tasks.Remove(task);
            vill.Build.Tasks.Insert(0, task);
            oldSelected = 0;
            UpdateBuildTasks(vill);
        }

        private void button22_Click(object sender, EventArgs e) //move bottom build task
        {
            var vill = GetSelectedVillage();
            var task = GetSelectedBuildingTask();
            var index = vill.Build.Tasks.IndexOf(task);
            if (index == vill.Build.Tasks.Count - 1) return; //already at the bottom
            vill.Build.Tasks.Remove(task);
            vill.Build.Tasks.Add(task);
            oldSelected = vill.Build.Tasks.Count - 1;
            UpdateBuildTasks(vill);
        }

        private void button11_Click(object sender, EventArgs e) //move up build task
        {
            var vill = GetSelectedVillage();
            var task = GetSelectedBuildingTask();
            var index = vill.Build.Tasks.IndexOf(task);
            if (index == 0) return;
            vill.Build.Tasks.Remove(task);
            vill.Build.Tasks.Insert(index - 1, task);
            oldSelected = index - 1;
            UpdateBuildTasks(vill);
        }

        private void button12_Click(object sender, EventArgs e) //move down build task
        {
            var vill = GetSelectedVillage();
            var task = GetSelectedBuildingTask();
            var index = vill.Build.Tasks.IndexOf(task);
            if (index == vill.Build.Tasks.Count - 1) return;
            vill.Build.Tasks.Remove(task);
            vill.Build.Tasks.Insert(index + 1, task);
            oldSelected = index + 1;
            UpdateBuildTasks(vill);
        }

        private void button13_Click(object sender, EventArgs e) //delete build task
        {
            var vill = GetSelectedVillage();
            var task = GetSelectedBuildingTask();
            var index = vill.Build.Tasks.Remove(task);
            UpdateUc();
        }

        private void button8_Click(object sender, EventArgs e) //clear all build tasks
        {
            var vill = GetSelectedVillage();
            vill.Build.Tasks.Clear();
            buildListView.Items.Clear();
            vill.Build.DemolishTasks.Clear();
            UpdateUc();
        }

        private void AutBuildResButton_Click(object sender, EventArgs e) //auto res build button
        {
            var acc = GetSelectedAcc();
            var vill = GetSelectedVillage(acc);

            var task = new BuildingTask
            {
                TaskType = BuildingHelper.BuildingType.AutoUpgradeResFields,
                Level = (int)autoBuildResLevel.Value,
                ResourceType = (ResTypeEnum)autoBuildResType.SelectedIndex,
                BuildingStrategy = (BuildingStrategyEnum)autoBuildResStrat.SelectedIndex
            };
            BuildingHelper.AddBuildingTask(acc, vill, task);
            UpdateUc();
        }

        private void button20_Click(object sender, EventArgs e) //support vill button
        {
            var acc = GetSelectedAcc();
            var vill = GetSelectedVillage();
            DefaultConfigurations.SupplyVillagePlan(acc, vill);
            BuildingHelper.RemoveCompletedTasks(vill, acc);
            UpdateUc();
        }

        private void button19_Click(object sender, EventArgs e) //off vill button
        {
            var acc = GetSelectedAcc();
            var vill = GetSelectedVillage();
            DefaultConfigurations.OffVillagePlan(acc, vill);
            BuildingHelper.RemoveCompletedTasks(vill, acc);
            UpdateUc();
        }

        private void button6_Click(object sender, EventArgs e) //deff vill button
        {
            var acc = GetSelectedAcc();
            var vill = GetSelectedVillage();
            DefaultConfigurations.DeffVillagePlan(acc, vill);
            BuildingHelper.RemoveCompletedTasks(vill, acc);
            UpdateUc();
        }

        private void button9_Click(object sender, EventArgs e) //export build tasks button
        {
            //remove buildingIds, they will get choosen appropriately when imported
            var buildTasks = GetSelectedVillage().Build.Tasks;
            buildTasks.ToList().ForEach(x => x.BuildingId = null);
            IoHelperForms.ExportBuildTasks(JsonConvert.SerializeObject(buildTasks));
        }

        private void button24_Click(object sender, EventArgs e) //import build tasks button
        {
            var acc = GetSelectedAcc();
            var vill = GetSelectedVillage();

            string location = IoHelperForms.PromptUserForBuidTasksLocation();
            if (location == null) return;
            IoHelperCore.AddBuildTasksFromFile(acc, vill, location);
            UpdateUc();
        }

        private void AutoBuildBonusBuildings_CheckedChanged(object sender, EventArgs e)
        {
            var vill = GetSelectedVillage();
            vill.Build.AutoBuildResourceBonusBuildings = AutoBuildBonusBuildings.Checked;
        }

        private void buildingsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedBuilding = null;
            var acc = GetSelectedAcc();
            var vill = GetSelectedVillage();

            var indicies = buildingsList.SelectedIndices;

            if (indicies.Count > 0) selectedBuilding = vill.Build.Buildings[indicies[0]];
            else return;

            // Check if there is already a building planner for that id
            var planedBuilding = vill.Build.Tasks.LastOrDefault(x => x.BuildingId == selectedBuilding.Id);

            // Building level selector
            if (selectedBuilding.Type != Classificator.BuildingEnum.Site) buildLevelUpDown.Value = selectedBuilding.Level + 1;
            else if (planedBuilding != null) buildLevelUpDown.Value = planedBuilding.Level + 1;
            else buildLevelUpDown.Value = 1;

            //construct new building
            buildTypeComboBox.Items.Clear();

            buildTypeComboBox.Enabled = false;
            if (selectedBuilding.Type == Classificator.BuildingEnum.Site)
            {
                if (planedBuilding != null)
                {
                    buildTypeComboBox.Items.Add(planedBuilding.Building.ToString());
                    buildTypeComboBox.SelectedIndex = 0;
                    return;
                }

                buildTypeComboBox.Enabled = true;
                for (int i = 5; i <= 45; i++)
                {
                    if (BuildingHelper.BuildingRequirementsAreMet((Classificator.BuildingEnum)i, vill, acc.AccInfo.Tribe ?? Classificator.TribeEnum.Natars))
                    {
                        buildTypeComboBox.Items.Add(((Classificator.BuildingEnum)i).ToString());
                    }
                }
            }
            else // Building already there
            {
                buildTypeComboBox.Items.Add(selectedBuilding.Type.ToString());
            }
            if (buildTypeComboBox.Items.Count > 0) buildTypeComboBox.SelectedIndex = 0;
        }

        private void demolishRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            buildLevelUpDown.Value = 0;
        }

        private void button1_Click(object sender, EventArgs e) //refreshes building list
        {
            UpdateUc(); //just update whole tab
        }

        private void buildListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            var indicies = buildListView.SelectedIndices;
            if (indicies.Count > 0)
            {
                oldSelected = indicies[0];
            }
            UpdateBuildTasks(GetSelectedVillage());
        }

        private void instaUpgradeCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            var vill = GetSelectedVillage();
            vill.Build.InstaBuild = instaUpgradeCheckbox.Checked;
            instaUpgradeUpDown.Enabled = instaUpgradeCheckbox.Checked;
        }

        private void instaUpgradeUpDown_ValueChanged(object sender, EventArgs e)
        {
            var vill = GetSelectedVillage();
            vill.Build.InstaBuildHours = (int)instaUpgradeUpDown.Value;
        }
    }
}
