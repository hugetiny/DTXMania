using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using FDK;
using SharpDX;
using SharpDX.DirectInput;

using Rectangle = System.Drawing.Rectangle;
using Color = System.Drawing.Color;

namespace DTXMania
{
	internal class CActConfigKeyAssign : CActivity
	{
		public bool bキー入力待ちの最中である
		{
			get
			{
				return this.bキー入力待ち;
			}
		}

		public void t開始(EPad pad, string strパッド名)
		{
			this.pad = pad;
			this.strパッド名 = strパッド名;
			for (int i = 0; i < CConfigXml.AssignableCodes - 2; i++)
			{
				this.structReset用KeyAssign[i].InputDevice = CDTXMania.Instance.ConfigIni.KeyAssign[pad][i].入力デバイス;
				this.structReset用KeyAssign[i].ID = CDTXMania.Instance.ConfigIni.KeyAssign[pad][i].ID;
				this.structReset用KeyAssign[i].Code = CDTXMania.Instance.ConfigIni.KeyAssign[pad][i].コード;
			}
		}

		public void tEnter押下()
		{
			if (!this.bキー入力待ち)
			{
				CDTXMania.Instance.Skin.sound決定音.t再生する();
				if (ptr == CConfigXml.AssignableCodes - 2)
				{
					for (int i = 0; i < CConfigXml.AssignableCodes - 2; i++)
					{
						CDTXMania.Instance.ConfigIni.KeyAssign[pad][i].CopyFrom(this.structReset用KeyAssign[i]);
					}
					return;
				}
				else if (ptr == CConfigXml.AssignableCodes - 1)
				{

					CDTXMania.Instance.stageコンフィグ.tアサイン完了通知();
					return;
				}
				this.bキー入力待ち = true;
			}
		}

		public void OnNext()
		{
			if (!this.bキー入力待ち)
			{
				CDTXMania.Instance.Skin.soundカーソル移動音.t再生する();
				ptr++;
				ptr %= CConfigXml.AssignableCodes;
			}
		}

		public void OnPrevious()
		{
			if (!this.bキー入力待ち)
			{
				CDTXMania.Instance.Skin.soundカーソル移動音.t再生する();
				--ptr;
				ptr += CConfigXml.AssignableCodes;
				ptr %= CConfigXml.AssignableCodes;
			}
		}

		public override void On活性化()
		{
			if (base.b活性化してない)
			{
				this.pad = EPad.Unknown;
				this.strパッド名 = "";
				this.ptr = 0;
				this.bキー入力待ち = false;
				this.structReset用KeyAssign = new CKeyAssign[CConfigXml.AssignableCodes - 2];
				for (int i = 0; i < this.structReset用KeyAssign.Length; ++i)
				{
					structReset用KeyAssign[i] = new CKeyAssign(EInputDevice.Unknown, 0, 0);
				}

				base.On活性化();
			}
		}

		public override void On非活性化()
		{
			if (base.b活性化してる)
			{
				TextureFactory.tテクスチャの解放(ref this.txカーソル);
				TextureFactory.tテクスチャの解放(ref this.txHitKeyダイアログ);
				base.On非活性化();
			}
		}

		public override void OnManagedリソースの作成()
		{
			if (base.b活性化してる)
			{
				this.prvFont = new CPrivateFastFont(CSkin.Path(@"Graphics\fonts\mplus-1p-heavy.ttf"), (int)(18 * Scale.Y)); // t項目リストの設定 の前に必要
				this.txカーソル = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenConfig menu cursor.png"), false);
				this.txHitKeyダイアログ = TextureFactory.tテクスチャの生成(CSkin.Path(@"Graphics\ScreenConfig hit key to assign dialog.png"), false);
				base.OnManagedリソースの作成();
			}
		}

		public override void OnManagedリソースの解放()
		{
			if (base.b活性化してる)
			{
				TextureFactory.t安全にDisposeする(ref prvFont);
			}
		}

