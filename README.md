# OpenTween

<img src="./OpenTween/Resources/LogoImage.png" alt="OpenTween" style="width: 150px; height: 150px" />

[![GitHub Actions](https://github.com/opentween/OpenTween/actions/workflows/build.yml/badge.svg?branch=develop&event=push)](https://github.com/opentween/OpenTween/actions/workflows/build.yml)
[![Codecov](https://codecov.io/gh/opentween/OpenTween/branch/develop/graph/badge.svg)](https://app.codecov.io/gh/opentween/OpenTween/tree/develop)

OpenTween は Windows 用の高機能な Twitter クライアントです。

---

⚠️ OpenTween の API キーは 2023/1/22 に Twitter のポリシー違反を理由に凍結されたため、OpenTween に標準で組み込まれている API キーで Twitter API にアクセスすることはできません。

## 動作要件

- Windows 10 以降
- [.NET Framework 4.8 ランタイム](https://dotnet.microsoft.com/ja-jp/download/dotnet-framework/net48)
  - Windows 10 21H1 以降には標準でインストールされています

## ダウンロード

最新のリリースは以下の Web サイトでダウンロードできます。

- GitHub: https://github.com/opentween/OpenTween/releases
- OSDN: https://osdn.net/projects/opentween/releases/p12655

開発版は AppVeyor からダウンロードできます。AppVeyor で公開されているビルドは OpenTween のリポジトリに変更が加えられるたびに更新されます。

- AppVeyor: https://ci.appveyor.com/project/upsilon/opentween/build/artifacts?branch=develop

## 更新履歴

[CHANGELOG.txt](./CHANGELOG.txt) を参照してください。

## ソースコードのビルド方法

ソースコードから OpenTween をビルドするためには以下の環境が必要です。

- [Visual Studio 2022](https://visualstudio.microsoft.com/downloads/) 17.4 以上
  - インストール時にワークロードの「.NET デスクトップ開発」を選択してください
- Git for Windows

ビルド手順:

1. `git clone https://github.com/opentween/OpenTween.git`
1. Visual Studio で `OpenTween.sln` を開く
1. メニューから「デバッグ」→「デバッグの開始」(F5) を実行する
1. ビルドが開始され、正常に完了すれば OpenTween が起動します

## Tween との関係について

Tween は [@kiri_feather](https://twitter.com/kiri_feather) 氏によって 2007 年から公開されている Twitter クライアントです。
かつて Tween はフリーソフトウェア (GPLv3) として公開されていましたが、2011 年にリリースされた Tween 1.2.0.0 からはプロプライエタリに変更されることが告知され、以後のリリースではソースコードが非公開となりました。

これを受けて [@kim_upsilon](https://twitter.com/kim_upsilon) が、GPLv3 で公開されていた最終版の Tween のソースコードをもとにフォークして立ち上げたプロジェクトが現在の OpenTween です。

Tween と OpenTween は現在に至るまで互いに独立した体制のもとで開発が行われています。名称から「Tween の最新版のソースコードが OpenTween として公開されている」ものと誤解されることがありますが、これは正しくありません。

OpenTween は 2011 年時点の Tween のソースコードを起点としていることから現在の Tween にも存在する機能を多く備えていますが、一方で OpenTween のフォーク以降については Tween と OpenTween で別々の発展を遂げてきたため実装された機能にも差異があります。
また、OpenTween の発足初期にソースコードを VB.NET から C# へ全面的に移行したことや、その後も内部の設計に変更を加えた影響で、外見上は似た機能を備えつつも原形となった Tween のソースコードからは乖離している部分が多くあります。

## ライセンス

© 2011 OpenTween contributors.

ソースコードは GPLv3 の下で利用することができます。詳細は [LICENSE.ja](./LICENSE.ja) を参照してください。

また、OpenTween のロゴなどの画像リソースは [CC BY-SA 2.1 JP](https://creativecommons.org/licenses/by-sa/2.1/jp/) の下で利用することができます。
詳細は [OpenTween-icons](https://github.com/opentween/OpenTween-icons) のリポジトリを参照してください。
