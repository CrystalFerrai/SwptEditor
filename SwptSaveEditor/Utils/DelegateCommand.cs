// Copyright 2021 Crystal Ferrai
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Windows.Input;

namespace SwptSaveEditor.Utils
{
    /// <summary>
    /// A simple implementation of ICommand that delegates control to the owner of the command
    /// </summary>
    internal class DelegateCommand : ICommand
    {
        /// <summary>
        /// The action the command should execute
        /// </summary>
        private Action mExecute;

        /// <summary>
        /// The function to call to determine if the command can execute
        /// </summary>
        private Func<bool> mCanExecute;

        /// <summary>
        /// Event fired changes have occurred which may affect whether the command can execute
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Creates a new instance of the DelegateCommand class
        /// </summary>
        /// <param name="execute">The action the command should execute</param>
        /// <param name="canExecute">The function to call to determine if the command can execute</param>
        public DelegateCommand(Action execute, Func<bool> canExecute = null)
        {
            mExecute = execute;
            mCanExecute = canExecute;
        }

        /// <summary>
        /// Executes the command
        /// </summary>
        /// <param name="parameter">The command parameter</param>
        public void Execute(object parameter)
        {
            if (mExecute != null) mExecute();
        }

        /// <summary>
        /// Returns whether the command can execute in its current state
        /// </summary>
        /// <param name="parameter">The command parameter</param>
        public bool CanExecute(object parameter)
        {
            if (mCanExecute != null) return mCanExecute();
            return true;
        }

        /// <summary>
        /// Fires the CanExecuteChanged eventm indicating that changes have occurred which may affect whether the command can execute
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            EventHandler handler = CanExecuteChanged;
            if (handler != null)
            {
                handler.Invoke(this, EventArgs.Empty);
            }
        }
    }

    /// <summary>
    /// A simple implementation of ICommand that delegates control to the owner of the command
    /// </summary>
    public class DelegateCommand<T> : ICommand
    {
        /// <summary>
        /// The action the command should execute
        /// </summary>
        private Action<T> mExecute;

        /// <summary>
        /// The function to call to determine if the command can execute
        /// </summary>
        private Func<T, bool> mCanExecute;

        /// <summary>
        /// Event fired changes have occurred which may affect whether the command can execute
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Creates a new instance of the DelegateCommand class
        /// </summary>
        /// <param name="execute">The action the command should execute</param>
        /// <param name="canExecute">The function to call to determine if the command can execute</param>
        public DelegateCommand(Action<T> execute, Func<T, bool> canExecute = null)
        {
            mExecute = execute;
            mCanExecute = canExecute;
        }

        /// <summary>
        /// Executes the command
        /// </summary>
        /// <param name="parameter">The command parameter</param>
        public void Execute(object parameter)
        {
            if (mExecute != null) mExecute((T)parameter);
        }

        /// <summary>
        /// Returns whether the command can execute in its current state
        /// </summary>
        /// <param name="parameter">The command parameter</param>
        public bool CanExecute(object parameter)
        {
            if (mCanExecute != null) return mCanExecute((T)parameter);
            return true;
        }

        /// <summary>
        /// Fires the CanExecuteChanged eventm indicating that changes have occurred which may affect whether the command can execute
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            EventHandler handler = CanExecuteChanged;
            if (handler != null)
            {
                handler.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
