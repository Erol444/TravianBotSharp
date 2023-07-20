using System;
using System.Threading.Tasks;

namespace MainCore.Helper.Interface
{
    public interface IGithubHelper
    {
        Task<Version> GetLatestVersion();

        string GetLink(string version);
    }
}