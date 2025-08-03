using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Resources;
using System.Drawing;

namespace Launcher
{
    /// <summary>
    /// メインのコンテクストメニューをホストするダミーの透明ウィンドウ
    /// このウィンドウのアクティブ状態を用いてContextMenuの表示/非表示を切り替える
    /// https://sourcechord.hatenablog.com/entry/2017/02/11/125649
    /// 主に以下の機能を持っている
    /// * コンテクストメニューの外観設定(xamlで設定する)
    /// * コンテクストメニューの表示と自動クローズ
    /// * 通知アイコンの表示
    /// * ホットキーのサポート
    /// </summary>
    public partial class LauncherHost : Window, IDisposable
    {
        /// <summary>通知アイコンオブジェクト</summary>
        public System.Windows.Forms.NotifyIcon? NotifyIcon { get; private set; }

        /// <summary>ホットキーの管理オブジェクト</summary>
        public util.HotkeyManager? HotkeyManager { get; private set; }

        /// <summary>コンストラクタ</summary>
        /// <param name="text">トレイアイコンのポップアップで表示するテキスト</param>
        /// <param name="iconResourcePath">アイコンリソースのパス(nullを指定するとアプリケーションアイコンを使用する)</param>
        public LauncherHost(string text, string? iconResourcePath = null)
        {
            InitializeComponent();
            //this.LauncherMainMenu.DataContext = new ViewModels.MenuItemViewModelBase(shortcutDirPath);

            #region コンテキストメニューの自動開閉処理用
            this.Activated += (s, e) =>
            { // ウィンドウがアクティブになったときはコンテキストメニューを開く
                this.LauncherMainMenu.IsOpen = true;         // コンテキストメニューを開く
            };
            this.Deactivated += (s, e) =>
            { // ウィンドウが非アクティブになったときはコンテキストメニューを閉じる
                this.LauncherMainMenu.IsOpen = false;        // コンテキストメニューを閉じる
            };
            this.LauncherMainMenu.Closed += (s, e) =>
            { // コンテキストメニューが閉じられたときはウィンドウを非表示にする
                // ESCでContextMenuを閉じるとホストのウインドウがActiveのままになって、
                // 次にContextMenuが開けなくなることを防ぐための処理
                this.Hide();
            };
            #endregion

            // トレイアイコンの初期化
            //InitializeTrayIcon("ショートカットフォルダランチャー", "Resources\\app.ico");
            InitializeTrayIcon(text, iconResourcePath);
            // ホットキー管理オブジェクト生成
            HotkeyManager = new util.HotkeyManager(this);
        }

        public void Dispose()
        {
            if (HotkeyManager != null)
            {
                HotkeyManager.Dispose();
                HotkeyManager = null;
            }
            if (NotifyIcon != null)
            {
                NotifyIcon.Visible = false;
                NotifyIcon.Dispose();
                NotifyIcon = null;
            }
        }

        /// <summary>コンテキストメニューを表示する</summary>
        /// <param name="root">メニューの内容</param>
        public void ShowContextMenu(ViewModels.MenuItemViewModelBase root)
        {
            this.LauncherMainMenu.DataContext = root;
            this.Show();
            this.Activate();
            this.LauncherMainMenu.Focus();  // キーボードで操作できるようにする
        }

        #region トレイアイコン
        /// <summary>トレイアイコンを初期化する</summary>
        /// <param name="text">トレイアイコンのポップアップで表示するテキスト</param>
        /// <param name="iconResourcePath">アイコンリソースのパス(nullを指定するとアプリケーションアイコンを使用する)</param>
        private void InitializeTrayIcon(string text, string? iconResourcePath)
        {
            // トレイアイコンを取得する
            System.Drawing.Icon? icon = null;
            if (iconResourcePath == null)
            { // アイコンが指定されなければ実行アセンブリのアイコンを使用する
                var entryAsmLocation = System.Reflection.Assembly.GetEntryAssembly()?.Location;
                if (entryAsmLocation != null)
                    icon = System.Drawing.Icon.ExtractAssociatedIcon(entryAsmLocation);
            }
            else
            { // アイコンが指定されているならそのアイコンを使用する
                icon = new System.Drawing.Icon(System.Windows.Application.GetResourceStream(new Uri(iconResourcePath, UriKind.Relative)).Stream);
            }

            // トレイにアプリを表示する
            NotifyIcon = new System.Windows.Forms.NotifyIcon()
            {
                Icon = icon,
                Visible = true,
                Text = text,
            };
        }
        #endregion
    }
}
