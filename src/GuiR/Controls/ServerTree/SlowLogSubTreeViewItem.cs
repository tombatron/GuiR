using System.Windows.Controls;

namespace GuiR.Controls.ServerTree
{
    public class SlowLogSubTreeViewItem : TreeViewItem
    {
        public ServerTreeViewItem ParentTreeItem { get; }

        public SlowLogSubTreeViewItem(ServerTreeViewItem parent)
        {
            ParentTreeItem = parent;

            Header = "SLOW LOG";
        }
    }
}
