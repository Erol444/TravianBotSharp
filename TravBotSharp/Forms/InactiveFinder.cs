using System;
using System.Net;
using System.Linq;
using System.Drawing;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows.Forms;

using RestSharp;

using TbsCore.Models;
using TbsCore.Models.VillageModels;

using TravBotSharp.Files.Parsers;
using TravBotSharp.Files.Helpers;
using TbsCore.Models.AccModels;

namespace TravBotSharp.Forms
{
    public partial class InactiveFinder : Form
    {
        private ListViewColumnSorter lvwColumnSorter;

        private RestClient Client;

        public List<Farm> InactiveFarms
        {
            get
            {
                List<Farm> result = new List<Farm>();
                foreach (ListViewItem item in InactiveList.SelectedItems)
                {
                    result.Add(new Farm()
                    {
                        Coords = MapParser.GetCoordinates(item.SubItems[2].Text),
                        Troops = troopsSelectorUc1.Troops
                    });
                }
                return result;
            }
        }

        private Account acc;

        public InactiveFinder(Account acc, string label)
        {
            InitializeComponent();

            // list view sorter
            lvwColumnSorter = new ListViewColumnSorter();
            this.InactiveList.ListViewItemSorter = lvwColumnSorter;

            // TODO: use acc.Wb.RestClient - for proxy & to save resources
            // i dont think we need this ^
            // because some proxies limit your bandwith and that is not worth to use proxy - VINAGHOST
            Client = new RestClient();

            this.acc = acc;
            this.flName.Text = label;

            troopsSelectorUc1.HeroEditable = false;
            troopsSelectorUc1.Init(acc.AccInfo.Tribe);

            // UI
            foreach (var vill in acc.Villages)
            {
                comboBoxVillages.Items.Add(vill.Name);
            }
            comboBoxVillages.SelectedIndex = 0;
        }

        /// <summary>
        /// world code for our world from travianstats.de
        /// </summary>
        private async Task<string> GetServerCode()
        {
            var serverUrl = acc.AccInfo.ServerUrl;

            // get serverUrl without https://
            var url = (new UriBuilder(serverUrl)).Host;

            //request to travaianstats.de
            Client.BaseUrl = new Uri("https://travianstats.de");
            var request = new RestRequest();

            var response = await Client.ExecuteAsync(request);

            if (response.StatusCode != HttpStatusCode.OK) return null;

            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(response.Content);
            // use this form to search code of our server

            // find our server
            return doc.GetElementbyId("welt")
                .Descendants()
                .FirstOrDefault(x => x.InnerText.Contains(url))?
                .GetAttributeValue("value", "");
        }

        /// <summary>
        /// This function for travianstats.de
        /// </summary>
        /// <param name="serverCode"></param>
        /// <returns></returns>
        private async Task<List<InactiveFarm>> GetFarms_TravianstatDE(string serverCode)
        {
            Client.BaseUrl = new Uri("https://travianstats.de");
            var request = new RestRequest($"?m=inactive_finder&w={serverCode}", Method.POST);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Cookie", $"tcn_world={serverCode}");
            request.AddParameter("m", "inactivefinder");
            request.AddParameter("w", serverCode);
            request.AddParameter("x", ((int)coordinatesUc1.Coords.x).ToString());
            request.AddParameter("y", ((int)coordinatesUc1.Coords.y).ToString());
            request.AddParameter("distance", ((int)Distance.Value).ToString());

            var response = await Client.ExecuteAsync(request);

            if (response.StatusCode != HttpStatusCode.OK) throw new Exception("SendGetReq failed!\n" + response.Content);

            if (response.Content.Contains("Nothing found"))
            {
                return null;
            }

            var doc = new HtmlAgilityPack.HtmlDocument();

            doc.LoadHtml(response.Content);

            // table
            var table = doc.DocumentNode.SelectNodes("//table[@id='myTable']//tbody") // they use myTable for naming their table ?_?
                        .Descendants("tr")
                        .Where(tr => tr.Elements("td").Count() > 1)
                        .Select(tr => tr.Elements("td").Select(td => td.InnerText.Trim().Replace("\t", "").Replace("\n", "")).ToList())
                        .ToList();

            var result = new List<InactiveFarm>();
            foreach (var row in table)
            {
                try
                {
                    result.Add(new InactiveFarm()
                    {
                        //status = row[0]
                        distance = Int32.Parse(row[1]),
                        coord = MapParser.GetCoordinates(row[2]),
                        namePlayer = row[3],
                        nameAlly = row[4],
                        nameVill = row[5],
                        population = Int32.Parse(row[6])
                        //functions = row[7]
                    });
                }
                catch (Exception) { }
            }

            return result;
        }

