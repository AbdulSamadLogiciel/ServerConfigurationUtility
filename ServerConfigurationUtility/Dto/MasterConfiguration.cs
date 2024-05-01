namespace ServerConfigurationUtility.Dto
{
    public class ConfigurationItem
    {
        public string Environment { get; set; }
        public string project { get; set; } 


    }

    public class MasterConfiguration
    {
        public List<ConfigurationItem> ConfigurationItems { get; set; }
    }
}
