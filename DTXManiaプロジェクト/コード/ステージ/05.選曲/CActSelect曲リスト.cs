using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using SlimDX;
using FDK;

namespace DTXMania
{
	internal class CActSelect曲リスト : CActivity
	{
		// プロパティ

		public bool bスクロール中
		{
			get
			{
				if( this.n目標のスクロールカウンタ == 0 )
				{
					return ( this.n現在のスクロールカウンタ != 0 );
				}
				return true;
			}
		}
		public int n現在のアンカ難易度レベル 
		{
			get;
			private set;
		}
		public int n現在選択中の曲の現在の難易度レベル
		{
			get
			{
				return this.n現在のアンカ難易度レベルに最も近い難易度レベルを返す( this.r現在選択中の曲 );
			}
		}
		public Cスコア r現在選択中のスコア
		{
			get
			{
				if( this.r現在選択中の曲 != null )
				{
					return this.r現在選択中の曲.arスコア[ this.n現在選択中の曲の現在の難易度レベル ];
				}
				return null;
			}
		}
		public C曲リストノード r現在選択中の曲 
		{
			get;
			private set;
		}


		// コンストラクタ

		public CActSelect曲リスト()
		{
			this.r現在選択中の曲 = null;
			this.n現在のアンカ難易度レベル = 0;
			base.b活性化してない = true;
		}


		// メソッド

		public int n現在のアンカ難易度レベルに最も近い難易度レベルを返す( C曲リストノード song )
		{
			if( song == null )
			{
				return this.n現在のアンカ難易度レベル;
			}
			if( ( song.eノード種別 == C曲リストノード.Eノード種別.BOX ) || ( song.eノード種別 == C曲リストノード.Eノード種別.BACKBOX ) )
			{
				return 0;
			}
			if( song.arスコア[ this.n現在のアンカ難易度レベル ] != null )
			{
				return this.n現在のアンカ難易度レベル;
			}
			int index = this.n現在のアンカ難易度レベル;
			int num2 = this.n現在のアンカ難易度レベル;
			for( int i = 0; i < 5; i++ )
			{
				if( song.arスコア[ index ] != null )
				{
					break;
				}
				index = ( index + 1 ) % 5;
			}
			if( index < num2 )
			{
				index = num2;
				for( int j = 0; j < 5; j++ )
				{
					if( song.arスコア[ index ] != null )
					{
						return index;
					}
					if( --index < 0 )
					{
						index = 4;
					}
				}
			}
			return index;
		}
		public C曲リストノード r指定された曲が存在するリストの先頭の曲( C曲リストノード song )
		{
			List<C曲リストノード> songList = GetSongListWithinMe( song );
			if ( songList == null )
			{
				return null;
			}
			else
			{
				return songList[ 0 ];
			}

		}
		public C曲リストノード r指定された曲が存在するリストの末尾の曲( C曲リストノード song )
		{
			List<C曲リストノード> songList = GetSongListWithinMe( song );
			if ( songList == null )
			{
				return null;
			}
			else
			{
				return songList[ songList.Count - 1 ];
			}
		}

		private List<C曲リストノード> GetSongListWithinMe( C曲リストノード song )
		{
			if ( song.r親ノード == null )					// root階層のノートだったら
			{
				return CDTXMania.Songs管理.list曲ルート;	// rootのリストを返す
			}
			else
			{
				if ( ( song.r親ノード.list子リスト != null ) && ( song.r親ノード.list子リスト.Count > 0 ) )
				{
					return song.r親ノード.list子リスト;
				}
				else
				{
					return null;
				}
			}
		}


		public delegate void DGSortFunc( List<C曲リストノード> songList, E楽器パート eInst, int order, params object[] p);
		/// <summary>
		/// 主にCSong管理.cs内にあるソート機能を、delegateで呼び出す。
		/// </summary>
		/// <param name="sf">ソート用に呼び出すメソッド</param>
		/// <param name="eInst">ソート基準とする楽器</param>
		/// <param name="order">-1=降順, 1=昇順</param>
		public void t曲リストのソート( DGSortFunc sf, E楽器パート eInst, int order, params object[] p )
		{
			List<C曲リストノード> songList = GetSongListWithinMe( this.r現在選択中の曲 );
			if ( songList == null )
			{
				// 何もしない;
			}
			else
			{
//				CDTXMania.Songs管理.t曲リストのソート3_演奏回数の多い順( songList, eInst, order );
				sf( songList, eInst, order, p );
//				this.r現在選択中の曲 = CDTXMania
				this.t現在選択中の曲を元に曲バーを再構成する();
			}
		}

