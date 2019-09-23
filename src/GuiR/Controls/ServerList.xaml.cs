using GuiR.Controls.ServerTree;
using GuiR.Models;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GuiR.Controls
{
    public delegate void SelectedItemChanged(object newValue);

    public partial class ServerList : UserControl
    {
        private ServerTreeViewItem _selectedServer;

        public event SelectedItemChanged SelectedItemChanged;

        public ServerList()
        {
            InitializeComponent();

            Servers.SelectedItemChanged += Servers_SelectedItemChanged;
        }

        public void AddServer(RedisServerInformation serverInfo) =>
            Servers.AddServer(serverInfo);

        public Task RemoveSelectedServerAsync()
        {
            Servers.Items.Remove(_selectedServer);

            _selectedServer = null;

            if (Servers.Items.Count > 0)
            {
                foreach (var treeItem in Servers.Items)
                {
                    if (treeItem is TreeView)
                    {
                        ((TreeViewItem)treeItem).IsSelected = false;
                    }
                }
            }

            return Task.CompletedTask;
        }

        private void Servers_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is ServerTreeViewItem)
            {
                _selectedServer = e.NewValue as ServerTreeViewItem;
            }
            else
            {
                _selectedServer = null;
            }

            SelectedItemChanged(e.NewValue);
        }
    }
}