		public override int On進行描画()
		{
			if (base.b活性化してる)
			{
				if (this.bキー入力待ち)
				{
					if (CDTXMania.Instance.Input管理.Keyboard.bキーが押された((int)SharpDX.DirectInput.Key.Escape))
					{
						CDTXMania.Instance.Skin.sound取消音.t再生する();
						this.bキー入力待ち = false;
						CDTXMania.Instance.Input管理.tポーリング(CDTXMania.Instance.bApplicationActive, false);
					}
					else if ((this.tキーチェックとアサイン_Keyboard() || this.tキーチェックとアサイン_MidiIn()) || (this.tキーチェックとアサイン_Joypad() || this.tキーチェックとアサイン_Mouse()))
					{
						this.bキー入力待ち = false;
						CDTXMania.Instance.Input管理.tポーリング(CDTXMania.Instance.bApplicationActive, false);
					}
				}
				else if (CDTXMania.Instance.Input管理.Keyboard.bキーが押された((int)SharpDX.DirectInput.Key.Delete))
				{
					CDTXMania.Instance.Skin.sound決定音.t再生する();
					CDTXMania.Instance.ConfigIni.KeyAssign[this.pad][ptr].Reset();
				}

				if (this.txカーソル != null)
				{
					int num = 20;
					int num2 = 0x144;
					int num3 = 0x3e + (num * (ptr + 1));
					this.txカーソル.vc拡大縮小倍率 = new Vector3(1f, 0.75f, 1f);
					this.txカーソル.t2D描画(CDTXMania.Instance.Device, num2 * Scale.X, num3 * Scale.Y - 6, new Rectangle(0, 0, (int)(0x10 * Scale.X), (int)(0x20 * Scale.Y)));
					num2 += 0x10;
					Rectangle rectangle = new Rectangle((int)(8 * Scale.X), 0, (int)(0x10 * Scale.X), (int)(0x20 * Scale.Y));
					for (int j = 0; j < 14; j++)
					{
						this.txカーソル.t2D描画(CDTXMania.Instance.Device, num2 * Scale.X, num3 * Scale.Y - 6, rectangle);
						num2 += 0x10;
					}
					this.txカーソル.t2D描画(CDTXMania.Instance.Device, num2 * Scale.X, num3 * Scale.Y - 6, new Rectangle((int)(0x10 * Scale.X), 0, (int)(0x10 * Scale.X), (int)(0x20 * Scale.Y)));
				}

				int num5 = 20;
				int x = 0x134;
				int y = 0x40;
				using (Bitmap bmpStr = prvFont.DrawPrivateFont(this.strパッド名, Color.White, Color.Black))
				{
					CTexture txStr = TextureFactory.tテクスチャの生成(bmpStr);
					txStr.vc拡大縮小倍率 = new Vector3(0.75f, 0.75f, 1f);
					txStr.t2D描画(CDTXMania.Instance.Device, x * Scale.X, y * Scale.Y - 20);
					TextureFactory.tテクスチャの解放(ref txStr);
				}

				y += num5;
				string strParam;
				bool b強調;
				for (int i = 0; i < CConfigXml.AssignableCodes - 2; i++)
				{
					COptionKeyAssign stkeyassignArray = CDTXMania.Instance.ConfigIni.KeyAssign[this.pad][i];
					switch (stkeyassignArray.入力デバイス)
					{
						case EInputDevice.Keyboard:
							this.tアサインコードの描画_Keyboard(i + 1, x + 20, y, stkeyassignArray.ID, stkeyassignArray.コード, ptr == i);
							break;

						case EInputDevice.MIDIIn:
							this.tアサインコードの描画_MidiIn(i + 1, x + 20, y, stkeyassignArray.ID, stkeyassignArray.コード, ptr == i);
							break;

						case EInputDevice.JoyPad:
							this.tアサインコードの描画_Joypad(i + 1, x + 20, y, stkeyassignArray.ID, stkeyassignArray.コード, ptr == i);
							break;

						case EInputDevice.Mouse:
							this.tアサインコードの描画_Mouse(i + 1, x + 20, y, stkeyassignArray.ID, stkeyassignArray.コード, ptr == i);
							break;

						default:
							strParam = string.Format("{0,2}.", i + 1);
							b強調 = (ptr == i);
							using (Bitmap bmpStr = b強調 ?
									prvFont.DrawPrivateFont(strParam, Color.White, Color.Black, Color.Yellow, Color.OrangeRed) :
									prvFont.DrawPrivateFont(strParam, Color.White, Color.Black))
							{
								CTexture txStr = TextureFactory.tテクスチャの生成(bmpStr, false);
								txStr.vc拡大縮小倍率 = new Vector3(0.75f, 0.75f, 1f);
								txStr.t2D描画(CDTXMania.Instance.Device, (x + 20) * Scale.X, y * Scale.Y - 20);
								TextureFactory.tテクスチャの解放(ref txStr);
							}
							break;
					}
					y += num5;
				}

				strParam = "Reset All Assign";
				b強調 = (ptr == CConfigXml.AssignableCodes - 2);
				using (Bitmap bmpStr = b強調 ?
						prvFont.DrawPrivateFont(strParam, Color.White, Color.Black, Color.Yellow, Color.OrangeRed) :
						prvFont.DrawPrivateFont(strParam, Color.White, Color.Black))
				{
					CTexture txStr = TextureFactory.tテクスチャの生成(bmpStr, false);
					txStr.vc拡大縮小倍率 = new Vector3(0.75f, 0.75f, 1f);
					txStr.t2D描画(CDTXMania.Instance.Device, (x + 20) * Scale.X, y * Scale.Y - 20);
					TextureFactory.tテクスチャの解放(ref txStr);
				}

				y += num5;
				strParam = "<< Returnto List";
				b強調 = (ptr == CConfigXml.AssignableCodes - 1);
				using (Bitmap bmpStr = b強調 ?
						prvFont.DrawPrivateFont(strParam, Color.White, Color.Black, Color.Yellow, Color.OrangeRed) :
						prvFont.DrawPrivateFont(strParam, Color.White, Color.Black))
				{
					CTexture txStr = TextureFactory.tテクスチャの生成(bmpStr, false);
					txStr.vc拡大縮小倍率 = new Vector3(0.75f, 0.75f, 1f);
					txStr.t2D描画(CDTXMania.Instance.Device, (x + 20) * Scale.X, y * Scale.Y - 20);
					TextureFactory.tテクスチャの解放(ref txStr);
				}

				if (this.bキー入力待ち && (this.txHitKeyダイアログ != null))
				{
					this.txHitKeyダイアログ.t2D描画(CDTXMania.Instance.Device, 0x185 * Scale.X, 0xd7 * Scale.Y);
				}
			}
			return 0;
		}