		public void tBOXに入る()
		{
			if( ( this.r現在選択中の曲.list子リスト != null ) && ( this.r現在選択中の曲.list子リスト.Count > 0 ) )
			{
				this.r現在選択中の曲 = this.r現在選択中の曲.list子リスト[ 0 ];
				this.t現在選択中の曲を元に曲バーを再構成する();
			}
		}
		public void tBOXを出る()
		{
			if( this.r現在選択中の曲.r親ノード != null )
			{
				this.r現在選択中の曲 = this.r現在選択中の曲.r親ノード;
				this.t現在選択中の曲を元に曲バーを再構成する();
			}
		}
		public void t現在選択中の曲を元に曲バーを再構成する()
		{
			this.tバーの初期化();
			for( int i = 0; i < 13; i++ )
			{
				this.t曲名バーの生成( i, this.stバー情報[ i ].strタイトル文字列, this.stバー情報[ i ].col文字色 );
			}
		}
		public void t次に移動()
		{
			if( this.r現在選択中の曲 != null )
			{
				this.n目標のスクロールカウンタ += 100;
			}
		}
		public void t前に移動()
		{
			if( this.r現在選択中の曲 != null )
			{
				this.n目標のスクロールカウンタ -= 100;
			}
		}
		public void t難易度レベルをひとつ進める()
		{
			if( ( this.r現在選択中の曲 != null ) && ( this.r現在選択中の曲.nスコア数 > 1 ) )
			{
				this.n現在のアンカ難易度レベル = this.n現在のアンカ難易度レベルに最も近い難易度レベルを返す( this.r現在選択中の曲 );
				for( int i = 0; i < 5; i++ )
				{
					this.n現在のアンカ難易度レベル = ( this.n現在のアンカ難易度レベル + 1 ) % 5;
					if( this.r現在選択中の曲.arスコア[ this.n現在のアンカ難易度レベル ] != null )
					{
						break;
					}
				}
				C曲リストノード song = this.r現在選択中の曲;
				for( int j = 0; j < 5; j++ )
				{
					song = this.r前の曲( song );
				}
				for( int k = this.n現在の選択行 - 5; k < ( ( this.n現在の選択行 - 5 ) + 13 ); k++ )
				{
					int index = ( k + 13 ) % 13;
					for( int m = 0; m < 3; m++ )
					{
						this.stバー情報[ index ].nスキル値[ m ] = (int) song.arスコア[ this.n現在のアンカ難易度レベルに最も近い難易度レベルを返す( song ) ].譜面情報.最大スキル[ m ];
					}
					song = this.r次の曲( song );
				}
				CDTXMania.stage選曲.t選択曲変更通知();
			}
		}

		
		// CActivity 実装

