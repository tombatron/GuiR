using System;
using System.Windows.Input;

namespace GuiR.ViewModels
{
    internal class DelegateCommand : ICommand
    {
        private readonly Action _action;

        public event EventHandler CanExecuteChanged;

        public DelegateCommand(Action action) => _action = action;

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter) => _action();
    }
}