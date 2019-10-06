using GuiR.Controls.KeyDisplay.SortedSet;
using System.Windows.Controls;

namespace GuiR.Controls.KeyDisplay
{
    public partial class SortedSetKey : UserControl
    {
        private readonly string _key;

        public SortedSetKey(string key)
        {
            _key = key;

            InitializeComponent();
        }

        private void SortedSet_Loaded(object sender, System.Windows.RoutedEventArgs e) => LoadSortedSet();

        private void CheckBox_Toggled(object sender, System.Windows.RoutedEventArgs e) => LoadSortedSet();

        private void LoadSortedSet()
        {
            if (IsGeoData.IsChecked ?? false)
            {
                SortedSetContent.Content = new Geo(_key);
            }
            else
            {
                SortedSetContent.Content = new Default(_key);
            }
        }
    }
}
