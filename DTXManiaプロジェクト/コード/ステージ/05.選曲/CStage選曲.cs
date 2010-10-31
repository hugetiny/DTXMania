using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Diagnostics;
using System.IO;
using FDK;

namespace DTXMania
{
	internal class CStage選曲 : CStage
	{
		// プロパティ

		public bool bスクロール中
		{
			get
			{
				return this.act曲リスト.bスクロール中;
			}
		}
		public int n確定された曲の難易度
		{
			get;
			private set;
		}
		public Cスコア r確定されたスコア
		{
			get;
			private set;
		}
		public C曲リストノード r確定された曲 
		{
			get;
			private set;
		}
		public int n現在選択中の曲の難易度
		{
			get
			{
				return this.act曲リスト.n現在選択中の曲の現在の難易度レベル;
			}
		}
		public Cスコア r現在選択中のスコア
		{
			get
			{
				return this.act曲リスト.r現在選択中のスコア;
			}
		}
		public C曲リストノード r現在選択中の曲
		{
			get
			{
				return this.act曲リスト.r現在選択中の曲;
			}
		}


		// コンストラクタ
		public CStage選曲()
		{
			base.eステージID = CStage.Eステージ.選曲;
			base.eフェーズID = CStage.Eフェーズ.共通_通常状態;
			base.b活性化してない = true;
			base.list子Activities.Add( this.actオプションパネル = new CActオプションパネル() );
			base.list子Activities.Add( this.actFIFO = new CActFIFOBlack() );
			base.list子Activities.Add( this.actFIfrom結果画面 = new CActFIFOBlack() );
			base.list子Activities.Add( this.actFOtoNowLoading = new CActFIFOBlack() );
			base.list子Activities.Add( this.act曲リスト = new CActSelect曲リスト() );
			base.list子Activities.Add( this.actステータスパネル = new CActSelectステータスパネル() );
			base.list子Activities.Add( this.act演奏履歴パネル = new CActSelect演奏履歴パネル() );
			base.list子Activities.Add( this.actPreimageパネル = new CActSelectPreimageパネル() );
			base.list子Activities.Add( this.actPresound = new CActSelectPresound() );
			base.list子Activities.Add( this.actArtistComment = new CActSelectArtistComment() );
			base.list子Activities.Add( this.actInformation = new CActSelectInformation() );
		}
		
		
		// メソッド

		public void t選択曲変更通知()
		{
			this.actPreimageパネル.t選択曲が変更された();
			this.actPresound.t選択曲が変更された();
			this.act演奏履歴パネル.t選択曲が変更された();
			this.actステータスパネル.t選択曲が変更された();
			this.actArtistComment.t選択曲が変更された();

			#region [ プラグインにも通知する（BOX, RANDOM, BACK なら通知しない）]
			//---------------------
			if( CDTXMania.app != null )
			{
				var c曲リストノード = CDTXMania.stage選曲.r現在選択中の曲;
				var cスコア = CDTXMania.stage選曲.r現在選択中のスコア;

				if( c曲リストノード != null && cスコア != null && c曲リストノード.eノード種別 == C曲リストノード.Eノード種別.SCORE )
				{
					string str選択曲ファイル名 = cスコア.ファイル情報.ファイルの絶対パス;
					CSetDef setDef = null;
					int nブロック番号inSetDef = -1;
					int n曲番号inブロック = -1;

					if( !string.IsNullOrEmpty( c曲リストノード.pathSetDefの絶対パス ) && File.Exists( c曲リストノード.pathSetDefの絶対パス ) )
					{
						setDef = new CSetDef( c曲リストノード.pathSetDefの絶対パス );
						nブロック番号inSetDef = c曲リストノード.SetDefのブロック番号;
						n曲番号inブロック = CDTXMania.stage選曲.act曲リスト.n現在のアンカ難易度レベルに最も近い難易度レベルを返す( c曲リストノード );
					}

					foreach( CDTXMania.STPlugin stPlugin in CDTXMania.app.listプラグイン )
					{
						Directory.SetCurrentDirectory( stPlugin.strプラグインフォルダ );
						stPlugin.plugin.On選択曲変更( str選択曲ファイル名, setDef, nブロック番号inSetDef, n曲番号inブロック );
						Directory.SetCurrentDirectory( CDTXMania.strEXEのあるフォルダ );
					}
				}
			}
			//---------------------
			#endregion
		}


