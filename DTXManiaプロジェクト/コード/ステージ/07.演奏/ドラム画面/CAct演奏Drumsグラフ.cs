using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.IO;
using SlimDX;
using FDK;

namespace DTXMania
{
	internal class CAct演奏Drumsグラフ : CActivity
	{
        // #24074 2011.01.23 ikanick グラフの描画
        // 実装内容
        // ・左を現在、右を目標
        // ・基準線(60,70,80,90,100%)を超えると線が黄色くなる（元は白）
        // ・目標を超えると現在が光る
        // ・オート時には描画しない
        // 要望・実装予定
        // ・グラフを波打たせるなどの視覚の向上→実装済
        // 修正等
        // ・画像がないと落ちる→修正済

		private int XPos = 3 * 345;
		private int YPos = 200;
		private int DispHeight = 400;
		private int DispWidth = 60;
		private CCounter counterYposInImg = null;
		private readonly int slices = 10;

		// プロパティ

        public double dbグラフ値現在_渡
        {
            get
            {
                return this.dbグラフ値現在;
            }
            set
            {
                this.dbグラフ値現在 = value;
            }
        }
        public double dbグラフ値目標_渡
        {
            get
            {
                return this.dbグラフ値目標;
            }
            set
            {
                this.dbグラフ値目標 = value;
            }
        }
		
		// コンストラクタ

		public CAct演奏Drumsグラフ()
		{
			base.b活性化してない = true;
		}


		// CActivity 実装

