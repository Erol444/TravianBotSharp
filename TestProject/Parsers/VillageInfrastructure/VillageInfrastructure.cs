using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestProject.Parsers
{
    [TestClass]
    public class VillageInfrastructure
    {
        private readonly HtmlDocument _ttwarsDoc = new();
        private readonly HtmlDocument _travianDoc = new();
        private readonly HtmlDocument _travianHeroDoc = new();

        [TestInitialize]
        public void InitializeTests()
        {
            _ttwarsDoc.Load("Parsers/VillageInfrastructure/TTWars.html");
            _travianDoc.Load("Parsers/VillageInfrastructure/Travian.html");
            _travianHeroDoc.Load("Parsers/VillageInfrastructure/TravianHeroUI.html");
        }

        [TestMethod]
        public void TTWarsGetBuildingNodes()
        {
            var value = TTWarsCore.Parsers.VillageInfrastructure.GetBuildingNodes(_ttwarsDoc);
            Assert.AreEqual(22, value.Count);
        }

        [TestMethod]
        public void TravianOfficialGetBuildingNodes()
        {
            var value = TravianOfficialCore.Parsers.VillageInfrastructure.GetBuildingNodes(_travianDoc);
            Assert.AreEqual(22, value.Count);
        }

        [TestMethod]
        public void TravianOfficialHeroGetBuildingNodes()
        {
            var value = TravianOfficialNewHeroUICore.Parsers.VillageInfrastructure.GetBuildingNodes(_travianHeroDoc);
            Assert.AreEqual(22, value.Count);
        }

        [TestMethod]
        public void TTWarsGetId()
        {
            var nodes = TTWarsCore.Parsers.VillageInfrastructure.GetBuildingNodes(_ttwarsDoc);
            var result = TTWarsCore.Parsers.VillageInfrastructure.GetId(nodes[7]);
            Assert.AreEqual(26, result);
        }

        [TestMethod]
        public void TravianOfficialGetId()
        {
            var nodes = TravianOfficialCore.Parsers.VillageInfrastructure.GetBuildingNodes(_travianDoc);
            var result = TravianOfficialCore.Parsers.VillageInfrastructure.GetId(nodes[1]);
            Assert.AreEqual(20, result);
        }

        [TestMethod]
        public void TravianOfficialHeroGetId()
        {
            var nodes = TravianOfficialNewHeroUICore.Parsers.VillageInfrastructure.GetBuildingNodes(_travianHeroDoc);
            var result = TravianOfficialNewHeroUICore.Parsers.VillageInfrastructure.GetId(nodes[1]);
            Assert.AreEqual(20, result);
        }

        [TestMethod]
        public void TTWarsGetType()
        {
            var nodes = TTWarsCore.Parsers.VillageInfrastructure.GetBuildingNodes(_ttwarsDoc);
            var result = TTWarsCore.Parsers.VillageInfrastructure.GetType(nodes[7]);
            Assert.AreEqual(15, result);
        }

        [TestMethod]
        public void TravianOfficialGetType()
        {
            var nodes = TravianOfficialCore.Parsers.VillageInfrastructure.GetBuildingNodes(_travianDoc);
            var result = TravianOfficialCore.Parsers.VillageInfrastructure.GetType(nodes[1]);
            Assert.AreEqual(8, result);
        }

        [TestMethod]
        public void TravianOfficialHeroGetLevel()
        {
            var nodes = TravianOfficialNewHeroUICore.Parsers.VillageInfrastructure.GetBuildingNodes(_travianHeroDoc);
            var result = TravianOfficialNewHeroUICore.Parsers.VillageInfrastructure.GetLevel(nodes[1]);
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void TTWarsGetLevel()
        {
            var nodes = TTWarsCore.Parsers.VillageInfrastructure.GetBuildingNodes(_ttwarsDoc);
            var result = TTWarsCore.Parsers.VillageInfrastructure.GetLevel(nodes[7]);
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void TravianOfficialGetLevel()
        {
            var nodes = TravianOfficialCore.Parsers.VillageInfrastructure.GetBuildingNodes(_travianDoc);
            var result = TravianOfficialCore.Parsers.VillageInfrastructure.GetType(nodes[1]);
            Assert.AreEqual(8, result);
        }

        [TestMethod]
        public void TravianOfficialHeroGetType()
        {
            var nodes = TravianOfficialNewHeroUICore.Parsers.VillageInfrastructure.GetBuildingNodes(_travianHeroDoc);
            var result = TravianOfficialNewHeroUICore.Parsers.VillageInfrastructure.GetLevel(nodes[1]);
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void TTWarsIsUnderConstruction()
        {
            var nodes = TTWarsCore.Parsers.VillageInfrastructure.GetBuildingNodes(_ttwarsDoc);
            var result = TTWarsCore.Parsers.VillageInfrastructure.IsUnderConstruction(nodes[7]);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TravianOfficialIsUnderConstruction()
        {
            var nodes = TravianOfficialCore.Parsers.VillageInfrastructure.GetBuildingNodes(_travianDoc);
            var result = TravianOfficialCore.Parsers.VillageInfrastructure.IsUnderConstruction(nodes[1]);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TravianOfficialHeroIsUnderConstruction()
        {
            var nodes = TravianOfficialNewHeroUICore.Parsers.VillageInfrastructure.GetBuildingNodes(_travianHeroDoc);
            var result = TravianOfficialNewHeroUICore.Parsers.VillageInfrastructure.IsUnderConstruction(nodes[1]);
            Assert.IsFalse(result);
        }
    }
}