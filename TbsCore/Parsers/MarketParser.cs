using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using TbsCore.Models.ResourceModels;

namespace TbsCore.Parsers
{
    public static class MarketParser
    {
        /// <summary>
        /// Returns merchants available/all
        /// </summary>
        /// <param name="htmlDoc"></param>
        /// <returns>(Available, All)</returns>
        public static (int, int) GetMerchantsNumber(HtmlDocument htmlDoc)
        {
            var tradersNode = htmlDoc.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("traderCount"));
            if (tradersNode == null) return (0, 0);

            var text = tradersNode.InnerText.Split('/');
            int current = (int)Parser.RemoveNonNumeric(text[0]);
            int all = (int)Parser.RemoveNonNumeric(text[1]);
            return (current, all);
        }

        /// <summary>
        /// Get soonest time own merchant arrive (ignore fact they go away or go back village)
        /// </summary>
        /// <param name="htmlDoc">Html</param>
        public static DateTime GetSoonestMerchant(HtmlDocument htmlDoc)
        {
            // i know id is incoming, but trust me =))
            var divOwnMerchants = htmlDoc.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("incomingMerchants"));

            var table = divOwnMerchants.Descendants("table").FirstOrDefault();

            var soonest = DateTime.Now.Add(TimeParser.ParseTimer(table));

