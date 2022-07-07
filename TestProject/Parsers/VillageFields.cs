using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestProject.Parsers
{
    [TestClass]
    public class VillageFields
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
        public void GetResourceNodes()
        {
            var result = TTWarsCore.Parsers.VillageFields.GetResourceNodes(_ttwarsDoc);
            Assert.AreEqual(18, result.Count);
            result = TravianOffcialCore.Parsers.VillageFields.GetResourceNodes(_travianDoc);
            Assert.AreEqual(18, result.Count);
            result = TravianOfficalNewHeroUICore.Parsers.VillageFields.GetResourceNodes(_travianHeroDoc);
            Assert.AreEqual(18, result.Count);
        }

        [TestMethod]
        public void GetId()
        {
            var nodes = TTWarsCore.Parsers.VillageFields.GetResourceNodes(_ttwarsDoc);
            var result = TTWarsCore.Parsers.VillageFields.GetId(nodes[0]);
            Assert.AreEqual(1, result);
            nodes = TravianOffcialCore.Parsers.VillageFields.GetResourceNodes(_travianDoc);
            result = TravianOffcialCore.Parsers.VillageFields.GetId(nodes[1]);
            Assert.AreEqual(2, result);
            nodes = TravianOfficalNewHeroUICore.Parsers.VillageFields.GetResourceNodes(_travianHeroDoc);
            result = TravianOfficalNewHeroUICore.Parsers.VillageFields.GetId(nodes[2]);
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void GetType_()
        {
            var nodes = TTWarsCore.Parsers.VillageFields.GetResourceNodes(_ttwarsDoc);
            var result = TTWarsCore.Parsers.VillageFields.GetType(nodes[0]);
            Assert.AreEqual(1, result);
            nodes = TravianOffcialCore.Parsers.VillageFields.GetResourceNodes(_travianDoc);
            result = TravianOffcialCore.Parsers.VillageFields.GetType(nodes[1]);
            Assert.AreEqual(4, result);
            nodes = TravianOfficalNewHeroUICore.Parsers.VillageFields.GetResourceNodes(_travianHeroDoc);
            result = TravianOfficalNewHeroUICore.Parsers.VillageFields.GetType(nodes[2]);
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void GetLevel()
        {
            var nodes = TTWarsCore.Parsers.VillageFields.GetResourceNodes(_ttwarsDoc);
            var result = TTWarsCore.Parsers.VillageFields.GetLevel(nodes[0]);
            Assert.AreEqual(10, result);
            nodes = TravianOffcialCore.Parsers.VillageFields.GetResourceNodes(_travianDoc);
            result = TravianOffcialCore.Parsers.VillageFields.GetLevel(nodes[1]);
            Assert.AreEqual(10, result);
            nodes = TravianOfficalNewHeroUICore.Parsers.VillageFields.GetResourceNodes(_travianHeroDoc);
            result = TravianOfficalNewHeroUICore.Parsers.VillageFields.GetLevel(nodes[2]);
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void IsUnderConstruction()
        {
            var nodes = TTWarsCore.Parsers.VillageFields.GetResourceNodes(_ttwarsDoc);
            var result = TTWarsCore.Parsers.VillageFields.IsUnderConstruction(nodes[10]);
            Assert.IsTrue(result);
            nodes = TravianOffcialCore.Parsers.VillageFields.GetResourceNodes(_travianDoc);
            result = TravianOffcialCore.Parsers.VillageFields.IsUnderConstruction(nodes[10]);
            Assert.IsTrue(result);
            nodes = TravianOfficalNewHeroUICore.Parsers.VillageFields.GetResourceNodes(_travianHeroDoc);
            result = TravianOfficalNewHeroUICore.Parsers.VillageFields.IsUnderConstruction(nodes[0]);
            Assert.IsTrue(result);
        }
    }
}