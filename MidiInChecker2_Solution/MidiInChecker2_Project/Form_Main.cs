using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.IO;

namespace MidiInChecker2
{
	public partial class Form_Main : Form
	{
		CInputManager InputManager;
		System.Threading.Timer timer;
		object lockobj = new object();
		int looptimes = 16;
		bool bYAMAHA_way_note = false;

		private void Form_Main_Shown(object sender, EventArgs e)
		{
			#region [ Title bar configuration (append version info) ]
			System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
			int ver_asm_major = asm.GetName().Version.Major;
			this.Text = "MidiInChecker2 Rel" + ver_asm_major.ToString("D3");
			#endregion
			RichLogTextBox.LanguageOption = RichTextBoxLanguageOptions.UIFonts;
		}

		/// <summary>
		/// Initializations
		/// </summary>
		public Form_Main()
		{
			InitializeComponent();
			InputManager = new CInputManager();

			RichLogTextBox.AppendText( "Number of MIDI devices: " + InputManager.nInputMidiDevices + System.Environment.NewLine );
			foreach ( string s in InputManager.listStrMidiDevices )
			{
				RichLogTextBox.AppendText( s + System.Environment.NewLine );
			}
			RichLogTextBox.AppendText(
				System.Environment.NewLine +
				"Hit any MIDI Pad to show the signal info." +
				System.Environment.NewLine +
				System.Environment.NewLine +
				System.Environment.NewLine
			);

			#region [ Set callback to update log area (richtextbox) every 0.1sec ]
			TimerCallback timerDelegate = new TimerCallback(mainloop);
			timer = new System.Threading.Timer(timerDelegate, null , 0, 100);
			#endregion
		}


		private void mainloop( object o )
		{
			lock ( lockobj )
			{
				InputManager.tポーリング( true, true );
				List<STInputEvent> list = new List<STInputEvent>();

				// For all MIDI input devices...
				foreach ( IInputDevice device in InputManager.list入力デバイス )
				{
					if ( ( device.list入力イベント != null ) && ( device.list入力イベント.Count != 0 ) && ( device.e入力デバイス種別 == E入力デバイス種別.MidiIn ) )
					{
						// if MIDI-input exists, invoke all of them, then push them to the RichTextBox
						foreach ( STInputEvent ev in device.list入力イベント )
						{
							Invoke( new AppendTextDelegate( appendLogText ), device.ID, ev);
							looptimes = 0;
						}
					}
				}
				if (looptimes++ == 15)	// Insert horizontal rule if no inputs in 15times-loop (1.5sec)
				{
					Invoke(new AppendTextDelegate( appendLogText ), -1, new STInputEvent() );
				}
			}
		}

		delegate void AppendTextDelegate( int id, STInputEvent ev );
		private void appendLogText( int id, STInputEvent ev )
		{
			string text;

			if (id == -1)	// Insert Horizontal Rule
			{
				RichLogTextBox.SelectionColor = Color.Black;    // reset color
				text = "------------------------------------------------------------\r\n";
			}
			else
			{
				int nMIDIevent = ev.nKey & 0xFF;
				int nNote = (ev.nKey >> 8) & 0xFF;
				string[] strNote =
				{
					"C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B"
				};

				#region [ At first, put time, and MIDI event info ]
				text = string.Format(
					"{0}: Device={1}, MidiEvent=0x{2}",
					new DateTime(ev.nTimeStamp).ToString("HH:mm:ss.fff"),
					id,
					nMIDIevent.ToString("X2")
				);
				#endregion
				#region [ Then, Append MIDI event detail descriptions ]
				switch (nMIDIevent & 0xF0)
				{
					case 0x80:	// Note off
					case 0x90:	// Note off/on
						text += string.Format(
							", Note#=0x{0} (Key={1}{2}) {3}, Velocity={4}",
							nNote.ToString("X2"),
							strNote[nNote % 12],
							(nNote / 12) - 1 - (bYAMAHA_way_note ? 1 : 0),
							((nMIDIevent & 0xF0) == 0x80 || ev.nVelocity == 0x00) ? "Off" : "On ",
							ev.nVelocity.ToString("D3")
						);
						break;

					case 0xA0:  // Polyphonic Key Pressure
						text += string.Format(
							", Note#=0x{0} (Key={1}{2}), PolyphonicKeyPresssure={3}",
							nNote.ToString("X2"),
							strNote[nNote % 12],
							(nNote / 12) - 1 - (bYAMAHA_way_note ? 1 : 0),
							ev.nVelocity.ToString("D3")
						);
						break;

					case 0xB0:  // Control Change
						text += strCtrlChangeText(nNote, ev.nVelocity);
						break;

					case 0xC0:  // Program Change
						text += string.Format(
							", ChangeProgram to {0}",
							nNote.ToString("X2")
						);
						break;

					case 0xD0:  // Channel Pressure
						text += string.Format(
							", ChannelPresssure={0}",
							nNote
						);
						break;

					case 0xE0:  // Pitch Wheel Change
						text += string.Format(
							", PitchWheelChange to {0}",
							((nNote + (ev.nVelocity * 128)) - 8192)
						);
						break;

					case 0xF0:  // System Common Message
						text += string.Format(
							", SystemCommonMessage {0}",
							nNote.ToString("X2")
						);
						break;
				}
				#endregion
				#region [ Configure text color ]
				Color[] cMidiEvent =
				{
					Color.Gray,			// 0x8x: Note Off
					Color.Black,		// 0x9x: Note On
					Color.Black,		// 0xAx: Poliphonic Key Pressure
					Color.Green,		// 0xBx: Control change / Channel Mode Message
					Color.Black,		// 0xCx: Program Change
					Color.Black,		// 0xDx: Channel Pressure
					Color.Blue,			// 0xEx: Pitch Wheel Change
					Color.Purple,       // 0xFx: System Common Message
				};
				Color c = cMidiEvent[(nMIDIevent >> 4) - 8];
				if ( (nMIDIevent & 0xF0) == 0x90 && (ev.nVelocity == 0) )
				{
					c = cMidiEvent[0];	// Note off color
				}
				RichLogTextBox.SelectionColor = c;
				#endregion
			}
			RichLogTextBox.Focus();
			RichLogTextBox.AppendText( text + System.Environment.NewLine );
			RichLogTextBox.SelectionColor = Color.Black;	// reset color
		}

