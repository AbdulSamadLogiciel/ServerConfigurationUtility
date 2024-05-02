using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerConfigurationUtility.Dto
{
    public class Configurations
    {
        public string MasterConfiguration { get; set; }
        public string ServerTemplates { get; set; }
        public string ServerFileName { get; set; }
        public string ServerConfigs { get; set; }
        public string DataAccessQueryServerConfiguration { get; set; }
        public string DataAccessConfigurations { get; set; }
    }
}

