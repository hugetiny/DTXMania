using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using SlimDX;
using FDK;

namespace DTXMania
{
	internal class CAct演奏Combo共通 : CActivity
	{
		// プロパティ

		public STCOMBO n現在のコンボ数;
		[StructLayout( LayoutKind.Sequential )]
		public struct STCOMBO
		{
			public CAct演奏Combo共通 act;

			public int this[ int index ]
			{
				get
				{
					switch( index )
					{
						case 0:
							return this.Drums;

						case 1:
							return this.Guitar;

						case 2:
							return this.Bass;
					}
					throw new IndexOutOfRangeException();
				}
				set
				{
					switch( index )
					{
						case 0:
							this.Drums = value;
							return;

						case 1:
							this.Guitar = value;
							return;

						case 2:
							this.Bass = value;
							return;
					}
					throw new IndexOutOfRangeException();
				}
			}
			public int Drums
			{
				get
				{
					return this.drums;
				}
				set
				{
					this.drums = value;
					if( this.drums > this.Drums最高値 )
					{
						this.Drums最高値 = this.drums;
					}
					this.act.status.Drums.nCOMBO値 = this.drums;
					this.act.status.Drums.n最高COMBO値 = this.Drums最高値;
				}
			}
			public int Guitar
			{
				get
				{
					return this.guitar;
				}
				set
				{
					this.guitar = value;
					if( this.guitar > this.Guitar最高値 )
					{
						this.Guitar最高値 = this.guitar;
					}
					this.act.status.Guitar.nCOMBO値 = this.guitar;
					this.act.status.Guitar.n最高COMBO値 = this.Guitar最高値;
				}
			}
			public int Bass
			{
				get
				{
					return this.bass;
				}
				set
				{
					this.bass = value;
					if( this.bass > this.Bass最高値 )
					{
						this.Bass最高値 = this.bass;
					}
					this.act.status.Bass.nCOMBO値 = this.bass;
					this.act.status.Bass.n最高COMBO値 = this.Bass最高値;
				}
			}
			public int Drums最高値 { get; private set; }
			public int Guitar最高値 { get; private set; }
			public int Bass最高値 { get; private set; }

			private int drums;
			private int guitar;
			private int bass;
		}
		
		protected enum EEvent
		{
			非表示,
			数値更新,
			同一数値,
			ミス通知
		}
		protected enum EMode
		{
			非表示中,
			進行表示中,
			残像表示中
		}
		protected const int nギターコンボのCOMBO文字の高さ = 0x10;
		protected const int nギターコンボのCOMBO文字の幅 = 0x2d;
		protected const int nギターコンボの高さ = 0x23;
		protected const int nギターコンボの幅 = 0x17;
		protected const int nギターコンボの文字間隔 = 1;
		protected const int nドラムコンボのCOMBO文字の高さ = 0x20;
		protected const int nドラムコンボのCOMBO文字の幅 = 90;
		protected const int nドラムコンボの高さ = 70;
		protected const int nドラムコンボの幅 = 0x2d;
		protected const int nドラムコンボの文字間隔 = 2;
		protected int[] nジャンプ差分値 = new int[ 180 ];
		protected CSTATUS status;
		protected CTexture txCOMBOギター;
		protected CTexture txCOMBOドラム;

		// 内部クラス

		protected class CSTATUS
		{
			public CSTAT Bass = new CSTAT();
			public CSTAT Drums = new CSTAT();
			public CSTAT Guitar = new CSTAT();
			public CSTAT this[ int index ]
			{
				get
				{
					switch( index )
					{
						case 0:
							return this.Drums;

						case 1:
							return this.Guitar;

						case 2:
							return this.Bass;
					}
					throw new IndexOutOfRangeException();
				}
				set
				{
					switch( index )
					{
						case 0:
							this.Drums = value;
							return;

						case 1:
							this.Guitar = value;
							return;

						case 2:
							this.Bass = value;
							return;
					}
					throw new IndexOutOfRangeException();
				}
			}

			public class CSTAT
			{
				public CAct演奏Combo共通.EMode e現在のモード;
				public int nCOMBO値;
				public long nコンボが切れた時刻;
				public int nジャンプインデックス値;
				public int n現在表示中のCOMBO値;
				public int n最高COMBO値;
				public int n残像表示中のCOMBO値;
				public long n前回の時刻・ジャンプ用;
			}
		}


		// コンストラクタ

		public CAct演奏Combo共通()
		{
			base.b活性化してない = true;
			for( int i = 0; i < 180; i++ )
			{
				this.nジャンプ差分値[ i ] = (int) ( -15.0 * Math.Sin( ( 3.1415926535897931 * i ) / 180.0 ) );
			}
		}


		// メソッド

