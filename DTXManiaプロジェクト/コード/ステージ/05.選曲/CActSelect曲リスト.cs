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
			// 事前チェック。

			if( song == null )
				return this.n現在のアンカ難易度レベル;	// 曲がまったくないよ

			if( song.arスコア[ this.n現在のアンカ難易度レベル ] != null )
				return this.n現在のアンカ難易度レベル;	// 難易度ぴったりの曲があったよ

			if( ( song.eノード種別 == C曲リストノード.Eノード種別.BOX ) || ( song.eノード種別 == C曲リストノード.Eノード種別.BACKBOX ) )
				return 0;								// BOX と BACKBOX は関係無いよ


			// 現在のアンカレベルから、難易度上向きに検索開始。

			int n最も近いレベル = this.n現在のアンカ難易度レベル;

			for( int i = 0; i < 5; i++ )
			{
				if( song.arスコア[ n最も近いレベル ] != null )
					break;	// 曲があった。

				n最も近いレベル = ( n最も近いレベル + 1 ) % 5;	// 曲がなかったので次の難易度レベルへGo。（5以上になったら0に戻る。）
			}


			// 見つかった曲がアンカより下のレベルだった場合……
			// アンカから下向きに検索すれば、もっとアンカに近い曲があるんじゃね？

			if( n最も近いレベル < this.n現在のアンカ難易度レベル )
			{
				// 現在のアンカレベルから、難易度下向きに検索開始。

				n最も近いレベル = this.n現在のアンカ難易度レベル;

				for( int i = 0; i < 5; i++ )
				{
					if( song.arスコア[ n最も近いレベル ] != null )
						break;	// 曲があった。

					n最も近いレベル = ( ( n最も近いレベル - 1 ) + 5 ) % 5;	// 曲がなかったので次の難易度レベルへGo。（0未満になったら4に戻る。）
				}
			}

			return n最も近いレベル;
		}
		public C曲リストノード r指定された曲が存在するリストの先頭の曲( C曲リストノード song )
		{
			List<C曲リストノード> songList = GetSongListWithinMe( song );
			return ( songList == null ) ? null : songList[ 0 ];
		}
		public C曲リストノード r指定された曲が存在するリストの末尾の曲( C曲リストノード song )
		{
			List<C曲リストノード> songList = GetSongListWithinMe( song );
			return ( songList == null ) ? null : songList[ songList.Count - 1 ];
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
			if( ( this.r現在選択中の曲 == null ) || ( this.r現在選択中の曲.nスコア数 <= 1 ) )
				return;		// 曲にスコアが０～１個しかないなら進める意味なし。
			

			// 難易度レベルを＋１し、現在選曲中のスコアを変更する。

			this.n現在のアンカ難易度レベル = this.n現在のアンカ難易度レベルに最も近い難易度レベルを返す( this.r現在選択中の曲 );

			for( int i = 0; i < 5; i++ )
			{
				this.n現在のアンカ難易度レベル = ( this.n現在のアンカ難易度レベル + 1 ) % 5;	// ５以上になったら０に戻る。
				if( this.r現在選択中の曲.arスコア[ this.n現在のアンカ難易度レベル ] != null )	// 曲が存在してるならここで終了。存在してないなら次のレベルへGo。
					break;
			}


			// 曲毎に表示しているスキル値を、新しい難易度レベルに合わせて取得し直す。（表示されている13曲全部。）

			C曲リストノード song = this.r現在選択中の曲;
			for( int i = 0; i < 5; i++ )
				song = this.r前の曲( song );

			for( int i = this.n現在の選択行 - 5; i < ( ( this.n現在の選択行 - 5 ) + 13 ); i++ )
			{
				int index = ( i + 13 ) % 13;
				for( int m = 0; m < 3; m++ )
				{
					this.stバー情報[ index ].nスキル値[ m ] = (int) song.arスコア[ this.n現在のアンカ難易度レベルに最も近い難易度レベルを返す( song ) ].譜面情報.最大スキル[ m ];
				}
				song = this.r次の曲( song );
			}


			// 選曲ステージに変更通知を発出し、関係Activityの対応を行ってもらう。

			CDTXMania.stage選曲.t選択曲変更通知();
		}

		
		// CActivity 実装

		public override void On活性化()
		{
			if( this.b活性化してる )
				return;

			this.e楽器パート = E楽器パート.DRUMS;
			this.b登場アニメ全部完了 = false;
			this.n目標のスクロールカウンタ = 0;
			this.n現在のスクロールカウンタ = 0;
			this.nスクロールタイマ = -1;

			// フォント作成。
			// 曲リスト文字は２倍（面積４倍）でテクスチャに描画してから縮小表示するので、フォントサイズは２倍とする。

			FontStyle regular = FontStyle.Regular;
			if( CDTXMania.ConfigIni.b選曲リストフォントを斜体にする ) regular |= FontStyle.Italic;
			if( CDTXMania.ConfigIni.b選曲リストフォントを太字にする ) regular |= FontStyle.Bold;
			this.ft曲リスト用フォント = new Font( CDTXMania.ConfigIni.str選曲リストフォント, (float) ( CDTXMania.ConfigIni.n選曲リストフォントのサイズdot * 2 ), regular, GraphicsUnit.Pixel );
			

			// 現在選択中の曲がない（＝はじめての活性化）なら、現在選択中の曲をルートの先頭ノードに設定する。

			if( ( this.r現在選択中の曲 == null ) && ( CDTXMania.Songs管理.list曲ルート.Count > 0 ) )
				this.r現在選択中の曲 = CDTXMania.Songs管理.list曲ルート[ 0 ];


			// バー情報を初期化する。

			this.tバーの初期化();

			base.On活性化();
		}
		public override void On非活性化()
		{
			if( this.b活性化してない )
				return;

			CDTXMania.t安全にDisposeする( ref this.ft曲リスト用フォント );

			for( int i = 0; i < 13; i++ )
				this.ct登場アニメ用[ i ] = null;

			base.On非活性化();
		}
		public override void OnManagedリソースの作成()
		{
			if( this.b活性化してない )
				return;

			this.tx曲名バー.Score = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenSelect bar score.png" ), false );
			this.tx曲名バー.Box = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenSelect bar box.png" ), false );
			this.tx曲名バー.Other = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenSelect bar other.png" ), false );
			this.tx選曲バー.Score = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenSelect bar score selected.png" ), false );
			this.tx選曲バー.Box = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenSelect bar box selected.png" ), false );
			this.tx選曲バー.Other = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenSelect bar other selected.png" ), false );
			this.txスキル数字 = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenSelect skill number on list.png" ), false );
			for( int i = 0; i < 13; i++ )
				this.t曲名バーの生成( i, this.stバー情報[ i ].strタイトル文字列, this.stバー情報[ i ].col文字色 );

			try
			{
				using( Bitmap image = new Bitmap( 300, 32 ) )
				using( Graphics graphics = Graphics.FromImage( image ) )
				{
					graphics.DrawString( "Song not found...", this.ft曲リスト用フォント, Brushes.DarkGray, (float) 2f, (float) 2f );
					graphics.DrawString( "Song not found...", this.ft曲リスト用フォント, Brushes.White, (float) 0f, (float) 0f );
					this.txSongNotFound = new CTexture( CDTXMania.app.Device, image, CDTXMania.TextureFormat );

					this.txSongNotFound.vc拡大縮小倍率 = new Vector3( 0.5f, 0.5f, 1f );	// 半分のサイズで表示する。
				}
			}
			catch( CTextureCreateFailedException )
			{
				Trace.TraceError( "SoungNotFoundテクスチャの作成に失敗しました。" );
				this.txSongNotFound = null;
			}

			base.OnManagedリソースの作成();
		}
		public override void OnManagedリソースの解放()
		{
			if( this.b活性化してない )
				return;

			for( int i = 0; i < 13; i++ )
				CDTXMania.t安全にDisposeする( ref this.stバー情報[ i ].txタイトル名 );

			CDTXMania.t安全にDisposeする( ref this.txスキル数字 );
			CDTXMania.t安全にDisposeする( ref this.txSongNotFound );
			CDTXMania.t安全にDisposeする( ref this.tx曲名バー.Score );
			CDTXMania.t安全にDisposeする( ref this.tx曲名バー.Box );
			CDTXMania.t安全にDisposeする( ref this.tx曲名バー.Other );
			CDTXMania.t安全にDisposeする( ref this.tx選曲バー.Score );
			CDTXMania.t安全にDisposeする( ref this.tx選曲バー.Box );
			CDTXMania.t安全にDisposeする( ref this.tx選曲バー.Other );

			base.OnManagedリソースの解放();
		}
		public override int On進行描画()
		{
			if( this.b活性化してない )
				return 0;

			#region [ 初めての進行描画 ]
			//-----------------
			if( this.b初めての進行描画 )
			{
				for( int i = 0; i < 13; i++ )
					this.ct登場アニメ用[ i ] = new CCounter( -i * 10, 100, 3, CDTXMania.Timer );

				this.nスクロールタイマ = CDTXMania.Timer.n現在時刻;
				CDTXMania.stage選曲.t選択曲変更通知();
				
				base.b初めての進行描画 = false;
			}
			//-----------------
			#endregion

			
			// まだ選択中の曲が決まってなければ、曲ツリールートの最初の曲にセットする。

			if( ( this.r現在選択中の曲 == null ) && ( CDTXMania.Songs管理.list曲ルート.Count > 0 ) )
				this.r現在選択中の曲 = CDTXMania.Songs管理.list曲ルート[ 0 ];


			// 本ステージは、(1)登場アニメフェーズ → (2)通常フェーズ　と二段階にわけて進む。
			// ２つしかフェーズがないので CStage.eフェーズID を使ってないところがまた本末転倒。

			
			// 進行。

			if( !this.b登場アニメ全部完了 )
			{
				#region [ (1) 登場アニメフェーズの進行。]
				//-----------------
				for( int i = 0; i < 13; i++ )	// パネルは全13枚。
				{
					this.ct登場アニメ用[ i ].t進行();

					if( this.ct登場アニメ用[ i ].b終了値に達した )
						this.ct登場アニメ用[ i ].t停止();
				}

				// 全部の進行が終わったら、this.b登場アニメ全部完了 を true にする。

				this.b登場アニメ全部完了 = true;
				for( int i = 0; i < 13; i++ )	// パネルは全13枚。
				{
					if( this.ct登場アニメ用[ i ].b進行中 )
					{
						this.b登場アニメ全部完了 = false;	// まだ進行中のアニメがあるなら false のまま。
						break;
					}
				}
				//-----------------
				#endregion
			}
			else
			{
				#region [ (2) 通常フェーズの進行。]
				//-----------------
				long n現在時刻 = CDTXMania.Timer.n現在時刻;
				
				if( n現在時刻 < this.nスクロールタイマ )	// 念のため
					this.nスクロールタイマ = n現在時刻;

				const int nアニメ間隔 = 2;
				while( ( n現在時刻 - this.nスクロールタイマ ) >= nアニメ間隔 )
				{
					int n加速度 = 1;
					int n残距離 = Math.Abs( (int) ( this.n目標のスクロールカウンタ - this.n現在のスクロールカウンタ ) );

					#region [ 残距離が遠いほどスクロールを速くする（＝n加速度を多くする）。]
					//-----------------
					if( n残距離 <= 100 )
					{
						n加速度 = 2;
					}
					else if( n残距離 <= 300 )
					{
						n加速度 = 3;
					}
					else if( n残距離 <= 500 )
					{
						n加速度 = 4;
					}
					else
					{
						n加速度 = 8;
					}
					//-----------------
					#endregion

					#region [ 加速度を加算し、現在のスクロールカウンタを目標のスクロールカウンタまで近づける。 ]
					//-----------------
					if( this.n現在のスクロールカウンタ < this.n目標のスクロールカウンタ )		// (A) 正の方向に未達の場合：
					{
						this.n現在のスクロールカウンタ += n加速度;								// カウンタを正方向に移動する。

						if( this.n現在のスクロールカウンタ > this.n目標のスクロールカウンタ )
							this.n現在のスクロールカウンタ = this.n目標のスクロールカウンタ;	// 到着！スクロール停止！
					}

					else if( this.n現在のスクロールカウンタ > this.n目標のスクロールカウンタ )	// (B) 負の方向に未達の場合：
					{
						this.n現在のスクロールカウンタ -= n加速度;								// カウンタを負方向に移動する。

						if( this.n現在のスクロールカウンタ < this.n目標のスクロールカウンタ )	// 到着！スクロール停止！
							this.n現在のスクロールカウンタ = this.n目標のスクロールカウンタ;
					}
					//-----------------
					#endregion

					if( this.n現在のスクロールカウンタ >= 100 )		// １行＝100カウント。
					{
						#region [ パネルを１行上にシフトする。]
						//-----------------

						// 選択曲と選択行を１つ下の行に移動。

						this.r現在選択中の曲 = this.r次の曲( this.r現在選択中の曲 );
						this.n現在の選択行 = ( this.n現在の選択行 + 1 ) % 13;


						// 選択曲から７つ下のパネル（＝新しく最下部に表示されるパネル。消えてしまう一番上のパネルを再利用する）に、新しい曲の情報を記載する。

						C曲リストノード song = this.r現在選択中の曲;
						for( int i = 0; i < 7; i++ )
							song = this.r次の曲( song );

						int index = ( this.n現在の選択行 + 7 ) % 13;	// 新しく最下部に表示されるパネルのインデックス（0～12）。
						this.stバー情報[ index ].strタイトル文字列 = song.strタイトル;
						this.stバー情報[ index ].col文字色 = song.col文字色;
						this.t曲名バーの生成( index, this.stバー情報[ index ].strタイトル文字列, this.stバー情報[ index ].col文字色 );


						// stバー情報[] の内容を1行ずつずらす。
						
						C曲リストノード song2 = this.r現在選択中の曲;
						for( int i = 0; i < 5; i++ )
							song2 = this.r前の曲( song2 );

						for( int i = 0; i < 13; i++ )
						{
							int n = ( ( ( this.n現在の選択行 - 5 ) + i ) + 13 ) % 13;
							this.stバー情報[ n ].eバー種別 = this.e曲のバー種別を返す( song2 );
							song2 = this.r次の曲( song2 );
						}

						
						// 新しく最下部に表示されるパネル用のスキル値を取得。

						for( int i = 0; i < 3; i++ )
							this.stバー情報[ index ].nスキル値[ i ] = (int) song.arスコア[ this.n現在のアンカ難易度レベルに最も近い難易度レベルを返す( song ) ].譜面情報.最大スキル[ i ];


						// 1行(100カウント)移動完了。

						this.n現在のスクロールカウンタ -= 100;
						this.n目標のスクロールカウンタ -= 100;
						
						if( this.n目標のスクロールカウンタ == 0 )
							CDTXMania.stage選曲.t選択曲変更通知();		// スクロール完了＝選択曲変更！

						//-----------------
						#endregion
					}
					else if( this.n現在のスクロールカウンタ <= -100 )
					{
						#region [ パネルを１行下にシフトする。]
						//-----------------

						// 選択曲と選択行を１つ上の行に移動。

						this.r現在選択中の曲 = this.r前の曲( this.r現在選択中の曲 );
						this.n現在の選択行 = ( ( this.n現在の選択行 - 1 ) + 13 ) % 13;


						// 選択曲から５つ上のパネル（＝新しく最上部に表示されるパネル。消えてしまう一番下のパネルを再利用する）に、新しい曲の情報を記載する。

						C曲リストノード song = this.r現在選択中の曲;
						for( int i = 0; i < 5; i++ )
							song = this.r前の曲( song );

						int index = ( ( this.n現在の選択行 - 5 ) + 13 ) % 13;	// 新しく最上部に表示されるパネルのインデックス（0～12）。
						this.stバー情報[ index ].strタイトル文字列 = song.strタイトル;
						this.stバー情報[ index ].col文字色 = song.col文字色;
						this.t曲名バーの生成( index, this.stバー情報[ index ].strタイトル文字列, this.stバー情報[ index ].col文字色 );


						// stバー情報[] の内容を1行ずつずらす。
						
						C曲リストノード song2 = this.r現在選択中の曲;
						for( int i = 0; i < 5; i++ )
							song2 = this.r前の曲( song2 );

						for( int i = 0; i < 13; i++ )
						{
							int n = ( ( ( this.n現在の選択行 - 5 ) + i ) + 13 ) % 13;
							this.stバー情報[ n ].eバー種別 = this.e曲のバー種別を返す( song2 );
							song2 = this.r次の曲( song2 );
						}

		
						// 新しく最上部に表示されるパネル用のスキル値を取得。
						
						for( int i = 0; i < 3; i++ )
							this.stバー情報[ index ].nスキル値[ i ] = (int) song.arスコア[ this.n現在のアンカ難易度レベルに最も近い難易度レベルを返す( song ) ].譜面情報.最大スキル[ i ];


						// 1行(100カウント)移動完了。

						this.n現在のスクロールカウンタ += 100;
						this.n目標のスクロールカウンタ += 100;

						if( this.n目標のスクロールカウンタ == 0 )
							CDTXMania.stage選曲.t選択曲変更通知();		// スクロール完了＝選択曲変更！
						//-----------------
						#endregion
					}

					this.nスクロールタイマ += nアニメ間隔;
				}
				//-----------------
				#endregion
			}


			// 描画。

			if( this.r現在選択中の曲 == null )
			{
				#region [ 曲が１つもないなら「Songs not found.」を表示してここで帰れ。]
				//-----------------
				if( this.txSongNotFound != null )
					this.txSongNotFound.t2D描画( CDTXMania.app.Device, 320, 200 );
				//-----------------
				#endregion

				return 0;
			}

			if( !this.b登場アニメ全部完了 )
			{
				#region [ (1) 登場アニメフェーズの描画。]
				//-----------------
				for( int i = 0; i < 13; i++ )	// パネルは全13枚。
				{
					if( this.ct登場アニメ用[ i ].n現在の値 >= 0 )
					{
						double db割合0to1 = ( (double) this.ct登場アニメ用[ i ].n現在の値 ) / 100.0;
						double db回転率 = Math.Sin( Math.PI * 3 / 5 * db割合0to1 );
						int nパネル番号 = ( ( ( this.n現在の選択行 - 5 ) + i ) + 13 ) % 13;
						
						if( i == 5 )
						{
							// (A) 選択曲パネルを描画。

							#region [ バーテクスチャを描画。]
							//-----------------
							int width = (int) ( 425.0 / Math.Sin( Math.PI * 3 / 5 ) );
							int x = 640 - ( (int) ( width * db回転率 ) );
							int y = 181;
							this.tバーの描画( x, y, this.stバー情報[ nパネル番号 ].eバー種別, true );
							//-----------------
							#endregion
							#region [ タイトル名テクスチャを描画。]
							//-----------------
							if( this.stバー情報[ nパネル番号 ].txタイトル名 != null )
								this.stバー情報[ nパネル番号 ].txタイトル名.t2D描画( CDTXMania.app.Device, ( x + 44 ) + 16, ( y + 4 ) + 16 );
							//-----------------
							#endregion
							#region [ スキル値を描画。]
							//-----------------
							if( ( this.stバー情報[ nパネル番号 ].eバー種別 == Eバー種別.Score ) && ( this.e楽器パート != E楽器パート.UNKNOWN ) )
								this.tスキル値の描画( x + 28, y + 30, this.stバー情報[ nパネル番号 ].nスキル値[ (int) this.e楽器パート ] );
							//-----------------
							#endregion
						}
						else
						{
							// (B) その他のパネルの描画。

							#region [ バーテクスチャの描画。]
							//-----------------
							int width = (int) ( ( (double) ( ( 640 - this.ptバーの基本座標[ i ].X ) + 1 ) ) / Math.Sin( Math.PI * 3 / 5 ) );
							int x = 640 - ( (int) ( width * db回転率 ) );
							int y = this.ptバーの基本座標[ i ].Y;
							this.tバーの描画( x, y, this.stバー情報[ nパネル番号 ].eバー種別, false );
							//-----------------
							#endregion
							#region [ タイトル名テクスチャを描画。]
							//-----------------
							if( this.stバー情報[ nパネル番号 ].txタイトル名 != null )
								this.stバー情報[ nパネル番号 ].txタイトル名.t2D描画( CDTXMania.app.Device, x + 44, y + 4 );
							//-----------------
							#endregion
							#region [ スキル値を描画。]
							//-----------------
							if( ( this.stバー情報[ nパネル番号 ].eバー種別 == Eバー種別.Score ) && ( this.e楽器パート != E楽器パート.UNKNOWN ) )
								this.tスキル値の描画( x + 14, y + 14, this.stバー情報[ nパネル番号 ].nスキル値[ (int) this.e楽器パート ] );
							//-----------------
							#endregion
						}
					}
				}
				//-----------------
				#endregion
			}
			else
			{
				#region [ (2) 通常フェーズの描画。]
				//-----------------
				for( int i = 0; i < 13; i++ )	// パネルは全13枚。
				{
					if( ( i == 0 && this.n現在のスクロールカウンタ > 0 ) ||		// 最上行は、上に移動中なら表示しない。
						( i == 12 && this.n現在のスクロールカウンタ < 0 ) )		// 最下行は、下に移動中なら表示しない。
						continue;

					int nパネル番号 = ( ( ( this.n現在の選択行 - 5 ) + i ) + 13 ) % 13;
					int n見た目の行番号 = i;
					int n次のパネル番号 = ( this.n現在のスクロールカウンタ <= 0 ) ? ( ( i + 1 ) % 13 ) : ( ( ( i - 1 ) + 13 ) % 13 );
					int x = this.ptバーの基本座標[ n見た目の行番号 ].X + ( (int) ( ( this.ptバーの基本座標[ n次のパネル番号 ].X - this.ptバーの基本座標[ n見た目の行番号 ].X ) * ( ( (double) Math.Abs( this.n現在のスクロールカウンタ ) ) / 100.0 ) ) );
					int y = this.ptバーの基本座標[ n見た目の行番号 ].Y + ( (int) ( ( this.ptバーの基本座標[ n次のパネル番号 ].Y - this.ptバーの基本座標[ n見た目の行番号 ].Y ) * ( ( (double) Math.Abs( this.n現在のスクロールカウンタ ) ) / 100.0 ) ) );
		
					if( ( i == 5 ) && ( this.n現在のスクロールカウンタ == 0 ) )
					{
						// (A) スクロールが停止しているときの選択曲バーの描画。

						#region [ バーテクスチャを描画。]
						//-----------------
						this.tバーの描画( 216, 181, this.stバー情報[ nパネル番号 ].eバー種別, true );
						//-----------------
						#endregion
						#region [ タイトル名テクスチャを描画。]
						//-----------------
						if( this.stバー情報[ nパネル番号 ].txタイトル名 != null )
							this.stバー情報[ nパネル番号 ].txタイトル名.t2D描画( CDTXMania.app.Device, 0x114, 0xc9 );
						//-----------------
						#endregion
						#region [ スキル値を描画。]
						//-----------------
						if( ( this.stバー情報[ nパネル番号 ].eバー種別 == Eバー種別.Score ) && ( this.e楽器パート != E楽器パート.UNKNOWN ) )
							this.tスキル値の描画( 0xf4, 0xd3, this.stバー情報[ nパネル番号 ].nスキル値[ (int) this.e楽器パート ] );
						//-----------------
						#endregion
					}
					else
					{
						// (B) スクロール中の選択曲バー、またはその他のバーの描画。

						#region [ バーテクスチャを描画。]
						//-----------------
						this.tバーの描画( x, y, this.stバー情報[ nパネル番号 ].eバー種別, false );
						//-----------------
						#endregion
						#region [ タイトル名テクスチャを描画。]
						//-----------------
						if( this.stバー情報[ nパネル番号 ].txタイトル名 != null )
							this.stバー情報[ nパネル番号 ].txタイトル名.t2D描画( CDTXMania.app.Device, x + 44, y + 4 );
						//-----------------
						#endregion
						#region [ スキル値を描画。]
						//-----------------
						if( ( this.stバー情報[ nパネル番号 ].eバー種別 == Eバー種別.Score ) && ( this.e楽器パート != E楽器パート.UNKNOWN ) )
							this.tスキル値の描画( x + 14, y + 14, this.stバー情報[ nパネル番号 ].nスキル値[ (int) this.e楽器パート ] );
						//-----------------
						#endregion
					}
				}
				//-----------------
				#endregion
			}

			return 0;
		}
		

		// その他

		#region [ private ]
		//-----------------
		private enum Eバー種別 { Score, Box, Other }

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

		private struct STバー情報
		{
			public CActSelect曲リスト.Eバー種別 eバー種別;
			public string strタイトル文字列;
			public CTexture txタイトル名;
			public STDGBVALUE<int> nスキル値;
			public Color col文字色;
		}

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
				return null;

			List<C曲リストノード> list = ( song.r親ノード != null ) ? song.r親ノード.list子リスト : CDTXMania.Songs管理.list曲ルート;
	
			int index = list.IndexOf( song );

			if( index < 0 )
				return null;

			if( index == ( list.Count - 1 ) )
				return list[ 0 ];

			return list[ index + 1 ];
		}
		private C曲リストノード r前の曲( C曲リストノード song )
		{
			if( song == null )
				return null;

			List<C曲リストノード> list = ( song.r親ノード != null ) ? song.r親ノード.list子リスト : CDTXMania.Songs管理.list曲ルート;

			int index = list.IndexOf( song );
	
			if( index < 0 )
				return null;

			if( index == 0 )
				return list[ list.Count - 1 ];

			return list[ index - 1 ];
		}
		private void tスキル値の描画( int x, int y, int nスキル値 )
		{
			if( nスキル値 <= 0 || nスキル値 > 100 )		// スキル値 0 ＝ 未プレイ なので表示しない。
				return;

			int color = ( nスキル値 == 100 ) ? 3 : ( nスキル値 / 25 );

			int n百の位 = nスキル値 / 100;
			int n十の位 = ( nスキル値 % 100 ) / 10;
			int n一の位 = ( nスキル値 % 100 ) % 10;


			// 百の位の描画。

			if( n百の位 > 0 )
				this.tスキル値の描画・１桁描画( x, y, n百の位, color );


			// 十の位の描画。

			if( n百の位 != 0 || n十の位 != 0 )
				this.tスキル値の描画・１桁描画( x + 7, y, n十の位, color );


			// 一の位の描画。

			this.tスキル値の描画・１桁描画( x + 14, y, n一の位, color );
		}
		private void tスキル値の描画・１桁描画( int x, int y, int n数値, int color )
		{
			int dx = ( n数値 % 5 ) * 9;
			int dy = ( n数値 / 5 ) * 12;
			
			switch( color )
			{
				case 0:
					if( this.txスキル数字 != null )
						this.txスキル数字.t2D描画( CDTXMania.app.Device, x, y, new Rectangle( 45 + dx, 24 + dy, 9, 12 ) );
					break;

				case 1:
					if( this.txスキル数字 != null )
						this.txスキル数字.t2D描画( CDTXMania.app.Device, x, y, new Rectangle( 45 + dx, dy, 9, 12 ) );
					break;

				case 2:
					if( this.txスキル数字 != null )
						this.txスキル数字.t2D描画( CDTXMania.app.Device, x, y, new Rectangle( dx, 24 + dy, 9, 12 ) );
					break;

				case 3:
					if( this.txスキル数字 != null )
						this.txスキル数字.t2D描画( CDTXMania.app.Device, x, y, new Rectangle( dx, dy, 9, 12 ) );
					break;
			}
		}
		private void tバーの初期化()
		{
			C曲リストノード song = this.r現在選択中の曲;
			
			if( song == null )
				return;

			for( int i = 0; i < 5; i++ )
				song = this.r前の曲( song );

			for( int i = 0; i < 13; i++ )
			{
				this.stバー情報[ i ].strタイトル文字列 = song.strタイトル;
				this.stバー情報[ i ].col文字色 = song.col文字色;
				this.stバー情報[ i ].eバー種別 = this.e曲のバー種別を返す( song );
				
				for( int j = 0; j < 3; j++ )
					this.stバー情報[ i ].nスキル値[ j ] = (int) song.arスコア[ this.n現在のアンカ難易度レベルに最も近い難易度レベルを返す( song ) ].譜面情報.最大スキル[ j ];

				song = this.r次の曲( song );
			}

			this.n現在の選択行 = 5;
		}
		private void tバーの描画( int x, int y, Eバー種別 type, bool b選択曲 )
		{
			if( x >= SampleFramework.GameWindowSize.Width || y >= SampleFramework.GameWindowSize.Height )
				return;

			if( b選択曲 )
			{
				#region [ (A) 選択曲の場合 ]
				//-----------------
				if( this.tx選曲バー[ (int) type ] != null )
					this.tx選曲バー[ (int) type ].t2D描画( CDTXMania.app.Device, x, y, new Rectangle( 0, 0, 128, 64 ) );	// ヘサキ
				x += 128;

				var rc = new Rectangle( 64, 0, 64, 64 );
				while( x < 640 )
				{
					if( this.tx選曲バー[ (int) type ] != null )
						this.tx選曲バー[ (int) type ].t2D描画( CDTXMania.app.Device, x, y, rc );	// 胴体；64pxずつ横につなげていく。
					x += 64;
				}
				//-----------------
				#endregion
			}
			else
			{
				#region [ (B) その他の場合 ]
				//-----------------
				if( this.tx曲名バー[ (int) type ] != null )
					this.tx曲名バー[ (int) type ].t2D描画( CDTXMania.app.Device, x, y, new Rectangle( 0, 0, 64, 32 ) );		// ヘサキ
				x += 64;

				var rc = new Rectangle( 0, 32, 64, 32 );
				while( x < 640 )
				{
					if( this.tx曲名バー[ (int) type ] != null )
						this.tx曲名バー[ (int) type ].t2D描画( CDTXMania.app.Device, x, y, rc );	// 胴体；64pxずつ横につなげていく。
					x += 64;
				}
				//-----------------
				#endregion
			}
		}
		private void t曲名バーの生成( int nバー番号, string str曲名, Color color )
		{
			if( nバー番号 < 0 || nバー番号 > 12 )
				return;

			try
			{
				SizeF sz曲名;

				#region [ 曲名表示に必要となるサイズを取得する。]
				//-----------------
				using( var bmpDummy = new Bitmap( 1, 1 ) )
				{
					var g = Graphics.FromImage( bmpDummy );
					g.PageUnit = GraphicsUnit.Pixel;
					sz曲名 = g.MeasureString( str曲名, this.ft曲リスト用フォント );
				}
				//-----------------
				#endregion

				int n最大幅px = 392;
				int height = 25;
				int width = (int) ( ( sz曲名.Width + 2 ) * 0.5f );
				if( width > ( CDTXMania.app.Device.Capabilities.MaxTextureWidth / 2 ) )
					width = CDTXMania.app.Device.Capabilities.MaxTextureWidth / 2;	// 右端断ち切れ仕方ないよね

				float f拡大率X = ( width <= n最大幅px ) ? 0.5f : ( ( (float) n最大幅px / (float) width ) * 0.5f );	// 長い文字列は横方向に圧縮。

				using( var bmp = new Bitmap( width * 2, height * 2, PixelFormat.Format32bppArgb ) )		// 2倍（面積4倍）のBitmapを確保。（0.5倍で表示する前提。）
				using( var g = Graphics.FromImage( bmp ) )
				{
					float y = ( ( (float) bmp.Height ) / 2f ) - ( ( CDTXMania.ConfigIni.n選曲リストフォントのサイズdot * 2f ) / 2f );
					g.DrawString( str曲名, this.ft曲リスト用フォント, new SolidBrush( this.color文字影 ), (float) 2f, (float) ( y + 2f ) );
					g.DrawString( str曲名, this.ft曲リスト用フォント, new SolidBrush( color ), 0f, y );

					CDTXMania.t安全にDisposeする( ref this.stバー情報[ nバー番号 ].txタイトル名 );

					this.stバー情報[ nバー番号 ].txタイトル名 = new CTexture( CDTXMania.app.Device, bmp, CDTXMania.TextureFormat );
					this.stバー情報[ nバー番号 ].txタイトル名.vc拡大縮小倍率 = new Vector3( f拡大率X, 0.5f, 1f );
				}
			}
			catch( CTextureCreateFailedException )
			{
				Trace.TraceError( "曲名テクスチャの作成に失敗しました。[{0}]", str曲名 );
				this.stバー情報[ nバー番号 ].txタイトル名 = null;
			}
		}
		//-----------------
		#endregion
	}
}
