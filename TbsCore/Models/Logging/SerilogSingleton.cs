using System.Text;
using Serilog;

namespace TbsCore.Models.Logging
{
    public class SerilogSingleton
    {
        public static void Init()
        {
            Log.Logger = new LoggerConfiguration()
              .WriteTo.Map("Username", "Other", (name, wt) => wt.File($"./logs/log-{name}-.txt",
                                                                        rollingInterval: RollingInterval.Day,
                                                                        encoding: Encoding.Unicode,
                                                                        outputTemplate: "{Timestamp:HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}"))
              .WriteTo.TbsSink()
              .CreateLogger();
        }

        public static void Close()
        {
            Log.CloseAndFlush();
        }
    }
}