using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System;
using System.Threading.Tasks;
using TbsCore.Models.AccModels;
using TravBotSharp.Files.Helpers;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    /// <summary>
    /// Just a random navigation event - to make bot less suspicious to Travian.
    /// </summary>
    public class RandomTask : BotTask
    {
        public int MinWait { get; set; } = 5000;
        public int MaxWait { get; set; } = 20000;

        private readonly string[] Url = new string[] {
            "/statistiken.php", // player
            "/statistiken.php?id=1", // ally
            "/statistiken.php?id=1&idSub=3", // ally TOP10
            "/statistiken.php?id=3", // hero
            "/statistiken.php?id=0&idSub=3", // player top10
            "/spieler.php", // profile
            "/statistiken.php?id=0&idSub=1", // players top attackers
            "/statistiken.php?id=0&idSub=2", // players top deffenders
            "/messages.php", // messages
            "/hero.php?t=4" // hero auctions
        };
        public override async Task<TaskRes> Execute(Account acc)
        {
            var ran = new Random();
            var id = ran.Next(0, Url.Length - 1);

            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}{Url[id]}");
            await Task.Delay(ran.Next(MinWait, MaxWait));

            return TaskRes.Executed;
        }
    }
}
