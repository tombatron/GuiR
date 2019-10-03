using GuiR.Configuration;
using GuiR.ViewModels.Keys.KeyDisplay;
using System.Windows.Controls;

namespace GuiR.Controls.KeyDisplay
{
    public partial class ListKey : UserControl
    {
        public ListKey(string key) : this(key, ServiceLocator.GetService<ListKeyViewModel>())
        { }

        public ListKey(string key, ListKeyViewModel viewModel)
        {
            viewModel.Key = key;

            DataContext = viewModel;

            InitializeComponent();
        }
    }
}