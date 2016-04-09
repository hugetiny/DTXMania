using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using SlimDX;
using FDK;

namespace DTXMania
{
	internal class CAct演奏パネル文字列 : CActivity
	{
		public CAct演奏パネル文字列()
		{
			base.b活性化してない = true;
			this.Start();
		}

		public void SetPanelString(string str)
		{
			if (base.b活性化してる)
			{
				TextureFactory.tテクスチャの解放(ref this.txPanel);

				if (!string.IsNullOrEmpty(str))
				{
					using (FontFamily ff = new FontFamily("MS PGothic"))
					{
						using (CPrivateFont cpf = new CPrivateFont(ff, 24))
						{
							using (Bitmap bmp = cpf.DrawPrivateFont(str, Color.White, Color.DarkGray))
							{
								this.txPanel = TextureFactory.tテクスチャの生成(bmp);
							}
						}
					}
					this.Start();
				}
			}
		}

		public void Stop()
		{
			this.bMute = true;
		}

		public void Start()
		{
			this.bMute = false;
		}

		public override void On活性化()
		{
			this.txPanel = null;
			this.Start();
			base.On活性化();
		}

		public override void On非活性化()
		{
			TextureFactory.tテクスチャの解放(ref this.txPanel);
			base.On非活性化();
		}

		public override void OnManagedリソースの作成()
		{
			if (!base.b活性化してない)
			{
				base.OnManagedリソースの作成();
			}
		}

		public override void OnManagedリソースの解放()
		{
			if (!base.b活性化してない)
			{
				TextureFactory.tテクスチャの解放(ref this.txPanel);
				base.OnManagedリソースの解放();
			}
		}

		public override int On進行描画()
		{
			if (b活性化してる && !this.bMute)
			{
				int x = CDTXMania.Instance.Coordinates.Panel.X;
				int y = CDTXMania.Instance.Coordinates.Panel.Y;
        if (this.txPanel != null)
				{
					Rectangle rectangle = new Rectangle(0, 0, txPanel.sz画像サイズ.Width, txPanel.sz画像サイズ.Height);
					txPanel.fZ軸中心回転 = (float)(Math.PI / 2);
					this.txPanel.t2D描画(CDTXMania.Instance.Device, x - rectangle.Width / 2 - rectangle.Height / 2, y + rectangle.Width / 2 - rectangle.Height / 2, rectangle);

				}
			}
			return 0;
		}

		private CTexture txPanel;
		private bool bMute;
	}
}