        /// <summary>
        /// This function for www.inactivesearch.it
        /// </summary>
        /// <returns></returns>
        private async Task<List<InactiveFarm>> GetFarms_InactiveSearchIt()
        {
            var serverUrl = acc.AccInfo.ServerUrl;
            // get serverUrl without https://
            var url = (new UriBuilder(serverUrl)).Host;
            Client.BaseUrl = new Uri($"https://www.inactivesearch.it/inactives/{url}");

            var request = new RestRequest($"?c={(int)coordinatesUc1.Coords.x}|{(int)coordinatesUc1.Coords.y}", Method.GET);
            request.AddHeader("Cookie", $"IS_filters=mnd=0&mxd={(int)Distance.Value}&d=4&mnp=0&mxp=100000&mnv=0&mxv=3000&mnc=0&mxc=0&iw=1&ha=&hp=&hr=0&ht=0&hg=0&hn=0&hf=undefined&hmv=1&hma=1&sn=1");

            var response = await Client.ExecuteAsync(request);

            if (response.StatusCode != HttpStatusCode.OK) throw new Exception("SendGetReq failed!\n" + response.Content);

            if (response.Content.Contains("Nothing found"))
            {
                return null;
            }

            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(response.Content);

            // table
            var table = doc.DocumentNode.SelectNodes("//table[@class='table table-condensed table-inactives table-shadow']//tbody") // they use myTable for naming their table ?_?
                        .Descendants("tr")
                        .Where(tr => tr.Elements("td").Count() > 1)
                        .Select(tr => tr.Elements("td").Select(td => td.InnerText.Trim().Replace("\t", "").Replace("\n", "")).ToList())
                        .ToList();

            var result = new List<InactiveFarm>();
            foreach (var row in table)
            {
                try
                {
                    result.Add(new InactiveFarm()
                    {
                        distance = int.Parse(row[0]),
                        coord = MapParser.GetCoordinates(row[1]),
                        nameVill = row[2],
                        // row[3] hide village button
                        // row[4] attack button
                        population = int.Parse(row[5]),
                        // row[6], [7], [8], [9] population previous day
                        namePlayer = row[10],
                        nameAlly = row[11],
                    });
                }
                catch (Exception) { }
            }

            return result;
        }

        private async void button2_Click(object sender, System.EventArgs e)
        {
            var serverCode = await GetServerCode();
            List<InactiveFarm> Inactives;
            if (!string.IsNullOrEmpty(serverCode))
            {
                Inactives = await GetFarms_TravianstatDE(serverCode);
            }
            else
            {
                Inactives = await GetFarms_InactiveSearchIt();
            }

            InactiveList.Items.Clear();

            if (Inactives == null) return;

            for (int i = 0; i < Inactives.Count; i++)
            {
                var Inactive = Inactives[i];
                var item = new ListViewItem();

                item.SubItems[0].Text = (i + 1).ToString();
                item.SubItems.Add(Inactive.distance.ToString());
                item.SubItems.Add(Inactive.coord.ToString());
                item.SubItems.Add(Inactive.namePlayer);
                item.SubItems.Add(Inactive.nameAlly);
                item.SubItems.Add(Inactive.nameVill);
                item.SubItems.Add(Inactive.population.ToString());
                item.ForeColor = Color.White;

                InactiveList.Items.Add(item);
            }
        }

        private void comboBoxVillages_SelectedIndexChanged(object sender, EventArgs e)
        {
            var vill = acc.Villages[comboBoxVillages.SelectedIndex];
            if (vill == null) return;
            coordinatesUc1.Coords = vill.Coordinates;
        }

        private void InactiveList_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            //found at https://docs.microsoft.com/en-US/troubleshoot/dotnet/csharp/sort-listview-by-column

            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == lvwColumnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (lvwColumnSorter.Order == SortOrder.Ascending)
                {
                    lvwColumnSorter.Order = SortOrder.Descending;
                }
                else
                {
                    lvwColumnSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            this.InactiveList.Sort();
        }

        private void InactiveList_SelectedIndexChanged(object sender, EventArgs e)
        {
            countFarmChose.Text = InactiveList.SelectedItems.Count.ToString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}