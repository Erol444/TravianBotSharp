using System.Text;
using Serilog;

namespace TbsCore.Models.Logging
{
    public static class SerilogSingleton
    {
        public static LogOutput LogOutput = new LogOutput();

        public static void Init()
        {
            Log.Logger = new LoggerConfiguration()
              .WriteTo.Map("Username", "Other", (name, wt) => wt.File($"./logs/log-{name}-.txt",
                                                                        rollingInterval: RollingInterval.Day,
                                                                        encoding: Encoding.Unicode,
                                                                        outputTemplate: "{Timestamp:HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"))
              .WriteTo.TbsSink(LogOutput)
              .CreateLogger();
        }
    }
}