		// Need improvement...
		private void Form_Main_FormClosing( object sender, FormClosingEventArgs e )
		{
			// mainloop処理中なら、待つ
			lock ( lockobj )
			{ };

			// mainloop処理中でないことを確認して、(mainloopを呼び出している)timerを止める
			timer.Dispose();

			// timerを止めてmainloop処理が発生しないことを担保してから、InputMangerを開放する
			InputManager.Dispose();
			InputManager = null;
		}

		private void button_OK_Click( object sender, EventArgs e )
		{
			Application.Exit();
		}

		private void exitToolStripMenuItem1_Click( object sender, EventArgs e )
		{
			Application.Exit();
		}

		/// <summary>
		/// [Ctrl-A] => SelectAll
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void RichLogTextBox_KeyDown( object sender, KeyEventArgs e )
		{
			if ( e.KeyCode == System.Windows.Forms.Keys.A & e.Control == true )
			{
				RichLogTextBox.SelectAll();
			} 
		}

		/// <summary>
		/// Output logs about control change
		/// </summary>
		/// <param name="ctrl"></param>
		/// <param name="data"></param>
		/// <returns></returns>
		private string strCtrlChangeText(int ctrl, int data)
		{
			#region [ Control Change Number => strings ]
			string[] strCtrlChgName =
			{
			"BankSelect<MSB>",					// 0x00
			"Modulation",
			"BreathController",
			"",
			"FootController",
			"PortamentTime",
			"DataEntry",
			"ChannelVolume",
			"Balance",							// 0x08
			"",
			"Pan",
			"Expression",
			"EffectControl1",
			"EffectControl2",
			"",
			"",
			"GeneralPurposeControler1",			// 0x10
			"GeneralPurposeControler2",			// 0x11
			"GeneralPurposeControler3",			// 0x12
			"GeneralPurposeControler4",			// 0x13
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"BankSelect<LSB>",					// 0x20
			"Modulation<LSB>",
			"BreathContoller<LSB>",
			"",
			"FootController<LSB>",
			"PortamentTime<LSB>",
			"DataEntry<LSB>",
			"ChannelVOlume<LSB>",
			"Balance<LSB>",
			"",
			"Expression<LSB>",
			"Pan<LSB>",
			"Expression<LSB>",
			"EffectControl1<LSB>",
			"EffectControl1<LSB>",
			"",
			"",
			"GeneralPurposeContoller1<LSB>",	// 0x30
			"GeneralPurposeContoller2<LSB>",	// 0x31
			"GeneralPurposeContoller3<LSB>",	// 0x32
			"GeneralPurposeContoller4<LSB>",	// 0x34
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"Hold1",							// 0x40
			"Portament",
			"Sostenuto",
			"SoftPedal",
			"LegatoFootswitch",
			"Hold2",
			"SoundVariation",
			"Timbre/HarmonicIntensity",
			"ReleaseTime",
			"AttackTime",
			"Brightness",
			"DecayTime",
			"VibratoTime",
			"VibratoDepth",
			"VibratoDelay",
			"",
			"GeneralPurposeController5",		// 0x50
			"GeneralPurposeController6",		// 0x51
			"GeneralPurposeController7",		// 0x52
			"GeneralPurposeController8",		// 0x53
			"PortamentContol",
			"",
			"",
			"",
			"",
			"",
			"",
			"Effect1Depth(ReverbSendLevel)",
			"Effect2Depth(TremoloDepth)",
			"Effect3Depth(ChorusSendLevel)",
			"Effect4Depth(CelesteDepth)",
			"Effect5Depth(PhaserDepth)",
			"DataIncrement",					// 0x60
			"DataDecrement",
			"NRPN(LSB)",
			"NRPN(MSB)",
			"RPN(LSB)",
			"RPN(MSB",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",									// 0x70
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"AllSoundOff",						// 0x78
			"ResetAllControllers",
			"LocalControl",
			"AllNoteOff",
			"OmniOff",
			"OmniOn",
			"Mono",
			"Poly",									// 0x7F
		};
			#endregion

			string ret = string.Format(
				", CtrlChgNo=0x{0} ({1}), data={2}",
				ctrl.ToString("D2"),
				strCtrlChgName[ctrl],
				data
			);
			
			return ret;
		}

		/// <summary>
		/// Save Logs to the file (txt / rtf)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (saveFileDialog1.ShowDialog() == DialogResult.OK)
			{
				string filename = saveFileDialog1.FileName;
				bool bRTF = (Path.GetExtension(filename).ToLower() == ".rtf");

				RichLogTextBox.SaveFile(
					filename,
					bRTF ? RichTextBoxStreamType.RichText : RichTextBoxStreamType.PlainText
				);
			}
		}

		#region [ Copy/SelectAll support ]
		private void copyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			RichLogTextBox.Copy();
		}

		private void pasteVToolStripMenuItem_Click(object sender, EventArgs e)
		{
			RichLogTextBox.SelectAll();
		}

		private void copyToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			RichLogTextBox.Copy();
		}

		private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			RichLogTextBox.SelectAll();
		}
		#endregion
	}
}
