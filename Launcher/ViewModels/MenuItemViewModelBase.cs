using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;
using System.IO;
using System.Runtime.CompilerServices;
using Launcher.util;
using System.Linq;


namespace Launcher.ViewModels
{
    public class MenuItemViewModelBase : ObservableObject
    {
        /// <summary>クリック時に実行されるコマンド</summary>
        public ICommand? ExecuteCommand { get; set; } = null;
        /// <summary>ダブルクリック時に実行されるコマンド</summary>
        public ICommand? DoubleClickCommand { get; set; } = null;

        /// <summary>アイテムの名前</summary>
        public string DisplayName { get; set; } = "";
        /// <summary>アイコン</summary>
        public ImageSource? IconSource { get; set; } = null;
        /// <summary>子要素</summary>
        public ObservableCollection<MenuItemViewModelBase> Children { get; }


        public MenuItemViewModelBase()
        {
            Children = new ObservableCollection<MenuItemViewModelBase>();
        }
    }
}