using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestProject.FindElements
{
    [TestClass]
    public class HeroPage
    {
        [TestMethod]
        public void TTWarsGetHeroInventory()
        {
            var doc = new HtmlDocument();
            doc.Load("FindElements/HeroPage/inventory/TTWars.html");
            var value = TTWarsCore.FindElements.HeroPage.GetHeroInventory(doc);
            Assert.IsNotNull(value);
        }

        [TestMethod]
        public void TravianOfficialGetHeroInventory()
        {
            var doc = new HtmlDocument();
            doc.Load("FindElements/HeroPage/inventory/Travian.html");
            var value = TravianOfficialCore.FindElements.HeroPage.GetHeroInventory(doc);
            Assert.IsNotNull(value);
        }

        [TestMethod]
        public void TravianOfficialHeroGetHeroAvatar()
        {
            var doc = new HtmlDocument();
            doc.Load("FindElements/HeroPage/inventory/TravianHeroUI.html");
            var value = TravianOfficialNewHeroUICore.FindElements.HeroPage.GetHeroAvatar(doc);
            Assert.IsNotNull(value);
        }

        [TestMethod]
        public void TTWarsGetAdventuresButton()
        {
            var doc = new HtmlDocument();
            doc.Load("FindElements/HeroPage/inventory/TTWars.html");
            var value = TTWarsCore.FindElements.HeroPage.GetAdventuresButton(doc);
            Assert.IsNotNull(value);
        }

        [TestMethod]
        public void TravianOfficialGetAdventuresButton()
        {
            var doc = new HtmlDocument();
            doc.Load("FindElements/HeroPage/inventory/Travian.html");
            var value = TravianOfficialCore.FindElements.HeroPage.GetAdventuresButton(doc);
            Assert.IsNotNull(value);
        }

        [TestMethod]
        public void TravianOfficialHeroGetAdventuresButton()
        {
            var doc = new HtmlDocument();
            doc.Load("FindElements/HeroPage/inventory/TravianHeroUI.html");
            var value = TravianOfficialNewHeroUICore.FindElements.HeroPage.GetAdventuresButton(doc);
            Assert.IsNotNull(value);
        }

        [TestMethod]
        public void TTWarsGetHealth()
        {
            var doc = new HtmlDocument();
            doc.Load("FindElements/HeroPage/inventory/TTWars.html");
            var value = TTWarsCore.FindElements.HeroPage.GetItemSlot(doc, 114);
            Assert.IsNotNull(value);
        }

        [TestMethod]
        public void TravianOfficialGetHealth()
        {
            var doc = new HtmlDocument();
            doc.Load("FindElements/HeroPage/inventory/Travian.html");
            var value = TravianOfficialCore.FindElements.HeroPage.GetItemSlot(doc, 108);
            Assert.IsNotNull(value);
        }

        [TestMethod]
        public void TravianOfficialHeroGetHealth()
        {
            var doc = new HtmlDocument();
            doc.Load("FindElements/HeroPage/inventory/TravianHeroUI.html");
            var value = TravianOfficialNewHeroUICore.FindElements.HeroPage.GetItemSlot(doc, 58);
            Assert.IsNotNull(value);
        }

        [TestMethod]
        public void TTWarsGetAmountBox()
        {
            var doc = new HtmlDocument();
            doc.Load("FindElements/HeroPage/dialog/TTWars.html");
            var value = TTWarsCore.FindElements.HeroPage.GetAmountBox(doc);
            Assert.IsNotNull(value);
        }

        [TestMethod]
        public void TravianOfficialGetAmountBox()
        {
            var doc = new HtmlDocument();
            doc.Load("FindElements/HeroPage/dialog/Travian.html");
            var value = TravianOfficialCore.FindElements.HeroPage.GetAmountBox(doc);
            Assert.IsNotNull(value);
        }

        [TestMethod]
        public void TravianOfficialHeroGetAmountBox()
        {
            var doc = new HtmlDocument();
            doc.Load("FindElements/HeroPage/dialog/TravianHeroUI.html");
            var value = TravianOfficialNewHeroUICore.FindElements.HeroPage.GetAmountBox(doc);
            Assert.IsNotNull(value);
        }

        [TestMethod]
        public void TTWarsGetConfirmButton()
        {
            var doc = new HtmlDocument();
            doc.Load("FindElements/HeroPage/dialog/TTWars.html");
            var value = TTWarsCore.FindElements.HeroPage.GetConfirmButton(doc);
            Assert.IsNotNull(value);
        }

        [TestMethod]
        public void TravianOfficialGetConfirmButton()
        {
            var doc = new HtmlDocument();
            doc.Load("FindElements/HeroPage/dialog/Travian.html");
            var value = TravianOfficialCore.FindElements.HeroPage.GetConfirmButton(doc);
            Assert.IsNotNull(value);
        }

        [TestMethod]
        public void TravianOfficialHeroGetConfirmButton()
        {
            var doc = new HtmlDocument();
            doc.Load("FindElements/HeroPage/dialog/TravianHeroUI.html");
            var value = TravianOfficialNewHeroUICore.FindElements.HeroPage.GetConfirmButton(doc);
            Assert.IsNotNull(value);
        }

        [TestMethod]
        public void TTWarsGetStartAdventureButton()
        {
            var doc = new HtmlDocument();
            doc.Load("FindElements/HeroPage/adventure/TTWars.html");
            var value = TTWarsCore.FindElements.HeroPage.GetStartAdventureButton(doc, 65, 49);
            Assert.IsNotNull(value);
        }

        [TestMethod]
        public void TravianOfficialGetStartAdventureButton()
        {
            var doc = new HtmlDocument();
            doc.Load("FindElements/HeroPage/adventure/Travian.html");
            var value = TravianOfficialCore.FindElements.HeroPage.GetStartAdventureButton(doc, 112, -118);
            Assert.IsNotNull(value);
        }

        [TestMethod]
        public void TravianOfficialHeroGetStartAdventureButton()
        {
            var doc = new HtmlDocument();
            doc.Load("FindElements/HeroPage/adventure/TravianHeroUI.html");
            var value = TravianOfficialNewHeroUICore.FindElements.HeroPage.GetStartAdventureButton(doc, -49, -36);
            Assert.IsNotNull(value);
        }
    }
}