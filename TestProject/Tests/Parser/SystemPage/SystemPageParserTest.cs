using HtmlAgilityPack;
using MainCore.Enums;
using MainCore.Parsers.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TestProject.Tests.Parser.SystemPage
{
    [TestClass]
    public class SystemPageParserTest
    {
        private static readonly List<ISystemPageParser> _instance = new(){
            new MainCore.Parsers.Implementations.TravianOfficial.SystemPageParser(),
            new MainCore.Parsers.Implementations.TTWars.SystemPageParser()
        };

        private static List<string> _version;

        private readonly HtmlDocument _doc = new();

        private readonly string _path = Path.Combine("Parser", "SystemPage", "HtmlFiles");

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _version = Enum.GetNames(typeof(VersionEnums)).ToList();

            Assert.AreEqual(_instance.Count, _version.Count);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial)]
        [DataRow(VersionEnums.TTWars)]
        public void GetUsernameNodeTest(VersionEnums version)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}_login.html");
            _doc.Load(file);

            var node = _instance[index].GetUsernameNode(_doc);
            Assert.IsNotNull(node);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial)]
        [DataRow(VersionEnums.TTWars)]
        public void GetPasswordNodeTest(VersionEnums version)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}_login.html");
            _doc.Load(file);

            var node = _instance[index].GetPasswordNode(_doc);
            Assert.IsNotNull(node);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial)]
        [DataRow(VersionEnums.TTWars)]
        public void GetLoginButtonTest(VersionEnums version)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}_login.html");
            _doc.Load(file);

            var node = _instance[index].GetLoginButton(_doc);
            Assert.IsNotNull(node);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial)]
        [DataRow(VersionEnums.TTWars)]
        public void GetContractNodeTest(VersionEnums version)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}_upgrade.html");
            _doc.Load(file);

            var node = _instance[index].GetContractNode(_doc);
            Assert.IsNotNull(node);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial)]
        [DataRow(VersionEnums.TTWars)]
        public void GetAdventuresDetailTest(VersionEnums version)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}_adventure.html");
            _doc.Load(file);

            var node = _instance[index].GetAdventuresDetail(_doc);
            Assert.IsNotNull(node);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial)]
        [DataRow(VersionEnums.TTWars)]
        public void GetNpcSumNodeTest(VersionEnums version)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}_market.html");
            _doc.Load(file);

            var node = _instance[index].GetNpcSumNode(_doc);
            Assert.IsNotNull(node);
        }
    }
}