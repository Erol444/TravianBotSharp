using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.AccModels;
using TravBotSharp.Files.Models.Settings;
using TravBotSharp.Files.Tasks.LowLevel;
using XPTable.Editors;
using XPTable.Models;

namespace TravBotSharp.Views
{
    public partial class OverviewUc : UserControl
    {
        TableModel tableModelMain = new TableModel();
        TableModel tableModelGlobal = new TableModel();
        ControlPanel main;
        public OverviewUc()
        {
            InitializeComponent();
        }
        public void Init(ControlPanel _main)
        {
            main = _main;
        }
        private Account getSelectedAcc()
        {
            return main?.GetSelectedAcc();
        }
        internal void UpdateOverviewTab()
        {
            var acc = getSelectedAcc();
            if (acc.Villages.Count == 0) return;

            InitTable();
            InitGlobalTable();

            tableModelMain.Rows.Clear();
            foreach (var vill in acc.Villages)
            {
                var r = new Row();
                r.Cells.Add(new Cell(vill.Id.ToString())); //vill id
                r.Cells.Add(new Cell(vill.Name)); //vill name
                r.Cells.Add(new Cell(vill.Settings.Type.ToString())); //vill type
                r.Cells.Add(new Cell(vill.Settings.BarracksTrain.ToString())); //barracks training
                r.Cells.Add(new Cell("", vill.Settings.GreatBarracksTrain)); //GB
                r.Cells.Add(new Cell(vill.Settings.StableTrain.ToString())); //stable training
                r.Cells.Add(new Cell("", vill.Settings.GreatStableTrain)); //GS
                r.Cells.Add(new Cell(vill.Settings.WorkshopTrain.ToString())); //workshop training
                r.Cells.Add(new Cell("", vill.Settings.GetRes)); //Get resources from
                r.Cells.Add(new Cell("", vill.Settings.SendRes)); //Send resources to
                tableModelMain.Rows.Add(r);
            }
        }

