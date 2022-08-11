using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestProject.Parsers
{
    [TestClass]
    public class RightBar
    {
        private readonly HtmlDocument _ttwarsDoc = new();
        private readonly HtmlDocument _travianDoc = new();
        private readonly HtmlDocument _travianHeroDoc = new();

        [TestInitialize]
        public void InitializeTests()
        {
            _ttwarsDoc.Load("Parsers/RightBar/TTWars.html");
            _travianDoc.Load("Parsers/RightBar/Travian.html");
            _travianHeroDoc.Load("Parsers/RightBar/TravianHeroUI.html");
        }

        [TestMethod]
        public void TTWarsHasPlusAccount()
        {
            var value = TTWarsCore.Parsers.RightBar.HasPlusAccount(_ttwarsDoc);
            Assert.IsNotNull(value);
        }

        [TestMethod]
        public void TravianOfficialHasPlusAccount()
        {
            var value = TravianOfficialCore.Parsers.RightBar.HasPlusAccount(_travianDoc);
            Assert.IsNotNull(value);
        }

        [TestMethod]
        public void TravianOfficialHeroHasPlusAccount()
        {
            var value = TravianOfficialCore.Parsers.RightBar.HasPlusAccount(_travianHeroDoc);
            Assert.IsNotNull(value);
        }
    }
}