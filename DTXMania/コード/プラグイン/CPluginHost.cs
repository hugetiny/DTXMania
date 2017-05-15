using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using SharpDX;
using SharpDX.Direct3D9;
using FDK;

namespace DTXMania
{
	internal class CPluginHost : IPluginHost
	{
		// コンストラクタ

		public CPluginHost()
		{
			this._DTXManiaVersion = new CDTXVersion(CDTXMania.VERSION);
		}


		// IPluginHost 実装

		public CDTXVersion DTXManiaVersion
		{
			get { return this._DTXManiaVersion; }
		}
		public Device D3D9Device
		{
			get { return (CDTXMania.Instance != null) ? CDTXMania.Instance.Device : null; }
		}
		public Format TextureFormat
		{
			get { return CDTXMania.Instance.TextureFormat; }
		}
		public CTimer Timer
		{
			get { return CDTXMania.Instance.Timer; }
		}
		public CSound管理 Sound管理
		{
			get { return CDTXMania.Instance.Sound管理; }
		}
		public Size ClientSize
		{
			get { return CDTXMania.Instance.Window.ClientSize; }
		}
		public CStage.Eステージ e現在のステージ
		{
			get { return (CDTXMania.Instance.r現在のステージ != null) ? CDTXMania.Instance.r現在のステージ.eステージID : CStage.Eステージ.何もしない; }
		}
		public CStage.Eフェーズ e現在のフェーズ
		{
			get { return (CDTXMania.Instance.r現在のステージ != null) ? CDTXMania.Instance.r現在のステージ.eフェーズID : CStage.Eフェーズ.共通_通常状態; }
		}
		public bool t入力を占有する(IPluginActivity act)
		{
			if (CDTXMania.Instance.act現在入力を占有中のプラグイン != null)
				return false;

			CDTXMania.Instance.act現在入力を占有中のプラグイン = act;
			return true;
		}
		public bool t入力の占有を解除する(IPluginActivity act)
		{
			if (CDTXMania.Instance.act現在入力を占有中のプラグイン == null || CDTXMania.Instance.act現在入力を占有中のプラグイン != act)
				return false;

			CDTXMania.Instance.act現在入力を占有中のプラグイン = null;
			return true;
		}
		public void tシステムサウンドを再生する(Eシステムサウンド sound)
		{
			if (CDTXMania.Instance.Skin != null)
				CDTXMania.Instance.Skin[sound].t再生する();
		}


		// その他

		#region [ private ]
		//-----------------
		private CDTXVersion _DTXManiaVersion;
		//-----------------
		#endregion
	}
}
