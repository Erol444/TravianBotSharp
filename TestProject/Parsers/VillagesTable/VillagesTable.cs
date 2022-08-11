using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestProject.Parsers
{
    [TestClass]
    public class VillagesTable
    {
        private readonly HtmlDocument _ttwarsDoc = new();
        private readonly HtmlDocument _travianDoc = new();
        private readonly HtmlDocument _travianHeroDoc = new();

        [TestInitialize]
        public void InitializeTests()
        {
            _ttwarsDoc.Load("Parsers/VillagesTable/TTWars.html");
            _travianDoc.Load("Parsers/VillagesTable/Travian.html");
            _travianHeroDoc.Load("Parsers/VillagesTable/TravianHeroUI.html");
        }

        [TestMethod]
        public void TTWarsGetVillageNodes()
        {
            var nodes = TTWarsCore.Parsers.VillagesTable.GetVillageNodes(_ttwarsDoc);
            Assert.AreEqual(1, nodes.Count);
        }

        [TestMethod]
        public void TravianOfficialGetVillageNodes()
        {
            var nodes = TravianOfficialCore.Parsers.VillagesTable.GetVillageNodes(_travianDoc);
            Assert.AreEqual(4, nodes.Count);
        }

        [TestMethod]
        public void TravianOfficialHeroGetVillageNodes()
        {
            var nodes = TravianOfficialNewHeroUICore.Parsers.VillagesTable.GetVillageNodes(_travianHeroDoc);
            Assert.AreEqual(1, nodes.Count);
        }

        [TestMethod]
        public void TTWarsIsUnderAttack()
        {
            var nodes = TTWarsCore.Parsers.VillagesTable.GetVillageNodes(_ttwarsDoc);
            var result = TTWarsCore.Parsers.VillagesTable.IsUnderAttack(nodes[0]);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TravianOfficialIsUnderAttack()
        {
            var nodes = TravianOfficialCore.Parsers.VillagesTable.GetVillageNodes(_travianDoc);
            var result = TravianOfficialCore.Parsers.VillagesTable.IsUnderAttack(nodes[0]);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TravianOfficialHeroIsUnderAttack()
        {
            var nodes = TravianOfficialNewHeroUICore.Parsers.VillagesTable.GetVillageNodes(_travianHeroDoc);
            var result = TravianOfficialNewHeroUICore.Parsers.VillagesTable.IsUnderAttack(nodes[0]);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TTWarsIsActive()
        {
            var nodes = TTWarsCore.Parsers.VillagesTable.GetVillageNodes(_ttwarsDoc);
            var result = TTWarsCore.Parsers.VillagesTable.IsActive(nodes[0]);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TravianOfficialIsActive()
        {
            var nodes = TravianOfficialCore.Parsers.VillagesTable.GetVillageNodes(_travianDoc);
            var result = TravianOfficialCore.Parsers.VillagesTable.IsActive(nodes[0]);
            Assert.IsFalse(result);
            result = TravianOfficialCore.Parsers.VillagesTable.IsActive(nodes[1]);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TravianOfficialHeroIsActive()
        {
            var nodes = TravianOfficialNewHeroUICore.Parsers.VillagesTable.GetVillageNodes(_travianHeroDoc);
            var result = TravianOfficialCore.Parsers.VillagesTable.IsActive(nodes[0]);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TTWarsGetId()
        {
            var nodes = TTWarsCore.Parsers.VillagesTable.GetVillageNodes(_ttwarsDoc);
            var result = TTWarsCore.Parsers.VillagesTable.GetId(nodes[0]);
            Assert.AreEqual(22385, result);
        }

        [TestMethod]
        public void TravianOfficialGetId()
        {
            var nodes = TravianOfficialCore.Parsers.VillagesTable.GetVillageNodes(_travianDoc);
            var result = TravianOfficialCore.Parsers.VillagesTable.GetId(nodes[0]);
            Assert.AreEqual(19176, result);
        }

        [TestMethod]
        public void TravianOfficialHeroGetId()
        {
            var nodes = TravianOfficialNewHeroUICore.Parsers.VillagesTable.GetVillageNodes(_travianHeroDoc);
            var result = TravianOfficialCore.Parsers.VillagesTable.GetId(nodes[0]);
            Assert.AreEqual(20212, result);
        }

        [TestMethod]
        public void TTWarsGetName()
        {
            var nodes = TTWarsCore.Parsers.VillagesTable.GetVillageNodes(_ttwarsDoc);
            var result = TTWarsCore.Parsers.VillagesTable.GetName(nodes[0]);
            Assert.AreEqual("vinaghost`s village", result);
        }

        [TestMethod]
        public void TravianOfficialGetName()
        {
            var nodes = TravianOfficialCore.Parsers.VillagesTable.GetVillageNodes(_travianDoc);
            var result = TravianOfficialCore.Parsers.VillagesTable.GetName(nodes[0]);
            Assert.AreEqual("ASOJEN", result);
        }

        [TestMethod]
        public void TravianOfficialHeroGetName()
        {
            var nodes = TravianOfficialNewHeroUICore.Parsers.VillagesTable.GetVillageNodes(_travianHeroDoc);
            var result = TravianOfficialNewHeroUICore.Parsers.VillagesTable.GetName(nodes[0]);
            Assert.AreEqual("closercry`s village", result);
        }

        [TestMethod]
        public void TTWarsGetX()
        {
            var nodes = TTWarsCore.Parsers.VillagesTable.GetVillageNodes(_ttwarsDoc);
            var result = TTWarsCore.Parsers.VillagesTable.GetX(nodes[0]);
            Assert.AreEqual(-27, result);
        }

        [TestMethod]
        public void TravianOfficialGetX()
        {
            var nodes = TravianOfficialCore.Parsers.VillagesTable.GetVillageNodes(_travianDoc);
            var result = TravianOfficialCore.Parsers.VillagesTable.GetX(nodes[0]);
            Assert.AreEqual(30, result);
        }

        [TestMethod]
        public void TravianOfficialHeroGetX()
        {
            var nodes = TravianOfficialNewHeroUICore.Parsers.VillagesTable.GetVillageNodes(_travianHeroDoc);
            var result = TravianOfficialNewHeroUICore.Parsers.VillagesTable.GetX(nodes[0]);
            Assert.AreEqual(-17, result);
        }

        [TestMethod]
        public void TTWarsGetY()
        {
            var nodes = TTWarsCore.Parsers.VillagesTable.GetVillageNodes(_ttwarsDoc);
            var result = TTWarsCore.Parsers.VillagesTable.GetY(nodes[0]);
            Assert.AreEqual(-11, result);
        }

        [TestMethod]
        public void TravianOfficialGetY()
        {
            var nodes = TravianOfficialCore.Parsers.VillagesTable.GetVillageNodes(_travianDoc);
            var result = TravianOfficialCore.Parsers.VillagesTable.GetY(nodes[0]);
            Assert.AreEqual(-57, result);
        }

        [TestMethod]
        public void TravianOfficialHeroGetY()
        {
            var nodes = TravianOfficialNewHeroUICore.Parsers.VillagesTable.GetVillageNodes(_travianHeroDoc);
            var result = TravianOfficialNewHeroUICore.Parsers.VillagesTable.GetY(nodes[0]);
            Assert.AreEqual(-68, result);
        }
    }
}