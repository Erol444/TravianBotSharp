using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestProject.Parsers
{
    [TestClass]
    public class LoginPage
    {
        private readonly HtmlDocument _ttwarsDoc = new();
        private readonly HtmlDocument _travianDoc = new();

        [TestInitialize]
        public void InitializeTests()
        {
            _ttwarsDoc.Load("TestFile/LoginPage/TTWars.html");
            _travianDoc.Load("TestFile/LoginPage/Travian.html");
        }

        [TestMethod]
        public void GetUsernameNode()
        {
            var node = TTWarsCore.Parsers.LoginPage.GetUsernameNode(_ttwarsDoc);
            Assert.IsNotNull(node);
            node = TravianOffcialCore.Parsers.LoginPage.GetUsernameNode(_travianDoc);
            Assert.IsNotNull(node);
            node = TravianOfficalNewHeroUICore.Parsers.LoginPage.GetUsernameNode(_travianDoc);
            Assert.IsNotNull(node);
        }

        [TestMethod]
        public void GetPasswordNode()
        {
            var node = TTWarsCore.Parsers.LoginPage.GetPasswordNode(_ttwarsDoc);
            Assert.IsNotNull(node);
            node = TravianOffcialCore.Parsers.LoginPage.GetPasswordNode(_travianDoc);
            Assert.IsNotNull(node);
            node = TravianOfficalNewHeroUICore.Parsers.LoginPage.GetPasswordNode(_travianDoc);
            Assert.IsNotNull(node);
        }

        [TestMethod]
        public void GetLoginButton()
        {
            var node = TTWarsCore.Parsers.LoginPage.GetLoginButton(_ttwarsDoc);
            Assert.IsNotNull(node);
            node = TravianOffcialCore.Parsers.LoginPage.GetLoginButton(_travianDoc);
            Assert.IsNotNull(node);
            node = TravianOfficalNewHeroUICore.Parsers.LoginPage.GetLoginButton(_travianDoc);
            Assert.IsNotNull(node);
        }
    }
}