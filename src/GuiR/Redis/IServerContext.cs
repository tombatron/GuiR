using GuiR.Models;

namespace GuiR.Redis
{
    interface IServerContext
    {
        RedisServerInformation ServerInfo { get; set; }

        int DatabaseId { get; set; }

        string SelectedKey { get; set; }
    }
}