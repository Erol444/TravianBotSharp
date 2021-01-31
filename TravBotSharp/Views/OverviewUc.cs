using System;
using System.Collections.Generic;
using System.Linq;
using TbsCore.Models.Settings;
using TbsCore.Models.VillageModels;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Tasks.LowLevel;
using TravBotSharp.Interfaces;
using XPTable.Editors;
using XPTable.Models;

namespace TravBotSharp.Views
{
    public partial class OverviewUc : TbsBaseUc, ITbsUc
    {
        TableModel tableModelMain = new TableModel();
        TableModel tableModelGlobal = new TableModel();
        public OverviewUc()
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
                r.Cells.Add(new Cell(vill.Settings.Type.ToString())); //vill type
                r.Cells.Add(new Cell("", vill.Settings.GetRes)); //Get resources from
                r.Cells.Add(new Cell("", vill.Settings.SendRes)); //Send resources to
                r.Cells.Add(new Cell(vill.Expansion.Celebrations.ToString())); // Auto-celebrations
                r.Cells.Add(new Cell("", vill.Settings.AutoExpandStorage)); // Auto-Expand storage
                r.Cells.Add(new Cell("", vill.Settings.UseHeroRes)); // Use hero res
                r.Cells.Add(new Cell(vill.Settings.Donate.ToString())); // Donate to ally bonus
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
            r.Cells.Add(new Cell(vill.Settings.Type.ToString())); //vill type
            r.Cells.Add(new Cell("", vill.Settings.GetRes)); //Get resources from
            r.Cells.Add(new Cell("", vill.Settings.SendRes)); //Send resources to
            r.Cells.Add(new Cell(vill.Expansion.Celebrations.ToString())); // Auto-celebrations
            r.Cells.Add(new Cell("", vill.Settings.AutoExpandStorage)); // Auto-Expand storage
            r.Cells.Add(new Cell("", vill.Settings.UseHeroRes)); // Use hero res
            r.Cells.Add(new Cell(vill.Settings.Donate.ToString())); // Donate to ally bonus
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

            //Village type
            ComboBoxColumn typeColumn = new ComboBoxColumn
            {
                Text = "Type",
                ToolTipText = "Type of the village",
                Width = 70
            };

            ComboBoxCellEditor typeEditor = new ComboBoxCellEditor
            {
                DropDownStyle = DropDownStyle.DropDownList
            };
            typeEditor.Items.AddRange(new string[] { "Farm", "Support", "Deff", "Off" });
            typeColumn.Editor = typeEditor;

            columnModel.Columns.Add(typeColumn);

            //get resources for building
            CheckBoxColumn GetRes = new CheckBoxColumn
            {
                Text = "Get Res",
                Width = 60,
                ToolTipText = "Select the supplying village when there are not enough resources"
            };
            columnModel.Columns.Add(GetRes);
            //get resources for troops
            columnModel.Columns.Add(new CheckBoxColumn
            {
                Text = "Send Res",
                Width = 75,
                ToolTipText = "Select where to send resources when too many"
            });

            ComboBoxCellEditor celebrationsEditor = new ComboBoxCellEditor
            {
                DropDownStyle = DropDownStyle.DropDownList
            };
            celebrationsEditor.Items.AddRange(new string[] { "None", "Small", "Big" });

            columnModel.Columns.Add(new ComboBoxColumn
            {
                Text = "Celebrations",
                ToolTipText = "Auto-Start celebrations",
                Editor = celebrationsEditor,
                Width = 85
            });

            columnModel.Columns.Add(new CheckBoxColumn
            {
                Text = "AutoExpandStorage",
                Width = 130,
                ToolTipText = "Automatically Expand storage when it's full"
            });
            columnModel.Columns.Add(new CheckBoxColumn
            {
                Text = "UseHeroRes",
                Width = 85,
                ToolTipText = "Use hero resources"
            });
            // Donate resources to ally bonus
            ComboBoxCellEditor donationEditor = new ComboBoxCellEditor
            {
                DropDownStyle = DropDownStyle.DropDownList
            };
            donationEditor.Items.AddRange(new string[] { "None", "ExcludeCrop", "OnlyCrop" });

            columnModel.Columns.Add(new ComboBoxColumn
            {
                Text = "Donate",
                Width = 70,
                ToolTipText = "Donate resources to the ally bonuses",
                Editor = donationEditor
            });

        }
        #endregion

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
                UpdateVillageType(vill, cells, column);
                column++;
                vill.Settings.GetRes = cells[column].Checked;
                column++;
                vill.Settings.SendRes = cells[column].Checked;
                column++;
                vill.Expansion.Celebrations = (CelebrationEnum)Enum.Parse(typeof(CelebrationEnum), cells[column].Text);
                column++;
                vill.Settings.AutoExpandStorage = cells[column].Checked;
                column++;
                vill.Settings.UseHeroRes = cells[column].Checked;
                column++;
                vill.Settings.Donate = (DonateEnum)Enum.Parse(typeof(DonateEnum), cells[column].Text);

                if (vill.Expansion.Celebrations != CelebrationEnum.None && acc.Tasks != null) AccountHelper.ReStartCelebration(acc, vill);
            }
            //Change name of village/s
            if (0 < changeVillNames.Count && acc.Tasks != null)
            {
                TaskExecutor.AddTaskIfNotExists(acc,
                        new ChangeVillageName()
                        {
                            ExecuteAt = DateTime.Now,
                            ChangeList = changeVillNames
                        });
            }
        }

        private void UpdateVillageType(Village vill, CellCollection cells, int column)
        {
            var acc = GetSelectedAcc();
            var type = (VillType)Enum.Parse(typeof(VillType), cells[column].Text);
            if (type == vill.Settings.Type) return;
            vill.Settings.Type = type;

            if (acc.Wb == null) return;
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
    }
}
