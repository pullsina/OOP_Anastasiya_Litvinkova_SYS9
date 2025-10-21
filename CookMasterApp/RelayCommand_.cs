using System.Windows.Input;

namespace MVVM_KlonaMIg.MVVM
{
    public class RelayCommand : ICommand
    {
        //Fält för att hålla referenser till metoder som definierar vad som ska göras (Execute)
        private Action<object> execute;

        //Kollar om kommandot kan köras
        private Func<object, bool> canExecute;


        //Event som signalerar när kommandots möjlighet att köras har ändrats
        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        //Bestämmer om kommandot kan köras eller inte
        public bool CanExecute(object? parameter)
        {
            return canExecute == null || canExecute(parameter);
        }

        //Kör den logik som tilldelats via execute metoden
        public void Execute(object? parameter)
        {
            execute(parameter);
        }
    }
}
