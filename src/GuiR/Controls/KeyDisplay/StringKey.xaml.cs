using GuiR.Configuration;
using GuiR.Models;
using GuiR.ViewModels.Keys.KeyDisplay;
using System.Windows.Controls;

namespace GuiR.Controls.KeyDisplay
{
    public partial class StringKey : UserControl
    {
        public StringKey(RedisServerInformation serverInfo, int databaseId, string key)
        {
            var viewModel = ServiceLocator.GetService<StringKeyViewModel>();
            viewModel.ServerInfo = serverInfo;
            viewModel.DatabaseId = databaseId;
            viewModel.Key = key;

            DataContext = viewModel;

            InitializeComponent();
        }

        private async void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            // TODO: Change this to a command that is invoke on load. 
            await ((StringKeyViewModel)DataContext).LoadValue();
        }
    }
}
