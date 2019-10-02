using GuiR.Models;

namespace GuiR.Redis
{
    public interface IServerContext
    {
        RedisServerInformation ServerInfo { get; set; }

        int DatabaseId { get; set; }

        string SelectedKey { get; set; }
    }
}