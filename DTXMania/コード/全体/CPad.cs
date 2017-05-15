using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using FDK;

namespace DTXMania
{
	public class CPad
	{
		// プロパティ

		internal STHIT st検知したデバイス;
		[StructLayout(LayoutKind.Sequential)]
		internal struct STHIT
		{
			public bool Keyboard;
			public bool MIDIIN;
			public bool Joypad;
			public bool Mouse;
			public void Clear()
			{
				this.Keyboard = false;
				this.MIDIIN = false;
				this.Joypad = false;
				this.Mouse = false;
			}
		}


		// コンストラクタ

		internal CPad()
		{
			this.st検知したデバイス.Clear();
		}


		// メソッド

		public List<STInputEvent> GetEvents(EPad pad)
		{
			COptionKeyAssign[] stkeyassignArray = CDTXMania.Instance.ConfigIni.KeyAssign[pad];
			List<STInputEvent> list = new List<STInputEvent>();

			// すべての入力デバイスについて…
			foreach (IInputDevice device in CDTXMania.Instance.Input管理.list入力デバイス)
			{
				if ((device.list入力イベント != null) && (device.list入力イベント.Count != 0))
				{
					foreach (STInputEvent event2 in device.list入力イベント)
					{
						for (int i = 0; i < CConfigXml.AssignableCodes; i++)
						{
							switch (stkeyassignArray[i].入力デバイス)
							{
								case EInputDevice.Keyboard:
									if ((device.e入力デバイス種別 == E入力デバイス種別.Keyboard) && (event2.nKey == stkeyassignArray[i].コード))
									{
										list.Add(event2);
										this.st検知したデバイス.Keyboard = true;
									}
									break;

								case EInputDevice.MIDIIn:
									if (((device.e入力デバイス種別 == E入力デバイス種別.MidiIn) && (device.ID == stkeyassignArray[i].ID)) && (event2.nKey == stkeyassignArray[i].コード))
									{
										list.Add(event2);
										this.st検知したデバイス.MIDIIN = true;
									}
									break;

								case EInputDevice.JoyPad:
									if (((device.e入力デバイス種別 == E入力デバイス種別.Joystick) && (device.ID == stkeyassignArray[i].ID)) && (event2.nKey == stkeyassignArray[i].コード))
									{
										list.Add(event2);
										this.st検知したデバイス.Joypad = true;
									}
									break;

								case EInputDevice.Mouse:
									if ((device.e入力デバイス種別 == E入力デバイス種別.Mouse) && (event2.nKey == stkeyassignArray[i].コード))
									{
										list.Add(event2);
										this.st検知したデバイス.Mouse = true;
									}
									break;
							}
						}
					}
					continue;
				}
			}
			return list;
		}
		public bool b押された(EPad pad)
		{
			COptionKeyAssign[] stkeyassignArray = CDTXMania.Instance.ConfigIni.KeyAssign[pad];
			for (int i = 0; i < CConfigXml.AssignableCodes; i++)
			{
				switch (stkeyassignArray[i].入力デバイス)
				{
					case EInputDevice.Keyboard:
						if (!CDTXMania.Instance.Input管理.Keyboard.bキーが押された(stkeyassignArray[i].コード))
							break;

						this.st検知したデバイス.Keyboard = true;
						return true;

					case EInputDevice.MIDIIn:
						{
							IInputDevice device2 = CDTXMania.Instance.Input管理.MidiIn(stkeyassignArray[i].ID);
							if ((device2 == null) || !device2.bキーが押された(stkeyassignArray[i].コード))
								break;

							this.st検知したデバイス.MIDIIN = true;
							return true;
						}
					case EInputDevice.JoyPad:
						{
							if (!CDTXMania.Instance.ConfigIni.dicJoystick.Value.ContainsKey(stkeyassignArray[i].ID))
								break;

							IInputDevice device = CDTXMania.Instance.Input管理.Joystick(stkeyassignArray[i].ID);
							if ((device == null) || !device.bキーが押された(stkeyassignArray[i].コード))
								break;

							this.st検知したデバイス.Joypad = true;
							return true;
						}
					case EInputDevice.Mouse:
						if (!CDTXMania.Instance.Input管理.Mouse.bキーが押された(stkeyassignArray[i].コード))
							break;

						this.st検知したデバイス.Mouse = true;
						return true;
				}
			}
			return false;
		}

		public bool bDecidePadIsPressedDGB()
		{
			return
				b押された(EPad.GtDecide) || b押された(EPad.BsDecide) || b押された(EPad.CY) ||
				b押された(EPad.LC) || b押された(EPad.RD);
		}

		public bool bDecidePadIsPressedGB()
		{
			return
				b押された(EPad.GtDecide) || b押された(EPad.BsDecide);
		}

		public bool bCancelPadIsPressedDGB()
		{
			return b押された(EPad.FT) || b押された(EPad.GtCancel) || b押された(EPad.BsCancel);
		}

		public bool bCancelPadIsPressedGB()
		{
			return b押された(EPad.GtCancel) || b押された(EPad.BsCancel);
		}

		public bool b押されている(EPad pad)
		{
			COptionKeyAssign[] stkeyassignArray = CDTXMania.Instance.ConfigIni.KeyAssign[pad];
			for (int i = 0; i < CConfigXml.AssignableCodes; i++)
			{
				switch (stkeyassignArray[i].入力デバイス)
				{
					case EInputDevice.Keyboard:
						if (!CDTXMania.Instance.Input管理.Keyboard.bキーが押されている(stkeyassignArray[i].コード))
						{
							break;
						}
						this.st検知したデバイス.Keyboard = true;
						return true;

					case EInputDevice.JoyPad:
						{
							if (!CDTXMania.Instance.ConfigIni.dicJoystick.Value.ContainsKey(stkeyassignArray[i].ID))
							{
								break;
							}
							IInputDevice device = CDTXMania.Instance.Input管理.Joystick(stkeyassignArray[i].ID);
							if ((device == null) || !device.bキーが押されている(stkeyassignArray[i].コード))
							{
								break;
							}
							this.st検知したデバイス.Joypad = true;
							return true;
						}
					case EInputDevice.Mouse:
						if (!CDTXMania.Instance.Input管理.Mouse.bキーが押されている(stkeyassignArray[i].コード))
						{
							break;
						}
						this.st検知したデバイス.Mouse = true;
						return true;
				}

			}
			return false;
		}
	}
}
