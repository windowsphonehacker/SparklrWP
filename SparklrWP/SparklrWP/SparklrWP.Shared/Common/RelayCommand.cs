using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SparklrWP.Common
{
    /// <summary>
    /// Un comando con l'unico scopo di affidarsi alla propria funzionalità 
    /// ad altri oggetti richiamando i delegati. 
    /// Il valore restituito predefinito per il metodo CanExecute è 'true'.
    /// <see cref="RaiseCanExecuteChanged"/> deve essere chiamato ogni volta che
    /// <see cref="CanExecute"/> è previsto che venga restituito un valore differente.
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        /// <summary>
        /// Generato quando RaiseCanExecuteChanged viene chiamato.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Crea un nuovo comando che può essere sempre eseguito.
        /// </summary>
        /// <param name="execute">La logica di esecuzione.</param>
        public RelayCommand(Action execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// Crea un nuovo comando.
        /// </summary>
        /// <param name="execute">La logica di esecuzione.</param>
        /// <param name="canExecute">La logica dello stato di esecuzione.</param>
        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");
            _execute = execute;
            _canExecute = canExecute;
        }

        /// <summary>
        /// Determina se <see cref="RelayCommand"/> può essere eseguito nello stato corrente.
        /// </summary>
        /// <param name="parameter">
        /// Dati utilizzati dal comando. Se il comando non richiede il passaggio di dati, è possibile impostare questo oggetto su null.
        /// </param>
        /// <returns>true se il comando può essere eseguito; in caso contrario, false.</returns>
        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute();
        }

        /// <summary>
        /// Esegue <see cref="RelayCommand"/> sulla destinazione del comando corrente.
        /// </summary>
        /// <param name="parameter">
        /// Dati utilizzati dal comando. Se il comando non richiede il passaggio di dati, è possibile impostare questo oggetto su null.
        /// </param>
        public void Execute(object parameter)
        {
            _execute();
        }

        /// <summary>
        /// Il metodo utilizzato per generare l'evento <see cref="CanExecuteChanged"/>
        /// per indicare che il valore restituito di <see cref="CanExecute"/>
        /// il metodo è cambiato.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}