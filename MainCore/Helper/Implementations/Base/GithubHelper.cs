using MainCore.Helper.Interface;
using Octokit;
using System;
using System.Threading.Tasks;

namespace MainCore.Helper.Implementations.Base
{
    public sealed class GithubHelper : IGithubHelper
    {
        private const string _username = "Erol444";
        private const string _repo = "TravianBotSharp";
        private readonly GitHubClient _client = new(new ProductHeaderValue("TBS"));

        public string GetLink(string version)
        {
            if (string.IsNullOrEmpty(version))
            {
                return $"https://github.com/{_username}/{_repo}/releases/latest";
            }
            return $"https://github.com/{_username}/{_repo}/releases/tag/{version}";
        }

        public async Task<Version> GetLatestVersion()
        {
            try
            {
                var latest = await _client.Repository.Release.GetLatest(_username, _repo);
                if (latest is not null) return new Version(latest.TagName);
            }
            catch
            {
            }
            return null;
        }

        public async Task<bool> IsNewVersion(Version current)
        {
            var lastVersion = await GetLatestVersion();
            if (lastVersion is null) return false;
            return lastVersion > current;
        }
    }
}