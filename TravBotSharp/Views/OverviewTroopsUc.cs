using System;
using System.Collections.Generic;
using System.Linq;
using TbsCore.Models.AccModels;
using TbsCore.Models.VillageModels;
using TbsCore.Helpers;
using TbsCore.Tasks.LowLevel;
using TravBotSharp.Interfaces;
using XPTable.Editors;
using XPTable.Models;

namespace TravBotSharp.Views
{
    public partial class OverviewTroopsUc : TbsBaseUc, ITbsUc
    {
        private TableModel tableModelMain = new TableModel();
        private TableModel tableModelGlobal = new TableModel();

        public OverviewTroopsUc()
        {
            InitializeComponent();
        }

        public void UpdateUc()
        {
            var acc = GetSelectedAcc();
            if (acc.Villages.Count == 0) return;

            InitTable();
            InitGlobalTable();

            tableModelMain.Rows.Clear();
            foreach (var vill in acc.Villages)
            {
                var r = new Row();
                r.Cells.Add(new Cell(vill.Id.ToString())); //vill id
                r.Cells.Add(new Cell(vill.Name)); //vill name
                r.Cells.Add(new Cell(vill.Settings.BarracksTrain.ToString())); //barracks training
                r.Cells.Add(new Cell("", vill.Settings.GreatBarracksTrain)); //GB
                r.Cells.Add(new Cell(vill.Settings.StableTrain.ToString())); //stable training
                r.Cells.Add(new Cell("", vill.Settings.GreatStableTrain)); //GS
                r.Cells.Add(new Cell(vill.Settings.WorkshopTrain.ToString())); //workshop training
                r.Cells.Add(new Cell("", vill.Settings.AutoImprove)); // Auto-improve troops
                tableModelMain.Rows.Add(r);
            }
        }

        private void InitGlobalTable()
        {
            XpTableGlobal.TableModel = tableModelGlobal;
            var acc = GetSelectedAcc();
            var vill = acc.Villages.FirstOrDefault();

            tableModelGlobal.Rows.Clear();
            // Change multiple row
            var r = new Row();
            r.Cells.Add(new Cell("")); //vill id
            r.Cells.Add(new Cell("CHANGE MULTIPLE")); //vill name
            r.Cells.Add(new Cell(vill.Settings.BarracksTrain.ToString())); //barracks training
            r.Cells.Add(new Cell("", vill.Settings.GreatBarracksTrain)); //GB
            r.Cells.Add(new Cell(vill.Settings.StableTrain.ToString())); //stable training
            r.Cells.Add(new Cell("", vill.Settings.GreatStableTrain)); //GS
            r.Cells.Add(new Cell(vill.Settings.WorkshopTrain.ToString())); //workshop training
            r.Cells.Add(new Cell("", vill.Settings.AutoImprove)); // Auto-improve troops
            tableModelGlobal.Rows.Add(r);

            //var newVills = acc.NewVillages.DefaultSettings;
            //var r1 = new Row();
            //r1.Cells.Add(new Cell("")); //vill id
            //r1.Cells.Add(new Cell("NEW VILLAGES")); //vill name
            //r1.Cells.Add(new Cell(newVills.Type.ToString())); //vill type
            //r1.Cells.Add(new Cell(newVills.BarracksTrain.ToString())); //barracks training
            //r1.Cells.Add(new Cell("", newVills.GreatBarracksTrain)); //GB
            //r1.Cells.Add(new Cell(newVills.StableTrain.ToString())); //stable training
            //r1.Cells.Add(new Cell("", newVills.GreatStableTrain)); //GS
            //r1.Cells.Add(new Cell(newVills.WorkshopTrain.ToString())); //workshop training
            //r1.Cells.Add(new Cell("", newVills.GetRes)); //Get resources from
            //r1.Cells.Add(new Cell("", newVills.SendRes)); //Send resources to
            //r1.Cells.Add(new Cell("", false)); // Auto-celebrations
            //r1.Cells.Add(new Cell("", false)); // Big celebrations
            //tableModelGlobal.Rows.Add(r);
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

            // Auto-Improve troops
            columnModel.Columns.Add(new CheckBoxColumn
            {
                Text = "AutoImprove",
                ToolTipText = "Auto Improve troops in smithy",
                Width = 100
            });
        }

