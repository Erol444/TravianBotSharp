using System;
using System.Net;
using System.Linq;
using System.Drawing;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows.Forms;

using RestSharp;

using TbsCore.Models.MapModels;
using TbsCore.Models.AccModels;

using TravBotSharp.Files.Parsers;
using TravBotSharp.Views;
using TravBotSharp.Interfaces;

namespace TravBotSharp
{
    public partial class FarmFinderUc : TbsBaseUc, ITbsUc
    {
        private ListViewColumnSorter lvwColumnSorter;

        private RestClient client;

        public FarmFinderUc()
        {
            InitializeComponent();
            client = new RestClient("https://travianstats.de/index.php");
        }

        public void Init()
        {
        }

        public void UpdateUc()
        {
            var acc = GetSelectedAcc();
            comboBoxVillages.Items.Clear();
            foreach (var vill in acc.Villages)
            {
                comboBoxVillages.Items.Add(vill.Name);
            }
            comboBoxVillages.SelectedIndex = 0;

            //update server code
            button2.Enabled = true;
            button2.Text = "SERVER CODE";

            //disable search button untill we got code server
            button1.Enabled = false;

            InactiveList.Items.Clear();
        }

        /// <summary>
        /// world code for our world from travianstats.de
        /// </summary>
        /// <returns></returns>
        private async Task<string> GetServerCode(Account acc)
        {
            // get serverUrl without https://
            var url = (new UriBuilder(acc.AccInfo.ServerUrl)).Host;

            //request to travaianstats.de
            var request = new RestRequest();

            var response = await client.ExecuteAsync(request);

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

        private async Task<List<InactiveFarmModel>> GetFarms(Account acc)
        {
            var request = new RestRequest($"?m=inactive_finder&w={acc.AccInfo.ServerCode}", Method.POST);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Cookie", $"tcn_world={acc.AccInfo.ServerCode}");
            request.AddParameter("m", "inactivefinder");
            request.AddParameter("w", acc.AccInfo.ServerCode);
            request.AddParameter("x", ((int)X.Value).ToString());
            request.AddParameter("y", ((int)Y.Value).ToString());
            request.AddParameter("distance", ((int)Distance.Value).ToString());

            var response = await client.ExecuteAsync(request);

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

            var result = new List<InactiveFarmModel>();
            foreach (var row in table)
            {
                result.Add(new InactiveFarmModel()
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

        private async void button1_Click(object sender, System.EventArgs e)
        {
            var acc = GetSelectedAcc();
            button1.Enabled = false;
            var Inactives = await GetFarms(acc);
            button1.Enabled = true;

            InactiveList.Items.Clear();

            if (Inactives == null) return;

            for (int i = 0; i < Inactives.Count; i++)
            {
                var Inactive = Inactives[i];
                var item = new ListViewItem();

                item.SubItems[0].Text = i.ToString();
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
            var acc = GetSelectedAcc();

            var vill = acc.Villages[comboBoxVillages.SelectedIndex];
            if (vill == null) return;
            X.Value = vill.Coordinates.x;
            Y.Value = vill.Coordinates.y;
        }

        /// <summary>
        /// Get server code
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void button2_Click(object sender, EventArgs e)
        {
            var acc = GetSelectedAcc();
            acc.AccInfo.ServerCode = await GetServerCode(acc);

            button2.Enabled = false;

            if (acc.AccInfo.ServerCode.Length < 1)
            {
                button2.Text = "CANNOT FIND";
                return;
            }

            button2.Text = acc.AccInfo.ServerCode;
            button1.Enabled = true;
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
    }

    public class InactiveFarmModel
    {
        public Coordinates coord { get; set; }
        public int distance { get; set; }
        public string namePlayer { get; set; }
        public string nameAlly { get; set; }
        public string nameVill { get; set; }
        public int population { get; set; }
    }
}