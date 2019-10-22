using GuiR.Configuration;
using GuiR.ViewModels.Keys;
using System.Windows.Controls;

namespace GuiR.Controls
{
    public partial class Keys : UserControl
    {
        private readonly KeysControlViewModel _viewModel;

        public Keys()
        {
            _viewModel = ServiceLocator.GetService<KeysControlViewModel>();

            InitializeComponent();

            DataContext = _viewModel;

            Database.SelectionChanged += Database_SelectionChanged;

            Unloaded += Keys_Unloaded;
        }

        private void Database_SelectionChanged(object sender, SelectionChangedEventArgs e) => KeyList.SelectedIndex = -1;

        private void Keys_Unloaded(object sender, System.Windows.RoutedEventArgs e) => _viewModel.Dispose();
    }
}
