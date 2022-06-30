using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace MainCore.Services
{
    public class UseragentManager : IUseragentManager
    {
        public UseragentManager(IRestClientManager restClientManager)
        {
            _restClient = restClientManager.Get(-1);
        }

        private List<string> _userAgentList { get; set; }
        private DateTime _dateTime { get; set; }

        private const string _userAgentUrl = "https://raw.githubusercontent.com/vinaghost/user-agent/main/user-agent.json";

        private async Task Update()
        {
            var reqest = new RestRequest(_userAgentUrl);
            var result = await _restClient.GetAsync(reqest);
            var responseBody = result.Content;
            _userAgentList = JsonSerializer.Deserialize<List<string>>(responseBody);

            _dateTime = DateTime.Now.AddMonths(1); // need update after 1 month, thought so
            Save();
        }

        private void Save()
        {
            var userAgentJsonString = JsonSerializer.Serialize(new Model
            {
                UserAgentList = _userAgentList,
                DateTime = _dateTime,
            });
            var path = Path.Combine(AppContext.BaseDirectory, "Data", "useragent.json");

            File.WriteAllText(path, userAgentJsonString);
        }

        public async Task Load()
        {
            var pathFolder = Path.Combine(AppContext.BaseDirectory, "Data");
            if (!Directory.Exists(pathFolder)) Directory.CreateDirectory(pathFolder);
            var pathFile = Path.Combine(pathFolder, "useragent.json");
            if (!File.Exists(pathFile))
            {
                await Update();
                return;
            }
            var userAgentJsonString = File.ReadAllText(pathFile);
            var modelLoaded = JsonSerializer.Deserialize<Model>(userAgentJsonString);
            _userAgentList = modelLoaded.UserAgentList;
            _dateTime = modelLoaded.DateTime;

            if (_dateTime < DateTime.Now || _userAgentList.Count < 1000)
            {
                await Update();
            }
        }

        public string Get()
        {
            var index = rnd.Next(0, _userAgentList.Count);
            var result = _userAgentList[index];
            _userAgentList.RemoveAt(index);
            Save();
            return result;
        }

        private readonly RestClient _restClient;
        private readonly Random rnd = new();

        private class Model
        {
            public List<string> UserAgentList { get; set; }
            public DateTime DateTime { get; set; }
        }
    }
}