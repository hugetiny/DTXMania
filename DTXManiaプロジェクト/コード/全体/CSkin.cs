using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
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
					if ( this.rSound[ 0 ] == null || this.rSound[ 0 ].bストリーム再生する )
					{
						this.rSound[ 1 ] = null;
					}
					else
					{
						this.rSound[ 1 ] = (CSound) this.rSound[ 0 ].Clone();	// #27790 2012.3.10 yyagi add: to accelerate loading chip sounds
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

		public Cシステムサウンド bgmオプション画面;
		public Cシステムサウンド bgmコンフィグ画面;
		public Cシステムサウンド bgm起動画面;
		public Cシステムサウンド bgm選曲画面;
		public Cシステムサウンド soundSTAGEFAILED音;
		public Cシステムサウンド soundカーソル移動音;
		public Cシステムサウンド soundゲーム開始音;
		public Cシステムサウンド soundゲーム終了音;
		public Cシステムサウンド soundステージクリア音;
		public Cシステムサウンド soundタイトル音;
		public Cシステムサウンド soundフルコンボ音;
		public Cシステムサウンド sound歓声音;
		public Cシステムサウンド sound曲読込開始音;
		public Cシステムサウンド sound決定音;
		public Cシステムサウンド sound取消音;
		public Cシステムサウンド sound変更音;
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


		// コンストラクタ

		public CSkin()
		{
			Cシステムサウンド cシステムサウンド = new Cシステムサウンド();
			cシステムサウンド.strファイル名 = @"Sounds\Move.ogg";
			cシステムサウンド.bループ = false;
			cシステムサウンド.b排他 = false;
			cシステムサウンド.bCompact対象 = false;
			this.soundカーソル移動音 = cシステムサウンド;
			Cシステムサウンド cシステムサウンド2 = new Cシステムサウンド();
			cシステムサウンド2.strファイル名 = @"Sounds\Decide.ogg";
			cシステムサウンド2.bループ = false;
			cシステムサウンド2.b排他 = false;
			cシステムサウンド2.bCompact対象 = false;
			this.sound決定音 = cシステムサウンド2;
			Cシステムサウンド cシステムサウンド3 = new Cシステムサウンド();
			cシステムサウンド3.strファイル名 = @"Sounds\Change.ogg";
			cシステムサウンド3.bループ = false;
			cシステムサウンド3.b排他 = false;
			cシステムサウンド3.bCompact対象 = false;
			this.sound変更音 = cシステムサウンド3;
			Cシステムサウンド cシステムサウンド4 = new Cシステムサウンド();
			cシステムサウンド4.strファイル名 = @"Sounds\Cancel.ogg";
			cシステムサウンド4.bループ = false;
			cシステムサウンド4.b排他 = false;
			cシステムサウンド4.bCompact対象 = true;
			this.sound取消音 = cシステムサウンド4;
			Cシステムサウンド cシステムサウンド5 = new Cシステムサウンド();
			cシステムサウンド5.strファイル名 = @"Sounds\Audience.ogg";
			cシステムサウンド5.bループ = false;
			cシステムサウンド5.b排他 = false;
			cシステムサウンド5.bCompact対象 = true;
			this.sound歓声音 = cシステムサウンド5;
			Cシステムサウンド cシステムサウンド6 = new Cシステムサウンド();
			cシステムサウンド6.strファイル名 = @"Sounds\Stage failed.ogg";
			cシステムサウンド6.bループ = false;
			cシステムサウンド6.b排他 = true;
			cシステムサウンド6.bCompact対象 = true;
			this.soundSTAGEFAILED音 = cシステムサウンド6;
			Cシステムサウンド cシステムサウンド7 = new Cシステムサウンド();
			cシステムサウンド7.strファイル名 = @"Sounds\Game start.ogg";
			cシステムサウンド7.bループ = false;
			cシステムサウンド7.b排他 = false;
			cシステムサウンド7.bCompact対象 = false;
			this.soundゲーム開始音 = cシステムサウンド7;
			Cシステムサウンド cシステムサウンド8 = new Cシステムサウンド();
			cシステムサウンド8.strファイル名 = @"Sounds\Game end.ogg";
			cシステムサウンド8.bループ = false;
			cシステムサウンド8.b排他 = true;
			cシステムサウンド8.bCompact対象 = false;
			this.soundゲーム終了音 = cシステムサウンド8;
			Cシステムサウンド cシステムサウンド9 = new Cシステムサウンド();
			cシステムサウンド9.strファイル名 = @"Sounds\Stage clear.ogg";
			cシステムサウンド9.bループ = false;
			cシステムサウンド9.b排他 = true;
			cシステムサウンド9.bCompact対象 = true;
			this.soundステージクリア音 = cシステムサウンド9;
			Cシステムサウンド cシステムサウンド10 = new Cシステムサウンド();
			cシステムサウンド10.strファイル名 = @"Sounds\Full combo.ogg";
			cシステムサウンド10.bループ = false;
			cシステムサウンド10.b排他 = false;
			cシステムサウンド10.bCompact対象 = true;
			this.soundフルコンボ音 = cシステムサウンド10;
			Cシステムサウンド cシステムサウンド11 = new Cシステムサウンド();
			cシステムサウンド11.strファイル名 = @"Sounds\Now loading.ogg";
			cシステムサウンド11.bループ = false;
			cシステムサウンド11.b排他 = true;
			cシステムサウンド11.bCompact対象 = true;
			this.sound曲読込開始音 = cシステムサウンド11;
			Cシステムサウンド cシステムサウンド12 = new Cシステムサウンド();
			cシステムサウンド12.strファイル名 = @"Sounds\Title.ogg";
			cシステムサウンド12.bループ = false;
			cシステムサウンド12.b排他 = true;
			cシステムサウンド12.bCompact対象 = false;
			this.soundタイトル音 = cシステムサウンド12;
			Cシステムサウンド cシステムサウンド13 = new Cシステムサウンド();
			cシステムサウンド13.strファイル名 = @"Sounds\Setup BGM.ogg";
			cシステムサウンド13.bループ = true;
			cシステムサウンド13.b排他 = true;
			cシステムサウンド13.bCompact対象 = false;
			this.bgm起動画面 = cシステムサウンド13;
			Cシステムサウンド cシステムサウンド14 = new Cシステムサウンド();
			cシステムサウンド14.strファイル名 = @"Sounds\Option BGM.ogg";
			cシステムサウンド14.bループ = true;
			cシステムサウンド14.b排他 = true;
			cシステムサウンド14.bCompact対象 = false;
			this.bgmオプション画面 = cシステムサウンド14;
			Cシステムサウンド cシステムサウンド15 = new Cシステムサウンド();
			cシステムサウンド15.strファイル名 = @"Sounds\Config BGM.ogg";
			cシステムサウンド15.bループ = true;
			cシステムサウンド15.b排他 = true;
			cシステムサウンド15.bCompact対象 = false;
			this.bgmコンフィグ画面 = cシステムサウンド15;
			Cシステムサウンド cシステムサウンド16 = new Cシステムサウンド();
			cシステムサウンド16.strファイル名 = @"Sounds\Select BGM.ogg";
			cシステムサウンド16.bループ = true;
			cシステムサウンド16.b排他 = true;
			cシステムサウンド16.bCompact対象 = false;
			this.bgm選曲画面 = cシステムサウンド16;
		}


		// メソッド

		public static string Path( string strファイルの相対パス )
		{
			return ( CDTXMania.strEXEのあるフォルダ + @"System\" + strファイルの相対パス );
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
