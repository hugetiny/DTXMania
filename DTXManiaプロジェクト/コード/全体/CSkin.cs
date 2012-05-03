using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using FDK;

namespace DTXMania
{
	// グローバル定数

	public enum Eシステムサウンド
	{
		BGMオプション画面,
		BGMコンフィグ画面,
		BGM起動画面,
		BGM選曲画面,
		SOUNDステージ失敗音,
		SOUNDカーソル移動音,
		SOUNDゲーム開始音,
		SOUNDゲーム終了音,
		SOUNDステージクリア音,
		SOUNDタイトル音,
		SOUNDフルコンボ音,
		SOUND歓声音,
		SOUND曲読込開始音,
		SOUND決定音,
		SOUND取消音,
		SOUND変更音,
	}

	internal class CSkin : IDisposable
	{
		public static string PrefixSkinFolder = "";		// "SkinFiles.";

		// クラス

		public class Cシステムサウンド : IDisposable
		{
			// static フィールド

			public static CSkin.Cシステムサウンド r最後に再生した排他システムサウンド;

			// フィールド、プロパティ

			public bool bCompact対象;
			public bool bループ;
			public bool b読み込み成功;
			public bool b排他;
			public string strファイル名 = "";
			public bool b再生中
			{
				get
				{
					if( this.rSound[ 1 - this.n次に鳴るサウンド番号 ] == null )
						return false;

					return this.rSound[ 1 - this.n次に鳴るサウンド番号 ].b再生中;
				}
			}
			public int n位置・現在のサウンド
			{
				get
				{
					CSound sound = this.rSound[ 1 - this.n次に鳴るサウンド番号 ];
					if( sound == null )
						return 0;

					return sound.n位置;
				}
				set
				{
					CSound sound = this.rSound[ 1 - this.n次に鳴るサウンド番号 ];
					if( sound != null )
						sound.n位置 = value;
				}
			}
			public int n位置・次に鳴るサウンド
			{
				get
				{
					CSound sound = this.rSound[ this.n次に鳴るサウンド番号 ];
					if( sound == null )
						return 0;

					return sound.n位置;
				}
				set
				{
					CSound sound = this.rSound[ this.n次に鳴るサウンド番号 ];
					if( sound != null )
						sound.n位置 = value;
				}
			}
			public int n音量・現在のサウンド
			{
				get
				{
					CSound sound = this.rSound[ 1 - this.n次に鳴るサウンド番号 ];
					if( sound == null )
						return 0;

					return sound.n音量;
				}
				set
				{
					CSound sound = this.rSound[ 1 - this.n次に鳴るサウンド番号 ];
					if( sound != null )
						sound.n音量 = value;
				}
			}
			public int n音量・次に鳴るサウンド
			{
				get
				{
					CSound sound = this.rSound[ this.n次に鳴るサウンド番号 ];
					if( sound == null )
					{
						return 0;
					}
					return sound.n音量;
				}
				set
				{
					CSound sound = this.rSound[ this.n次に鳴るサウンド番号 ];
					if( sound != null )
					{
						sound.n音量 = value;
					}
				}
			}
			public int n長さ・現在のサウンド
			{
				get
				{
					CSound sound = this.rSound[ 1 - this.n次に鳴るサウンド番号 ];
					if( sound == null )
					{
						return 0;
					}
					return sound.n総演奏時間ms;
				}
			}
			public int n長さ・次に鳴るサウンド
			{
				get
				{
					CSound sound = this.rSound[ this.n次に鳴るサウンド番号 ];
					if( sound == null )
					{
						return 0;
					}
					return sound.n総演奏時間ms;
				}
			}


			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="strファイル名"></param>
			/// <param name="bループ"></param>
			/// <param name="b排他"></param>
			/// <param name="bCompact対象"></param>
			public Cシステムサウンド(string strファイル名, bool bループ, bool b排他, bool bCompact対象)
			{
				this.strファイル名 = strファイル名;
				this.bループ = bループ;
				this.b排他 = b排他;
				this.bCompact対象 = bCompact対象;
			}
			public Cシステムサウンド()
			{
			}
			

