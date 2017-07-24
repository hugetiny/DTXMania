============================================================
  DTXMania .NET style
  (C) 2000 2017 DTXMania Group
============================================================

■動作に必要なもの

(1) OS ...  Windows 7 (x86, x64) / 8 (x86, x64) / 8.1 (x86, x64) / 10 (x86, x64)
(2) .NET Framework ... 4.7
　(Win10 Creaturs Update以前のOSでは、.NET Framework 4.7の追加インストールが必要です)
　https://support.microsoft.com/ja-jp/help/3186497/the-net-framework-4-7-offline-installer-for-windows
(3) DirectX エンドユーザ ランタイム ... June 2010 以降
　(Win8以降では、DirectX 9.0cの追加インストールが必要です)
(4) Microsoft Visual C++ 2013 Redistributable Package (x86)

上記が導入されていない場合、以下のようなエラーダイアログが
表示され、DTXMania を起動することができません。

「アプリケーションを正しく初期化できませんでした」
「****.dll が見つからなかったため、このアプリケーションを
　開始できませんでした」（※ **** は任意の名前）

■DirectX エンドユーザランタイムのインストール

前節(3)の DirectX エンドユーザランタイムは、本アーカイブの
"DirectX Redist" の中に、DTXMania の動作に「必要最小限」の
構成を入れてあります。

ここにある DXSETUP.exe で、DirectX をインストールして下さい。
(以前は、Microsoft のサイトからDirectXのweb setupを利用することが
 できましたが、Windows10ではその方法が使えません。
 DirectX RedistフォルダのDXSETUP.exeを使ってください)


■DTXMania のインストール

DTXMania のインストールは不要です。
お好きな場所に配置して下さい。

■アンインストール

DTXMania のフォルダごと全部削除して下さい。
レジストリ等はいじってません。

■曲データのインストール

曲データ(DTX, GDAなど)は本アーカイブには含まれていませんので、
初期状態では曲が1つも表示されません。
各自、いろいろな手段で入手して下さい。（汗

入手した曲データは、DTXManiaGR.exe と同じフォルダの中に
適当な名前(何でもよい)のフォルダを作り、その中にコピーして
下さい。

DTXMania は、通常、DTXManiaGR.exeがあるフォルダをルートフォルダと
して、曲データを検索します。

ルートフォルダからの階層の深さは問いません。どこまでも深く探しに
行きます。

Config.xml（DTXManiaを1回でも起動すると、DTXManiaGR.exeと同じ
フォルダに自動的に作成されます）の strSongDataPath の内容を変更
することで、ルートフォルダを変更できます。
(この際に、ルートフォルダは、セミコロンで区切って、複数指定できます)



DTXMania は、曲の演奏結果を、曲データの存在するフォルダ内に
socre.ini ファイルとして出力します。そのため、
曲データの存在するフォルダに"書き込み権限"が無ければ、DTXMania が
エラー終了するか、または記録が残されません。

よって、PC の管理者以外の人が DTXMania で遊ぶ場合は、曲データの配置
場所にご注意ください。


■WASAPI/ASIO対応について
DTXManiaは、Release 096以降、WASAPI/ASIOに対応しています。
また、Release 109以降では、WASAPIは、WASAPI-Exclusive(WASAPI排他)と
WASAPI-Shared(WASAPI共有)の両方に対応しました。
(Release 096～108のWASAPI対応は、WASAPI排他のみでした)

WASAPI排他又はASIOを使用することで、従来のバージョンと比べて、
パッドを叩いてから音が出るまでのラグを小さくすることができます。

初期状態ではWASAPI排他でサウンドを再生する設定になっています。
ASIOに変更する場合は、CONFIGURATIONで、SoundTypeをASIOに変更して下さい。
(XPをお使いの場合は、初期設定はDirectSound(従来と同じ)です。)


ASIOを使用するように設定しても、これが使用できない場合は、自動的に
WASAPI_Exclusive(WASAPI排他)を使うよう試みます。更にWASAPI排他が使用
できない場合はWASAPI_Shared(WASAPI共有)を、WASAPI共有が使用できない
ときは、DirectSoundを使います。


ウインドウのタイトルに、使用しているサウンド出力方式と、
バッファサイズを表示しています。
設定を固めるまでは、DTXManiaをウインドウモードにして
あれこれ試してみることをお勧めします。
フルスクリーンモードではこれらの情報が表示されないため、
設定が大変です。

また、ASIOは、設定をきちんと行わないと効果が出ません。
以下の注意事項をよく読んで、正しく設定してご使用下さい。


■WASAPI排他/共有使用時の注意
WASAPIは、Vista以降で使用できます。XPでは使用できません。

■ASIO使用時の注意
ASIOは、XPでも使用可能ですが、ASIOに対応したサウンドカードが
必要です。ASIOに対応していないサウンドカードでASIOを使う
場合は、フリーソフトの"ASIO4ALL"をインストールしておくことで、
ある程度ASIOの性能を利用することができます。

ASIO使用時は、ASIOのバッファサイズを適切に設定して下さい。
このバッファサイズは、DTXManiaで設定するのではなく、
サウンドデバイス側で設定する必要があります。
(サウンドデバイスに設定ツールが付属していない場合は、
 フリーソフトの"ASIO caps"などが使用可能です)

また、ASIOを使う場合は、更にサウンド出力デバイスを
CONFIGURATION/System/Sound Options/ASIODeviceで選択する必要が
あります。
(WASAPIとDirectSoundの場合は、OSの既定のサウンドデバイスを
 使うため、サウンド出力デバイスの選択は不要です。)
ここで存在しないデバイスなどが指定されている場合は、
DTXManiaが起動しないことがあります。

ASIOの設定が適切でない場合、DTXManiaはASIOで動作できず、
代わりにWASAPI(排他/共有)やDirectSoundで動作します。
