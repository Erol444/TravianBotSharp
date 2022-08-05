using MainCore.Services;

#if TRAVIAN_OFFICIAL

using TravianOfficialCore.Parsers;

#elif TRAVIAN_OFFICIAL_HEROUI

using TravianOfficialNewHeroUICore.Parsers;

#elif TTWARS

using TTWarsCore.Parsers;

#endif

namespace MainCore.Helper
{
    public static class CheckHelper
    {
        public static bool IsCorrectVillage(AppDbContext context, IChromeBrowser chromeBrowser, int villageId)
        {
            var html = chromeBrowser.GetHtml();

            var listNode = VillagesTable.GetVillageNodes(html);
            foreach (var node in listNode)
            {
                var id = VillagesTable.GetId(node);
                if (id != villageId) continue;
                return VillagesTable.IsActive(node);
            }
            return false;
        }
    }
}