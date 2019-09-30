using GuiR.Configuration;
using GuiR.Models;
using GuiR.ViewModels.Keys.KeyDisplay;
using System.Windows.Controls;

namespace GuiR.Controls.KeyDisplay
{
    public partial class HashKey : UserControl
    {
        public HashKey(RedisServerInformation serverInfo, int databaseId, string key)
        {
            var viewModel = ServiceLocator.GetService<HashKeyViewModel>();
            viewModel.ServerInfo = serverInfo;
            viewModel.DatabaseId = databaseId;
            viewModel.Key = key;

            DataContext = viewModel;

            InitializeComponent();
        }
    }
}