using HtmlAgilityPack;
using MainCore.Enums;
using MainCore.Parsers.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TestProject.Tests.Parser.BuildingTab
{
    [TestClass]
    public class BuildingTabParserTest
    {
        private static readonly List<IBuildingTabParser> _instance = new(){
            new MainCore.Parsers.Implementations.TravianOfficial.BuildingTabParser(),
            new MainCore.Parsers.Implementations.TTWars.BuildingTabParser()
        };

        private static List<string> _version;

        private readonly HtmlDocument _doc = new();

        private readonly string _path = Path.Combine("Tests", "Parser", "BuildingTab", "HtmlFiles");

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _version = Enum.GetNames(typeof(VersionEnums)).ToList();

            Assert.AreEqual(_instance.Count, _version.Count);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial, 5)]
        [DataRow(VersionEnums.TTWars, 5)]
        public void GetBuildingTabNodesTest(VersionEnums version, int expected)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}.html");
            _doc.Load(file);

            var nodes = _instance[index].GetBuildingTabNodes(_doc);
            Assert.AreEqual(expected, nodes.Count);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial, 1, true)]
        [DataRow(VersionEnums.TravianOfficial, 2, false)]
        [DataRow(VersionEnums.TTWars, 0, true)]
        [DataRow(VersionEnums.TTWars, 1, false)]
        public void IsCurrentTabTest(VersionEnums version, int location, bool expected)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}.html");
            _doc.Load(file);

            var nodes = _instance[index].GetBuildingTabNodes(_doc);
            var actual = _instance[index].IsCurrentTab(nodes[location]);
            Assert.AreEqual(expected, actual);
        }
    }
}