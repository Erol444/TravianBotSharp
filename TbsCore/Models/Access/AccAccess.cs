using System;
using System.Collections.Generic;
using System.IO;

namespace TravBotSharp.Files.Models.AccModels
{
    public class AccessInfo
    {
        public List<Access> AllAccess { get; set; }
        public int CurrentAccess { get; set; }
        public Access GetCurrentAccess() => AllAccess[CurrentAccess];

        public void Init()
        {
            AllAccess = new List<Access>();
        }

        public Access GetNewAccess()
        {
            CurrentAccess++;

            if (CurrentAccess >= AllAccess.Count) CurrentAccess = 0;

            var access = GetCurrentAccess();
            access.LastUsed = DateTime.Now;

            return access;
        }

        public void AddNewAccess(Access access)
        {
            AllAccess.Add(access);
        }
        public void AddNewAccess(string pw, string proxy, int port)
        {
            string userAgent = "";
            //randomly select a UserAgent
            try
            {
                // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader("data/useragents.txt"))
                {
                    var agents = sr.ReadToEnd().Split('\n');
                    Random rand = new Random();
                    int i = rand.Next(agents.Length);
                    userAgent = agents[i];
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("AddNewAccess failed, Exception thrown: " + e.Message);
                userAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/78.0.3904.70 Safari/537.36";
            }

            var accs = new Access()
            {
                Password = pw,
                Proxy = proxy,
                ProxyPort = port,
                UserAgent = userAgent,
                IsSittering = false,
                LastUsed = DateTime.MinValue
            };
            AllAccess.Add(accs);
        }
    }
}