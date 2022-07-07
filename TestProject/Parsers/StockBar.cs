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
    public class StockBar
    {
        private readonly HtmlDocument _ttwarsDoc = new();
        private readonly HtmlDocument _travianDoc = new();

        [TestInitialize]
        public void InitializeTests()
        {
            _ttwarsDoc.Load("TestFile/Dorf1/TTWars.html");
            _travianDoc.Load("TestFile/Dorf1/Travian.html");
        }

        [TestMethod]
        public void GetWood()
        {
            var value = TTWarsCore.Parsers.StockBar.GetWood(_ttwarsDoc);
            Assert.AreEqual(9200000, value);
            value = TravianOffcialCore.Parsers.StockBar.GetWood(_travianDoc);
            Assert.AreEqual(70235, value);
            value = TravianOfficalNewHeroUICore.Parsers.StockBar.GetWood(_travianDoc);
            Assert.AreEqual(70235, value);
        }

        [TestMethod]
        public void GetClay()
        {
            var value = TTWarsCore.Parsers.StockBar.GetClay(_ttwarsDoc);
            Assert.AreEqual(9200000, value);
            value = TravianOffcialCore.Parsers.StockBar.GetClay(_travianDoc);
            Assert.AreEqual(71602, value);
            value = TravianOfficalNewHeroUICore.Parsers.StockBar.GetClay(_travianDoc);
            Assert.AreEqual(71602, value);
        }

        [TestMethod]
        public void GetIron()
        {
            var value = TTWarsCore.Parsers.StockBar.GetIron(_ttwarsDoc);
            Assert.AreEqual(9200000, value);
            value = TravianOffcialCore.Parsers.StockBar.GetIron(_travianDoc);
            Assert.AreEqual(74439, value);
            value = TravianOfficalNewHeroUICore.Parsers.StockBar.GetIron(_travianDoc);
            Assert.AreEqual(74439, value);
        }

        [TestMethod]
        public void GetCrop()
        {
            var value = TTWarsCore.Parsers.StockBar.GetCrop(_ttwarsDoc);
            Assert.AreEqual(9200000, value);
            value = TravianOffcialCore.Parsers.StockBar.GetCrop(_travianDoc);
            Assert.AreEqual(75464, value);
            value = TravianOfficalNewHeroUICore.Parsers.StockBar.GetCrop(_travianDoc);
            Assert.AreEqual(75464, value);
        }

        [TestMethod]
        public void GetFreeCrop()
        {
            var value = TTWarsCore.Parsers.StockBar.GetFreeCrop(_ttwarsDoc);
            Assert.AreEqual(136319762, value);
            value = TravianOffcialCore.Parsers.StockBar.GetFreeCrop(_travianDoc);
            Assert.AreEqual(3389, value);
            value = TravianOfficalNewHeroUICore.Parsers.StockBar.GetFreeCrop(_travianDoc);
            Assert.AreEqual(3389, value);
        }

        [TestMethod]
        public void GetWarehouseCapacity()
        {
            var value = TTWarsCore.Parsers.StockBar.GetWarehouseCapacity(_ttwarsDoc);
            Assert.AreEqual(9200000, value);
            value = TravianOffcialCore.Parsers.StockBar.GetWarehouseCapacity(_travianDoc);
            Assert.AreEqual(80000, value);
            value = TravianOfficalNewHeroUICore.Parsers.StockBar.GetWarehouseCapacity(_travianDoc);
            Assert.AreEqual(80000, value);
        }

        [TestMethod]
        public void GetGranaryCapacity()
        {
            var value = TTWarsCore.Parsers.StockBar.GetGranaryCapacity(_ttwarsDoc);
            Assert.AreEqual(9200000, value);
            value = TravianOffcialCore.Parsers.StockBar.GetGranaryCapacity(_travianDoc);
            Assert.AreEqual(80000, value);
            value = TravianOfficalNewHeroUICore.Parsers.StockBar.GetGranaryCapacity(_travianDoc);
            Assert.AreEqual(80000, value);
        }
    }
}