		public override void On活性化()
		{
			this.e楽器パート = E楽器パート.DRUMS;
			this.b登場アニメ全部完了 = false;
			this.n目標のスクロールカウンタ = 0;
			this.n現在のスクロールカウンタ = 0;
			this.nスクロールタイマ = -1;
			FontStyle regular = FontStyle.Regular;
			if( CDTXMania.ConfigIni.b選曲リストフォントを斜体にする )
			{
				regular |= FontStyle.Italic;
			}
			if( CDTXMania.ConfigIni.b選曲リストフォントを太字にする )
			{
				regular |= FontStyle.Bold;
			}
			this.ft曲リスト用フォント = new Font( CDTXMania.ConfigIni.str選曲リストフォント, (float) ( CDTXMania.ConfigIni.n選曲リストフォントのサイズdot * 2 ), regular, GraphicsUnit.Pixel );
			if( ( this.r現在選択中の曲 == null ) && ( CDTXMania.Songs管理.list曲ルート.Count > 0 ) )
			{
				this.r現在選択中の曲 = CDTXMania.Songs管理.list曲ルート[ 0 ];
			}
			this.tバーの初期化();
			base.On活性化();
		}
		public override void On非活性化()
		{
			if( this.ft曲リスト用フォント != null )
			{
				this.ft曲リスト用フォント.Dispose();
				this.ft曲リスト用フォント = null;
			}
			for( int i = 0; i < 13; i++ )
			{
				this.ct登場アニメ用[ i ] = null;
			}
			base.On非活性化();
		}
		public override void OnManagedリソースの作成()
		{
			if( !base.b活性化してない )
			{
				this.tx曲名バー.Score = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenSelect bar score.png" ), false );
				this.tx曲名バー.Box = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenSelect bar box.png" ), false );
				this.tx曲名バー.Other = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenSelect bar other.png" ), false );
				this.tx選曲バー.Score = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenSelect bar score selected.png" ), false );
				this.tx選曲バー.Box = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenSelect bar box selected.png" ), false );
				this.tx選曲バー.Other = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenSelect bar other selected.png" ), false );
				this.txスキル数字 = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenSelect skill number on list.png" ), false );
				for( int i = 0; i < 13; i++ )
				{
					this.t曲名バーの生成( i, this.stバー情報[ i ].strタイトル文字列, this.stバー情報[ i ].col文字色 );
				}
				try
				{
					Bitmap image = new Bitmap( 300, 0x20 );
					Graphics graphics = Graphics.FromImage( image );
					graphics.DrawString( "Song not found...", this.ft曲リスト用フォント, Brushes.DarkGray, (float) 2f, (float) 2f );
					graphics.DrawString( "Song not found...", this.ft曲リスト用フォント, Brushes.White, (float) 0f, (float) 0f );
					graphics.Dispose();
					this.txSongNotFound = new CTexture( CDTXMania.app.Device, image, CDTXMania.TextureFormat );
					this.txSongNotFound.vc拡大縮小倍率 = new Vector3( 0.5f, 0.5f, 1f );
					image.Dispose();
				}
				catch( CTextureCreateFailedException )
				{
					Trace.TraceError( "SoungNotFoundテクスチャの作成に失敗しました。" );
					this.txSongNotFound = null;
				}
				base.OnManagedリソースの作成();
			}
		}
		public override void OnManagedリソースの解放()
		{
			if( !base.b活性化してない )
			{
				for( int i = 0; i < 13; i++ )
				{
					if( this.stバー情報[ i ].txタイトル名 != null )
					{
						this.stバー情報[ i ].txタイトル名.Dispose();
						this.stバー情報[ i ].txタイトル名 = null;
					}
				}

				if( this.txスキル数字 != null )
				{
					this.txスキル数字.Dispose();
					this.txスキル数字 = null;
				}
				if( this.txSongNotFound != null )
				{
					this.txSongNotFound.Dispose();
					this.txSongNotFound = null;
				}
				if( this.tx曲名バー.Score != null )
				{
					this.tx曲名バー.Score.Dispose();
					this.tx曲名バー.Score = null;
				}
				if( this.tx曲名バー.Box != null )
				{
					this.tx曲名バー.Box.Dispose();
					this.tx曲名バー.Box = null;
				}
				if( this.tx曲名バー.Other != null )
				{
					this.tx曲名バー.Other.Dispose();
					this.tx曲名バー.Other = null;
				}
				if( this.tx選曲バー.Score != null )
				{
					this.tx選曲バー.Score.Dispose();
					this.tx選曲バー.Score = null;
				}
				if( this.tx選曲バー.Box != null )
				{
					this.tx選曲バー.Box.Dispose();
					this.tx選曲バー.Box = null;
				}
				if( this.tx選曲バー.Other != null )
				{
					this.tx選曲バー.Other.Dispose();
					this.tx選曲バー.Other = null;
				}
				base.OnManagedリソースの解放();
			}
		}
		public override int On進行描画()
		{
			if( !base.b活性化してない )
			{
				if( base.b初めての進行描画 )
				{
					for( int i = 0; i < 13; i++ )
					{
						this.ct登場アニメ用[ i ] = new CCounter( -i * 10, 100, 3, CDTXMania.Timer );
					}
					this.nスクロールタイマ = CDTXMania.Timer.n現在時刻;
					CDTXMania.stage選曲.t選択曲変更通知();
					base.b初めての進行描画 = false;
				}
				if( ( this.r現在選択中の曲 == null ) && ( CDTXMania.Songs管理.list曲ルート.Count > 0 ) )
				{
					this.r現在選択中の曲 = CDTXMania.Songs管理.list曲ルート[ 0 ];
				}
				if( this.b登場アニメ全部完了 )
				{
					long num2 = CDTXMania.Timer.n現在時刻;
					if( num2 < this.nスクロールタイマ )
					{
						this.nスクロールタイマ = num2;
					}
					int num3 = 2;
					while( ( num2 - this.nスクロールタイマ ) >= num3 )
					{
						int num5;
						int num4 = Math.Abs( (int) ( this.n目標のスクロールカウンタ - this.n現在のスクロールカウンタ ) );
						if( num4 <= 100 )
						{
							num5 = 2;
						}
						else if( num4 <= 300 )
						{
							num5 = 3;
						}
						else if( num4 <= 500 )
						{
							num5 = 4;
						}
						else
						{
							num5 = 8;
						}
						if( this.n現在のスクロールカウンタ < this.n目標のスクロールカウンタ )
						{
							this.n現在のスクロールカウンタ += num5;
							if( this.n現在のスクロールカウンタ > this.n目標のスクロールカウンタ )
							{
								this.n現在のスクロールカウンタ = this.n目標のスクロールカウンタ;
							}
						}
						else if( this.n現在のスクロールカウンタ > this.n目標のスクロールカウンタ )
						{
							this.n現在のスクロールカウンタ -= num5;
							if( this.n現在のスクロールカウンタ < this.n目標のスクロールカウンタ )
							{
								this.n現在のスクロールカウンタ = this.n目標のスクロールカウンタ;
							}
						}
						if( this.n現在のスクロールカウンタ >= 100 )
						{
							this.r現在選択中の曲 = this.r次の曲( this.r現在選択中の曲 );
							this.n現在の選択行 = ( this.n現在の選択行 + 1 ) % 13;
							C曲リストノード song = this.r現在選択中の曲;
							for( int j = 0; j < 7; j++ )
							{
								song = this.r次の曲( song );
							}
							int index = ( this.n現在の選択行 + 7 ) % 13;
							this.stバー情報[ index ].strタイトル文字列 = song.strタイトル;
							this.stバー情報[ index ].col文字色 = song.col文字色;
							this.t曲名バーの生成( index, this.stバー情報[ index ].strタイトル文字列, this.stバー情報[ index ].col文字色 );
							C曲リストノード c曲リストノード2 = this.r現在選択中の曲;
							for( int k = 0; k < 5; k++ )
							{
								c曲リストノード2 = this.r前の曲( c曲リストノード2 );
							}
							for( int m = 0; m < 13; m++ )
							{
								int num10 = ( ( ( this.n現在の選択行 - 5 ) + m ) + 13 ) % 13;
								this.stバー情報[ num10 ].eバー種別 = this.e曲のバー種別を返す( c曲リストノード2 );
								c曲リストノード2 = this.r次の曲( c曲リストノード2 );
							}
							for( int n = 0; n < 3; n++ )
							{
								this.stバー情報[ index ].nスキル値[ n ] = (int) song.arスコア[ this.n現在のアンカ難易度レベルに最も近い難易度レベルを返す( song ) ].譜面情報.最大スキル[ n ];
							}
							this.n現在のスクロールカウンタ -= 100;
							this.n目標のスクロールカウンタ -= 100;
							if( this.n目標のスクロールカウンタ == 0 )
							{
								CDTXMania.stage選曲.t選択曲変更通知();
							}
						}
						else if( this.n現在のスクロールカウンタ <= -100 )
						{
							this.r現在選択中の曲 = this.r前の曲( this.r現在選択中の曲 );
							this.n現在の選択行 = ( ( this.n現在の選択行 - 1 ) + 13 ) % 13;
							C曲リストノード c曲リストノード3 = this.r現在選択中の曲;
							for( int num12 = 0; num12 < 5; num12++ )
							{
								c曲リストノード3 = this.r前の曲( c曲リストノード3 );
							}
							int num13 = ( ( this.n現在の選択行 - 5 ) + 13 ) % 13;
							this.stバー情報[ num13 ].strタイトル文字列 = c曲リストノード3.strタイトル;
							this.stバー情報[ num13 ].col文字色 = c曲リストノード3.col文字色;
							this.t曲名バーの生成( num13, this.stバー情報[ num13 ].strタイトル文字列, this.stバー情報[ num13 ].col文字色 );
							C曲リストノード c曲リストノード4 = this.r現在選択中の曲;
							for( int num14 = 0; num14 < 5; num14++ )
							{
								c曲リストノード4 = this.r前の曲( c曲リストノード4 );
							}
							for( int num15 = 0; num15 < 13; num15++ )
							{
								int num16 = ( ( ( this.n現在の選択行 - 5 ) + num15 ) + 13 ) % 13;
								this.stバー情報[ num16 ].eバー種別 = this.e曲のバー種別を返す( c曲リストノード4 );
								c曲リストノード4 = this.r次の曲( c曲リストノード4 );
							}
							for( int num17 = 0; num17 < 3; num17++ )
							{
								this.stバー情報[ num13 ].nスキル値[ num17 ] = (int) c曲リストノード3.arスコア[ this.n現在のアンカ難易度レベルに最も近い難易度レベルを返す( c曲リストノード3 ) ].譜面情報.最大スキル[ num17 ];
							}
							this.n現在のスクロールカウンタ += 100;
							this.n目標のスクロールカウンタ += 100;
							if( this.n目標のスクロールカウンタ == 0 )
							{
								CDTXMania.stage選曲.t選択曲変更通知();
							}
						}
						this.nスクロールタイマ += num3;
					}
				}
				else
				{
					for( int num18 = 0; num18 < 13; num18++ )
					{
						this.ct登場アニメ用[ num18 ].t進行();
						if( this.ct登場アニメ用[ num18 ].b終了値に達した )
						{
							this.ct登場アニメ用[ num18 ].t停止();
						}
					}
					this.b登場アニメ全部完了 = true;
					for( int num19 = 0; num19 < 13; num19++ )
					{
						if( this.ct登場アニメ用[ num19 ].b進行中 )
						{
							this.b登場アニメ全部完了 = false;
							break;
						}
					}
				}
				if( this.r現在選択中の曲 == null )
				{
					if( this.txSongNotFound != null )
					{
						this.txSongNotFound.t2D描画( CDTXMania.app.Device, 320, 200 );
					}
					return 0;
				}
				if( this.b登場アニメ全部完了 )
				{
					for( int num20 = 0; num20 < 13; num20++ )
					{
						if( ( ( num20 != 0 ) || ( this.n現在のスクロールカウンタ <= 0 ) ) && ( ( num20 != 12 ) || ( this.n現在のスクロールカウンタ >= 0 ) ) )
						{
							int num21 = ( ( ( this.n現在の選択行 - 5 ) + num20 ) + 13 ) % 13;
							int num22 = num20;
							int num23 = ( this.n現在のスクロールカウンタ <= 0 ) ? ( ( num20 + 1 ) % 13 ) : ( ( ( num20 - 1 ) + 13 ) % 13 );
							int x = this.ptバーの基本座標[ num22 ].X + ( (int) ( ( this.ptバーの基本座標[ num23 ].X - this.ptバーの基本座標[ num22 ].X ) * ( ( (double) Math.Abs( this.n現在のスクロールカウンタ ) ) / 100.0 ) ) );
							int y = this.ptバーの基本座標[ num22 ].Y + ( (int) ( ( this.ptバーの基本座標[ num23 ].Y - this.ptバーの基本座標[ num22 ].Y ) * ( ( (double) Math.Abs( this.n現在のスクロールカウンタ ) ) / 100.0 ) ) );
							if( ( num20 == 5 ) && ( this.n現在のスクロールカウンタ == 0 ) )
							{
								this.tバーの描画( 0xd8, 0xb5, this.stバー情報[ num21 ].eバー種別, true );
								if( this.stバー情報[ num21 ].txタイトル名 != null )
								{
									this.stバー情報[ num21 ].txタイトル名.t2D描画( CDTXMania.app.Device, 0x114, 0xc9 );
								}
								if( ( this.stバー情報[ num21 ].eバー種別 == Eバー種別.Score ) && ( this.e楽器パート != E楽器パート.UNKNOWN ) )
								{
									this.tスキル値の描画( 0xf4, 0xd3, this.stバー情報[ num21 ].nスキル値[ (int) this.e楽器パート ] );
								}
							}
							else
							{
								this.tバーの描画( x, y, this.stバー情報[ num21 ].eバー種別, false );
								if( this.stバー情報[ num21 ].txタイトル名 != null )
								{
									this.stバー情報[ num21 ].txタイトル名.t2D描画( CDTXMania.app.Device, x + 0x2c, y + 4 );
								}
								if( ( this.stバー情報[ num21 ].eバー種別 == Eバー種別.Score ) && ( this.e楽器パート != E楽器パート.UNKNOWN ) )
								{
									this.tスキル値の描画( x + 14, y + 14, this.stバー情報[ num21 ].nスキル値[ (int) this.e楽器パート ] );
								}
							}
						}
					}
				}
				else
				{
					for( int num26 = 0; num26 < 13; num26++ )
					{
						if( this.ct登場アニメ用[ num26 ].n現在の値 >= 0 )
						{
							double num27 = ( (double) this.ct登場アニメ用[ num26 ].n現在の値 ) / 100.0;
							double num28 = Math.Sin( Math.PI * 3 / 5 * num27 );
							int num29 = ( ( ( this.n現在の選択行 - 5 ) + num26 ) + 13 ) % 13;
							if( num26 == 5 )
							{
								int num30 = (int) ( 425.0 / Math.Sin( Math.PI * 3 / 5 ) );
								int num31 = 640 - ( (int) ( num30 * num28 ) );
								int num32 = 0xb5;
								this.tバーの描画( num31, num32, this.stバー情報[ num29 ].eバー種別, true );
								if( this.stバー情報[ num29 ].txタイトル名 != null )
								{
									this.stバー情報[ num29 ].txタイトル名.t2D描画( CDTXMania.app.Device, ( num31 + 0x2c ) + 0x10, ( num32 + 4 ) + 0x10 );
								}
								if( ( this.stバー情報[ num29 ].eバー種別 == Eバー種別.Score ) && ( this.e楽器パート != E楽器パート.UNKNOWN ) )
								{
									this.tスキル値の描画( num31 + 0x1c, num32 + 30, this.stバー情報[ num29 ].nスキル値[ (int) this.e楽器パート ] );
								}
							}
							else
							{
								int num33 = (int) ( ( (double) ( ( 640 - this.ptバーの基本座標[ num26 ].X ) + 1 ) ) / Math.Sin( Math.PI * 3 / 5 ) );
								int num34 = 640 - ( (int) ( num33 * num28 ) );
								int num35 = this.ptバーの基本座標[ num26 ].Y;
								this.tバーの描画( num34, num35, this.stバー情報[ num29 ].eバー種別, false );
								if( this.stバー情報[ num29 ].txタイトル名 != null )
								{
									this.stバー情報[ num29 ].txタイトル名.t2D描画( CDTXMania.app.Device, num34 + 0x2c, num35 + 4 );
								}
								if( ( this.stバー情報[ num29 ].eバー種別 == Eバー種別.Score ) && ( this.e楽器パート != E楽器パート.UNKNOWN ) )
								{
									this.tスキル値の描画( num34 + 14, num35 + 14, this.stバー情報[ num29 ].nスキル値[ (int) this.e楽器パート ] );
								}
							}
						}
					}
				}
			}
			return 0;
		}
		

