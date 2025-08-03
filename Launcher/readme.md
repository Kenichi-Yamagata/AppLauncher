# ショートカットランチャープログラム

## 概要

アプリケーション設定ファイル(appsettings.json)の"ShortcutDirPath"に記述されたフォルダにあるショートカットからプログラムを起動するランチャアプリケーション。

## 機能

* 設定ファイルは"appsettings.json"
	* "ShortcutDirPath": ランチャとして表示するショートカットを保存しているフォルダパスを指定する(エントリに含まれる環境変数文字列は適切に展開されます)
	* "HotKeys": ホットキーを指定します
		* "OpenLauncher": ランチャを開くためのホットキーを指定します
* ランチャの呼び出しは以下のいずれか
	* タスクトレイのアイコンを左クリックする
	* ホットキー`Alt + 1`(ホットキーは設定ファイルで変更可能)を押す
* 管理メニュー
	* 呼び出し: タスクトレイのアイコンを右クリックする
	* 管理メニュー項目
		* Exit : アプリケーションを終了します
		* Reload ShortCuts : ショートカットフォルダの内容を再読み込みします
		* Open ShortCuts Dir with Explorer : ショートカットフォルダをエクスプローラで開きます

## 参考リンク

* [WPF用にNotifyIconクラスをラップしてみた](https://sourcechord.hatenablog.com/entry/2017/02/11/125649) 基本的にこれに従って実装しています。
* [WPFでタスクトレイ常駐アプリを作る](https://qiita.com/TiggeZaki/items/aa17edbef0cc5f4736d9)

* グローバルホットキーを設定する場合は、以下のようにする
	* [Windows, F# - グローバルホットキーを設定する](https://zenn.dev/shikatan/articles/f6c4c52c134b61)
    * [C#|グローバルホットキーを登録する](https://anis774.net/codevault/hotkey.html)

## 将来対応

* メニューの展開が左向きになっているのをいつか修正したい
