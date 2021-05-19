using System;
using System.Threading.Tasks;
using Octokit;

namespace TravBotSharp.Files.Helpers
{
    public class GithubHelper
    {
        // public static string username = "vinaghost";
        // public static string repo = "TravianBotSharpVinaVersion";

        public static string username = "Erol444";
        public static string repo = "TravianBotSharp";

        public static async Task<Version> CheckGitHubLatestVersion()
        {
            GitHubClient client = new GitHubClient(new ProductHeaderValue("TBS"));
            try
            {
                var latest = await client.Repository.Release.GetLatest(username, repo);
                if (latest != null) return new Version(latest.TagName);
            }
            catch (Octokit.NotFoundException)
            {
            }

            return null;
        }

        public static async Task<Version> CheckGitHublatestBuild()
        {
            GitHubClient client = new GitHubClient(new ProductHeaderValue("TBS"));
            try
            {
                var releases = await client.Repository.Release.GetAll(username, repo);
                if (releases.Count > 0) return new Version(releases[0].TagName);
            }
            catch (Octokit.NotFoundException)
            {
            }

            return null;
        }
    }
}