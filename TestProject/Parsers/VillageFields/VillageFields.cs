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
            _ttwarsDoc.Load("Parsers/VillageFields/TTWars.html");
            _travianDoc.Load("Parsers/VillageFields/Travian.html");
            _travianHeroDoc.Load("Parsers/VillageFields/TravianHeroUI.html");
        }

        [TestMethod]
        public void TTWarsGetResourceNodes()
        {
            var value = TTWarsCore.Parsers.VillageFields.GetResourceNodes(_ttwarsDoc);
            Assert.AreEqual(18, value.Count);
        }

        [TestMethod]
        public void TravianOfficialGetResourceNodes()
        {
            var value = TravianOfficialCore.Parsers.VillageFields.GetResourceNodes(_travianDoc);
            Assert.AreEqual(18, value.Count);
        }

        [TestMethod]
        public void TravianOfficialHeroGetResourceNodes()
        {
            var value = TravianOfficialCore.Parsers.VillageFieldParser.GetResourceNodes(_travianHeroDoc);
            Assert.AreEqual(18, value.Count);
        }

        [TestMethod]
        public void TTWarsGetId()
        {
            var nodes = TTWarsCore.Parsers.VillageFields.GetResourceNodes(_ttwarsDoc);
            var result = TTWarsCore.Parsers.VillageFields.GetId(nodes[0]);
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void TravianOfficialGetId()
        {
            var nodes = TravianOfficialCore.Parsers.VillageFields.GetResourceNodes(_travianDoc);
            var result = TravianOfficialCore.Parsers.VillageFields.GetId(nodes[1]);
            Assert.AreEqual(2, result);
        }

        [TestMethod]
        public void TravianOfficialHeroGetId()
        {
            var nodes = TravianOfficialCore.Parsers.VillageFieldParser.GetResourceNodes(_travianHeroDoc);
            var result = TravianOfficialCore.Parsers.VillageFieldParser.GetId(nodes[2]);
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void TTWarsGetType()
        {
            var nodes = TTWarsCore.Parsers.VillageFields.GetResourceNodes(_ttwarsDoc);
            var result = TTWarsCore.Parsers.VillageFields.GetType(nodes[0]);
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void TravianOfficialGetType()
        {
            var nodes = TravianOfficialCore.Parsers.VillageFields.GetResourceNodes(_travianDoc);
            var result = TravianOfficialCore.Parsers.VillageFields.GetType(nodes[1]);
            Assert.AreEqual(4, result);
        }

        [TestMethod]
        public void TravianOfficialHeroGetType()
        {
            var nodes = TravianOfficialCore.Parsers.VillageFieldParser.GetResourceNodes(_travianHeroDoc);
            var result = TravianOfficialCore.Parsers.VillageFieldParser.GetType(nodes[2]);
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void TTWarsGetLevel()
        {
            var nodes = TTWarsCore.Parsers.VillageFields.GetResourceNodes(_ttwarsDoc);
            var result = TTWarsCore.Parsers.VillageFields.GetLevel(nodes[0]);
            Assert.AreEqual(10, result);
        }

        [TestMethod]
        public void TravianOfficialGetLevel()
        {
            var nodes = TravianOfficialCore.Parsers.VillageFields.GetResourceNodes(_travianDoc);
            var result = TravianOfficialCore.Parsers.VillageFields.GetLevel(nodes[1]);
            Assert.AreEqual(10, result);
        }

        [TestMethod]
        public void TravianOfficialHeroGetLevel()
        {
            var nodes = TravianOfficialCore.Parsers.VillageFieldParser.GetResourceNodes(_travianHeroDoc);
            var result = TravianOfficialCore.Parsers.VillageFieldParser.GetLevel(nodes[2]);
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void TTWarsIsUnderConstruction()
        {
            var nodes = TTWarsCore.Parsers.VillageFields.GetResourceNodes(_ttwarsDoc);
            var result = TTWarsCore.Parsers.VillageFields.IsUnderConstruction(nodes[0]);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TravianOfficialIsUnderConstruction()
        {
            var nodes = TravianOfficialCore.Parsers.VillageFields.GetResourceNodes(_travianDoc);
            var result = TravianOfficialCore.Parsers.VillageFields.IsUnderConstruction(nodes[1]);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TravianOfficialHeroIsUnderConstruction()
        {
            var nodes = TravianOfficialCore.Parsers.VillageFieldParser.GetResourceNodes(_travianHeroDoc);
            var result = TravianOfficialCore.Parsers.VillageFieldParser.IsUnderConstruction(nodes[2]);
            Assert.IsFalse(result);
        }
    }
}