﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TbsCore.Models.SendTroopsModels;
using TbsCore.Models.VillageModels;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.VillageModels;
using TravBotSharp.Files.Tasks;
using TravBotSharp.Files.Tasks.LowLevel;
using TravBotSharp.Interfaces;
using XPTable.Editors;
using XPTable.Models;

namespace TravBotSharp.Views
{
    public partial class DeffendingUc : TbsBaseUc, ITbsUc
    {
        private readonly TableModel tableModelGlobal = new TableModel();
        private readonly TableModel tableModelMain = new TableModel();

        public DeffendingUc()
        {
            InitializeComponent();

            InitTables();
            InitGlobalTable();
        }

        public void UpdateUc()
        {
            var acc = GetSelectedAcc();
            if (acc.Villages.Count == 0) return;

            tableModelMain?.Rows?.Clear();
            foreach (var vill in acc.Villages)
            {
                var r = new Row();
                r.Cells.Add(new Cell(vill.Id.ToString())); //vill id
                r.Cells.Add(new Cell(vill.Name)); //vill name
                r.Cells.Add(new Cell(vill.Deffing.AlertType.ToString())); //Type of alert
                r.Cells.Add(new Cell("", vill.Deffing.OnlyAlertOnHero)); //Alert only on hero
                tableModelMain.Rows.Add(r);
            }
        }

        private void InitGlobalTable()
        {
            XpTableGlobal.TableModel = tableModelGlobal;
            var r = new Row();
            r.Cells.Add(new Cell("")); //vill id
            r.Cells.Add(new Cell("CHANGE MULTIPLE")); //vill name
            r.Cells.Add(new Cell(0)); //Type of alert
            r.Cells.Add(new Cell("", true)); //Alert only on hero
            tableModelGlobal.Rows.Add(r);
        }

        #region Initialize table columns

        private void InitTables()
        {
            var columnModel = new ColumnModel();

            //VillageId
            var villId = new TextColumn
            {
                Editable = false,
                Text = "Id",
                ToolTipText = "Village Id",
                Width = 40
            };
            columnModel.Columns.Add(villId);
            //Village name
            var vill = new TextColumn
            {
                Editable = false,
                Text = "Village",
                ToolTipText = "Village name",
                Width = 120
            };
            columnModel.Columns.Add(vill);

            //Alert type
            var alertType = new ComboBoxColumn
            {
                Text = "Alert Type",
                ToolTipText = "On what type of attack should I alert you?",
                Width = 80
            };

            var typeEditor = new ComboBoxCellEditor
            {
                DropDownStyle = DropDownStyle.DropDownList
            };
            typeEditor.Items.AddRange(new[] {"Disabled", "FullAttack", "AnyAttack"});
            alertType.Editor = typeEditor;

            columnModel.Columns.Add(alertType);

            // Alert only if hero is present in the attack, for spies art users
            var onHero = new CheckBoxColumn
            {
                Text = "AlertOnHero",
                Width = 85,
                ToolTipText = "Only alert if hero is present in attack. For spies art users. Otherwise ignored"
            };
            columnModel.Columns.Add(onHero);

            // set the Table's ColumModel and TableModel
            table1.ColumnModel = columnModel;
            XpTableGlobal.ColumnModel = columnModel;

            table1.TableModel = tableModelMain;
        }

        #endregion

        //Save button
        private void button1_Click(object sender, EventArgs e)
        {
            var acc = GetSelectedAcc();
            //change vill names list
            var changeVillNames = new List<(int, string)>();
            for (var i = 0; i < tableModelMain.Rows.Count; i++)
            {
                var cells = tableModelMain.Rows[i].Cells;
                var column = 0;
                //Village id
                var id = int.Parse(cells[column].Text);
                var vill = acc.Villages.First(x => x.Id == id);
                column++;
                column++;
                UpdateAlertType(vill, cells, column);
                column++;
                vill.Deffing.OnlyAlertOnHero = cells[column].Checked;
            }

            //Change name of village/s
            if (changeVillNames.Count > 0)
                TaskExecutor.AddTaskIfNotExists(acc, new ChangeVillageName
                {
                    ExecuteAt = DateTime.Now,
                    ChangeList = changeVillNames
                });
        }

        private void UpdateAlertType(Village vill, CellCollection cells, int column)
        {
            var text = cells[column].Text;
            vill.Deffing.AlertType = (AlertTypeEnum) Enum.Parse(typeof(AlertTypeEnum), text.Replace(" ", ""));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var selectedCells = new List<CellChanges>();
            for (var i = 0; i < tableModelMain.Rows.Count; i++)
            for (var y = 0; y < tableModelMain.Rows[i].SelectedItems.Count(); y++)
                selectedCells.Add(new CellChanges
                {
                    Cell = tableModelMain.Rows[i].SelectedItems[y],
                    Num = tableModelMain.Rows[i].SelectedIndicies[y]
                });

            foreach (var selectedCell in selectedCells)
            {
                var global = tableModelGlobal.Rows[0].Cells[selectedCell.Num];
                selectedCell.Cell.Text = global.Text;
                selectedCell.Cell.Checked = global.Checked;
            }
        }

        private void button1_Click_1(object sender, EventArgs e) // Send deff to specific coordinates
        {
            var acc = GetSelectedAcc();

            var deffCount = (int) maxDeff.Value;
            if (deffCount == 0) deffCount = int.MaxValue;
            var amount = new SendDeffAmount {Amount = deffCount};

            var node = new SendDeff();
            foreach (var vill in acc.Villages)
            {
                var sendDeff = new SendDeff
                {
                    Vill = vill,
                    DeffAmount = amount,
                    TargetVillage = sendDeffCoords.Coords,
                    Priority = BotTask.TaskPriority.High,
                    NextTask = node
                };
                node = sendDeff;
            }

            node.ExecuteAt = DateTime.MinValue;
            TaskExecutor.AddTaskIfNotExists(acc, node);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Not yet implemented");

            //var acc = GetSelectedAcc();
            //var coords = new Coordinates(-52, -59);

            //var waves = new List<SendWaveModel>();
            //for (var i = 0; i < 10; i++)
            //{
            //    var attk = new SendWaveModel();
            //    attk.Troops = new int[11];
            //    if (i == 0)
            //    {
            //        attk.Arrival = DateTime.Now.AddHours(-1).AddMinutes(2);
            //        attk.Arrival = attk.Arrival.AddSeconds(60 - attk.Arrival.Second);
            //        acc.Wb.Log($"Arrive at {attk.Arrival}");
            //    }
            //    else
            //    {
            //        attk.DelayMs = 1000;
            //    }

            //    attk.TargetCoordinates = coords;
            //    attk.MovementType = Classificator.MovementType.Reinforcement;
            //    attk.Troops[0] = 5555;

            //    waves.Add(attk);
            //}


            //var waveTask = new SendWaves
            //{
            //    ExecuteAt = DateTime.Now,
            //    Vill = AccountHelper.GetMainVillage(acc),
            //    SendWaveModels = waves.ToList(),
            //    Priority = BotTask.TaskPriority.High
            //};
            //TaskExecutor.AddTask(acc, waveTask);
        }

        private class CellChanges
        {
            public Cell Cell { get; set; }
            public int Num { get; set; }
        }
    }
}