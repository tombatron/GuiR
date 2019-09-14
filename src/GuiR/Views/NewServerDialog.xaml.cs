using GuiR.Models;
using GuiR.ViewModels;
using System;
using System.Windows;

namespace GuiR.Views
{
    public partial class NewServerDialog : Window
    {
        public NewServerDialog(Action<RedisServerInformation> onSave)
        {
            InitializeComponent();

            (DataContext as NewServer).OnSave = onSave;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e) => Close();

        private void CancelButton_Click(object sender, RoutedEventArgs e) => Close();
    }
}
