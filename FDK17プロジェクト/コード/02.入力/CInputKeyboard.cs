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

				if( bバッファ入力を使用する )
				{
					#region [ a.バッファ入力 ]
					//-----------------------------
					var bufferedData = this.devKeyboard.GetBufferedData();
					if( Result.Last.IsSuccess && bufferedData != null )
					{
						foreach( KeyboardState data in bufferedData )
						{
							foreach( Key key3 in data.PressedKeys )
							{
								STInputEvent event4 = new STInputEvent();
								STInputEvent item = event4;
								item.nKey = (int) key3;
								item.b押された = true;
								item.b離された = false;
								item.nTimeStamp = data.TimeStamp;
								item.nVelocity = CInput管理.n通常音量;
								this.list入力イベント.Add( item );

								this.bKeyState[ (int) key3 ] = true;
								this.bKeyPushDown[ (int) key3 ] = true;
							}
							foreach( Key key4 in data.ReleasedKeys )
							{
								STInputEvent event5 = new STInputEvent();
								STInputEvent event3 = event5;
								event3.nKey = (int) key4;
								event3.b押された = false;
								event3.b離された = true;
								event3.nTimeStamp = data.TimeStamp;
								event3.nVelocity = CInput管理.n通常音量;
								this.list入力イベント.Add( event3 );

								this.bKeyState[ (int) key4 ] = false;
								this.bKeyPullUp[ (int) key4 ] = true;
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
							}
						}
						foreach( Key key2 in currentState.ReleasedKeys )
						{
							if( this.bKeyState[ (int) key2 ] == true )
							{
								var ev = new STInputEvent() {
									nKey = (int) key2,
									b押された = false,
									b離された = true,
									nTimeStamp = this.timer.nシステム時刻,
									nVelocity = CInput管理.n通常音量,
								};
								this.list入力イベント.Add( ev );

								this.bKeyState[ (int) key2 ] = false;
								this.bKeyPullUp[ (int) key2 ] = true;
							}
						}
					}
					//-----------------------------
					#endregion
				}
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
