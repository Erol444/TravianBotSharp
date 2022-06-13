using System;
using System.Collections.Generic;
using System.Linq;
using TbsCore.Helpers;
using TbsCore.TravianData;
using TravBotSharp.Interfaces;

namespace TravBotSharp.Views
{
    public partial class OverviewTroopsUc : TbsBaseUc, ITbsUc
    {
        public OverviewTroopsUc()
        {
            InitializeComponent();

            dataGridView1.AutoGenerateColumns = false;
        }

        public void UpdateUc()
        {
            var acc = GetSelectedAcc();
            if (acc == null) return;
            if (acc.Villages.Count == 0) return;

            if (acc.AccInfo.Tribe != null)
            {
                int troopsEnum = ((int)acc.AccInfo.Tribe - 1) * 10;
                var barrackTroops = new List<string>() { Classificator.TroopsEnum.None.ToString() };
                var stableTroops = new List<string>() { Classificator.TroopsEnum.None.ToString() };
                var workshopTroops = new List<string>() { Classificator.TroopsEnum.None.ToString() };

                for (var i = troopsEnum + 1; i < troopsEnum + 11; i++)
                {
                    Classificator.TroopsEnum troop = (Classificator.TroopsEnum)i;
                    switch (TroopsData.GetTroopBuilding(troop, false))
                    {
                        case Classificator.BuildingEnum.Barracks:
                            barrackTroops.Add(troop.ToString());
                            break;

                        case Classificator.BuildingEnum.Stable:
                            stableTroops.Add(troop.ToString());
                            break;

                        case Classificator.BuildingEnum.Workshop:
                            workshopTroops.Add(troop.ToString());
                            break;

                        default:
                            break;
                    }
                }
                BarracksColumn.DataSource = barrackTroops;
                StableColumn.DataSource = stableTroops;
                WorkshopColumn.DataSource = workshopTroops;
            }

            var dataList = new List<Data>();
            foreach (var item in acc.Villages)
            {
                dataList.Add(new Data
                {
                    Id = item.Id,
                    Village = item.Name,
                    Barracks = item.Settings.BarracksTrain.ToString(),
                    GB = item.Settings.GreatBarracksTrain,
                    Stable = item.Settings.StableTrain.ToString(),
                    GS = item.Settings.GreatStableTrain,
                    Workshop = item.Settings.WorkshopTrain.ToString(),
                    AutoImprove = item.Settings.AutoImprove,
                });
            }

            bindingSource1.DataSource = dataList;
            dataGridView1.DataSource = bindingSource1;
        }

        //Save button
        private void button1_Click(object sender, EventArgs e)
        {
            var acc = GetSelectedAcc();
            if (acc == null) return;
            if (acc.Villages.Count == 0) return;

            var dataList = bindingSource1.DataSource as List<Data>;
            foreach (var item in dataList)
            {
                var village = acc.Villages.FirstOrDefault(x => x.Id == item.Id);
                if (village != null)
                {
                    village.Settings.BarracksTrain = (Classificator.TroopsEnum)Enum.Parse(typeof(Classificator.TroopsEnum), item.Barracks);
                    village.Settings.StableTrain = (Classificator.TroopsEnum)Enum.Parse(typeof(Classificator.TroopsEnum), item.Stable);
                    village.Settings.WorkshopTrain = (Classificator.TroopsEnum)Enum.Parse(typeof(Classificator.TroopsEnum), item.Workshop);
                    village.Settings.GreatBarracksTrain = item.GB;
                    village.Settings.GreatStableTrain = item.GS;
                    village.Settings.AutoImprove = item.AutoImprove;

                    TroopsHelper.ReStartResearchAndImprovement(acc, village);
                }
            }
        }

        public class Data
        {
            public int Id { get; set; }
            public string Village { get; set; }
            public string Barracks { get; set; }
            public bool GB { get; set; }
            public string Stable { get; set; }
            public bool GS { get; set; }
            public string Workshop { get; set; }
            public bool AutoImprove { get; set; }
        }
    }
}