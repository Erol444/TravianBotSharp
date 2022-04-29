using System.Collections.Generic;
using TbsCore.Models.ResourceModels;

namespace TbsCore.TravianData
{
    /// <summary>
    /// Class for dealing with localization. TODO: add new languages, save language into the acc/access model
    /// </summary>
    public static class Localizations
    {
        private static Dictionary<Language, List<string>> merchants = new Dictionary<Language, List<string>>()
        {
             { Language.English, new List<string> { "returning merchants:", "incoming merchants:", "ongoing merchants:" } }
        };

        public static TransitType MercahntDirectionFromString(string str)
        {
            var strs = merchants[Language.English];
            var index = strs.IndexOf(str.Trim().ToLower());
            return (TransitType)index;
        }

        public enum Language
        {
            English
        }
    }
}