			// メソッド

			public void t読み込み()
			{
				this.b読み込み成功 = false;
				if( string.IsNullOrEmpty( this.strファイル名 ) )
					throw new InvalidOperationException( "ファイル名が無効です。" );

				if( !File.Exists( CSkin.Path( this.strファイル名 ) ) )
				{
					throw new FileNotFoundException( this.strファイル名 );
				}
//				for( int i = 0; i < 2; i++ )		// #27790 2012.3.10 yyagi 2回読み出しを、1回読みだし＋1回メモリコピーに変更
//				{
					try
					{
						this.rSound[ 0 ] = CDTXMania.Sound管理.tサウンドを生成する( CSkin.Path( this.strファイル名 ) );
					}
					catch
					{
						this.rSound[ 0 ] = null;
						throw;
					}
					if ( this.rSound[ 0 ] == null )	// #28243 2012.5.3 yyagi "this.rSound[ 0 ].bストリーム再生する"時もCloneするようにし、rSound[1]がnullにならないよう修正→rSound[1]の再生正常化
					{
						this.rSound[ 1 ] = null;
					}
					else
					{
						this.rSound[ 1 ] = ( CSound ) this.rSound[ 0 ].Clone();	// #27790 2012.3.10 yyagi add: to accelerate loading chip sounds
						CDTXMania.Sound管理.tサウンドを登録する( this.rSound[ 1 ] );	// #28243 2012.5.3 yyagi add (登録漏れによりストリーム再生処理が発生していなかった)
					}

//				}
				this.b読み込み成功 = true;
			}
			public void t再生する()
			{
				if( this.b排他 )
				{
					if( r最後に再生した排他システムサウンド != null )
						r最後に再生した排他システムサウンド.t停止する();

					r最後に再生した排他システムサウンド = this;
				}
				CSound sound = this.rSound[ this.n次に鳴るサウンド番号 ];
				if( sound != null )
					sound.t再生を開始する( this.bループ );

				this.n次に鳴るサウンド番号 = 1 - this.n次に鳴るサウンド番号;
			}
			public void t停止する()
			{
				if( this.rSound[ 0 ] != null )
					this.rSound[ 0 ].t再生を停止する();

				if( this.rSound[ 1 ] != null )
					this.rSound[ 1 ].t再生を停止する();

				if( r最後に再生した排他システムサウンド == this )
					r最後に再生した排他システムサウンド = null;
			}

			#region [ IDisposable 実装 ]
			//-----------------
			public void Dispose()
			{
				if( !this.bDisposed済み )
				{
					for( int i = 0; i < 2; i++ )
					{
						if( this.rSound[ i ] != null )
						{
							CDTXMania.Sound管理.tサウンドを破棄する( this.rSound[ i ] );
							this.rSound[ i ] = null;
						}
					}
					this.b読み込み成功 = false;
					this.bDisposed済み = true;
				}
			}
			//-----------------
			#endregion

			#region [ private ]
			//-----------------
			private bool bDisposed済み;
			private int n次に鳴るサウンド番号;
			private CSound[] rSound = new CSound[ 2 ];
			//-----------------
			#endregion
		}

	
		// プロパティ

