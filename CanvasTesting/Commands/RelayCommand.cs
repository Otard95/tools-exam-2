using System;
using System.Windows.Input;

namespace CanvasTesting.Commands {
    public class RelayCommand : ICommand {

        private Action _command;
        private Func<bool> _can_excecute;

        public RelayCommand (Action command, Func<bool> canExcecute) {
            _command = command;
            _can_excecute = canExcecute;
        }

        public event EventHandler CanExecuteChanged {
            add    { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute (object parameter) {
            return _can_excecute.Invoke();
        }

        public void Execute (object parameter) {
            _command.Invoke();
        }
    }
}
