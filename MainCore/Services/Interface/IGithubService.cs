using System;
using System.Threading.Tasks;

namespace MainCore.Services.Interface
{
    public interface IGithubService
    {
        public string GetLink(string version);

        public Task<Version> GetLatestVersion();

        public Task<bool> IsNewVersion(Version current);
    }
}