using GuiR.Models;
using System.Windows.Controls;

namespace GuiR.Controls.ServerTree
{
    public class ServerTreeViewItem : TreeViewItem
    {
        public string ServerName => ServerInfo.ServerName;

        public string ServerAddress => ServerInfo.ServerAddress;

        public int ServerPort => ServerInfo.ServerPort;

        public RedisServerInformation ServerInfo { get; }

        public ServerTreeViewItem(RedisServerInformation serverInfo)
        {
            ServerInfo = serverInfo;

            Header = ServerName;

            Items.Add(new KeysSubTreeViewItem(this));
            Items.Add(new SlowLogSubTreeViewItem(this));
            Items.Add(new InfoSubTreeViewItem(this));
        }
    }
}
