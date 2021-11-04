using System;
using System.Collections.Generic;
using System.Linq;

using TbsCore.Models.AccModels;
using TravBotSharp.Interfaces;
using XPTable.Models;

namespace TravBotSharp.Views
{
    public partial class OverviewMarketUc : TbsBaseUc, ITbsUc
    {
        private enum TypeModel
        {
            NPC = 0,
            AUTOMARKET,
        }

        private TableModel[] tableModels = new TableModel[2];
        private ColumnModel[] columnModels = new ColumnModel[2];
        private TypeModel currentType;

        public OverviewMarketUc()
        {
            InitializeComponent();
            currentType = TypeModel.NPC;
            foreach (int i in Enum.GetValues(typeof(TypeModel)))
            {
                tableModels[i] = new TableModel();
                columnModels[i] = new ColumnModel();
            }
        }

        public void UpdateUc()
        {
            var acc = GetSelectedAcc();
            if (acc.Villages.Count == 0) return;

            table1.ColumnModel = columnModels[(int)currentType];
            table1.ColumnModel.Columns.Clear();
            table1.TableModel = tableModels[(int)currentType];
            table1.TableModel.Rows.Clear();

            switch (currentType)
            {
                case TypeModel.NPC:
                    InitNPC();
                    foreach (var vill in acc.Villages)
                    {
                        var r = new Row();
                        r.Cells.Add(new Cell(vill.Id.ToString())); //vill id
                        r.Cells.Add(new Cell(vill.Name)); //vill name

                        r.Cells.Add(new Cell("", vill.Market.Npc.Enabled)); //Auto NPC
                        r.Cells.Add(new Cell(vill.Market.Npc.ResourcesRatio.Wood)); //Wood ratio
                        r.Cells.Add(new Cell(vill.Market.Npc.ResourcesRatio.Clay)); //Clay ratio
                        r.Cells.Add(new Cell(vill.Market.Npc.ResourcesRatio.Iron)); //Iron ratio
                        r.Cells.Add(new Cell(vill.Market.Npc.ResourcesRatio.Crop)); //Crop ratio
                        r.Cells.Add(new Cell("", vill.Market.Npc.NpcIfOverflow)); // Overflow ignore
                        table1.TableModel.Rows.Add(r);
                    }
                    break;

                case TypeModel.AUTOMARKET:
                    InitAutoMarket();
                    foreach (var vill in acc.Villages)
                    {
                        var r = new Row();
                        r.Cells.Add(new Cell(vill.Id.ToString())); //vill id
                        r.Cells.Add(new Cell(vill.Name)); //vill name

                        r.Cells.Add(new Cell("", false));
                        r.Cells.Add(new Cell(1000));
                        r.Cells.Add(new Cell(80));
                        r.Cells.Add(new Cell("", true));
                        r.Cells.Add(new Cell(1000));
                        r.Cells.Add(new Cell(80));
                        table1.TableModel.Rows.Add(r);
                    }
                    break;
            }
        }

        #region Initialize column model

        private void InitNPC()
        {
            var columnmodel = table1.ColumnModel;

            //VillageId
            TextColumn villId = new TextColumn
            {
                Editable = false,
                Text = "Id",
                ToolTipText = "Village Id",
                Width = 40
            };
            columnmodel.Columns.Add(villId);

            //Village name
            TextColumn vill = new TextColumn
            {
                Editable = false,
                Text = "Village",
                ToolTipText = "Village name",
                Width = 120
            };
            columnmodel.Columns.Add(vill);

            //Auto NPC
            CheckBoxColumn autoNPC = new CheckBoxColumn
            {
                Text = "NPC",
                Width = 40,
                ToolTipText = "Auto NPC Crop to other resource"
            };
            columnmodel.Columns.Add(autoNPC);

            NumberColumn woodRatio = new NumberColumn
            {
                Text = "Wood",
                Width = 50,
                ToolTipText = "Wood ratio"
            };
            columnmodel.Columns.Add(woodRatio);

            NumberColumn clayRatio = new NumberColumn
            {
                Text = "Clay",
                Width = 50,
                ToolTipText = "Clay ratio"
            };
            columnmodel.Columns.Add(clayRatio);

            NumberColumn ironRatio = new NumberColumn
            {
                Text = "Iron",
                Width = 50,
                ToolTipText = "Iron ratio"
            };
            columnmodel.Columns.Add(ironRatio);

            NumberColumn cropRatio = new NumberColumn
            {
                Text = "Crop",
                Width = 50,
                ToolTipText = "Crop ratio"
            };
            columnmodel.Columns.Add(cropRatio);

            CheckBoxColumn overflow = new CheckBoxColumn
            {
                Text = "Overflow ignore",
                Width = 110,
                ToolTipText = "NPC even if overflow"
            };
            columnmodel.Columns.Add(overflow);
        }

