using System;
using System.Linq;
using System.Windows.Forms;

using TbsCore.Models.AccModels;
using TravBotSharp.Interfaces;
using TravBotSharp.Forms;
using XPTable.Models;
using XPTable.Events;

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

            table1.CellButtonClicked += cellButton_Click;
            table1.CellPropertyChanged += cell_PropertyChanged;
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
                        r.Cells.Add(new Cell(vill.Id)
                        {
                            Tag = currentType
                        }); //vill id
                        r.Cells.Add(new Cell(vill.Name)); //vill name

                        r.Cells.Add(new Cell("", vill.Market.Npc.Enabled)); //Auto NPC
                        r.Cells.Add(new Cell(vill.Market.Npc.ResourcesRatio.Wood)); //Wood ratio
                        r.Cells.Add(new Cell(vill.Market.Npc.ResourcesRatio.Clay)); //Clay ratio
                        r.Cells.Add(new Cell(vill.Market.Npc.ResourcesRatio.Iron)); //Iron ratio
                        r.Cells.Add(new Cell(vill.Market.Npc.ResourcesRatio.Crop)); //Crop ratio
                        r.Cells.Add(new Cell("", vill.Market.Npc.NpcIfOverflow)); // Ignore Overflow
                        table1.TableModel.Rows.Add(r);
                    }
                    break;

                case TypeModel.AUTOMARKET:
                    InitAutoMarket();
                    foreach (var vill in acc.Villages)
                    {
                        var r = new Row();
                        r.Cells.Add(new Cell(vill.Id)
                        {
                            Tag = currentType
                        }); //vill id
                        r.Cells.Add(new Cell(vill.Name)); //vill name

                        r.Cells.Add(new Cell("", vill.Market.AutoMarket.SendToMain.Enabled)); //send to main
                        r.Cells.Add(new Cell("Resource")); // amount
                        r.Cells.Add(new Cell("Condition")); // condition
                        r.Cells.Add(new Cell("", vill.Market.AutoMarket.SendToNeed.Enabled)); // send to need
                        r.Cells.Add(new Cell("Resource")); // amount
                        r.Cells.Add(new Cell("Condition")); // condition
                        r.Cells.Add(new Cell("", vill.Market.AutoMarket.NeedWhenBuild));
                        r.Cells.Add(new Cell("", vill.Market.AutoMarket.NeedWhenTrain));
                        r.Cells.Add(new Cell("", vill.Market.AutoMarket.NeedWhenOther));
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
            NumberColumn villId = new NumberColumn
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
                ToolTipText = "Auto NPC Crop to other resource",
            };
            columnmodel.Columns.Add(autoNPC);

            NumberColumn woodRatio = new NumberColumn
            {
                Text = "Wood",
                Width = 50,
                ToolTipText = "Wood ratio",
                Maximum = long.MaxValue
            };

            columnmodel.Columns.Add(woodRatio);

            NumberColumn clayRatio = new NumberColumn
            {
                Text = "Clay",
                Width = 50,
                ToolTipText = "Clay ratio",
                Maximum = long.MaxValue
            };

            columnmodel.Columns.Add(clayRatio);

            NumberColumn ironRatio = new NumberColumn
            {
                Text = "Iron",
                Width = 50,
                ToolTipText = "Iron ratio",
                Maximum = long.MaxValue
            };

            columnmodel.Columns.Add(ironRatio);

            NumberColumn cropRatio = new NumberColumn
            {
                Text = "Crop",
                Width = 50,
                ToolTipText = "Crop ratio",
                Maximum = long.MaxValue
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
            NumberColumn villId = new NumberColumn
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
            ButtonColumn sendToMainAmount = new ButtonColumn
            {
                Text = "resources",
                Width = 70,
            };
            columnmodel.Columns.Add(sendToMainAmount);

            //Send to main condition
            ButtonColumn sendToMainCondition = new ButtonColumn
            {
                Text = "when above",
                Width = 90,
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

            //Send to need amount
            ButtonColumn sendToNeedAmount = new ButtonColumn
            {
                Text = "resources",
                Width = 70,
            };
            columnmodel.Columns.Add(sendToNeedAmount);

            //Send to need condition
            ButtonColumn sendToNeedCondition = new ButtonColumn
            {
                Text = "when above",
                Width = 90,
            };
            columnmodel.Columns.Add(sendToNeedCondition);

            //Need when build building
            CheckBoxColumn needWhenBuild = new CheckBoxColumn
            {
                Text = "Building",
                Width = 70,
            };
            columnmodel.Columns.Add(needWhenBuild);

            //Need when train troop
            CheckBoxColumn needWhenTrain = new CheckBoxColumn
            {
                Text = "Training",
                Width = 70,
            };
            columnmodel.Columns.Add(needWhenTrain);

            //Need when other
            CheckBoxColumn needWhenOther = new CheckBoxColumn
            {
                Text = "Other",
                Width = 70,
            };
            columnmodel.Columns.Add(needWhenOther);
        }

        #endregion Initialize column model

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentType = (TypeModel)comboBox1.SelectedIndex;
            UpdateUc();
        }

        private void cellButton_Click(object sender, CellButtonEventArgs e)
        {
            var row = table1.TableModel.Rows[e.Row];
            var firstCell = row.Cells[0];
            var idVillage = firstCell.Data as int?;
            var acc = GetSelectedAcc();
            var vill = acc.Villages.FirstOrDefault(x => x.Id == idVillage);
            var type = firstCell.Tag as TypeModel?;

            switch (type)
            {
                case TypeModel.NPC:
                    break;

                case TypeModel.AUTOMARKET:
                    switch (e.Column)
                    {
                        case 3:
                            using (var form = new ResourceSelector(vill.Market.AutoMarket.SendToMain.Amount))
                            {
                                var result = form.ShowDialog();
                                if (result == DialogResult.OK)
                                {
                                    vill.Market.AutoMarket.SendToMain.Amount = form.Resources;
                                }
                            }
                            break;

                        case 4:
                            using (var form = new ResourceSelector(vill.Market.AutoMarket.SendToMain.Condition))
                            {
                                var result = form.ShowDialog();
                                if (result == DialogResult.OK)
                                {
                                    vill.Market.AutoMarket.SendToMain.Condition = form.Resources;
                                }
                            }
                            break;

                        case 6:
                            using (var form = new ResourceSelector(vill.Market.AutoMarket.SendToNeed.Amount))
                            {
                                var result = form.ShowDialog();
                                if (result == DialogResult.OK)
                                {
                                    vill.Market.AutoMarket.SendToNeed.Amount = form.Resources;
                                }
                            }
                            break;

                        case 7:
                            using (var form = new ResourceSelector(vill.Market.AutoMarket.SendToNeed.Condition))
                            {
                                var result = form.ShowDialog();
                                if (result == DialogResult.OK)
                                {
                                    vill.Market.AutoMarket.SendToNeed.Condition = form.Resources;
                                }
                            }
                            break;
                    }

                    break;

                default:
                    break;
            }
        }

        private void cell_PropertyChanged(object sender, CellEventArgs e)
        {
            var row = table1.TableModel.Rows[e.Row];
            var firstCell = row.Cells[0];
            var idVillage = firstCell.Data as int?;
            var acc = GetSelectedAcc();
            var vill = acc.Villages.FirstOrDefault(x => x.Id == idVillage);
            var type = firstCell.Tag as TypeModel?;

            switch (type)
            {
                case TypeModel.NPC:
                    switch (e.Column)
                    {
                        case 2:
                            vill.Market.Npc.Enabled = e.Cell.Checked;
                            break;

                        case 3:
                            vill.Market.Npc.ResourcesRatio.Wood = Convert.ToInt64(e.Cell.Data);
                            break;

                        case 4:
                            vill.Market.Npc.ResourcesRatio.Clay = Convert.ToInt64(e.Cell.Data);
                            break;

                        case 5:
                            vill.Market.Npc.ResourcesRatio.Iron = Convert.ToInt64(e.Cell.Data);
                            break;

                        case 6:
                            vill.Market.Npc.ResourcesRatio.Crop = Convert.ToInt64(e.Cell.Data);
                            break;

                        case 7:
                            vill.Market.Npc.NpcIfOverflow = e.Cell.Checked;
                            break;
                    }
                    break;

                case TypeModel.AUTOMARKET:
                    switch (e.Column)
                    {
                        case 2:
                            vill.Market.AutoMarket.SendToMain.Enabled = e.Cell.Checked;
                            break;

                        case 5:
                            vill.Market.AutoMarket.SendToNeed.Enabled = e.Cell.Checked;
                            break;

                        case 8:
                            vill.Market.AutoMarket.NeedWhenBuild = e.Cell.Checked;
                            break;

                        case 9:
                            vill.Market.AutoMarket.NeedWhenTrain = e.Cell.Checked;
                            break;

                        case 10:
                            vill.Market.AutoMarket.NeedWhenOther = e.Cell.Checked;
                            break;
                    }
                    break;

                default:
                    break;
            }
        }
    }
}