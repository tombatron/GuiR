using GuiR.Configuration;
using System.Windows.Controls;

namespace GuiR.Controls.KeyDisplay
{
    public partial class StreamKey : UserControl
    {
        public StreamKey(string key) : this(key, ServiceLocator.GetService<StreamKeyViewModel>())
        public StreamKey(string key, StreamKeyViewModel viewModel)
        {
            viewModel.Key = key;

            DataContext = viewModel;

            InitializeComponent();
        }
    }
}
