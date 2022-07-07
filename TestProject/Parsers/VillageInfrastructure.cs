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
            _ttwarsDoc.Load("TestFile/Dorf2/TTWars.html");
            _travianDoc.Load("TestFile/Dorf2/Travian.html");
            _travianHeroDoc.Load("TestFile/Dorf2/TravianHeroUI.html");
        }

        [TestMethod]
        public void GetBuildingNodes()
        {
            var result = TTWarsCore.Parsers.VillageInfrastructure.GetBuildingNodes(_ttwarsDoc);
            Assert.AreEqual(22, result.Count);
            result = TravianOffcialCore.Parsers.VillageInfrastructure.GetBuildingNodes(_travianDoc);
            Assert.AreEqual(22, result.Count);
            result = TravianOfficalNewHeroUICore.Parsers.VillageInfrastructure.GetBuildingNodes(_travianHeroDoc);
            Assert.AreEqual(22, result.Count);
        }

        [TestMethod]
        public void GetId()
        {
            var nodes = TTWarsCore.Parsers.VillageInfrastructure.GetBuildingNodes(_ttwarsDoc);
            var result = TTWarsCore.Parsers.VillageInfrastructure.GetId(nodes[0]);
            Assert.AreEqual(19, result);
            nodes = TravianOffcialCore.Parsers.VillageInfrastructure.GetBuildingNodes(_travianDoc);
            result = TravianOffcialCore.Parsers.VillageInfrastructure.GetId(nodes[1]);
            Assert.AreEqual(20, result);
            nodes = TravianOfficalNewHeroUICore.Parsers.VillageInfrastructure.GetBuildingNodes(_travianHeroDoc);
            result = TravianOfficalNewHeroUICore.Parsers.VillageInfrastructure.GetId(nodes[2]);
            Assert.AreEqual(21, result);
        }

        [TestMethod]
        public void GetType_()
        {
            var nodes = TTWarsCore.Parsers.VillageInfrastructure.GetBuildingNodes(_ttwarsDoc);
            var result = TTWarsCore.Parsers.VillageInfrastructure.GetType(nodes[7]);
            Assert.AreEqual(15, result);
            nodes = TravianOffcialCore.Parsers.VillageInfrastructure.GetBuildingNodes(_travianDoc);
            result = TravianOffcialCore.Parsers.VillageInfrastructure.GetType(nodes[1]);
            Assert.AreEqual(10, result);
            nodes = TravianOfficalNewHeroUICore.Parsers.VillageInfrastructure.GetBuildingNodes(_travianHeroDoc);
            result = TravianOfficalNewHeroUICore.Parsers.VillageInfrastructure.GetType(nodes[7]);
            Assert.AreEqual(15, result);
        }

        [TestMethod]
        public void GetLevel()
        {
            var nodes = TTWarsCore.Parsers.VillageInfrastructure.GetBuildingNodes(_ttwarsDoc);
            var result = TTWarsCore.Parsers.VillageInfrastructure.GetLevel(nodes[7]);
            Assert.AreEqual(1, result);
            nodes = TravianOffcialCore.Parsers.VillageInfrastructure.GetBuildingNodes(_travianDoc);
            result = TravianOffcialCore.Parsers.VillageInfrastructure.GetLevel(nodes[1]);
            Assert.AreEqual(20, result);
            nodes = TravianOfficalNewHeroUICore.Parsers.VillageInfrastructure.GetBuildingNodes(_travianHeroDoc);
            result = TravianOfficalNewHeroUICore.Parsers.VillageInfrastructure.GetLevel(nodes[7]);
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void IsUnderConstruction()
        {
            var nodes = TTWarsCore.Parsers.VillageInfrastructure.GetBuildingNodes(_ttwarsDoc);
            var result = TTWarsCore.Parsers.VillageInfrastructure.IsUnderConstruction(nodes[20]);
            Assert.IsTrue(result);
            nodes = TravianOffcialCore.Parsers.VillageInfrastructure.GetBuildingNodes(_travianDoc);
            result = TravianOffcialCore.Parsers.VillageInfrastructure.IsUnderConstruction(nodes[12]);
            Assert.IsTrue(result);
            nodes = TravianOfficalNewHeroUICore.Parsers.VillageInfrastructure.GetBuildingNodes(_travianHeroDoc);
            result = TravianOfficalNewHeroUICore.Parsers.VillageInfrastructure.IsUnderConstruction(nodes[7]);
            Assert.IsTrue(result);
        }
    }
}