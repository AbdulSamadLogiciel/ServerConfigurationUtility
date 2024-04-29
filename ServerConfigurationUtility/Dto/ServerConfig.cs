namespace ServerConfigurationUtility.Dto
{
    public class ServerConfig
    {
        public int Port { get; set; }
        public bool IsDefault { get; set; }
        public bool IsJwtTokenAllowed { get; set; }
    }

    public class RootObject
    {
        public List<ServerConfig>? ServerConfig { get; set; }
    }
}
