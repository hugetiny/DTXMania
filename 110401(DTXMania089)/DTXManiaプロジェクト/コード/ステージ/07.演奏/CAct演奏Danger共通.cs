using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using FDK;

namespace DTXMania
{
	internal abstract class CAct演奏Danger共通 : CActivity
	{
		// コンストラクタ

		public CAct演奏Danger共通()
		{
			base.b活性化してない = true;
		}


		// CActivity 実装

		public override void On活性化()
		{
			this.bDanger中 = false;
			this.ct移動用 = new CCounter();
			this.ct透明度用 = new CCounter();
			base.On活性化();
		}
		public override void On非活性化()
		{
			this.ct移動用 = null;
			this.ct透明度用 = null;
			base.On非活性化();
		}

		public abstract int t進行描画( bool bDangerである );



		// その他

		#region [ private ]
		//-----------------
		protected bool bDanger中;
		protected CCounter ct移動用;
		protected CCounter ct透明度用;
		//-----------------
		#endregion
	
	}
}
