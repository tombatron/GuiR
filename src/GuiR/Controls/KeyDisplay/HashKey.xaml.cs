using GuiR.Configuration;
using GuiR.ViewModels.Keys.KeyDisplay;
using System.Windows.Controls;

namespace GuiR.Controls.KeyDisplay
{
    public partial class HashKey : UserControl
    {
        public HashKey(string key)
        {
            var viewModel = ServiceLocator.GetService<HashKeyViewModel>();
            viewModel.Key = key;

            DataContext = viewModel;

            InitializeComponent();
        }
    }
}