		// その他

		#region [ private ]
		//-----------------
		private enum Eバー種別
		{
			Score,
			Box,
			Other
		}

		[StructLayout( LayoutKind.Sequential )]
		private struct STバー
		{
			public CTexture Score;
			public CTexture Box;
			public CTexture Other;
			public CTexture this[ int index ]
			{
				get
				{
					switch( index )
					{
						case 0:
							return this.Score;

						case 1:
							return this.Box;

						case 2:
							return this.Other;
					}
					throw new IndexOutOfRangeException();
				}
				set
				{
					switch( index )
					{
						case 0:
							this.Score = value;
							return;

						case 1:
							this.Box = value;
							return;

						case 2:
							this.Other = value;
							return;
					}
					throw new IndexOutOfRangeException();
				}
			}
		}
		[StructLayout( LayoutKind.Sequential )]
		private struct STバー情報
		{
			public CActSelect曲リスト.Eバー種別 eバー種別;
			public string strタイトル文字列;
			public CTexture txタイトル名;
			public STDGBVALUE<int> nスキル値;
			public Color col文字色;
		}
		[StructLayout( LayoutKind.Sequential )]
		private struct ST選曲バー
		{
			public CTexture Score;
			public CTexture Box;
			public CTexture Other;
			public CTexture this[ int index ]
			{
				get
				{
					switch( index )
					{
						case 0:
							return this.Score;

						case 1:
							return this.Box;

						case 2:
							return this.Other;
					}
					throw new IndexOutOfRangeException();
				}
				set
				{
					switch( index )
					{
						case 0:
							this.Score = value;
							return;

						case 1:
							this.Box = value;
							return;

						case 2:
							this.Other = value;
							return;
					}
					throw new IndexOutOfRangeException();
				}
			}
		}

