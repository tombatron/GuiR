using GuiR.Configuration;
using GuiR.ViewModels.Keys.KeyDisplay.SortedSet;
using System.Windows.Controls;

namespace GuiR.Controls.KeyDisplay.SortedSet
{
    public partial class Default : UserControl
    {
        public Default(string key)
        {
            var viewModel = ServiceLocator.GetService<DefaultViewModel>();
            viewModel.Key = key;

            DataContext = viewModel;

            InitializeComponent();
        }
    }
}