        private void InitGlobalTable()
        {
            XpTableGlobal.TableModel = tableModelGlobal;
            var acc = getSelectedAcc();
            var vill = acc.Villages.FirstOrDefault();
            var newVills = acc.NewVillages.DefaultSettings;

            tableModelGlobal.Rows.Clear();
            // Change multiple row
            var r = new Row();
            r.Cells.Add(new Cell("")); //vill id
            r.Cells.Add(new Cell("CHANGE MULTIPLE")); //vill name
            r.Cells.Add(new Cell(vill.Settings.Type.ToString())); //vill type
            r.Cells.Add(new Cell(vill.Settings.BarracksTrain.ToString())); //barracks training
            r.Cells.Add(new Cell("", vill.Settings.GreatBarracksTrain)); //GB
            r.Cells.Add(new Cell(vill.Settings.StableTrain.ToString())); //stable training
            r.Cells.Add(new Cell("", vill.Settings.GreatStableTrain)); //GS
            r.Cells.Add(new Cell(vill.Settings.WorkshopTrain.ToString())); //workshop training
            r.Cells.Add(new Cell("", vill.Settings.GetRes)); //Get resources from
            r.Cells.Add(new Cell("", vill.Settings.SendRes)); //Send resources to
            tableModelGlobal.Rows.Add(r);

            var r1 = new Row();
            r1.Cells.Add(new Cell("")); //vill id
            r1.Cells.Add(new Cell("NEW VILLAGES")); //vill name
            r1.Cells.Add(new Cell(newVills.Type.ToString())); //vill type
            r1.Cells.Add(new Cell(newVills.BarracksTrain.ToString())); //barracks training
            r1.Cells.Add(new Cell("", newVills.GreatBarracksTrain)); //GB
            r1.Cells.Add(new Cell(newVills.StableTrain.ToString())); //stable training
            r1.Cells.Add(new Cell("", newVills.GreatStableTrain)); //GS
            r1.Cells.Add(new Cell(newVills.WorkshopTrain.ToString())); //workshop training
            r1.Cells.Add(new Cell("", newVills.GetRes)); //Get resources from
            r1.Cells.Add(new Cell("", newVills.SendRes)); //Send resources to
            tableModelGlobal.Rows.Add(r);
        }
        #region Initialize table columns
        private void InitTable()
        {
            ColumnModel columnModel = new ColumnModel();

            // set the Table's ColumModel and TableModel
            table1.ColumnModel = columnModel;
            XpTableGlobal.ColumnModel = columnModel;

            table1.TableModel = tableModelMain;

            //VillageId
            TextColumn villId = new TextColumn
            {
                Editable = false,
                Text = "Id",
                ToolTipText = "Village Id",
                Width = 40
            };
            columnModel.Columns.Add(villId);
            //Village name
            TextColumn vill = new TextColumn
            {
                Editable = true,
                Text = "Village",
                ToolTipText = "Village name",
                Width = 120
            };
            columnModel.Columns.Add(vill);

            //Village type
            ComboBoxColumn typeColumn = new ComboBoxColumn
            {
                Text = "Type",
                ToolTipText = "Type of the village",
                Width = 100
            };

            ComboBoxCellEditor typeEditor = new ComboBoxCellEditor
            {
                DropDownStyle = DropDownStyle.DropDownList
            };
            typeEditor.Items.AddRange(new string[] { "Farm", "Support", "Deff", "Off" });
            typeColumn.Editor = typeEditor;

            columnModel.Columns.Add(typeColumn);

            //Village barracks trainign
            ComboBoxColumn barracks = new ComboBoxColumn
            {
                Text = "Barracks",
                ToolTipText = "Troops to train in Barracks",
                Width = 100
            };

            ComboBoxCellEditor barracksEditor = new ComboBoxCellEditor
            {
                DropDownStyle = DropDownStyle.DropDownList
            };
            barracksEditor.Items.AddRange(GetPossibleTroops(Classificator.BuildingEnum.Barracks));
            barracks.Editor = barracksEditor;

            columnModel.Columns.Add(barracks);

            //great barracks training
            CheckBoxColumn GB = new CheckBoxColumn
            {
                Text = "GB",
                ToolTipText = "Train troops in Great Barracks",
                Width = 40
            };
            columnModel.Columns.Add(GB);

            //stable
            ComboBoxColumn stable = new ComboBoxColumn
            {
                Text = "Stable",
                ToolTipText = "Troops to train in Stable",
                Width = 100
            };

            ComboBoxCellEditor stableEditor = new ComboBoxCellEditor
            {
                DropDownStyle = DropDownStyle.DropDownList
            };
            stableEditor.Items.AddRange(GetPossibleTroops(Classificator.BuildingEnum.Stable));
            stable.Editor = stableEditor;

            columnModel.Columns.Add(stable);
            //great stable
            CheckBoxColumn GS = new CheckBoxColumn
            {
                Text = "GS",
                ToolTipText = "Train troops in Great Stable",
                Width = 40
            };
            columnModel.Columns.Add(GS);
            //workshop
            ComboBoxColumn workshop = new ComboBoxColumn
            {
                Text = "Workshop",
                ToolTipText = "Troops to train in Workshop",
                Width = 100
            };

            ComboBoxCellEditor workshopEditor = new ComboBoxCellEditor
            {
                DropDownStyle = DropDownStyle.DropDownList
            };
            workshopEditor.Items.AddRange(GetPossibleTroops(Classificator.BuildingEnum.Workshop));
            workshop.Editor = workshopEditor;

            columnModel.Columns.Add(workshop);
            //get resources for building
            CheckBoxColumn GetRes = new CheckBoxColumn
            {
                Text = "Get Res",
                Width = 75,
                ToolTipText = "Select the supplying village when there are not enough resources"
            };
            columnModel.Columns.Add(GetRes);
            //get resources for troops
            CheckBoxColumn SendRes = new CheckBoxColumn
            {
                Text = "Send Res",
                Width = 80,
                ToolTipText = "Select where to send resources when too many"
            };
            columnModel.Columns.Add(SendRes);
        }
        #endregion
        private string[] GetPossibleTroops(Classificator.BuildingEnum building)
        {
            List<string> ret = new List<string>();
            ret.Add("None");
            var acc = getSelectedAcc();
            if (acc.Villages.Count == 0) return ret.ToArray(); //Acc has now been initialised
            int troopsEnum = ((int)acc.AccInfo.Tribe - 1) * 10;
            for (var i = troopsEnum + 1; i < troopsEnum + 11; i++)
            {
                Classificator.TroopsEnum troop = (Classificator.TroopsEnum)i;
                if (TroopsHelper.GetTroopBuilding(troop, false) == building)
                {
                    ret.Add(VillageHelper.EnumStrToString(troop.ToString()));
                }
            }
            return ret.ToArray();
        }

        //Save button
        private void button1_Click(object sender, EventArgs e)
        {
            var acc = getSelectedAcc();
            //change vill names list
            var changeVillNames = new List<(int, string)>();
            for (int i = 0; i < tableModelMain.Rows.Count; i++)
            {
                var cells = tableModelMain.Rows[i].Cells;
                int column = 0;
                //Village id
                var id = Int32.Parse(cells[column].Text);
                var vill = acc.Villages.First(x => x.Id == id);

                //check if name is different. if it is, change the name
                var name = cells[++column].Text;
                if (name != vill.Name)
                {
                    changeVillNames.Add((id, name));
                }
                column++;
                UpdateVillageType(vill, cells, column);
                column++;
                UpdateBarracks(acc, vill, cells, column);
                column++;
                UpdateGB(acc, vill, cells, column);
                column++;
                UpdateStable(acc, vill, cells, column);
                column++;
                UpdateGS(acc, vill, cells, column);
                column++;
                UpdateWorkshop(acc, vill, cells, column);
                column++;
                vill.Settings.GetRes = cells[column].Checked;
                column++;
                vill.Settings.SendRes = cells[column].Checked;

                // Reset training
                if (!TroopsHelper.EverythingFilled(acc, vill)) TroopsHelper.ReStartTroopTraining(acc, vill);
            }
            //Change name of village/s
            if (changeVillNames.Count > 0)
            {
                TaskExecutor.AddTaskIfNotExists(acc,
                        new ChangeVillageName()
                        {
                            ExecuteAt = DateTime.Now,
                            ChangeList = changeVillNames
                        });
            }
        }

