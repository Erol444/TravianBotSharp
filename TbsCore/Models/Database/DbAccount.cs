using Newtonsoft.Json;
using System;
using TbsCore.Models.AccModels;

namespace TbsCore.Models.Database
{
    public class DbAccount : AccRawDTO
    {
        public Guid Id { get; set; }
        public string JsonData { get; set; }

        public Account Deserialize()
        {
            return JsonConvert.DeserializeObject<Account>(this.JsonData);
        }
    }
}
