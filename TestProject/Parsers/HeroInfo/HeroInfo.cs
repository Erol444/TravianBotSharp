using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject.Parsers
{
    [TestClass]
    public class HeroInfo
    {
        [TestMethod]
        public void TTWarsGetHealth()
        {
            var doc = new HtmlDocument();
            doc.Load("Parsers/HeroInfo/logo/TTWars.html");
            var value = TTWarsCore.Parsers.HeroInfo.GetHealth(doc);
            Assert.AreEqual(100, value);
        }

        [TestMethod]
        public void TravianOfficialGetHealth()
        {
            var doc = new HtmlDocument();
            doc.Load("Parsers/HeroInfo/logo/Travian.html");
            var value = TravianOfficialCore.Parsers.HeroInfo.GetHealth(doc);
            Assert.AreEqual(100, value);
        }

        [TestMethod]
        public void TravianOfficialHeroGetHealth()
        {
            var doc = new HtmlDocument();
            doc.Load("Parsers/HeroInfo/logo/TravianHeroUI.html");
            var value = TravianOfficialNewHeroUICore.Parsers.HeroInfo.GetHealth(doc);
            Assert.AreEqual(100, value);
        }

        [TestMethod]
        public void TTWarsGetStatus()
        {
            var doc = new HtmlDocument();
            doc.Load("Parsers/HeroInfo/logo/TTWars.html");
            var value = TTWarsCore.Parsers.HeroInfo.GetStatus(doc);
            Assert.AreEqual(1, value);
        }

        [TestMethod]
        public void TravianOfficialGetStatus()
        {
            var doc = new HtmlDocument();
            doc.Load("Parsers/HeroInfo/logo/Travian.html");
            var value = TravianOfficialCore.Parsers.HeroInfo.GetStatus(doc);
            Assert.AreEqual(1, value);
        }

        [TestMethod]
        public void TravianOfficialHeroGetStatus()
        {
            var doc = new HtmlDocument();
            doc.Load("Parsers/HeroInfo/logo/TravianHeroUI.html");
            var value = TravianOfficialNewHeroUICore.Parsers.HeroInfo.GetStatus(doc);
            Assert.AreEqual(1, value);
        }

        [TestMethod]
        public void TTWarsGetAdventureNum()
        {
            var doc = new HtmlDocument();
            doc.Load("Parsers/HeroInfo/logo/TTWars.html");
            var value = TTWarsCore.Parsers.HeroInfo.GetAdventureNum(doc);
            Assert.AreEqual(3, value);
        }

        [TestMethod]
        public void TravianOfficialGetAdventureNum()
        {
            var doc = new HtmlDocument();
            doc.Load("Parsers/HeroInfo/logo/Travian.html");
            var value = TravianOfficialCore.Parsers.HeroInfo.GetAdventureNum(doc);
            Assert.AreEqual(13, value);
        }

        [TestMethod]
        public void TravianOfficialHeroGetAdventureNum()
        {
            var doc = new HtmlDocument();
            doc.Load("Parsers/HeroInfo/logo/TravianHeroUI.html");
            var value = TravianOfficialNewHeroUICore.Parsers.HeroInfo.GetAdventureNum(doc);
            Assert.AreEqual(3, value);
        }

        [TestMethod]
        public void TTWarsGetItems()
        {
            var doc = new HtmlDocument();
            doc.Load("Parsers/HeroInfo/inventory/TTWars.html");
            var value = TTWarsCore.Parsers.HeroInfo.GetItems(doc);
            Assert.AreEqual(5, value.Count);
        }

        [TestMethod]
        public void TravianOfficialGetItems()
        {
            var doc = new HtmlDocument();
            doc.Load("Parsers/HeroInfo/inventory/Travian.html");
            var value = TravianOfficialCore.Parsers.HeroInfo.GetItems(doc);
            Assert.AreEqual(19, value.Count);
        }

        [TestMethod]
        public void TravianOfficialHeroGetItems()
        {
            var doc = new HtmlDocument();
            doc.Load("Parsers/HeroInfo/inventory/TravianHeroUI.html");
            var value = TravianOfficialNewHeroUICore.Parsers.HeroInfo.GetItems(doc);
            Assert.AreEqual(11, value.Count);
        }

        [TestMethod]
        public void TTWarsGetAdventures()
        {
            var doc = new HtmlDocument();
            doc.Load("Parsers/HeroInfo/adventure/TTWars.html");
            var value = TTWarsCore.Parsers.HeroInfo.GetAdventures(doc);
            Assert.AreEqual(11, value.Count);
        }

        [TestMethod]
        public void TravianOfficialGetAdventures()
        {
            var doc = new HtmlDocument();
            doc.Load("Parsers/HeroInfo/adventure/Travian.html");
            var value = TravianOfficialCore.Parsers.HeroInfo.GetAdventures(doc);
            Assert.AreEqual(21, value.Count);
        }

        [TestMethod]
        public void TravianOfficialHeroGetAdventures()
        {
            var doc = new HtmlDocument();
            doc.Load("Parsers/HeroInfo/adventure/TravianHeroUI.html");
            var value = TravianOfficialNewHeroUICore.Parsers.HeroInfo.GetAdventures(doc);
            Assert.AreEqual(1, value.Count);
        }
    }
}