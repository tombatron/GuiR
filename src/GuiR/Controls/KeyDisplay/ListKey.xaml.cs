using GuiR.Configuration;
using GuiR.Models;
using GuiR.ViewModels.Keys.KeyDisplay;
using System.Windows.Controls;

namespace GuiR.Controls.KeyDisplay
{
    public partial class ListKey : UserControl
    {
        public ListKey(RedisServerInformation serverInfo, int databaseId, string key)
        {
            var viewModel = ServiceLocator.GetService<ListKeyViewModel>();
            viewModel.ServerInfo = serverInfo;
            viewModel.DatabaseId = databaseId;
            viewModel.Key = key;

            InitializeComponent();
        }
    }
}
