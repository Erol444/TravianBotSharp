using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace TbsCrossPlatform.Services
{
    internal class UseragentService : IUseragentService
    {
        private readonly Random rnd = new();

        private List<string> userAgentList;
        private DateTime dateTime;
        private readonly RestClient client = new();

        private const string userAgentUrl = "https://raw.githubusercontent.com/vinaghost/user-agent/main/user-agent.json";
        private readonly string useragentFilePath = Path.Combine(AppContext.BaseDirectory, "useragent.json");

        public UseragentService()
        {
            Load();
        }

        public string Get()
        {
            var index = rnd.Next(0, userAgentList.Count);

            var result = userAgentList[index];
            userAgentList.RemoveAt(index);

            Save();

            return result;
        }

        private void Load()
        {
            if (!File.Exists(useragentFilePath)) Update();
            var userAgentJsonString = File.ReadAllText(useragentFilePath);
            var modelLoaded = JsonSerializer.Deserialize<Model>(userAgentJsonString);
            userAgentList = modelLoaded.UserAgentList;
            dateTime = modelLoaded.DateTime;

            if (dateTime < DateTime.Now || userAgentList.Count < 1000) Update();
        }

        private void Save()
        {
            var userAgentJsonString = JsonSerializer.Serialize(new Model
            {
                UserAgentList = userAgentList,
                DateTime = dateTime,
            });
            File.WriteAllText(useragentFilePath, userAgentJsonString);
        }

        private void Update()
        {
            var reqest = new RestRequest(userAgentUrl);
            var task = client.GetAsync(reqest);
            task.Wait();
            string responseBody = task.Result.Content;
            userAgentList = JsonSerializer.Deserialize<List<string>>(responseBody);
            dateTime = DateTime.Now.AddMonths(1);
            Save();
        }

        private class Model
        {
            public List<string> UserAgentList { get; set; }
            public DateTime DateTime { get; set; }
        }
    }
}