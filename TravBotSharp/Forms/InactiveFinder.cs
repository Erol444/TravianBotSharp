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

namespace TravBotSharp.Forms
{
    public partial class InactiveFinder : Form
    {
        private ListViewColumnSorter lvwColumnSorter;

        private RestClient Client;
        private List<Village> Villages;

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

        private string ServerUrl;
        public string ServerCode;

        public InactiveFinder(string ServerUrl, List<Village> Villages, Classificator.TribeEnum? tribeEnum)
        {
            InitializeComponent();
            // list view sorter
            lvwColumnSorter = new ListViewColumnSorter();
            this.InactiveList.ListViewItemSorter = lvwColumnSorter;

            //client http
            Client = new RestClient("https://travianstats.de/index.php");

            //
            this.ServerUrl = ServerUrl;
            this.ServerCode = "";
            this.Villages = Villages;
            troopsSelectorUc1.HeroEditable = false;
            troopsSelectorUc1.Init(tribeEnum ?? Classificator.TribeEnum.Nature);

            // UI
            foreach (var vill in Villages)
            {
                comboBoxVillages.Items.Add(vill.Name);
            }
            comboBoxVillages.SelectedIndex = 0;

            //disable search button untill we get code server
            button2.Enabled = false;
        }

        /// <summary>
        /// world code for our world from travianstats.de
        /// </summary>
        /// <returns></returns>
        private async Task<string> GetServerCode(string ServerUrl)
        {
            // get serverUrl without https://
            var url = (new UriBuilder(ServerUrl)).Host;

            //request to travaianstats.de
            var request = new RestRequest();

            var response = await Client.ExecuteAsync(request);

            if (response.StatusCode != HttpStatusCode.OK) throw new Exception("SendGetReq failed!\n" + response.Content);

            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(response.Content);
            // use this form to search code of our server
            var form = doc.GetElementbyId("welt");

            // find our server
            foreach (var node in form.Descendants())
            {
                if (node.InnerText.Contains(url))
                {
                    return node.Attributes["value"].Value;
                }
            }

            return "";
        }

        private async Task<List<InactiveFarm>> GetFarms(string ServerCode)
        {
            var request = new RestRequest($"?m=inactive_finder&w={ServerCode}", Method.POST);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Cookie", $"tcn_world={ServerCode}");
            request.AddParameter("m", "inactivefinder");
            request.AddParameter("w", ServerCode);
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

            return result;
        }

        private async void button2_Click(object sender, System.EventArgs e)
        {
            button2.Enabled = false;
            List<InactiveFarm> Inactives;
            while (true)
            {
                try
                {
                    Inactives = await GetFarms(ServerCode);
                    break;
                }
                catch (Exception)
                {
                    continue;
                }
            }

            button2.Enabled = true;

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
            var vill = Villages[comboBoxVillages.SelectedIndex];
            if (vill == null) return;
            coordinatesUc1.Coords = vill.Coordinates;
        }

        /// <summary>
        /// Get server code
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            ServerCode = await GetServerCode(ServerUrl);

            button1.Enabled = false;

            if (ServerCode.Length < 1)
            {
                button1.Text = "CANNOT FIND";
                return;
            }

            button1.Text = ServerCode;
            button2.Enabled = true;
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