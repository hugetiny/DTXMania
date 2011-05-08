using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.IO;
using FDK;

namespace DTXMania
{
	internal class CStage起動 : CStage
	{
		// コンストラクタ

		public CStage起動()
		{
			base.eステージID = CStage.Eステージ.起動;
			base.b活性化してない = true;
		}


		// CStage 実装

		public override void On活性化()
		{
			Trace.TraceInformation( "起動ステージを活性化します。" );
			Trace.Indent();
			try
			{
				this.list進行文字列 = new List<string>();
				base.eフェーズID = CStage.Eフェーズ.共通_通常状態;
				base.On活性化();
				Trace.TraceInformation( "起動ステージの活性化を完了しました。" );
			}
			finally
			{
				Trace.Unindent();
			}
		}
		public override void On非活性化()
		{
			Trace.TraceInformation( "起動ステージを非活性化します。" );
			Trace.Indent();
			try
			{
				this.list進行文字列 = null;
				if( ( this.thリスト構築 != null ) && this.thリスト構築.IsAlive )
				{
					Trace.TraceWarning( "リスト構築スレッドを強制停止します。" );
					this.thリスト構築.Abort();
					this.thリスト構築.Join();
				}
				base.On非活性化();
				Trace.TraceInformation( "起動ステージの非活性化を完了しました。" );
			}
			finally
			{
				Trace.Unindent();
			}
		}
		public override void OnManagedリソースの作成()
		{
			if( !base.b活性化してない )
			{
				this.tx背景 = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenSetup background.jpg" ), false );
				base.OnManagedリソースの作成();
			}
		}
		public override void OnManagedリソースの解放()
		{
			if( !base.b活性化してない )
			{
				CDTXMania.tテクスチャの解放( ref this.tx背景 );
				base.OnManagedリソースの解放();
			}
		}
		public override int On進行描画()
		{
			if( !base.b活性化してない )
			{
				if( base.b初めての進行描画 )
				{
					this.list進行文字列.Add( "DTXMania powered by YAMAHA Silent Session Drums\n" );
					this.list進行文字列.Add( "Release: " + CDTXMania.VERSION + " [" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() + "]" );
					this.thリスト構築 = new Thread( new ThreadStart( this.t曲リストの構築 ) );
					this.thリスト構築.Name = "曲リストの構築";
					this.thリスト構築.IsBackground = true;
					this.thリスト構築.Start();
					base.b初めての進行描画 = false;
					return 0;
				}

				CSongs管理 s管理 = CDTXMania.Songs管理;

				if( this.tx背景 != null )
					this.tx背景.t2D描画( CDTXMania.app.Device, 0, 0 );

				#region [ this.str現在進行中 の決定 ]
				//-----------------
				switch( base.eフェーズID )
				{
					case CStage.Eフェーズ.起動0_システムサウンドを構築:
						this.str現在進行中 = "Loading system sounds ... ";
						break;

					case CStage.Eフェーズ.起動1_SongsDBからスコアキャッシュを構築:
						this.str現在進行中 = "Loading songs.db ... ";
						break;

					case CStage.Eフェーズ.起動2_曲を検索してリストを作成する:
						this.str現在進行中 = string.Format( "{0} ... {1}", "Enumerating songs", s管理.n検索されたスコア数 );
						break;

					case CStage.Eフェーズ.起動3_スコアキャッシュをリストに反映する:
						this.str現在進行中 = string.Format( "{0} ... {1}/{2}", "Loading score properties from songs.db", CDTXMania.Songs管理.nスコアキャッシュから反映できたスコア数, s管理.n検索されたスコア数 );
						break;

					case CStage.Eフェーズ.起動4_スコアキャッシュになかった曲をファイルから読み込んで反映する:
						this.str現在進行中 = string.Format( "{0} ... {1}/{2}", "Loading score properties from files", CDTXMania.Songs管理.nファイルから反映できたスコア数, CDTXMania.Songs管理.n検索されたスコア数 - s管理.nスコアキャッシュから反映できたスコア数 );
						break;

					case CStage.Eフェーズ.起動5_曲リストへ後処理を適用する:
						this.str現在進行中 = string.Format( "{0} ... ", "Building songlists" );
						break;

					case CStage.Eフェーズ.起動6_スコアキャッシュをSongsDBに出力する:
						this.str現在進行中 = string.Format( "{0} ... ", "Saving songs.db" );
						break;

					case CStage.Eフェーズ.起動7_完了:
						this.str現在進行中 = "Setup done.";
						break;
				}
				//-----------------
				#endregion
				#region [ this.list進行文字列＋this.現在進行中 の表示 ]
				//-----------------
				lock( this.list進行文字列 )
				{
					int x = 0;
					int y = 0;
					foreach( string str in this.list進行文字列 )
					{
						CDTXMania.act文字コンソール.tPrint( x, y, C文字コンソール.Eフォント種別.灰細, str );
						y += 14;
					}
					CDTXMania.act文字コンソール.tPrint( x, y, C文字コンソール.Eフォント種別.灰細, this.str現在進行中 );
				}
				//-----------------
				#endregion

				if( !this.thリスト構築.IsAlive )
				{
					return 1;
				}
			}
			return 0;
		}


