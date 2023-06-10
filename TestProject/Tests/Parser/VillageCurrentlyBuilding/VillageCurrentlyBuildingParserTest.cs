using HtmlAgilityPack;
using MainCore.Enums;
using MainCore.Parsers.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TestProject.Tests.Parser.VillageCurrentlyBuilding
{
    [TestClass]
    public class VillageCurrentlyBuildingParserTest
    {
        private static readonly List<IVillageCurrentlyBuildingParser> _instance = new(){
            new MainCore.Parsers.Implementations.TravianOfficial.VillageCurrentlyBuildingParser(),
            new MainCore.Parsers.Implementations.TTWars.VillageCurrentlyBuildingParser()
        };

        private static List<string> _version;

        private readonly HtmlDocument _doc = new();

        private readonly string _path = Path.Combine("Parser", "VillageCurrentlyBuilding", "HtmlFiles");

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _version = Enum.GetNames(typeof(VersionEnums)).ToList();

            Assert.AreEqual(_instance.Count, _version.Count);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial, 2)]
        [DataRow(VersionEnums.TTWars, 1)]
        public void GetItemsTest(VersionEnums version, int expected)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}.html");
            _doc.Load(file);

            var nodes = _instance[index].GetItems(_doc);
            Assert.AreEqual(expected, nodes.Count);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial, 0, "TradeOffice")]
        [DataRow(VersionEnums.TravianOfficial, 1, "TradeOffice")]
        [DataRow(VersionEnums.TTWars, 0, "MainBuilding")]
        public void GetBuildingTypeTest(VersionEnums version, int location, string expected)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}.html");
            _doc.Load(file);

            var nodes = _instance[index].GetItems(_doc);
            var actual = _instance[index].GetBuildingType(nodes[location]);
            Assert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial, 0, 13)]
        [DataRow(VersionEnums.TravianOfficial, 1, 14)]
        [DataRow(VersionEnums.TTWars, 0, 8)]
        public void GetLevelTest(VersionEnums version, int location, int expected)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}.html");
            _doc.Load(file);

            var nodes = _instance[index].GetItems(_doc);
            var actual = _instance[index].GetLevel(nodes[location]);
            Assert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial, 0, 13_457)]
        [DataRow(VersionEnums.TravianOfficial, 1, 29_247)]
        [DataRow(VersionEnums.TTWars, 0, 6)]
        public void GetDurationTest(VersionEnums version, int location, int expected)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}.html");
            _doc.Load(file);

            var nodes = _instance[index].GetItems(_doc);
            var actual = _instance[index].GetDuration(nodes[location]);

            Assert.AreEqual(TimeSpan.FromSeconds(expected), actual);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial)]
        [DataRow(VersionEnums.TTWars)]
        public void GetConfirmFinishNowButtonTest(VersionEnums version)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}_dialogue.html");
            _doc.Load(file);

            var node = _instance[index].GetConfirmFinishNowButton(_doc);

            Assert.IsNotNull(node);
        }
    }
}