		public Cシステムサウンド bgmオプション画面 = null;
		public Cシステムサウンド bgmコンフィグ画面 = null;
		public Cシステムサウンド bgm起動画面 = null;
		public Cシステムサウンド bgm選曲画面 = null;
		public Cシステムサウンド soundSTAGEFAILED音 = null;
		public Cシステムサウンド soundカーソル移動音 = null;
		public Cシステムサウンド soundゲーム開始音 = null;
		public Cシステムサウンド soundゲーム終了音 = null;
		public Cシステムサウンド soundステージクリア音 = null;
		public Cシステムサウンド soundタイトル音 = null;
		public Cシステムサウンド soundフルコンボ音 = null;
		public Cシステムサウンド sound歓声音 = null;
		public Cシステムサウンド sound曲読込開始音 = null;
		public Cシステムサウンド sound決定音 = null;
		public Cシステムサウンド sound取消音 = null;
		public Cシステムサウンド sound変更音 = null;
		public readonly int nシステムサウンド数 = 16;
		public Cシステムサウンド this[ Eシステムサウンド sound ]
		{
			get
			{
				switch( sound )
				{
					case Eシステムサウンド.SOUNDカーソル移動音:
						return this.soundカーソル移動音;

					case Eシステムサウンド.SOUND決定音:
						return this.sound決定音;

					case Eシステムサウンド.SOUND変更音:
						return this.sound変更音;

					case Eシステムサウンド.SOUND取消音:
						return this.sound取消音;

					case Eシステムサウンド.SOUND歓声音:
						return this.sound歓声音;

					case Eシステムサウンド.SOUNDステージ失敗音:
						return this.soundSTAGEFAILED音;

					case Eシステムサウンド.SOUNDゲーム開始音:
						return this.soundゲーム開始音;

					case Eシステムサウンド.SOUNDゲーム終了音:
						return this.soundゲーム終了音;

					case Eシステムサウンド.SOUNDステージクリア音:
						return this.soundステージクリア音;

					case Eシステムサウンド.SOUNDフルコンボ音:
						return this.soundフルコンボ音;

					case Eシステムサウンド.SOUND曲読込開始音:
						return this.sound曲読込開始音;

					case Eシステムサウンド.SOUNDタイトル音:
						return this.soundタイトル音;

					case Eシステムサウンド.BGM起動画面:
						return this.bgm起動画面;

					case Eシステムサウンド.BGMオプション画面:
						return this.bgmオプション画面;

					case Eシステムサウンド.BGMコンフィグ画面:
						return this.bgmコンフィグ画面;

					case Eシステムサウンド.BGM選曲画面:
						return this.bgm選曲画面;
				}
				throw new IndexOutOfRangeException();
			}
		}
		public Cシステムサウンド this[ int index ]
		{
			get
			{
				switch( index )
				{
					case 0:
						return this.soundカーソル移動音;

					case 1:
						return this.sound決定音;

					case 2:
						return this.sound変更音;

					case 3:
						return this.sound取消音;

					case 4:
						return this.sound歓声音;

					case 5:
						return this.soundSTAGEFAILED音;

					case 6:
						return this.soundゲーム開始音;

					case 7:
						return this.soundゲーム終了音;

					case 8:
						return this.soundステージクリア音;

					case 9:
						return this.soundフルコンボ音;

					case 10:
						return this.sound曲読込開始音;

					case 11:
						return this.soundタイトル音;

					case 12:
						return this.bgm起動画面;

					case 13:
						return this.bgmオプション画面;

					case 14:
						return this.bgmコンフィグ画面;

					case 15:
						return this.bgm選曲画面;
				}
				throw new IndexOutOfRangeException();
			}
		}

		public string[] strSkinSubfolders = null;		// List<string>だとignoreCaseな検索が面倒なので、配列に逃げる :-)
		public static string strSkinSubfolder = null;

		// コンストラクタ
		public CSkin( string _strSkinSubfolder )
		{
			strSkinSubfolder = _strSkinSubfolder;
			ReloadSkinPaths();
			PrepareReloadSkin();
		}
		public CSkin()
		{
			ReloadSkinPaths();
			PrepareReloadSkin();
		}

