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
    public class HeroSectionParserTest
    {
        private static readonly List<IHeroSectionParser> _instance = new(){
            new MainCore.Parsers.Implementations.TravianOfficial.HeroSectionParser(),
            new MainCore.Parsers.Implementations.TTWars.HeroSectionParser()
        };

        private static List<string> _version;

        private readonly HtmlDocument _doc = new();

        private readonly string _path = Path.Combine("Tests", "Parser", "HeroSection", "HtmlFiles");

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _version = Enum.GetNames(typeof(VersionEnums)).ToList();

            Assert.AreEqual(_instance.Count, _version.Count);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial, 100)]
        [DataRow(VersionEnums.TTWars, 79)]
        public void GetHealthTest(VersionEnums version, int expected)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}_inventory.html");
            _doc.Load(file);

            var actual = _instance[index].GetHealth(_doc);
            Assert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial, 1)]
        [DataRow(VersionEnums.TTWars, 1)]
        public void GetStatusTest(VersionEnums version, int expected)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}_inventory.html");
            _doc.Load(file);

            var actual = _instance[index].GetStatus(_doc);
            Assert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial, 19)]
        [DataRow(VersionEnums.TTWars, 2)]
        public void GetAdventureNumTest(VersionEnums version, int expected)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}_inventory.html");
            _doc.Load(file);

            var actual = _instance[index].GetAdventureNum(_doc);
            Assert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial)]
        [DataRow(VersionEnums.TTWars)]
        public void GetHeroAvatarTest(VersionEnums version)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}_inventory.html");
            _doc.Load(file);

            var actual = _instance[index].GetHeroAvatar(_doc);
            Assert.IsNotNull(actual);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial)]
        [DataRow(VersionEnums.TTWars)]
        public void GetAdventuresButtonTest(VersionEnums version)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}_inventory.html");
            _doc.Load(file);

            var actual = _instance[index].GetAdventuresButton(_doc);
            Assert.IsNotNull(actual);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial, 2)]
        [DataRow(VersionEnums.TTWars, 3)]
        public void GetHeroTabTest(VersionEnums version, int location)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}_inventory.html");
            _doc.Load(file);

            var actual = _instance[index].GetHeroTab(_doc, location);
            Assert.IsNotNull(actual);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial, 0, false)]
        [DataRow(VersionEnums.TravianOfficial, 1, true)]
        [DataRow(VersionEnums.TTWars, 0, false)]
        [DataRow(VersionEnums.TTWars, 1, true)]
        public void IsCurrentTabTest(VersionEnums version, int location, bool expected)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}_inventory.html");
            _doc.Load(file);

            var node = _instance[index].GetHeroTab(_doc, location);
            var actual = _instance[index].IsCurrentTab(node);
            Assert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial, 31)]
        [DataRow(VersionEnums.TTWars, 6)]
        public void GetItemsTest(VersionEnums version, int expected)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}_inventory.html");
            _doc.Load(file);

            var actual = _instance[index].GetItems(_doc).Count;
            Assert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial, 145)]
        [DataRow(VersionEnums.TTWars, 145)]
        public void GetItemSlotTest(VersionEnums version, int location)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}_inventory.html");
            _doc.Load(file);

            var actual = _instance[index].GetItemSlot(_doc, location);
            Assert.IsNotNull(actual);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial, 19)]
        [DataRow(VersionEnums.TTWars, 5)]
        public void GetAdventuresTest(VersionEnums version, int expected)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}_adventures.html");
            _doc.Load(file);

            var actual = _instance[index].GetAdventures(_doc).Count;
            Assert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial, 1, 1)]
        [DataRow(VersionEnums.TTWars, 4, 1)]
        public void GetAdventureDifficultTest(VersionEnums version, int location, int expected)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}_adventures.html");
            _doc.Load(file);

            var node = _instance[index].GetAdventures(_doc)[location];
            var actual = _instance[index].GetAdventureDifficult(node);
            Assert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial, 0, 101, -31)]
        [DataRow(VersionEnums.TTWars, 0, 26, 75)]
        public void GetAdventureCoordinatesTest(VersionEnums version, int location, int xExpected, int yExpected)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}_adventures.html");
            _doc.Load(file);

            var node = _instance[index].GetAdventures(_doc)[location];
            (var xActual, var yActual) = _instance[index].GetAdventureCoordinates(node);
            Assert.AreEqual(xExpected, xActual);
            Assert.AreEqual(yExpected, yActual);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial, 101, -31)]
        [DataRow(VersionEnums.TTWars, 26, 75)]
        public void GetStartAdventureButtonTest(VersionEnums version, int x, int y)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}_adventures.html");
            _doc.Load(file);

            var actual = _instance[index].GetStartAdventureButton(_doc, x, y);
            Assert.IsNotNull(actual);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial)]
        [DataRow(VersionEnums.TTWars)]
        public void GetAmountBoxTest(VersionEnums version)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}_inputdialog.html");
            _doc.Load(file);

            var actual = _instance[index].GetAmountBox(_doc);
            Assert.IsNotNull(actual);
        }

        [DataTestMethod]
        [DataRow(VersionEnums.TravianOfficial)]
        [DataRow(VersionEnums.TTWars)]
        public void GetConfirmButtonTest(VersionEnums version)
        {
            var index = (int)version;
            var file = Path.Combine(_path, $"{_version[index]}_inputdialog.html");
            _doc.Load(file);

            var actual = _instance[index].GetConfirmButton(_doc);
            Assert.IsNotNull(actual);
        }
    }
}