using GuiR.Models;

namespace GuiR.Redis
{
    class DefaultServerContext : IServerContext
    {
        public RedisServerInformation ServerInfo { get; set; }
        public int DatabaseId { get; set; }
        public string SelectedKey { get; set; }
    }
}