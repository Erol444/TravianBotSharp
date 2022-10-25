using Octokit;
using System;
using System.Threading.Tasks;

namespace MainCore.Helper
{
    public class GithubHelper
    {
        // public static string username = "vinaghost";
        // public static string repo = "TravianBotSharpVinaVersion";

        private const string _username = "Erol444";
        private const string _repo = "TravianBotSharp";
        private static readonly GitHubClient _client = new(new ProductHeaderValue("TBS"));

        public static string GetLink(string version)
        {
            return $"https://github.com/{_username}/{_repo}/releases/tag/{version}";
        }

        public static async Task<Version> CheckGitHubLatestVersion()
        {
            try
            {
                var latest = await _client.Repository.Release.GetLatest(_username, _repo);
                if (latest != null) return new Version(latest.TagName);
            }
            catch
            {
            }
            return null;
        }
    }
}