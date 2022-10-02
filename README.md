# ChroMapper-RhythmMarker

BeatSaberの作譜ツールの[ChroMapper](https://github.com/Caeden117/ChroMapper)で、グリッド上に一定間隔で色付きのラインマークを作成します。

Create colored line marks at regular intervals on the grid with [ChroMapper](https://github.com/Caeden117/ChroMapper) in BeatSaber's music composition tool.

![image](https://user-images.githubusercontent.com/14249877/193462676-37273fc4-e85d-4b76-b74d-76fea864661f.png)

# インストール方法 (How to Install)

1. [リリースページ](https://github.com/rynan4818/ChroMapper-RhythmMarker/releases)から、最新のプラグインのzipファイルをダウンロードして下さい。

    Download the latest plug-in zip file from the [release page](https://github.com/rynan4818/ChroMapper-RhythmMarker/releases).

2. ダウンロードしたzipファイルを解凍してChroMapperのインストールフォルダにある`Plugins`フォルダに`ChroMapper-RhythmMarker.dll`をコピーします。

    Unzip the downloaded zip file and copy `ChroMapper-RhythmMarker.dll` to the `Plugins` folder in the ChroMapper installation folder.

# 使用方法 (How to use)

譜面を読み込んでエディタ画面を出して下さい。**Tabキー**を押すと右側にアイコンパネルが出ますので、緑色のアイコンを押すと下の設定パネルが開きます。

Load the map and bring up the editor screen. Press the **Tab key** to bring up the icon panel on the right side, then press the green icon to open the settings panel below.

![image](https://user-images.githubusercontent.com/14249877/193462702-20c6249c-1e74-4987-95d9-f45a2bba33f5.png)

- Start Beat
    - 開始する拍番号
    - Start beat number
- End Beat
    - 終了する拍番号 (空欄にすると末尾まで)
    - End beat number (To the end if left blank)
- Interval
    - マーキングの間隔
    - Marking interval
- Color
    - マーキングの色
    - Marking color
- Create Marker
    - 上記設定でマーキングを作成
    - Create markings with the above settings
- Delete Marker
    - Start Beat から End Beat までのマーキングを削除
    - Remove markings from Start Beat to End Beat
- Marker View
    - マーキングを表示
    - Show marking
- Close
    - パネルを閉じる
    - Close the panel

u キーを押しながらマウスホイールでマーキング間を移動します。

Move between markings with the mouse wheel while holding down the u key.
# 開発者情報 (Developers)
このプロジェクトをビルドするには、ChroMapperのインストールパスを指定する`ChroMapper-RhythmMarker\ChroMapper-RhythmMarker.csproj.user`ファイルを作成する必要があります。

To build this project, you must create a `ChroMapper-RhythmMarker\ChroMapper-RhythmMarker.csproj.user` file that specifies the ChroMapper installation path.

```xml
<?xml version="1.0" encoding="utf-8"?>
<Project ToolsVersion="Current" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <PropertyGroup>
    <ChroMapperDir>C:\TOOL\ChroMapper\chromapper</ChroMapperDir>
  </PropertyGroup>
</Project>
```
