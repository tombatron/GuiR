using GuiR.Configuration;
using GuiR.ViewModels.Keys.KeyDisplay;
using System.Windows.Controls;

namespace GuiR.Controls.KeyDisplay
{
    public partial class ListKey : UserControl
    {
        public ListKey(string key)
        {
            var viewModel = ServiceLocator.GetService<ListKeyViewModel>();
            viewModel.Key = key;

            DataContext = viewModel;

            InitializeComponent();
        }
    }
}