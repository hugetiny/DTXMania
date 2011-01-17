using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using SlimDX;
using SlimDX.DirectInput;

namespace FDK
{
	public class CInputKeyboard : IInputDevice, IDisposable
	{
		// コンストラクタ

		public CInputKeyboard( IntPtr hWnd, DirectInput directInput )
		{
			this.e入力デバイス種別 = E入力デバイス種別.Keyboard;
			this.GUID = "";
			this.ID = 0;
			try
			{
				this.devKeyboard = new Keyboard( directInput );
				this.devKeyboard.SetCooperativeLevel( hWnd, CooperativeLevel.NoWinKey | CooperativeLevel.Foreground | CooperativeLevel.Nonexclusive );
				this.devKeyboard.Properties.BufferSize = 0x20;
				Trace.TraceInformation( this.devKeyboard.Information.ProductName + " を生成しました。" );
			}
			catch( DirectInputException )
			{
				if( this.devKeyboard != null )
				{
					this.devKeyboard.Dispose();
					this.devKeyboard = null;
				}
				Trace.TraceWarning( "Keyboard デバイスの生成に失敗しました。" );
				throw;
			}
			try
			{
				this.devKeyboard.Acquire();
			}
			catch( DirectInputException )
			{
			}

			for( int i = 0; i < this.bKeyState.Length; i++ )
				this.bKeyState[ i ] = false;

			this.timer = new CTimer( CTimer.E種別.MultiMedia );
		}


		// メソッド

		#region [ IInputDevice 実装 ]
		//-----------------
		public E入力デバイス種別 e入力デバイス種別 { get; private set; }
		public string GUID { get; private set; }
		public int ID { get; private set; }
		public List<STInputEvent> list入力イベント { get; private set; }

