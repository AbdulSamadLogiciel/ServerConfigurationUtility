using System.Collections.Generic;

namespace ServerConfigurationUtility.Dto
{

    public class RootObject
    {
        public List<Dictionary<string, string>>? ServerConfig { get; set; }
    }
}
