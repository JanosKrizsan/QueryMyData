﻿using System;
using System.Windows.Input;

namespace QueryMyData.Model
{
    public class DelegateCommand<T> : ICommand where T : class
    {

        private readonly Action<T> _execute;
        private readonly Predicate<T> _canExecute;

        public DelegateCommand()
        {
        }

        public DelegateCommand(Action<T> execute)
            : this(execute, null)
        {
        }

        public DelegateCommand(Action<T> execute, Predicate<T> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute((T)parameter);
        }

        public void Execute(object parameter)
        {
            _execute((T)parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}