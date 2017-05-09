using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using SharpDX;
using SharpDX.Direct3D9;
using FDK;
using System.Diagnostics;

namespace DTXMania
{
	internal class CAct演奏AVI : CActivity
	{
		private long n移動開始時刻ms;
		private int n画像側開始位置X;
		private int n画像側開始位置Y;
		private int n画像側終了位置X;
		private int n画像側終了位置Y;
		private int n開始サイズH;
		private int n開始サイズW;
		private int n終了サイズH;
		private int n終了サイズW;
		private int n総移動時間ms;
		private int n表示側開始位置X;
		private int n表示側開始位置Y;
		private int n表示側終了位置X;
		private int n表示側終了位置Y;
		private CDTX.CAVI rAVI;
		private CTexture tx描画用;

		/// <summary>
		/// プレビュームービーかどうか
		/// </summary>
		/// <remarks>
		/// On活性化()の前にフラグ操作すること。(活性化中に、本フラグを見て動作を変える部分があるため)
		/// </remarks>
		public bool bIsPreviewMovie
		{
			get;
			set;
		}
		public bool bHasBGA
		{
			get;
			set;
		}
		public bool bFullScreenMovie
		{
			get;
			set;
		}

		public void PrepareProperSizeTexture(int width, int height)
		{
			try
			{
				if (this.tx描画用 != null && (this.tx描画用.szテクスチャサイズ.Width != width || this.tx描画用.szテクスチャサイズ.Height != height))
				{
					this.tx描画用.Dispose();
					this.tx描画用 = null;
				}
				if (this.tx描画用 == null)
				{
#if TEST_Direct3D9Ex
					if ( width % 32 != 0 )
					{
						width /= 32;
						width++;
						width *= 32;
					}
					this.tx描画用 = new CTexture(
						CDTXMania.Instance.Device, width, height,
						CDTXMania.Instance.GraphicsDeviceManager.CurrentSettings.BackBufferFormat,
						Pool.Default, Usage.Dynamic );
#else
					this.tx描画用 = new CTexture(
						CDTXMania.Instance.Device, width, height,
						CDTXMania.Instance.GraphicsDeviceManager.CurrentSettings.BackBufferFormat,
						Pool.Managed);
#endif
				}
			}
			catch (CTextureCreateFailedException e)
			{
				Trace.TraceError("CActAVI: OnManagedリソースの作成(): " + e.Message);
				this.tx描画用 = null;
			}
		}

