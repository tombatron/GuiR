using GuiR.Configuration;
using GuiR.ViewModels.Keys.KeyDisplay;
using System.Windows.Controls;

namespace GuiR.Controls.KeyDisplay
{
    public partial class StringKey : UserControl
    {
        public StringKey(string key) : this(key, ServiceLocator.GetService<StringKeyViewModel>()) { }

        public StringKey(string key, StringKeyViewModel viewModel)
        {
            viewModel.Key = key;

            DataContext = viewModel;

            InitializeComponent();
        }
    }
}