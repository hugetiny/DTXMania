using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
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
        // ・グラフを波打たせるなどの視覚の向上


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
                this.txグラフ = CDTXMania.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenPlay graph.png"));
				base.OnManagedリソースの作成();
			}
		}
		public override void OnManagedリソースの解放()
		{
			if( !base.b活性化してない )
			{
				CDTXMania.tテクスチャの解放( ref this.txグラフ );
				base.OnManagedリソースの解放();
			}
		}
		public override int On進行描画()
		{
			if( !base.b活性化してない )
			{
				if( base.b初めての進行描画 )
				{
					base.b初めての進行描画 = false;
                }
                // 背景暗幕
                if (this.txグラフ != null)
                {
                    this.txグラフ.n透明度 = 128;
                    this.txグラフ.vc拡大縮小倍率 = new Vector3(38f, 230f, 1f);
                }
                Rectangle rectangle = new Rectangle(22, 0, 1, 1);
                this.txグラフ.t2D描画(CDTXMania.app.Device, 345, 88, rectangle);
                
                // 基準線
                if (this.txグラフ != null)
                {
                    this.txグラフ.n透明度 = 192;
                    this.txグラフ.vc拡大縮小倍率 = new Vector3(38f, 1f, 1f);
                }
                for (int i = 0; i < 4; i++)
                {
                    // 基準線を越えたら黄色くなる
                    if (this.dbグラフ値現在 >= (100 - i * 10))
                    {
                        rectangle = new Rectangle(21, 0, 1, 1);
                    }
                    else
                    {
                        rectangle = new Rectangle(20, 0, 1, 1);
                    }
                    this.txグラフ.t2D描画(CDTXMania.app.Device, 345, 88+i*23, rectangle);
                }

                // グラフ
                // --現在値
                if (this.txグラフ != null)
                {
                    this.txグラフ.n透明度 = 192;
                    this.txグラフ.vc拡大縮小倍率 = new Vector3(1f, 1f, 1f);
                }
                rectangle = new Rectangle(0, 0, 10, (int)(230f * this.dbグラフ値現在 / 100));
                this.txグラフ.t2D描画(CDTXMania.app.Device, 350, 318 - (int)(230f * this.dbグラフ値現在 / 100), rectangle);
                if (dbグラフ値現在 >= dbグラフ値目標)
                {
                    this.txグラフ.vc拡大縮小倍率 = new Vector3(1.2f, 1f, 1f);
                    this.txグラフ.n透明度 = 64;
                    this.txグラフ.b加算合成 = true;
                    this.txグラフ.t2D描画(CDTXMania.app.Device, 349, 318 - (int)(230f * this.dbグラフ値現在 / 100), rectangle);
                }
                // --目標値
                if (this.txグラフ != null)
                {
                    this.txグラフ.n透明度 = 192;
                    this.txグラフ.vc拡大縮小倍率 = new Vector3(1f, 1f, 1f);
                    this.txグラフ.b加算合成 = false;
                }
                rectangle = new Rectangle(10, 0, 10, (int)(230f * this.dbグラフ値目標 / 100));
                this.txグラフ.t2D描画(CDTXMania.app.Device, 368, 318 - (int)(230f * this.dbグラフ値目標 / 100), rectangle);
                
			}
			return 0;
		}


		// その他

		#region [ private ]
		//----------------
        private double dbグラフ値目標;
        private double dbグラフ値現在;
		private CTexture txグラフ;
		//-----------------
		#endregion
	}
}
