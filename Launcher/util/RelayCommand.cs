using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Launcher.util
{
    /// <summary>
    /// コマンドを簡単に実装するためのRelayCommandクラス
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool>? _canExecute;

        /// <summary>
        /// RelayCommandの新しいインスタンスを初期化します
        /// </summary>
        /// <param name="execute">コマンドが実行される際に呼び出されるアクション</param>
        /// <param name="canExecute">コマンドが実行可能かどうかを判断する関数</param>
        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        /// <summary>
        /// コマンドの実行状態が変更されたときに発生します
        /// </summary>
        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// コマンドが現在の状態で実行可能かどうかを判断します
        /// </summary>
        /// <param name="parameter">コマンドによって使用されるデータ</param>
        /// <returns>コマンドが実行可能な場合はtrue、それ以外の場合はfalse</returns>
        public bool CanExecute(object? parameter)
        {
            return _canExecute == null || _canExecute();
        }

        /// <summary>
        /// コマンドを実行します
        /// </summary>
        /// <param name="parameter">コマンドによって使用されるデータ</param>
        public void Execute(object? parameter)
        {
            _execute();
        }
    }

    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool>? _canExecute;

        /// <summary>
        /// RelayCommandの新しいインスタンスを初期化します
        /// </summary>
        /// <param name="execute">コマンドが実行される際に呼び出されるアクション</param>
        /// <param name="canExecute">コマンドが実行可能かどうかを判断する関数</param>
        public RelayCommand(Action<T> execute, Func<T, bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        /// <summary>
        /// コマンドの実行状態が変更されたときに発生します
        /// </summary>
        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// コマンドが現在の状態で実行可能かどうかを判断します
        /// </summary>
        /// <param name="parameter">コマンドによって使用されるデータ</param>
        /// <returns>コマンドが実行可能な場合はtrue、それ以外の場合はfalse</returns>
        public bool CanExecute(object? parameter)
        {
            if (parameter is T typedParameter || parameter == null)
            {
                return _canExecute == null || _canExecute((T)parameter!);
            }
            return false;
        }

        /// <summary>
        /// コマンドを実行します
        /// </summary>
        /// <param name="parameter">コマンドによって使用されるデータ</param>
        public void Execute(object? parameter)
        {
            if (parameter is T typedParameter || parameter == null)
            {
                _execute((T)parameter!);
            }
        }
    }
}
