using HtmlAgilityPack;
using MainCore.Enums;
using MainCore.Parsers.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TestProject.Tests.Parser.HeroSection
{
    [TestClass]
    public class FarmListParserTest
    {
        private static readonly List<IFarmListParser> _instance = new(){
            new MainCore.Parsers.Implementations.TravianOfficial.FarmListParser(),
            new MainCore.Parsers.Implementations.TTWars.FarmListParser()
        };

        private static List<string> _version;

        private readonly HtmlDocument _doc = new();

        private readonly string _path = Path.Combine("Tests", "Parser", "FarmList", "HtmlFiles");

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _version = Enum.GetNames(typeof(VersionEnums)).ToList();

            Assert.AreEqual(_instance.Count, _version.Count);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial, 8)]
        [DataRow(VersionEnums.TTWars, 5)]
        public void GetFarmNodesTest(VersionEnums version, int expected)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}.html");
            _doc.Load(file);

            var actual = _instance[index].GetFarmNodes(_doc);
            Assert.AreEqual(expected, actual.Count);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial, 0, "Inactive")]
        [DataRow(VersionEnums.TTWars, 0, "1")]
        public void GetNameTest(VersionEnums version, int pos, string expected)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}.html");
            _doc.Load(file);

            var nodes = _instance[index].GetFarmNodes(_doc);
            var actual = _instance[index].GetName(nodes[pos]);
            Assert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial, 0, 1233)]
        [DataRow(VersionEnums.TTWars, 0, 54)]
        public void GetIdTest(VersionEnums version, int pos, int expected)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}.html");
            _doc.Load(file);

            var nodes = _instance[index].GetFarmNodes(_doc);
            var actual = _instance[index].GetId(nodes[pos]);
            Assert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial, 0, 19)]
        [DataRow(VersionEnums.TTWars, 0, 2)]
        public void GetNumOfFarmsTest(VersionEnums version, int pos, int expected)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}.html");
            _doc.Load(file);

            var nodes = _instance[index].GetFarmNodes(_doc);
            var actual = _instance[index].GetNumOfFarms(nodes[pos]);
            Assert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial)]
        [DataRow(VersionEnums.TTWars)]
        public void GetStartAllTest(VersionEnums version)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}.html");
            _doc.Load(file);

            var actual = _instance[index].GetStartAllButton(_doc);
            Assert.IsNotNull(actual);
        }
    }
}