using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Drawing;
using System.IO;
using SharpDX;
using FDK;

namespace DTXMania
{
	internal class CActSelectQuickConfig : CActSelectPopupMenu
	{
		readonly string QuickCfgTitle = "Quick Config";
		COptionStringList QTarget;
		COptionStringList QAuto;
		List<COptionBase> lci;
		Font ft表示用フォント;
		CTexture txパネル本体;
		CTexture tx文字列パネル;

		public CActSelectQuickConfig()
		{
			CActSelectQuickConfigMain();
		}

		private void CActSelectQuickConfigMain()
		{
			/*
			•Target: Drums/Guitar/Bass 
			•Auto Mode: All ON/All OFF/CUSTOM 
			•Auto Lane: 
			•Scroll Speed: 
			•Play Speed: 
			•Risky: 
			•Hidden/Sudden: None/Hidden/Sudden/Both 
			•AUTO Ghost: Perfect/Last Play/Hi Skill/Hi Score/Online
			•Target Ghost: None/Perfect/Last Play/Hi Skill/Hi Score/Online
			•Conf SET: SET-1/SET-2/SET-3 
			•More... 
			•EXIT 
			*/
			QTarget = new COptionStringList("Drums");
			QTarget.Initialize("Target", "", new string[] { "Drums", "Guitar", "Bass" });
			QTarget.OnEnterDelegate = () =>
			{
				EPart nCurrentTarget = 0;
				if (QTarget.Index == 0)
				{
					nCurrentTarget = EPart.Drums;
				}
				else if (QTarget.Index == 1)
				{
					nCurrentTarget = EPart.Guitar;
				}
				else if (QTarget.Index == 2)
				{
					nCurrentTarget = EPart.Bass;
				}
				lci = MakeListCItemBase(nCurrentTarget);
				// eInst = (E楽器パート) nCurrentTarget;
				// ここではeInstは変えない。メニューを開いたタイミングでのみeInstを使う
				Initialize(lci, true, QuickCfgTitle, n現在の選択行);
				MakeAutoPanel();
			};
			lci = MakeListCItemBase(EPart.Drums);
			// ConfSet=0, nInst=Drums
			base.Initialize(lci, true, QuickCfgTitle, 2);
		}