		protected virtual void tコンボ表示・ドラム( int nCombo値, int nジャンプインデックス )
		{
			if( CDTXMania.ConfigIni.ドラムコンボ文字の表示位置 != Eドラムコンボ文字の表示位置.OFF )
			{
				int[] numArray = new int[ 10 ];
				int num2 = nCombo値;
				int index = 0;
				while( ( num2 > 0 ) && ( index < 10 ) )
				{
					numArray[ index ] = num2 % 10;
					num2 = ( num2 - ( num2 % 10 ) ) / 10;
					index++;
				}
				if( index != 0 )
				{
					int num3;
					bool guitar = CDTXMania.DTX.bチップがある.Guitar;
					bool bass = CDTXMania.DTX.bチップがある.Bass;
					Eドラムコンボ文字の表示位置 lEFT = CDTXMania.ConfigIni.ドラムコンボ文字の表示位置;
					if( CDTXMania.ConfigIni.bGuitar有効 )
					{
						if( bass )
						{
							lEFT = Eドラムコンボ文字の表示位置.LEFT;
						}
						else if( guitar && ( lEFT == Eドラムコンボ文字の表示位置.RIGHT ) )
						{
							lEFT = Eドラムコンボ文字の表示位置.CENTER;
						}
					}
					switch( lEFT )
					{
						case Eドラムコンボ文字の表示位置.LEFT:
							num3 = 0xbb;
							break;

						case Eドラムコンボ文字の表示位置.CENTER:
							num3 = 320;
							break;

						case Eドラムコンボ文字の表示位置.RIGHT:
							num3 = 0x1e5;
							break;

						default:
							num3 = 0xbb;
							break;
					}
					int num4 = CDTXMania.ConfigIni.bReverse.Drums ? 350 : 60;
					int num5 = ( 0x2f * index ) + 90;
					int x = ( num3 + ( num5 / 2 ) ) - 90;
					int y = ( num4 + 70 ) - 0x20;
					int num8 = nジャンプインデックス - ( index * 50 );
					if( ( num8 >= 0 ) && ( num8 < 180 ) )
					{
						y += this.nジャンプ差分値[ num8 ];
					}
					if( this.txCOMBOドラム != null )
					{
						this.txCOMBOドラム.t2D描画( CDTXMania.app.Device, x, y, new Rectangle( 0, 140, 90, 0x20 ) );
					}
					for( int i = 0; i < index; i++ )
					{
						x -= 0x2f;
						y = num4;
						num8 = nジャンプインデックス - ( ( ( index - i ) - 1 ) * 50 );
						if( ( num8 >= 0 ) && ( num8 < 180 ) )
						{
							y += this.nジャンプ差分値[ num8 ];
						}
						if( this.txCOMBOドラム != null )
						{
							this.txCOMBOドラム.t2D描画( CDTXMania.app.Device, x, y, new Rectangle( ( numArray[ i ] % 5 ) * 0x2d, ( numArray[ i ] / 5 ) * 70, 0x2d, 70 ) );
						}
					}
				}
			}
		}
		protected virtual void tコンボ表示・ギター( int nCombo値, int nジャンプインデックス )
		{
		}
		protected virtual void tコンボ表示・ベース( int nCombo値, int nジャンプインデックス )
		{
		}
		protected void tコンボ表示・ギター( int nCombo値, int n表示中央X, int n表示中央Y, int nジャンプインデックス )
		{
			if( ( (E判定文字表示位置) CDTXMania.ConfigIni.判定文字表示位置.Guitar ) != E判定文字表示位置.表示OFF )
			{
				int[] numArray = new int[ 10 ];
				int num2 = nCombo値;
				int index = 0;
				while( ( num2 > 0 ) && ( index < 10 ) )
				{
					numArray[ index ] = num2 % 10;
					num2 = ( num2 - ( num2 % 10 ) ) / 10;
					index++;
				}
				if( index != 0 )
				{
					int num3 = 0x17 * index;
					int num4 = nジャンプインデックス;
					float x = 1f;
					if( ( num4 >= 0 ) && ( num4 < 180 ) )
					{
						x = 1f - ( ( (float) this.nジャンプ差分値[ num4 ] ) / 45f );
					}
					if( this.txCOMBOギター != null )
					{
						this.txCOMBOギター.vc拡大縮小倍率 = new Vector3( x, x, 1f );
					}
					int num6 = n表示中央X - ( (int) ( ( 45f * x ) / 2f ) );
					int y = n表示中央Y;
					if( this.txCOMBOギター != null )
					{
						this.txCOMBOギター.t2D描画( CDTXMania.app.Device, num6, y, new Rectangle( 0, 70, 0x2d, 0x10 ) );
					}
					num6 = n表示中央X + ( num3 / 2 );
					for( int i = 0; i < index; i++ )
					{
						num4 = nジャンプインデックス;
						x = 1f;
						if( ( num4 >= 0 ) && ( num4 < 180 ) )
						{
							x = 1f - ( ( (float) this.nジャンプ差分値[ num4 ] ) / 45f );
						}
						if( this.txCOMBOギター != null )
						{
							this.txCOMBOギター.vc拡大縮小倍率 = new Vector3( x, x, 1f );
						}
						num6 -= 0x18;
						y = n表示中央Y - 0x23;
						if( this.txCOMBOギター != null )
						{
							this.txCOMBOギター.t2D描画( CDTXMania.app.Device, num6 - ( (int) ( ( ( x - 1f ) * 23f ) / 2f ) ), y - ( (int) ( ( ( x - 1f ) * 35f ) / 2f ) ), new Rectangle( ( numArray[ i ] % 5 ) * 0x17, ( numArray[ i ] / 5 ) * 0x23, 0x17, 0x23 ) );
						}
					}
				}
			}
		}
		protected void tコンボ表示・ベース( int nCombo値, int n表示中央X, int n表示中央Y, int nジャンプインデックス )
		{
			if( ( (E判定文字表示位置) CDTXMania.ConfigIni.判定文字表示位置.Bass ) != E判定文字表示位置.表示OFF )
			{
				int[] numArray = new int[ 10 ];
				int num2 = nCombo値;
				int index = 0;
				while( ( num2 > 0 ) && ( index < 10 ) )
				{
					numArray[ index ] = num2 % 10;
					num2 = ( num2 - ( num2 % 10 ) ) / 10;
					index++;
				}
				if( index != 0 )
				{
					int num3 = 0x17 * index;
					int num4 = nジャンプインデックス;
					float x = 1f;
					if( ( num4 >= 0 ) && ( num4 < 180 ) )
					{
						x = 1f - ( ( (float) this.nジャンプ差分値[ num4 ] ) / 45f );
					}
					if( this.txCOMBOギター != null )
					{
						this.txCOMBOギター.vc拡大縮小倍率 = new Vector3( x, x, 1f );
					}
					int num6 = n表示中央X - ( (int) ( ( 45f * x ) / 2f ) );
					int y = n表示中央Y;
					if( this.txCOMBOギター != null )
					{
						this.txCOMBOギター.t2D描画( CDTXMania.app.Device, num6, y, new Rectangle( 0, 70, 0x2d, 0x10 ) );
					}
					num6 = n表示中央X + ( num3 / 2 );
					for( int i = 0; i < index; i++ )
					{
						num4 = nジャンプインデックス;
						x = 1f;
						if( ( num4 >= 0 ) && ( num4 < 180 ) )
						{
							x = 1f - ( ( (float) this.nジャンプ差分値[ num4 ] ) / 45f );
						}
						if( this.txCOMBOギター != null )
						{
							this.txCOMBOギター.vc拡大縮小倍率 = new Vector3( x, x, 1f );
						}
						num6 -= 0x18;
						y = n表示中央Y - 0x23;
						if( this.txCOMBOギター != null )
						{
							this.txCOMBOギター.t2D描画( CDTXMania.app.Device, num6 - ( (int) ( ( ( x - 1f ) * 23f ) / 2f ) ), y - ( (int) ( ( ( x - 1f ) * 35f ) / 2f ) ), new Rectangle( ( numArray[ i ] % 5 ) * 0x17, ( numArray[ i ] / 5 ) * 0x23, 0x17, 0x23 ) );
						}
					}
				}
			}
		}


