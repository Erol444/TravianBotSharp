using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestProject.Parsers
{
    [TestClass]
    public class StockBar
    {
        private readonly HtmlDocument _ttwarsDoc = new();
        private readonly HtmlDocument _travianDoc = new();
        private readonly HtmlDocument _travianHeroDoc = new();

        [TestInitialize]
        public void InitializeTests()
        {
            _ttwarsDoc.Load("Parsers/StockBar/TTWars.html");
            _travianDoc.Load("Parsers/StockBar/Travian.html");
            _travianHeroDoc.Load("Parsers/StockBar/TravianHeroUI.html");
        }

        [TestMethod]
        public void TTWarsGetWood()
        {
            var value = TTWarsCore.Parsers.StockBar.GetWood(_ttwarsDoc);
            Assert.AreEqual(9200000, value);
        }

        [TestMethod]
        public void TravianOfficialGetWood()
        {
            var value = TravianOfficialCore.Parsers.StockBar.GetWood(_travianDoc);
            Assert.AreEqual(59635, value);
        }

        [TestMethod]
        public void TravianOfficialHeroGetWood()
        {
            var value = TravianOfficialNewHeroUICore.Parsers.StockBar.GetWood(_travianHeroDoc);
            Assert.AreEqual(661, value);
        }

        [TestMethod]
        public void TTWarsGetClay()
        {
            var value = TTWarsCore.Parsers.StockBar.GetClay(_ttwarsDoc);
            Assert.AreEqual(9200000, value);
        }

        [TestMethod]
        public void TravianOfficialGetClay()
        {
            var value = TravianOfficialCore.Parsers.StockBar.GetClay(_travianDoc);
            Assert.AreEqual(22097, value);
        }

        [TestMethod]
        public void TravianOfficialHeroGetClay()
        {
            var value = TravianOfficialNewHeroUICore.Parsers.StockBar.GetClay(_travianHeroDoc);
            Assert.AreEqual(701, value);
        }

        [TestMethod]
        public void TTWarsGetIron()
        {
            var value = TTWarsCore.Parsers.StockBar.GetIron(_ttwarsDoc);
            Assert.AreEqual(9200000, value);
        }

        [TestMethod]
        public void TravianOfficialGetIron()
        {
            var value = TravianOfficialCore.Parsers.StockBar.GetIron(_travianDoc);
            Assert.AreEqual(30581, value);
        }

        [TestMethod]
        public void TravianOfficialHeroGetIron()
        {
            var value = TravianOfficialNewHeroUICore.Parsers.StockBar.GetIron(_travianHeroDoc);
            Assert.AreEqual(675, value);
        }

        [TestMethod]
        public void TTWarsGetCrop()
        {
            var value = TTWarsCore.Parsers.StockBar.GetCrop(_ttwarsDoc);
            Assert.AreEqual(9200000, value);
        }

        [TestMethod]
        public void TravianOfficialGetCrop()
        {
            var value = TravianOfficialCore.Parsers.StockBar.GetCrop(_travianDoc);
            Assert.AreEqual(20599, value);
        }

        [TestMethod]
        public void TravianOfficialHeroGetCrop()
        {
            var value = TravianOfficialNewHeroUICore.Parsers.StockBar.GetCrop(_travianHeroDoc);
            Assert.AreEqual(726, value);
        }

        [TestMethod]
        public void TTWarsGetFreeCrop()
        {
            var value = TTWarsCore.Parsers.StockBar.GetFreeCrop(_ttwarsDoc);
            Assert.AreEqual(136319762, value);
        }

        [TestMethod]
        public void TravianOfficialGetFreetCrop()
        {
            var value = TravianOfficialCore.Parsers.StockBar.GetFreeCrop(_travianDoc);
            Assert.AreEqual(2278, value);
        }

        [TestMethod]
        public void TravianOfficialHeroGetFreeCrop()
        {
            var value = TravianOfficialNewHeroUICore.Parsers.StockBar.GetFreeCrop(_travianHeroDoc);
            Assert.AreEqual(19, value);
        }

        [TestMethod]
        public void TTWarsGetWarehouseCapacity()
        {
            var value = TTWarsCore.Parsers.StockBar.GetWarehouseCapacity(_ttwarsDoc);
            Assert.AreEqual(9200000, value);
        }

        [TestMethod]
        public void TravianOfficialGetWarehouseCapacity()
        {
            var value = TravianOfficialCore.Parsers.StockBar.GetWarehouseCapacity(_travianDoc);
            Assert.AreEqual(105900, value);
        }

        [TestMethod]
        public void TravianOfficialHeroGetWarehouseCapacity()
        {
            var value = TravianOfficialNewHeroUICore.Parsers.StockBar.GetWarehouseCapacity(_travianHeroDoc);
            Assert.AreEqual(800, value);
        }

        [TestMethod]
        public void TTWarsGetGranaryCapacity()
        {
            var value = TTWarsCore.Parsers.StockBar.GetGranaryCapacity(_ttwarsDoc);
            Assert.AreEqual(9200000, value);
        }

        [TestMethod]
        public void TravianOfficialGetGranaryCapacity()
        {
            var value = TravianOfficialCore.Parsers.StockBar.GetGranaryCapacity(_travianDoc);
            Assert.AreEqual(87800, value);
        }

        [TestMethod]
        public void TravianOfficialHeroGetGranaryCapacity()
        {
            var value = TravianOfficialNewHeroUICore.Parsers.StockBar.GetGranaryCapacity(_travianHeroDoc);
            Assert.AreEqual(800, value);
        }

        [TestMethod]
        public void TTWarsGetGold()
        {
            var value = TTWarsCore.Parsers.StockBar.GetGold(_ttwarsDoc);
            Assert.AreEqual(250, value);
        }

        [TestMethod]
        public void TravianOfficialGetGold()
        {
            var value = TravianOfficialCore.Parsers.StockBar.GetGold(_travianDoc);
            Assert.AreEqual(32, value);
        }

        [TestMethod]
        public void TravianOfficialHeroGetGold()
        {
            var value = TravianOfficialNewHeroUICore.Parsers.StockBar.GetGold(_travianHeroDoc);
            Assert.AreEqual(130, value);
        }

        [TestMethod]
        public void TTWarsGetSilver()
        {
            var value = TTWarsCore.Parsers.StockBar.GetSilver(_ttwarsDoc);
            Assert.AreEqual(0, value);
        }

        [TestMethod]
        public void TravianOfficialGetSilver()
        {
            var value = TravianOfficialCore.Parsers.StockBar.GetSilver(_travianDoc);
            Assert.AreEqual(2409, value);
        }

        [TestMethod]
        public void TravianOfficialHeroGetSilver()
        {
            var value = TravianOfficialNewHeroUICore.Parsers.StockBar.GetSilver(_travianHeroDoc);
            Assert.AreEqual(0, value);
        }
    }
}