            return soonest;
        }

        /// <summary>
        /// 2 if plus account
        /// 3 if gold club
        /// 1 otherwise
        /// </summary>
        public static int MaxMerchantTimes(HtmlDocument html)
        {
            var select = html.GetElementbyId("x2");
            if (select == null) return 1;
            return select.ChildNodes.Count(x => x.Name == "option");
        }

        public static int MerchantsCapacity(HtmlDocument html)
        {
            var carry = html.GetElementbyId("build").Descendants("div").FirstOrDefault(x => x.HasClass("carry"));
            return (int)Parser.RemoveNonNumeric(carry.InnerText);
        }

        public static List<MerchantsUnderWay> ParseTransits(HtmlDocument htmlDoc)
        {
            var ret = new List<MerchantsUnderWay>();
            var formulat = htmlDoc.GetElementbyId("merchantsOnTheWayFormular");

            // Returning, Incoming, Outgoing
            var num = formulat.ChildNodes.Count(x => x.HasClass("spacer"));
            (int available, int _) = GetMerchantsNumber(htmlDoc);
            bool returning = formulat.Descendants("span").Any(x => x.HasClass("none"));
            var cap = MerchantsCapacity(htmlDoc);

            var traders = new List<MerchantsUnderWay>[num];
            int type = -1;
            foreach (var child in formulat.ChildNodes)
            {
                if (child.HasClass("spacer"))
                {
                    type++;
                    traders[type] = new List<MerchantsUnderWay>();
                    var a = child.InnerText;
                    continue;
                }
                else if (child.HasClass("traders"))
                {
                    var time = DateTime.Now.Add(TimeParser.ParseTimer(child));
                    var villid = (int)Parser.RemoveNonNumeric(child.Descendants("td").First(x => x.HasClass("dorf")).Descendants("a").First().GetAttributeValue("href", ""));
                    var repeat = child.Descendants("div").FirstOrDefault(x => x.HasClass("repeat"));
                    int times = repeat == null ? 1 : (int)Parser.RemoveNonNumeric(repeat.InnerText);

                    traders[type].Add(new MerchantsUnderWay()
                    {
                        Arrival = time,
                        TargetVillageId = villid,
                        RepeatTimes = times,
                        Resources = ResourceParser.ParseResourcesMerchants(child),
                    });
                }
            }
            return ret;
        }

        /*
         * Inside form id=merchantsOnTheWayFormular
         * 
         * TODO: add parsing of merchants on the way. span=None means merchants are returning to your village.
         * 
         <h4 class="spacer">Returning Merchants:</h4><table class="traders" cellspacing="1" cellpadding="1"><thead><tr><td><a href="spieler.php?uid=107">test1</a></td><td class="dorf">Return from village <a href="karte.php?d=35516"></a><a href="karte.php?d=35516">011</a></td></tr></thead><tbody><tr><th>Arrival</th><td><div class="in">in <span class="timer" counting="down" value="13">0:00:13</span> hour.</div><div class="at"> at 16:01</div></td></tr><tr class="res"><th>Resources</th><td colspan="1"><span class="none">
        
		<img class="r1" src="img/x.gif" alt="Lumber"> 0
		<img class="r2" src="img/x.gif" alt="Clay"> 0
		<img class="r3" src="img/x.gif" alt="Iron"> 16000
		<img class="r4" src="img/x.gif" alt="Crop"> 0
		</span></td></tr></tbody></table><table class="traders" cellspacing="1" cellpadding="1"><thead><tr><td><a href="spieler.php?uid=107">test1</a></td><td class="dorf">Return from village <a href="karte.php?d=35310"></a><a href="karte.php?d=35310">004</a></td></tr></thead><tbody><tr><th>Arrival</th><td><div class="in">in <span class="timer" counting="down" value="29">0:00:29</span> hour.</div><div class="at"> at 16:01</div></td></tr><tr class="res"><th>Resources</th><td colspan="1"><span class="none">
        
		<img class="r1" src="img/x.gif" alt="Lumber"> 6000
		<img class="r2" src="img/x.gif" alt="Clay"> 8200
		<img class="r3" src="img/x.gif" alt="Iron"> 1800
		<img class="r4" src="img/x.gif" alt="Crop"> 0
		</span></td></tr></tbody></table><table class="traders" cellspacing="1" cellpadding="1"><thead><tr><td><a href="spieler.php?uid=107">test1</a></td><td class="dorf">Return from village <a href="karte.php?d=35108"></a><a href="karte.php?d=35108">008</a></td></tr></thead><tbody><tr><th>Arrival</th><td><div class="in">in <span class="timer" counting="down" value="31">0:00:31</span> hour.</div><div class="at"> at 16:01</div></td></tr><tr class="res"><th>Resources</th><td colspan="1"><span class="none">
        
		<img class="r1" src="img/x.gif" alt="Lumber"> 0
		<img class="r2" src="img/x.gif" alt="Clay"> 78000
		<img class="r3" src="img/x.gif" alt="Iron"> 120000
		<img class="r4" src="img/x.gif" alt="Crop"> 0
		</span></td></tr></tbody></table><table class="traders" cellspacing="1" cellpadding="1"><thead><tr><td><a href="spieler.php?uid=107">test1</a></td><td class="dorf">Return from village <a href="karte.php?d=25038"></a><a href="karte.php?d=25038">001 A</a></td></tr></thead><tbody><tr><th>Arrival</th><td><div class="in">in <span class="timer" counting="down" value="118">0:01:58</span> hour.</div><div class="at"> at 16:03</div></td></tr><tr class="res"><th>Resources</th><td colspan="1"><span class="none">
        
		<img class="r1" src="img/x.gif" alt="Lumber"> 0
		<img class="r2" src="img/x.gif" alt="Clay"> 0
		<img class="r3" src="img/x.gif" alt="Iron"> 99000
		<img class="r4" src="img/x.gif" alt="Crop"> 0
		</span></td></tr></tbody></table><h4 class="spacer">Incoming Merchants:</h4><table class="traders" cellspacing="1" cellpadding="1"><thead><tr><td><a href="spieler.php?uid=107">test1</a></td><td class="dorf">Receive from village <a href="karte.php?d=34104"></a><a href="karte.php?d=34104">028</a></td></tr></thead><tbody><tr><th>Arrival</th><td><div class="in">in <span class="timer" counting="down" value="30">0:00:30</span> hour.</div><div class="at"> at 16:01</div></td></tr><tr class="res"><th>Resources</th><td colspan="1"><span class="">
        
		<img class="r1" src="img/x.gif" alt="Lumber"> 0
		<img class="r2" src="img/x.gif" alt="Clay"> 0
		<img class="r3" src="img/x.gif" alt="Iron"> 0
		<img class="r4" src="img/x.gif" alt="Crop"> 6200
		</span></td></tr></tbody></table><h4 class="spacer">Ongoing Merchants:</h4><table class="traders" cellspacing="1" cellpadding="1"><thead><tr><td><a href="spieler.php?uid=107">test1</a></td><td class="dorf">Send to village <a href="karte.php?d=36118"></a><a href="karte.php?d=36118">023</a></td></tr></thead><tbody><tr><th>Arrival</th><td><div class="in">in <span class="timer" counting="down" value="29">0:00:29</span> hour.</div><div class="at"> at 16:01</div></td></tr><tr class="res"><th>Resources</th><td colspan="1"><span class="">
        
		<img class="r1" src="img/x.gif" alt="Lumber"> 0
		<img class="r2" src="img/x.gif" alt="Clay"> 0
		<img class="r3" src="img/x.gif" alt="Iron"> 3400
		<img class="r4" src="img/x.gif" alt="Crop"> 0
		</span></td></tr></tbody></table><table class="traders" cellspacing="1" cellpadding="1"><thead><tr><td><a href="spieler.php?uid=107">test1</a></td><td class="dorf">Send to village <a href="karte.php?d=35913"></a><a href="karte.php?d=35913">021</a></td></tr></thead><tbody><tr><th>Arrival</th><td><div class="in">in <span class="timer" counting="down" value="37">0:00:37</span> hour.</div><div class="at"> at 16:01</div></td></tr><tr class="res"><th>Resources</th><td colspan="1"><span class="">
        
		<img class="r1" src="img/x.gif" alt="Lumber"> 0
		<img class="r2" src="img/x.gif" alt="Clay"> 0
		<img class="r3" src="img/x.gif" alt="Iron"> 2000
		<img class="r4" src="img/x.gif" alt="Crop"> 0
		</span></td></tr></tbody></table><table class="traders" cellspacing="1" cellpadding="1"><thead><tr><td><a href="spieler.php?uid=107">test1</a></td><td class="dorf">Send to village <a href="karte.php?d=25442"></a><a href="karte.php?d=25442">002 A</a></td></tr></thead><tbody><tr><th>Arrival</th><td><div class="in">in <span class="timer" counting="down" value="313">0:05:13</span> hour.</div><div class="at"> at 16:06</div></td></tr><tr class="res"><th>Resources</th><td colspan="1"><span class="">
        
		<img class="r1" src="img/x.gif" alt="Lumber"> 0
		<img class="r2" src="img/x.gif" alt="Clay"> 100000
		<img class="r3" src="img/x.gif" alt="Iron"> 140000
		<img class="r4" src="img/x.gif" alt="Crop"> 0
		</span></td></tr></tbody></table><table class="traders" cellspacing="1" cellpadding="1"><thead><tr><td><a href="spieler.php?uid=107">test1</a></td><td class="dorf">Send to village <a href="karte.php?d=25241"></a><a href="karte.php?d=25241">003 A</a></td></tr></thead><tbody><tr><th>Arrival</th><td><div class="in">in <span class="timer" counting="down" value="342">0:05:42</span> hour.</div><div class="at"> at 16:07</div></td></tr><tr class="res"><th>Resources</th><td colspan="1"><span class="">
        
		<img class="r1" src="img/x.gif" alt="Lumber"> 20000
		<img class="r2" src="img/x.gif" alt="Clay"> 63000
		<img class="r3" src="img/x.gif" alt="Iron"> 66000
		<img class="r4" src="img/x.gif" alt="Crop"> 0
		</span></td></tr></tbody></table>
         
         */
    }
}