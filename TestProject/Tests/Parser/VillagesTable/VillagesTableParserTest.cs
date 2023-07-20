using HtmlAgilityPack;
using MainCore.Enums;
using MainCore.Parsers.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TestProject.Tests.Parser.VillagesTable
{
    [TestClass]
    public class VillagesTableParserTest
    {
        private static readonly List<IVillagesTableParser> _instance = new(){
            new MainCore.Parsers.Implementations.TravianOfficial.VillagesTableParser(),
            new MainCore.Parsers.Implementations.TTWars.VillagesTableParser()
        };

        private static List<string> _version;

        private readonly HtmlDocument _doc = new();

        private readonly string _path = Path.Combine("Tests", "Parser", "VillagesTable", "HtmlFiles");

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _version = Enum.GetNames(typeof(VersionEnums)).ToList();

            Assert.AreEqual(_instance.Count, _version.Count);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial, 15)]
        [DataRow(VersionEnums.TTWars, 1)]
        public void GetVillagesTest(VersionEnums version, int expected)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}.html");
            _doc.Load(file);

            var nodes = _instance[index].GetVillages(_doc);
            Assert.AreEqual(expected, nodes.Count);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial, 0, false)]
        [DataRow(VersionEnums.TTWars, 0, false)]
        public void IsUnderAttackTest(VersionEnums version, int location, bool expected)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}.html");
            _doc.Load(file);

            var nodes = _instance[index].GetVillages(_doc);
            var actual = _instance[index].IsUnderAttack(nodes[location]);
            Assert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial, 0, false)]
        [DataRow(VersionEnums.TravianOfficial, 4, true)]
        [DataRow(VersionEnums.TTWars, 0, true)]
        public void IsActiveTest(VersionEnums version, int location, bool expected)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}.html");
            _doc.Load(file);

            var nodes = _instance[index].GetVillages(_doc);
            var actual = _instance[index].IsActive(nodes[location]);
            Assert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial, 0, 19_501)]
        [DataRow(VersionEnums.TTWars, 0, 255_147)]
        public void GetIdTest(VersionEnums version, int location, int expected)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}.html");
            _doc.Load(file);

            var nodes = _instance[index].GetVillages(_doc);
            var actual = _instance[index].GetId(nodes[location]);
            Assert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial, 0, "VinaVillage")]
        [DataRow(VersionEnums.TTWars, 0, "vinaghost`s village")]
        public void GetNameTest(VersionEnums version, int location, string expected)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}.html");
            _doc.Load(file);

            var nodes = _instance[index].GetVillages(_doc);
            var actual = _instance[index].GetName(nodes[location]);
            Assert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial, 0, 114)]
        [DataRow(VersionEnums.TTWars, 0, 28)]
        public void GetXTest(VersionEnums version, int location, int expected)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}.html");
            _doc.Load(file);

            var nodes = _instance[index].GetVillages(_doc);
            var actual = _instance[index].GetX(nodes[location]);
            Assert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial, 0, -32)]
        [DataRow(VersionEnums.TTWars, 0, 82)]
        public void GetYTest(VersionEnums version, int location, int expected)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}.html");
            _doc.Load(file);

            var nodes = _instance[index].GetVillages(_doc);
            var actual = _instance[index].GetY(nodes[location]);
            Assert.AreEqual(expected, actual);
        }
    }
}