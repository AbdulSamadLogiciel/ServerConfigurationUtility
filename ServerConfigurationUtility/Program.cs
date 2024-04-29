using Newtonsoft.Json;
using ServerConfigurationUtility;
using System.Text;
using System.Xml;


class Program
{
    public static void Main()
    {
       
        ServerConfiguration config = new();
        config.GenerateServerConfiguration();

    }
}
