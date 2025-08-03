using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Launcher
{
    /// <summary>アプリケーション設定</summary>
    public class AppConfig
    {
        /// <summary>設定オブジェクト</summary>
        public IConfiguration Config { get; }

        /// <summary>ランチャ用のショートカットが保存されているフォルダのパス(環境変数は展開される)</summary>
        public string ShortcutDirPath
        {
            get => Environment.ExpandEnvironmentVariables(Config["AppSettings:ShortcutDirPath"] ?? "%USERPROFILE%\\Links");
        }

        /// <summary>操作とホットキーの一覧を取得する</summary>
        public IEnumerable<(string Operation, KeyGesture? HotKey)> HotKeys {
            get
            {
                var kgc = new KeyGestureConverter();
                return Config.GetSection("AppSettings:HotKeys")
                    .GetChildren()
                    .Select(s => (Operation: s.Key, HotKey: s.Value == null ? null : (KeyGesture?)kgc.ConvertFromString(s.Value)));
                    //.Where(s => s.HotKey != null);  // ホットキーが指定されていないもののみ返す
            }
        }

        /// <summary>コンストラクタ</summary>
        public AppConfig()
        {
            // 設定ファイルを読み込む
            Config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                //.AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                //.AddEnvironmentVariables()
                .Build();
        }
    }
}