		public void tポーリング( bool bWindowがアクティブ中, bool bバッファ入力を使用する )
		{
			for( int i = 0; i < 256; i++ )
			{
				this.bKeyPushDown[ i ] = false;
				this.bKeyPullUp[ i ] = false;
			}

			if( ( ( bWindowがアクティブ中 && ( this.devKeyboard != null ) ) && !this.devKeyboard.Acquire().IsFailure ) && !this.devKeyboard.Poll().IsFailure )
			{
				this.list入力イベント = new List<STInputEvent>( 32 );
				int posEnter = -1;
				string d = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.ffff");

				if( bバッファ入力を使用する )
				{
					#region [ a.バッファ入力 ]
					//-----------------------------
					var bufferedData = this.devKeyboard.GetBufferedData();
					if( Result.Last.IsSuccess && bufferedData != null )
					{
						foreach( KeyboardState data in bufferedData )
						{
							foreach( Key key in data.PressedKeys )
							{
								STInputEvent item = new STInputEvent();
								item.nKey = (int) key;
								item.b押された = true;
								item.b離された = false;
								item.nTimeStamp = data.TimeStamp;
								item.nVelocity = CInput管理.n通常音量;
								this.list入力イベント.Add( item );

								this.bKeyState[ (int) key ] = true;
								this.bKeyPushDown[ (int) key ] = true;
#if TEST_CancelEnterCodeInAltEnter
								if ( (int)key == (int)SlimDX.DirectInput.Key.Return )
								{
									posEnter = this.list入力イベント.Count - 1;	//  #23708 2011.1.16 yyagi Enterのlist位置を記憶
								}
if ( (int)key == (int)SlimDX.DirectInput.Key.RightAlt) {
Debug.WriteLine( d + ": RAlt State/PushDown=true");
}
#endif
							}
							foreach( Key key in data.ReleasedKeys )
							{
								STInputEvent event3 = new STInputEvent();
								event3.nKey = (int) key;
								event3.b押された = false;
								event3.b離された = true;
								event3.nTimeStamp = data.TimeStamp;
								event3.nVelocity = CInput管理.n通常音量;
								this.list入力イベント.Add( event3 );

								this.bKeyState[ (int) key ] = false;
								this.bKeyPullUp[ (int) key ] = true;
#if TEST_CancelEnterCodeInAltEnter
if ( (int)key == (int)SlimDX.DirectInput.Key.RightAlt) {
Debug.WriteLine( d + ": RAlt State = false PullUp=true");
}
#endif
							}
						}
					}
					//-----------------------------
					#endregion
				}
				else
				{
					#region [ b.状態入力 ]
					//-----------------------------
					KeyboardState currentState = this.devKeyboard.GetCurrentState();
					if( Result.Last.IsSuccess && currentState != null )
					{
						foreach( Key key in currentState.PressedKeys )
						{
							if( this.bKeyState[ (int) key ] == false )
							{
								var ev = new STInputEvent() {
									nKey = (int) key,
									b押された = true,
									b離された = false,
									nTimeStamp = this.timer.nシステム時刻,
									nVelocity = CInput管理.n通常音量,
								};
								this.list入力イベント.Add( ev );

								this.bKeyState[ (int) key ] = true;
								this.bKeyPushDown[ (int) key ] = true;

#if TEST_CancelEnterCodeInAltEnter
								if ( (int)key == (int)SlimDX.DirectInput.Key.Return )
								{
									posEnter = this.list入力イベント.Count - 1;	//  #23708 2011.1.16 yyagi Enterのlist位置を記憶
								}
#endif
							}
						}
						foreach( Key key in currentState.ReleasedKeys )
						{
							if( this.bKeyState[ (int) key ] == true )
							{
								var ev = new STInputEvent() {
									nKey = (int) key,
									b押された = false,
									b離された = true,
									nTimeStamp = this.timer.nシステム時刻,
									nVelocity = CInput管理.n通常音量,
								};
								this.list入力イベント.Add( ev );

								this.bKeyState[ (int) key ] = false;
								this.bKeyPullUp[ (int) key ] = true;
							}
						}
					}
					//-----------------------------
					#endregion
				}
#if TEST_CancelEnterCodeInAltEnter
				#region [#23708 2011.1.16 yyagi Alt+Enter発生時、Enter押下情報を削除する]
				if ( this.bKeyPushDown[ (int) SlimDX.DirectInput.Key.Return ] == true )
				{
Debug.WriteLine( d + ": Enter PushDown=true" );
					if ( this.bKeyPullUp[ (int) SlimDX.DirectInput.Key.Return ] == true )
					{
Debug.WriteLine( d + ": Enter PullUp=true" );
						// #23708 2011.1.16 yyagi
						// フルスクリーンとウインドウを切り替える際、バッファ内でEnterのPushDownとPullUpが両方ともtrueとなることがある。
						// その際はどちらもfalseにして、Enter入力を無かったことにし、フルスクリーンとウインドウ切り替え時の
						// 余分なEnter動作を回避する。(対処療法的なやり方だが・・・)
						this.bKeyPushDown[ (int) SlimDX.DirectInput.Key.Return ] =
						this.bKeyPullUp[ (int) SlimDX.DirectInput.Key.Return ] = false;
					}
					if ( this.bKeyState[ (int) SlimDX.DirectInput.Key.LeftAlt ] == true || this.bKeyState[ (int) SlimDX.DirectInput.Key.RightAlt ] == true )
					{
Debug.WriteLine( d + ": Alt LALTstate=" + this.bKeyState[ (int)SlimDX.DirectInput.Key.LeftAlt] + ", RALTstate=" + this.bKeyState[ (int)SlimDX.DirectInput.Key.RightAlt] );
Debug.WriteLine( d + ": Alt LALTPushDown=" + this.bKeyPushDown[ (int) SlimDX.DirectInput.Key.LeftAlt ] + ", RALTPushDown=" + this.bKeyPushDown[ (int) SlimDX.DirectInput.Key.RightAlt ] );
Debug.WriteLine( d + ": Alt LALTPullUp=" + this.bKeyPullUp[ (int) SlimDX.DirectInput.Key.LeftAlt ] + ", RALTPullUp=" + this.bKeyPullUp[ (int) SlimDX.DirectInput.Key.RightAlt ] );

						// #23708 2011.1.16 yyagi
						// alt-enter押下時は、DTXMania内部としては Enter押下を無かったことにする。
						// (しかしFormの処理としてはEnter押下が残るので、フルスクリーンとウインドウの切り替えのみ実行され、
						//  選曲画面等での実行動作は無くなる。)
						if ( posEnter >= 0 )
						{
							this.bKeyState[ (int) SlimDX.DirectInput.Key.Return ] = false;
							this.bKeyPushDown[ (int) SlimDX.DirectInput.Key.Return ] = false;
							this.list入力イベント.RemoveAt( posEnter );
						}
					}
				}
				#endregion
#endif
			}
		}
		public bool bキーが押された( int nKey )
		{
			return this.bKeyPushDown[ nKey ];
		}
		public bool bキーが押されている( int nKey )
		{
			return this.bKeyState[ nKey ];
		}
		public bool bキーが離された( int nKey )
		{
			return this.bKeyPullUp[ nKey ];
		}
		public bool bキーが離されている( int nKey )
		{
			return !this.bKeyState[ nKey ];
		}
		//-----------------
		#endregion

		#region [ IDisposable 実装 ]
		//-----------------
		public void Dispose()
		{
			if( !this.bDispose完了済み )
			{
				if( this.devKeyboard != null )
				{
					this.devKeyboard.Dispose();
					this.devKeyboard = null;
				}
				if( this.timer != null )
				{
					this.timer.Dispose();
					this.timer = null;
				}
				this.bDispose完了済み = true;
			}
		}
		//-----------------
		#endregion


		// その他

		#region [ private ]
		//-----------------
		private bool bDispose完了済み;
		private bool[] bKeyPullUp = new bool[ 0x100 ];
		private bool[] bKeyPushDown = new bool[ 0x100 ];
		private bool[] bKeyState = new bool[ 0x100 ];
		private Keyboard devKeyboard;
		private CTimer timer;
		//-----------------
		#endregion
	}
}
