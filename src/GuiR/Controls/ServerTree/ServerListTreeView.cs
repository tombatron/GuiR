using GuiR.Models;
using System.Windows.Controls;

namespace GuiR.Controls.ServerTree
{
    public class ServerListTreeView : TreeView
    {
        // TODO: Add event handler that will load a saved configuration here 
        //       when the control is loaded.

        public void AddServer(RedisServerInformation serverInfo) =>
            Items.Add(new ServerTreeViewItem(serverInfo));
    }
}