		/// <summary>
		/// Skin(Sounds)を再読込する準備をする(再生停止,Dispose,ファイル名再設定)。
		/// あらかじめstrSkinSubfolderを適切に設定しておくこと。
		/// その後、ReloadSkinPaths()を実行し、strSkinSubfolderの正当性を確認した上で、本メソッドを呼び出すこと。
		/// 本メソッド呼び出し後に、ReloadSkin()を実行することで、システムサウンドを読み込み直す。
		/// ReloadSkin()の内容は本メソッド内に含めないこと。起動時はReloadSkin()相当の処理をCEnumSongsで行っているため。
		/// </summary>
		public void PrepareReloadSkin()
		{
			Trace.TraceInformation( "SkinPath設定: {0}", strSkinSubfolder );

			for ( int i = 0; i < nシステムサウンド数; i++ )
			{
				if ( this[ i ] != null && this[i].b読み込み成功 )
				{
					this[ i ].t停止する();
					this[ i ].Dispose();
				}
			}
			this.soundカーソル移動音	= new Cシステムサウンド( @"Sounds\Move.ogg",			false, false, false );
			this.sound決定音			= new Cシステムサウンド( @"Sounds\Decide.ogg",			false, false, false );
			this.sound変更音			= new Cシステムサウンド( @"Sounds\Change.ogg",			false, false, false );
			this.sound取消音			= new Cシステムサウンド( @"Sounds\Cancel.ogg",			false, false, true  );
			this.sound歓声音			= new Cシステムサウンド( @"Sounds\Audience.ogg",		false, false, true  );
			this.soundSTAGEFAILED音		= new Cシステムサウンド( @"Sounds\Stage failed.ogg",	false, true,  true  );
			this.soundゲーム開始音		= new Cシステムサウンド( @"Sounds\Game start.ogg",		false, false, false );
			this.soundゲーム終了音		= new Cシステムサウンド( @"Sounds\Game end.ogg",		false, true,  false );
			this.soundステージクリア音	= new Cシステムサウンド( @"Sounds\Stage clear.ogg",		false, true,  true  );
			this.soundフルコンボ音		= new Cシステムサウンド( @"Sounds\Full combo.ogg",		false, false, true  );
			this.sound曲読込開始音		= new Cシステムサウンド( @"Sounds\Now loading.ogg",		false, true,  true  );
			this.soundタイトル音		= new Cシステムサウンド( @"Sounds\Title.ogg",			false, true,  false );
			this.bgm起動画面			= new Cシステムサウンド( @"Sounds\Setup BGM.ogg",		true,  true,  false );
			this.bgmオプション画面		= new Cシステムサウンド( @"Sounds\Option BGM.ogg",		true,  true,  false );
			this.bgmコンフィグ画面		= new Cシステムサウンド( @"Sounds\Config BGM.ogg",		true,  true,  false );
			this.bgm選曲画面			= new Cシステムサウンド( @"Sounds\Select BGM.ogg",		true,  true,  false );
		}

		public void ReloadSkin()
		{
			for ( int i = 0; i < nシステムサウンド数; i++ )
			{
				Cシステムサウンド cシステムサウンド = this[ i ];
				if ( !CDTXMania.bコンパクトモード || cシステムサウンド.bCompact対象 )
				{
					try
					{
						cシステムサウンド.t読み込み();
						Trace.TraceInformation( "システムサウンドを読み込みました。({0})", cシステムサウンド.strファイル名 );
					}
					catch ( FileNotFoundException )
					{
						Trace.TraceWarning( "システムサウンドが存在しません。({0})", cシステムサウンド.strファイル名 );
					}
					catch ( Exception e )
					{
						Trace.TraceError( e.Message );
						Trace.TraceWarning( "システムサウンドの読み込みに失敗しました。({0})", cシステムサウンド.strファイル名 );
					}
				}
			}
		}


