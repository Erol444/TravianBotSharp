using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestProject.FindElements
{
    [TestClass]
    public class LoginPage
    {
        private readonly HtmlDocument _ttwarsDoc = new();
        private readonly HtmlDocument _travianDoc = new();

        [TestInitialize]
        public void InitializeTests()
        {
            _ttwarsDoc.Load("FindElements/LoginPage_TTWars.html");
            _travianDoc.Load("FindElements/LoginPage_Travian.html");
        }

        [TestMethod]
        public void GetUsernameNode()
        {
            var node = TTWarsCore.FindElements.LoginPage.GetUsernameNode(_ttwarsDoc);
            Assert.IsNotNull(node);
            node = TravianOffcialCore.FindElements.LoginPage.GetUsernameNode(_travianDoc);
            Assert.IsNotNull(node);
            node = TravianOfficalNewHeroUICore.FindElements.LoginPage.GetUsernameNode(_travianDoc);
            Assert.IsNotNull(node);
        }

        [TestMethod]
        public void GetPasswordNode()
        {
            var node = TTWarsCore.FindElements.LoginPage.GetPasswordNode(_ttwarsDoc);
            Assert.IsNotNull(node);
            node = TravianOffcialCore.FindElements.LoginPage.GetPasswordNode(_travianDoc);
            Assert.IsNotNull(node);
            node = TravianOfficalNewHeroUICore.FindElements.LoginPage.GetPasswordNode(_travianDoc);
            Assert.IsNotNull(node);
        }

        [TestMethod]
        public void GetLoginButton()
        {
            var node = TTWarsCore.FindElements.LoginPage.GetLoginButton(_ttwarsDoc);
            Assert.IsNotNull(node);
            node = TravianOffcialCore.FindElements.LoginPage.GetLoginButton(_travianDoc);
            Assert.IsNotNull(node);
            node = TravianOfficalNewHeroUICore.FindElements.LoginPage.GetLoginButton(_travianDoc);
            Assert.IsNotNull(node);
        }
    }
}