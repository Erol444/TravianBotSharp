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
            _ttwarsDoc.Load("FindElements/LoginPage/TTWars.html");
            _travianDoc.Load("FindElements/LoginPage/Travian.html");
        }

        [TestMethod]
        public void TTWarsGetUsernameNode()
        {
            var node = TTWarsCore.FindElements.LoginPage.GetUsernameNode(_ttwarsDoc);
            Assert.IsNotNull(node);
        }

        [TestMethod]
        public void TravianOfficialGetUsernameNode()
        {
            var node = TravianOfficialCore.FindElements.LoginPage.GetUsernameNode(_travianDoc);
            Assert.IsNotNull(node);
        }

        [TestMethod]
        public void TravianOfficialHeroGetUsernameNode()
        {
            var node = TravianOfficialNewHeroUICore.FindElements.LoginPage.GetUsernameNode(_travianDoc);
            Assert.IsNotNull(node);
        }

        [TestMethod]
        public void TTWarsGetPasswordNode()
        {
            var node = TTWarsCore.FindElements.LoginPage.GetPasswordNode(_ttwarsDoc);
            Assert.IsNotNull(node);
        }

        [TestMethod]
        public void TravianOfficialGetPasswordNode()
        {
            var node = TravianOfficialCore.FindElements.LoginPage.GetPasswordNode(_travianDoc);
            Assert.IsNotNull(node);
        }

        [TestMethod]
        public void TravianOfficiaHeroGetPasswordNode()
        {
            var node = TravianOfficialNewHeroUICore.FindElements.LoginPage.GetPasswordNode(_travianDoc);
            Assert.IsNotNull(node);
        }

        [TestMethod]
        public void TTWarsGetLoginButton()
        {
            var node = TTWarsCore.FindElements.LoginPage.GetLoginButton(_ttwarsDoc);
            Assert.IsNotNull(node);
        }

        [TestMethod]
        public void TravianOfficialGetLoginButton()
        {
            var node = TravianOfficialCore.FindElements.LoginPage.GetLoginButton(_travianDoc);
            Assert.IsNotNull(node);
        }

        [TestMethod]
        public void TravianOfficialHeroGetLoginButton()
        {
            var node = TravianOfficialNewHeroUICore.FindElements.LoginPage.GetLoginButton(_travianDoc);
            Assert.IsNotNull(node);
        }
    }
}