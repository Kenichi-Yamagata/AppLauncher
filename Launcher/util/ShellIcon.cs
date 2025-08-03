using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Launcher.util
{
    /// <summary>
    /// ファイルのアイコンを取得するためのユーティリティクラス
    /// </summary>
    public class ShellIcon
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct SHFILEINFO
        {
            public nint hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }

        [DllImport("shell32.dll")]
        public static extern nint SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool DestroyIcon(nint hIcon);

        public const uint SHGFI_ICON = 0x000000100;
        public const uint SHGFI_LARGEICON = 0x000000000;
        public const uint SHGFI_SMALLICON = 0x000000001;
        public const uint FILE_ATTRIBUTE_NORMAL = 0x00000080;

        /// <summary>指定されたファイルパスからアイコンを取得します</summary>
        /// <param name="filePath">アイコンを取得するファイルのパス</param>
        /// <param name="isLargeIcon">大きいアイコンを取得する場合はtrue、小さいアイコンの場合はfalse</param>
        /// <returns>ファイルのアイコンを表すImageSource、取得に失敗した場合はnull</returns>
        public static BitmapSource? GetIcon(string filePath, bool isLargeIcon = true)
        {
            // 指定されたパスに対応するファイルまたはディレクトリが存在しない場合はnullを返す
            if (!File.Exists(filePath) && !Directory.Exists(filePath)) return null;

            // アイコンの取得
            SHFILEINFO shfi = new SHFILEINFO(); // アイコンリソース(要開放)
            nint result = SHGetFileInfo(
                filePath,
                FILE_ATTRIBUTE_NORMAL,
                ref shfi,
                (uint)Marshal.SizeOf(shfi),
                SHGFI_ICON | (isLargeIcon ? SHGFI_LARGEICON : SHGFI_SMALLICON)
                );
            // アイコンリソースの取得に失敗した場合はnullを返す
            if (result == nint.Zero || shfi.hIcon == nint.Zero) return null;

            try
            {
                // アイコンハンドルからBitmapSourceを作成
                return Imaging.CreateBitmapSourceFromHIcon(
                    shfi.hIcon,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                // アイコンリソースを解放
                DestroyIcon(shfi.hIcon);
            }
        }
    }
}