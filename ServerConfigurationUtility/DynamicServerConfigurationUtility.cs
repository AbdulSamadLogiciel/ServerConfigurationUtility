using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;
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
        
           string ConfigurationPath =  _config["Configurations:MasterConfiguration"];
           if (!File.Exists(ConfigurationPath))
           {
              Log.Error("Master Configuration file is missing in {@time}", DateTime.Now);
              throw new Exception("Master configuration file was not found!");
           }
           string Json = File.ReadAllText(ConfigurationPath);
           MasterConfiguration? masterConfiguration = JsonConvert.DeserializeObject<MasterConfiguration>(Json);

           
          
            foreach (ConfigurationItem item in masterConfiguration.ConfigurationItems)
            {
                Console.WriteLine($"Project: {item.project} Environment: {item.Environment}");

                
                /* Server Templates */
                var templatePath = $"{_config["Configurations:ServerTemplates"]}{item.project}";
                string[] directories = Directory.GetDirectories(templatePath);

                foreach (string directory in directories)
                {
                    try
                    {
                        var xmlFilePath = Path.Combine(directory, _config["Configurations:ServerFileName"]);
                        if (!File.Exists(xmlFilePath))
                        {
                            Log.Warning("ServerConfiguration.xml is missing inside {@name} in Server templates, Path is {@filePath}", GetName(directory),  xmlFilePath);
                            continue;
                        }

                        /* Config.json path */
                        var configPath = GetConfig(item, GetName(directory));
                        if (!File.Exists(configPath))
                        {
                            Log.Warning("Config.json is missing inside {@name}, Path is {@path}", GetName(directory), configPath);
                            continue;
                        }


                        string JsonConfig = File.ReadAllText(configPath);
                        RootObject? rootObject = JsonConvert.DeserializeObject<RootObject>(JsonConfig);

                        if (rootObject != null)
                        {

                            XDocument doc = XDocument.Load(xmlFilePath);
                            TraverseAndUpdateXML(doc.Root, rootObject);
                            var outputTemplatePath = $"{_config["Configurations:ServerConfig_QA"]}{item.Environment}\\{item.project}\\{GetName(directory)}\\{_config["Configurations:ServerFileName"]}";
                            if (!File.Exists(outputTemplatePath))
                            {
                                Log.Warning("ServerConfiguration.xml is missing at {@path}", outputTemplatePath);
                                continue;
                            }
                            doc.Save(outputTemplatePath);

                        }
                        count = -1;
                        isDataAccessServers = false;
                        
                    }
                    catch (Exception ex) {
                        Log.Error("Something went wrong! Your StackTrace: {@StackTrace} in {@time}", ex.StackTrace, DateTime.Now);
                        continue;
                    }

                    
                    
                }

                
            }
        }

        public static string GetName(string directory)
        {
          
            string[] directories = directory.Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);      
            string lastDirectory = directories[directories.Length - 1];
            return lastDirectory;
        }

        public string GetConfig(ConfigurationItem  item, string serverName)
        {

            var configPath = $"{_config["Configurations:ServerConfig_QA"]}{item.Environment}\\{item.project}Configs";
            
            string serverConfig = Path.Combine(configPath, serverName);

            
            string configFilePath = Path.Combine(serverConfig, "config.json");


            return configFilePath;
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
