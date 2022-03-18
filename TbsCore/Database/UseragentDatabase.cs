using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using TbsCore.Extensions;
using TbsCore.Helpers;

namespace TbsCore.Database
{
    public class UseragentDatabase
    {
        private static readonly HttpClient client = new HttpClient();
        private static readonly Random rnd = new Random();

        private UseragentDatabase()
        {
        }

        private static UseragentDatabase instance = null;

        public static UseragentDatabase Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new UseragentDatabase();
                }
                return instance;
            }
        }

        private List<string> _userAgentList { get; set; }
        private DateTime _dateTime { get; set; }

        private const string _userAgentUrl = "https://raw.githubusercontent.com/vinaghost/user-agent/main/user-agent.json";

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
            File.WriteAllText(IoHelperCore.UseragentPath, userAgentJsonString);
        }

        public void Load()
        {
            if (!IoHelperCore.UserAgentExists())
            {
                Update();
                return;
            }
            var userAgentJsonString = File.ReadAllText(IoHelperCore.UseragentPath);
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
            var accounts = DbRepository.GetAccounts();
            bool duplicate;
            using (var hash = SHA256.Create())
            {
                int index;
                do
                {
                    index = rnd.Next(0, _userAgentList.Count);

                    var byteArray = hash.ComputeHash(Encoding.UTF8.GetBytes(_userAgentList[index]));
                    var userAgentHash = BitConverter.ToString(byteArray).ToLower();
                    duplicate = false;

                    foreach (var account in accounts)
                    {
                        foreach (var proxy in account.Access.AllAccess)
                        {
                            if (proxy.UseragentHash?.Equals(userAgentHash) ?? false)
                            {
                                duplicate = true;
                                break; // proxy loop
                            }
                        }
                        if (duplicate) break; // account loop
                    }
                }
                while (duplicate);
                return _userAgentList[index]; // i dont think this will loop over 5000 times =))            }
            }
        }

        private class Model
        {
            public List<string> UserAgentList { get; set; }
            public DateTime DateTime { get; set; }
        }
    }
}