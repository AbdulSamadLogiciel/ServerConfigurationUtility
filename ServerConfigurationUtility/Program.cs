using Microsoft.Extensions.Configuration;


namespace ServerConfigurationUtility
{
    public class Program
    {
        static void Main()
        {
    
            string currentDirectory = Directory.GetCurrentDirectory();
            string configFilePath = Path.Combine(currentDirectory, "..", "..", "..", "appsettings.json");
      
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile(configFilePath, optional: false, reloadOnChange: true);

            IConfiguration config = builder.Build();

            DynamicServerConfigurationUtility serverConfig = new DynamicServerConfigurationUtility(config);
            serverConfig.ModifyFile();




        }
       
    }
}