        #endregion Initialize table columns

        private string[] GetPossibleTroops(Classificator.BuildingEnum building)
        {
            List<string> ret = new List<string>();
            ret.Add("None");
            var acc = GetSelectedAcc();
            if (acc.Villages.Count == 0) return ret.ToArray(); //Acc has now been initialised

            var tribes = new List<Classificator.TribeEnum>(5);
            if (NYS.Checked)
            {
                tribes.Add(Classificator.TribeEnum.Egyptians);
                tribes.Add(Classificator.TribeEnum.Gauls);
                tribes.Add(Classificator.TribeEnum.Huns);
                tribes.Add(Classificator.TribeEnum.Romans);
                tribes.Add(Classificator.TribeEnum.Teutons);
            }
            else tribes.Add(acc.AccInfo.Tribe ?? Classificator.TribeEnum.Any);

            foreach (var tribe in tribes)
            {
                int troopsEnum = ((int)tribe - 1) * 10;
                for (var i = troopsEnum + 1; i < troopsEnum + 11; i++)
                {
                    Classificator.TroopsEnum troop = (Classificator.TroopsEnum)i;
                    if (TroopsHelper.GetTroopBuilding(troop, false) == building)
                    {
                        ret.Add(VillageHelper.EnumStrToString(troop.ToString()));
                    }
                }
            }
            return ret.ToArray();
        }

        //Save button
        private void button1_Click(object sender, EventArgs e)
        {
            var acc = GetSelectedAcc();
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
                vill.Settings.AutoImprove = cells[column].Checked;

                // Reset training
                if (!TroopsHelper.EverythingFilled(acc, vill) && acc.Tasks != null) TroopsHelper.ReStartTroopTraining(acc, vill);
            }
            //Change name of village/s
            if (0 < changeVillNames.Count && acc.Tasks != null)
            {
                acc.Tasks.Add(
                        new ChangeVillageName()
                        {
                            ExecuteAt = DateTime.Now,
                            ChangeList = changeVillNames
                        }, true);
            }
        }

        private void UpdateBarracks(Account acc, Village vill, CellCollection cells, int column)
        {
            var text = cells[column].Text;
            var troop = (Classificator.TroopsEnum)Enum.Parse(typeof(Classificator.TroopsEnum), text.Replace(" ", ""));
            if (troop == vill.Settings.BarracksTrain) return; //no difference
            vill.Settings.BarracksTrain = troop;

            if (acc.Wb == null) return;
            TroopsHelper.ReStartResearchAndImprovement(acc, vill);
        }

        private void UpdateStable(Account acc, Village vill, CellCollection cells, int column)
        {
            var text = cells[column].Text;
            var troop = (Classificator.TroopsEnum)Enum.Parse(typeof(Classificator.TroopsEnum), text.Replace(" ", ""));
            if (troop == vill.Settings.StableTrain) return; //no difference
            vill.Settings.StableTrain = troop;

            if (acc.Wb == null) return;
            TroopsHelper.ReStartResearchAndImprovement(acc, vill);
        }

        private void UpdateWorkshop(Account acc, Village vill, CellCollection cells, int column)
        {
            var text = cells[column].Text;
            var troop = (Classificator.TroopsEnum)Enum.Parse(typeof(Classificator.TroopsEnum), text.Replace(" ", ""));
            if (troop == vill.Settings.WorkshopTrain) return; //no difference
            vill.Settings.WorkshopTrain = troop;

            if (acc.Wb == null) return;
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
            var acc = GetSelectedAcc();

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

        private void NYS_CheckedChanged(object sender, EventArgs e)
        {
            if (!NYS.Checked) return;
            UpdateUc();
        }
    }
}