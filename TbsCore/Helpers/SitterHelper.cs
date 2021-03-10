namespace TravBotSharp.Files.Helpers
{
    public static class SitterHelper
    {
        public static bool isSitter(HtmlAgilityPack.HtmlDocument htmlDoc, Classificator.ServerVersionEnum version)
        {
            // TODO: check T4.4
            var auction = htmlDoc.DocumentNode.SelectSingleNode("//a[contains(@class,'auction')]");
            if (auction != null && auction.HasClass("disable"))
            {
                return false;
            }
            return true;
        }
    }
}