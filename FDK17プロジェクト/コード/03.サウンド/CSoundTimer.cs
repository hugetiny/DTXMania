using System;
using System.Collections.Generic;
using System.Text;

namespace FDK
{
	public class CSoundTimer : CTimerBase
	{
		public override long nシステム時刻ms
		{
			get
			{
				if( this.Device.e出力デバイス == ESoundDeviceType.ExclusiveWASAPI || 
					this.Device.e出力デバイス == ESoundDeviceType.SharedWASAPI ||
					this.Device.e出力デバイス == ESoundDeviceType.ASIO )
				{
					// BASS 系の ISoundDevice.n経過時間ms はオーディオバッファの更新間隔ずつでしか更新されないため、単にこれを返すだけではとびとびの値になる。
					// そこで、更新間隔の最中に呼ばれた場合は、システムタイマを使って補間する。
					// この場合の経過時間との誤差は更新間隔以内に収まるので問題ないと判断する。

					return this.Device.n経過時間ms
						+ ( this.Device.tmシステムタイマ.nシステム時刻ms - this.Device.n経過時間を更新したシステム時刻ms );
				}
				else if( this.Device.e出力デバイス == ESoundDeviceType.DirectSound )
				{
					return this.Device.n経過時間ms;
				}
				return CTimerBase.n未使用;
			}
		}
		
		public CSoundTimer( ISoundDevice device )
		{
			this.Device = device;
		}
		public override void Dispose()
		{
			// 特になし； ISoundDevice の解放は呼び出し元で行うこと。
		}

		protected ISoundDevice Device = null;
	}
}
