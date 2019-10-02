using GuiR.Controls;
using GuiR.Controls.ServerTree;
using GuiR.Views;
using System.Windows;

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

        private void Servers_SelectedItemChanged(object newValue)
        {
            if (newValue is KeysSubTreeViewItem)
            {
                MainContent.Content = new Keys();
            }

            if (newValue is SlowLogSubTreeViewItem)
            {
                MainContent.Content = new SlowLog(((SlowLogSubTreeViewItem)newValue).ParentTreeItem.ServerInfo);
            }

            if (newValue is InfoSubTreeViewItem)
            {
                MainContent.Content = new Info(((InfoSubTreeViewItem)newValue).ParentTreeItem.ServerInfo);
            }
        }

        private void Servers_SelectedItemChanged_RemoveServerState(object newValue)
        {
            if (newValue is ServerTreeViewItem)
            {
                RemoveServer.IsEnabled = true;
            }
            else
            {
                RemoveServer.IsEnabled = false;
            }
        }

        private void AddServer_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new NewServerDialog(async (info) =>
            {
                await Servers.AddServerAsync(info);
            });

            dialog.Show();
        }

        private async void RemoveServer_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to remove the server?", "Remove server?", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                await Servers.RemoveSelectedServerAsync();

                RemoveServer.IsEnabled = false;
            }
        }
    }
}
