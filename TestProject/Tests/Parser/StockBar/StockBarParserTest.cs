using HtmlAgilityPack;
using MainCore.Enums;
using MainCore.Parsers.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TestProject.Tests.Parser.StockBar
{
    [TestClass]
    public class StockBarParserTest
    {
        private static readonly List<IStockBarParser> _instance = new(){
            new MainCore.Parsers.Implementations.TravianOfficial.StockBarParser(),
            new MainCore.Parsers.Implementations.TTWars.StockBarParser()
        };

        private static List<string> _version;

        private readonly HtmlDocument _doc = new();

        private readonly string _path = Path.Combine("Parser", "StockBar", "HtmlFiles");

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _version = Enum.GetNames(typeof(VersionEnums)).ToList();

            Assert.AreEqual(_instance.Count, _version.Count);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial, 173_604)]
        [DataRow(VersionEnums.TTWars, 6_000_000)]
        public void GetWoodTest(VersionEnums version, int expected)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}.html");
            _doc.Load(file);

            var actual = _instance[index].GetWood(_doc);
            Assert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial, 129)]
        [DataRow(VersionEnums.TTWars, 6_000_000)]
        public void GetClayTest(VersionEnums version, int expected)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}.html");
            _doc.Load(file);

            var actual = _instance[index].GetClay(_doc);
            Assert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial, 255_036)]
        [DataRow(VersionEnums.TTWars, 6_000_000)]
        public void GetIronTest(VersionEnums version, int expected)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}.html");
            _doc.Load(file);

            var actual = _instance[index].GetIron(_doc);
            Assert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial, 640_000)]
        [DataRow(VersionEnums.TTWars, 6_000_000)]
        public void GetCropTest(VersionEnums version, int expected)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}.html");
            _doc.Load(file);

            var actual = _instance[index].GetCrop(_doc);
            Assert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial, 74_061)]
        [DataRow(VersionEnums.TTWars, 20_319_811)]
        public void GetFreeCropTest(VersionEnums version, int expected)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}.html");
            _doc.Load(file);

            var actual = _instance[index].GetFreeCrop(_doc);
            Assert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial, 320_000)]
        [DataRow(VersionEnums.TTWars, 6_000_000)]
        public void GetWarehouseCapacityTest(VersionEnums version, int expected)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}.html");
            _doc.Load(file);

            var actual = _instance[index].GetWarehouseCapacity(_doc);
            Assert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial, 640_000)]
        [DataRow(VersionEnums.TTWars, 6_000_000)]
        public void GetGranaryCapacityTest(VersionEnums version, int expected)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}.html");
            _doc.Load(file);

            var actual = _instance[index].GetGranaryCapacity(_doc);
            Assert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial, 37)]
        [DataRow(VersionEnums.TTWars, 210)]
        public void GetGoldTest(VersionEnums version, int expected)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}.html");
            _doc.Load(file);

            var actual = _instance[index].GetGold(_doc);
            Assert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial, 562)]
        [DataRow(VersionEnums.TTWars, 0)]
        public void GetSilverTest(VersionEnums version, int expected)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}.html");
            _doc.Load(file);

            var actual = _instance[index].GetSilver(_doc);
            Assert.AreEqual(expected, actual);
        }
    }
}