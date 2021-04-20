using System.Collections.Generic;
using TravBotSharp.Files.Models.ResourceModels;

namespace TravBotSharp.Files.TravianData
{
    /// <summary>
    ///     Class for dealing with localization. TODO: add new languages, save language into the acc/access model
    /// </summary>
    public static class Localizations
    {
        public enum Language
        {
            English
        }

        private static readonly Dictionary<Language, List<string>> merchants = new Dictionary<Language, List<string>>
        {
            {Language.English, new List<string> {"returning merchants:", "incoming merchants:", "ongoing merchants:"}}
        };

        public static TransitType MercahntDirectionFromString(string str)
        {
            var strs = merchants[Language.English];
            var index = strs.IndexOf(str.Trim().ToLower());
            return (TransitType) index;
        }
    }
}