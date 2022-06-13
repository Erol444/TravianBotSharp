using System;
using System.Collections.Generic;
using System.Linq;
using TravBotSharp.Interfaces;

namespace TravBotSharp.Views
{
    public partial class OverviewUc : TbsBaseUc, ITbsUc
    {
        public OverviewUc()
        {
            InitializeComponent();
            dataGridView1.DataSource = bindingSource1;
            dataGridView1.AutoGenerateColumns = false;
        }

        public void UpdateUc()
        {
            var acc = GetSelectedAcc();
            if (acc == null) return;
            if (acc.Villages.Count == 0) return;
            var dataList = new List<Data>();
            foreach (var item in acc.Villages)
            {
                dataList.Add(new Data
                {
                    Id = item.Id,
                    Name = item.Name,
                    UseHeroRes = item.Settings.UseHeroRes,
                    ExpandStorage = item.Settings.AutoExpandStorage,
                });
            }
            bindingSource1.DataSource = dataList;
        }

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
                    village.Settings.UseHeroRes = item.UseHeroRes;
                    village.Settings.AutoExpandStorage = item.ExpandStorage;
                }
            }
        }

        public class Data
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public bool ExpandStorage { get; set; }
            public bool UseHeroRes { get; set; }
        }
    }
}