using GuiR.Models;
using System.Collections.Generic;
using System.Windows.Controls;

namespace GuiR.Controls.ServerTree
{
    public class ServerListTreeView : TreeView
    {
        public void AddServer(RedisServerInformation serverInfo) =>
            Items.Add(new ServerTreeViewItem(serverInfo));

        public IEnumerable<RedisServerInformation> GetAllServerInformation()
        {
            foreach (var item in Items)
            {
                if (item is ServerTreeViewItem)
                {
                    var treeViewItem = item as ServerTreeViewItem;

                    yield return treeViewItem.ServerInfo;
                }
            }
        }
    }
}
