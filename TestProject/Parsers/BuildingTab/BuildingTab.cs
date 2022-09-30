using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestProject.Parsers.BuildingTab
{
    [TestClass]
    public class BuildingTab
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
        public void hm()
        {
        }
    }
}