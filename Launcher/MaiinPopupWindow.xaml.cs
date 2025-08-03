using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.DirectoryServices.ActiveDirectory;



//using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Imaging;

using Launcher.util;

namespace Launcher
{

    /// <summary>
    /// MaiinPopupWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MaiinPopupWindow : Popup
    {
        public MaiinPopupWindow()
        {
            InitializeComponent();

            Placement = PlacementMode.MousePoint;
            StaysOpen = false;
            AllowsTransparency = true;

            this.DataContext = new PopupWindowVM(@"C:\Users\YA62101\Links");

            // マウスのフォーカスが外れたら自動的にポップアップを閉じる
            _treeView.MouseLeave += (s, e) => HidePopup();
        }

        /// <summary>ポップアップを表示する</summary>
        public void ShowPopup()
        {
            this.IsOpen = true;
        }

        /// <summary>ポップアップを非表示にする</summary>
        public void HidePopup()
        {
            this.IsOpen = false;
        }

        //private void TreeViewItem_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    if (sender is TreeViewItem item && item.Tag is string path)
        //    {
        //        if (!Directory.Exists(path))
        //        { // ディレクトリでなければ実行
        //            try
        //            {
        //                ExecuteShortcut(path);
        //            }
        //            catch (Exception ex)
        //            {
        //                MessageBox.Show(ex.Message, "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
        //            }
        //            IsOpen = false; // ポップアップを閉じる
        //        }
        //        e.Handled = true; // これ以上のイベント処理はさせない
        //    }
        //}

        ///// <summary>シェルのデフォルトの動作でショートカットを実行する</summary>
        ///// <param name="shortcutPath">ショートカットのパス</param>
        //private void ExecuteShortcut(string shortcutPath)
        //{
        //    var psi = new System.Diagnostics.ProcessStartInfo
        //    {
        //        FileName = shortcutPath, // 開きたいファイルのパス
        //        UseShellExecute = true   // シェル経由で開く
        //    };
        //    System.Diagnostics.Process.Start(psi);
        //}
    }
}
