using System;
using System.Threading.Tasks;
using TbsCore.Models.AccModels;

namespace TbsCore.Tasks.LowLevel
{
    /// <summary>
    /// Just a random navigation event - to make bot less suspicious to Travian.
    /// </summary>
    public class RandomTask : BotTask
    {
        public int MinWait { get; set; } = 5000;
        public int MaxWait { get; set; } = 20000;

        private readonly string[] UrlT4_4 = new string[] {
            "/statistiken.php", // player
            "/statistiken.php?id=1", // ally
            "/statistiken.php?id=1&idSub=3", // ally TOP10
            "/statistiken.php?id=3", // hero
            "/statistiken.php?id=0&idSub=3", // player top10
            "/spieler.php", // profile
            "/statistiken.php?id=0&idSub=1", // players top attackers
            "/statistiken.php?id=0&idSub=2", // players top deffenders
            "/messages.php", // messages
            "/hero.php?t=4", // Hero auctions
            "/reports.php",
            "/reports.php?t=1",
            "/reports.php?t=2",
            "/reports.php?t=3",
            "/reports.php?t=4",
        };

        private readonly string[] UrlT4_5 = new string[] {
            "/statistics", // player
            "/statistics/alliance", // ally
            "/statistics/alliance?idSub=3", // ally TOP10
            "/statistics/hero", // hero
            "/statistics/player?idSub=3", // player top10
            "/profile", // profile
            "/statistics/player?idSub=1", // players top attackers
            "/statistics/player?idSub=2", // players top deffenders
            "/messages.php", // messages
            "/hero/auction", // Hero auctions
            "/report",
            "/report/offensive",
            "/report/defensive",
            "/report/scouting",
            "/report/other",
        };

        public override async Task<TaskRes> Execute(Account acc)
        {
            string[] Urls = new string[0];
            switch (acc.AccInfo.ServerVersion)
            {
                case Helpers.Classificator.ServerVersionEnum.TTwars:
                    Urls = UrlT4_4;
                    break;

                case Helpers.Classificator.ServerVersionEnum.T4_5:
                    Urls = UrlT4_5;
                    break;
            }

            var ran = new Random();
            var id = ran.Next(0, Urls.Length - 1);

            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}{Urls[id]}");
            await Task.Delay(ran.Next(MinWait, MaxWait));

            return TaskRes.Executed;
        }
    }
}