using System;
using System.Net;
using System.Linq;
using System.Threading.Tasks;

using RestSharp;
using HtmlAgilityPack;

using TbsCore.Models.AccModels;

using TravBotSharp.Views;
using TravBotSharp.Interfaces;

namespace TravBotSharp
{
    public partial class FarmFinderUc : TbsBaseUc, ITbsUc
    {
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
        }

        /// <summary>
        /// Server code for travianstats.de
        /// </summary>
        /// <returns></returns>
        private async Task<string> GetServerCode(Account acc)
        {
            //get serverUrl without https://
            var url = (new UriBuilder(acc.AccInfo.ServerUrl)).Host;

            //request to travaianstats.de
            var request = new RestRequest("", DataFormat.None);
            var response = await client.ExecuteAsync(request);

            if (response.StatusCode != HttpStatusCode.OK) throw new Exception("SendGetReq failed!\n" + response.Content);

            var doc = new HtmlDocument();
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

            return "404";
        }

        private async void button1_Click(object sender, System.EventArgs e)
        {
            var acc = GetSelectedAcc();
            acc.Wb.Log(await GetServerCode(acc));
        }
    }
}