		private List<COptionBase> MakeListCItemBase(EPart nInst)
		{
			List<COptionBase> ret = new List<COptionBase>();

			QAuto = new COptionStringList("Custom");
			if (nInst == EPart.Drums)
			{
				string[] items_dr = new string[] { "All On", "Auto HH", "Auto BD", "Custom", "All Off" };
				int dr_init_idx = 3;
				if (CDTXMania.Instance.ConfigIni.bAutoPlay.IsAllTrue(EPart.Drums))
				{
					dr_init_idx = 0;	// All On
				}
				else if (CDTXMania.Instance.ConfigIni.bAutoPlay.bIsAutoHH)
				{
					dr_init_idx = 1;	// Auto HH
				}
				else if (CDTXMania.Instance.ConfigIni.bAutoPlay.bIsAutoBD)
				{
					dr_init_idx = 2;	// Auto BD
				}
				else if (CDTXMania.Instance.ConfigIni.bAutoPlay.IsAllFalse(EPart.Drums))
				{
					dr_init_idx = 4;	// All Off
				}
				QAuto.Initialize("Auto", "", items_dr);
				QAuto.Index = dr_init_idx;
				QAuto.OnEnterDelegate = () =>
				{
					//if (QAuto.Value == "All On")
					//{
					//	//CDTXMania.Instance.ConfigIni.bAutoPlay.Set(EPart.Drums, EThreeState.On);
					//}
					//else if (QAuto.Value == "All Off")
					//{
					//	//CDTXMania.Instance.ConfigIni.bAutoPlay.Set(EPart.Drums, EThreeState.Off);
					//}
					//else if (QAuto.Value == "Auto HH")
					//{
					//	//CDTXMania.Instance.ConfigIni.bAutoPlay.SetAutoHH();
					//}
					//else if (QAuto.Value == "Auto BD")
					//{
					//	//CDTXMania.Instance.ConfigIni.bAutoPlay.SetAutoBD();
					//}
					MakeAutoPanel();
				};
			}
			else if (nInst == EPart.Guitar || nInst == EPart.Bass)
			{
				string[] items_gt = new string[] { "All On", "Auto Pick", "Auto Neck", "Custom", "All Off" };
				// 初期値の決定
				int gt_init_idx = 3;
				if (CDTXMania.Instance.ConfigIni.bAutoPlay.IsAllTrue(nInst))
				{
					gt_init_idx = 0;
				}
				else if (CDTXMania.Instance.ConfigIni.bAutoPlay.bIsAutoPick(nInst))
				{
					gt_init_idx = 1;
				}
				else if (CDTXMania.Instance.ConfigIni.bAutoPlay.bIsAutoNeck(nInst))
				{
					gt_init_idx = 2;
				}
				else if (CDTXMania.Instance.ConfigIni.bAutoPlay.IsAllFalse(nInst))
				{
					gt_init_idx = 4;
				}
				QAuto.Initialize("Auto", "", items_gt);
				QAuto.Index = gt_init_idx;
				QAuto.OnEnterDelegate = () =>
				{
					//if (QAuto.Value == "All On")
					//{
					//	CDTXMania.Instance.ConfigIni.bAutoPlay.Set(nInst, EThreeState.On);
					//}
					//else if (QAuto.Value == "All Off")
					//{
					//	CDTXMania.Instance.ConfigIni.bAutoPlay.Set(nInst, EThreeState.Off);
					//}
					//else if (QAuto.Value == "Auto Neck")
					//{
					//	CDTXMania.Instance.ConfigIni.bAutoPlay.SetAutoNeck(nInst);
					//}
					//else if (QAuto.Value == "Auto Pick")
					//{
					//	CDTXMania.Instance.ConfigIni.bAutoPlay.SetAutoPick(nInst);
					//}
					//else if (QAuto.Value == "All Off")
					//{
					//	CDTXMania.Instance.ConfigIni.bAutoPlay.Set(nInst, EThreeState.Off);
					//}
					MakeAutoPanel();
				};
			}

			COptionLabel more = new COptionLabel("More", "");
			more.OnEnterDelegate = () =>
			{
				bGotoDetailConfig = true;
				SetAutoParameters();
				tDeativatePopupMenu();
			};

			COptionLabel tret = new COptionLabel("Return", "");
			tret.OnEnterDelegate = () =>
			{
				SetAutoParameters();
				tDeativatePopupMenu();
			};

			ret.Add(QTarget);
			ret.Add(QAuto);
			ret.Add(CDTXMania.Instance.ConfigIni.nScrollSpeed[nInst]);
			ret.Add(CDTXMania.Instance.ConfigIni.eDark);
			ret.Add(CDTXMania.Instance.ConfigIni.nRisky);
			ret.Add(CDTXMania.Instance.ConfigIni.nPlaySpeed);
			ret.Add(CDTXMania.Instance.ConfigIni.eSudHidInv[nInst]);
			ret.Add(CDTXMania.Instance.ConfigIni.eAutoGhost[nInst]);
			ret.Add(CDTXMania.Instance.ConfigIni.eTargetGhost[nInst]);
			ret.Add(more);
			ret.Add(tret);

			return ret;
		}

		// メソッド
		public override void tActivatePopupMenu(EPart einst)
		{
			this.CActSelectQuickConfigMain();
			base.tActivatePopupMenu(einst);
		}

		/// <summary>
		/// Auto Modeにフォーカスを合わせているときだけ、AUTOの設定状態を表示する。
		/// 現状はDrumでのみ表示。
		/// </summary>
		public override void t進行描画sub()
		{
			if (lci[base.n現在の選択行] == QAuto)
			{
				if (tx文字列パネル == null)   // TagetとAuto Modeを全く変更せずにAuto Modeまで動かした場合限り、ここに来る
				{
					MakeAutoPanel();
				}

				if (this.txパネル本体 != null)
				{
					this.txパネル本体.t2D描画(CDTXMania.Instance.Device, 213 * Scale.X, 167 * Scale.Y);
				}
				if (this.tx文字列パネル != null)
				{
					int x = (QTarget.Index == 0) ? 230 : 260;
					this.tx文字列パネル.t2D描画(CDTXMania.Instance.Device, x * Scale.X, 190 * Scale.Y);

				}
			}
		}

