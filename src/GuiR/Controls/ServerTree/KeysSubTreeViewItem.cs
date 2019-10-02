using System.Windows.Controls;

namespace GuiR.Controls.ServerTree
{
    public class KeysSubTreeViewItem : TreeViewItem, IServerTreeViewItem
    {
        public ServerTreeViewItem ParentTreeItem { get; }

        public KeysSubTreeViewItem(ServerTreeViewItem parent)
        {
            ParentTreeItem = parent;

            Header = "KEYS";
        }
    }
}
