using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Drawing;
using System.Threading;
using SharpDX;
using FDK;
using System.IO;

using Color = System.Drawing.Color;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;

namespace DTXMania
{
	internal class CActConfigList : CActivity
	{
		STDGBSValue<COptionLabel> ReturnToMenu;
		STDGBSValue<COptionLabel> KeyAssignMenu;
		STDGBSValue<COptionLabel> DisplayMenu;
		STDGBSValue<COptionLabel> EDrumsMenu;
		COptionLabel SoundMenu;
		COptionLabel HitRangeMenu;
		COptionStringList skins;

		bool b項目リスト側にフォーカスがある;
		bool b要素値にフォーカス中;
		CCounter ct三角矢印アニメ;
		Eメニュー種別 eメニュー種別;
		// #33689 2014.6.17 yyagi
		bool InitialUseOSTimer;
		bool InitialWASAPIEventDriven;
		ESoundDeviceTypeForConfig InitialSystemSoundType;
		int InitialWASAPIBufferSizeMs;
		int InitialASIODevice;
		bool InitialForceHighPower;
		List<COptionBase> list項目リスト;
		long nスクロール用タイマ値;
		int n現在のスクロールカウンタ;
		int n目標のスクロールカウンタ;
		Point[] ptパネルの基本座標 = new Point[]
		{
			new Point(0x12d, 3), new Point(0x12d, 0x35), new Point(0x12d, 0x67),
			new Point(0x12d, 0x99), new Point(0x114, 0xcb), new Point(0x12d, 0xfd),
			new Point(0x12d, 0x12f), new Point(0x12d, 0x161), new Point(0x12d, 0x193),
			new Point(0x12d, 0x1c5)
		};
		CTextureAf txその他項目行パネル;
		CTexture tx三角矢印;
		CTextureAf tx通常項目行パネル;
		// #28195 2012.5.2 yyagi
		CTexture txSkinSample;
		string[] skinSubFolders;
		string[] skinNames;
		string skinInitial;
		CPrivateFastFont prvFont;
		private struct stMenuItemRight
		{
			public CTexture txMenuItemRight;
			public int nParam;
			public string strParam;
			public CTexture txParam;
			public CTexture txParamColored;
		}
		stMenuItemRight[] listMenu;

		/// <summary>
		/// #32059 2013.9.17 yyagi
		/// </summary>
		public bool bIsFocusingParameter
		{
			get
			{
				return b要素値にフォーカス中;
			}
		}

		public bool b現在選択されている項目はReturnToMenuである
		{
			get
			{
				COptionBase sel = this.list項目リスト[this.n現在の選択項目];
				bool ret = false;
				for (EPart i = EPart.Drums; i <= EPart.System; ++i)
				{
					if (sel == ReturnToMenu[i])
					{
						ret = true;
					}
				}
				return ret;
			}
		}

		public bool b楽器またはシステム用メニューである
		{
			get
			{
				return eメニュー種別 == Eメニュー種別.Bass || eメニュー種別 == Eメニュー種別.Drums || eメニュー種別 == Eメニュー種別.Guitar || eメニュー種別 == Eメニュー種別.System;
			}
		}

		public COptionBase ib現在の選択項目
		{
			get
			{
				return this.list項目リスト[this.n現在の選択項目];
			}
		}

		public int n現在の選択項目;

		/// <summary>
		/// ESC押下時の右メニュー描画
		/// </summary>
		public void tEsc押下()
		{
			if (this.b要素値にフォーカス中)
			{
				// #32059 2013.9.17 add yyagi
				this.b要素値にフォーカス中 = false;
			}

			if (this.eメニュー種別 == Eメニュー種別.KeyAssignSystem)
			{
				t項目リストの設定(Eメニュー種別.System);
			}
			else if (this.eメニュー種別 == Eメニュー種別.KeyAssignDrums)
			{
				t項目リストの設定(Eメニュー種別.Drums);
			}
			else if (this.eメニュー種別 == Eメニュー種別.KeyAssignGuitar)
			{
				t項目リストの設定(Eメニュー種別.Guitar);
			}
			else if (this.eメニュー種別 == Eメニュー種別.KeyAssignBass)
			{
				t項目リストの設定(Eメニュー種別.Bass);
			}
			if (this.eメニュー種別 == Eメニュー種別.DisplaySystem)
			{
				t項目リストの設定(Eメニュー種別.System);
			}
			else if (this.eメニュー種別 == Eメニュー種別.DisplayDrums)
			{
				t項目リストの設定(Eメニュー種別.Drums);
			}
			else if (this.eメニュー種別 == Eメニュー種別.DisplayGuitar)
			{
				t項目リストの設定(Eメニュー種別.Guitar);
			}
			else if (this.eメニュー種別 == Eメニュー種別.DisplayBass)
			{
				t項目リストの設定(Eメニュー種別.Bass);
			}
			else if (this.eメニュー種別 == Eメニュー種別.EDrumsSettings)
			{
				t項目リストの設定(Eメニュー種別.Drums);
			}
			else if (this.eメニュー種別 == Eメニュー種別.HitRangeSettings)
			{
				t項目リストの設定(Eメニュー種別.System);
			}
			else if ( this.eメニュー種別 == Eメニュー種別.SoundSettings)
			{
				t項目リストの設定(Eメニュー種別.System);
			}
		}

		public bool tEnter押下()
		{
			bool ret = false;
			CDTXMania.Instance.Skin.sound決定音.t再生する();

			if (this.b要素値にフォーカス中)
			{
				this.b要素値にフォーカス中 = false;
			}
			else if (this.list項目リスト[this.n現在の選択項目] is COptionInteger)
			{
				this.b要素値にフォーカス中 = true;
			}
			else if (this.list項目リスト[this.n現在の選択項目] is IOptionList || this.list項目リスト[this.n現在の選択項目] is COptionBool)
			{
				this.list項目リスト[this.n現在の選択項目].OnNext();
			}

			// #27029 2012.1.5 from
			if (
					CDTXMania.Instance.ConfigIni.eBDGroup == EBDGroup.Group &&
					(
					(this.list項目リスト[this.n現在の選択項目] == CDTXMania.Instance.ConfigIni.eHHGroup ||
					 this.list項目リスト[this.n現在の選択項目] == CDTXMania.Instance.ConfigIni.eHitSoundPriorityHH)))
			{
				// 変更禁止（何もしない）
			}
			else
			{
				if (b現在選択されている項目はReturnToMenuである && b楽器またはシステム用メニューである)
				{
					ret = true;
				}
				this.list項目リスト[this.n現在の選択項目].OnEnter();
			}
			return ret;
		}

