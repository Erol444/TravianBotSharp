using Serilog;
using System;
using System.Net.Http;
using TbsCore.Helpers;

namespace TravBotSharp.Files.Helpers
{
    public static class Utils
    {
        public static TbsLoggerSink LoggerSink = new TbsLoggerSink();

#if DEBUG
        public static readonly Serilog.Core.Logger Log = new LoggerConfiguration()
                    //.WriteTo.Elasticsearch("https://elasticsearch.rike.pro")
                    .WriteTo.Sink(LoggerSink)
                    .CreateLogger();
#else
        public static readonly Serilog.Core.Logger Log = new LoggerConfiguration()
                            //.WriteTo.Elasticsearch("https://elasticsearch.rike.pro")
                            .WriteTo.Sink(TbsLogger)
                            .CreateLogger();
#endif

        public static readonly HttpClient HttpClient = new HttpClient();
    }
}
