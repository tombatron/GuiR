using GuiR.Configuration;
using GuiR.ViewModels.Keys.KeyDisplay;
using System.Windows.Controls;

namespace GuiR.Controls.KeyDisplay
{
    public partial class SortedSetKey : UserControl
    {
        public SortedSetKey(string key)
        {
            var viewModel = ServiceLocator.GetService<SortedSetKeyViewModel>();
            viewModel.Key = key;

            DataContext = viewModel;

            InitializeComponent();
        }
    }
}
