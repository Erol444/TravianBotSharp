using FluentResults;
using MainCore.Helper.Interface;
using MainCore.Models.Runtime;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System;

namespace MainCore.Helper.Implementations.TTWars
{
    public class NPCHelper : Base.NPCHelper
    {
        public NPCHelper(IChromeManager chromeManager, IGeneralHelper generalHelper, IDbContextFactory<AppDbContext> contextFactory) : base(chromeManager, generalHelper, contextFactory)
        {
        }

        protected override Result CheckGold()
        {
            throw new NotImplementedException();
        }

        protected override Result EnterNumber(Resources ratio)
        {
            throw new NotImplementedException();
        }
    }
}