using HtmlAgilityPack;
using System.Linq;

namespace TTWarsCore.FindElements
{
    public static class InstantComplete
    {
        public static HtmlNode GetFinishButton(HtmlDocument doc)
        {
            var finishClass = doc.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("finishNow"));
            if (finishClass is null) return null;
            return finishClass.Descendants("button").FirstOrDefault();
        }

        public static HtmlNode GetConfirmButton(HtmlDocument doc)
        {
            var dialog = doc.GetElementbyId("finishNowDialog");
            if (dialog is null) return null;
            return dialog.Descendants("button").FirstOrDefault();
        }
    }
}