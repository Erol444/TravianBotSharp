using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using TbsCore.Models.AccModels;
using TbsCore.Models.Database;
using TravBotSharp.Files.Helpers;

namespace TbsCore.Database
{
    public static class DbRepository
    {
        public static List<Account> GetAccounts()
        {
            using (var context = new TbsContext())
            {
                try
                {
                    var accounts = context.DbAccount.AsQueryable().Select(x => x.Deserialize());
                    //accounts.ForEach(x => ObjectHelper.FixAccObj(x, x));
                    return accounts.ToList();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception thrown: " + e.Message);
                }
                return new List<Account>();
            }
        }

        public static void SaveAccount(Account acc)
        {
            using (var context = new TbsContext())
            {
                var saved = FindDbAccount(acc, context);

                if (saved == null)
                {
                    context.DbAccount.Add(ConvertAcc(acc));
                }
                else
                {
                    saved.JsonData = JsonConvert.SerializeObject(acc);
                }

                context.SaveChanges();
            }
        }

        public static void RemoveAccount(Account acc)
        {
            using (var context = new TbsContext())
            {
                context.DbAccount.Remove(FindDbAccount(acc, context));
                context.SaveChanges();
            }
        }

        public static void SyncAccountsTxt()
        {
            using (var context = new TbsContext())
            {
                var accs = IoHelperCore.ReadAccounts();
                var dbAccs = new List<DbAccount>(accs.Count);

                foreach (var acc in accs)
                {
                    dbAccs.Add(ConvertAcc(acc));
                }

                context.DbAccount.AddRange(dbAccs);
                context.SaveChanges();
            }
        }

        private static DbAccount FindDbAccount(Account acc, TbsContext context) =>
            context.DbAccount.FirstOrDefault(x => x.Username == acc.AccInfo.Nickname && x.Server == acc.AccInfo.ServerUrl);

        private static DbAccount ConvertAcc(Account acc) =>
            new DbAccount
            {
                Username = acc.AccInfo.Nickname,
                Server = acc.AccInfo.ServerUrl,
                JsonData = JsonConvert.SerializeObject(acc)
            };
    }

    //public class RawAcc
    //{
    //    public string Username { get; set; }
    //    public string Server { get; set; }
    //}
}
