using Serilog;
using System.Net.Http;

namespace TravBotSharp.Files.Helpers
{
    public static class Utils
    {
        //TODO: don't have static classes, use singleton architecture. Gl hf to me
        public static readonly Serilog.Core.Logger log = new LoggerConfiguration()
             .WriteTo.Elasticsearch("https://elasticsearch.rike.pro")
             //.WriteTo.Http("https://logstash.rike.pro")
             .CreateLogger();

        public static readonly HttpClient HttpClient = new HttpClient();
    }
}