		// CStage 実装

		public override void On活性化()
		{
			Trace.TraceInformation( "選曲ステージを活性化します。" );
			Trace.Indent();
			try
			{
				this.eフェードアウト完了時の戻り値 = E戻り値.継続;
				this.bBGM再生済み = false;
				this.n前回Bassを踏んだ時刻 = -1;
				this.n前回HHを叩いた時刻 = -1;
				this.n前回ギターをPickした時刻 = -1;
				this.n前回ベースをPickした時刻 = -1;
				this.ftフォント = new Font( "MS PGothic", 26f, GraphicsUnit.Pixel );
				for( int i = 0; i < 4; i++ )
					this.ctキー反復用[ i ] = new CCounter( 0, 0, 0, CDTXMania.Timer );

				base.On活性化();

				this.actステータスパネル.t選択曲が変更された();	// 最大ランクを更新
			}
			finally
			{
				Trace.TraceInformation( "選曲ステージの活性化を完了しました。" );
				Trace.Unindent();
			}
		}
		public override void On非活性化()
		{
			Trace.TraceInformation( "選曲ステージを非活性化します。" );
			Trace.Indent();
			try
			{
				if( this.ftフォント != null )
				{
					this.ftフォント.Dispose();
					this.ftフォント = null;
				}
				for( int i = 0; i < 4; i++ )
				{
					this.ctキー反復用[ i ] = null;
				}
				base.On非活性化();
			}
			finally
			{
				Trace.TraceInformation( "選曲ステージの非活性化を完了しました。" );
				Trace.Unindent();
			}
		}
		public override void OnManagedリソースの作成()
		{
			if( !base.b活性化してない )
			{
				this.tx背景 = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenSelect background.jpg" ), false );
				this.tx上部パネル = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenSelect header panel.png" ), true );
				this.tx下部パネル = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenSelect footer panel.png" ), true );
				this.txコメントバー = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenSelect comment bar.png" ), true );
				base.OnManagedリソースの作成();
			}
		}
		public override void OnManagedリソースの解放()
		{
			if( !base.b活性化してない )
			{
				CDTXMania.tテクスチャの解放( ref this.tx背景 );
				CDTXMania.tテクスチャの解放( ref this.tx上部パネル );
				CDTXMania.tテクスチャの解放( ref this.tx下部パネル );
				CDTXMania.tテクスチャの解放( ref this.txコメントバー );
				base.OnManagedリソースの解放();
			}
		}
		public override int On進行描画()
		{
			if( !base.b活性化してない )
			{
				#region [ 初めての進行描画 ]
				//---------------------
				if( base.b初めての進行描画 )
				{
					this.ct登場時アニメ用共通 = new CCounter( 0, 100, 3, CDTXMania.Timer );
					if( CDTXMania.r直前のステージ == CDTXMania.stage結果 )
					{
						this.actFIfrom結果画面.tフェードイン開始();
						base.eフェーズID = CStage.Eフェーズ.選曲_結果画面からのフェードイン;
					}
					else
					{
						this.actFIFO.tフェードイン開始();
						base.eフェーズID = CStage.Eフェーズ.共通_フェードイン;
					}
					this.t選択曲変更通知();
					base.b初めての進行描画 = false;
				}
				//---------------------
				#endregion

				this.ct登場時アニメ用共通.t進行();

				if( this.tx背景 != null )
					this.tx背景.t2D描画( CDTXMania.app.Device, 0, 0 );

				this.actPreimageパネル.On進行描画();
				this.act曲リスト.On進行描画();
				int y = 0;
				if( this.ct登場時アニメ用共通.b進行中 )
				{
					double num2 = ( (double) this.ct登場時アニメ用共通.n現在の値 ) / 100.0;
					double num3 = Math.Sin( 1.5707963267948966 * num2 );
					y = ( (int) ( this.tx上部パネル.sz画像サイズ.Height * num3 ) ) - this.tx上部パネル.sz画像サイズ.Height;
				}
				if( this.tx上部パネル != null )
						this.tx上部パネル.t2D描画( CDTXMania.app.Device, 0, y );

				this.actInformation.On進行描画();
				if( this.tx下部パネル != null )
					this.tx下部パネル.t2D描画( CDTXMania.app.Device, 0, 480 - this.tx下部パネル.sz画像サイズ.Height );

				this.actステータスパネル.On進行描画();
				this.act演奏履歴パネル.On進行描画();
				this.actPresound.On進行描画();
				if( this.txコメントバー != null )
				{
					this.txコメントバー.t2D描画( CDTXMania.app.Device, 0xf2, 0xe4 );
				}
				this.actArtistComment.On進行描画();
				this.actオプションパネル.On進行描画();
				switch( base.eフェーズID )
				{
					case CStage.Eフェーズ.共通_フェードイン:
						if( this.actFIFO.On進行描画() != 0 )
						{
							base.eフェーズID = CStage.Eフェーズ.共通_通常状態;
						}
						break;

					case CStage.Eフェーズ.共通_フェードアウト:
						if( this.actFIFO.On進行描画() == 0 )
						{
							break;
						}
						return (int) this.eフェードアウト完了時の戻り値;

					case CStage.Eフェーズ.選曲_結果画面からのフェードイン:
						if( this.actFIfrom結果画面.On進行描画() != 0 )
						{
							base.eフェーズID = CStage.Eフェーズ.共通_通常状態;
						}
						break;

					case CStage.Eフェーズ.選曲_NowLoading画面へのフェードアウト:
						if( this.actFOtoNowLoading.On進行描画() == 0 )
						{
							break;
						}
						return (int) this.eフェードアウト完了時の戻り値;
				}
				if( !this.bBGM再生済み && ( base.eフェーズID == CStage.Eフェーズ.共通_通常状態 ) )
				{
					CDTXMania.Skin.bgm選曲画面.n音量・次に鳴るサウンド = 100;
					CDTXMania.Skin.bgm選曲画面.t再生する();
					this.bBGM再生済み = true;
				}

				// キー入力

				if( base.eフェーズID == CStage.Eフェーズ.共通_通常状態 
					&& CDTXMania.act現在入力を占有中のプラグイン == null )
				{
					if( CDTXMania.Input管理.Keyboard.bキーが押された( 0x35 ) || ( ( CDTXMania.Pad.b押された( E楽器パート.DRUMS, Eパッド.FT ) || CDTXMania.Pad.b押されたGB( Eパッド.FT ) ) && ( ( this.act曲リスト.r現在選択中の曲 != null ) && ( this.act曲リスト.r現在選択中の曲.r親ノード == null ) ) ) )
					{
						CDTXMania.Skin.sound取消音.t再生する();
						this.eフェードアウト完了時の戻り値 = E戻り値.タイトルに戻る;
						this.actFIFO.tフェードアウト開始();
						base.eフェーズID = CStage.Eフェーズ.共通_フェードアウト;
						return 0;
					}
					if( ( CDTXMania.Input管理.Keyboard.bキーが押されている( 120 ) || CDTXMania.Input管理.Keyboard.bキーが押されている( 0x4e ) ) && CDTXMania.Input管理.Keyboard.bキーが押された( 0x36 ) )
					{
						this.actPresound.tサウンド停止();
						this.eフェードアウト完了時の戻り値 = E戻り値.オプション呼び出し;
						this.actFIFO.tフェードアウト開始();
						base.eフェーズID = CStage.Eフェーズ.共通_フェードアウト;
						CDTXMania.Skin.sound取消音.t再生する();
						return 0;
					}
					if( ( CDTXMania.Input管理.Keyboard.bキーが押されている( 120 ) || CDTXMania.Input管理.Keyboard.bキーが押されている( 0x4e ) ) && CDTXMania.Input管理.Keyboard.bキーが押された( 0x37 ) )
					{
						this.actPresound.tサウンド停止();
						this.eフェードアウト完了時の戻り値 = E戻り値.コンフィグ呼び出し;
						this.actFIFO.tフェードアウト開始();
						base.eフェーズID = CStage.Eフェーズ.共通_フェードアウト;
						CDTXMania.Skin.sound取消音.t再生する();
						return 0;
					}
					if( this.act曲リスト.r現在選択中の曲 != null )
					{
						if( ( CDTXMania.Pad.b押されたDGB( Eパッド.CY ) || CDTXMania.Pad.b押された( E楽器パート.DRUMS, Eパッド.RD ) ) || ( CDTXMania.Pad.b押された( E楽器パート.DRUMS, Eパッド.LC ) || ( CDTXMania.ConfigIni.bEnterがキー割り当てのどこにも使用されていない && CDTXMania.Input管理.Keyboard.bキーが押された( 0x75 ) ) ) )
						{
							CDTXMania.Skin.sound決定音.t再生する();
							if( this.act曲リスト.r現在選択中の曲 != null )
							{
								switch( this.act曲リスト.r現在選択中の曲.eノード種別 )
								{
									case C曲リストノード.Eノード種別.SCORE:
										this.t曲を選択する();
										break;

									case C曲リストノード.Eノード種別.SCORE_MIDI:
										this.t曲を選択する();
										break;

									case C曲リストノード.Eノード種別.BOX:
										this.act曲リスト.tBOXに入る();
										break;

									case C曲リストノード.Eノード種別.BACKBOX:
										this.act曲リスト.tBOXを出る();
										break;

									case C曲リストノード.Eノード種別.RANDOM:
										this.t曲をランダム選択する();
										break;
								}
							}
						}
						this.ctキー反復用.Up.tキー反復( CDTXMania.Input管理.Keyboard.bキーが押されている( 0x84 ), new CCounter.DGキー処理( this.tカーソルを上へ移動する ) );
						this.ctキー反復用.R.tキー反復( CDTXMania.Pad.b押されているGB( Eパッド.HH ), new CCounter.DGキー処理( this.tカーソルを上へ移動する ) );
						if( CDTXMania.Pad.b押された( E楽器パート.DRUMS, Eパッド.SD ) )
						{
							this.tカーソルを上へ移動する();
						}
						this.ctキー反復用.Down.tキー反復( CDTXMania.Input管理.Keyboard.bキーが押されている( 50 ), new CCounter.DGキー処理( this.tカーソルを下へ移動する ) );
						this.ctキー反復用.B.tキー反復( CDTXMania.Pad.b押されているGB( Eパッド.BD ), new CCounter.DGキー処理( this.tカーソルを下へ移動する ) );
						if( CDTXMania.Pad.b押された( E楽器パート.DRUMS, Eパッド.LT ) )
						{
							this.tカーソルを下へ移動する();
						}
						if( ( ( this.act曲リスト.r現在選択中の曲 != null ) && ( this.act曲リスト.r現在選択中の曲.r親ノード != null ) ) && ( CDTXMania.Pad.b押された( E楽器パート.DRUMS, Eパッド.FT ) || CDTXMania.Pad.b押されたGB( Eパッド.FT ) ) )
						{
							this.actPresound.tサウンド停止();
							CDTXMania.Skin.sound取消音.t再生する();
							this.act曲リスト.tBOXを出る();
							this.t選択曲変更通知();
						}
						if( CDTXMania.Pad.b押された( E楽器パート.DRUMS, Eパッド.BD ) )
						{
							if( ( this.n前回Bassを踏んだ時刻 != -1 ) && ( ( CDTXMania.Timer.n現在時刻 - this.n前回Bassを踏んだ時刻 ) < 400 ) )
							{
								CDTXMania.ConfigIni.n譜面スクロール速度.Drums = ( CDTXMania.ConfigIni.n譜面スクロール速度.Drums + 1 ) % 0x10;
								CDTXMania.Skin.sound変更音.t再生する();
								this.n前回Bassを踏んだ時刻 = -1;
							}
							else
							{
								this.n前回Bassを踏んだ時刻 = CDTXMania.Timer.n現在時刻;
							}
						}
						if( CDTXMania.Pad.b押された( E楽器パート.DRUMS, Eパッド.HH ) || CDTXMania.Pad.b押された( E楽器パート.DRUMS, Eパッド.HHO ) )
						{
							if( ( this.n前回HHを叩いた時刻 != -1 ) && ( ( CDTXMania.Timer.n現在時刻 - this.n前回HHを叩いた時刻 ) < 500 ) )
							{
								this.act曲リスト.t難易度レベルをひとつ進める();
								CDTXMania.Skin.sound変更音.t再生する();
								this.n前回HHを叩いた時刻 = -1;
							}
							else
							{
								this.n前回HHを叩いた時刻 = CDTXMania.Timer.n現在時刻;
							}
						}
						if( CDTXMania.Pad.b押されている( E楽器パート.GUITAR, Eパッド.SD ) || CDTXMania.Pad.b押された( E楽器パート.GUITAR, Eパッド.HT ) )
						{
							if( ( this.n前回ギターをPickした時刻 != -1 ) && ( ( CDTXMania.Timer.n現在時刻 - this.n前回ギターをPickした時刻 ) < 500 ) )
							{
								this.act曲リスト.t難易度レベルをひとつ進める();
								CDTXMania.Skin.sound変更音.t再生する();
								this.n前回ギターをPickした時刻 = -1;
							}
							else
							{
								this.n前回ギターをPickした時刻 = CDTXMania.Timer.n現在時刻;
							}
						}
						if( CDTXMania.Pad.b押されている( E楽器パート.BASS, Eパッド.SD ) || CDTXMania.Pad.b押された( E楽器パート.BASS, Eパッド.HT ) )
						{
							if( ( this.n前回ベースをPickした時刻 != -1 ) && ( ( CDTXMania.Timer.n現在時刻 - this.n前回ベースをPickした時刻 ) < 500 ) )
							{
								this.act曲リスト.t難易度レベルをひとつ進める();
								CDTXMania.Skin.sound変更音.t再生する();
								this.n前回ベースをPickした時刻 = -1;
							}
							else
							{
								this.n前回ベースをPickした時刻 = CDTXMania.Timer.n現在時刻;
							}
						}
						if( CDTXMania.Pad.b押された( E楽器パート.DRUMS, Eパッド.HT ) )
						{
							if( ( this.n前回HTを叩いた時刻 != -1 ) && ( ( CDTXMania.Timer.n現在時刻 - this.n前回HTを叩いた時刻 ) < 400 ) )
							{
								this.n前回HTを叩いた時刻 = -1;
							}
							else
							{
								this.n前回HTを叩いた時刻 = CDTXMania.Timer.n現在時刻;
							}
						}
					}
				}
			}
			return 0;
		}
		public enum E戻り値 : int
		{
			継続,
			タイトルに戻る,
			選曲した,
			オプション呼び出し,
			コンフィグ呼び出し
		}
		

