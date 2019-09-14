using GuiR.Configuration;
using GuiR.Models;
using GuiR.ViewModels.Info;
using System.Windows.Controls;

namespace GuiR.Controls
{
    public partial class Info : UserControl
    {
        private readonly RedisServerInformation _serverInfo;
        private readonly InfoControlViewModel _viewModel;

        public Info(RedisServerInformation serverInfo)
        {
            _viewModel = ServiceLocator.GetService<InfoControlViewModel>();
            _serverInfo = serverInfo;

            InitializeComponent();

            DataContext = _viewModel;
        }

        private async void UserControl_Initialized(object sender, System.EventArgs e) =>
            await _viewModel.LoadAsync(_serverInfo);
    }
}