		public void Start(EChannel nチャンネル番号, CDTX.CAVI rAVI, int n開始サイズW, int n開始サイズH, int n終了サイズW, int n終了サイズH, int n画像側開始位置X, int n画像側開始位置Y, int n画像側終了位置X, int n画像側終了位置Y, int n表示側開始位置X, int n表示側開始位置Y, int n表示側終了位置X, int n表示側終了位置Y, int n総移動時間ms, int n移動開始時刻ms)
		{
			if (nチャンネル番号 == EChannel.Movie || nチャンネル番号 == EChannel.MovieFull)
			{
				this.rAVI = rAVI;
				this.n開始サイズW = n開始サイズW;
				this.n開始サイズH = n開始サイズH;
				this.n終了サイズW = n終了サイズW;
				this.n終了サイズH = n終了サイズH;
				this.n画像側開始位置X = n画像側開始位置X;
				this.n画像側開始位置Y = n画像側開始位置Y;
				this.n画像側終了位置X = n画像側終了位置X;
				this.n画像側終了位置Y = n画像側終了位置Y;
				this.n表示側開始位置X = n表示側開始位置X * 2;
				this.n表示側開始位置Y = n表示側開始位置Y * 2;
				this.n表示側終了位置X = n表示側終了位置X * 2;
				this.n表示側終了位置Y = n表示側終了位置Y * 2;
				this.n総移動時間ms = n総移動時間ms;
				this.PrepareProperSizeTexture((int)this.rAVI.avi.nフレーム幅, (int)this.rAVI.avi.nフレーム高さ);
				this.n移動開始時刻ms = (n移動開始時刻ms != -1) ? n移動開始時刻ms : CSound管理.rc演奏用タイマ.n現在時刻;
				this.rAVI.avi.Run();
			}
		}
		public void SkipStart(int n移動開始時刻ms)
		{
			foreach (CChip chip in CDTXMania.Instance.DTX.listChip)
			{
				if (chip.n発声時刻ms > n移動開始時刻ms)
				{
					break;
				}
				switch (chip.eAVI種別)
				{
					case EAVIType.AVI:
						{
							if (chip.rAVI != null)
							{
								if (this.rAVI == null)
								{
									this.rAVI = chip.rAVI;    // DTXVモードで、最初に途中再生で起動したときに、ここに来る
								}
								this.bFullScreenMovie = (chip.eチャンネル番号 == EChannel.MovieFull || CDTXMania.Instance.ConfigIni.bFullAVI);   // DTXVモードで、最初に途中再生で起動したときのために必要
								this.rAVI.avi.Seek(n移動開始時刻ms - chip.n発声時刻ms);
								//this.Start( chip.eチャンネル番号, chip.rAVI, SampleFramework.GameWindowSize.Width, SampleFramework.GameWindowSize.Height, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, chip.n発声時刻ms );
								this.Start( chip.eチャンネル番号, chip.rAVI, (int)chip.rAVI.avi.nフレーム幅, (int)chip.rAVI.avi.nフレーム高さ, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, chip.n発声時刻ms );
							}
							continue;
						}
					case EAVIType.AVIPAN:
						{
							if (chip.rAVIPan != null)
							{
								if (this.rAVI == null)
								{
									this.rAVI = chip.rAVI;    // DTXVモードで、最初に途中再生で起動したときに、ここに来る
								}
								this.bFullScreenMovie = (chip.eチャンネル番号 == EChannel.MovieFull || CDTXMania.Instance.ConfigIni.bFullAVI);   // DTXVモードで、最初に途中再生で起動したときのために必要
								this.rAVI.avi.Seek(n移動開始時刻ms - chip.n発声時刻ms);
								this.Start(chip.eチャンネル番号, chip.rAVI, chip.rAVIPan.sz開始サイズ.Width, chip.rAVIPan.sz開始サイズ.Height, chip.rAVIPan.sz終了サイズ.Width, chip.rAVIPan.sz終了サイズ.Height, chip.rAVIPan.pt動画側開始位置.X, chip.rAVIPan.pt動画側開始位置.Y, chip.rAVIPan.pt動画側終了位置.X, chip.rAVIPan.pt動画側終了位置.Y, chip.rAVIPan.pt表示側開始位置.X, chip.rAVIPan.pt表示側開始位置.Y, chip.rAVIPan.pt表示側終了位置.X, chip.rAVIPan.pt表示側終了位置.Y, chip.n総移動時間, chip.n発声時刻ms);
							}
							continue;
						}
				}
			}
		}
		public void Stop()
		{
			if ((this.rAVI != null) && (this.rAVI.avi != null))
			{
				this.n移動開始時刻ms = -1;
			}
		}

		public void Cont(int n再開時刻ms)
		{
			if ((this.rAVI != null) && (this.rAVI.avi != null))
			{
				this.n移動開始時刻ms = n再開時刻ms;
			}
		}

		/// <summary>
		/// この関数は AVI 再生のために使用できません。かわりに座標と大きさ指定可能な描画関数を使用してください。
		/// </summary>
		/// <returns></returns>
		public override int On進行描画()
		{
			return 0;
		}