		// その他

		#region [ private ]
		//-----------------
		[StructLayout( LayoutKind.Sequential )]
		private struct STキー反復用カウンタ
		{
			public CCounter Up;
			public CCounter Down;
			public CCounter R;
			public CCounter B;
			public CCounter this[ int index ]
			{
				get
				{
					switch( index )
					{
						case 0:
							return this.Up;

						case 1:
							return this.Down;

						case 2:
							return this.R;

						case 3:
							return this.B;
					}
					throw new IndexOutOfRangeException();
				}
				set
				{
					switch( index )
					{
						case 0:
							this.Up = value;
							return;

						case 1:
							this.Down = value;
							return;

						case 2:
							this.R = value;
							return;

						case 3:
							this.B = value;
							return;
					}
					throw new IndexOutOfRangeException();
				}
			}
		}

		private CActSelectArtistComment actArtistComment;
		private CActFIFOBlack actFIFO;
		private CActFIFOBlack actFIfrom結果画面;
		private CActFIFOBlack actFOtoNowLoading;
		private CActSelectInformation actInformation;
		private CActSelectPreimageパネル actPreimageパネル;
		private CActSelectPresound actPresound;
		private CActオプションパネル actオプションパネル;
		private CActSelectステータスパネル actステータスパネル;
		private CActSelect演奏履歴パネル act演奏履歴パネル;
		private CActSelect曲リスト act曲リスト;
		private bool bBGM再生済み;
		private STキー反復用カウンタ ctキー反復用;
		private CCounter ct登場時アニメ用共通;
		private E戻り値 eフェードアウト完了時の戻り値;
		private Font ftフォント;
		private long n前回Bassを踏んだ時刻;
		private long n前回HHを叩いた時刻;
		private long n前回HTを叩いた時刻;
		private long n前回ギターをPickした時刻;
		private long n前回ベースをPickした時刻;
		private CTexture txコメントバー;
		private CTexture tx下部パネル;
		private CTexture tx上部パネル;
		private CTexture tx背景;

