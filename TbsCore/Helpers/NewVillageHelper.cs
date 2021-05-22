using TbsCore.Models.AccModels;

namespace TbsCore.Helpers
{
    public static class NewVillageHelper
    {
        public static string GenerateName(Account acc)
        {
            var villNum = acc.Villages.Count.ToString();
            if (villNum.Length == 1) villNum = "0" + villNum;

            return acc.NewVillages.NameTemplate.Replace("#NUM#", villNum);
        }
    }
}