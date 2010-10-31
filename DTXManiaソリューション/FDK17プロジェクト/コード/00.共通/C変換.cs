using System;
using System.Collections.Generic;
using System.Text;

namespace FDK
{
	public class C変換
	{
		// メソッド

		public static bool bONorOFF( char c )
		{
			return ( c != '0' );
		}

		public static double DegreeToRadian( double angle )
		{
			return ( ( Math.PI * angle ) / 180.0 );
		}
		public static double RadianToDegree( double angle )
		{
			return ( angle * 180.0 / Math.PI );
		}
		public static float DegreeToRadian( float angle )
		{
			return (float) DegreeToRadian( (double) angle );
		}
		public static float RadianToDegree( float angle )
		{
			return (float) RadianToDegree( (double) angle );
		}

		public static int n値を範囲内に丸めて返す( int n値, int n最小値, int n最大値 )
		{
			if( n値 < n最小値 )
				return n最小値;

			if( n値 > n最大値 )
				return n最大値;

			return n値;
		}
		public static int n値を文字列から取得して範囲内に丸めて返す( string str数値文字列, int n最小値, int n最大値, int n取得失敗時のデフォルト値 )
		{
			int num;
			if( ( int.TryParse( str数値文字列, out num ) && ( num >= n最小値 ) ) && ( num <= n最大値 ) )
				return num;

			return n取得失敗時のデフォルト値;
		}
		public static int n値を文字列から取得して返す( string str数値文字列, int n取得失敗時のデフォルト値 )
		{
			int num;
			if( !int.TryParse( str数値文字列, out num ) )
				num = n取得失敗時のデフォルト値;

			return num;
		}
		
		public static int n16進数2桁の文字列を数値に変換して返す( string num )
		{
			if( num.Length < 2 )
				return -1;

			int index = _16進数文字.IndexOf( num[ 0 ] );
			if( index < 0 )
				return -1;

			if( index >= 16 )
				index -= 6;

			int num3 = _16進数文字.IndexOf( num[ 1 ] );
			if( num3 < 0 )
				return -1;

			if( num3 >= 16 )
				num3 -= 6;

			return ( ( index * 16 ) + num3 );
		}
		public static int n36進数2桁の文字列を数値に変換して返す( string num )
		{
			if( num.Length < 2 )
				return -1;

			int index = _36進数文字.IndexOf( num[ 0 ] );
			if( index < 0 )
				return -1;

			if( index >= 0x24 )
				index -= 0x1a;

			int num3 = _36進数文字.IndexOf( num[ 1 ] );
			if( num3 < 0 )
				return -1;

			if( num3 >= 0x24 )
				num3 -= 0x1a;

			return ( ( index * 0x24 ) + num3 );
		}
		public static int n小節番号の文字列3桁を数値に変換して返す( string num )
		{
			if( num.Length >= 3 )
			{
				int index = _36進数文字.IndexOf( num[ 0 ] );
				if( index < 0 )
					return -1;

				if( index >= 0x24 )
					index -= 0x1a;

				int num3 = _16進数文字.IndexOf( num[ 1 ] );
				if( ( num3 < 0 ) || ( num3 > 9 ) )
					return -1;

				int num4 = _16進数文字.IndexOf( num[ 2 ] );
				if( ( num4 >= 0 ) && ( num4 <= 9 ) )
					return ( ( ( index * 100 ) + ( num3 * 10 ) ) + num4 );
			}
			return -1;
		}
		
		public static string str小節番号を文字列3桁に変換して返す( int num )
		{
			if( ( num < 0 ) || ( num >= 0xe10 ) )
				return "000";

			int num2 = num / 100;
			int num3 = ( num % 100 ) / 10;
			int num4 = ( num % 100 ) % 10;
			char ch = _36進数文字[ num2 ];
			char ch2 = _16進数文字[ num3 ];
			char ch3 = _16進数文字[ num4 ];
			return ( ch.ToString() + ch2.ToString() + ch3.ToString() );
		}
		public static string str数値を16進数2桁に変換して返す( int num )
		{
			if( ( num < 0 ) || ( num >= 0x100 ) )
				return "00";

			char ch = _16進数文字[ num / 0x10 ];
			char ch2 = _16進数文字[ num % 0x10 ];
			return ( ch.ToString() + ch2.ToString() );
		}
		public static string str数値を36進数2桁に変換して返す( int num )
		{
			if( ( num < 0 ) || ( num >= 0x510 ) )
				return "00";

			char ch = _36進数文字[ num / 0x24 ];
			char ch2 = _36進数文字[ num % 0x24 ];
			return ( ch.ToString() + ch2.ToString() );
		}


		// その他

		#region [ private ]
		//-----------------

		// private コンストラクタでインスタンス生成を禁止する。
		private C変換()
		{
		}

		private static readonly string _16進数文字 = "0123456789ABCDEFabcdef";
		private static readonly string _36進数文字 = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
		//-----------------
		#endregion
	} 
}