		// その他
		[StructLayout(LayoutKind.Sequential)]
		private struct STKEYLABEL
		{
			public int nCode;
			public string strLabel;
			public STKEYLABEL(int nCode, string strLabel)
			{
				this.nCode = nCode;
				this.strLabel = strLabel;
			}
		}

		private bool bキー入力待ち;

		private STKEYLABEL[] KeyLabel = new STKEYLABEL[] {
			#region [ *** ]
			new STKEYLABEL((int)Key.Escape, "[ESC]"),
			new STKEYLABEL((int)Key.D1, "[ 1 ]"),
			new STKEYLABEL((int)Key.D2, "[ 2 ]"),
			new STKEYLABEL((int)Key.D3, "[ 3 ]"),
			new STKEYLABEL((int)Key.D4, "[ 4 ]"),
			new STKEYLABEL((int)Key.D5, "[ 5 ]"),
			new STKEYLABEL((int)Key.D6, "[ 6 ]"),
			new STKEYLABEL((int)Key.D7, "[ 7 ]"),
			new STKEYLABEL((int)Key.D8, "[ 8 ]"),
			new STKEYLABEL((int)Key.D9, "[ 9 ]"),
			new STKEYLABEL((int)Key.D0, "[ 0 ]"),
			new STKEYLABEL((int)Key.Minus, "[ - ]"),
			new STKEYLABEL((int)Key.Equals, "[ = ]"),
			new STKEYLABEL((int)Key.Back, "[BSC]"),
			new STKEYLABEL((int)Key.Tab, "[TAB]"),
			new STKEYLABEL((int)Key.Q, "[ Q ]"),
			new STKEYLABEL((int)Key.W, "[ W ]"),
			new STKEYLABEL((int)Key.E, "[ E ]"),
			new STKEYLABEL((int)Key.R, "[ R ]"),
			new STKEYLABEL((int)Key.T, "[ T ]"),
			new STKEYLABEL((int)Key.Y, "[ Y ]"),
			new STKEYLABEL((int)Key.U, "[ U ]"),
			new STKEYLABEL((int)Key.I, "[ I ]"),
			new STKEYLABEL((int)Key.O, "[ O ]"),
			new STKEYLABEL((int)Key.P, "[ P ]"),
			new STKEYLABEL((int)Key.LeftBracket, "[ [ ]"),
			new STKEYLABEL((int)Key.RightBracket, "[ ] ]"),
			new STKEYLABEL((int)Key.Return, "[Enter]"),
			new STKEYLABEL((int)Key.LeftControl, "[L-Ctrl]"),
			new STKEYLABEL((int)Key.A, "[ A ]"),
			new STKEYLABEL((int)Key.S, "[ S ]"),
			new STKEYLABEL((int)Key.D, "[ D ]"),
			new STKEYLABEL((int)Key.F, "[ F ]"),
			new STKEYLABEL((int)Key.G, "[ G ]"),
			new STKEYLABEL((int)Key.H, "[ H ]"),
			new STKEYLABEL((int)Key.J, "[ J ]"),
			new STKEYLABEL((int)Key.K, "[ K ]"),
			new STKEYLABEL((int)Key.L, "[ L ]"),
			new STKEYLABEL((int)Key.Semicolon, "[ ; ]"),
			new STKEYLABEL((int)Key.Apostrophe, "[ ' ]"),
			new STKEYLABEL((int)Key.Grave, "[ ` ]"),
			new STKEYLABEL((int)Key.LeftShift, "[L-Shift]"),
			new STKEYLABEL((int)Key.Backslash, @"[ \]"),
			new STKEYLABEL((int)Key.Z, "[ Z ]"),
			new STKEYLABEL((int)Key.X, "[ X ]"),
			new STKEYLABEL((int)Key.C, "[ C ]"),
			new STKEYLABEL((int)Key.V, "[ V ]"),
			new STKEYLABEL((int)Key.B, "[ B ]"),
			new STKEYLABEL((int)Key.N, "[ N ]"),
			new STKEYLABEL((int)Key.M, "[ M ]"),
			new STKEYLABEL((int)Key.Comma, "[ , ]"),
			new STKEYLABEL((int)Key.Period, "[ . ]"),
			new STKEYLABEL((int)Key.Slash, "[ / ]"),
			new STKEYLABEL((int)Key.RightShift, "[R-Shift]"),
			new STKEYLABEL((int)Key.Multiply, "[ * ]"),
			new STKEYLABEL((int)Key.LeftAlt, "[L-Alt]"),
			new STKEYLABEL((int)Key.Space, "[Space]"),
			new STKEYLABEL((int)Key.Capital, "[CAPS]"),
			new STKEYLABEL((int)Key.F1, "[F1]"),
			new STKEYLABEL((int)Key.F2, "[F2]"),
			new STKEYLABEL((int)Key.F3, "[F3]"),
			new STKEYLABEL((int)Key.F4, "[F4]"),
			new STKEYLABEL((int)Key.F5, "[F5]"),
			new STKEYLABEL((int)Key.F6, "[F6]"),
			new STKEYLABEL((int)Key.F7, "[F7]"),
			new STKEYLABEL((int)Key.F8, "[F8]"),
			new STKEYLABEL((int)Key.F9, "[F9]"),
			new STKEYLABEL((int)Key.F10, "[F10]"),
			new STKEYLABEL((int)Key.NumberLock, "[NumLock]"),
			new STKEYLABEL((int)Key.ScrollLock, "[Scroll]"),
			new STKEYLABEL((int)Key.NumberPad7, "[NPad7]"),
			new STKEYLABEL((int)Key.NumberPad8, "[NPad8]"),
			new STKEYLABEL((int)Key.NumberPad9, "[NPad9]"),
			new STKEYLABEL((int)Key.Subtract, "[NPad-]"),
			new STKEYLABEL((int)Key.NumberPad4, "[NPad4]"),
			new STKEYLABEL((int)Key.NumberPad5, "[NPad5]"),
			new STKEYLABEL((int)Key.NumberPad6, "[NPad6]"),
			new STKEYLABEL((int)Key.Add, "[NPad+]"),
			new STKEYLABEL((int)Key.NumberPad1, "[NPad1]"),
			new STKEYLABEL((int)Key.NumberPad2, "[NPad2]"),
			new STKEYLABEL((int)Key.NumberPad3, "[NPad3]"),
			new STKEYLABEL((int)Key.NumberPad0, "[NPad0]"),
			new STKEYLABEL((int)Key.Decimal, "[NPad.]"),
			new STKEYLABEL((int)Key.F11, "[F11]"),
			new STKEYLABEL((int)Key.F12, "[F12]"),
			new STKEYLABEL((int)Key.F13, "[F13]"),
			new STKEYLABEL((int)Key.F14, "[F14]"),
			new STKEYLABEL((int)Key.F15, "[F15]"),
			new STKEYLABEL((int)Key.Kana, "[Kana]"),
			new STKEYLABEL((int)Key.AbntC1, "[ ? ]"),
			new STKEYLABEL((int)Key.Convert, "[Henkan]"),
			new STKEYLABEL((int)Key.NoConvert, "[MuHenkan]"),
			new STKEYLABEL((int)Key.Backslash, @"[ \ ]"),
			new STKEYLABEL((int)Key.AbntC2, "[NPad.]"),
			new STKEYLABEL((int)Key.NumberPadEquals, "[NPad=]"),
			new STKEYLABEL((int)Key.PreviousTrack, "[ ^ ]"),	// DIK_CIRCUMFLEX = 0x90
			new STKEYLABEL((int)Key.AT, "[ @ ]"),
			new STKEYLABEL((int)Key.Colon, "[ : ]"),
			new STKEYLABEL((int)Key.Underline, "[ _ ]"),
			new STKEYLABEL((int)Key.Kanji, "[Kanji]"),
			new STKEYLABEL((int)Key.Stop, "[Stop]"),
			new STKEYLABEL((int)Key.AX, "[AX]"),
			new STKEYLABEL((int)Key.NumberPadEnter, "[NPEnter]"),
			new STKEYLABEL((int)Key.RightControl, "[R-Ctrl]"),
			new STKEYLABEL((int)Key.Mute, "[Mute]"),
			new STKEYLABEL((int)Key.Calculator, "[Calc]"),
			new STKEYLABEL((int)Key.PlayPause, "[PlayPause]"),
			new STKEYLABEL((int)Key.MediaStop, "[MediaStop]"),
			new STKEYLABEL((int)Key.VolumeDown, "[Volume-]"),
			new STKEYLABEL((int)Key.VolumeUp, "[Volume+]"),
			new STKEYLABEL((int)Key.WebHome, "[WebHome]"),
			new STKEYLABEL((int)Key.NumberPadComma, "[NPad,]"),
			new STKEYLABEL((int)Key.Divide, "[ / ]"),
			new STKEYLABEL((int)Key.PrintScreen, "[PrtScn]"),
			new STKEYLABEL((int)Key.RightAlt, "[R-Alt]"),
			new STKEYLABEL((int)Key.Pause, "[Pause]"),
			new STKEYLABEL((int)Key.Home, "[Home]"),
			new STKEYLABEL((int)Key.Up, "[Up]"),
			new STKEYLABEL((int)Key.PageUp, "[PageUp]"),
			new STKEYLABEL((int)Key.Left, "[Left]"),
			new STKEYLABEL((int)Key.Right, "[Right]"),
			new STKEYLABEL((int)Key.End, "[End]"),
			new STKEYLABEL((int)Key.Down, "[Down]"),
			new STKEYLABEL((int)Key.PageDown, "[PageDown]"),
			new STKEYLABEL((int)Key.Insert, "[Insert]"),
			new STKEYLABEL((int)Key.Delete, "[Delete]"),
			new STKEYLABEL((int)Key.LeftWindowsKey, "[L-Win]"),
			new STKEYLABEL((int)Key.RightWindowsKey, "[R-Win]"),
			new STKEYLABEL((int)Key.Applications, "[APP]"),
			new STKEYLABEL((int)Key.Power, "[Power]"),
			new STKEYLABEL((int)Key.Sleep, "[Sleep]"),
			new STKEYLABEL((int)Key.Wake, "[Wake]"),
			#endregion
		};