        private void InitAutoMarket()
        {
            var columnmodel = table1.ColumnModel;
            //VillageId
            TextColumn villId = new TextColumn
            {
                Editable = false,
                Text = "Id",
                ToolTipText = "Village Id",
                Width = 40
            };
            columnmodel.Columns.Add(villId);

            //Village name
            TextColumn vill = new TextColumn
            {
                Editable = false,
                Text = "Village",
                ToolTipText = "Village name",
                Width = 120
            };
            columnmodel.Columns.Add(vill);

            //Send to main
            CheckBoxColumn sendToMain = new CheckBoxColumn
            {
                Text = "Send to main",
                Width = 90,
                ToolTipText = "Auto send to main"
            };
            columnmodel.Columns.Add(sendToMain);

            //Send to main amount
            NumberColumn sendToMainAmount = new NumberColumn
            {
                Text = "resources",
                Width = 70,
                ToolTipText = "number if below 100, otherwise percent of storage"
            };
            columnmodel.Columns.Add(sendToMainAmount);

            //Send to main condition
            NumberColumn sendToMainCondition = new NumberColumn
            {
                Text = "when above",
                Width = 90,
                ToolTipText = "Auto send to main when any resource above this number if below 100, otherwise percent of storage"
            };
            columnmodel.Columns.Add(sendToMainCondition);

            //Send to need
            CheckBoxColumn sendToNeed = new CheckBoxColumn
            {
                Text = "Send to need",
                Width = 90,
                ToolTipText = "Auto send to need"
            };
            columnmodel.Columns.Add(sendToNeed);

            //Send to main amount
            NumberColumn sendToNeedAmount = new NumberColumn
            {
                Text = "resources",
                Width = 70,
                ToolTipText = "number if below 100, otherwise percent of storage"
            };
            columnmodel.Columns.Add(sendToNeedAmount);

            //Send to main condition
            NumberColumn sendToNeedCondition = new NumberColumn
            {
                Text = "when above",
                Width = 90,
                ToolTipText = "Auto send to main when any resource above this number if below 100, otherwise percent of storage"
            };
            columnmodel.Columns.Add(sendToNeedCondition);
        }

        #endregion Initialize column model

        #region Save function

        private void saveNPC(Account acc)
        {
            var tablemodel = table1.TableModel;
            for (int i = 0; i < tablemodel.Rows.Count; i++)
            {
                var cells = tablemodel.Rows[i].Cells;
                int column = 0;
                //Village id
                var id = int.Parse(cells[column].Text);
                var vill = acc.Villages.First(x => x.Id == id);

                //name
                column++;

                //Auto NPC check
                column++;
                vill.Market.Npc.Enabled = cells[column].Checked;

                //Wood ratio
                column++;
                vill.Market.Npc.ResourcesRatio.Wood = (long)cells[column].Data;

                //Clay ratio
                column++;
                vill.Market.Npc.ResourcesRatio.Clay = (long)cells[column].Data;

                //Iron ratio
                column++;
                vill.Market.Npc.ResourcesRatio.Iron = (long)cells[column].Data;

                //Crop ratio
                column++;
                vill.Market.Npc.ResourcesRatio.Crop = (long)cells[column].Data;

                //Overflow ignore
                column++;
                vill.Market.Npc.NpcIfOverflow = cells[column].Checked;
            }
        }

        #endregion Save function

        //Save button
        private void button1_Click(object sender, EventArgs e)
        {
            var acc = GetSelectedAcc();

            switch (currentType)
            {
                case TypeModel.NPC:
                    saveNPC(acc);
                    break;

                case TypeModel.AUTOMARKET:

                    break;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentType = (TypeModel)comboBox1.SelectedIndex;
            UpdateUc();
        }
    }
}