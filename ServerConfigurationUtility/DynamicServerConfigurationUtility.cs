using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ServerConfigurationUtility.Dto;
using System.Xml.Linq;

namespace ServerConfigurationUtility
{
    public class DynamicServerConfigurationUtility
    {
       


        static int count = -1;
        static bool isDataAccessServers = false;
        private readonly IConfiguration _config;

        public DynamicServerConfigurationUtility(IConfiguration config) { 
        
            _config = config;
        }
        
        public void ModifyFile()
        {
           string RootPath = AppDomain.CurrentDomain.BaseDirectory;
           string ConfigurationPath = Path.Combine(RootPath, _config["Configurations:MasterConfiguration"]);
           string Json = File.ReadAllText(ConfigurationPath);
           MasterConfiguration? masterConfiguration = JsonConvert.DeserializeObject<MasterConfiguration>(Json);

            _ = masterConfiguration ?? throw new Exception("Master configuration is invalid.");
            foreach (ConfigurationItem item in masterConfiguration.ConfigurationItems)
            {
                Console.WriteLine($"Project: {item.project} Environment: {item.Environment}");

                
                /* Server Templates */
                var templatePath = $"{_config["Configurations:ServerTemplates"]}{item.project}";
                string[] directories = Directory.GetDirectories(templatePath);

                foreach (string directory in directories)
                {


                    /* Server Configuration Template */
                    var xmlFilePath = Path.Combine(directory, _config["Configurations:ServerFileName"]);
                    

                    /* Config.json path */
                    var configPath = GetConfig(item.project, GetName(directory));
                    Console.WriteLine(configPath);


                    string JsonConfig = File.ReadAllText(configPath);
                    RootObject? rootObject = JsonConvert.DeserializeObject<RootObject>(JsonConfig);

                    if (rootObject != null)
                    {

                        XDocument doc = XDocument.Load(xmlFilePath);
                        TraverseAndUpdateXML(doc.Root, rootObject);
                        var outputTemplatePath = $"{_config["Configurations:ServerConfig_QA"]}{item.Environment}\\{item.project}\\{GetName(directory)}\\{_config["Configurations:ServerFileName"]}";
                        doc.Save(outputTemplatePath);
                      
                    }
                    count = -1;
                    isDataAccessServers = false;
                }

                
            }
        }

        public static string GetName(string directory)
        {
          
            string[] directories = directory.Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);      
            string lastDirectory = directories[directories.Length - 1];
            return lastDirectory;
        }

        public  string GetConfig(string project, string serverName)
        {

            var configPath = $"{_config["Configurations:ServerConfigs"]}{project}";
            
            string serverConfig = Path.Combine(configPath, serverName);

            
            string configFilePath = Path.Combine(serverConfig, "config.json");

            
            if (File.Exists(configFilePath))
            {
                return configFilePath;
            }
            else
            {
                throw new FileNotFoundException($"The config.json file was not found in the '{serverConfig}' directory.");
            }
        }


        public  void TraverseAndUpdateXML(XElement element, RootObject? rootObject)
        {


            if (element.Name.LocalName == _config["Configurations:DataAccessQueryServerConfiguration"] || element.Name.LocalName == _config["Configurations:DataAccessConfigurations"])
            {
                count++;
                isDataAccessServers = true;
            }


            if (isDataAccessServers && count <= rootObject.ServerConfig.Count() - 1  && rootObject.ServerConfig[count].ContainsKey(element.Name.LocalName))
            {

                element.Value = rootObject.ServerConfig[count][element.Name.ToString()];
            }


            foreach (XElement child in element.Elements())
            {
                TraverseAndUpdateXML(child, rootObject);
            }
        }

    }
}
