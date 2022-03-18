using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;

namespace TbsBrowser
{
    internal class Useragent
    {
        private static readonly HttpClient client = new();
        private static readonly Random rnd = new();

        private Useragent()
        {
        }

        private static Useragent instance = null;

        public static Useragent Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new();
                }
                return instance;
            }
        }

        private List<string> _userAgentList { get; set; }
        private DateTime _dateTime { get; set; }

        private const string _userAgentUrl = "https://raw.githubusercontent.com/vinaghost/user-agent/main/user-agent.json";
        private const string _userAgent = "useragent.json";

        private void Update()
        {
            var task = client.GetStringAsync(_userAgentUrl);
            task.Wait();
            string responseBody = task.Result;
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
            File.WriteAllText(_userAgent, userAgentJsonString);
        }

        public void Load()
        {
            if (!File.Exists(_userAgent))
            {
                Update();
                return;
            }
            var userAgentJsonString = File.ReadAllText(_userAgent);
            var modelLoaded = JsonSerializer.Deserialize<Model>(userAgentJsonString);
            _userAgentList = modelLoaded.UserAgentList;
            _dateTime = modelLoaded.DateTime;

            if (_dateTime.IsExpired())
            {
                Update();
            }
        }

        public string GetUserAgent()
        {
            int index = rnd.Next(0, _userAgentList.Count);
            var useragent = _userAgentList[index];
            _userAgentList.RemoveAt(index);
            Save();
            return useragent;
        }

        private class Model
        {
            public List<string> UserAgentList { get; set; }
            public DateTime DateTime { get; set; }
        }
    }

    public static class DateTimeExtension
    {
        public static bool IsExpired(this DateTime specificDate)
        {
            return specificDate < DateTime.Now;
        }
    }
}