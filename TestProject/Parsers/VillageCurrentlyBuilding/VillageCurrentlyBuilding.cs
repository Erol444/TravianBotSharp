using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace TestProject.Parsers
{
    [TestClass]
    public class VillageCurrentlyBuilding
    {
        private readonly HtmlDocument _ttwarsDoc = new();
        private readonly HtmlDocument _travianDoc = new();
        private readonly HtmlDocument _travianHeroDoc = new();

        [TestInitialize]
        public void InitializeTests()
        {
            _ttwarsDoc.Load("Parsers/VillageCurrentlyBuilding/TTWars.html");
            _travianDoc.Load("Parsers/VillageCurrentlyBuilding/Travian.html");
            _travianHeroDoc.Load("Parsers/VillageCurrentlyBuilding/TravianHeroUI.html");
        }

        [TestMethod]
        public void TTWarsGetNodes()
        {
            var nodes = TTWarsCore.Parsers.VillageCurrentlyBuilding.GetNodes(_ttwarsDoc);
            Assert.AreEqual(0, nodes.Count);
        }

        [TestMethod]
        public void TravianOfficialGetNodes()
        {
            var nodes = TravianOfficialCore.Parsers.VillageCurrentlyBuilding.GetNodes(_travianDoc);
            Assert.AreEqual(0, nodes.Count);
        }

        [TestMethod]
        public void TravianOfficialHeroGetNodes()
        {
            var nodes = TravianOfficialNewHeroUICore.Parsers.VillageCurrentlyBuilding.GetNodes(_travianHeroDoc);
            Assert.AreEqual(1, nodes.Count);
        }

        [TestMethod]
        [Ignore]
        public void TTWarsGetType()
        {
            var nodes = TTWarsCore.Parsers.VillageCurrentlyBuilding.GetNodes(_ttwarsDoc);
            var result = TTWarsCore.Parsers.VillageCurrentlyBuilding.GetType(nodes[0]);
            Assert.AreEqual("", result);
        }

        [TestMethod]
        [Ignore]
        public void TravianOfficialGetType()
        {
            var nodes = TravianOfficialCore.Parsers.VillageCurrentlyBuilding.GetNodes(_travianDoc);
            var result = TravianOfficialCore.Parsers.VillageCurrentlyBuilding.GetType(nodes[0]);
            Assert.AreEqual("", result);
        }

        [TestMethod]
        public void TravianOfficialHeroGetType()
        {
            var nodes = TravianOfficialNewHeroUICore.Parsers.VillageCurrentlyBuilding.GetNodes(_travianHeroDoc);
            var result = TravianOfficialNewHeroUICore.Parsers.VillageCurrentlyBuilding.GetType(nodes[0]);
            Assert.AreEqual("MainBuilding", result);
        }

        [TestMethod]
        [Ignore]
        public void TTWarsGetLevel()
        {
            var nodes = TTWarsCore.Parsers.VillageCurrentlyBuilding.GetNodes(_ttwarsDoc);
            var result = TTWarsCore.Parsers.VillageCurrentlyBuilding.GetLevel(nodes[0]);
            Assert.AreEqual("", result);
        }

        [TestMethod]
        [Ignore]
        public void TravianOfficialGetLevel()
        {
            var nodes = TravianOfficialCore.Parsers.VillageCurrentlyBuilding.GetNodes(_travianDoc);
            var result = TravianOfficialCore.Parsers.VillageCurrentlyBuilding.GetLevel(nodes[0]);
            Assert.AreEqual("", result);
        }

        [TestMethod]
        public void TravianOfficialHeroGetLevel()
        {
            var nodes = TravianOfficialNewHeroUICore.Parsers.VillageCurrentlyBuilding.GetNodes(_travianHeroDoc);
            var result = TravianOfficialNewHeroUICore.Parsers.VillageCurrentlyBuilding.GetLevel(nodes[0]);
            Assert.AreEqual(2, result);
        }

        [TestMethod]
        [Ignore]
        public void TTWarsGetDuration()
        {
            var nodes = TTWarsCore.Parsers.VillageCurrentlyBuilding.GetNodes(_ttwarsDoc);
            var result = TTWarsCore.Parsers.VillageCurrentlyBuilding.GetDuration(nodes[0]);
            Assert.AreEqual("", result);
        }

        [TestMethod]
        [Ignore]
        public void TravianOfficialGetDuration()
        {
            var nodes = TravianOfficialCore.Parsers.VillageCurrentlyBuilding.GetNodes(_travianDoc);
            var result = TravianOfficialCore.Parsers.VillageCurrentlyBuilding.GetDuration(nodes[0]);
            Assert.AreEqual("", result);
        }

        [TestMethod]
        public void TravianOfficialHeroGetDuration()
        {
            var nodes = TravianOfficialNewHeroUICore.Parsers.VillageCurrentlyBuilding.GetNodes(_travianHeroDoc);
            var result = TravianOfficialNewHeroUICore.Parsers.VillageCurrentlyBuilding.GetDuration(nodes[0]);
            Assert.AreEqual(TimeSpan.Parse("00:43:20"), result);
        }
    }
}