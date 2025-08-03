using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows;

namespace Launcher.util
{
    /// <summary>ホットキー管理オブジェクト</summary>
    public class HotkeyManager : IDisposable
    {
        // http://nineworks2.blog.fc2.com/blog-entry-17.html
        // https://tan.hatenadiary.jp/entry/20160313/1457797655

        /// <summary>ホットキー要素</summary>
        public class HotkeyItem
        {
            /// <summary>ID</summary>
            public int HotKeyID { get; }
            /// <summary>ホットキー情報</summary>
            public KeyGesture HotKey { get; }
            /// <summary>ホットキーに対応する処理</summary>
            public Action Action { get; }

            /// <summary>コンストラクタ</summary>
            /// <param name="hotkeyId">ID</param>
            /// <param name="hotkey">ホットキー</param>
            /// <param name="action">処理</param>
            public HotkeyItem(int hotkeyId, KeyGesture hotkey, Action action)
            {
                this.HotKeyID = hotkeyId;
                this.HotKey = hotkey;
                this.Action = action;
            }
        }

        /// <summary>HotKey登録を行う</summary>
        /// <param name="hWnd"></param>
        /// <param name="id"></param>
        /// <param name="modKey"></param>
        /// <param name="vKey"></param>
        /// <returns>失敗の場合は0（他で使用されている）、成功の場合は0以外の数値が返される。</returns>
        [DllImport("user32.dll")]
        private static extern int RegisterHotKey(IntPtr hWnd, int id, int modKey, int vKey);
        /// <summary>HotKey解除を行う</summary>
        /// <param name="hWnd"></param>
        /// <param name="id"></param>
        /// <returns>失敗の場合は0、成功の場合は0以外の数値が返される</returns>
        [DllImport("user32.dll")]
        private static extern int UnregisterHotKey(IntPtr hWnd, int id);

        const int WM_HOTKEY = 0x0312;

        /// <summary>対象ウインドウ</summary>
        private Window Win { get; set; }
        /// <summary>対象ウインドウのウインドウハンドル</summary>
        private IntPtr WindowHandle { get => new WindowInteropHelper(this.Win).Handle; }

        /// <summary>ホットキー情報</summary>
        Dictionary<int, HotkeyItem> _hotkeyItems;

        /// <summary>コンストラクタ(ホットキーを登録して処理できるようにする)</summary>
        /// <param name="win">対象ウインドウ</param>
        /// <param name="hotkeyItems">ホットキーオブジェクト</param>
        public HotkeyManager(Window win)
        {
            this.Win = win;                                     // Windowへの参照を保存
            this.Win.Closed += Win_Closed;                      // Windowを閉じたときにホットキーを開放する
            _hotkeyItems = new Dictionary<int, HotkeyItem>();   // とりあえず空の辞書を割り当てておく
        }

        public void Init(HotkeyItem[] hotkeyItems)
        {
            // ホットキー情報を保存
            _hotkeyItems.Clear();
            foreach(var hk in hotkeyItems)
                _hotkeyItems.Add(hk.HotKeyID, hk);
            // ホットキーを登録
            foreach (var item in _hotkeyItems.Values)
            {
                var res = RegisterHotKey(WindowHandle, item.HotKeyID, (int)item.HotKey.Modifiers, KeyInterop.VirtualKeyFromKey(item.HotKey.Key));
                if (res == 0) System.Windows.MessageBox.Show($"HotKeyID'{item.HotKeyID}'の登録に失敗しました。");
            }
            // ホットキー処理用イベントハンドラ
            ComponentDispatcher.ThreadPreprocessMessage += ComponentDispatcher_ThreadPreprocessMessage;
        }

        /// <summary>ホットキーやイベントハンドラを開放する</summary>
        public void Dispose()
        {
            // ホットキー処理のハンドラを開放
            ComponentDispatcher.ThreadPreprocessMessage -= ComponentDispatcher_ThreadPreprocessMessage;
            // ホットキーを開放
            foreach (var item in _hotkeyItems.Values)
                UnregisterHotKey(WindowHandle, item.HotKeyID);
            // ホットキーオブジェクトを開放
            _hotkeyItems.Clear();
        }

        /// <summary>対象ウインドウが閉じたときにホットキーを開放するためのイベントハンドラ</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Win_Closed(object? sender, EventArgs e)
        {
            // 自分自身を開放
            this.Dispose();
            // イベントハンドラ開放
            this.Win.Closed -= Win_Closed;
        }

        // ここでHotKeyが押された際の挙動を設定する。
        private void ComponentDispatcher_ThreadPreprocessMessage(ref MSG msg, ref bool handled)
        {
            // HotKeyが押されたかどうかを判定。
            if (msg.message != WM_HOTKEY) return;

            // 対応するHotkeyIdを取得
            var hotkeyId = msg.wParam.ToInt32();
            if (_hotkeyItems.TryGetValue(hotkeyId, out HotkeyItem? item))
            { // 対象のHotkeyがあれば処理する
                try
                {
                    item?.Action.Invoke();
                    //SynchronizationContext.Current.Post(new SendOrPostCallback(_ => {
                    //    // コンテキストメニューを表示する
                    //    item.Action.Invoke();
                    //}), null);
                }
                catch (Exception)
                {

                }
            }
        }
    }
}
