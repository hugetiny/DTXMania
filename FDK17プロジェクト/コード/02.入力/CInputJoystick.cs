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
					this.devJoystick.GetObjectPropertiesById( (int) instance.ObjectType ).SetRange( -1000, 1000 );
					this.devJoystick.GetObjectPropertiesById( (int) instance.ObjectType ).DeadZone = 5000;		// 50%をデッドゾーンに設定
																												// 軸をON/OFFの2値で使うならこれで十分
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
			for ( int i = 0; i < this.nPovState.Length; i++ )
				this.nPovState[ i ] = -1;

			this.timer = new CTimer( CTimer.E種別.MultiMedia );

			this.list入力イベント = new List<STInputEvent>( 32 );
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
			#region [ bButtonフラグ初期化 ]
			for ( int i = 0; i < 256; i++ )
			{
				this.bButtonPushDown[ i ] = false;
				this.bButtonPullUp[ i ] = false;
			}
			#endregion

			if ( ( bWindowがアクティブ中 && !this.devJoystick.Acquire().IsFailure ) && !this.devJoystick.Poll().IsFailure )
			{
				// this.list入力イベント = new List<STInputEvent>( 32 );
				this.list入力イベント.Clear();						// #xxxxx 2012.6.11 yyagi; To optimize, I removed new();


				if( bバッファ入力を使用する )
				{
					#region [ a.バッファ入力 ]
					//-----------------------------
					var bufferedData = this.devJoystick.GetBufferedData();
					if( Result.Last.IsSuccess && bufferedData != null )
					{
						foreach ( JoystickState data in bufferedData )
						{
#if false
//if ( 0 < data.X && data.X < 128 && 0 < data.Y && data.Y < 128 && 0 < data.Z && data.Z < 128 )
{
Trace.TraceInformation( "TS={0}: offset={4}, X={1},Y={2},Z={3}", data.TimeStamp, data.X, data.Y, data.Z, data.JoystickDeviceType);
if ( data.JoystickDeviceType == (int) JoystickDeviceType.POV0 ||
	 data.JoystickDeviceType == (int) JoystickDeviceType.POV1 ||
	 data.JoystickDeviceType == (int) JoystickDeviceType.POV2 ||
	 data.JoystickDeviceType == (int) JoystickDeviceType.POV3) {

//if ( data.JoystickDeviceType== (int)JoystickDeviceType.POV0 )
//{
	 Debug.WriteLine( "POV0です!!" );
}
//Trace.TraceInformation( "TS={0}: X={1},Y={2},Z={3}", data.TimeStamp, data.X, data.Y, data.Z );
string pp = "";
int[] pp0 = data.GetPointOfViewControllers();
for ( int ii = 0; ii < pp0.Length; ii++ )
{
pp += pp0[ ii ];
}
Trace.TraceInformation( "TS={0}: povs={1}", data.TimeStamp, pp );
string pp2 = "", pp3 = "";
for ( int ii = 0; ii < 32; ii++ )
{
pp2 += ( data.IsPressed( ii ) ) ? "1" : "0";
pp3 += ( data.IsReleased( ii ) ) ? "1" : "0";
}
Trace.TraceInformation( "TS={0}: IsPressed={1}, IsReleased={2}", data.TimeStamp, pp2, pp3 );
}
#endif
							switch ( data.JoystickDeviceType )
							{
								case (int)JoystickDeviceType.X:
									#region [ X軸－ ]
									//-----------------------------
									bButtonUpDown( data, data.X, 0, 1 );
									//-----------------------------
									#endregion
									#region [ X軸＋ ]
									//-----------------------------
									bButtonUpDown( data, data.X, 1, 0 );
									//-----------------------------
									#endregion
									break;
								case (int)JoystickDeviceType.Y:
									#region [ Y軸－ ]
									//-----------------------------
									bButtonUpDown( data, data.Y, 2, 3 );
									//-----------------------------
									#endregion
									#region [ Y軸＋ ]
									//-----------------------------
									bButtonUpDown( data, data.Y, 3, 2 );
									#endregion
									break;
								case (int)JoystickDeviceType.Z:
									#region [ Z軸－ ]
									//-----------------------------
									bButtonUpDown( data, data.Z, 4, 5 );
									//-----------------------------
									#endregion
									#region [ Z軸＋ ]
									//-----------------------------
									bButtonUpDown( data, data.Z, 5, 4 );
									#endregion
									break;
								case (int)JoystickDeviceType.POV0:
								case (int)JoystickDeviceType.POV1:
								case (int)JoystickDeviceType.POV2:
								case (int)JoystickDeviceType.POV3:
									// #24341 2011.3.12 yyagi: POV support
									// #26880 2011.12.6 yyagi: improve to support "pullup" of POV buttons
									#region [ POV HAT 4/8way ]
									int[] povs = data.GetPointOfViewControllers();
									if ( povs != null )
									{
										STInputEvent e = new STInputEvent();
										int p = (int) data.JoystickDeviceType - (int) JoystickDeviceType.POV0;	// p = 0,1,2,3
										int nPovDegree = povs[ p ];
										int nWay = ( nPovDegree + 2250 ) / 4500;
										if ( nWay == 8 ) nWay = 0;
									//Debug.WriteLine( "POVS:" + povs[ 0 ].ToString( CultureInfo.CurrentCulture ) + ", " +stevent.nKey );
//Debug.WriteLine( "nPovDegree=" + nPovDegree );
										if ( nPovDegree == -1 )
										{
											e.nKey = 6 + 128 + this.nPovState[ p ];
											this.nPovState[ p ] = -1;
//Debug.WriteLine( "POVS離された" + data.TimeStamp + " " + e.nKey );
											e.b押された = false;
											e.nVelocity = 0;
											this.bButtonState[ e.nKey ] = false;
											this.bButtonPullUp[ e.nKey ] = true;
										} else {
											this.nPovState[ p ] = nWay;
											e.nKey = 6 + 128 + nWay;
											e.b押された = true;
											e.nVelocity = CInput管理.n通常音量;
											this.bButtonState[ e.nKey ] = true;
											this.bButtonPushDown[ e.nKey ] = true;
//Debug.WriteLine( "POVS押された" + data.TimeStamp + " " + e.nKey );
										}
										e.nTimeStamp = data.TimeStamp;
										this.list入力イベント.Add( e );
									}
									#endregion
									break;
								default:
									#region [ ボタン ]
									//-----------------------------
									for ( int i = 0; i < 32; i++ )
									{
										if ( data.IsPressed( i ) )
										{
											STInputEvent e = new STInputEvent();
											e.nKey = 6 + i;
											e.b押された = true;
											e.b離された = false;
											e.nTimeStamp = data.TimeStamp;
											e.nVelocity = CInput管理.n通常音量;
											this.list入力イベント.Add( e );

											this.bButtonState[ 6 + i ] = true;
											this.bButtonPushDown[ 6 + i ] = true;
										}
										else if ( data.IsReleased( i ) )
										{
											var ev = new STInputEvent()
											{
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
									break;
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
						bool bIsButtonPressedReleased = false;
						bool[] buttons = currentState.GetButtons();
						for( int j = 0; ( j < buttons.Length ) && ( j < 128 ); j++ )
						{
							if( this.bButtonState[ 6 + j ] == false && buttons[ j ] )
							{
								STInputEvent item = new STInputEvent();
								item.nKey = 6 + j;
								item.b押された = true;
								item.nTimeStamp = this.timer.nシステム時刻;
								item.nVelocity = CInput管理.n通常音量;
								this.list入力イベント.Add( item );

								this.bButtonState[ 6 + j ] = true;
								this.bButtonPushDown[ 6 + j ] = true;
								bIsButtonPressedReleased = true;
							}
							else if( this.bButtonState[ 6 + j ] == true && !buttons[ j ] )
							{
								STInputEvent event3 = new STInputEvent();
								event3.nKey = 6 + j;
								event3.b押された = false;
								event3.nTimeStamp = this.timer.nシステム時刻;
								event3.nVelocity = CInput管理.n通常音量;
								this.list入力イベント.Add( event3 );

								this.bButtonState[ 6 + j ] = false;
								this.bButtonPullUp[ 6 + j ] = true;
								bIsButtonPressedReleased = true;
							}
						}
						//-----------------------------
						#endregion
						// #24341 2011.3.12 yyagi: POV support
						#region [ POV HAT 4/8way (only single POV switch is supported)]
						int[] povs = currentState.GetPointOfViewControllers();
						if ( povs != null )
						{
							if ( povs[ 0 ] >= 0 )
							{
								int nPovDegree = povs[ 0 ];
								int nWay = ( nPovDegree + 2250 ) / 4500;
								if ( nWay == 8 ) nWay = 0;

								if ( this.bButtonState[ 6 + 128 + nWay ] == false )
								{
									STInputEvent stevent = new STInputEvent();
									stevent.nKey = 6 + 128 + nWay;
									//Debug.WriteLine( "POVS:" + povs[ 0 ].ToString( CultureInfo.CurrentCulture ) + ", " +stevent.nKey );
									stevent.b押された = true;
									stevent.nTimeStamp = currentState.TimeStamp;
									stevent.nVelocity = CInput管理.n通常音量;
									this.list入力イベント.Add( stevent );

									this.bButtonState[ stevent.nKey ] = true;
									this.bButtonPushDown[ stevent.nKey ] = true;
								}
							}
							else if ( bIsButtonPressedReleased == false )	// #xxxxx 2011.12.3 yyagi 他のボタンが何も押され/離されてない＝POVが離された
							{
								int nWay = 0;
								for ( int i = 6 + 0x80; i < 6 + 0x80 + 8; i++ )
								{											// 離されたボタンを調べるために、元々押されていたボタンを探す。
									if ( this.bButtonState[ i ] == true )	// DirectInputを直接いじるならこんなことしなくて良いのに、あぁ面倒。
									{										// この処理が必要なために、POVを1個しかサポートできない。無念。
										nWay = i;
										break;
									}
								}
								if ( nWay != 0 )
								{
									STInputEvent stevent = new STInputEvent();
									stevent.nKey = nWay;
									stevent.b押された = false;
									stevent.nTimeStamp = currentState.TimeStamp;
									stevent.nVelocity = 0;
									this.list入力イベント.Add( stevent );

									this.bButtonState[ nWay ] = false;
									this.bButtonPullUp[ nWay ] = true;
								}
							}
						}
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
				if ( this.list入力イベント != null )
				{
					this.list入力イベント = null;
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
		private bool[] bButtonState = new bool[ 0x100 ];		// 0-5: XYZ, 6 - 0x128+5: buttons, 0x128+6 - 0x128+6+8: POV/HAT
		private int[] nPovState = new int[ 4 ];					// POVの現在値を保持
		private bool bDispose完了済み;
		private Joystick devJoystick;
		private CTimer timer;

		private void bButtonUpDown( JoystickState data, int axisdata, int target, int contrary )	// #26871 2011.12.3 軸の反転に対応するためにリファクタ
		{
			int targetsign = ( target < contrary ) ? -1 : 1;
			if ( Math.Abs( axisdata ) > 500 && ( targetsign == Math.Sign( axisdata ) ) )			// 軸の最大値の半分を超えていて、かつ
			{
				if ( bDoUpDownCore( target, data, false ) )											// 直前までは超えていなければ、今回ON
				{
					//Debug.WriteLine( "X-ON " + data.TimeStamp + " " + axisdata );
				}
				else
				{
					//Debug.WriteLine( "X-ONx " + data.TimeStamp + " " + axisdata );
				}
				bDoUpDownCore( contrary, data, true );												// X軸+ == ON から X軸-のONレンジに来たら、X軸+はOFF
			}
			else if ( ( axisdata <= 0 && targetsign <= 0 ) || ( axisdata >= 0 && targetsign >= 0 ) )	// 軸の最大値の半分を超えておらず、かつ  
			{
				//Debug.WriteLine( "X-OFF? " + data.TimeStamp + " " + axisdata );
				if ( bDoUpDownCore( target, data, true ) )											// 直前までは超えていたのならば、今回OFF
				{
					//Debug.WriteLine( "X-OFF " + data.TimeStamp + " " + axisdata );
				}
				else if ( bDoUpDownCore( contrary, data, true ) )									// X軸+ == ON から X軸-のOFFレンジにきたら、X軸+はOFF
				{
					//Debug.WriteLine( "X-OFFx " + data.TimeStamp + " " + axisdata );
				}
			}
		}

		/// <summary>
		/// 必要に応じて軸ボタンの上げ下げイベントを発生する
		/// </summary>
		/// <param name="target">軸ボタン番号 0=-X 1=+X ... 5=+Z</param>
		/// <param name="data"></param>
		/// <param name="currentMode">直前のボタン状態 true=押されていた</param>
		/// <returns>上げ下げイベント発生時true</returns>
		private bool bDoUpDownCore( int target, JoystickState data, bool lastMode )
		{
			if ( this.bButtonState[ target ] == lastMode )
			{
				STInputEvent e = new STInputEvent();
				e.nKey = target;
				e.b押された = !lastMode;
				e.nTimeStamp = data.TimeStamp;
				e.nVelocity = ( lastMode ) ? 0 : CInput管理.n通常音量;
				this.list入力イベント.Add( e );

				this.bButtonState[ target ] = !lastMode;
				if ( lastMode )
				{
					this.bButtonPullUp[ target ] = true;
				}
				else
				{
					this.bButtonPushDown[ target ] = true;
				}
				return true;
			}
			return false;
		}
		//-----------------
		#endregion
	}
}
