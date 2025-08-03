using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launcher
{
    /// <summary>ランチャのメイン処理オブジェクト</summary>
    class LauncherMain : IDisposable
    {
        /// <summary>アプリケーションの設定</summary>
        public AppConfig Config { get; private set; }

        /// <summary>ランチャのホストウインドウ</summary>
        LauncherHost _launcherHost;

        /// <summary>ランチャーメニュー</summary>
        ViewModels.ShortcutDir _launcherMenu;
        /// <summary>右クリックメニュー</summary>
        ViewModels.MenuItemViewModelBase _rightClickMenu;

        /// <summary>コンストラクタ</summary>
        public LauncherMain()
        {
            // 設定ファイルを読み込む
            this.Config = new AppConfig();

            // ランチャホスト(不可視のメインウインドウ)を生成する
            _launcherHost = new LauncherHost("ショートカットフォルダランチャー");
            // ランチャメニュー(ショートカットフォルダの中身をロード)
            _launcherMenu = new ViewModels.ShortcutDir(Config.ShortcutDirPath);
            // 右クリックのコンテンツを生成
            _rightClickMenu = CreateRightClickMenu();
            // タスクトレイ用のイベントハンドラ設定
            _launcherHost.NotifyIcon!.MouseClick += _notifyIcon_MouseClick;
            _launcherHost.NotifyIcon!.MouseDoubleClick += _notifyIcon_MouseDoubleClick;
            // ホットキーの設定
            InitHotKey();
        }

        /// <summary>ホットキーを設定する</summary>
        private void InitHotKey()
        {
            // 設定ファイル中のコマンド名と実際に実行されるメソッドの組み合わせ
            var operations = new Dictionary<string, Action>()
                {
                    { "OpenLauncher", ShowLauncher },
                };
            // ホットキーを設定
            var hotkeys = Config.HotKeys
                .Where(hk => hk.HotKey != null && operations.ContainsKey(hk.Operation)) // ホットキーが有効で操作も定義されているものだけを処理する
                .Select((hk, i) => new util.HotkeyManager.HotkeyItem(i, hk.HotKey!, operations[hk.Operation]))
                .ToArray();
            _launcherHost.HotkeyManager!.Init(hotkeys);
        }

        /// <summary>右クリックメニュー作成</summary>
        /// <returns>右クリックメニューオブジェクト</returns>
        private ViewModels.MenuItemViewModelBase CreateRightClickMenu()
        {
            var rightClickMenu = new ViewModels.MenuItemViewModelBase();
            rightClickMenu.Children.Add(new ViewModels.MenuItemViewModelBase()
            {
                DisplayName = "終了",
                IconSource = null,
                ExecuteCommand = new util.RelayCommand<object?>(_ => App.Current.Shutdown()),
            });
            rightClickMenu.Children.Add(new ViewModels.MenuItemViewModelBase()
            {
                DisplayName = "更新",
                IconSource = null,
                ExecuteCommand = new util.RelayCommand<object?>(_ => _launcherMenu.Load()),
            });
            rightClickMenu.Children.Add(new ViewModels.MenuItemViewModelBase()
            {
                DisplayName = "フォルダを開く",
                IconSource = null,
                ExecuteCommand = new util.RelayCommand<object?>(_ => {
                    System.Diagnostics.Process.Start(
                        new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = _launcherMenu.Path,
                            UseShellExecute = true
                        });
                }),
            });
            return rightClickMenu;
        }

        /// <summary>NotifyIconのクリックイベントハンドラ</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void _notifyIcon_MouseClick(object? sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            { // 左クリックでランチャ表示
                ShowLauncher();
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Right)
            { // 右クリックでメニュー表示
                _launcherHost.ShowContextMenu(_rightClickMenu);
            }
        }

        /// <summary>NotifyIconのダブルクリックイベントハンドラ</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void _notifyIcon_MouseDoubleClick(object? sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            { // 左ダブルクリックで終了
                App.Current.Shutdown();
            }
        }

        /// <summary>現在のマウスカーソル位置にランチャを表示する</summary>
        public void ShowLauncher()
        {
            _launcherHost.ShowContextMenu(_launcherMenu);
        }

        public void Dispose()
        {
            if (_launcherHost != null)
            {
                if (_launcherHost.NotifyIcon != null)
                {
                    _launcherHost.NotifyIcon.MouseClick -= _notifyIcon_MouseClick;               // タスクトレイのアイコンをクリックしたときの動作
                    _launcherHost.NotifyIcon.MouseDoubleClick -= _notifyIcon_MouseDoubleClick;   // タスクトレイのアイコンを左ダブルクリックしたときの動作
                }
                _launcherHost.Dispose();
                //_launcherHost = null;
            }
        }
    }
}
