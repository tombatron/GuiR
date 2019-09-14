using GuiR.Models;
using System.Windows.Controls;

namespace GuiR.Controls.ServerTree
{
    public class ServerListTreeView : TreeView
    {
        public void AddServer(RedisServerInformation serverInfo) =>
            Items.Add(new ServerTreeViewItem(serverInfo));
    }
}