		private bool b登場アニメ全部完了;
		private Color color文字影 = Color.FromArgb( 0x40, 10, 10, 10 );
		private CCounter[] ct登場アニメ用 = new CCounter[ 13 ];
		private E楽器パート e楽器パート;
		private Font ft曲リスト用フォント;
		private long nスクロールタイマ;
		private int n現在のスクロールカウンタ;
		private int n現在の選択行;
		private int n目標のスクロールカウンタ;
		private readonly Point[] ptバーの基本座標 = new Point[] { new Point( 0x162, 20 ), new Point( 0x139, 0x36 ), new Point( 0x121, 0x58 ), new Point( 0x111, 0x7a ), new Point( 0x108, 0x9c ), new Point( 0xe8, 0xc5 ), new Point( 0x112, 0x102 ), new Point( 0x121, 0x124 ), new Point( 0x138, 0x146 ), new Point( 0x157, 360 ), new Point( 0x18a, 0x18a ), new Point( 0x1f2, 0x1ac ), new Point( 640, 0x1ce ) };
		private STバー情報[] stバー情報 = new STバー情報[ 13 ];
		private CTexture txSongNotFound;
		private CTexture txスキル数字;
		private STバー tx曲名バー;
		private ST選曲バー tx選曲バー;

		private Eバー種別 e曲のバー種別を返す( C曲リストノード song )
		{
			if( song != null )
			{
				switch( song.eノード種別 )
				{
					case C曲リストノード.Eノード種別.SCORE:
					case C曲リストノード.Eノード種別.SCORE_MIDI:
						return Eバー種別.Score;

					case C曲リストノード.Eノード種別.BOX:
					case C曲リストノード.Eノード種別.BACKBOX:
						return Eバー種別.Box;
				}
			}
			return Eバー種別.Other;
		}
		private C曲リストノード r次の曲( C曲リストノード song )
		{
			if( song == null )
			{
				return null;
			}
			List<C曲リストノード> list = ( song.r親ノード != null ) ? song.r親ノード.list子リスト : CDTXMania.Songs管理.list曲ルート;
			int index = list.IndexOf( song );
			if( index < 0 )
			{
				return null;
			}
			if( index == ( list.Count - 1 ) )
			{
				return list[ 0 ];
			}
			return list[ index + 1 ];
		}
		private C曲リストノード r前の曲( C曲リストノード song )
		{
			if( song == null )
			{
				return null;
			}
			List<C曲リストノード> list = ( song.r親ノード != null ) ? song.r親ノード.list子リスト : CDTXMania.Songs管理.list曲ルート;
			int index = list.IndexOf( song );
			if( index < 0 )
			{
				return null;
			}
			if( index == 0 )
			{
				return list[ list.Count - 1 ];
			}
			return list[ index - 1 ];
		}
		private void tスキル値の描画( int x, int y, int nスキル値 )
		{
			if( ( nスキル値 > 0 ) && ( nスキル値 <= 100 ) )
			{
				int color = ( nスキル値 == 100 ) ? 3 : ( nスキル値 / 0x19 );
				int num2 = nスキル値 / 100;
				int num3 = ( nスキル値 % 100 ) / 10;
				int num4 = ( nスキル値 % 100 ) % 10;
				if( num2 > 0 )
				{
					this.tスキル値の描画・１桁描画( x, y, num2, color );
				}
				if( ( num2 != 0 ) || ( num3 != 0 ) )
				{
					this.tスキル値の描画・１桁描画( x + 7, y, num3, color );
				}
				this.tスキル値の描画・１桁描画( x + 14, y, num4, color );
			}
		}
		private void tスキル値の描画・１桁描画( int x, int y, int n数値, int color )
		{
			int num = ( n数値 % 5 ) * 9;
			int num2 = ( n数値 / 5 ) * 12;
			switch( color )
			{
				case 0:
					if( this.txスキル数字 == null )
					{
						break;
					}
					this.txスキル数字.t2D描画( CDTXMania.app.Device, x, y, new Rectangle( 0x2d + num, 0x18 + num2, 9, 12 ) );
					return;

				case 1:
					if( this.txスキル数字 == null )
					{
						break;
					}
					this.txスキル数字.t2D描画( CDTXMania.app.Device, x, y, new Rectangle( 0x2d + num, num2, 9, 12 ) );
					return;

				case 2:
					if( this.txスキル数字 == null )
					{
						break;
					}
					this.txスキル数字.t2D描画( CDTXMania.app.Device, x, y, new Rectangle( num, 0x18 + num2, 9, 12 ) );
					return;

				case 3:
					if( this.txスキル数字 != null )
					{
						this.txスキル数字.t2D描画( CDTXMania.app.Device, x, y, new Rectangle( num, num2, 9, 12 ) );
					}
					break;

				default:
					return;
			}
		}
		private void tバーの初期化()
		{
			C曲リストノード song = this.r現在選択中の曲;
			if( song != null )
			{
				for( int i = 0; i < 5; i++ )
				{
					song = this.r前の曲( song );
				}
				for( int j = 0; j < 13; j++ )
				{
					this.stバー情報[ j ].strタイトル文字列 = song.strタイトル;
					this.stバー情報[ j ].col文字色 = song.col文字色;
					this.stバー情報[ j ].eバー種別 = this.e曲のバー種別を返す( song );
					for( int k = 0; k < 3; k++ )
					{
						this.stバー情報[ j ].nスキル値[ k ] = (int) song.arスコア[ this.n現在のアンカ難易度レベルに最も近い難易度レベルを返す( song ) ].譜面情報.最大スキル[ k ];
					}
					song = this.r次の曲( song );
				}
				this.n現在の選択行 = 5;
			}
		}
		private void tバーの描画( int x, int y, Eバー種別 type, bool b選択曲 )
		{
			if ((x < SampleFramework.GameWindowSize.Width) && (y < SampleFramework.GameWindowSize.Height))
			{
				if( !b選択曲 )
				{
					if( this.tx曲名バー[ (int) type ] != null )
					{
						this.tx曲名バー[ (int) type ].t2D描画( CDTXMania.app.Device, x, y, new Rectangle( 0, 0, 0x40, 0x20 ) );
					}
					x += 0x40;
					Rectangle rectangle = new Rectangle( 0, 0x20, 0x40, 0x20 );
					while( x < 640 )
					{
						if( this.tx曲名バー[ (int) type ] != null )
						{
							this.tx曲名バー[ (int) type ].t2D描画( CDTXMania.app.Device, x, y, rectangle );
						}
						x += 0x40;
					}
				}
				else
				{
					if( this.tx選曲バー[ (int) type ] != null )
					{
						this.tx選曲バー[ (int) type ].t2D描画( CDTXMania.app.Device, x, y, new Rectangle( 0, 0, 0x80, 0x40 ) );
					}
					x += 0x80;
					Rectangle rectangle2 = new Rectangle( 0x40, 0, 0x40, 0x40 );
					while( x < 640 )
					{
						if( this.tx選曲バー[ (int) type ] != null )
						{
							this.tx選曲バー[ (int) type ].t2D描画( CDTXMania.app.Device, x, y, rectangle2 );
						}
						x += 0x40;
					}
				}
			}
		}
		private void t曲名バーの生成( int num, string str曲名, Color color )
		{
			if( ( num >= 0 ) && ( num <= 12 ) )
			{
				try
				{
					Bitmap image = new Bitmap( 1, 1 );
					Graphics graphics = Graphics.FromImage( image );
					graphics.PageUnit = GraphicsUnit.Pixel;
					SizeF ef = graphics.MeasureString( str曲名, this.ft曲リスト用フォント );
					image.Dispose();
					int num2 = 0x188;
					int num3 = 0x19;
					int num4 = ( ( (int) ef.Width ) + 2 ) / 2;
					if( num4 > ( CDTXMania.app.Device.Capabilities.MaxTextureWidth / 2 ) )
					{
						num4 = CDTXMania.app.Device.Capabilities.MaxTextureWidth / 2;
					}
					float x = ( num4 <= num2 ) ? 0.5f : ( ( ( (float) num2 ) / ( (float) num4 ) ) * 0.5f );
					Bitmap bitmap2 = new Bitmap( num4 * 2, num3 * 2, PixelFormat.Format32bppArgb );
					Graphics graphics2 = Graphics.FromImage( bitmap2 );
					float y = ( ( (float) bitmap2.Height ) / 2f ) - ( ( CDTXMania.ConfigIni.n選曲リストフォントのサイズdot * 2f ) / 2f );
					graphics2.DrawString( str曲名, this.ft曲リスト用フォント, new SolidBrush( this.color文字影 ), (float) 2f, (float) ( y + 2f ) );
					graphics2.DrawString( str曲名, this.ft曲リスト用フォント, new SolidBrush( color ), 0f, y );
					graphics2.Dispose();
					if( this.stバー情報[ num ].txタイトル名 != null )
					{
						this.stバー情報[ num ].txタイトル名.Dispose();
					}
					this.stバー情報[ num ].txタイトル名 = new CTexture( CDTXMania.app.Device, bitmap2, CDTXMania.TextureFormat );
					this.stバー情報[ num ].txタイトル名.vc拡大縮小倍率 = new Vector3( x, 0.5f, 1f );
					bitmap2.Dispose();
				}
				catch( CTextureCreateFailedException )
				{
					Trace.TraceError( "曲名テクスチャの作成に失敗しました。[{0}]", new object[] { str曲名 } );
					this.stバー情報[ num ].txタイトル名 = null;
				}
			}
		}
		//-----------------
		#endregion
	}
}
