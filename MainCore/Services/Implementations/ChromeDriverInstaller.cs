using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using File = System.IO.File;

namespace MainCore.Services
{
    public static class ChromeDriverInstaller
    {
        private static readonly HttpClient httpClient = new();

        private const string apiEndpoint = "https://googlechromelabs.github.io/chrome-for-testing/known-good-versions-with-downloads.json";

        public static async Task Install()
        {
            CheckPlatform();
            var chromeVersion = await GetChromeVersion();

            if (chromeVersion.Major <= 114)
            {
                throw new Exception($"Your chrome version is {chromeVersion}. Please update your chrome first");
            }

            var chromeDriverVersion = await GetCurrentDriverVersion();
            if (chromeDriverVersion.Major >= chromeVersion.Major)
            {
                return;
            }
            await Download(chromeVersion);
        }

        private static async Task Download(Version chromeVersion)
        {
            var asset = await httpClient.GetFromJsonAsync<CftAsset>(apiEndpoint);
            if (asset is null)
            {
                throw new Exception("Failed to get asset from chrome API");
            }

            var orderedVersion = asset.Versions.OrderByDescending(x => x.Version);

            var version = orderedVersion.FirstOrDefault(x => x.Version.Major == chromeVersion.Major);
            if (version is null)
            {
                throw new Exception($"ChromeDriver version not found for Chrome version {chromeVersion}");
            }

            if (version.Downloads.ChromeDriver is null)
            {
                throw new Exception($"ChromeDriver version not found for Chrome version {chromeVersion} [no download link ]");
            }

            var link = version.Downloads.ChromeDriver.FirstOrDefault(x => x.Platform == "win64");
            if (link is null)
            {
                throw new Exception($"ChromeDriver version not found for Chrome version {chromeVersion} [no win64 download link]");
            }

            var driverZipResponse = await httpClient.GetAsync(link.Url);
            if (!driverZipResponse.IsSuccessStatusCode)
            {
                throw new Exception($"ChromeDriver download request failed with status code: {driverZipResponse.StatusCode}, reason phrase: {driverZipResponse.ReasonPhrase}");
            }
            var driverName = GetDriverName();
            string targetPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            targetPath = Path.Combine(targetPath, driverName);

            using (var zipFileStream = await driverZipResponse.Content.ReadAsStreamAsync())
            using (var zipArchive = new ZipArchive(zipFileStream, ZipArchiveMode.Read))
            using (var chromeDriverWriter = new FileStream(targetPath, FileMode.Create))
            {
                var entry = zipArchive.Entries.FirstOrDefault(x => x.Name.Contains(driverName));
                using Stream chromeDriverStream = entry.Open();
                await chromeDriverStream.CopyToAsync(chromeDriverWriter);
            }
        }

        private static async Task<Version> GetCurrentDriverVersion()
        {
            var driverName = GetDriverName();
            string targetPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            targetPath = Path.Combine(targetPath, driverName);
            if (!File.Exists(targetPath)) return new Version(0, 0, 0, 0);

            using var process = Process.Start(
                new ProcessStartInfo
                {
                    FileName = targetPath,
                    ArgumentList = { "--version" },
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                }
            );
            string existingChromeDriverVersion = await process.StandardOutput.ReadToEndAsync();
            string error = await process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();
            process.Kill(true);

            // expected output is something like "ChromeDriver 88.0.4324.96 (68dba2d8a0b149a1d3afac56fa74648032bcf46b-refs/branch-heads/4324@{#1784})"
            // the following line will extract the version number and leave the rest
            existingChromeDriverVersion = existingChromeDriverVersion.Split(" ")[1];

            var version = new Version(existingChromeDriverVersion);

            if (!string.IsNullOrEmpty(error))
            {
                throw new Exception($"Failed to execute {driverName} --version");
            }

            return version;
        }

        private static void CheckPlatform()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            { return; }
            throw new PlatformNotSupportedException("Your operating system is not supported.");
        }

        private static string GetDriverName()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return "chromedriver.exe";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return "chromedriver";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return "chromedriver";
            }
            return "";
        }

        private static async Task<Version> GetChromeVersion()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                string chromePath = (string)Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\App Paths\\chrome.exe", null, null);
                if (chromePath == null)
                {
                    throw new Exception("Google Chrome not found in registry");
                }

                var fileVersionInfo = FileVersionInfo.GetVersionInfo(chromePath);
                return new Version(fileVersionInfo.FileVersion);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                try
                {
                    using var process = Process.Start(
                        new ProcessStartInfo
                        {
                            FileName = "google-chrome",
                            ArgumentList = { "--product-version" },
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                        }
                    );
                    string output = await process.StandardOutput.ReadToEndAsync();
                    string error = await process.StandardError.ReadToEndAsync();
                    await process.WaitForExitAsync();
                    process.Kill(true);

                    if (!string.IsNullOrEmpty(error))
                    {
                        throw new Exception(error);
                    }

                    return new Version(output);
                }
                catch (Exception ex)
                {
                    throw new Exception("An error occurred trying to execute 'google-chrome --product-version'", ex);
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                try
                {
                    using var process = Process.Start(
                        new ProcessStartInfo
                        {
                            FileName = "/Applications/Google Chrome.app/Contents/MacOS/Google Chrome",
                            ArgumentList = { "--version" },
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                        }
                    );
                    string output = await process.StandardOutput.ReadToEndAsync();
                    string error = await process.StandardError.ReadToEndAsync();
                    await process.WaitForExitAsync();
                    process.Kill(true);

                    if (!string.IsNullOrEmpty(error))
                    {
                        throw new Exception(error);
                    }

                    output = output.Replace("Google Chrome ", "");
                    return new Version(output);
                }
                catch (Exception ex)
                {
                    throw new Exception($"An error occurred trying to execute '/Applications/Google Chrome.app/Contents/MacOS/Google Chrome --version'", ex);
                }
            }
            else
            {
                throw new PlatformNotSupportedException("Your operating system is not supported.");
            }
        }
    }

    public class CftAsset
    {
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonPropertyName("versions")]
        public List<CftVersion> Versions { get; set; }
    }

    public class CftVersion
    {
        [JsonPropertyName("version")]
        public Version Version { get; set; }

        [JsonPropertyName("revision")]
        public int Revision { get; set; }

        [JsonPropertyName("downloads")]
        public CftDownloads Downloads { get; set; }
    }

    public class CftDownloads
    {
        [JsonPropertyName("chrome")]
        public List<CftLink> Chrome { get; set; }

        [JsonPropertyName("chromedriver")]
        public List<CftLink> ChromeDriver { get; set; }
    }

    public class CftLink
    {
        [JsonPropertyName("platform")]
        public string Platform { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }
    }
}