		private EPad pad;
		int ptr;
		private CKeyAssign[] structReset用KeyAssign;
		private string strパッド名;
		private CTexture txHitKeyダイアログ;
		private CTexture txカーソル;
		private CPrivateFastFont prvFont;

		private void tアサインコードの描画_Joypad(int line, int x, int y, int nID, int nCode, bool b強調)
		{
			string str = "";
			switch (nCode)
			{
				case 0:
					str = "Left";
					break;

				case 1:
					str = "Right";
					break;

				case 2:
					str = "Up";
					break;

				case 3:
					str = "Down";
					break;

				case 4:
					str = "Forward";
					break;

				case 5:
					str = "Back";
					break;

				default:
					if ((6 <= nCode) && (nCode < 6 + 128))              // other buttons (128 types)
					{
						str = string.Format("Button{0}", nCode - 5);
					}
					else if ((6 + 128 <= nCode) && (nCode < 6 + 128 + 8))       // POV HAT ( 8 types; 45 degrees per HATs)
					{
						str = string.Format("POV {0}", (nCode - 6 - 128) * 45);
					}
					else
					{
						str = string.Format("Code{0}", nCode);
					}
					break;
			}
			using (Bitmap bmpStr = b強調 ?
					prvFont.DrawPrivateFont(str, Color.White, Color.Black, Color.Yellow, Color.OrangeRed) :
					prvFont.DrawPrivateFont(str, Color.White, Color.Black))
			{
				CTexture txStr = TextureFactory.tテクスチャの生成(bmpStr, false);
				txStr.vc拡大縮小倍率 = new Vector3(0.75f, 0.75f, 1f);
				txStr.t2D描画(CDTXMania.Instance.Device, x * Scale.X, y * Scale.Y - 20);
				TextureFactory.tテクスチャの解放(ref txStr);
			}
		}