		private void tカーソルを下へ移動する()
		{
			CDTXMania.Skin.soundカーソル移動音.t再生する();
			this.act曲リスト.t次に移動();
		}
		private void tカーソルを上へ移動する()
		{
			CDTXMania.Skin.soundカーソル移動音.t再生する();
			this.act曲リスト.t前に移動();
		}
		private void t曲をランダム選択する()
		{
			C曲リストノード song = this.act曲リスト.r現在選択中の曲;
			if( ( song.stackランダム演奏番号.Count == 0 ) || ( song.listランダム用ノードリスト == null ) )
			{
				if( song.listランダム用ノードリスト == null )
				{
					song.listランダム用ノードリスト = this.t指定された曲が存在する場所の曲を列挙する・子リスト含む( song );
				}
				int count = song.listランダム用ノードリスト.Count;
				if( count == 0 )
				{
					return;
				}
				int[] numArray = new int[ count ];
				for( int i = 0; i < count; i++ )
				{
					numArray[ i ] = i;
				}
				for( int j = 0; j < ( count * 1.5 ); j++ )
				{
					int index = CDTXMania.Random.Next( count );
					int num5 = CDTXMania.Random.Next( count );
					int num6 = numArray[ num5 ];
					numArray[ num5 ] = numArray[ index ];
					numArray[ index ] = num6;
				}
				for( int k = 0; k < count; k++ )
				{
					song.stackランダム演奏番号.Push( numArray[ k ] );
				}
				if( CDTXMania.ConfigIni.bLogDTX詳細ログ出力 )
				{
					StringBuilder builder = new StringBuilder( 0x400 );
					builder.Append( string.Format( "ランダムインデックスリストを作成しました: {0}曲: ", song.stackランダム演奏番号.Count ) );
					for( int m = 0; m < count; m++ )
					{
						builder.Append( string.Format( "{0} ", numArray[ m ] ) );
					}
					Trace.TraceInformation( builder.ToString() );
				}
			}
			this.r確定された曲 = song.listランダム用ノードリスト[ song.stackランダム演奏番号.Pop() ];
			this.n確定された曲の難易度 = this.act曲リスト.n現在のアンカ難易度レベルに最も近い難易度レベルを返す( this.r確定された曲 );
			this.r確定されたスコア = this.r確定された曲.arスコア[ this.n確定された曲の難易度 ];
			this.eフェードアウト完了時の戻り値 = E戻り値.選曲した;
			this.actFOtoNowLoading.tフェードアウト開始();
			base.eフェーズID = CStage.Eフェーズ.選曲_NowLoading画面へのフェードアウト;
			if( CDTXMania.ConfigIni.bLogDTX詳細ログ出力 )
			{
				int[] numArray2 = song.stackランダム演奏番号.ToArray();
				StringBuilder builder2 = new StringBuilder( 0x400 );
				builder2.Append( "ランダムインデックスリスト残り: " );
				if( numArray2.Length > 0 )
				{
					for( int n = 0; n < numArray2.Length; n++ )
					{
						builder2.Append( string.Format( "{0} ", numArray2[ n ] ) );
					}
				}
				else
				{
					builder2.Append( "(なし)" );
				}
				Trace.TraceInformation( builder2.ToString() );
			}
		}
		private void t曲を選択する()
		{
			this.r確定された曲 = this.act曲リスト.r現在選択中の曲;
			this.r確定されたスコア = this.act曲リスト.r現在選択中のスコア;
			this.n確定された曲の難易度 = this.act曲リスト.n現在選択中の曲の現在の難易度レベル;
			if( ( this.r確定された曲 != null ) && ( this.r確定されたスコア != null ) )
			{
				this.eフェードアウト完了時の戻り値 = E戻り値.選曲した;
				this.actFOtoNowLoading.tフェードアウト開始();
				base.eフェーズID = CStage.Eフェーズ.選曲_NowLoading画面へのフェードアウト;
			}
		}
		private List<C曲リストノード> t指定された曲が存在する場所の曲を列挙する・子リスト含む( C曲リストノード song )
		{
			List<C曲リストノード> list = new List<C曲リストノード>();
			song = song.r親ノード;
			if( ( song == null ) && ( CDTXMania.Songs管理.list曲ルート.Count > 0 ) )
			{
				foreach( C曲リストノード c曲リストノード in CDTXMania.Songs管理.list曲ルート )
				{
					if( ( c曲リストノード.eノード種別 == C曲リストノード.Eノード種別.SCORE ) || ( c曲リストノード.eノード種別 == C曲リストノード.Eノード種別.SCORE_MIDI ) )
					{
						list.Add( c曲リストノード );
					}
					if( ( c曲リストノード.list子リスト != null ) && CDTXMania.ConfigIni.bランダムセレクトで子BOXを検索対象とする )
					{
						this.t指定された曲の子リストの曲を列挙する・孫リスト含む( c曲リストノード, ref list );
					}
				}
				return list;
			}
			this.t指定された曲の子リストの曲を列挙する・孫リスト含む( song, ref list );
			return list;
		}
		private void t指定された曲の子リストの曲を列挙する・孫リスト含む( C曲リストノード r親, ref List<C曲リストノード> list )
		{
			if( ( r親 != null ) && ( r親.list子リスト != null ) )
			{
				foreach( C曲リストノード c曲リストノード in r親.list子リスト )
				{
					if( ( c曲リストノード.eノード種別 == C曲リストノード.Eノード種別.SCORE ) || ( c曲リストノード.eノード種別 == C曲リストノード.Eノード種別.SCORE_MIDI ) )
					{
						list.Add( c曲リストノード );
					}
					if( ( c曲リストノード.list子リスト != null ) && CDTXMania.ConfigIni.bランダムセレクトで子BOXを検索対象とする )
					{
						this.t指定された曲の子リストの曲を列挙する・孫リスト含む( c曲リストノード, ref list );
					}
				}
			}
		}
		//-----------------
		#endregion
	}
}