		public override void On活性化()
        {
            this.dbグラフ値目標 = 80f;
            this.dbグラフ値現在 = 0f;
			base.On活性化();
		}
		public override void On非活性化()
		{
			base.On非活性化();
		}
		public override void OnManagedリソースの作成()
		{
			if( !base.b活性化してない )
			{
				string pathグラフ = CSkin.Path( @"Graphics\ScreenPlay graph.png" );
				if ( File.Exists( pathグラフ ) )
				{
					this.txグラフ = TextureFactory.tテクスチャの生成( pathグラフ );
				}
				base.OnManagedリソースの作成();
			}
		}
		public override void OnManagedリソースの解放()
		{
			if( !base.b活性化してない )
			{
				TextureFactory.tテクスチャの解放( ref this.txグラフ );
				base.OnManagedリソースの解放();
			}
		}
		public override int On進行描画()
		{
			if( !base.b活性化してない )
			{
				if( base.b初めての進行描画 )
				{
					/* #35804 エフェクトを変更しました。
					for( int k = 0; k < 64; k++ )
					{
						this.stキラキラ[ k ].x = CDTXMania.Instance.Random.Next( 30 );
						this.stキラキラ[ k ].fScale = 1f + ( CDTXMania.Instance.Random.Next( 9 ) * 0.2f );
						this.stキラキラ[ k ].Trans = 0 + CDTXMania.Instance.Random.Next( 32 ) ;
                        if (k < 32)
                        {
                            this.stキラキラ[ k ].ct進行 = new CCounter(0, DispHeight, 10 + CDTXMania.Instance.Random.Next(20), CDTXMania.Instance.Timer);
                        }
                        else if (k < 64)
                        {
                            this.stキラキラ[ k ].ct進行 = new CCounter(0, DispHeight, 20 + CDTXMania.Instance.Random.Next(50), CDTXMania.Instance.Timer);
                        }
                        this.stキラキラ[ k ].ct進行.n現在の値 = CDTXMania.Instance.Random.Next(DispHeight);
					}
					for( int k = 0; k < 16; k++ )
					{
						this.stフラッシュ[ k ].y = -1;
						this.stフラッシュ[ k ].Trans = 0;
					}
					*/
					base.b初めての進行描画 = false;
					counterYposInImg = new CCounter(0, 35, 16, CDTXMania.Instance.Timer);
                }

				counterYposInImg.t進行Loop();
				int stYposInImg = counterYposInImg.n現在の値;

				// レーン表示位置によって変更
				if( CDTXMania.Instance.ConfigIni.eドラムレーン表示位置 == Eドラムレーン表示位置.Center )
				{
					XPos = 1350;
				}

                // 背景暗幕
                if (this.txグラフ != null)
                {
					this.txグラフ.vc拡大縮小倍率 = new Vector3(DispWidth, DispHeight, 1f);
                    this.txグラフ.n透明度 = 128;
                    this.txグラフ.t2D描画(
						CDTXMania.Instance.Device,
						XPos,
						YPos,
						new Rectangle(62, 0, 1, 1)
					);
                }
                
                // 基準線

				if (this.txグラフ != null)
                {
					this.txグラフ.n透明度 = 128;
                    this.txグラフ.vc拡大縮小倍率 = new Vector3(DispWidth, 1f, 1f);
                    for (int i = 0; i < slices; i++)
                    {
                        this.txグラフ.t2D描画(
							CDTXMania.Instance.Device,
							XPos,
							YPos + DispHeight * i / slices,
							new Rectangle(60, 0, 1, 1)
						);
                    }
					/* #35804 chnmr0 縦線は非表示にしました。
                    this.txグラフ.vc拡大縮小倍率 = new Vector3(1f, DispHeight, 1f);
                    for (int i = 0; i < 2; i++)
                    {
                        this.txグラフ.t2D描画(
							CDTXMania.Instance.app.Device,
							(XPos + 12 + i * 54),
							YPos,
							rectangle
						);
                        this.txグラフ.t2D描画(
							CDTXMania.Instance.app.Device,
							(45 + XPos + i * 54),
							YPos,
							rectangle
						);
                    }
					*/
                }

                if (this.txグラフ != null)
                {
                    this.txグラフ.vc拡大縮小倍率 = new Vector3(DispWidth, 1f, 1f);
                }
                for (int i = 0; i < 5; i++)
                {
					Rectangle rectangle;
                    // 基準線を越えたら線が黄色くなる
                    if (this.dbグラフ値現在 >= (100 - i * slices))
                    {
                        rectangle = new Rectangle(61,0,1,1);	//黄色
						if (this.txグラフ != null)
                        {
                            this.txグラフ.n透明度 = 224;
                        }
                    }
                    else
                    {
                        rectangle = new Rectangle(60,0,1,1);
                        if (this.txグラフ != null)
                        {
                            this.txグラフ.n透明度 = 160;
                        }
                    }

                    if (this.txグラフ != null)
                    {
                        this.txグラフ.t2D描画(
							CDTXMania.Instance.Device,
							XPos,
							YPos + i * DispHeight / slices,
							rectangle
						);
                    }
                }
                // グラフ
                // --現在値
                if (this.dbグラフ値現在_表示 < this.dbグラフ値現在)
                {
                    this.dbグラフ値現在_表示 += (this.dbグラフ値現在 - this.dbグラフ値現在_表示) / 5 + 0.01;
                }
                if (this.dbグラフ値現在_表示 >= this.dbグラフ値現在)
                {
                    this.dbグラフ値現在_表示 = this.dbグラフ値現在;
                }
				int ar = (int)(DispHeight * this.dbグラフ値現在_表示 / 100.0);

				if (this.txグラフ != null)
                {
                    this.txグラフ.vc拡大縮小倍率 = new Vector3(1f, 1f, 1f);
                    this.txグラフ.n透明度 = 255;
                    this.txグラフ.t2D描画(
						CDTXMania.Instance.Device,
						XPos,
						YPos + DispHeight - ar,
						new Rectangle(0,5 + stYposInImg,30,ar)
					);
					this.txグラフ.t2D描画( // 上部白いバー
						CDTXMania.Instance.Device,
						XPos,
						YPos + DispHeight - ar,
						new Rectangle(0, 0, 30, 5)
					);
				}
				/*
				for( int k = 0; k < 32; k++ )
				{
                    rectangle = new Rectangle(60,0,1,1);
                    if (this.txグラフ != null)
                    {
				    	this.stキラキラ[ k ].ct進行.t進行Loop();
                        int num1 = (int)this.stキラキラ[ k ].x;
                        int num2 = this.stキラキラ[ k ].ct進行.n現在の値;
                        this.txグラフ.vc拡大縮小倍率 = new Vector3(this.stキラキラ[ k ].fScale, this.stキラキラ[ k ].fScale, this.stキラキラ[ k ].fScale);
                        this.txグラフ.n透明度 = 138 - 2 * this.stキラキラ[ k ].Trans;
                        if ( num2 < (2.3f * this.dbグラフ値現在_表示) )
                        {
                            this.txグラフ.t2D描画(
								CDTXMania.Instance.app.Device,
								XPos + 15 + num1,
								YPos + DispHeight - num2,
								rectangle
							);
                        }
                    }
				}
				*/

                // --現在値_追加エフェクト
				/*
                if (this.dbグラフ値直前 != this.dbグラフ値現在)
                {
                    this.stフラッシュ[ nグラフフラッシュct ].y = 0;
                    this.stフラッシュ[ nグラフフラッシュct ].Trans = 224;
                    nグラフフラッシュct ++;
                    if (nグラフフラッシュct >= 16)
                    {
                        nグラフフラッシュct = 0;
                    }
                }
                this.dbグラフ値直前 = this.dbグラフ値現在;

                for (int m = 0; m < 16; m++)
                {
					rectangle = new Rectangle(60,0,1,1);

					if ((this.stフラッシュ[ m ].y >= 0) &&
						(this.stフラッシュ[ m ].y+3 < ar) &&
						(this.txグラフ != null))
                    {
                        // this.txグラフ.vc拡大縮小倍率 = new Vector3(1f, 1f, 1f);
                        this.txグラフ.n透明度 = this.stフラッシュ[ m ].Trans;
                        this.txグラフ.t2D描画(
							CDTXMania.Instance.app.Device,
							XPos + 15,
							(this.stフラッシュ[ m ].y + YPos + DispHeight - ar),
							rectangle
						);
                        this.txグラフ.n透明度 = this.stフラッシュ[ m ].Trans;
                        this.txグラフ.t2D描画(
							CDTXMania.Instance.app.Device,
							XPos + 15,
							(this.stフラッシュ[ m ].y + 2 + YPos + DispHeight - ar),
							rectangle
						);
                    }
                    this.stフラッシュ[ m ].y += 4;
                    this.stフラッシュ[ m ].Trans -= 4;
                }
				*/

                // --現在値_目標越
                if ((dbグラフ値現在 >= dbグラフ値目標) && (this.txグラフ != null))
                {
                    // this.txグラフ.vc拡大縮小倍率 = new Vector3(1.4f, 1f, 1f);
                    this.txグラフ.n透明度 = 128;
                    this.txグラフ.b加算合成 = true;
                    this.txグラフ.t2D描画(
						CDTXMania.Instance.Device,
						XPos,
						YPos + DispHeight - ar,
						new Rectangle(0, 5 + stYposInImg, 30, ar)
					);
                    this.txグラフ.b加算合成 = false;
                }
                // --目標値
                if (this.dbグラフ値目標_表示 < this.dbグラフ値目標)
                {
                    this.dbグラフ値目標_表示 += (this.dbグラフ値目標 - this.dbグラフ値目標_表示) / 5 + 0.01;
                }
                if (this.dbグラフ値目標_表示 >= this.dbグラフ値目標)
                {
                    this.dbグラフ値目標_表示 = this.dbグラフ値目標;
                }
				ar = (int)(DispHeight * this.dbグラフ値目標_表示 / 100.0);

				if (this.txグラフ != null)
                {
                    // this.txグラフ.vc拡大縮小倍率 = new Vector3(1f, 1f, 1f);
                    // this.txグラフ.n透明度 = 192;
                    this.txグラフ.t2D描画(
						CDTXMania.Instance.Device,
						XPos + 30,
						YPos + DispHeight - ar,
						new Rectangle(30, 5 + stYposInImg, 30, ar)
					);
					this.txグラフ.n透明度 = 255;
					this.txグラフ.t2D描画( // 上部白いバー
						CDTXMania.Instance.Device,
						XPos + 30,
						YPos + DispHeight - ar,
						new Rectangle(30, 0, 30, 5)
					);
					/*
                    // this.txグラフ.vc拡大縮小倍率 = new Vector3(1.4f, 1f, 1f);
                    this.txグラフ.n透明度 = 48;
                    this.txグラフ.b加算合成 = true;
                    this.txグラフ.t2D描画(
						CDTXMania.Instance.app.Device,
						XPos + 63,
						YPos + DispHeight - ar,
						rectangle
					);
                    this.txグラフ.b加算合成 = false;
					*/
                }
				/*
				for( int k = 32; k < 64; k++ )
				{
                    rectangle = new Rectangle(60,0,1,1);
                    if (this.txグラフ != null)
                    {
				    	this.stキラキラ[ k ].ct進行.t進行Loop();
                        int num1 = (int)this.stキラキラ[ k ].x;
                        int num2 = this.stキラキラ[ k ].ct進行.n現在の値;
                        this.txグラフ.vc拡大縮小倍率 = new Vector3(this.stキラキラ[ k ].fScale, this.stキラキラ[ k ].fScale, this.stキラキラ[ k ].fScale);
                        this.txグラフ.n透明度 = 138 - 2 * this.stキラキラ[ k ].Trans;
                        if ( num2 < (2.3f * this.dbグラフ値目標_表示) )
                        {
                            this.txグラフ.t2D描画(
								CDTXMania.Instance.app.Device,
								(XPos + 69 + 3 * num1),
								YPos + DispHeight - num2,
								rectangle
							);
                        }
                    }
				}
				*/
                
			}
			return 0;
		}


		// その他

		#region [ private ]
		//----------------
		/*
		[StructLayout( LayoutKind.Sequential )]
		private struct STキラキラ
		{
			public int x;
			public int y;
			public float fScale;
			public int Trans;
			public CCounter ct進行;
		}
		*/
        // private STキラキラ[] stキラキラ = new STキラキラ[ 64 ];
        // private STキラキラ[] stフラッシュ = new STキラキラ[ 16 ];

        private double dbグラフ値目標;
        private double dbグラフ値目標_表示;
        private double dbグラフ値現在;
        private double dbグラフ値現在_表示;
        // private double dbグラフ値直前;
        // private int nグラフフラッシュct;

		private CTexture txグラフ;
		//-----------------
		#endregion
	}
}