		// その他

		#region [ private ]
		//-----------------
		private List<string> list進行文字列;
		private const string MSG進行0 = "Loading system sounds";
		private const string MSG進行1 = "Loading songs.db";
		private const string MSG進行2 = "Enumerating songs";
		private const string MSG進行3 = "Loading score properties from songs.db";
		private const string MSG進行4 = "Loading score properties from files";
		private const string MSG進行5 = "Loading score properties from socre.ini";
		private const string MSG進行6 = "Building songlists";
		private const string MSG進行7 = "Saving songs.db";
		private string str現在進行中 = "";
		private Thread thリスト構築;
		private CTexture tx背景;

		private void t曲リストの構築()
		{
			// ！注意！
			// 本メソッドは別スレッドで動作するが、プラグイン側でカレントディレクトリを変更しても大丈夫なように、
			// すべてのファイルアクセスは「絶対パス」で行うこと。(2010.9.16)

			DateTime now = DateTime.Now;
			string str = CDTXMania.strEXEのあるフォルダ + "songs.db";

			try
			{
				#region [ 0) システムサウンドの構築  ]
				//-----------------------------
				base.eフェーズID = CStage.Eフェーズ.起動0_システムサウンドを構築;

				Trace.TraceInformation( "0) システムサウンドを構築します。" );
				Trace.Indent();

				try
				{
					for( int i = 0; i < CDTXMania.Skin.nシステムサウンド数; i++ )
					{
						CSkin.Cシステムサウンド cシステムサウンド = CDTXMania.Skin[ i ];
						if( !CDTXMania.bコンパクトモード || cシステムサウンド.bCompact対象 )
						{
							try
							{
								cシステムサウンド.t読み込み();
								Trace.TraceInformation( "システムサウンドを読み込みました。({0})", new object[] { cシステムサウンド.strファイル名 } );
								if( ( cシステムサウンド == CDTXMania.Skin.bgm起動画面 ) && cシステムサウンド.b読み込み成功 )
								{
									cシステムサウンド.t再生する();
								}
							}
							catch( FileNotFoundException )
							{
								Trace.TraceWarning( "システムサウンドが存在しません。({0})", new object[] { cシステムサウンド.strファイル名 } );
							}
							catch( Exception exception )
							{
								Trace.TraceError( exception.Message );
								Trace.TraceWarning( "システムサウンドの読み込みに失敗しました。({0})", new object[] { cシステムサウンド.strファイル名 } );
							}
						}
					}
					lock( this.list進行文字列 )
					{
						this.list進行文字列.Add( "Loading system sounds ... OK " );
					}
				}
				finally
				{
					Trace.Unindent();
				}
				//-----------------------------
				#endregion

				if( CDTXMania.bコンパクトモード )
				{
					Trace.TraceInformation( "コンパクトモードなので残りの起動処理は省略します。" );
					return;
				}

				#region [ 1) songs.db の読み込み ]
				//-----------------------------
				base.eフェーズID = CStage.Eフェーズ.起動1_SongsDBからスコアキャッシュを構築;

				Trace.TraceInformation( "1) songs.db を読み込みます。" );
				Trace.Indent();

				try
				{
					if( !CDTXMania.ConfigIni.bConfigIniがないかDTXManiaのバージョンが異なる )
					{
						try
						{
							CDTXMania.Songs管理.tSongsDBを読み込む( str );
						}
						catch
						{
							Trace.TraceError( "songs.db の読み込みに失敗しました。" );
						}
						Trace.TraceInformation( "songs.db の読み込みを完了しました。[{0}スコア]", new object[] { CDTXMania.Songs管理.nSongsDBから取得できたスコア数 } );
						lock( this.list進行文字列 )
						{
							this.list進行文字列.Add( "Loading songs.db ... OK" );
						}
					}
					else
					{
						Trace.TraceInformation( "初回の起動であるかまたはDTXManiaのバージョンが上がったため、songs.db の読み込みをスキップします。" );
						lock( this.list進行文字列 )
						{
							this.list進行文字列.Add( "Loading songs.db ... Skip" );
						}
					}
				}
				finally
				{
					Trace.Unindent();
				}
				//-----------------------------
				#endregion
				#region [ 2) 曲データの検索 ]
				//-----------------------------
				base.eフェーズID = CStage.Eフェーズ.起動2_曲を検索してリストを作成する;

				Trace.TraceInformation( "2) 曲データを検索します。" );
				Trace.Indent();

				try
				{
					if( !string.IsNullOrEmpty( CDTXMania.ConfigIni.str曲データ検索パス ) )
					{
						string[] strArray = CDTXMania.ConfigIni.str曲データ検索パス.Split( new char[] { ';' } );
						if( strArray.Length > 0 )
						{
							// 全パスについて…
							foreach( string str2 in strArray )
							{
								string path = str2;
								if( !Path.IsPathRooted( path ) )
								{
									path = CDTXMania.strEXEのあるフォルダ + str2;	// 相対パスの場合、絶対パスに直す(2010.9.16)
								}

								if( !string.IsNullOrEmpty( path ) )
								{
									Trace.TraceInformation( "検索パス: " + path );
									Trace.Indent();

									try
									{
										CDTXMania.Songs管理.t曲を検索してリストを作成する( path, true );
									}
									catch( Exception exception2 )
									{
										Trace.TraceError( exception2.Message );
										Trace.TraceError( exception2.StackTrace );
										Trace.TraceError( "例外が発生しましたが処理を継続します。" );
									}
									finally
									{
										Trace.Unindent();
									}
								}
							}
						}
					}
					else
					{
						Trace.TraceWarning( "曲データの検索パス(DTXPath)の指定がありません。" );
					}
				}
				finally
				{
					Trace.TraceInformation( "曲データの検索を完了しました。[{0}曲{1}スコア]", new object[] { CDTXMania.Songs管理.n検索された曲ノード数, CDTXMania.Songs管理.n検索されたスコア数 } );
					Trace.Unindent();
				}
				lock( this.list進行文字列 )
				{
					this.list進行文字列.Add( string.Format( "{0} ... {1} scores ({2} songs)", "Enumerating songs", CDTXMania.Songs管理.n検索されたスコア数, CDTXMania.Songs管理.n検索された曲ノード数 ) );
				}
				//-----------------------------
				#endregion
				#region [ 3) songs.db 情報の曲リストへの反映 ]
				//-----------------------------
				base.eフェーズID = CStage.Eフェーズ.起動3_スコアキャッシュをリストに反映する;

				Trace.TraceInformation( "3) songs.db の情報を曲リストへ反映します。" );
				Trace.Indent();

				try
				{
					CDTXMania.Songs管理.tスコアキャッシュを曲リストに反映する();
				}
				catch( Exception exception3 )
				{
					Trace.TraceError( exception3.Message );
					Trace.TraceError( exception3.StackTrace );
					Trace.TraceError( "例外が発生しましたが処理を継続します。" );
				}
				finally
				{
					Trace.TraceInformation( "曲リストへの反映を完了しました。[{0}/{1}スコア]", new object[] { CDTXMania.Songs管理.nスコアキャッシュから反映できたスコア数, CDTXMania.Songs管理.n検索されたスコア数 } );
					Trace.Unindent();
				}
				lock( this.list進行文字列 )
				{
					this.list進行文字列.Add( string.Format( "{0} ... {1}/{2}", "Loading score properties from songs.db", CDTXMania.Songs管理.nスコアキャッシュから反映できたスコア数, CDTXMania.Songs管理.n検索されたスコア数 ) );
				}
				//-----------------------------
				#endregion
				#region [ 4) songs.db になかった曲データをファイルから読み込んで反映 ]
				//-----------------------------
				base.eフェーズID = CStage.Eフェーズ.起動4_スコアキャッシュになかった曲をファイルから読み込んで反映する;

				int num2 = CDTXMania.Songs管理.n検索されたスコア数 - CDTXMania.Songs管理.nスコアキャッシュから反映できたスコア数;

				Trace.TraceInformation( "4) songs.db になかった曲データ[{0}スコア]の情報をファイルから読み込んで反映します。", new object[] { num2 } );
				Trace.Indent();

				try
				{
					CDTXMania.Songs管理.tSongsDBになかった曲をファイルから読み込んで反映する();
				}
				catch( Exception exception4 )
				{
					Trace.TraceError( exception4.Message );
					Trace.TraceError( exception4.StackTrace );
					Trace.TraceError( "例外が発生しましたが処理を継続します。" );
				}
				finally
				{
					Trace.TraceInformation( "曲データへの反映を完了しました。[{0}/{1}スコア]", new object[] { CDTXMania.Songs管理.nファイルから反映できたスコア数, num2 } );
					Trace.Unindent();
				}
				lock( this.list進行文字列 )
				{
					this.list進行文字列.Add( string.Format( "{0} ... {1}/{2}", "Loading score properties from files", CDTXMania.Songs管理.nファイルから反映できたスコア数, CDTXMania.Songs管理.n検索されたスコア数 - CDTXMania.Songs管理.nスコアキャッシュから反映できたスコア数 ) );
				}
				//-----------------------------
				#endregion
				#region [ 5) 曲リストへの後処理の適用 ]
				//-----------------------------
				base.eフェーズID = CStage.Eフェーズ.起動5_曲リストへ後処理を適用する;

				Trace.TraceInformation( "5) 曲リストへの後処理を適用します。" );
				Trace.Indent();

				try
				{
					CDTXMania.Songs管理.t曲リストへ後処理を適用する();
				}
				catch( Exception exception5 )
				{
					Trace.TraceError( exception5.Message );
					Trace.TraceError( exception5.StackTrace );
					Trace.TraceError( "例外が発生しましたが処理を継続します。" );
				}
				finally
				{
					Trace.TraceInformation( "曲リストへの後処理を完了しました。" );
					Trace.Unindent();
				}
				lock( this.list進行文字列 )
				{
					this.list進行文字列.Add( string.Format( "{0} ... OK", "Building songlists" ) );
				}
				//-----------------------------
				#endregion
				#region [ 6) songs.db への保存 ]
				//-----------------------------
				base.eフェーズID = CStage.Eフェーズ.起動6_スコアキャッシュをSongsDBに出力する;

				Trace.TraceInformation( "6) 曲データの情報を songs.db へ出力します。" );
				Trace.Indent();

				try
				{
					CDTXMania.Songs管理.tスコアキャッシュをSongsDBに出力する( str );
				}
				catch( Exception exception6 )
				{
					Trace.TraceError( exception6.Message );
					Trace.TraceError( exception6.StackTrace );
					Trace.TraceError( "例外が発生しましたが処理を継続します。" );
				}
				finally
				{
					Trace.TraceInformation( "songs.db への出力を完了しました。[{0}スコア]", new object[] { CDTXMania.Songs管理.nSongsDBへ出力できたスコア数 } );
					Trace.Unindent();
				}
				lock( this.list進行文字列 )
				{
					this.list進行文字列.Add( string.Format( "{0} ... OK", "Saving songs.db" ) );
				}
				//-----------------------------
				#endregion
			}
			finally
			{
				base.eフェーズID = CStage.Eフェーズ.起動7_完了;
				TimeSpan span = (TimeSpan) ( DateTime.Now - now );
				Trace.TraceInformation( "起動所要時間: {0}", new object[] { span.ToString() } );
			}
		}
		//-----------------
		#endregion
	}
}