        private void UpdateBarracks(Account acc, Village vill, CellCollection cells, int column)
        {
            var text = cells[column].Text;
            var troop = (Classificator.TroopsEnum)Enum.Parse(typeof(Classificator.TroopsEnum), text.Replace(" ", ""));
            if (troop == vill.Settings.BarracksTrain) return; //no difference

            vill.Settings.BarracksTrain = troop;
            TroopsHelper.ReStartResearchAndImprovement(acc, vill);
        }
        private void UpdateStable(Account acc, Village vill, CellCollection cells, int column)
        {
            var text = cells[column].Text;
            var troop = (Classificator.TroopsEnum)Enum.Parse(typeof(Classificator.TroopsEnum), text.Replace(" ", ""));
            if (troop == vill.Settings.StableTrain) return; //no difference

            vill.Settings.StableTrain = troop;
            TroopsHelper.ReStartResearchAndImprovement(acc, vill);
        }
        private void UpdateWorkshop(Account acc, Village vill, CellCollection cells, int column)
        {
            var text = cells[column].Text;
            var troop = (Classificator.TroopsEnum)Enum.Parse(typeof(Classificator.TroopsEnum), text.Replace(" ", ""));
            if (troop == vill.Settings.WorkshopTrain) return; //no difference

            vill.Settings.WorkshopTrain = troop;
            TroopsHelper.ReStartResearchAndImprovement(acc, vill);
        }
        private void UpdateGB(Account acc, Village vill, CellCollection cells, int column)
        {
            bool enabled = cells[column].Checked;
            if (vill.Settings.GreatBarracksTrain == enabled) return; //no difference

            vill.Settings.GreatBarracksTrain = enabled;
        }
        private void UpdateGS(Account acc, Village vill, CellCollection cells, int column)
        {
            bool enabled = cells[column].Checked;
            if (vill.Settings.GreatStableTrain == enabled) return; //no difference

            vill.Settings.GreatStableTrain = enabled;
        }
        private void UpdateVillageType(Village vill, CellCollection cells, int column)
        {
            var acc = getSelectedAcc();
            var type = (VillType)Enum.Parse(typeof(VillType), cells[column].Text);
            if (type != vill.Settings.Type)
            {
                vill.Settings.Type = type;
                //User just selected different Village Type
                switch (type)
                {
                    case VillType.Farm:
                        DefaultConfigurations.FarmVillagePlan(acc, vill);
                        return;
                    case VillType.Support:
                        DefaultConfigurations.SupplyVillagePlan(acc, vill);
                        return;
                    case VillType.Deff:
                        DefaultConfigurations.DeffVillagePlan(acc, vill);
                        return;
                    case VillType.Off:
                        DefaultConfigurations.OffVillagePlan(acc, vill);
                        return;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            List<CellChanges> selectedCells = new List<CellChanges>();
            for (int i = 0; i < tableModelMain.Rows.Count; i++)
            {
                for (int y = 0; y < tableModelMain.Rows[i].SelectedItems.Count(); y++)
                {
                    selectedCells.Add(new CellChanges()
                    {
                        Cell = tableModelMain.Rows[i].SelectedItems[y],
                        Num = tableModelMain.Rows[i].SelectedIndicies[y]
                    });
                }
            }

            foreach (var selectedCell in selectedCells)
            {
                var global = tableModelGlobal.Rows[0].Cells[selectedCell.Num];
                selectedCell.Cell.Text = global.Text;
                selectedCell.Cell.Checked = global.Checked;
            }
        }
        private class CellChanges
        {
            public Cell Cell { get; set; }
            public int Num { get; set; }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            var acc = getSelectedAcc();

            string location = IoHelperForms.PromptUserForBuidTasksLocation();
            if (location == null) return;

            for (int i = 0; i < tableModelMain.Rows.Count; i++)
            {
                if (tableModelMain.Rows[i].SelectedItems.Count() > 0)
                {
                    var cells = tableModelMain.Rows[i].Cells;
                    //Village id
                    var id = Int32.Parse(cells[0].Text);
                    var vill = acc.Villages.First(x => x.Id == id);

                    IoHelperCore.AddBuildTasksFromFile(acc, vill, location);
                }
            }

        }

        private void button3_Click(object sender, EventArgs e) // Save new village settings
        {

        }
    }
}
