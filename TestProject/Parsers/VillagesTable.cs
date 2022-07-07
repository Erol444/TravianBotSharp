using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestProject.Parsers
{
    [TestClass]
    public class VillagesTable
    {
        private readonly HtmlDocument _ttwarsDoc = new();
        private readonly HtmlDocument _travianDoc = new();

        [TestInitialize]
        public void InitializeTests()
        {
            _ttwarsDoc.Load("TestFile/Dorf1/TTWars.html");
            _travianDoc.Load("TestFile/Dorf1/Travian.html");
        }

        [TestMethod]
        public void GetVillageNodes()
        {
            var nodes = TTWarsCore.Parsers.VillagesTable.GetVillageNodes(_ttwarsDoc);
            Assert.AreEqual(1, nodes.Count);
            nodes = TravianOffcialCore.Parsers.VillagesTable.GetVillageNodes(_travianDoc);
            Assert.AreEqual(4, nodes.Count);
            nodes = TravianOfficalNewHeroUICore.Parsers.VillagesTable.GetVillageNodes(_travianDoc);
            Assert.AreEqual(4, nodes.Count);
        }

        [TestMethod]
        public void IsUnderAttack()
        {
            var nodes = TTWarsCore.Parsers.VillagesTable.GetVillageNodes(_ttwarsDoc);
            var result = TTWarsCore.Parsers.VillagesTable.IsUnderAttack(nodes[0]);
            Assert.IsFalse(result);
            nodes = TravianOffcialCore.Parsers.VillagesTable.GetVillageNodes(_travianDoc);
            result = TravianOffcialCore.Parsers.VillagesTable.IsUnderAttack(nodes[0]);
            Assert.IsFalse(result);
            nodes = TravianOfficalNewHeroUICore.Parsers.VillagesTable.GetVillageNodes(_travianDoc);
            result = TravianOfficalNewHeroUICore.Parsers.VillagesTable.IsUnderAttack(nodes[0]);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsActive()
        {
            var nodes = TTWarsCore.Parsers.VillagesTable.GetVillageNodes(_ttwarsDoc);
            var result = TTWarsCore.Parsers.VillagesTable.IsActive(nodes[0]);
            Assert.IsTrue(result);
            nodes = TravianOffcialCore.Parsers.VillagesTable.GetVillageNodes(_travianDoc);
            result = TravianOffcialCore.Parsers.VillagesTable.IsActive(nodes[0]);
            Assert.IsFalse(result);
            result = TravianOffcialCore.Parsers.VillagesTable.IsActive(nodes[2]);
            Assert.IsTrue(result);

            nodes = TravianOfficalNewHeroUICore.Parsers.VillagesTable.GetVillageNodes(_travianDoc);
            result = TravianOfficalNewHeroUICore.Parsers.VillagesTable.IsActive(nodes[0]);
            Assert.IsFalse(result);
            result = TravianOffcialCore.Parsers.VillagesTable.IsActive(nodes[2]);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void GetId()
        {
            var nodes = TTWarsCore.Parsers.VillagesTable.GetVillageNodes(_ttwarsDoc);
            var result = TTWarsCore.Parsers.VillagesTable.GetId(nodes[0]);
            Assert.AreEqual(22385, result);
            nodes = TravianOffcialCore.Parsers.VillagesTable.GetVillageNodes(_travianDoc);
            result = TravianOffcialCore.Parsers.VillagesTable.GetId(nodes[0]);
            Assert.AreEqual(19312, result);
            nodes = TravianOfficalNewHeroUICore.Parsers.VillagesTable.GetVillageNodes(_travianDoc);
            result = TravianOfficalNewHeroUICore.Parsers.VillagesTable.GetId(nodes[0]);
            Assert.AreEqual(19312, result);
        }

        [TestMethod]
        public void GetName()
        {
            var nodes = TTWarsCore.Parsers.VillagesTable.GetVillageNodes(_ttwarsDoc);
            var result = TTWarsCore.Parsers.VillagesTable.GetName(nodes[0]);
            Assert.AreEqual("vinaghost`s village", result);
            nodes = TravianOffcialCore.Parsers.VillagesTable.GetVillageNodes(_travianDoc);
            result = TravianOffcialCore.Parsers.VillagesTable.GetName(nodes[0]);
            Assert.AreEqual("7ld", result);
            nodes = TravianOfficalNewHeroUICore.Parsers.VillagesTable.GetVillageNodes(_travianDoc);
            result = TravianOfficalNewHeroUICore.Parsers.VillagesTable.GetName(nodes[0]);
            Assert.AreEqual("7ld", result);
        }

        [TestMethod]
        public void GetX()
        {
            var nodes = TTWarsCore.Parsers.VillagesTable.GetVillageNodes(_ttwarsDoc);
            var result = TTWarsCore.Parsers.VillagesTable.GetX(nodes[0]);
            Assert.AreEqual(-27, result);
            nodes = TravianOffcialCore.Parsers.VillagesTable.GetVillageNodes(_travianDoc);
            result = TravianOffcialCore.Parsers.VillagesTable.GetX(nodes[0]);
            Assert.AreEqual(50, result);
            nodes = TravianOfficalNewHeroUICore.Parsers.VillagesTable.GetVillageNodes(_travianDoc);
            result = TravianOfficalNewHeroUICore.Parsers.VillagesTable.GetX(nodes[0]);
            Assert.AreEqual(50, result);
        }

        [TestMethod]
        public void GetY()
        {
            var nodes = TTWarsCore.Parsers.VillagesTable.GetVillageNodes(_ttwarsDoc);
            var result = TTWarsCore.Parsers.VillagesTable.GetY(nodes[0]);
            Assert.AreEqual(-11, result);
            nodes = TravianOffcialCore.Parsers.VillagesTable.GetVillageNodes(_travianDoc);
            result = TravianOffcialCore.Parsers.VillagesTable.GetY(nodes[0]);
            Assert.AreEqual(-42, result);
            nodes = TravianOfficalNewHeroUICore.Parsers.VillagesTable.GetVillageNodes(_travianDoc);
            result = TravianOfficalNewHeroUICore.Parsers.VillagesTable.GetY(nodes[0]);
            Assert.AreEqual(-42, result);
        }
    }
}