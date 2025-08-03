using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace Launcher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>ランチャのメインオブジェクト</summary>
        LauncherMain? _launcher;

        public App()
        {
            ShutdownMode = ShutdownMode.OnExplicitShutdown;     // Shutdown()で明示的に終了しない限りアプリケーションを終了させない
            _launcher = new LauncherMain();                     // ビューモデル
        }

        /// <summary>アプリケーションの終了処理</summary>
        /// <param name="e"></param>
        protected override void OnExit(ExitEventArgs e)
        {
            if (_launcher != null)
            {
                _launcher.Dispose();
                _launcher = null;
            }
        }
    }
}