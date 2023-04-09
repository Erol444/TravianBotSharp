using HtmlAgilityPack;
using MainCore.Enums;
using MainCore.Parsers.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TestProject.Parser.TroopTrain
{
    [TestClass]
    public class TrainTroopParserTest
    {
        private static readonly List<ITrainTroopParser> _instance = new(){
            new MainCore.Parsers.Implementations.TravianOfficial.TrainTroopParser(),
        };

        private static List<string> _version;

        private readonly HtmlDocument _doc = new();

        private readonly string _path = Path.Combine("Parser", "TroopTrain", "HtmlFiles");

        private const string BARRACK = "barrack";
        private const string STABLE = "stable";
        private const string WORKSHOP = "workshop";

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _version = Enum.GetNames(typeof(VersionEnums)).ToList();

            //Assert.AreEqual(_instance.Count, _version.Count);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial, BARRACK, 2)]
        [DataRow(VersionEnums.TravianOfficial, STABLE, 4)]
        [DataRow(VersionEnums.TravianOfficial, WORKSHOP, 2)]
        public void GetTroopNodesTest(VersionEnums version, string type, int expected)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}_{type}.html");
            _doc.Load(file);

            var nodes = _instance[index].GetTroopNodes(_doc);
            Assert.AreEqual(expected, nodes.Count);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial, BARRACK, 0, 22)]
        [DataRow(VersionEnums.TravianOfficial, STABLE, 0, 24)]
        [DataRow(VersionEnums.TravianOfficial, WORKSHOP, 0, 27)]
        public void GetTroopTypeTest(VersionEnums version, string type, int location, int expected)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}_{type}.html");
            _doc.Load(file);

            var nodes = _instance[index].GetTroopNodes(_doc);
            var actual = _instance[index].GetTroopType(nodes[location]);
            Assert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial, BARRACK, 0, 140, 150, 185, 60)]
        [DataRow(VersionEnums.TravianOfficial, STABLE, 0, 350, 450, 230, 60)]
        [DataRow(VersionEnums.TravianOfficial, WORKSHOP, 0, 950, 555, 330, 75)]
        public void GetTrainCostTest(VersionEnums version, string type, int location, int wood, int clay, int iron, int crop)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}_{type}.html");
            _doc.Load(file);

            var nodes = _instance[index].GetTroopNodes(_doc);
            var actual = _instance[index].GetTrainCost(nodes[location]);
            Assert.AreEqual(wood, actual.Wood);
            Assert.AreEqual(clay, actual.Clay);
            Assert.AreEqual(iron, actual.Iron);
            Assert.AreEqual(crop, actual.Crop);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial, BARRACK, 0, 191)]
        [DataRow(VersionEnums.TravianOfficial, STABLE, 0, 328)]
        [DataRow(VersionEnums.TravianOfficial, WORKSHOP, 0, 1121)]
        public void GetTrainTimeTest(VersionEnums version, string type, int location, int expected)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}_{type}.html");
            _doc.Load(file);

            var nodes = _instance[index].GetTroopNodes(_doc);
            var actual = _instance[index].GetTrainTime(nodes[location]);
            Assert.AreEqual(TimeSpan.FromSeconds(expected), actual);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial, BARRACK, 0)]
        [DataRow(VersionEnums.TravianOfficial, STABLE, 0)]
        [DataRow(VersionEnums.TravianOfficial, WORKSHOP, 0)]
        public void GetInputBoxTest(VersionEnums version, string type, int location)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}_{type}.html");
            _doc.Load(file);

            var nodes = _instance[index].GetTroopNodes(_doc);
            var node = _instance[index].GetInputBox(nodes[location]);
            Assert.IsNotNull(node);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial, BARRACK, 0, 108)]
        [DataRow(VersionEnums.TravianOfficial, STABLE, 0, 86)]
        [DataRow(VersionEnums.TravianOfficial, WORKSHOP, 0, 51)]
        public void GetMaxAmountTest(VersionEnums version, string type, int location, int expected)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}_{type}.html");
            _doc.Load(file);

            var nodes = _instance[index].GetTroopNodes(_doc);
            var actual = _instance[index].GetMaxAmount(nodes[location]);
            Assert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial, BARRACK)]
        [DataRow(VersionEnums.TravianOfficial, STABLE)]
        [DataRow(VersionEnums.TravianOfficial, WORKSHOP)]
        public void GetTrainButtonTest(VersionEnums version, string type)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}_{type}.html");
            _doc.Load(file);

            var node = _instance[index].GetTrainButton(_doc);
            Assert.IsNotNull(node);
        }
    }
}