using HtmlAgilityPack;
using MainCore.Enums;
using MainCore.Parser.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TestProject.Parser.VillageField
{
    [TestClass]
    public class VillageFieldParserTest
    {
        private static readonly List<IVillageFieldParser> _instance = new(){
            new MainCore.Parser.Implementations.TravianOfficial.VillageFieldParser(),
            new MainCore.Parser.Implementations.TTWars.VillageFieldParser()
        };

        private static List<string> _version;

        private readonly HtmlDocument _doc = new();

        private readonly string _path = Path.Combine("Parser", "VillageField", "HtmlFiles");

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _version = Enum.GetNames(typeof(VersionEnums)).ToList();

            Assert.AreEqual(_instance.Count, _version.Count);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial, 18)]
        [DataRow(VersionEnums.TTWars, 18)]
        public void GetBuildingTabNodesTest(VersionEnums version, int expected)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}.html");
            _doc.Load(file);

            var nodes = _instance[index].GetNodes(_doc);
            Assert.AreEqual(expected, nodes.Count);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial, 0, 1)]
        [DataRow(VersionEnums.TTWars, 0, 1)]
        public void GetIdTest(VersionEnums version, int location, int expected)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}.html");
            _doc.Load(file);

            var nodes = _instance[index].GetNodes(_doc);
            var actual = _instance[index].GetId(nodes[location]);
            Assert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial, 0, 1)]
        [DataRow(VersionEnums.TTWars, 0, 1)]
        public void GetBuildingTypeTest(VersionEnums version, int location, int expected)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}.html");
            _doc.Load(file);

            var nodes = _instance[index].GetNodes(_doc);
            var actual = _instance[index].GetBuildingType(nodes[location]);
            Assert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial, 0, 10)]
        [DataRow(VersionEnums.TTWars, 0, 10)]
        public void GetLevelTest(VersionEnums version, int location, int expected)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}.html");
            _doc.Load(file);

            var nodes = _instance[index].GetNodes(_doc);
            var actual = _instance[index].GetLevel(nodes[location]);
            Assert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial, 0, false)]
        [DataRow(VersionEnums.TTWars, 0, false)]
        public void IsUnderConstructionTest(VersionEnums version, int location, bool expected)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}.html");
            _doc.Load(file);

            var nodes = _instance[index].GetNodes(_doc);
            var actual = _instance[index].IsUnderConstruction(nodes[location]);
            Assert.AreEqual(expected, actual);
        }
    }
}