		/// <summary>
		/// ESC押下時の処理。設定情報の保持を行う。
		/// </summary>
		public override void tCancel()
		{
			SetAutoParameters();
			base.tCancel();
		}

		// 本当は、現在のレーン順に合わせた表示順にしたいが・・・
		private string[] strPadNames = new[] {
			"LC", "HH", "HO", "SD", "BD", "HT", "LT", "FT", "CY", "RD"
		};

		/// <summary>
		/// DrumsのAUTOパラメータを一覧表示するパネルを作成する
		/// </summary>
		public void MakeAutoPanel()
		{
			Bitmap image = new Bitmap((int)(300 * Scale.X), (int)(130 * Scale.Y));
			Graphics graphics = Graphics.FromImage(image);
			graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

			string header = "", state = "";
			switch (QTarget.Index)
			{
				case 0:		// Drums
					header = "";
					for (int i = 0; i < strPadNames.Length; i++)
					{
						//header += e[i].GetType().GetEnumName;
						//Trace.TraceInformation(e[i].GetType..type.GetType()..Name);
						//Trace.TraceInformation(e[i].GetType().Name);
						//Trace.TraceInformation(e[i].GetType().GetEnumName(e[i]));
						//header += e[i].GetType().GetEnumName(e[i]);

						header += strPadNames[i];
					}

					//header = nameof(EPad.LC);
					//header += nameof(CDTXMania.Instance.ConfigIni.bAutoPlay.LC);


					state = GetAutoParameters(EPart.Drums);
					break;
				case 1:		// Guitar
					header = "R G B P W ";
					state = GetAutoParameters(EPart.Guitar);
					break;
				case 2:		// Bass
					header = "R G B P W ";
					state = GetAutoParameters(EPart.Bass);
					break;
				default:
					break;
			}
			for (int i = 0; i < header.Length; i += 2)
			{
				string hh = header.Substring(i, 2);
				string ss = state.Substring(i/2, 1);

				if (hh[1] == ' ')
				{
					graphics.DrawString(hh[0].ToString(), this.ft表示用フォント, Brushes.White, (float)(i/2) * 20 * Scale.X, (float)0f);
				}
				else
				{
					graphics.ScaleTransform(0.5F, 0.5F);
					graphics.DrawString(hh[0].ToString(), this.ft表示用フォント, Brushes.White, (float)i * 20 * Scale.X, (float)0f);
					graphics.DrawString(hh[1].ToString(), this.ft表示用フォント, Brushes.White, (float)(i+0.7f) * 20 * Scale.X, (float)12f * Scale.Y);
					graphics.ResetTransform();
				}

				graphics.DrawString(ss.ToString(), this.ft表示用フォント, Brushes.White, (float)(i/2) * 20 * Scale.X, (float)24f * Scale.Y);
			}
			graphics.Dispose();

			try
			{
				if (this.tx文字列パネル != null)
				{
					this.tx文字列パネル.Dispose();
				}
				this.tx文字列パネル = new CTexture(CDTXMania.Instance.Device, image, CDTXMania.Instance.TextureFormat);
				this.tx文字列パネル.vc拡大縮小倍率 = new Vector3(1f, 1f, 1f);
				image.Dispose();
			}
			catch (CTextureCreateFailedException)
			{
				Trace.TraceError("演奏履歴文字列テクスチャの作成に失敗しました。");
				this.tx文字列パネル = null;
			}
		}

