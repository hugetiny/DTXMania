using System;
using System.Collections.Generic;
using System.Text;

namespace FDK
{
	public class CCounter : IDisposable
	{
		// プロパティ(1) 値系

		public int n開始値
		{
			get;
			protected set;
		}
		public int n終了値
		{
			get;
			protected set;
		}
		public int n現在の値
		{
			get;
			set;
		}
		public long n現在の経過時間ms
		{
			get;
			set;
		}


		// プロパティ(2) 状態系

		public bool b進行中
		{
			get { return ( this.n現在の経過時間ms != -1 ); }
		}
		public bool b停止中
		{
			get { return !this.b進行中; }
		}
		public bool b終了値に達した
		{
			get { return ( this.n現在の値 >= this.n終了値 ); }
		}
		public bool b終了値に達してない
		{
			get { return !this.b終了値に達した; }
		}


		// コンストラクタ

		/// <summary>
		/// <para>生成のみ行い、開始はしない。</para>
		/// </summary>
		public CCounter()
		{
			this.timer = new CTimer( CTimer.E種別.MultiMedia );
			this.n開始値 = 0;
			this.n終了値 = 0;
			this.n間隔ms = 0;
			this.n現在の値 = 0;
			this.n現在の経過時間ms = CTimer.n未使用;
			this.m一定間隔処理 = null;
		}

		/// <summary>
		/// <para>生成と同時に開始する。</para>
		/// </summary>
		public CCounter( int n最初のカウント値, int n最後のカウント値, int nカウント値を１増加させるのにかける時間ms )
			: this()
		{
			this.t開始( n最初のカウント値, n最後のカウント値, nカウント値を１増加させるのにかける時間ms );
		}

		#region [ DTXMania用の変換 ]
		public CCounter( int n開始値, int n終了値, int n間隔ms, CTimer timer )
			: this()
		{
//			this.timer = timer;
			this.t開始( n開始値, n終了値, n間隔ms );
		}
		public void t開始( int n最初のカウント値, int n最後のカウント値, int nカウント値を１増加させるのにかける時間ms, CTimer timer )
		{
//			this.timer = timer;
			this.t開始( n最初のカウント値, n最後のカウント値, nカウント値を１増加させるのにかける時間ms );
		}
		#endregion

		// 状態操作メソッド

		/// <summary>
		/// <para>カウントを開始する。</para>
		/// </summary>
		public void t開始( int n最初のカウント値, int n最後のカウント値, int nカウント値を１増加させるのにかける時間ms )
		{
			this.timer.tリセット();
			this.n開始値 = n最初のカウント値;
			this.n終了値 = n最後のカウント値;
			this.n間隔ms = nカウント値を１増加させるのにかける時間ms;
			this.n現在の経過時間ms = this.timer.n現在時刻ms;
			this.n現在の値 = n最初のカウント値;
			this.m一定間隔処理 = new C一定間隔処理();
		}

		/// <summary>
		/// <para>前回の t進行() の呼び出しからの経過時間をもとに、必要なだけカウント値を増加させる。</para>
		/// <para>カウント値が終了値に達している場合は、それ以上増加しない（終了値を維持する）。</para>
		/// <para>戻り値として自分自身を返す。</para>
		/// </summary>
		public CCounter t進行()
		{
			if ( this.m一定間隔処理 != null )
			{
				this.m一定間隔処理.t進行( this.n間隔ms, () =>
				{
					if ( ++this.n現在の値 > this.n終了値 )
						this.n現在の値 = this.n終了値;
				} );
			}

			return this;
		}

		/// <summary>
		/// <para>前回の t進行Loop() の呼び出しからの経過時間をもとに、必要なだけカウント値を増加させる。</para>
		/// <para>カウント値が終了値に達している場合は、次の増加タイミングで開始値に戻る（値がループする）。</para>
		/// <para>戻り値として自分自身を返す。</para>
		/// </summary>
		public CCounter t進行Loop()
		{
			this.m一定間隔処理.t進行( this.n間隔ms, () => {
				if( ++this.n現在の値 > this.n終了値 )
					this.n現在の値 = this.n開始値;
			} );

			return this;
		}

		/// <summary>
		/// <para>カウントを停止する。</para>
		/// <para>これ以降に t進行() や t進行Loop() を呼び出しても何も処理されない。</para>
		/// </summary>
		public void t停止()
		{
			this.n現在の経過時間ms = CTimer.n未使用;
		}


		// その他

		#region [ IDisposable 実装 ]
		//-----------------
		public void Dispose()
		{
			C共通.tDisposeする( ref this.timer );
			C共通.tDisposeする( ref this.m一定間隔処理 );
		}
		//-----------------
		#endregion

		#region [ 応用：キーの反復入力をエミュレーションする ]
		//-----------------

		/// <summary>
		/// <para>「bキー押下」引数が true の間中、「tキー処理」デリゲート引数を呼び出す。</para>
		/// <para>ただし、2回目の呼び出しは1回目から 200ms の間を開けてから行い、3回目以降の呼び出しはそれぞれ 30ms の間隔で呼び出す。</para>
		/// <para>「bキー押下」が false の場合は何もせず、呼び出し回数を 0 にリセットする。</para>
		/// </summary>
		/// <param name="bキー押下">キーが押下されている場合は true。</param>
		/// <param name="tキー処理">キーが押下されている場合に実行する処理。</param>
		public void tキー反復( bool bキー押下, DGキー処理 tキー処理 )
		{
			const int n1回目 = 0;
			const int n2回目 = 1;
			const int n3回目以降 = 2;

			if( bキー押下 )
			{
				switch( this.n現在の値 )
				{
					case n1回目:

						tキー処理();
						this.n現在の値 = n2回目;
						this.n現在の経過時間ms = this.timer.n現在時刻ms;
						return;

					case n2回目:

						if( ( this.timer.n現在時刻ms - this.n現在の経過時間ms ) > 200 )
						{
							tキー処理();
							this.n現在の経過時間ms = this.timer.n現在時刻ms;
							this.n現在の値 = n3回目以降;
						}
						return;

					case n3回目以降:

						if( ( this.timer.n現在時刻ms - this.n現在の経過時間ms ) > 30 )
						{
							tキー処理();
							this.n現在の経過時間ms = this.timer.n現在時刻ms;
						}
						return;
				}
			}
			else
			{
				this.n現在の値 = n1回目;
			}
		}
		public delegate void DGキー処理();

		//-----------------
		#endregion

		#region [ protected ]
		//-----------------
		protected CTimer timer;
		protected int n間隔ms;
		protected C一定間隔処理 m一定間隔処理;
		//-----------------
		#endregion
	}
}