		private void tアサインコードの描画_Keyboard(int line, int x, int y, int nID, int nCode, bool b強調)
		{
			string str = null;
			foreach (STKEYLABEL stkeylabel in this.KeyLabel)
			{
				if (stkeylabel.nCode == nCode)
				{
					str = string.Format("{0,2}. Key {1}", line, stkeylabel.strLabel);
					break;
				}
			}
			if (str == null)
			{
				str = string.Format("{0,2}. Key 0x{1:X2}", line, nCode);
			}

			using (Bitmap bmpStr = b強調 ?
					prvFont.DrawPrivateFont(str, Color.White, Color.Black, Color.Yellow, Color.OrangeRed) :
					prvFont.DrawPrivateFont(str, Color.White, Color.Black))
			{
				CTexture txStr = TextureFactory.tテクスチャの生成(bmpStr, false);
				txStr.vc拡大縮小倍率 = new Vector3(0.75f, 0.75f, 1f);
				txStr.t2D描画(CDTXMania.Instance.Device, x * Scale.X, y * Scale.Y - 20);
				TextureFactory.tテクスチャの解放(ref txStr);
			}
		}

		private void tアサインコードの描画_MidiIn(int line, int x, int y, int nID, int nCode, bool b強調)
		{
			string str = string.Format("{0,2}. MidiIn #{1} code.{2}", line, nID, nCode);
			using (Bitmap bmpStr = b強調 ?
					prvFont.DrawPrivateFont(str, Color.White, Color.Black, Color.Yellow, Color.OrangeRed) :
					prvFont.DrawPrivateFont(str, Color.White, Color.Black))
			{
				CTexture txStr = TextureFactory.tテクスチャの生成(bmpStr, false);
				txStr.vc拡大縮小倍率 = new Vector3(0.75f, 0.75f, 1f);
				txStr.t2D描画(CDTXMania.Instance.Device, x * Scale.X, y * Scale.Y - 20);
				TextureFactory.tテクスチャの解放(ref txStr);
			}
		}

