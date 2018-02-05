using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

namespace MidiInChecker2
{
	public partial class Form_Main : Form
	{
		CInputManager InputManager;
		System.Threading.Timer timer;
		object lockobj = new object();
		int looptimes = 11;

		private void Form_Main_Shown(object sender, EventArgs e)
		{
			#region [ タイトルバーの設定 ]
			System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
			int ver_asm_major = asm.GetName().Version.Major;
			this.Text = "MidiInChecker2 Rel" + ver_asm_major.ToString("D3");
			#endregion
		}

		public Form_Main()
		{
			InitializeComponent();
			InputManager = new CInputManager();

			RichLogTextBox.AppendText( "Number of MIDI devices: " + InputManager.nInputMidiDevices + "\r\n" );
			foreach ( string s in InputManager.listStrMidiDevices )
			{
				RichLogTextBox.AppendText( s + "\r\n" );
			}
			RichLogTextBox.AppendText( "\r\nHit any MIDI Pad to show the signal info.\r\n\r\n\n" );

			#region [タイマーで0.1秒ごとにログ画面を更新するように初期化する]
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

				// すべての入力デバイスについて…
				foreach ( IInputDevice device in InputManager.list入力デバイス )
				{
					if ( ( device.list入力イベント != null ) && ( device.list入力イベント.Count != 0 ) && ( device.e入力デバイス種別 == E入力デバイス種別.MidiIn ) )
					{
						foreach ( STInputEvent ev in device.list入力イベント )
						{
							Invoke( new AppendTextDelegate( appendLogText ), device.ID, ev);
							looptimes = 0;
						}
					}
				}
				if (looptimes++ == 10)	// 10回ループ(1秒間)の間に入力がなければ、空行を挿入する。
				{
					Invoke(new AppendTextDelegate( appendLogText ), -1, new STInputEvent() );
				}
			}
		}

		delegate void AppendTextDelegate( int id, STInputEvent ev );
		private void appendLogText( int id, STInputEvent ev )
		{
			string text;

			if (id == -1)
			{
				RichLogTextBox.SelectionColor = Color.Black;    // reset color
				text = "------------------------------------------------------------\r\n";
			}
			else
			{
				int nMIDIevent = ev.nKey & 0xFF;
				int nNote = (ev.nKey >> 8) & 0xFF;    // note#
													  // int nVelo = ( ev.nKey >> 16 ) & 0xFF;	// velocity

				DateTime dt = new DateTime(ev.nTimeStamp);

				text = dt.ToString("hh:mm:ss.fff") +
						": Device=" + id +
						", MidiEvent=0x" + nMIDIevent.ToString("X2") +
						", Note#=0x" + nNote.ToString("X2") +
						", Velocity=" + ev.nVelocity.ToString("D3") +
						"\r\n";

				Color[] cMidiEvent =
				{
					Color.Gray,			// 0x8x Note Off
					Color.Black,		// 0x9x Note On
					Color.Black,		// 0xAx
					Color.Green,		// 0xBx Control change
					Color.Black,		// 0xCx
					Color.Black,		// 0xDx
					Color.Blue,			// 0xEx Pitch bend
					Color.Black,        // 0xFx
				};
				Color c = cMidiEvent[(nMIDIevent >> 4) - 8];
				if ( (nMIDIevent & 0xF0) == 0x90 && (ev.nVelocity == 0) )
				{
					c = cMidiEvent[0];	// Note off color
				}
				RichLogTextBox.SelectionColor = c;
			}
			RichLogTextBox.Focus();
			RichLogTextBox.AppendText( text );
			RichLogTextBox.SelectionColor = Color.Black;	// reset color
		}

		// ダサい。後日改善予定。
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

		private void button1_Click( object sender, EventArgs e )
		{
			Application.Exit();
		}

		private void exitToolStripMenuItem1_Click( object sender, EventArgs e )
		{
			Application.Exit();
		}

		/// <summary>
		/// textboxで、Ctrl-Aでの全選択ができるようにする
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LogTextBox_KeyDown( object sender, KeyEventArgs e )
		{
			if ( e.KeyCode == System.Windows.Forms.Keys.A & e.Control == true )
			{
				RichLogTextBox.SelectAll();
			} 
		}
	}
}
