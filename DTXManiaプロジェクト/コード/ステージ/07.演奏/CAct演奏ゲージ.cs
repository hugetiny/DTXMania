using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using SlimDX;
using FDK;

namespace DTXMania
{
	/// <summary>
	/// CAct演奏Drumsゲージ と CAct演奏Gutiarゲージ のbaseクラス
	/// </summary>
	public class CAct演奏ゲージ : CActivity
	{
		public double dbゲージ値	// Drums専用
		{
			get
			{
				return this.db現在のゲージ値.Drums;
			}
			set
			{
				this.db現在のゲージ値.Drums = value;
				if ( this.db現在のゲージ値.Drums > 1.0 )
				{
					this.db現在のゲージ値.Drums = 1.0;
				}
			}
		}

		public STDGBVALUE<double> db現在のゲージ値;
		protected CCounter ct本体移動;
		protected CCounter ct本体振動;
		protected CTexture txゲージ;
	}
}
