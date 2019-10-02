using System.Windows.Controls;

namespace GuiR.Controls.ServerTree
{
    public class InfoSubTreeViewItem : TreeViewItem, IServerTreeViewItem
    {
        public ServerTreeViewItem ParentTreeItem { get; }

        public InfoSubTreeViewItem(ServerTreeViewItem parent)
        {
            ParentTreeItem = parent;

            Header = "INFO";
        }
    }
}