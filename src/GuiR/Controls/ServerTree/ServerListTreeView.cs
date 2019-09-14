using GuiR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GuiR.Controls.ServerTree
{
    public class ServerListTreeView : TreeView
    {
        public void AddServer(RedisServerInformation serverInfo) =>
            Items.Add(new ServerTreeViewItem(serverInfo));
    }
}
