using GuiR.Configuration;
using GuiR.Controls.ServerTree;
using GuiR.Models;
using GuiR.Settings;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GuiR.Controls
{
    public delegate void SelectedItemChanged(object newValue);

    public partial class ServerList : UserControl
    {
        private readonly ISettingsProvider _settingsProvider;

        private ServerTreeViewItem _selectedServer;

        public event SelectedItemChanged SelectedItemChanged;

        public ServerList()
        {
            InitializeComponent();

            _settingsProvider = ServiceLocator.GetService<ISettingsProvider>();

            Servers.SelectedItemChanged += Servers_SelectedItemChanged;
        }

        public async Task AddServerAsync(RedisServerInformation serverInfo)
        {
            Servers.AddServer(serverInfo);

            await _settingsProvider.SaveServerSettingsAsync(Servers.GetAllServerInformation());
        }
            

        public async Task RemoveSelectedServerAsync()
        {
            Servers.Items.Remove(_selectedServer);

            await _settingsProvider.SaveServerSettingsAsync(Servers.GetAllServerInformation());

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
