using Newtonsoft.Json;
using ServerConfigurationUtility.Dto;
using System.Xml.Linq;

namespace ServerConfigurationUtility
{



    public class ServerUtility
    {
       
        static int count = -1;
        static bool isDataAccessServers = false;
        static string RootPath = AppDomain.CurrentDomain.BaseDirectory;
        static string ConfigurationPath = Path.Combine(RootPath, "Config.json");
        static string Json = File.ReadAllText(ConfigurationPath);
        static RootObject? rootObject = JsonConvert.DeserializeObject<RootObject>(Json);

        public static void TraverseAndUpdateXML(XElement element)
        {

            if(element.Name.LocalName == "DataAccessConfigurations")
            {
                count++;
                isDataAccessServers = true;
                
            }
           
            
            if (isDataAccessServers && rootObject.ServerConfig[count].ContainsKey(element.Name.LocalName))
            {
                
                element.Value = rootObject.ServerConfig[count][element.Name.ToString()];
            }
            
            
            foreach (XElement child in element.Elements())
            {
                TraverseAndUpdateXML(child);
            }
        }

    }
}