		private void tGenerateSkinSample()
		{
			string path = skinSubFolders[skins.Index];
			path = System.IO.Path.Combine(path, @"Graphics\ScreenTitle background.jpg");
			using (Bitmap bmSrc = new Bitmap(path))
			{
				using (Bitmap bmDest = new Bitmap(bmSrc.Width / 4, bmSrc.Height / 4))
				{
					using (Graphics g = Graphics.FromImage(bmDest))
					{
						g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
						g.DrawImage(bmSrc, new Rectangle(0, 0, bmSrc.Width / 4, bmSrc.Height / 4),
								0, 0, bmSrc.Width, bmSrc.Height, GraphicsUnit.Pixel);
						TextureFactory.tテクスチャの解放(ref txSkinSample);
						txSkinSample = TextureFactory.tテクスチャの生成(bmDest, false);
					}
				}
			}
		}

		// #region [ 項目リストの設定 ( Exit, KeyAssignSystem/Drums/Guitar/Bass) ]
		public void t項目リストの設定_Exit()
		{
			CDTXMania.Instance.SaveConfig();
			this.eメニュー種別 = Eメニュー種別.Unknown;
		}

		public void t項目リストの設定(Eメニュー種別 eMenu)
		{
			this.list項目リスト.Clear();

			Func<EPad, string, string, COptionString> PadNotifier = (pad, lbl, expl) =>
			{
				COptionString opt = new COptionString("");
				opt.Initialize(lbl, expl);
				opt.OnEnterDelegate = () =>
				{
					CDTXMania.Instance.stageコンフィグ.tパッド選択通知(pad);
				};
				return opt;
			};

			CResources cr = CDTXMania.Instance.Resources;

			if (eMenu == Eメニュー種別.DisplaySystem)
			{
				list項目リスト.Add(ReturnToMenu.System);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.bFullScreen);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.bBGA);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.bAVI);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.bForceScalingAVI);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.bVSyncWait);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.cdDebugX.Both);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.cdDebugY.Both);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.cdDebugX.DrOnly);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.cdDebugY.DrOnly);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.cdDebugX.GBOnly);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.cdDebugY.GBOnly);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.cdLegacyAVIX.Both);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.cdLegacyAVIY.Both);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.cdLegacyAVIX.DrOnly);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.cdLegacyAVIY.DrOnly);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.cdLegacyAVIX.GBOnly);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.cdLegacyAVIY.GBOnly);
				//list項目リスト.Add(CDTXMania.Instance.ConfigIni.bForceFullMovieCentering.Both);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.cdForceScaledMovieX.Both);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.cdForceScaledMovieY.Both);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.cdForceScaledMovieW.Both);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.cdForceScaledMovieH.Both);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.cdForceScaledMovieX.DrOnly);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.cdForceScaledMovieY.DrOnly);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.cdForceScaledMovieW.DrOnly);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.cdForceScaledMovieH.DrOnly);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.cdForceScaledMovieX.GBOnly);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.cdForceScaledMovieY.GBOnly);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.cdForceScaledMovieW.GBOnly);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.cdForceScaledMovieH.GBOnly);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.nBGAlpha);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.nShowLagType);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.eJudgePriority);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.bShowSongPath);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.bDebugInfo);
			}
			else if (eMenu == Eメニュー種別.DisplayDrums)
			{
				list項目リスト.Add(ReturnToMenu.Drums);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.cdInstX.Drums.Both);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.cdInstX.Drums.DrOnly);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.cdComboX.Drums.Both);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.cdComboX.Drums.DrOnly);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.cdJudgeLineY.Drums);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.cdJudgeY.Drums);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.cdLaneOrder.LC);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.cdLaneOrder.HH);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.cdLaneOrder.SD);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.cdLaneOrder.BD);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.cdLaneOrder.HT);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.cdLaneOrder.LT);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.cdLaneOrder.FT);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.cdLaneOrder.CY);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.eSudHidInv.Drums);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.bReverse.Drums);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.bDisplayCombo.Drums);		// 112追加
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.nMinComboDisp.Drums);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.bDisplayJudge.Drums);		// 112追加
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.bGraph.Drums);
			}
			else if (eMenu == Eメニュー種別.DisplayGuitar)
			{
				list項目リスト.Add(ReturnToMenu.Guitar);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.cdInstX.Guitar.Both);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.cdInstX.Guitar.GBOnly);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.cdComboX.Guitar.Both);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.cdComboX.Guitar.GBOnly);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.cdJudgeLineY.Guitar);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.cdJudgeY.Guitar);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.cdLaneOrder.GtR);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.cdLaneOrder.GtG);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.cdLaneOrder.GtB);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.eSudHidInv.Guitar);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.bReverse.Guitar);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.bDisplayCombo.Guitar);		// 112追加
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.nMinComboDisp.Guitar);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.bDisplayJudge.Guitar);		// 112追加
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.bGraph.Guitar);
			}
			else if (eMenu == Eメニュー種別.DisplayBass)
			{
				list項目リスト.Add(ReturnToMenu.Bass);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.cdInstX.Bass.Both);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.cdInstX.Bass.GBOnly);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.cdComboX.Bass.Both);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.cdComboX.Bass.GBOnly);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.cdJudgeLineY.Bass);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.cdJudgeY.Bass);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.cdLaneOrder.BsR);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.cdLaneOrder.BsG);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.cdLaneOrder.BsB);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.eSudHidInv.Bass);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.bReverse.Bass);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.bDisplayCombo.Bass);		// 112追加
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.nMinComboDisp.Bass);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.bDisplayJudge.Bass);		// 112追加
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.bGraph.Bass);
			}
			else if (eMenu == Eメニュー種別.EDrumsSettings)
			{
				list項目リスト.Add(ReturnToMenu.Drums);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.nVelocityMin.LC);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.nVelocityMin.HH);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.nVelocityMin.SD);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.nVelocityMin.BD);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.nVelocityMin.HT);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.nVelocityMin.LT);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.nVelocityMin.FT);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.nVelocityMin.CY);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.nVelocityMin.RD);
			}
			else if (eMenu == Eメニュー種別.HitRangeSettings)
			{
				list項目リスト.Add(ReturnToMenu.System);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.nHitRange.Perfect);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.nHitRange.Great);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.nHitRange.Good);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.nHitRange.Poor);
			}
			else if (eMenu == Eメニュー種別.KeyAssignSystem)
			{
				list項目リスト.Add(ReturnToMenu.System);
				list項目リスト.Add(PadNotifier(EPad.Capture, cr.Label("strCfgKeyAssignCapture"), cr.Explanation("strCfgKeyAssignCapture") ));
				list項目リスト.Add(PadNotifier(EPad.Up,      cr.Label("strCfgKeyAssignUp"),      cr.Explanation("strCfgKeyAssignUp")));
				list項目リスト.Add(PadNotifier(EPad.Down,    cr.Label("strCfgKeyAssignDown"),    cr.Explanation("strCfgKeyAssignDown")));
			}
			else if (eMenu == Eメニュー種別.KeyAssignDrums)
			{
				list項目リスト.Add(ReturnToMenu.Drums);
				list項目リスト.Add(PadNotifier(EPad.LC, cr.Label("strCfgKeyAssignLC"), cr.Explanation("strCfgKeyAssignLC") ));
				list項目リスト.Add(PadNotifier(EPad.HH, cr.Label("strCfgKeyAssignHH"), cr.Explanation("strCfgKeyAssignHH") ));
				list項目リスト.Add(PadNotifier(EPad.HHO,cr.Label("strCfgKeyAssignHHO"), cr.Explanation("strCfgKeyAssignHHO") ));
				list項目リスト.Add(PadNotifier(EPad.SD, cr.Label("strCfgKeyAssignSD"), cr.Explanation("strCfgKeyAssignSD") ));
				list項目リスト.Add(PadNotifier(EPad.BD, cr.Label("strCfgKeyAssignBD"), cr.Explanation("strCfgKeyAssignBD") ));
				list項目リスト.Add(PadNotifier(EPad.HT, cr.Label("strCfgKeyAssignHT"), cr.Explanation("strCfgKeyAssignHT") ));
				list項目リスト.Add(PadNotifier(EPad.LT, cr.Label("strCfgKeyAssignLT"), cr.Explanation("strCfgKeyAssignLT") ));
				list項目リスト.Add(PadNotifier(EPad.FT, cr.Label("strCfgKeyAssignFT"), cr.Explanation("strCfgKeyAssignFT") ));
				list項目リスト.Add(PadNotifier(EPad.CY, cr.Label("strCfgKeyAssignCY"), cr.Explanation("strCfgKeyAssignCY") ));
				list項目リスト.Add(PadNotifier(EPad.RD, cr.Label("strCfgKeyAssignRD"), cr.Explanation("strCfgKeyAssignRD") ));
				list項目リスト.Add(PadNotifier(EPad.HP, cr.Label("strCfgKeyAssignHP"), cr.Explanation("strCfgKeyAssignHP") ));
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.eHHGroup);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.eFTGroup);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.eCYGroup);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.eBDGroup);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.bCymbalFree);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.nInputAdjustTimeMs.Drums);
			}
			else if (eMenu == Eメニュー種別.KeyAssignGuitar)
			{
				list項目リスト.Add(ReturnToMenu.Guitar);
				list項目リスト.Add(PadNotifier(EPad.GtR, cr.Label("strCfgKeyAssignGtR"), cr.Explanation("strCfgKeyAssignGtR") ));
				list項目リスト.Add(PadNotifier(EPad.GtG, cr.Label("strCfgKeyAssignGtG"), cr.Explanation("strCfgKeyAssignGtG") ));
				list項目リスト.Add(PadNotifier(EPad.GtB, cr.Label("strCfgKeyAssignGtB"), cr.Explanation("strCfgKeyAssignGtB") ));
				list項目リスト.Add(PadNotifier(EPad.GtPick, cr.Label("strCfgKeyAssignGtPick"), cr.Explanation("strCfgKeyAssignGtPick") ));
				list項目リスト.Add(PadNotifier(EPad.GtWail, cr.Label("strCfgKeyAssignGtWailing"), cr.Explanation("strCfgKeyAssignGtWailing") ));
				list項目リスト.Add(PadNotifier(EPad.GtDecide, cr.Label("strCfgKeyAssignGtDecide"), cr.Explanation("strCfgKeyAssignGtDecide") ));
				list項目リスト.Add(PadNotifier(EPad.GtCancel, cr.Label("strCfgKeyAssignGtCancel"), cr.Explanation("strCfgKeyAssignGtCancel") ));
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.nInputAdjustTimeMs.Guitar);
			}
			else if (eMenu == Eメニュー種別.KeyAssignBass)
			{
				list項目リスト.Add(ReturnToMenu.Bass);
				list項目リスト.Add(PadNotifier(EPad.BsR, cr.Label("strCfgKeyAssignBsR"), cr.Explanation("strCfgKeyAssignBsR") ));
				list項目リスト.Add(PadNotifier(EPad.BsG, cr.Label("strCfgKeyAssignBsG"), cr.Explanation("strCfgKeyAssignBsG") ));
				list項目リスト.Add(PadNotifier(EPad.BsB, cr.Label("strCfgKeyAssignBsB"), cr.Explanation("strCfgKeyAssignBsB") ));
				list項目リスト.Add(PadNotifier(EPad.BsPick, cr.Label("strCfgKeyAssignBsPick"), cr.Explanation("strCfgKeyAssignBsPick") ));
				list項目リスト.Add(PadNotifier(EPad.BsWail, cr.Label("strCfgKeyAssignBsWailing"), cr.Explanation("strCfgKeyAssignBsWailing") ));
				list項目リスト.Add(PadNotifier(EPad.BsDecide, cr.Label("strCfgKeyAssignBsDecide"), cr.Explanation("strCfgKeyAssignBsDecide") ));
				list項目リスト.Add(PadNotifier(EPad.BsCancel, cr.Label("strCfgKeyAssignBsCancel"), cr.Explanation("strCfgKeyAssignBsCancel") ));
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.nInputAdjustTimeMs.Bass);
			}
			else if (eMenu == Eメニュー種別.System)
			{
				list項目リスト.Add(ReturnToMenu.System);
				COptionStringList langlist = new COptionStringList("default");
				langlist.Initialize( "strCfgLanguage", cr.LanguageDispList );
				langlist.Index = CDTXMania.Instance.Resources.LanguageCodeIndex;
				langlist.OnEnterDelegate = () =>
				{
					int index = langlist.Index;
					CDTXMania.Instance.ConfigIni.strLanguage.Value = CDTXMania.Instance.Resources.Language = CDTXMania.Instance.Resources.LanguageCodeList[ index ];
					 
				}; 
				list項目リスト.Add( langlist );
			
				COptionString enumsongs = new COptionString("");
				enumsongs.Initialize( "strCfgSysEnumSongs" );
				list項目リスト.Add(enumsongs);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.bEnumerateSongsInBoot);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.eDark);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.nRisky);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.eActiveInst);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.nPlaySpeed);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.nSleepPerFrameMs);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.nSleepUnfocusMs);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.bForceHighPowerPlan);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.bStageFailed);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.bRandSubBox);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.nPreSoundWeightMs);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.nPreImageWeightMs);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.eDamageLevel);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.bScoreIni);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.nChipVolume);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.nAutoVolume);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.bStoicMode);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.bIsAutoResultCapture);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.bBufferedInput);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.bLog);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.bLogEnumerateSongs);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.bLogCreateRelease);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.bLogDTX);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.bLoadSoundSpeed);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.bLoadDTXDetail);// #36177 2016.7.30 ikanick
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.bUseBoxDefSkin);
				list項目リスト.Add(skins);
				list項目リスト.Add(SoundMenu);
				list項目リスト.Add(DisplayMenu.System);
				list項目リスト.Add(HitRangeMenu);
				list項目リスト.Add(KeyAssignMenu.System);
				enumsongs.OnEnterDelegate = () =>
				{
					if (CDTXMania.Instance.EnumSongs.IsEnumerating)
					{
						// Debug.WriteLine( "バックグラウンドでEnumeratingSongs中だったので、一旦中断します。" );
						CDTXMania.Instance.EnumSongs.Abort();
						CDTXMania.Instance.actEnumSongs.On非活性化();
					}

					CDTXMania.Instance.EnumSongs.StartEnumFromDisk();
					CDTXMania.Instance.EnumSongs.ChangeEnumeratePriority(ThreadPriority.Normal);
					CDTXMania.Instance.actEnumSongs.bコマンドでの曲データ取得 = true;
					CDTXMania.Instance.actEnumSongs.On活性化();
				};
			}
			else if ( eMenu == Eメニュー種別.SoundSettings )
			{
				list項目リスト.Add( ReturnToMenu.System );
				list項目リスト.Add( CDTXMania.Instance.ConfigIni.bTimeStretch );
				list項目リスト.Add( CDTXMania.Instance.ConfigIni.bWaveAdjust );
				list項目リスト.Add( CDTXMania.Instance.ConfigIni.bBGMPlay );
				list項目リスト.Add( CDTXMania.Instance.ConfigIni.bAudience );
				list項目リスト.Add( CDTXMania.Instance.ConfigIni.nChipVolume );
				list項目リスト.Add( CDTXMania.Instance.ConfigIni.nAutoVolume );
				list項目リスト.Add( CDTXMania.Instance.ConfigIni.nSoundDeviceType );
				list項目リスト.Add( CDTXMania.Instance.ConfigIni.nWASAPIBufferSizeMs );
				list項目リスト.Add( CDTXMania.Instance.ConfigIni.bEventDrivenWASAPI );
				list項目リスト.Add( CDTXMania.Instance.ConfigIni.strASIODevice );
				list項目リスト.Add( CDTXMania.Instance.ConfigIni.bUseOSTimer );
				list項目リスト.Add( CDTXMania.Instance.ConfigIni.bDynamicBassMixerManagement );
				list項目リスト.Add( CDTXMania.Instance.ConfigIni.nMasterVolume );
				list項目リスト.Add( CDTXMania.Instance.ConfigIni.nPolyphonicSounds );
				list項目リスト.Add( CDTXMania.Instance.ConfigIni.nPolyphonicSoundsGB );
				list項目リスト.Add( CDTXMania.Instance.ConfigIni.eClickType );
				list項目リスト.Add( CDTXMania.Instance.ConfigIni.nClickHighVolume );
				list項目リスト.Add( CDTXMania.Instance.ConfigIni.nClickLowVolume );
			}
			else if ( eMenu == Eメニュー種別.Drums )
			{
				list項目リスト.Add(ReturnToMenu.Drums);
				COptionEnum<EThreeState> AllAuto = new COptionEnum<EThreeState>(EThreeState.X);
				AllAuto.Initialize( "strCfgDrAllAuto", typeof(EThreeState));
				AllAuto.OnEnterDelegate = () =>
				{
					if ( AllAuto.Value == EThreeState.X )
					{
						AllAuto.OnNext();
					}
					CDTXMania.Instance.ConfigIni.bAutoPlay.Set( EPart.Drums, AllAuto );
				};
				list項目リスト.Add(AllAuto);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.bAutoPlay.LC);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.bAutoPlay.HH);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.bAutoPlay.HHO);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.bAutoPlay.SD);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.bAutoPlay.BD);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.bAutoPlay.HT);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.bAutoPlay.LT);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.bAutoPlay.FT);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.bAutoPlay.CY);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.bAutoPlay.RD);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.nScrollSpeed.Drums);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.eHitSoundPriorityHH);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.eHitSoundPriorityFT);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.eHitSoundPriorityCY);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.bTight);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.bFillin);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.bDrumsHitSound);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.nStrictHitSoundRange);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.bEmphasizePlaySound.Drums);
				list項目リスト.Add(DisplayMenu.Drums);
				list項目リスト.Add(EDrumsMenu.Drums);
				list項目リスト.Add(KeyAssignMenu.Drums);
			}
			else if (eMenu == Eメニュー種別.Guitar)
			{
				list項目リスト.Add(ReturnToMenu.Guitar);
				COptionEnum<EThreeState> AllAuto = new COptionEnum<EThreeState>(EThreeState.X);
				AllAuto.Initialize( "strCfgDrAllAuto", typeof(EThreeState));
				AllAuto.OnEnterDelegate = () =>
				{
					if ( AllAuto.Value == EThreeState.X )
					{
						AllAuto.OnNext();
					}
					CDTXMania.Instance.ConfigIni.bAutoPlay.Set( EPart.Guitar, AllAuto );
				};
				list項目リスト.Add(AllAuto);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.bAutoPlay.GtR);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.bAutoPlay.GtG);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.bAutoPlay.GtB);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.bAutoPlay.GtPick);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.bAutoPlay.GtWail);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.nScrollSpeed.Guitar);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.eRandom.Guitar);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.bLight.Guitar);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.bEmphasizePlaySound.Guitar);
				list項目リスト.Add(DisplayMenu.Guitar);
				list項目リスト.Add(KeyAssignMenu.Guitar);
			}
			else if (eMenu == Eメニュー種別.Bass)
			{
				list項目リスト.Add(ReturnToMenu.Bass);
				COptionEnum<EThreeState> AllAuto = new COptionEnum<EThreeState>(EThreeState.X);
				AllAuto.Initialize( "strCfgDrAllAuto", typeof(EThreeState));
				AllAuto.OnEnterDelegate = () =>
				{
					if ( AllAuto.Value == EThreeState.X )
					{
						AllAuto.OnNext();
					}
					CDTXMania.Instance.ConfigIni.bAutoPlay.Set( EPart.Bass, AllAuto );
				};
				list項目リスト.Add(AllAuto);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.bAutoPlay.BsR);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.bAutoPlay.BsG);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.bAutoPlay.BsB);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.bAutoPlay.BsPick);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.bAutoPlay.BsWail);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.nScrollSpeed.Bass);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.eRandom.Bass);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.bLight.Bass);
				list項目リスト.Add(CDTXMania.Instance.ConfigIni.bEmphasizePlaySound.Bass);
				list項目リスト.Add(DisplayMenu.Bass);
				list項目リスト.Add(KeyAssignMenu.Bass);
			}
			OnListMenuの初期化();
			n現在の選択項目 = 0;
			eメニュー種別 = eMenu;
		}

		public void OnNext()
		{
			CDTXMania.Instance.Skin.soundカーソル移動音.t再生する();
			if (b要素値にフォーカス中)
			{
				list項目リスト[n現在の選択項目].OnPrevious();
				t要素値を上下に変更中の処理();
			}
			else
			{
				n目標のスクロールカウンタ += 100;
			}
		}

		public void OnPrevious()
		{
			CDTXMania.Instance.Skin.soundカーソル移動音.t再生する();
			if (b要素値にフォーカス中)
			{
				list項目リスト[n現在の選択項目].OnNext();
				t要素値を上下に変更中の処理();
			}
			else
			{
				n目標のスクロールカウンタ -= 100;
			}
		}

		private void t要素値を上下に変更中の処理()
		{
			if (this.list項目リスト[this.n現在の選択項目] == CDTXMania.Instance.ConfigIni.nMasterVolume)
			{
				// #33700 2014.4.26 yyagi
				CDTXMania.Instance.Sound管理.nMasterVolume = CDTXMania.Instance.ConfigIni.nMasterVolume;
			}
		}

		public override void On活性化()
		{
			if (b活性化してない)
			{
				this.list項目リスト = new List<COptionBase>();
				this.eメニュー種別 = Eメニュー種別.Unknown;

				CResources cr = CDTXMania.Instance.Resources;
				string fontname = cr.Explanation("strCfgConfigurationItemsFontFileName");
				string path = Path.Combine(@"Graphics\fonts", fontname);
				this.prvFont = new CPrivateFastFont(CSkin.Path(path), (int)(18 * Scale.Y));
				this.b要素値にフォーカス中 = false;
				this.n目標のスクロールカウンタ = 0;
				this.n現在のスクロールカウンタ = 0;
				this.nスクロール用タイマ値 = -1;
				this.ct三角矢印アニメ = new CCounter();


				DisplayMenu = new STDGBSValue<COptionLabel>();
				KeyAssignMenu = new STDGBSValue<COptionLabel>();
				EDrumsMenu = new STDGBSValue<COptionLabel>();
				ReturnToMenu = new STDGBSValue<COptionLabel>();

				//CResources cr = CDTXMania.Instance.Resources;
				for ( EPart i = EPart.Drums; i <= EPart.System; ++i )
				{
					DisplayMenu[i] = new COptionLabel( "strCfgDisplayOption" );
					KeyAssignMenu[i] = new COptionLabel( "strCfgInputOption" );
					ReturnToMenu[i] = new COptionLabel( "strCfgReturnToMenu" );
				}

				EDrumsMenu[ EPart.Drums ] = new COptionLabel( "strCfgEDrumsOption" );
				HitRangeMenu = new COptionLabel( "strCfgHitRangeOption" );
				SoundMenu = new COptionLabel( "strCfgSoundOption" );

				DisplayMenu.Drums.OnEnterDelegate = () =>
				{
					t項目リストの設定(Eメニュー種別.DisplayDrums);
				};
				DisplayMenu.Guitar.OnEnterDelegate = () =>
				{
					t項目リストの設定(Eメニュー種別.DisplayGuitar);
				};
				DisplayMenu.Bass.OnEnterDelegate = () =>
				{
					t項目リストの設定(Eメニュー種別.DisplayBass);
				};
				DisplayMenu.System.OnEnterDelegate = () =>
				{
					t項目リストの設定(Eメニュー種別.DisplaySystem);
				};
				KeyAssignMenu.Drums.OnEnterDelegate = () =>
				{
					t項目リストの設定(Eメニュー種別.KeyAssignDrums);
				};
				KeyAssignMenu.Guitar.OnEnterDelegate = () =>
				{
					t項目リストの設定(Eメニュー種別.KeyAssignGuitar);
				};
				KeyAssignMenu.Bass.OnEnterDelegate = () =>
				{
					t項目リストの設定(Eメニュー種別.KeyAssignBass);
				};
				KeyAssignMenu.System.OnEnterDelegate = () =>
				{
					t項目リストの設定(Eメニュー種別.KeyAssignSystem);
				};

				ReturnToMenu.Drums.OnEnterDelegate = () =>
				{
					t項目リストの設定(Eメニュー種別.Drums);
				};
				ReturnToMenu.Guitar.OnEnterDelegate = () =>
				{
					t項目リストの設定(Eメニュー種別.Guitar);
				};
				ReturnToMenu.Bass.OnEnterDelegate = () =>
				{
					t項目リストの設定(Eメニュー種別.Bass);
				};
				ReturnToMenu.System.OnEnterDelegate = () =>
				{
					t項目リストの設定(Eメニュー種別.System);
				};
				EDrumsMenu.Drums.OnEnterDelegate = () =>
				{
					t項目リストの設定(Eメニュー種別.EDrumsSettings);
				};
				HitRangeMenu.OnEnterDelegate = () =>
				{
					t項目リストの設定( Eメニュー種別.HitRangeSettings );
				};
				SoundMenu.OnEnterDelegate = () =>
				{
					t項目リストの設定( Eメニュー種別.SoundSettings );
				};

				#region [ スキン選択肢と、現在選択中のスキン(index)の準備 #28195 2012.5.2 yyagi ]
				int ns = (CDTXMania.Instance.Skin.strSystemSkinSubfolders == null) ? 0 : CDTXMania.Instance.Skin.strSystemSkinSubfolders.Length;
				int nb = (CDTXMania.Instance.Skin.strBoxDefSkinSubfolders == null) ? 0 : CDTXMania.Instance.Skin.strBoxDefSkinSubfolders.Length;

				skinSubFolders = new string[ns + nb];
				skinInitial = CDTXMania.Instance.Skin.GetCurrentSkinSubfolderFullName(true);
				int initIdx = 0;
				for (int i = 0; i < ns; i++)
				{
					skinSubFolders[i] = CDTXMania.Instance.Skin.strSystemSkinSubfolders[i];
				}
				for (int i = 0; i < nb; i++)
				{
					skinSubFolders[ns + i] = CDTXMania.Instance.Skin.strBoxDefSkinSubfolders[i];
				}
				Array.Sort(skinSubFolders);
				skinNames = CSkin.GetSkinName(skinSubFolders);
				initIdx = Array.BinarySearch(skinSubFolders, skinInitial);
				if (initIdx < 0)
				{
					initIdx = 0;
				}
				skins = new COptionStringList(skinNames[initIdx]);
				skins.Initialize("Skin", Properties.Resources.strCfgSysSkin, skinNames);

				skins.OnEnterDelegate = () =>
				{
					CDTXMania.Instance.Skin.SetCurrentSkinSubfolderFullName(skinSubFolders[skins.Index], true);
					CDTXMania.Instance.ConfigIni.strSystemSkinSubfolderPath.Value = skinSubFolders[skins.Index];
					tGenerateSkinSample();
				};
				#endregion

				// CONFIG脱出時にこの値から変更されているようなら
				// サウンドデバイスを再構築する
				this.InitialSystemSoundType = CDTXMania.Instance.ConfigIni.nSoundDeviceType;
				this.InitialWASAPIBufferSizeMs = CDTXMania.Instance.ConfigIni.nWASAPIBufferSizeMs;
				this.InitialASIODevice = CDTXMania.Instance.ConfigIni.strASIODevice.Index;
				this.InitialUseOSTimer = CDTXMania.Instance.ConfigIni.bUseOSTimer;
				this.InitialWASAPIEventDriven = CDTXMania.Instance.ConfigIni.bEventDrivenWASAPI;

				this.InitialForceHighPower = CDTXMania.Instance.ConfigIni.bForceHighPowerPlan;

				// #27795 2012.3.11 yyagi; System設定の中でDrumsの設定を参照しているため、
				this.t項目リストの設定(Eメニュー種別.Bass);
				// 活性化の時点でDrumsの設定も入れ込んでおかないと、System設定中に例外発生することがある。
				this.t項目リストの設定(Eメニュー種別.Guitar);
				this.t項目リストの設定(Eメニュー種別.Drums);
				// 順番として、最後にSystemを持ってくること。設定一覧の初期位置がSystemのため。
				this.t項目リストの設定(Eメニュー種別.System);

				base.On活性化();
			}
		}

		public override void On非活性化()
		{
			if (b活性化してる)
			{
				CDTXMania.Instance.SaveConfig();
				this.list項目リスト.Clear();
				this.ct三角矢印アニメ = null;

				OnListMenuの解放();
				prvFont.Dispose();

				base.On非活性化();
				#region [ Skin変更 ]
				if (CDTXMania.Instance.Skin.GetCurrentSkinSubfolderFullName(true) != this.skinInitial)
				{
					CDTXMania.Instance.stageChangeSkin.tChangeSkinMain();   // #28195 2012.6.11 yyagi CONFIG脱出時にSkin更新
				}
				#endregion

				// #24820 2013.1.22 yyagi CONFIGでWASAPI/ASIO/DirectSound関連の設定を変更した場合、サウンドデバイスを再構築する。
				// #33689 2014.6.17 yyagi CONFIGでSoundTimerTypeの設定を変更した場合も、サウンドデバイスを再構築する。
				#region [ サウンドデバイス変更 ]
				if (InitialSystemSoundType != CDTXMania.Instance.ConfigIni.nSoundDeviceType ||
						InitialWASAPIBufferSizeMs != CDTXMania.Instance.ConfigIni.nWASAPIBufferSizeMs ||
						InitialASIODevice != CDTXMania.Instance.ConfigIni.strASIODevice.Index ||
						InitialUseOSTimer != CDTXMania.Instance.ConfigIni.bUseOSTimer ||
						InitialWASAPIEventDriven != CDTXMania.Instance.ConfigIni.bEventDrivenWASAPI )
				{
					ESoundDeviceType soundDeviceType;
					switch (CDTXMania.Instance.ConfigIni.nSoundDeviceType.Value)
					{
						case ESoundDeviceTypeForConfig.DSound:
							soundDeviceType = ESoundDeviceType.DirectSound;
							break;
						case ESoundDeviceTypeForConfig.ASIO:
							soundDeviceType = ESoundDeviceType.ASIO;
							break;
						case ESoundDeviceTypeForConfig.WASAPI_Exclusive:
							soundDeviceType = ESoundDeviceType.ExclusiveWASAPI;
							break;
						case ESoundDeviceTypeForConfig.WASAPI_Shared:
							soundDeviceType = ESoundDeviceType.SharedWASAPI;
							break;
						default:
							soundDeviceType = ESoundDeviceType.Unknown;
							break;
					}

					CDTXMania.Instance.Sound管理.t初期化(
							soundDeviceType,
							CDTXMania.Instance.ConfigIni.nWASAPIBufferSizeMs,
							CDTXMania.Instance.ConfigIni.bEventDrivenWASAPI,
							0,
							CDTXMania.Instance.ConfigIni.strASIODevice.Index,
							CDTXMania.Instance.ConfigIni.bUseOSTimer);
					CDTXMania.Instance.ShowWindowTitleWithSoundType();
				}
				#endregion

				#region [ メトロノーム音 音量設定 ]
				CDTXMania.Instance.Skin.soundClickHigh.n音量 = CDTXMania.Instance.ConfigIni.nClickHighVolume;
				CDTXMania.Instance.Skin.soundClickLow.n音量 = CDTXMania.Instance.ConfigIni.nClickLowVolume;
				CDTXMania.Instance.Skin.soundClickBottom.n音量 = CDTXMania.Instance.ConfigIni.nClickLowVolume;
				#endregion

				#region [ サウンドのタイムストレッチモード変更 ]

				FDK.CSound管理.bIsTimeStretch = CDTXMania.Instance.ConfigIni.bTimeStretch.Value;

				#endregion
				#region [ 電源プラン変更 ]
				if ( CDTXMania.Instance.ConfigIni.bForceHighPowerPlan )
				{
					CPowerPlan.ChangeHighPerformance();
				}
				else
				{
					// HighPower=OFFを維持したとき、またはONからOFFにしたときは、
					// 特に電源プランの変更をしない。
					// 電源プランの復元は、アプリ終了時に行う。
					// CPowerPlan.RestoreCurrentPowerPlan();
				}
				#endregion
			}
		}

		public override void OnManagedリソースの作成()
		{
			if (b活性化してる)
			{
				this.tx通常項目行パネル = TextureFactory.tテクスチャの生成Af(CSkin.Path(@"Graphics\ScreenConfig itembox.png"), false);
				this.txその他項目行パネル = TextureFactory.tテクスチャの生成Af(CSkin.Path(@"Graphics\ScreenConfig itembox other.png"), false);
				this.tx三角矢印 = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenConfig triangle arrow.png"), false);
				// スキン選択時に動的に設定するため、ここでは初期化しない
				this.txSkinSample = null;
				OnListMenuの初期化();
				base.OnManagedリソースの作成();
			}
		}

		public override void OnManagedリソースの解放()
		{
			if (b活性化してる)
			{
				TextureFactory.tテクスチャの解放(ref this.txSkinSample);
				TextureFactory.tテクスチャの解放(ref this.tx通常項目行パネル);
				TextureFactory.tテクスチャの解放(ref this.txその他項目行パネル);
				TextureFactory.tテクスチャの解放(ref this.tx三角矢印);

				OnListMenuの解放();

				base.OnManagedリソースの解放();
			}
		}

		private void OnListMenuの初期化()
		{
			OnListMenuの解放();
			this.listMenu = new stMenuItemRight[this.list項目リスト.Count];
		}

		/// <summary>
		/// 事前にレンダリングしておいたテクスチャを解放する。
		/// </summary>
		private void OnListMenuの解放()
		{
			if (listMenu != null)
			{
				for (int i = 0; i < listMenu.Length; i++)
				{
					TextureFactory.tテクスチャの解放(ref listMenu[i].txParam);
					TextureFactory.tテクスチャの解放(ref listMenu[i].txParamColored);
					TextureFactory.tテクスチャの解放(ref listMenu[i].txMenuItemRight);
				}
				this.listMenu = null;
			}
		}

		public override int On進行描画()
		{
			throw new InvalidOperationException("t進行描画(bool)のほうを使用してください。");
		}

		public int t進行描画(bool b項目リスト側にフォーカスがある)
		{
			if (this.b活性化してる)
			{
				#region [ 初めての進行描画 ]
				//-----------------
				if (base.b初めての進行描画)
				{
					this.nスクロール用タイマ値 = CSound管理.rc演奏用タイマ.n現在時刻;
					this.ct三角矢印アニメ.t開始(0, 9, 50, CDTXMania.Instance.Timer);

					base.b初めての進行描画 = false;
				}
				//-----------------
				#endregion

				this.b項目リスト側にフォーカスがある = b項目リスト側にフォーカスがある;       // 記憶

				#region [ 項目スクロールの進行 ]
				//-----------------
				long n現在時刻 = CDTXMania.Instance.Timer.n現在時刻;
				if (n現在時刻 < this.nスクロール用タイマ値) this.nスクロール用タイマ値 = n現在時刻;

				const int INTERVAL = 2; // [ms]
				while ((n現在時刻 - this.nスクロール用タイマ値) >= INTERVAL)
				{
					int n目標項目までのスクロール量 = Math.Abs((int)(this.n目標のスクロールカウンタ - this.n現在のスクロールカウンタ));
					int n加速度 = 0;

					#region [ n加速度の決定；目標まで遠いほど加速する。]
					//-----------------
					if (n目標項目までのスクロール量 <= 100)
					{
						n加速度 = 2;
					}
					else if (n目標項目までのスクロール量 <= 300)
					{
						n加速度 = 3;
					}
					else if (n目標項目までのスクロール量 <= 500)
					{
						n加速度 = 4;
					}
					else
					{
						n加速度 = 8;
					}
					//-----------------
					#endregion
					#region [ this.n現在のスクロールカウンタに n加速度 を加減算。]
					//-----------------
					if (this.n現在のスクロールカウンタ < this.n目標のスクロールカウンタ)
					{
						this.n現在のスクロールカウンタ += n加速度;
						if (this.n現在のスクロールカウンタ > this.n目標のスクロールカウンタ)
						{
							// 目標を超えたら目標値で停止。
							this.n現在のスクロールカウンタ = this.n目標のスクロールカウンタ;
						}
					}
					else if (this.n現在のスクロールカウンタ > this.n目標のスクロールカウンタ)
					{
						this.n現在のスクロールカウンタ -= n加速度;
						if (this.n現在のスクロールカウンタ < this.n目標のスクロールカウンタ)
						{
							// 目標を超えたら目標値で停止。
							this.n現在のスクロールカウンタ = this.n目標のスクロールカウンタ;
						}
					}
					//-----------------
					#endregion
					#region [ 行超え処理、ならびに目標位置に到達したらスクロールを停止して項目変更通知を発行。]
					//-----------------
					if (this.n現在のスクロールカウンタ >= 100)
					{
						this.n現在の選択項目 = this.t次の項目(this.n現在の選択項目);
						this.n現在のスクロールカウンタ -= 100;
						this.n目標のスクロールカウンタ -= 100;
						if (this.n目標のスクロールカウンタ == 0)
						{
							CDTXMania.Instance.stageコンフィグ.t項目変更通知();
						}
					}
					else if (this.n現在のスクロールカウンタ <= -100)
					{
						this.n現在の選択項目 = this.t前の項目(this.n現在の選択項目);
						this.n現在のスクロールカウンタ += 100;
						this.n目標のスクロールカウンタ += 100;
						if (this.n目標のスクロールカウンタ == 0)
						{
							CDTXMania.Instance.stageコンフィグ.t項目変更通知();
						}
					}
					//-----------------
					#endregion

					this.nスクロール用タイマ値 += INTERVAL;
				}
				//-----------------
				#endregion

				#region [ ▲印アニメの進行 ]
				if (this.b項目リスト側にフォーカスがある && (this.n目標のスクロールカウンタ == 0))
				{
					this.ct三角矢印アニメ.t進行Loop();
				}
				#endregion


				// 描画

				// メニューにフォーカスがあるなら、項目リストの中央は頭を出さない。
				this.ptパネルの基本座標[4].X = this.b項目リスト側にフォーカスがある ? 276 : 301;

				#region [ 計11個の項目パネルを描画する。]
				//-----------------
				int nItem = this.n現在の選択項目;
				for (int i = 0; i < 4; i++)
				{
					nItem = this.t前の項目(nItem);
				}

				for (int n行番号 = -4; n行番号 < 6; n行番号++)
				{
					// n行番号 == 0 がフォーカスされている項目パネル。
					#region [ 今まさに画面外に飛びだそうとしている項目パネルは描画しない。]
					if (((n行番号 == -4) && (this.n現在のスクロールカウンタ > 0)) ||
							((n行番号 == +5) && (this.n現在のスクロールカウンタ < 0)))
					{
						nItem = this.t次の項目(nItem);
						continue;
					}
					#endregion

					int n移動元の行の基本位置 = n行番号 + 4;
					int n移動先の行の基本位置 = (this.n現在のスクロールカウンタ <= 0) ? ((n移動元の行の基本位置 + 1) % 10) : (((n移動元の行の基本位置 - 1) + 10) % 10);
					int x = this.ptパネルの基本座標[n移動元の行の基本位置].X + ((int)((this.ptパネルの基本座標[n移動先の行の基本位置].X - this.ptパネルの基本座標[n移動元の行の基本位置].X) * (((double)Math.Abs(this.n現在のスクロールカウンタ)) / 100.0)));
					int y = this.ptパネルの基本座標[n移動元の行の基本位置].Y + ((int)((this.ptパネルの基本座標[n移動先の行の基本位置].Y - this.ptパネルの基本座標[n移動元の行の基本位置].Y) * (((double)Math.Abs(this.n現在のスクロールカウンタ)) / 100.0)));

					#region [ 現在の行の項目パネル枠を描画。]
					if (this.list項目リスト[nItem].type == EOptionType.Normal)
					{
						if (this.tx通常項目行パネル != null)
						{
							this.tx通常項目行パネル.t2D描画(CDTXMania.Instance.Device, x * Scale.X, y * Scale.Y);
						}
					}
					else if (this.list項目リスト[nItem].type == EOptionType.Other)
					{
						if (this.txその他項目行パネル != null)
						{
							this.txその他項目行パネル.t2D描画(CDTXMania.Instance.Device, x * Scale.X, y * Scale.Y);
						}
					}
					#endregion

					#region [ 現在の行の項目名を描画。]
					if (listMenu[nItem].txMenuItemRight != null)
					{
						// 自前のキャッシュに含まれているようなら、再レンダリングせずキャッシュを使用
						listMenu[nItem].txMenuItemRight.t2D描画(CDTXMania.Instance.Device, (x + 0x12) * Scale.X, (y + 12) * Scale.Y - 20);
					}
					else
					{
						using (Bitmap bmpItem = prvFont.DrawPrivateFont(this.list項目リスト[nItem].label, Color.White, Color.Black))
						{
							TextureFactory.tテクスチャの解放(ref listMenu[nItem].txMenuItemRight);
							listMenu[nItem].txMenuItemRight = TextureFactory.tテクスチャの生成(bmpItem);
						}
					}
					#endregion

					#region [ 現在の行の項目の要素を描画。]
					string strParam = this.list項目リスト[nItem].ToString();
					//bool b強調 = false;

					#region [ 最初にSkinの選択肢にきたとき(Enterを押す前)に限り、サンプル生成が発生する。 #28195 2012.5.2 yyagi ]
					if (this.list項目リスト[this.n現在の選択項目] == skins)
					{
						if (txSkinSample == null)
						{
							tGenerateSkinSample();
						}
						if (txSkinSample != null)
						{
							txSkinSample.t2D描画(CDTXMania.Instance.Device, 56 * Scale.X, 300 * Scale.Y);
						}
					}
					#endregion

					#endregion
					int nIndex = this.list項目リスト[nItem].Index;
					bool bColored = list項目リスト[nItem] is COptionInteger && n行番号 == 0 && b要素値にフォーカス中;
					if (listMenu[nItem].nParam != nIndex ||
						(listMenu[nItem].txParam == null) ||
						(bColored && listMenu[nItem].txParamColored == null))
					{
						listMenu[nItem].nParam = nIndex;
						listMenu[nItem].strParam = this.list項目リスト[nItem].ToString();
						using (Bitmap bmpStr = prvFont.DrawPrivateFont(strParam, Color.White, Color.Black))
						{
							TextureFactory.tテクスチャの解放(ref listMenu[nItem].txParam);
							listMenu[nItem].txParam = TextureFactory.tテクスチャの生成(bmpStr, false);
							if (bColored)
							{
								using (Bitmap bmpStrColored = prvFont.DrawPrivateFont(strParam, Color.White, Color.Black, Color.Yellow, Color.OrangeRed))
								{
									TextureFactory.tテクスチャの解放(ref listMenu[nItem].txParamColored);
									listMenu[nItem].txParamColored = TextureFactory.tテクスチャの生成(bmpStrColored, false);
								}
							}
						}
					}

					if (bColored)
					{
						listMenu[nItem].txParamColored.t2D描画(CDTXMania.Instance.Device, (x + 210) * Scale.X, (y + 12) * Scale.Y - 20);
					}
					else
					{
						listMenu[nItem].txParam.t2D描画(CDTXMania.Instance.Device, (x + 210) * Scale.X, (y + 12) * Scale.Y - 20);
					}
					nItem = this.t次の項目(nItem);
				}
				#endregion

				#region [ 項目リストにフォーカスがあって、かつスクロールが停止しているなら、パネルの上下に▲印を描画する。]
				if (this.b項目リスト側にフォーカスがある && (this.n目標のスクロールカウンタ == 0))
				{
					int x;
					int y_upper;
					int y_lower;

					// 位置決定。

					if (this.b要素値にフォーカス中)
					{
						// 要素値の上下あたり。
						x = 528;
						y_upper = 198 - this.ct三角矢印アニメ.n現在の値;
						y_lower = 242 + this.ct三角矢印アニメ.n現在の値;
					}
					else
					{
						// 項目名の上下あたり。
						x = 276;
						y_upper = 186 - this.ct三角矢印アニメ.n現在の値;
						y_lower = 254 + this.ct三角矢印アニメ.n現在の値;
					}

					// 描画。

					if (this.tx三角矢印 != null)
					{
						this.tx三角矢印.t2D描画(CDTXMania.Instance.Device, x * Scale.X,
							y_upper * Scale.Y,
							new Rectangle(0, 0, (int)(32 * Scale.X), (int)(16 * Scale.Y)));
						this.tx三角矢印.t2D描画(CDTXMania.Instance.Device, x * Scale.X,
							y_lower * Scale.Y,
							new Rectangle(0, (int)(16 * Scale.Y), (int)(32 * Scale.X), (int)(16 * Scale.Y)));
					}
				}
				#endregion
			}
			return 0;
		}


		// その他
		internal enum Eメニュー種別
		{
			System,
			Drums,
			Guitar,
			Bass,
			// #24609 2011.4.12 yyagi: 画面キャプチャキーのアサイン
			KeyAssignSystem,
			KeyAssignDrums,
			KeyAssignGuitar,
			KeyAssignBass,
			DisplaySystem,
			DisplayDrums,
			DisplayGuitar,
			DisplayBass,
			EDrumsSettings,
			HitRangeSettings,
			SoundSettings,
			Unknown
		}

		private int t前の項目(int nItem)
		{
			if (--nItem < 0)
			{
				nItem = this.list項目リスト.Count - 1;
			}
			return nItem;
		}

		private int t次の項目(int nItem)
		{
			if (++nItem >= this.list項目リスト.Count)
			{
				nItem = 0;
			}
			return nItem;
		}
	}
}