		// CActivity 実装

		public override void On活性化()
		{
			STCOMBO stcombo = new STCOMBO();
			stcombo.act = this;
			this.n現在のコンボ数 = stcombo;
			this.status = new CSTATUS();
			for( int i = 0; i < 3; i++ )
			{
				this.status[ i ].e現在のモード = EMode.非表示中;
				this.status[ i ].nCOMBO値 = 0;
				this.status[ i ].n最高COMBO値 = 0;
				this.status[ i ].n現在表示中のCOMBO値 = 0;
				this.status[ i ].n残像表示中のCOMBO値 = 0;
				this.status[ i ].nジャンプインデックス値 = 0x1869f;
				this.status[ i ].n前回の時刻・ジャンプ用 = -1;
				this.status[ i ].nコンボが切れた時刻 = -1;
			}
			base.On活性化();
		}
		public override void On非活性化()
		{
			if( this.status != null )
			{
				this.status = null;
			}
			base.On非活性化();
		}
		public override void OnManagedリソースの作成()
		{
			if( !base.b活性化してない )
			{
				this.txCOMBOドラム = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenPlayDrums combo drums.png" ) );
				this.txCOMBOギター = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenPlayDrums combo guitar.png" ) );
				base.OnManagedリソースの作成();
			}
		}
		public override void OnManagedリソースの解放()
		{
			if( !base.b活性化してない )
			{
				CDTXMania.tテクスチャの解放( ref this.txCOMBOドラム );
				CDTXMania.tテクスチャの解放( ref this.txCOMBOギター );
				base.OnManagedリソースの解放();
			}
		}
		public override int On進行描画()
		{
			if( !base.b活性化してない )
			{
				for( int i = 2; i >= 0; i-- )
				{
					EEvent event2;
					if( this.status[ i ].n現在表示中のCOMBO値 == this.status[ i ].nCOMBO値 )
					{
						event2 = EEvent.同一数値;
					}
					else if( this.status[ i ].n現在表示中のCOMBO値 > this.status[ i ].nCOMBO値 )
					{
						event2 = EEvent.ミス通知;
					}
					else if( ( this.status[ i ].n現在表示中のCOMBO値 < CDTXMania.ConfigIni.n表示可能な最小コンボ数[ i ] ) && ( this.status[ i ].nCOMBO値 < CDTXMania.ConfigIni.n表示可能な最小コンボ数[ i ] ) )
					{
						event2 = EEvent.非表示;
					}
					else
					{
						event2 = EEvent.数値更新;
					}
					if( this.status[ i ].nジャンプインデックス値 < 360 )
					{
						if( ( this.status[ i ].n前回の時刻・ジャンプ用 == -1 ) || ( CDTXMania.Timer.n現在時刻 < this.status[ i ].n前回の時刻・ジャンプ用 ) )
						{
							this.status[ i ].n前回の時刻・ジャンプ用 = CDTXMania.Timer.n現在時刻;
						}
						while( ( CDTXMania.Timer.n現在時刻 - this.status[ i ].n前回の時刻・ジャンプ用 ) >= 2 )
						{
							if( this.status[ i ].nジャンプインデックス値 < 0x7d0 )
							{
								CSTATUS.CSTAT cstat1 = this.status[ i ];
								cstat1.nジャンプインデックス値 += 3;
							}
							CSTATUS.CSTAT cstat2 = this.status[ i ];
							cstat2.n前回の時刻・ジャンプ用 += 2;
						}
					}
				Label_017F:
					switch( this.status[ i ].e現在のモード )
					{
						case EMode.非表示中:
							if( event2 != EEvent.数値更新 )
							{
								break;
							}
							this.status[ i ].e現在のモード = EMode.進行表示中;
							this.status[ i ].nジャンプインデックス値 = 0;
							this.status[ i ].n前回の時刻・ジャンプ用 = CDTXMania.Timer.n現在時刻;
							goto Label_017F;

						case EMode.進行表示中:
							if( ( event2 != EEvent.非表示 ) && ( event2 != EEvent.ミス通知 ) )
							{
								goto Label_026F;
							}
							this.status[ i ].e現在のモード = EMode.残像表示中;
							this.status[ i ].n残像表示中のCOMBO値 = this.status[ i ].n現在表示中のCOMBO値;
							this.status[ i ].nコンボが切れた時刻 = CDTXMania.Timer.n現在時刻;
							goto Label_017F;

						case EMode.残像表示中:
							if( event2 != EEvent.数値更新 )
							{
								goto Label_037A;
							}
							this.status[ i ].e現在のモード = EMode.進行表示中;
							goto Label_017F;

						default:
							goto Label_03D6;
					}
					this.status[ i ].n現在表示中のCOMBO値 = this.status[ i ].nCOMBO値;
					goto Label_03D6;
				Label_026F:
					if( event2 == EEvent.数値更新 )
					{
						this.status[ i ].nジャンプインデックス値 = 0;
						this.status[ i ].n前回の時刻・ジャンプ用 = CDTXMania.Timer.n現在時刻;
					}
					this.status[ i ].n現在表示中のCOMBO値 = this.status[ i ].nCOMBO値;
					switch( i )
					{
						case 0:
							this.tコンボ表示・ドラム( this.status[ i ].nCOMBO値, this.status[ i ].nジャンプインデックス値 );
							goto Label_03D6;

						case 1:
							this.tコンボ表示・ギター( this.status[ i ].nCOMBO値, this.status[ i ].nジャンプインデックス値 );
							goto Label_03D6;

						case 2:
							this.tコンボ表示・ベース( this.status[ i ].nCOMBO値, this.status[ i ].nジャンプインデックス値 );
							goto Label_03D6;

						default:
							goto Label_03D6;
					}
				Label_037A:
					if( ( CDTXMania.Timer.n現在時刻 - this.status[ i ].nコンボが切れた時刻 ) > 0x3e8 )
					{
						this.status[ i ].e現在のモード = EMode.非表示中;
						goto Label_017F;
					}
					this.status[ i ].n現在表示中のCOMBO値 = this.status[ i ].nCOMBO値;
				Label_03D6: ;
				}
			}
			return 0;
		}
	}
}
