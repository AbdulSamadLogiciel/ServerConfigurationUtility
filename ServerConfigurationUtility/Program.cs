using Newtonsoft.Json;
using ServerConfigurationUtility.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ServerConfigurationUtility
{
    public class Program
    {
        static void Main()
        {
            string RootPath = AppDomain.CurrentDomain.BaseDirectory;
            string xmlFilePath = Path.Combine(RootPath, "ServerConfiguration.xml");
            XDocument doc = XDocument.Load(xmlFilePath);

            ServerUtility.TraverseAndUpdateXML(doc.Root);
         
           
            doc.Save(xmlFilePath);

            Console.WriteLine("XML traversal and element value update completed. Updated XML saved.");
        }
    }
}
