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
            _ttwarsDoc.Load("TestFile/Dorf1/TTWars.html");
            _travianDoc.Load("TestFile/Dorf1/Travian.html");
            _travianHeroDoc.Load("TestFile/Dorf1/TravianHeroUI.html");
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
            var nodes = TravianOffcialCore.Parsers.VillageCurrentlyBuilding.GetNodes(_travianDoc);
            Assert.AreEqual(0, nodes.Count);
        }

        [TestMethod]
        public void TravianOfficialHeroGetNodes()
        {
            var nodes = TravianOfficalNewHeroUICore.Parsers.VillageCurrentlyBuilding.GetNodes(_travianHeroDoc);
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
            var nodes = TravianOffcialCore.Parsers.VillageCurrentlyBuilding.GetNodes(_travianDoc);
            var result = TravianOffcialCore.Parsers.VillageCurrentlyBuilding.GetType(nodes[0]);
            Assert.AreEqual("", result);
        }

        [TestMethod]
        public void TravianOfficialHeroGetType()
        {
            var nodes = TravianOfficalNewHeroUICore.Parsers.VillageCurrentlyBuilding.GetNodes(_travianHeroDoc);
            var result = TravianOffcialCore.Parsers.VillageCurrentlyBuilding.GetType(nodes[0]);
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
            var nodes = TravianOffcialCore.Parsers.VillageCurrentlyBuilding.GetNodes(_travianDoc);
            var result = TravianOffcialCore.Parsers.VillageCurrentlyBuilding.GetLevel(nodes[0]);
            Assert.AreEqual("", result);
        }

        [TestMethod]
        public void TravianOfficialHeroGetLevel()
        {
            var nodes = TravianOfficalNewHeroUICore.Parsers.VillageCurrentlyBuilding.GetNodes(_travianHeroDoc);
            var result = TravianOffcialCore.Parsers.VillageCurrentlyBuilding.GetLevel(nodes[0]);
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
            var nodes = TravianOffcialCore.Parsers.VillageCurrentlyBuilding.GetNodes(_travianDoc);
            var result = TravianOffcialCore.Parsers.VillageCurrentlyBuilding.GetDuration(nodes[0]);
            Assert.AreEqual("", result);
        }

        [TestMethod]
        public void TravianOfficialHeroGetDuration()
        {
            var nodes = TravianOfficalNewHeroUICore.Parsers.VillageCurrentlyBuilding.GetNodes(_travianHeroDoc);
            var result = TravianOffcialCore.Parsers.VillageCurrentlyBuilding.GetDuration(nodes[0]);
            Assert.AreEqual(TimeSpan.Parse("00:43:20"), result);
        }
    }
}