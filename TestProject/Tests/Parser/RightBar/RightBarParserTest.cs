using HtmlAgilityPack;
using MainCore.Enums;
using MainCore.Parsers.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TestProject.Tests.Parser.RightBar
{
    [TestClass]
    public class RightBarParserTest
    {
        private static readonly List<IRightBarParser> _instance = new(){
            new MainCore.Parsers.Implementations.TravianOfficial.RightBarParser(),
            new MainCore.Parsers.Implementations.TTWars.RightBarParser()
        };

        private static List<string> _version;

        private readonly HtmlDocument _doc = new();

        private readonly string _path = Path.Combine("Parser", "RightBar", "HtmlFiles");

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _version = Enum.GetNames(typeof(VersionEnums)).ToList();

            Assert.AreEqual(_instance.Count, _version.Count);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial, true)]
        [DataRow(VersionEnums.TTWars, false)]
        public void HasPlusAccountTest(VersionEnums version, bool expected)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}.html");
            _doc.Load(file);

            var actual = _instance[index].HasPlusAccount(_doc);
            Assert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial, 3)]
        [DataRow(VersionEnums.TTWars, 0)]
        public void GetTribeTest(VersionEnums version, int expected)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}.html");
            _doc.Load(file);

            var actual = _instance[index].GetTribe(_doc);
            Assert.AreEqual(expected, actual);
        }
    }
}