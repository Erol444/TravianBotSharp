using MainCore.Helper.Interface;
using MainCore.Models.Database;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using RestSharp;
using System;
using System.Linq;

namespace MainCore.Helper.Implementations.Base
{
    public sealed class AccessHelper : IAccessHelper
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IRestClientManager _restClientManager;
        private readonly ILogHelper _logHelper;

        public AccessHelper(IDbContextFactory<AppDbContext> contextFactory, IRestClientManager restClientManager, ILogHelper logHelper)
        {
            _contextFactory = contextFactory;
            _restClientManager = restClientManager;
            _logHelper = logHelper;
        }

        public bool IsValid(RestClient client)
        {
            var request = new RestRequest
            {
                Method = Method.Get,
            };
            try
            {
                var response = client.Execute(request);
                return !string.IsNullOrWhiteSpace(response.Content);
            }
            catch (Exception e)
            {
                _ = e;
                return false;
            }
        }

        public bool IsLastAccess(int accountId, Access access)
        {
            using var context = _contextFactory.CreateDbContext();
            var accesses = context.Accesses.Where(x => x.AccountId == accountId).OrderBy(x => x.LastUsed).ToList();

            var lastAccess = accesses.Last();
            return access.Id == lastAccess.Id;
        }

        public Access GetNextAccess(int accountId)
        {
            using var context = _contextFactory.CreateDbContext();
            var accesses = context.Accesses.Where(x => x.AccountId == accountId).OrderBy(x => x.LastUsed).ToList();

            foreach (var access in accesses)
            {
                if (string.IsNullOrEmpty(access.ProxyHost))
                {
                    _logHelper.Information(accountId, $"Default connection is selected");
                    access.LastUsed = DateTime.Now;
                    context.Update(access);
                    context.SaveChanges();
                    return access;
                }

                var result = IsValid(_restClientManager.Get(new(access)));
                if (result)
                {
                    _logHelper.Information(accountId, $"Proxy {access.ProxyHost} is selected");
                    access.LastUsed = DateTime.Now;
                    context.Update(access);
                    context.SaveChanges();
                    return access;
                }
                else
                {
                    _logHelper.Warning(accountId, $"Proxy {access.ProxyHost} is not working");
                }
            }

            _logHelper.Warning(accountId, "All connection of this account is not working");

            return null;
        }
    }
}