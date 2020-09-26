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

namespace TravBotSharp.Views
{
    public partial class BuildUc : UserControl
    {
        Building selectedBuilding;
        ControlPanel main;
        public BuildUc()
        {
            InitializeComponent();
        }
        public void UpdateBuildTab()
        {
            Account acc = getSelectedAcc();
            var vill = getSelectedVillage();

            if (vill == null) return;

            RefreshBuildingsList(vill);

            //Building Tasks ListView
            buildListView.Items.Clear();

            foreach (var task in vill.Build.Tasks)
            {
                var item = new ListViewItem();
                //building
                if (task.TaskType == BuildingHelper.BuildingType.AutoUpgradeResFields)
                {
                    item.SubItems[0].Text = AutoBuildResFieldsStr(task);
                }
                else item.SubItems[0].Text = VillageHelper.BuildingTypeToString(task.Building);

                item.SubItems.Add(task.Level.ToString()); //lvl
                item.SubItems.Add(task.BuildingId.ToString()); //buildingId
                item.SubItems.Add(""); // Selected task

                var upgradeTask = acc.Tasks?.FirstOrDefault(x =>
                    x.GetType() == typeof(UpgradeBuilding)
                    && ((UpgradeBuilding)x).Task == task
                    );

                item.SubItems.Add(upgradeTask == null ? "" : upgradeTask.ExecuteAt.ToString()); //execute at
                buildListView.Items.Add(item);
            }
            UpdateCurrentlySelected(vill);
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
        }

        private void UpdateCurrentlySelected(Village vill)
        {
            if (oldSelected >= 0 && oldSelected < vill.Build.Tasks.Count)
            {
                for (int i = 0; i < buildListView.Items.Count; i++)
                {
                    buildListView.Items[i].SubItems[4].Text = (oldSelected == i) ? "⨞" : "";
                }
            }
        }

