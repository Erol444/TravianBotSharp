using HtmlAgilityPack;
using MainCore.Enums;
using MainCore.Parsers.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TestProject.Tests.Parser.TroopTrain
{
    [TestClass]
    public class TrainTroopParserTest
    {
        private static readonly List<ITrainTroopParser> _instance = new(){
            new MainCore.Parsers.Implementations.TravianOfficial.TrainTroopParser(),
            new MainCore.Parsers.Implementations.TTWars.TrainTroopParser(),
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

            Assert.AreEqual(_instance.Count, _version.Count);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial, BARRACK, 2)]
        [DataRow(VersionEnums.TravianOfficial, STABLE, 4)]
        [DataRow(VersionEnums.TravianOfficial, WORKSHOP, 2)]
        [DataRow(VersionEnums.TTWars, BARRACK, 2)]
        [DataRow(VersionEnums.TTWars, STABLE, 4)]
        [DataRow(VersionEnums.TTWars, WORKSHOP, 2)]
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
        [DataRow(VersionEnums.TTWars, BARRACK, 0, 21)]
        [DataRow(VersionEnums.TTWars, STABLE, 0, 23)]
        [DataRow(VersionEnums.TTWars, WORKSHOP, 0, 27)]
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
        [DataRow(VersionEnums.TTWars, BARRACK, 0, 100, 130, 55, 30)]
        [DataRow(VersionEnums.TTWars, STABLE, 0, 170, 150, 20, 40)]
        [DataRow(VersionEnums.TTWars, WORKSHOP, 0, 950, 555, 330, 75)]
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
        [DataRow(VersionEnums.TravianOfficial, BARRACK, 0, 191_000)]
        [DataRow(VersionEnums.TravianOfficial, STABLE, 0, 328_000)]
        [DataRow(VersionEnums.TravianOfficial, WORKSHOP, 0, 1_121_000)]
        [DataRow(VersionEnums.TTWars, BARRACK, 0, 7_024)]
        [DataRow(VersionEnums.TTWars, STABLE, 0, 9_186)]
        [DataRow(VersionEnums.TTWars, WORKSHOP, 0, 33_771)]
        public void GetTrainTimeTest(VersionEnums version, string type, int location, int expected)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}_{type}.html");
            _doc.Load(file);

            var nodes = _instance[index].GetTroopNodes(_doc);
            var actual = _instance[index].GetTrainTime(nodes[location]);
            Assert.AreEqual(TimeSpan.FromMilliseconds(expected), actual);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial, BARRACK, 0)]
        [DataRow(VersionEnums.TravianOfficial, STABLE, 0)]
        [DataRow(VersionEnums.TravianOfficial, WORKSHOP, 0)]
        [DataRow(VersionEnums.TTWars, BARRACK, 0)]
        [DataRow(VersionEnums.TTWars, STABLE, 0)]
        [DataRow(VersionEnums.TTWars, WORKSHOP, 0)]
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
        [DataRow(VersionEnums.TTWars, BARRACK, 0, 24_736)]
        [DataRow(VersionEnums.TTWars, STABLE, 0, 18_910)]
        [DataRow(VersionEnums.TTWars, WORKSHOP, 0, 3_518)]
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
        [DataRow(VersionEnums.TTWars, BARRACK)]
        [DataRow(VersionEnums.TTWars, STABLE)]
        [DataRow(VersionEnums.TTWars, WORKSHOP)]
        public void GetTrainButtonTest(VersionEnums version, string type)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}_{type}.html");
            _doc.Load(file);

            var node = _instance[index].GetTrainButton(_doc);
            Assert.IsNotNull(node);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial, BARRACK, 15153)]
        [DataRow(VersionEnums.TravianOfficial, STABLE, 26420)]
        [DataRow(VersionEnums.TravianOfficial, WORKSHOP, 0)]
        [DataRow(VersionEnums.TTWars, BARRACK, 0)]
        [DataRow(VersionEnums.TTWars, STABLE, 0)]
        [DataRow(VersionEnums.TTWars, WORKSHOP, 0)]
        public void GetQueueTrain(VersionEnums version, string type, int expected)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}_{type}.html");
            _doc.Load(file);

            var actual = _instance[index].GetQueueTrainTime(_doc);
            Assert.AreEqual(TimeSpan.FromSeconds(expected), actual);
        }
    }
}