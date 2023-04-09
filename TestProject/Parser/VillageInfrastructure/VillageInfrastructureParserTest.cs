using HtmlAgilityPack;
using MainCore.Enums;
using MainCore.Parsers.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TestProject.Parser.VillageInfrastructure
{
    [TestClass]
    public class VillageInfrastructureParserTest
    {
        private static readonly List<IVillageInfrastructureParser> _instance = new(){
            new MainCore.Parsers.Implementations.TravianOfficial.VillageInfrastructureParser(),
            new MainCore.Parsers.Implementations.TTWars.VillageInfrastructureParser()
        };

        private static List<string> _version;

        private readonly HtmlDocument _doc = new();

        private readonly string _path = Path.Combine("Parser", "VillageInfrastructure", "HtmlFiles");

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _version = Enum.GetNames(typeof(VersionEnums)).ToList();

            Assert.AreEqual(_instance.Count, _version.Count);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial, 22)]
        [DataRow(VersionEnums.TTWars, 22)]
        public void GetBuildingTabNodesTest(VersionEnums version, int expected)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}.html");
            _doc.Load(file);

            var nodes = _instance[index].GetNodes(_doc);
            Assert.AreEqual(expected, nodes.Count);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial, 26, 25)]
        [DataRow(VersionEnums.TTWars, 26, 25)]
        public void GetIdTest(VersionEnums version, int location, int expected)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}.html");
            _doc.Load(file);

            var nodes = _instance[index].GetNodes(_doc);
            var actual = _instance[index].GetId(nodes[location - 20]);
            Assert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial, 27, 15)]
        [DataRow(VersionEnums.TTWars, 27, 15)]
        public void GetBuildingTypeTest(VersionEnums version, int location, int expected)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}.html");
            _doc.Load(file);

            var nodes = _instance[index].GetNodes(_doc);
            var actual = _instance[index].GetBuildingType(nodes[location - 20]);
            Assert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial, 27, 20)]
        [DataRow(VersionEnums.TTWars, 27, 12)]
        public void GetLevelTest(VersionEnums version, int location, int expected)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}.html");
            _doc.Load(file);

            var nodes = _instance[index].GetNodes(_doc);
            var actual = _instance[index].GetLevel(nodes[location - 20]);
            Assert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial, 27, false)]
        [DataRow(VersionEnums.TTWars, 27, false)]
        public void IsUnderConstructionTest(VersionEnums version, int location, bool expected)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}.html");
            _doc.Load(file);

            var nodes = _instance[index].GetNodes(_doc);
            var actual = _instance[index].IsUnderConstruction(nodes[location - 20]);
            Assert.AreEqual(expected, actual);
        }
    }
}