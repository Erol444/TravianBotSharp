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
            Client = new RestClient("https://travianstats.de/index.php");

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

        private async Task<List<InactiveFarm>> GetFarms()
        {
            var serverCode = await GetServerCode();
            if (string.IsNullOrEmpty(serverCode))
            {
                string message = "Bot was unable to find the server code! This feature is only available for normal travian servers.";
                string caption = "Error getting server code";
                MessageBox.Show(message, caption, MessageBoxButtons.OK);
                return null;
            }

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

        private async void button2_Click(object sender, System.EventArgs e)
        {
            var Inactives = await GetFarms();

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