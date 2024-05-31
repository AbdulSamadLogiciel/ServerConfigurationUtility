using Serilog.Events;
using Serilog.Formatting.Json;
using Serilog;

namespace ServerConfigurationUtility.Logger
{
    public class Serilogger
    {
        public static void InitializeLogging()
        {
            Serilog.Log.Logger = new LoggerConfiguration()
                           .WriteTo.Console()
                           .WriteTo.File(new JsonFormatter(),
                                         "important.json",
                                         restrictedToMinimumLevel: LogEventLevel.Warning)
                           .WriteTo.File("all-.logs",
                                         rollingInterval: RollingInterval.Day)
                           .MinimumLevel.Debug()
                           .CreateLogger();
        }
    }
}