		public int t進行描画(int x, int y, int w, int h)
		{
			if (b活性化してる &&
				CDTXMania.Instance.ConfigIni.bAVI &&
				!CDTXMania.Instance.ConfigIni.bStoicMode)
			{
				if (((this.n移動開始時刻ms == -1) || (this.rAVI == null)) || (this.rAVI.avi == null))
				{
					return 0;
				}
				if (this.tx描画用 == null)
				{
					return 0;
				}
				int time = (int)((CSound管理.rc演奏用タイマ.n現在時刻 - this.n移動開始時刻ms) * (((double)CDTXMania.Instance.ConfigIni.nPlaySpeed) / 20.0));
				if ((this.n総移動時間ms != 0) && (this.n総移動時間ms < time))
				{
					this.n総移動時間ms = 0;
					this.n移動開始時刻ms = -1;
					return 0;
				}
				if ((this.n総移動時間ms == 0) && time >= this.rAVI.avi.GetDuration())
				{
					if (!bIsPreviewMovie)
					{
						this.n移動開始時刻ms = -1;
						return 0;
					}
					// PREVIEW時はループ再生する。移動開始時刻msを現時刻にして(=AVIを最初に巻き戻して)、ここまでに行った計算をやり直す。
					this.n移動開始時刻ms = CSound管理.rc演奏用タイマ.n現在時刻;
					time = (int)((CSound管理.rc演奏用タイマ.n現在時刻 - this.n移動開始時刻ms) * (((double)CDTXMania.Instance.ConfigIni.nPlaySpeed) / 20.0));
					this.rAVI.avi.Seek(0);
				}

				if ((this.tx描画用 != null) && (this.n総移動時間ms != -1))
				{
					this.rAVI.avi.tGetBitmap(CDTXMania.Instance.Device, this.tx描画用, time);
					// 旧動画 (278x355以下)の場合と、それ以上の場合とで、拡大/表示位置補正ロジックを変えること。
					// 旧動画の場合は、「278x355の領域に表示される」ことを踏まえて扱う必要あり。
					// 例: 上半分だけ動画表示するような場合は・・・「上半分だけ」という表示意図を維持すべきか？それとも無視して全画面拡大すべきか？？
					// chnmr0 : プレビューの場合表示領域いっぱいにアス比保持で拡縮します。
					//          プレビューでない場合単純に縦横2倍、位置変更なしで表示します。
					// yyagi: BGAの有無を見ないで、単純にFullScreenMovieならアス比保持で拡縮、そうでないなら縦横2倍＋位置変更なし。
					// chnmr0 : 従来の大きさ以上のプレビュー動画で不都合が起きますのでここは常にアス比保持でフィッティングします。

					int xx = x, yy = y;
					float magX = 2, magY = 2;
					if ( CDTXMania.Instance.DTX != null && CDTXMania.Instance.DTX.bUse556x710BGAAVI )
					{
						magX = magY = 1;
					}

					if (bFullScreenMovie || bIsPreviewMovie)
					{
						CPreviewMagnifier.EPreviewType e = CPreviewMagnifier.EPreviewType.PlayingFront;
						if ( bFullScreenMovie ) e = CPreviewMagnifier.EPreviewType.PlayingBackground;
						if ( bIsPreviewMovie ) e = CPreviewMagnifier.EPreviewType.MusicSelect;

						CPreviewMagnifier cmg = new CPreviewMagnifier( e, xx, yy );
						cmg.GetMagnifier(
							(int) this.rAVI.avi.nフレーム幅,
							(int) this.rAVI.avi.nフレーム高さ,
							1.0f,
							1.0f
						);
						magX = cmg.magX;
						magY = cmg.magY;
						xx = cmg.px;
						yy = cmg.py;
					}

					this.tx描画用.vc拡大縮小倍率.X = magX;
					this.tx描画用.vc拡大縮小倍率.Y = magY;
					this.tx描画用.vc拡大縮小倍率.Z = 1.0f;
					this.tx描画用.bFlipY = true;
					this.tx描画用.t2D描画(CDTXMania.Instance.Device, xx, yy);
				}
			}
			return 0;
		}

		public override void On活性化()
		{
			if (b活性化してない)
			{
				this.rAVI = null;
				this.n移動開始時刻ms = -1;
				this.bHasBGA = false;
				this.bFullScreenMovie = false;
				base.On活性化();
			}
		}

		public override void OnManagedリソースの作成()
		{
			if (b活性化してる)
			{
#if TEST_Direct3D9Ex
				this.PrepareProperSizeTexture(
						( bIsPreviewMovie ) ? 204 : SampleFramework.GameWindowSize.Width,
						( bIsPreviewMovie ) ? 269 : SampleFramework.GameWindowSize.Height
						);
#else
				this.PrepareProperSizeTexture(
						(bIsPreviewMovie) ? 204 : SampleFramework.GameWindowSize.Width,
						(bIsPreviewMovie) ? 269 : SampleFramework.GameWindowSize.Height
						);
#endif
				this.tx描画用.vc拡大縮小倍率 = new Vector3(Scale.X, Scale.Y, 1f);
				base.OnManagedリソースの作成();
			}
		}
		public override void OnManagedリソースの解放()
		{
			if (b活性化してる)
			{
				if (this.tx描画用 != null)
				{
					this.tx描画用.Dispose();
					this.tx描画用 = null;
				}
				base.OnManagedリソースの解放();
			}
		}
	}
}
