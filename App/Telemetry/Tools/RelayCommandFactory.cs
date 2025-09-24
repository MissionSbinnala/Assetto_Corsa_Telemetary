using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Telemetry.Tools
{
    public static class RelayCommandFactory
    {
        public static ICommand Create(Action execute, Func<bool> canExecute = null)
            => new RelayCommand(execute, canExecute);

        public static ICommand Create<T>(Action<T> execute, Predicate<T> canExecute = null)
            => new RelayCommand<T>(execute, canExecute);
    }

}