		/// <summary>
		/// 簡易CONFIG内のAUTO状態を、文字列で返す。
		/// </summary>
		/// <param name="target">対象楽器</param>
		/// <returns>AutoならA,さもなくば_。この文字が複数並んだ文字列。</returns>
		private string GetAutoParameters(EPart target)
		{
			string s = "";
			switch (target)
			{
				#region [ DRUMS ]
				case EPart.Drums:
					switch (QAuto.Index)
					{
						case 0: // All Auto
							s = "AAAAAAAAAA";
							break;
						case 1: // Auto HH
							s = "_AA_______";
							break;
						case 2: // Auto BD
							s = "___A______";
							break;
						case 3: // Custom
							// 本当は、現在のレーン順に合わせた表示順にしたいが・・・
							COptionBool[] e = new[] {
								CDTXMania.Instance.ConfigIni.bAutoPlay.LC,
								CDTXMania.Instance.ConfigIni.bAutoPlay.HH,
								CDTXMania.Instance.ConfigIni.bAutoPlay.HHO,
								CDTXMania.Instance.ConfigIni.bAutoPlay.SD,
								CDTXMania.Instance.ConfigIni.bAutoPlay.BD,
								CDTXMania.Instance.ConfigIni.bAutoPlay.HT,
								CDTXMania.Instance.ConfigIni.bAutoPlay.LT,
								CDTXMania.Instance.ConfigIni.bAutoPlay.FT,
								CDTXMania.Instance.ConfigIni.bAutoPlay.CY,
								CDTXMania.Instance.ConfigIni.bAutoPlay.RD
							};
							//for (EPad i = EPad.DrumsPadMin; i < EPad.DrumsPadMax; i++)
							for (int i = 0; i < e.Length; i++)
							{
								//s += (CDTXMania.Instance.ConfigIni.bAutoPlay[i]) ? "A" : "_";
								s += (e[i].Value) ? "A" : "_";
							}
							break;
						case 4: // OFF
							s = "__________";
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					break;
				#endregion
				#region [ Guitar / Bass ]
				case EPart.Guitar:
				case EPart.Bass:
					switch (QAuto.Index)
					{
						case 0: // All Auto
							s = "AAAAA";
							break;
						case 1: // Auto Pick
							s = "___A_";
							break;
						case 2: // Auto Neck
							s = "AAA__";
							break;
						case 3: // Custom
							EPad from = (target == EPart.Guitar) ? EPad.GuitarPadMin : EPad.BassPadMin;
							EPad to = (target == EPart.Guitar) ? EPad.GuitarPadMax : EPad.BassPadMax;
							for (EPad i = from; i < to; i++)
							{
								s += (CDTXMania.Instance.ConfigIni.bAutoPlay[i]) ? "A" : "_";
							}
							break;
						case 4: // OFF
							s = "_____";
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					break;
				default:
					break;
					#endregion
			}
			return s;
		}

		/// <summary>
		/// ConfigIni.bAutoPlayに簡易CONFIGの状態を反映する
		/// </summary>
		private void SetAutoParameters()
		{
			string s;
			#region [Drums]
			s = GetAutoParameters(EPart.Drums);
			CDTXMania.Instance.ConfigIni.bAutoPlay.LC.Value = (s[0] == 'A'); s = s.Remove(0, 1);
			CDTXMania.Instance.ConfigIni.bAutoPlay.HH.Value = (s[0] == 'A'); s = s.Remove(0, 1);
			CDTXMania.Instance.ConfigIni.bAutoPlay.HHO.Value = (s[0] == 'A'); s = s.Remove(0, 1);
			CDTXMania.Instance.ConfigIni.bAutoPlay.SD.Value = (s[0] == 'A'); s = s.Remove(0, 1);
			CDTXMania.Instance.ConfigIni.bAutoPlay.BD.Value = (s[0] == 'A'); s = s.Remove(0, 1);
			CDTXMania.Instance.ConfigIni.bAutoPlay.HT.Value = (s[0] == 'A'); s = s.Remove(0, 1);
			CDTXMania.Instance.ConfigIni.bAutoPlay.LT.Value = (s[0] == 'A'); s = s.Remove(0, 1);
			CDTXMania.Instance.ConfigIni.bAutoPlay.FT.Value = (s[0] == 'A'); s = s.Remove(0, 1);
			CDTXMania.Instance.ConfigIni.bAutoPlay.CY.Value = (s[0] == 'A'); s = s.Remove(0, 1);
			CDTXMania.Instance.ConfigIni.bAutoPlay.RD.Value = (s[0] == 'A'); s = s.Remove(0, 1);
			#endregion
			#region [Guitar]
			s = GetAutoParameters(EPart.Guitar);
			CDTXMania.Instance.ConfigIni.bAutoPlay.GtR.Value = (s[0] == 'A'); s = s.Remove(0, 1);
			CDTXMania.Instance.ConfigIni.bAutoPlay.GtG.Value = (s[0] == 'A'); s = s.Remove(0, 1);
			CDTXMania.Instance.ConfigIni.bAutoPlay.GtB.Value = (s[0] == 'A'); s = s.Remove(0, 1);
			CDTXMania.Instance.ConfigIni.bAutoPlay.GtPick.Value = (s[0] == 'A'); s = s.Remove(0, 1);
			CDTXMania.Instance.ConfigIni.bAutoPlay.GtWail.Value = (s[0] == 'A'); s = s.Remove(0, 1);
			#endregion
			#region [Bass]
			s = GetAutoParameters(EPart.Bass);
			CDTXMania.Instance.ConfigIni.bAutoPlay.GtR.Value = (s[0] == 'A'); s = s.Remove(0, 1);
			CDTXMania.Instance.ConfigIni.bAutoPlay.GtG.Value = (s[0] == 'A'); s = s.Remove(0, 1);
			CDTXMania.Instance.ConfigIni.bAutoPlay.GtB.Value = (s[0] == 'A'); s = s.Remove(0, 1);
			CDTXMania.Instance.ConfigIni.bAutoPlay.GtPick.Value = (s[0] == 'A'); s = s.Remove(0, 1);
			CDTXMania.Instance.ConfigIni.bAutoPlay.GtWail.Value = (s[0] == 'A'); s = s.Remove(0, 1);
			#endregion

			//for (EPart target = EPart.Guitar; target < EPart.Bass; target++)
			//{
			//	s += GetAutoParameters(target);
			//}
			////	EPad from = (target == EPart.Guitar) ? EPad.GuitarPadMin : EPad.BassPadMin;
			////	EPad to = (target == EPart.Guitar) ? EPad.GuitarPadMax : EPad.BassPadMax;
			//int j = 0;
			//for (EPad i = EPad.GuitarPadMin; i < EPad.BassPadMax; i++)
			//{
			//	CDTXMania.Instance.ConfigIni.bAutoPlay[i].Value = (s[j++] == 'A') ? true : false;
			//}
		}

		// CActivity 実装

		public override void On活性化()
		{
			if (base.b活性化してない)
			{
				this.ft表示用フォント = new Font("Arial", 26f * Scale.Y, FontStyle.Bold, GraphicsUnit.Pixel);
				base.On活性化();
				this.bGotoDetailConfig = false;
			}
		}

		public override void On非活性化()
		{
			if (base.b活性化してる)
			{
				if (this.ft表示用フォント != null)
				{
					this.ft表示用フォント.Dispose();
					this.ft表示用フォント = null;
				}
				base.On非活性化();
			}
		}

		public override void OnManagedリソースの作成()
		{
			if (base.b活性化してる)
			{
				string pathパネル本体 = CSkin.Path(@"Graphics\ScreenSelect popup auto settings.png");
				if (File.Exists(pathパネル本体))
				{
					this.txパネル本体 = TextureFactory.tテクスチャの生成(pathパネル本体, true);
				}
				base.OnManagedリソースの作成();
			}
		}

		public override void OnManagedリソースの解放()
		{
			if (base.b活性化してる)
			{
				TextureFactory.tテクスチャの解放(ref this.txパネル本体);
				TextureFactory.tテクスチャの解放(ref this.tx文字列パネル);
				base.OnManagedリソースの解放();
			}
		}

	}
}
