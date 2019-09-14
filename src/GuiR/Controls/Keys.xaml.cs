using GuiR.Configuration;
using GuiR.Controls.KeyDisplay;
using GuiR.Models;
using GuiR.ViewModels.Keys;
using System.Windows.Controls;

namespace GuiR.Controls
{
    public partial class Keys : UserControl
    {
        private readonly RedisServerInformation _serverInfo;
        private readonly KeysControlViewModel _viewModel;

        public Keys(RedisServerInformation serverInfo)
        {
            _serverInfo = serverInfo;

            _viewModel = ServiceLocator.GetService<KeysControlViewModel>();
            _viewModel.ServerInfo = _serverInfo;

            InitializeComponent();

            DataContext = _viewModel;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var keySelector = (ListBox)sender;

            if (keySelector.SelectedItem == null)
            {
                KeyContent.Content = null;
            }
            else
            {
                var selectedKey = keySelector.SelectedItem.ToString();

                KeyContent.Content = new StringKey(_serverInfo, _viewModel.DatabaseId, selectedKey);
            }
        }
    }
}
