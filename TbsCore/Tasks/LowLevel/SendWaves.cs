using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.AccModels;
using TravBotSharp.Files.Models.AttackModels;
using TravBotSharp.Files.Parsers;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class SendWaves : BotTask
    {
        public List<SendWaveModel> SendWaveModels { get; set; }
        // Time difference between server and computer time
        private TimeSpan timeDifference;
        private DateTime lastArriveAt;
        private string[] hiddenFields = new string[] { "timestamp", "timestamp_checksum", "b", "currentDid", "mpvt_token" };

        public override async Task<TaskRes> Execute(Account acc)
        {
            var wb = acc.Wb.Driver;
            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/build.php?tt=2&id=39");

            var wavesReady = new List<WaveReadyModel>();

            // Get request time for getting more accurate attacks
            Ping ping = new Ping();
            PingReply reply = ping.Send(IoHelperCore.UrlRemoveHttp(acc.AccInfo.ServerUrl));
            var reqTripMs = (int)(reply.RoundtripTime / 2);

            Random rnd = new Random();

            // Prepare the waves
            for (int i = 0; i < SendWaveModels.Count; i++)
            {
                Console.WriteLine(DateTime.Now + "Send wave 1");
                await Task.Delay(rnd.Next(800, 1000));

                var htmlDoc1 = await HttpHelper.SendGetReq(acc, "/build.php?tt=2&id=39");

                var build = htmlDoc1.GetElementbyId("build");

                var values = new Dictionary<string, string>
                {
                    {"dname", ""}, // Name of the village, empty. Bot uses coordinates
                    {"x", SendWaveModels[i].Coordinates.x.ToString()},
                    {"y", SendWaveModels[i].Coordinates.y.ToString()},
                    {"c", ((int)SendWaveModels[i].MovementType).ToString()}, // 2 = reinformance, 3 = attack, 4 = raid
                    {"s1", "ok"}
                };

                foreach (var hidden in hiddenFields)
                {
                    var value = build.Descendants("input").FirstOrDefault(x => x.GetAttributeValue("name", "") == hidden);
                    if (value == null) continue;
                    values.Add(hidden, value.GetAttributeValue("value", ""));
                }

                // Get available troops
                int[] troopsAtHome = TroopsMovementParser.GetTroopsInRallyPoint(htmlDoc1);
                // Send all off dirty hack
                if (SendWaveModels[i].AllOff)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        var troop = TroopsHelper.TroopFromInt(acc, j);
                        if (TroopsHelper.IsTroopOffensive(troop) || TroopsHelper.IsTroopRam(troop))
                        {
                            SendWaveModels[i].Troops[j] = troopsAtHome[j];
                            troopsAtHome[j] = 0;
                        }
                    }
                }
                // Send fake attack dirty hack
                if (SendWaveModels[i].FakeAttack)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        if (troopsAtHome[j] > 19)
                        {
                            SendWaveModels[i].Troops[j] = 19;
                            troopsAtHome[j] -= 19;
                            break;
                        }
                    }
                }

                for (int j = 0; j < SendWaveModels[i].Troops.Length; j++)
                {
                    switch (acc.AccInfo.ServerVersion)
                    {
                        case Classificator.ServerVersionEnum.T4_4:
                            values.Add($"t{j + 1}", TroopCount(SendWaveModels[i].Troops[j]));
                            break;
                        case Classificator.ServerVersionEnum.T4_5:
                            values.Add($"troops[0][t{j + 1}]", TroopCount(SendWaveModels[i].Troops[j]));
                            break;
                    }

                }


                var content = new FormUrlEncodedContent(values);

                await Task.Delay(rnd.Next(800, 1000));

                var ret = await HttpHelper.SendPostReq(acc, content, "/build.php?tt=2&id=39");

                var htmlDoc2 = new HtmlAgilityPack.HtmlDocument();
                htmlDoc2.LoadHtml(ret);

                // Get time it takes for troops to the target, for later usage
                var timespan = TroopsMovementParser.GetTimeOfMovement(htmlDoc2);
                lastArriveAt = TroopsMovementParser.GetArrivalTime(htmlDoc2);
                if (timeDifference == TimeSpan.Zero)
                {
                    var serverTime = TimeParser.GetServerTime(htmlDoc2);
                    timeDifference = DateTime.Now - serverTime;
                    // Negate seconds and milliseconds in time difference.
                    var negateMillis = timeDifference.Milliseconds;
                    negateMillis += timeDifference.Seconds * 1000;
                    timeDifference = timeDifference.Subtract(new TimeSpan(0, 0, 0, 0, negateMillis));

                    var executeTime = CorrectExecuteTime(timespan);
                    if (DateTime.Now.AddMinutes(2) < executeTime)
                    {
                        // Restart this task at the correct time
                        this.NextExecute = executeTime;
                        return TaskRes.Executed;
                    }
                }

                //var ajaxToken = await HttpHelper.GetAjaxToken(wb);
                var values2 = new Dictionary<string, string>
                {
                    {"s1", "ok"},
                };

                // Copy all hidden names and values
                var build2 = htmlDoc2.GetElementbyId("build");
                var hiddens2 = build2.Descendants("input").Where(x => x.GetAttributeValue("type", "") == "hidden");
                foreach (var hidden2 in hiddens2)
                {
                    var val = hidden2.GetAttributeValue("value", "");
                    var name = hidden2.GetAttributeValue("name", "");
                    values2.Add(name, val);
                }

                // Add catapult targets
                string cataCount = "0";
                switch (acc.AccInfo.ServerVersion)
                {
                    case Classificator.ServerVersionEnum.T4_4:
                        values2.TryGetValue("t8", out cataCount);
                        break;
                    case Classificator.ServerVersionEnum.T4_5:
                        values2.TryGetValue("troops[0][t8]", out cataCount);
                        // If T4.5, we need to get value "a" as well - From Confirm button
                        var button = htmlDoc2.GetElementbyId("btn_ok");
                        string a = button.GetAttributeValue("value", "");
                        values2.Add("a", a);
                        break;
                }

                if (int.Parse(cataCount) > 0)
                {
                    values2.Add("ctar1", "99"); // 99 = Random, 1 = woodcuter, 2 = claypit..
                    values2.Add("ctar2", "99"); // 99 = Random
                }

                wavesReady.Add(new WaveReadyModel
                {
                    Content = new FormUrlEncodedContent(values2),
                    MovementTime = timespan
                });

                Console.WriteLine(DateTime.Now + "Send wave 2");
            }

            var waitMs = 1000 - DateTime.Now.Millisecond - reqTripMs;
            if (waitMs < 0) waitMs += 1000;
            var wait = new TimeSpan(0, 0, 0, 0, waitMs);

            // Calculate how much you need to wait so waves will arrive at correct time!
            var targetArrival = SendWaveModels.FirstOrDefault(x => x.Arrival != DateTime.MinValue).Arrival;
            TimeSpan waitForTarget = (targetArrival - lastArriveAt);
            if (waitForTarget > TimeSpan.Zero)
            {
                var waitForTargetSec = waitForTarget.Seconds + (waitForTarget.Minutes * 60) - 1; // -1 to compensate
                var waitForTargetTimeSpan = new TimeSpan(0, 0, waitForTargetSec);
                wait = wait.Add(waitForTargetTimeSpan);
            }
            await Task.Delay(wait);

            // Send the waves
            for (int i = 0; i < wavesReady.Count; i++)
            {
                // Wait +- 10% selected delay
                var delay = SendWaveModels[i].DelayMs;
                var delay10Percent = (int)delay / 10;
                await Task.Delay(rnd.Next(delay - delay10Percent, delay + delay10Percent));

                _ = HttpHelper.SendPostReq(acc, wavesReady[i].Content, "/build.php?tt=2&id=39");
            }

            await Task.Delay(AccountHelper.Delay() * 2);
            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/build.php?gid=16&tt=1&filter=2&subfilters=4");
            //Todo: check waves?
            return TaskRes.Executed;
        }

        private string TroopCount(int v)
        {
            if (v == 0) return "";
            return v.ToString();
        }

        private DateTime CorrectExecuteTime(TimeSpan troopTime)
        {
            var sec = 10; // Base value
            sec += 4 * this.SendWaveModels.Count(); // + 4 sec for each wave
            var targetArrival = SendWaveModels.FirstOrDefault(x => x.Arrival != DateTime.MinValue).Arrival;

            DateTime executeAt = targetArrival.Add(timeDifference);
            executeAt = executeAt.Subtract(troopTime);
            executeAt = executeAt.AddSeconds(-sec);
            return executeAt;
        }
    }
}
