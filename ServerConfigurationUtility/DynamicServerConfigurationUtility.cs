using Newtonsoft.Json;
using ServerConfigurationUtility.Dto;
using System.Xml.Linq;

namespace ServerConfigurationUtility
{
    public class DynamicServerConfigurationUtility
    {
        static readonly string RootPath = AppDomain.CurrentDomain.BaseDirectory;
        static readonly string ConfigurationPath = Path.Combine(RootPath, "MasterConfiguration.json");
        static readonly string Json = File.ReadAllText(ConfigurationPath);
        static readonly MasterConfiguration? masterConfiguration = JsonConvert.DeserializeObject<MasterConfiguration>(Json);


        static int count = -1;
        static bool isDataAccessServers = false;
        
        public static void ModifyFile()
        {
            _ = masterConfiguration ?? throw new Exception("Master configuration is invalid.");
            foreach (ConfigurationItem item in masterConfiguration.ConfigurationItems)
            {
                Console.WriteLine($"Project: {item.project} Environment: {item.Environment}");

                
                /* Server Templates */
                var templatePath = $"..\\..\\..\\ServerConfigs_QA\\ServerTemplates\\{item.project}";
                string[] directories = Directory.GetDirectories(templatePath);

                foreach (string directory in directories)
                {


                    /* Server Configuration Template */
                    var xmlFilePath = Path.Combine(directory, "ServerConfiguration.xml");
                    

                    /* Config.json path */
                    var configPath = GetConfig(item.project, GetName(directory));
                    Console.WriteLine(configPath);


                    string Json = File.ReadAllText(configPath);
                    RootObject? rootObject = JsonConvert.DeserializeObject<RootObject>(Json);

                    if (rootObject != null)
                    {

                        XDocument doc = XDocument.Load(xmlFilePath);
                        TraverseAndUpdateXML(doc.Root, rootObject);
                        var outputTemplatePath = $"..\\..\\..\\ServerConfigs_QA\\{item.Environment}\\{item.project}\\{GetName(directory)}\\ServerConfiguration.xml";
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

        public static string GetConfig(string project, string serverName)
        {

            var configPath = $"..\\..\\..\\ServerConfigs_QA\\ServerConfigs\\{project}";
            
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


        public static void TraverseAndUpdateXML(XElement element, RootObject? rootObject)
        {


            if (element.Name.LocalName == "DataAccessQueryServerConfiguration" || element.Name.LocalName == "DataAccessConfigurations")
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
