using Newtonsoft.Json;
using ServerConfigurationUtility.Dto;
using System.Text;
using System.Xml;

namespace ServerConfigurationUtility
{
    public class ServerConfiguration
    {

        private readonly string RootPath;
        private readonly string FilePath;
        private readonly string Json;
        public ServerConfiguration()
        {
            RootPath = AppDomain.CurrentDomain.BaseDirectory;
            FilePath = Path.Combine(RootPath, "Config.json");
            Json = File.ReadAllText(FilePath);
        }

        private static void RemoveLastElement(StringBuilder xmlStringBuilder)
        {
            int lastLineBreakIndex = xmlStringBuilder.ToString().LastIndexOf(Environment.NewLine);

            if (lastLineBreakIndex >= 0)
            {

                xmlStringBuilder.Length = lastLineBreakIndex;
            }
        }

        private StringBuilder AdaptXMLForManipulation()
        {
            string xmlFilePath = Path.Combine(RootPath, "ServerConfiguration.xml");


            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFilePath);


            StringBuilder xmlStringBuilder = new StringBuilder();

            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "\t"
            };

            using (XmlWriter writer = XmlWriter.Create(xmlStringBuilder, settings))
            {
                doc.WriteTo(writer);
            }
            RemoveLastElement(xmlStringBuilder);
            return xmlStringBuilder;
        }

        public void GenerateServerConfiguration()
        {
            RootObject? rootObject = JsonConvert.DeserializeObject<RootObject>(this.Json) ?? throw new Exception("Configuration not present for xml generation.");
            var xmlStringBuilder = AdaptXMLForManipulation();

            _ = rootObject.ServerConfig ?? throw new Exception("Config.json is not valid");
            foreach (ServerConfig serverConfig in rootObject.ServerConfig)
            {
                xmlStringBuilder.AppendLine($"\r\n\t<!-- Update Server Config -->\r\n\t<DataAccessConfigurations>\r\n      <IsDefault>{serverConfig.IsDefault}</IsDefault>\r\n\t  <IsJWTCheckAllowed>{serverConfig.IsJwtTokenAllowed}</IsJWTCheckAllowed>\r\n\t  <OnlyDBAccessServer>false</OnlyDBAccessServer>\r\n\t  <CacheFile>LogonCache.txt</CacheFile>\r\n\t  <ServiceProviderTypeName>DataAccessServer.ServiceProvider.QueryResponseProvider`2[[System.Dynamic.ExpandoObject, System.Linq.Expressions, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[System.Dynamic.ExpandoObject, System.Linq.Expressions, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]]</ServiceProviderTypeName>\r\n\t\t<DAEndPointConfiguration>\r\n\t\t\t\t<Port>{serverConfig.Port}</Port>\r\n\t\t\t\t<HeartbeatInterval>-1</HeartbeatInterval>\r\n\t\t\t\t<ProtocolChannelType>Client.Communication.ProtocolChannel.OEChannel, Client.Communication</ProtocolChannelType>\r\n\t\t\t\t<ProtocolChannelProperty>Instance</ProtocolChannelProperty>\r\n\t\t\t\t<Name>TransactionalEndPoint</Name>\r\n\t\t</DAEndPointConfiguration>\r\n\t  <HandlerConfig>\r\n\t\t\t<HandlerTypeName>DataAccessServer.Handlers.UpdateHandler`2[[System.Dynamic.ExpandoObject, System.Linq.Expressions, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a],[System.Dynamic.ExpandoObject, System.Linq.Expressions, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a]]</HandlerTypeName>\r\n\t\t\t<HandlerProperty>Initialize</HandlerProperty>\r\n\t\t\t<RepositoryName>IgniteRepo</RepositoryName>\r\n\t  </HandlerConfig>\r\n    </DataAccessConfigurations>");
            }
            xmlStringBuilder.AppendLine("</DataAccessServerConfigurations>");

            string xmlOutputFilePath = Path.Combine(this.RootPath, "OmittedServerConfiguration.xml");
            File.WriteAllText(xmlOutputFilePath, xmlStringBuilder.ToString());

        }
    }
}