		/// <summary>
		/// Skinの一覧を再取得する。
		/// System/SkinFiles.*****/Graphics (やSounds/) というフォルダ構成を想定している。
		/// もし再取得の結果、現在使用中のSkinのパス(strSkinSubfloder)が消えていた場合は、
		/// 以下の優先順位で存在確認の上strSkinSubfolderを再設定する。
		/// 1. System/SkinFiles.Default/
		/// 2. System/SkinFiles.*****/ で最初にenumerateされたもの
 		/// 3. System/ (従来互換)
		/// </summary>
		public void ReloadSkinPaths()
		{
			string path;
			#region [ まず System/SkinFiles.*** をenumerateする ]
			path = System.IO.Path.Combine( CDTXMania.strEXEのあるフォルダ, "System" );
			string[] tempSkinSubfolders = System.IO.Directory.GetDirectories( path, PrefixSkinFolder + "*" );
			strSkinSubfolders = new string[ tempSkinSubfolders.Length ];
			int size = 0;
			for ( int i = 0; i < strSkinSubfolders.Length; i++ )
			{
				#region [ 検出したフォルダがスキンフォルダかどうか確認する]
				string filePathTitle;
				filePathTitle = System.IO.Path.Combine( tempSkinSubfolders[i], @"Graphics\ScreenTitle background.jpg" );
				if ( !File.Exists( filePathTitle ) )
					continue;
				#endregion
				#region [ スキンフォルダと確認できたものを、strSKinSubfoldersに入れる ]
				string[] spl = tempSkinSubfolders[ i ].Split( System.IO.Path.DirectorySeparatorChar );
				strSkinSubfolders[ size++ ] = spl[ spl.Length - 1 ];		// subfolder名から、～～/System/ までの部分を削除
				Trace.TraceInformation( "SkinPath検出: {0}", strSkinSubfolders[ i ] );
				#endregion
			}
			Array.Resize( ref strSkinSubfolders, size );
			Array.Sort( strSkinSubfolders );	// BinarySearch実行前にSortが必要
			#endregion

			#region [ 次に、カレントのSkinパスが存在するか調べる。あれば終了。]
			if ( Array.BinarySearch( strSkinSubfolders, strSkinSubfolder, StringComparer.InvariantCultureIgnoreCase ) >= 0 )
				return;
			#endregion
			#region [ カレントのSkinパスが消滅しているので、再設定する。]
			/// 以下の優先順位で現在使用中のSkinパスを再設定する。
			/// 1. System/SkinFiles.Default/
			/// 2. System/SkinFiles.*****/ で最初にenumerateされたもの
			/// 3. System/ (従来互換)
			#region [ System/SkinFiles.Default/ があるなら、そこにカレントSkinパスを設定する]
			if ( Array.BinarySearch( strSkinSubfolders, PrefixSkinFolder + "Default", StringComparer.InvariantCultureIgnoreCase ) >= 0 )
			{
				strSkinSubfolder = PrefixSkinFolder + "Default";
				return;
			}
			#endregion
			#region [ System/SkinFiles.*****/ で最初にenumerateされたものを、カレントSkinパスに再設定する ]
			if ( strSkinSubfolders.Length > 0 )
			{
				strSkinSubfolder = strSkinSubfolders[ 0 ];
				return;
			}
			#endregion
			#region [ System/ に、カレントSkinパスを再設定する。]
			strSkinSubfolder = "";			// ""にすることで、結果的に System/ 相当の相対パスになる。
			#endregion
			#endregion
		}

		// メソッド

		public static string Path( string strファイルの相対パス )
		{
			string path;
			path = System.IO.Path.Combine( CDTXMania.strEXEのあるフォルダ, "System" );
			path = System.IO.Path.Combine( path, strSkinSubfolder );
			path = System.IO.Path.Combine( path, strファイルの相対パス );
			return path;
		}
		
		#region [ IDisposable 実装 ]
		//-----------------
		public void Dispose()
		{
			if( !this.bDisposed済み )
			{
				for( int i = 0; i < this.nシステムサウンド数; i++ )
					this[ i ].Dispose();

				this.bDisposed済み = true;
			}
		}
		//-----------------
		#endregion


		// その他

		#region [ private ]
		//-----------------
		private bool bDisposed済み;
		//-----------------
		#endregion

	}
}
