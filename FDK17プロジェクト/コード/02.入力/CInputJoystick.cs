using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using SlimDX;
using SlimDX.DirectInput;

namespace FDK
{
	public class CInputJoystick : IInputDevice, IDisposable
	{
		// コンストラクタ

		public CInputJoystick( IntPtr hWnd, DeviceInstance di, DirectInput directInput )
		{
			this.e入力デバイス種別 = E入力デバイス種別.Joystick;
			this.GUID = di.InstanceGuid.ToString();
			this.ID = 0;
			try
			{
				this.devJoystick = new Joystick( directInput, di.InstanceGuid );
				this.devJoystick.SetCooperativeLevel( hWnd, CooperativeLevel.Foreground | CooperativeLevel.Exclusive );
				this.devJoystick.Properties.BufferSize = 32;
				Trace.TraceInformation( this.devJoystick.Information.InstanceName + "を生成しました。" );
			}
			catch( DirectInputException )
			{
				if( this.devJoystick != null )
				{
					this.devJoystick.Dispose();
					this.devJoystick = null;
				}
				Trace.TraceError( this.devJoystick.Information.InstanceName, new object[] { " の生成に失敗しました。" } );
				throw;
			}
			foreach( DeviceObjectInstance instance in this.devJoystick.GetObjects() )
			{
				if( ( instance.ObjectType & ObjectDeviceType.Axis ) != ObjectDeviceType.All )
				{
					this.devJoystick.GetObjectPropertiesById( (int) instance.ObjectType ).SetRange( -1000, 0x3e8 );
				}
			}
			try
			{
				this.devJoystick.Acquire();
			}
			catch( DirectInputException )
			{
			}

			for( int i = 0; i < this.bButtonState.Length; i++ )
				this.bButtonState[ i ] = false;

			this.timer = new CTimer( CTimer.E種別.MultiMedia );
		}
		
		
		// メソッド
		
		public void SetID( int nID )
		{
			this.ID = nID;
		}

		#region [ IInputDevice 実装 ]
		//-----------------
		public E入力デバイス種別 e入力デバイス種別
		{ 
			get;
			private set;
		}
		public string GUID
		{
			get;
			private set;
		}
		public int ID
		{ 
			get; 
			private set;
		}
		public List<STInputEvent> list入力イベント 
		{
			get;
			private set; 
		}

