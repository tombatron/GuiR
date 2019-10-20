﻿using GuiR.Configuration;
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

            Unloaded += Keys_Unloaded;
        }

        private void Keys_Unloaded(object sender, System.Windows.RoutedEventArgs e) => _viewModel.Dispose();
    }
}