		private void tアサインコードの描画_Mouse(int line, int x, int y, int nID, int nCode, bool b強調)
		{
			string str = string.Format("{0,2}. Mouse Button{1}", line, nCode);
			using (Bitmap bmpStr = b強調 ?
					prvFont.DrawPrivateFont(str, Color.White, Color.Black, Color.Yellow, Color.OrangeRed) :
					prvFont.DrawPrivateFont(str, Color.White, Color.Black))
			{
				CTexture txStr = TextureFactory.tテクスチャの生成(bmpStr, false);
				txStr.vc拡大縮小倍率 = new Vector3(0.75f, 0.75f, 1f);
				txStr.t2D描画(CDTXMania.Instance.Device, x * Scale.X, y * Scale.Y - 20);
				TextureFactory.tテクスチャの解放(ref txStr);
			}
		}

		private bool tキーチェックとアサイン_Joypad()
		{
			foreach (IInputDevice device in CDTXMania.Instance.Input管理.list入力デバイス)
			{
				if (device.e入力デバイス種別 == E入力デバイス種別.Joystick)
				{
					for (int i = 0; i < 6 + 0x80 + 8; i++)      // +6 for Axis, +8 for HAT
					{
						if (device.bキーが押された(i))
						{
							CDTXMania.Instance.Skin.sound決定音.t再生する();
							CDTXMania.Instance.ConfigIni.t指定した入力が既にアサイン済みである場合はそれを全削除する(EInputDevice.JoyPad, device.ID, i);
							CDTXMania.Instance.ConfigIni.KeyAssign[pad][ptr].入力デバイス = EInputDevice.JoyPad;
							CDTXMania.Instance.ConfigIni.KeyAssign[pad][ptr].ID = device.ID;
							CDTXMania.Instance.ConfigIni.KeyAssign[pad][ptr].コード = i;
							return true;
						}
					}
				}
			}
			return false;
		}

