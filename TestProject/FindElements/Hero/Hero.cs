using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestProject.FindElements
{
    [TestClass]
    public class Hero
    {
        [TestMethod]
        public void TTWarsGetHealth()
        {
            var doc = new HtmlDocument();
            doc.Load("FindElements/Hero/inventory/TTWars.html");
            var value = TTWarsCore.FindElements.Hero.GetItemSlot(doc, 114);
            Assert.IsNotNull(value);
        }

        [TestMethod]
        public void TravianOfficialGetHealth()
        {
            var doc = new HtmlDocument();
            doc.Load("FindElements/Hero/inventory/Travian.html");
            var value = TravianOfficialCore.FindElements.Hero.GetItemSlot(doc, 108);
            Assert.IsNotNull(value);
        }

        [TestMethod]
        public void TravianOfficialHeroGetHealth()
        {
            var doc = new HtmlDocument();
            doc.Load("FindElements/Hero/inventory/TravianHeroUI.html");
            var value = TravianOfficialNewHeroUICore.FindElements.Hero.GetItemSlot(doc, 58);
            Assert.IsNotNull(value);
        }

        [TestMethod]
        public void TTWarsGetAmountBox()
        {
            var doc = new HtmlDocument();
            doc.Load("FindElements/Hero/dialog/TTWars.html");
            var value = TTWarsCore.FindElements.Hero.GetAmountBox(doc);
            Assert.IsNotNull(value);
        }

        [TestMethod]
        public void TravianOfficialGetAmountBox()
        {
            var doc = new HtmlDocument();
            doc.Load("FindElements/Hero/dialog/Travian.html");
            var value = TravianOfficialCore.FindElements.Hero.GetAmountBox(doc);
            Assert.IsNotNull(value);
        }

        [TestMethod]
        public void TravianOfficialHeroGetAmountBox()
        {
            var doc = new HtmlDocument();
            doc.Load("FindElements/Hero/dialog/TravianHeroUI.html");
            var value = TravianOfficialNewHeroUICore.FindElements.Hero.GetAmountBox(doc);
            Assert.IsNotNull(value);
        }

        [TestMethod]
        public void TTWarsGetConfirmButton()
        {
            var doc = new HtmlDocument();
            doc.Load("FindElements/Hero/dialog/TTWars.html");
            var value = TTWarsCore.FindElements.Hero.GetConfirmButton(doc);
            Assert.IsNotNull(value);
        }

        [TestMethod]
        public void TravianOfficialGetConfirmButton()
        {
            var doc = new HtmlDocument();
            doc.Load("FindElements/Hero/dialog/Travian.html");
            var value = TravianOfficialCore.FindElements.Hero.GetConfirmButton(doc);
            Assert.IsNotNull(value);
        }

        [TestMethod]
        public void TravianOfficialHeroGetConfirmButton()
        {
            var doc = new HtmlDocument();
            doc.Load("FindElements/Hero/dialog/TravianHeroUI.html");
            var value = TravianOfficialNewHeroUICore.FindElements.Hero.GetConfirmButton(doc);
            Assert.IsNotNull(value);
        }
    }
}