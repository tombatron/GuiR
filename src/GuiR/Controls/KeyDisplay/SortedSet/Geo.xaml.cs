using GuiR.Configuration;
using GuiR.ViewModels.Keys.KeyDisplay.SortedSet;
using System.Windows.Controls;

namespace GuiR.Controls.KeyDisplay.SortedSet
{
    public partial class Geo : UserControl
    {
        public Geo(string key)
        {
            var viewModel = ServiceLocator.GetService<GeoViewModel>();
            viewModel.Key = key;

            DataContext = viewModel;

            InitializeComponent();
        }
    }
}