		private bool tキーチェックとアサイン_Keyboard()
		{
			for (int i = 0; i < 256; i++)
			{
				if (i != (int)Key.Escape &&
					i != (int)Key.Up &&
					i != (int)Key.Down &&
					i != (int)Key.Left &&
					i != (int)Key.Right &&
					i != (int)Key.Delete &&
					 CDTXMania.Instance.Input管理.Keyboard.bキーが押された(i))
				{
					CDTXMania.Instance.Skin.sound決定音.t再生する();

					// #xxxxx: 2017.5.7 from: アサイン済みのキーと今回割り当てるキーが同じである場合は、削除されないようコードを未使用値(ここでは-1)にする。
					if( i == CDTXMania.Instance.ConfigIni.KeyAssign[ pad ][ ptr ].コード )
						CDTXMania.Instance.ConfigIni.KeyAssign[ pad ][ ptr ].コード = -1;

					CDTXMania.Instance.ConfigIni.t指定した入力が既にアサイン済みである場合はそれを全削除する(EInputDevice.Keyboard, 0, i);

					CDTXMania.Instance.ConfigIni.KeyAssign[pad][ptr].入力デバイス = EInputDevice.Keyboard;
					CDTXMania.Instance.ConfigIni.KeyAssign[pad][ptr].ID = 0;
					CDTXMania.Instance.ConfigIni.KeyAssign[pad][ptr].コード = i;
					return true;
				}
			}
			return false;
		}

		private bool tキーチェックとアサイン_MidiIn()
		{
			foreach (IInputDevice device in CDTXMania.Instance.Input管理.list入力デバイス)
			{
				if (device.e入力デバイス種別 == E入力デバイス種別.MidiIn)
				{
					for (int i = 0; i < 0x100; i++)
					{
						if (device.bキーが押された(i))
						{
							CDTXMania.Instance.Skin.sound決定音.t再生する();
							CDTXMania.Instance.ConfigIni.t指定した入力が既にアサイン済みである場合はそれを全削除する(EInputDevice.MIDIIn, device.ID, i);
							CDTXMania.Instance.ConfigIni.KeyAssign[pad][ptr].入力デバイス = EInputDevice.MIDIIn;
							CDTXMania.Instance.ConfigIni.KeyAssign[pad][ptr].ID = device.ID;
							CDTXMania.Instance.ConfigIni.KeyAssign[pad][ptr].コード = i;
							return true;
						}
					}
				}
			}
			return false;
		}

		private bool tキーチェックとアサイン_Mouse()
		{
			for (int i = 0; i < 8; i++)
			{
				if (CDTXMania.Instance.Input管理.Mouse.bキーが押された(i))
				{
					CDTXMania.Instance.ConfigIni.t指定した入力が既にアサイン済みである場合はそれを全削除する(EInputDevice.Mouse, 0, i);
					CDTXMania.Instance.ConfigIni.KeyAssign[pad][ptr].入力デバイス = EInputDevice.Mouse;
					CDTXMania.Instance.ConfigIni.KeyAssign[pad][ptr].ID = 0;
					CDTXMania.Instance.ConfigIni.KeyAssign[pad][ptr].コード = i;
				}
			}
			return false;
		}
	}
}