        private string AutoBuildResFieldsStr(BuildingTask task)
        {
            var str = "Auto-upgrade Res Fields - ";
            switch (task.ResourceType)
            {
                case ResTypeEnum.AllResources:
                    str += "All resources - ";
                    break;
                case ResTypeEnum.ExcludeCrop:
                    str += "Exclude cropland - ";
                    break;
                case ResTypeEnum.OnlyCrop:
                    str += "Only cropland - ";
                    break;
            }
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

        public void Init(ControlPanel _main)
        {
            main = _main;
        }
        public Village getSelectedVillage(Account acc = null)
        {
            return main != null ? main.GetSelectedVillage(acc) : null;
        }
        public Account getSelectedAcc()
        {
            return main != null ? main.GetSelectedAcc() : null;
        }

        private int oldSelected = 0;
        private BuildingTask getSelectedBuildingTask()
        {
            var vill = getSelectedVillage();
            var tasks = vill.Build.Tasks;
            if (tasks.Count == 0) return null;

            return (oldSelected >= 0 && oldSelected < tasks.Count) ? tasks[oldSelected] : tasks[0];
        }


        private void buildButton_Click(object sender, EventArgs e) //build button
        {
            var acc = getSelectedAcc();
            var vill = getSelectedVillage(acc);

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
            UpdateBuildTab();
        }

        private void button23_Click(object sender, EventArgs e) //move top build task
        {
            var vill = getSelectedVillage();
            var task = getSelectedBuildingTask();
            var index = vill.Build.Tasks.IndexOf(task);
            if (index == 0) return;
            vill.Build.Tasks.Remove(task);
            vill.Build.Tasks.Insert(0, task);
            oldSelected = 0;
            UpdateBuildTab();
        }

        private void button22_Click(object sender, EventArgs e) //move bottom build task
        {
            var vill = getSelectedVillage();
            var task = getSelectedBuildingTask();
            var index = vill.Build.Tasks.IndexOf(task);
            if (index == vill.Build.Tasks.Count - 1) return; //already at the bottom
            vill.Build.Tasks.Remove(task);
            vill.Build.Tasks.Add(task);
            oldSelected = vill.Build.Tasks.Count - 1;
            UpdateBuildTab();
        }

        private void button11_Click(object sender, EventArgs e) //move up build task
        {
            var vill = getSelectedVillage();
            var task = getSelectedBuildingTask();
            var index = vill.Build.Tasks.IndexOf(task);
            if (index == 0) return;
            vill.Build.Tasks.Remove(task);
            vill.Build.Tasks.Insert(index - 1, task);
            oldSelected = index - 1;
            UpdateBuildTab();
        }

        private void button12_Click(object sender, EventArgs e) //move down build task
        {
            var vill = getSelectedVillage();
            var task = getSelectedBuildingTask();
            var index = vill.Build.Tasks.IndexOf(task);
            if (index == vill.Build.Tasks.Count - 1) return;
            vill.Build.Tasks.Remove(task);
            vill.Build.Tasks.Insert(index + 1, task);
            oldSelected = index + 1;
            UpdateBuildTab();
        }

        private void button13_Click(object sender, EventArgs e) //delete build task
        {
            var vill = getSelectedVillage();
            var task = getSelectedBuildingTask();
            var index = vill.Build.Tasks.Remove(task);
            UpdateBuildTab();
        }

        private void button8_Click(object sender, EventArgs e) //clear all build tasks
        {
            var vill = getSelectedVillage();
            vill.Build.Tasks.Clear();
            buildListView.Items.Clear();
            vill.Build.DemolishTasks.Clear();
            UpdateBuildTab();
        }

        private void AutBuildResButton_Click(object sender, EventArgs e) //auto res build button
        {
            var acc = getSelectedAcc();
            var vill = getSelectedVillage(acc);

            var task = new BuildingTask
            {
                TaskType = BuildingHelper.BuildingType.AutoUpgradeResFields,
                Level = (int)autoBuildResLevel.Value,
                ResourceType = (ResTypeEnum)autoBuildResType.SelectedIndex,
                BuildingStrategy = (BuildingStrategyEnum)autoBuildResStrat.SelectedIndex
            };
            BuildingHelper.AddBuildingTask(acc, vill, task);
            UpdateBuildTab();
        }

        private void button20_Click(object sender, EventArgs e) //support vill button
        {
            var acc = getSelectedAcc();
            var vill = getSelectedVillage();
            DefaultConfigurations.SupplyVillagePlan(acc, vill);
            BuildingHelper.RemoveCompletedTasks(vill, acc);
            UpdateBuildTab();
        }

        private void button19_Click(object sender, EventArgs e) //off vill button
        {
            var acc = getSelectedAcc();
            var vill = getSelectedVillage();
            DefaultConfigurations.OffVillagePlan(acc, vill);
            BuildingHelper.RemoveCompletedTasks(vill, acc);
            UpdateBuildTab();
        }

        private void button6_Click(object sender, EventArgs e) //deff vill button
        {
            var acc = getSelectedAcc();
            var vill = getSelectedVillage();
            DefaultConfigurations.DeffVillagePlan(acc, vill);
            BuildingHelper.RemoveCompletedTasks(vill, acc);
            UpdateBuildTab();
        }

        private void button9_Click(object sender, EventArgs e) //export build tasks button
        {
            //remove buildingIds, they will get choosen appropriately when imported
            var buildTasks = getSelectedVillage().Build.Tasks;
            buildTasks.ToList().ForEach(x => x.BuildingId = null);
            IoHelperForms.ExportBuildTasks(JsonConvert.SerializeObject(buildTasks));
        }

        private void button24_Click(object sender, EventArgs e) //import build tasks button
        {
            var acc = getSelectedAcc();
            var vill = getSelectedVillage();

            string location = IoHelperForms.PromptUserForBuidTasksLocation();
            if (location == null) return;
            IoHelperCore.AddBuildTasksFromFile(acc, vill, location);
            UpdateBuildTab();
        }

        private void AutoBuildBonusBuildings_CheckedChanged(object sender, EventArgs e)
        {
            var vill = getSelectedVillage();
            vill.Build.AutoBuildResourceBonusBuildings = AutoBuildBonusBuildings.Checked;
        }

        private void buildingsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedBuilding = null;
            var acc = getSelectedAcc();
            var vill = getSelectedVillage();

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
            UpdateBuildTab(); //just update whole tab
        }

        private void buildListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            var indicies = buildListView.SelectedIndices;
            if (indicies.Count > 0)
            {
                oldSelected = indicies[0];
            }
            UpdateCurrentlySelected(getSelectedVillage());
        }
    }
}
