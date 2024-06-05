using Microsoft.Extensions.Configuration;
using Serilog;
using ServerConfigurationUtility.Logger;



namespace ServerConfigurationUtility
{
    public class Program
    {
        static void Main()
        {
            try
            {

           
                Serilogger.InitializeLogging();
                Log.Information("Utility Started");
                string currentDirectory = Directory.GetCurrentDirectory();
                string configFilePath = Path.Combine(currentDirectory, "..", "..", "..", "appsettings.json");

                var builder = new ConfigurationBuilder();
                builder.SetBasePath(Directory.GetCurrentDirectory())
                       .AddJsonFile(configFilePath, optional: false, reloadOnChange: true);

                IConfiguration config = builder.Build();

                DynamicServerConfigurationUtility serverConfig = new DynamicServerConfigurationUtility(config);
                
                serverConfig.ModifyFile();

                Log.Information("Utility Ended");
            } catch (Exception ex)
            {
                Log.Error("Something went wrong! Your StackTrace: {@StackTrace} in {@time}", ex.StackTrace, DateTime.Now);
            }
        }
       
    }
}

// port and cache file should be in configuration env are qa130, ra-dev, rq replica
