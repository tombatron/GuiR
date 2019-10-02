using StackExchange.Redis;

namespace GuiR.Models
{
    public class RedisServerInformation
    {
        public string ServerName { get; set; }

        public string ServerAddress { get; set; }

        public int ServerPort { get; set; }

        public static implicit operator ConfigurationOptions(RedisServerInformation serverInfo)
        {
            var configOptions = ConfigurationOptions.Parse($"{serverInfo.ServerName}:{serverInfo.ServerPort}");

            configOptions.ClientName = "GuiR";
            configOptions.AllowAdmin = true;

            return configOptions;
        }
    }
}
