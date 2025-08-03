using Launcher.util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Launcher.ViewModels
{
    public class ShortcutDir : MenuItemViewModelBase
    {
        /// <summary>アイテムのパス</summary>
        public string Path { get; }

        /// <summary>アイテムがディレクトリかどうかを示す値</summary>
        public bool IsDirectory => Directory.Exists(Path);

        public ShortcutDir(string path) : base()
        {
            Path = path;
            var name = System.IO.Path.GetFileName(Path);
            if (name.EndsWith(".lnk", StringComparison.CurrentCultureIgnoreCase))
                name = name.Substring(0, name.Length - ".lnk".Length);
            DisplayName = name;

            IconSource = util.ShellIcon.GetIcon(path, true);
            ExecuteCommand = new RelayCommand<object?>(_ => OpenWithShell());

            Load();
        }

        /// <summary>既存の子要素を削除して自分の下位をロードする</summary>
        public void Load()
        {
            // これまで表示していたものはクリア
            Children.Clear();
            // フォルダなら下位のオブジェクトを取得
            if (IsDirectory)
            {
                foreach (var dir in Directory.GetDirectories(this.Path).OrderBy(d => d, StringComparer.CurrentCultureIgnoreCase))
                {
                    // 隠し属性の場合はスキップする
                    if ((File.GetAttributes(dir) & FileAttributes.Hidden) == FileAttributes.Hidden) continue;
                    // 子要素を追加
                    Children.Add(new ShortcutDir(dir));
                }
                foreach (var file in Directory.GetFiles(this.Path).OrderBy(f => f, StringComparer.CurrentCultureIgnoreCase))
                {
                    // 隠し属性の場合はスキップする
                    if ((File.GetAttributes(file) & FileAttributes.Hidden) == FileAttributes.Hidden) continue;
                    // 子要素を追加
                    Children.Add(new ShortcutDir(file));
                }
            }
        }

        /// <summary>シェルのデフォルトで開く</summary>
        public void OpenWithShell()
        {
            // フォルダの時には何もせずに戻る
            if (this.IsDirectory) return;
            // フォルダ意外ならシェルのデフォルトで開く
            System.Diagnostics.Process.Start(
                new System.Diagnostics.ProcessStartInfo
                {
                    FileName = this.Path,
                    UseShellExecute = true
                });
        }
    }
}