		public void tポーリング( bool bWindowがアクティブ中, bool bバッファ入力を使用する )
		{
			for( int i = 0; i < 256; i++ )
			{
				this.bButtonPushDown[ i ] = false;
				this.bButtonPullUp[ i ] = false;
			}

			if( ( bWindowがアクティブ中 && !this.devJoystick.Acquire().IsFailure ) && !this.devJoystick.Poll().IsFailure )
			{
				STInputEvent event29;
				this.list入力イベント = new List<STInputEvent>( 32 );

				if( bバッファ入力を使用する )
				{
					#region [ a.バッファ入力 ]
					//-----------------------------
					var bufferedData = this.devJoystick.GetBufferedData();

					if( Result.Last.IsSuccess && bufferedData != null )
					{
						foreach( JoystickState data in bufferedData )
						{
							#region [ X軸－ ]
							//-----------------------------
							if( data.X < -500 )
							{
								STInputEvent event43 = new STInputEvent();
								STInputEvent event16 = event43;
								event16.nKey = 0;
								event16.b押された = true;
								event16.nTimeStamp = data.TimeStamp;
								event16.nVelocity = CInput管理.n通常音量;
								this.list入力イベント.Add( event16 );

								this.bButtonState[ 0 ] = true;
								this.bButtonPushDown[ 0 ] = true;
							}
							else
							{
								STInputEvent event44 = new STInputEvent();
								STInputEvent event17 = event44;
								event17.nKey = 0;
								event17.b押された = false;
								event17.nTimeStamp = data.TimeStamp;
								event17.nVelocity = 0;
								this.list入力イベント.Add( event17 );

								this.bButtonState[ 0 ] = false;
								this.bButtonPullUp[ 0 ] = true;
							}
							//-----------------------------
							#endregion
							#region [ X軸＋ ]
							//-----------------------------
							if( data.X > 500 )
							{
								STInputEvent event45 = new STInputEvent();
								STInputEvent event18 = event45;
								event18.nKey = 1;
								event18.b押された = true;
								event18.nTimeStamp = data.TimeStamp;
								event18.nVelocity = CInput管理.n通常音量;
								this.list入力イベント.Add( event18 );

								this.bButtonState[ 1 ] = true;
								this.bButtonPushDown[ 1 ] = true;
							}
							else
							{
								STInputEvent event46 = new STInputEvent();
								STInputEvent event19 = event46;
								event19.nKey = 1;
								event19.b押された = false;
								event19.nTimeStamp = data.TimeStamp;
								event19.nVelocity = 0;
								this.list入力イベント.Add( event19 );

								this.bButtonState[ 1 ] = false;
								this.bButtonPullUp[ 1 ] = true;
							}
							//-----------------------------
							#endregion
							#region [ Y軸－ ]
							//-----------------------------
							if( data.Y < -500 )
							{
								STInputEvent event47 = new STInputEvent();
								STInputEvent event20 = event47;
								event20.nKey = 2;
								event20.b押された = true;
								event20.nTimeStamp = data.TimeStamp;
								event20.nVelocity = CInput管理.n通常音量;
								this.list入力イベント.Add( event20 );

								this.bButtonState[ 2 ] = true;
								this.bButtonPushDown[ 2 ] = true;
							}
							else
							{
								STInputEvent event48 = new STInputEvent();
								STInputEvent event21 = event48;
								event21.nKey = 2;
								event21.b押された = false;
								event21.nTimeStamp = data.TimeStamp;
								event21.nVelocity = 0;
								this.list入力イベント.Add( event21 );

								this.bButtonState[ 2 ] = false;
								this.bButtonPullUp[ 2 ] = true;
							}
							//-----------------------------
							#endregion
							#region [ Y軸＋ ]
							//-----------------------------
							if( data.Y > 500 )
							{
								event29 = new STInputEvent();
								STInputEvent event22 = event29;
								event22.nKey = 3;
								event22.b押された = true;
								event22.nTimeStamp = data.TimeStamp;
								event22.nVelocity = CInput管理.n通常音量;
								this.list入力イベント.Add( event22 );

								this.bButtonState[ 3 ] = true;
								this.bButtonPushDown[ 3 ] = true;
							}
							else
							{
								event29 = new STInputEvent();
								STInputEvent event23 = event29;
								event23.nKey = 3;
								event23.b押された = false;
								event23.nTimeStamp = data.TimeStamp;
								event23.nVelocity = 0;
								this.list入力イベント.Add( event23 );

								this.bButtonState[ 3 ] = false;
								this.bButtonPullUp[ 3 ] = true;
							}
							//-----------------------------
							#endregion
							#region [ Z軸－ ]
							//-----------------------------
							if( data.Z < -500 )
							{
								event29 = new STInputEvent();
								STInputEvent event24 = event29;
								event24.nKey = 4;
								event24.b押された = true;
								event24.nTimeStamp = data.TimeStamp;
								event24.nVelocity = CInput管理.n通常音量;
								this.list入力イベント.Add( event24 );

								this.bButtonState[ 4 ] = true;
								this.bButtonPushDown[ 4 ] = true;
							}
							else
							{
								event29 = new STInputEvent();
								STInputEvent event25 = event29;
								event25.nKey = 4;
								event25.b押された = false;
								event25.nTimeStamp = data.TimeStamp;
								event25.nVelocity = 0;
								this.list入力イベント.Add( event25 );

								this.bButtonState[ 4 ] = false;
								this.bButtonPullUp[ 4 ] = true;
							}
							//-----------------------------
							#endregion
							#region [ Z軸＋ ]
							//-----------------------------
							if( data.Z > 500 )
							{
								event29 = new STInputEvent();
								STInputEvent event26 = event29;
								event26.nKey = 5;
								event26.b押された = true;
								event26.nTimeStamp = data.TimeStamp;
								event26.nVelocity = CInput管理.n通常音量;
								this.list入力イベント.Add( event26 );

								this.bButtonState[ 5 ] = true;
								this.bButtonPushDown[ 5 ] = true;
							}
							else
							{
								event29 = new STInputEvent();
								STInputEvent event27 = event29;
								event27.nKey = 5;
								event27.b押された = false;
								event27.nTimeStamp = data.TimeStamp;
								event27.nVelocity = 0;
								this.list入力イベント.Add( event27 );

								this.bButtonState[ 5 ] = false;
								this.bButtonPullUp[ 5 ] = true;
							}
							//-----------------------------
							#endregion
							#region [ ボタン ]
							//-----------------------------
							for( int i = 0; i < 32; i++ )
							{
								if( data.IsPressed( i ) )
								{
									event29 = new STInputEvent();
									STInputEvent event28 = event29;
									event28.nKey = 6 + i;
									event28.b押された = true;
									event28.b離された = false;
									event28.nTimeStamp = data.TimeStamp;
									event28.nVelocity = CInput管理.n通常音量;
									this.list入力イベント.Add( event28 );

									this.bButtonState[ 6 + i ] = true;
									this.bButtonPushDown[ 6 + i ] = true;
								}
								else if( data.IsReleased( i ) )
								{
									var ev = new STInputEvent() {
										nKey = 6 + i,
										b押された = false,
										b離された = true,
										nTimeStamp = data.TimeStamp,
										nVelocity = CInput管理.n通常音量,
									};
									this.list入力イベント.Add( ev );

									this.bButtonState[ 6 + i ] = false;
									this.bButtonPullUp[ 6 + i ] = true;
								}
							}
							//-----------------------------
							#endregion
						}
					}
					//-----------------------------
					#endregion
				}
				else
				{
					#region [ b.状態入力 ]
					//-----------------------------
					JoystickState currentState = this.devJoystick.GetCurrentState();
					if( Result.Last.IsSuccess && currentState != null )
					{
						#region [ X軸－ ]
						//-----------------------------
						if( currentState.X < -500 )
						{
							if( this.bButtonState[ 0 ] == false )
							{
								STInputEvent event4 = new STInputEvent();
								event4.nKey = 0;
								event4.b押された = true;
								event4.nTimeStamp = this.timer.nシステム時刻;
								event4.nVelocity = CInput管理.n通常音量;
								this.list入力イベント.Add( event4 );

								this.bButtonState[ 0 ] = true;
								this.bButtonPushDown[ 0 ] = true;
							}
						}
						else
						{
							if( this.bButtonState[ 0 ] == true )
							{
								STInputEvent event5 = new STInputEvent();
								event5.nKey = 0;
								event5.b押された = false;
								event5.nTimeStamp = this.timer.nシステム時刻;
								event5.nVelocity = CInput管理.n通常音量;
								this.list入力イベント.Add( event5 );

								this.bButtonState[ 0 ] = false;
								this.bButtonPullUp[ 0 ] = true;
							}
						}
						//-----------------------------
						#endregion
						#region [ X軸＋ ]
						//-----------------------------
						if( currentState.X > 500 )
						{
							if( this.bButtonState[ 1 ] == false )
							{
								STInputEvent event6 = new STInputEvent();
								event6.nKey = 1;
								event6.b押された = true;
								event6.nTimeStamp = this.timer.nシステム時刻;
								event6.nVelocity = CInput管理.n通常音量;
								this.list入力イベント.Add( event6 );

								this.bButtonState[ 1 ] = true;
								this.bButtonPushDown[ 1 ] = true;
							}
						}
						else
						{
							if( this.bButtonState[ 1 ] == true )
							{
								STInputEvent event7 = new STInputEvent();
								event7.nKey = 1;
								event7.b押された = false;
								event7.nTimeStamp = this.timer.nシステム時刻;
								event7.nVelocity = CInput管理.n通常音量;
								this.list入力イベント.Add( event7 );

								this.bButtonState[ 1 ] = false;
								this.bButtonPullUp[ 1 ] = true;
							}
						}
						//-----------------------------
						#endregion
						#region [ Y軸－ ]
						//-----------------------------
						if( currentState.Y < -500 )
						{
							if( this.bButtonState[ 2 ] == false )
							{
								STInputEvent event8 = new STInputEvent();
								event8.nKey = 2;
								event8.b押された = true;
								event8.nTimeStamp = this.timer.nシステム時刻;
								event8.nVelocity = CInput管理.n通常音量;
								this.list入力イベント.Add( event8 );

								this.bButtonState[ 2 ] = true;
								this.bButtonPushDown[ 2 ] = true;
							}
						}
						else
						{
							if( this.bButtonState[ 2 ] == true )
							{
								STInputEvent event9 = new STInputEvent();
								event9.nKey = 2;
								event9.b押された = false;
								event9.nTimeStamp = this.timer.nシステム時刻;
								event9.nVelocity = CInput管理.n通常音量;
								this.list入力イベント.Add( event9 );

								this.bButtonState[ 2 ] = false;
								this.bButtonPullUp[ 2 ] = true;
							}
						}
						//-----------------------------
						#endregion
						#region [ Y軸＋ ]
						//-----------------------------
						if( currentState.Y > 500 )
						{
							if( this.bButtonState[ 3 ] == false )
							{
								STInputEvent event10 = new STInputEvent();
								event10.nKey = 3;
								event10.b押された = true;
								event10.nTimeStamp = this.timer.nシステム時刻;
								event10.nVelocity = CInput管理.n通常音量;
								this.list入力イベント.Add( event10 );

								this.bButtonState[ 3 ] = true;
								this.bButtonPushDown[ 3 ] = true;
							}
						}
						else
						{
							if( this.bButtonState[ 3 ] == true )
							{
								STInputEvent event11 = new STInputEvent();
								event11.nKey = 3;
								event11.b押された = false;
								event11.nTimeStamp = this.timer.nシステム時刻;
								event11.nVelocity = CInput管理.n通常音量;
								this.list入力イベント.Add( event11 );

								this.bButtonState[ 3 ] = false;
								this.bButtonPullUp[ 3 ] = true;
							}
						}
						//-----------------------------
						#endregion
						#region [ Z軸－ ]
						//-----------------------------
						if( currentState.Z < -500 )
						{
							if( this.bButtonState[ 4 ] == false )
							{
								STInputEvent event12 = new STInputEvent();
								event12.nKey = 4;
								event12.b押された = true;
								event12.nTimeStamp = this.timer.nシステム時刻;
								event12.nVelocity = CInput管理.n通常音量;
								this.list入力イベント.Add( event12 );

								this.bButtonState[ 4 ] = true;
								this.bButtonPushDown[ 4 ] = true;
							}
						}
						else
						{
							if( this.bButtonState[ 4 ] == true )
							{
								STInputEvent event13 = new STInputEvent();
								event13.nKey = 4;
								event13.b押された = false;
								event13.nTimeStamp = this.timer.nシステム時刻;
								event13.nVelocity = CInput管理.n通常音量;
								this.list入力イベント.Add( event13 );

								this.bButtonState[ 4 ] = false;
								this.bButtonPullUp[ 4 ] = true;
							}
						}
						//-----------------------------
						#endregion
						#region [ Z軸＋ ]
						//-----------------------------
						if( currentState.Z > 500 )
						{
							if( this.bButtonState[ 5 ] == false )
							{
								STInputEvent event14 = new STInputEvent();
								event14.nKey = 5;
								event14.b押された = true;
								event14.nTimeStamp = this.timer.nシステム時刻;
								event14.nVelocity = CInput管理.n通常音量;
								this.list入力イベント.Add( event14 );

								this.bButtonState[ 5 ] = true;
								this.bButtonPushDown[ 5 ] = true;
							}
						}
						else
						{
							if( this.bButtonState[ 5 ] == true )
							{
								STInputEvent event15 = new STInputEvent();
								event15.nKey = 5;
								event15.b押された = false;
								event15.nTimeStamp = this.timer.nシステム時刻;
								event15.nVelocity = CInput管理.n通常音量;
								this.list入力イベント.Add( event15 );

								this.bButtonState[ 5 ] = false;
								this.bButtonPullUp[ 5 ] = true;
							}
						}
						//-----------------------------
						#endregion
						#region [ ボタン ]
						//-----------------------------
						bool[] buttons = currentState.GetButtons();
						for( int j = 0; ( j < buttons.Length ) && ( j < 128 ); j++ )
						{
							if( this.bButtonState[ 6 + j ] == false && buttons[ j ] )
							{
								event29 = new STInputEvent();
								STInputEvent item = event29;
								item.nKey = 6 + j;
								item.b押された = true;
								item.nTimeStamp = this.timer.nシステム時刻;
								item.nVelocity = CInput管理.n通常音量;
								this.list入力イベント.Add( item );

								this.bButtonState[ 6 + j ] = true;
								this.bButtonPushDown[ 6 + j ] = true;
							}
							else if( this.bButtonState[ 6 + j ] == true && !buttons[ j ] )
							{
								STInputEvent event30 = new STInputEvent();
								STInputEvent event3 = event30;
								event3.nKey = 6 + j;
								event3.b押された = false;
								event3.nTimeStamp = this.timer.nシステム時刻;
								event3.nVelocity = CInput管理.n通常音量;
								this.list入力イベント.Add( event3 );

								this.bButtonState[ 6 + j ] = false;
								this.bButtonPullUp[ 6 + j ] = true;
							}
						}
						//-----------------------------
						#endregion
					}
					//-----------------------------
					#endregion
				}
			}
		}
		public bool bキーが押された( int nButton )
		{
			return this.bButtonPushDown[ nButton ];
		}
		public bool bキーが押されている( int nButton )
		{
			return this.bButtonState[ nButton ];
		}
		public bool bキーが離された( int nButton )
		{
			return this.bButtonPullUp[ nButton ];
		}
		public bool bキーが離されている( int nButton )
		{
			return !this.bButtonState[ nButton ];
		}
		//-----------------
		#endregion

		#region [ IDisposable 実装 ]
		//-----------------
		public void Dispose()
		{
			if( !this.bDispose完了済み )
			{
				if( this.devJoystick != null )
				{
					this.devJoystick.Dispose();
					this.devJoystick = null;
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
		private bool[] bButtonPullUp = new bool[ 0x100 ];
		private bool[] bButtonPushDown = new bool[ 0x100 ];
		private bool[] bButtonState = new bool[ 0x100 ];
		private bool bDispose完了済み;
		private Joystick devJoystick;
		private CTimer timer;
		//-----------------
		#endregion
	}
}
