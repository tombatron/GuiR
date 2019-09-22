using GuiR.Controls;
using GuiR.Controls.ServerTree;
using GuiR.Views;
using System.Windows;
using System.Windows.Controls;

namespace GuiR
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Servers.SelectedItemChanged += Servers_SelectedItemChanged;
            Servers.SelectedItemChanged += Servers_SelectedItemChanged_RemoveServerState;
        }

        private void Servers_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is KeysSubTreeViewItem)
            {
                MainContent.Content = new Keys(((KeysSubTreeViewItem)e.NewValue).ParentTreeItem.ServerInfo);
            }

            if (e.NewValue is SlowLogSubTreeViewItem)
            {
                MainContent.Content = new SlowLog(((SlowLogSubTreeViewItem)e.NewValue).ParentTreeItem.ServerInfo);
            }

            if (e.NewValue is InfoSubTreeViewItem)
            {
                MainContent.Content = new Info(((InfoSubTreeViewItem)e.NewValue).ParentTreeItem.ServerInfo);
            }
        }

        private void AddServer_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Handling here for saving the server settings to the configuration file. 
            var dialog = new NewServerDialog((info) =>
            {
                Servers.AddServer(info);
            });

            dialog.Show();
        }

        private void RemoveServer_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to remove the server?", "Remove server?", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                Servers.Items.Remove(_selectedServer);

                _selectedServer = null;

                RemoveServer.IsEnabled = false;

                if (Servers.Items.Count > 0)
                {
                    foreach (var treeItem in Servers.Items)
                    {
                        if (treeItem is TreeViewItem)
                        {
                            ((TreeViewItem)treeItem).IsSelected = false;
                        }
                    }
                }
            }
        }

        private ServerTreeViewItem _selectedServer;

        private void Servers_SelectedItemChanged_RemoveServerState(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is ServerTreeViewItem)
            {
                _selectedServer = e.NewValue as ServerTreeViewItem;

                RemoveServer.IsEnabled = true;
            }
            else
            {
                _selectedServer = null;

                RemoveServer.IsEnabled = false;
            }
        }
    }
}
