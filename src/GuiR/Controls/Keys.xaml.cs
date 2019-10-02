using GuiR.Configuration;
using GuiR.ViewModels.Keys;
using System.Windows.Controls;

namespace GuiR.Controls
{
    public partial class Keys : UserControl
    {
        public Keys()
        {
            InitializeComponent();

            DataContext = ServiceLocator.GetService<KeysControlViewModel>();
        }
    }
}
