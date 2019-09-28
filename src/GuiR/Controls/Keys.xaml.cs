using GuiR.Configuration;
using GuiR.Models;
using GuiR.ViewModels.Keys;
using System.Windows.Controls;

namespace GuiR.Controls
{
    public partial class Keys : UserControl
    {
        public Keys(RedisServerInformation serverInfo)
        {
            var viewModel = ServiceLocator.GetService<KeysControlViewModel>();
            viewModel.ServerInfo = serverInfo;

            InitializeComponent();

            DataContext = viewModel;
        }
    }
}
