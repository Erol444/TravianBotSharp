using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MainCore.Services.Implementations
{
    public static class ChromeDriverInstaller
    {
        public static string InstallChromeDriver()
        {
            var p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = Path.Combine("selenium-manager", "windows", "selenium-manager.exe");
            p.StartInfo.Arguments = "--driver chromedriver --output JSON ";
            p.Start();

            string output = p.StandardOutput.ReadToEnd();

            p.WaitForExit();

            var obj = JsonSerializer.Deserialize<SeleniumManagerObject>(output);

            return obj.Result.Message;
        }
    }

    public class SeleniumManagerLog
    {
        [JsonPropertyName("level")]
        public string Level { get; set; }

        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }
    }

    public class SeleniumManagerResult
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }
    }

    public class SeleniumManagerObject
    {
        [JsonPropertyName("logs")]
        public List<SeleniumManagerLog> Logs { get; set; }

        [JsonPropertyName("result")]
        public SeleniumManagerResult Result { get; set; }
    }
}