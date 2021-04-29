using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using TbsCore.Models.AccModels;
using TbsCore.Models.SendTroopsModels;
using TbsCore.TravianData;
using TravBotSharp.Files.Helpers;
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
            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/build.php?id=39&tt=2");

            var wavesReady = new List<WaveReadyModel>();

            // Get request time for getting more accurate attacks
            Ping ping = new Ping();
            PingReply reply = ping.Send(IoHelperCore.UrlRemoveHttp(acc.AccInfo.ServerUrl));
            var reqTripMs = (int)(reply.RoundtripTime / 2);

            Random rnd = new Random();

            // Prepare the waves
            for (int i = 0; i < SendWaveModels.Count; i++)
            {
                await Task.Delay(rnd.Next(800, 1000));
                acc.Wb.Log($"Preparing {i + 1}. wave...");

                // TODO: eliminate the need of this first request, will send a second on each wave
                var htmlDoc1 = HttpHelper.SendGetReq(acc, "/build.php?tt=2&id=39");

                var build = htmlDoc1.GetElementbyId("build");

                var req = new RestRequest
                {
                    Resource = "/build.php?tt=2&id=39",
                    Method = Method.POST,
                };

                req.AddParameter("dname", "");
                req.AddParameter("x", SendWaveModels[i].TargetCoordinates.x.ToString());
                req.AddParameter("y", SendWaveModels[i].TargetCoordinates.y.ToString());
                req.AddParameter("c", ((int)SendWaveModels[i].MovementType).ToString());
                req.AddParameter("s1", "ok");

                foreach (var hidden in hiddenFields)
                {
                    var value = build.Descendants("input").FirstOrDefault(x => x.GetAttributeValue("name", "") == hidden);
                    if (value == null) continue;
                    req.AddParameter(hidden, value.GetAttributeValue("value", ""));
                }

                // Get available troops
                int[] troopsAtHome = TroopsMovementParser.GetTroopsInRallyPoint(htmlDoc1);

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
                    // If negative value, send all available units
                    if (SendWaveModels[i].Troops[j] < 0)
                    {
                        SendWaveModels[i].Troops[j] = troopsAtHome[j];
                        troopsAtHome[j] = 0;
                    }

                    switch (acc.AccInfo.ServerVersion)
                    {
                        case Classificator.ServerVersionEnum.T4_4:
                            req.AddParameter($"t{j + 1}", TroopCount(SendWaveModels[i].Troops[j]));
                            break;

                        case Classificator.ServerVersionEnum.T4_5:
                            req.AddParameter($"troops[0][t{j + 1}]", TroopCount(SendWaveModels[i].Troops[j]));
                            break;
                    }
                }
                await Task.Delay(rnd.Next(800, 1000));

                var ret = HttpHelper.SendPostReq(acc, req);

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
                    if (DateTime.Now.AddMinutes(1) < executeTime)
                    {
                        // Restart this task at the correct time

                        acc.Wb.Log($"Bot will send waves in {TimeHelper.InSeconds(executeTime)} seconds");
                        this.NextExecute = executeTime;
                        return TaskRes.Executed;
                    }
                }

                //var ajaxToken = await HttpHelper.GetAjaxToken(wb);
                var req2 = new RestRequest
                {
                    Resource = "/build.php?tt=2&id=39",
                    Method = Method.POST,
                };

                req2.AddParameter("s1", "ok");

                // Copy all hidden names and values
                var build2 = htmlDoc2.GetElementbyId("build");
                var hiddens2 = build2.Descendants("input").Where(x => x.GetAttributeValue("type", "") == "hidden");
                foreach (var hidden2 in hiddens2)
                {
                    var val = hidden2.GetAttributeValue("value", "");
                    var name = hidden2.GetAttributeValue("name", "");
                    req2.AddParameter(name, val);
                }

                // Add catapult targets
                string cataCount = "0";
                switch (acc.AccInfo.ServerVersion)
                {
                    case Classificator.ServerVersionEnum.T4_4:
                        cataCount = req2.Parameters.FirstOrDefault(x => x.Name == "t8").Value.ToString();
                        break;

                    case Classificator.ServerVersionEnum.T4_5:
                        cataCount = req2.Parameters.FirstOrDefault(x => x.Name == "troops[0][t8]").Value.ToString();
                        // If T4.5, we need to get value "a" as well - From Confirm button
                        var button = htmlDoc2.GetElementbyId("btn_ok");
                        string a = button.GetAttributeValue("value", "");
                        req2.AddParameter("a", a);
                        break;
                }

                if (int.Parse(cataCount) > 0)
                {
                    req2.AddParameter("ctar1", "99"); // 99 = Random, 1 = woodcuter, 2 = claypit..
                    req2.AddParameter("ctar2", "99"); // 99 = Random
                }

                wavesReady.Add(new WaveReadyModel
                {
                    Request = req2,
                    MovementTime = timespan
                });
            }

            var waitMs = 1000 - DateTime.Now.Millisecond - reqTripMs;
            if (waitMs < 0) waitMs += 1000;
            var wait = new TimeSpan(0, 0, 0, 0, waitMs);

            // Calculate how much you need to wait so waves arrive at the correct time!
            var targetArrival = SendWaveModels.FirstOrDefault(x => x.Arrival != DateTime.MinValue).Arrival;
            TimeSpan waitForTarget = (targetArrival - lastArriveAt);
            if (waitForTarget > TimeSpan.Zero)
            {
                var waitForTargetSec = (int)waitForTarget.TotalSeconds - 1; // -1 to compensate
                var waitForTargetTimeSpan = new TimeSpan(0, 0, waitForTargetSec);
                wait = wait.Add(waitForTargetTimeSpan);
            }
            await Task.Delay(wait);

            // Send the waves
            DateTime lastSent = default;
            for (int i = 0; i < wavesReady.Count; i++)
            {
                lastSent = DateTime.Now;
                acc.Wb.Log($"{DateTime.Now.Second}.{DateTime.Now.Millisecond}] Sending wave {i + 1}");
                _ = HttpHelper.SendPostReq(acc, wavesReady[i].Request);

                // Wait +- 10% selected delay
                var delay = SendWaveModels[i].DelayMs;

                // Negate the time it took to send the request
                delay -= (int)(DateTime.Now - lastSent).TotalMilliseconds;
                Console.WriteLine("New Delay " + delay);

                //var delay10Percent = (int)delay / 100;
                //await Task.Delay(rnd.Next(delay - delay10Percent, delay + delay10Percent));
                if (0 < delay) await Task.Delay(delay);
            }
            acc.Wb.Log($"Successfully sent {wavesReady.Count} waves!");

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
            sec += 2 * this.SendWaveModels.Count(); // + 4 sec for each wave
            var targetArrival = SendWaveModels.FirstOrDefault(x => x.Arrival != DateTime.MinValue).Arrival;

            DateTime executeAt = targetArrival.Add(timeDifference);
            executeAt = executeAt.Subtract(troopTime);
            executeAt = executeAt.AddSeconds(-sec);
            return executeAt;
        }
    }
}