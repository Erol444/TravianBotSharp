using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestProject.Parsers.SubTab
{
    [TestClass]
    public class SubTab
    {
        private readonly HtmlDocument _ttwarsDoc = new();
        private readonly HtmlDocument _travianDoc = new();
        private readonly HtmlDocument _travianHeroDoc = new();

        [TestInitialize]
        public void InitializeTests()
        {
            _ttwarsDoc.Load("Parsers/SubTab/TTWars.html");
            _travianDoc.Load("Parsers/SubTab/Travian.html");
            _travianHeroDoc.Load("Parsers/SubTab/TravianHeroUI.html");
        }

        [TestMethod]
        public void TTWarsGetProductionNum()
        {
            var value = TTWarsCore.Parsers.SubTab.GetProductionNum(_ttwarsDoc);
            Assert.AreEqual(4, value.Count);
        }

        [TestMethod]
        public void TravianOfficialGetProductionNum()
        {
            var value = TravianOfficialCore.Parsers.SubTab.GetProductionNum(_travianDoc);
            Assert.AreEqual(4, value.Count);
        }

        [TestMethod]
        public void TravianOfficialHeroGetProductionNum()
        {
            var value = TravianOfficialCore.Parsers.SubTab.GetProductionNum(_travianHeroDoc);
            Assert.AreEqual(4, value.Count);
        }

        [TestMethod]
        public void TTWarsGetNum()
        {
            var value = TTWarsCore.Parsers.SubTab.GetProductionNum(_ttwarsDoc);
            var result = TTWarsCore.Parsers.SubTab.GetNum(value[2]);
            Assert.AreEqual(91520000, result);
        }

        [TestMethod]
        public void TravianOfficialGetNum()
        {
            var value = TravianOfficialCore.Parsers.SubTab.GetProductionNum(_travianDoc);
            var result = TravianOfficialCore.Parsers.SubTab.GetNum(value[2]);
            Assert.AreEqual(1050, result);
        }

        [TestMethod]
        public void TravianOfficialHeroGetNum()
        {
            var value = TravianOfficialNewHeroUICore.Parsers.SubTab.GetProductionNum(_travianHeroDoc);
            var result = TravianOfficialNewHeroUICore.Parsers.SubTab.GetNum(value[2]);
            Assert.AreEqual(48, result);
        }
    }
}