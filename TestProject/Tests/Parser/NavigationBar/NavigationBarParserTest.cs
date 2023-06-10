using HtmlAgilityPack;
using MainCore.Enums;
using MainCore.Parsers.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TestProject.Tests.Parser.NavigationBar
{
    [TestClass]
    public class NavigationBarParserTest
    {
        private static readonly List<INavigationBarParser> _instance = new(){
            new MainCore.Parsers.Implementations.TravianOfficial.NavigationBarParser(),
            new MainCore.Parsers.Implementations.TTWars.NavigationBarParser()
        };

        private static List<string> _version;

        private readonly HtmlDocument _doc = new();

        private readonly string _path = Path.Combine("Parser", "NavigationBar", "HtmlFiles");

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _version = Enum.GetNames(typeof(VersionEnums)).ToList();

            Assert.AreEqual(_instance.Count, _version.Count);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial)]
        [DataRow(VersionEnums.TTWars)]
        public void GetResourceButtonTest(VersionEnums version)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}.html");
            _doc.Load(file);

            var node = _instance[index].GetResourceButton(_doc);
            Assert.IsNotNull(node);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial)]
        [DataRow(VersionEnums.TTWars)]
        public void GetBuildingButtonTest(VersionEnums version)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}.html");
            _doc.Load(file);

            var node = _instance[index].GetBuildingButton(_doc);
            Assert.IsNotNull(node);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial)]
        [DataRow(VersionEnums.TTWars)]
        public void GetMapButtonTest(VersionEnums version)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}.html");
            _doc.Load(file);

            var node = _instance[index].GetMapButton(_doc);
            Assert.IsNotNull(node);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial)]
        [DataRow(VersionEnums.TTWars)]
        public void GetStatisticsButtonTest(VersionEnums version)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}.html");
            _doc.Load(file);

            var node = _instance[index].GetStatisticsButton(_doc);
            Assert.IsNotNull(node);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial)]
        [DataRow(VersionEnums.TTWars)]
        public void GetReportsButtonTest(VersionEnums version)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}.html");
            _doc.Load(file);

            var node = _instance[index].GetReportsButton(_doc);
            Assert.IsNotNull(node);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial)]
        [DataRow(VersionEnums.TTWars)]
        public void GetMessageButtonTest(VersionEnums version)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}.html");
            _doc.Load(file);

            var node = _instance[index].GetMessageButton(_doc);
            Assert.IsNotNull(node);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial)]
        [DataRow(VersionEnums.TTWars)]
        public void GetDailyButtonTest(VersionEnums version)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}.html");
            _doc.Load(file);

            var node = _instance[index].GetDailyButton(_doc);
            Assert.IsNotNull(node);
        }
    }
}