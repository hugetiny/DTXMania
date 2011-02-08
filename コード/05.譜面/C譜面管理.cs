using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DTXCreator.UndoRedo;
using FDK;

namespace DTXCreator.譜面
{
	internal class C譜面管理
	{
		public Bitmap bmp背景画像;
		public Dictionary<int, float> dicBPx = new Dictionary<int, float>();
		public Dictionary<int, C小節> dic小節 = new Dictionary<int, C小節>();
		public List<Cレーン> listレーン = new List<Cレーン>();
		public static readonly int nレーン割付チップ番号表示高さdot = 10;
		public static readonly int nレーン番号表示高さdot = 0x20;
		public int n現在のガイド幅grid = ( C小節.n基準の高さgrid / 0x10 );
		public int n現在の譜面表示下辺の譜面先頭からの位置grid;
		public string strPATH_WAV = "";

		public C譜面管理( Cメインフォーム cm )
		{
			this._Form = cm;
		}
		public bool bOPENチップである( Cチップ cc )
		{
			Cレーン cレーン = this.listレーン[ cc.nレーン番号0to ];
			return ( ( cc.n値・整数1to1295 == 2 ) && ( ( cレーン.eレーン種別 == Cレーン.E種別.GtR ) || ( cレーン.eレーン種別 == Cレーン.E種別.BsR ) ) );
		}
		public bool b確定選択中のチップがある()
		{
			foreach( KeyValuePair<int, C小節> pair in this.dic小節 )
			{
				foreach( Cチップ cチップ in pair.Value.listチップ )
				{
					if( cチップ.b確定選択中 || cチップ.bドラッグで選択中 )
					{
						return true;
					}
				}
			}
			return false;
		}
		public decimal dc譜面先頭からの位置gridにおけるBPMを返す( int n譜面先頭からの位置grid )
		{
			decimal num = this._Form.dc現在のBPM;
			C小節 c小節 = this.p譜面先頭からの位置gridを含む小節を返す( n譜面先頭からの位置grid );
			if( c小節 != null )
			{
				for( int i = 0; i <= c小節.n小節番号0to3599; i++ )
				{
					C小節 c小節2 = this.p小節を返す( i );
					if( c小節2 != null )
					{
						int num3 = this.n譜面先頭からみた小節先頭の位置gridを返す( i );
						foreach( Cチップ cチップ in c小節2.listチップ )
						{
							if( ( cチップ.nチャンネル番号00toFF == 8 ) && ( ( num3 + cチップ.n位置grid ) <= n譜面先頭からの位置grid ) )
							{
								num = (decimal) cチップ.f値・浮動小数;
							}
						}
					}
				}
			}
			return num;
		}
		public int nX座標dotが位置するレーン番号を返す( int nXdot )
		{
			if( nXdot >= 0 )
			{
				int num = 0;
				int num2 = 0;
				foreach( Cレーン cレーン in this.listレーン )
				{
					num += cレーン.n幅dot;
					if( nXdot < num )
					{
						return num2;
					}
					num2++;
				}
			}
			return -1;
		}
		public int nY座標dotが位置するgridを返す・ガイド幅単位( int nY )
		{
			int num = this.nY座標dotが位置するgridを返す・最高解像度( nY );
			C小節 c小節 = this.p譜面先頭からの位置gridを含む小節を返す( num );
			if( c小節 == null )
			{
				c小節 = this.p小節を返す( this.n現在の最大の小節番号を返す() );
			}
			int num2 = this.n譜面先頭からみた小節先頭の位置gridを返す( c小節.n小節番号0to3599 );
			int num3 = ( ( num - num2 ) / this.n現在のガイド幅grid ) * this.n現在のガイド幅grid;
			return ( num2 + num3 );
		}
		public int nY座標dotが位置するgridを返す・最高解像度( int nY )
		{
			int num = this._Form.pictureBox譜面パネル.ClientSize.Height - nY;
			return ( ( num / C小節.n１グリッドの高さdot ) + this.n現在の譜面表示下辺の譜面先頭からの位置grid );
		}
		public int nレーンの左端X座標dotを返す( int nレーン番号0to )
		{
			if( ( nレーン番号0to >= 0 ) && ( nレーン番号0to < this.listレーン.Count ) )
			{
				return this.listレーン[ nレーン番号0to ].n位置Xdot;
			}
			return -1;
		}
		public int nレーン名に対応するレーン番号を返す( string strレーン名 )
		{
			int num = 0;
			foreach( Cレーン cレーン in this.listレーン )
			{
				if( cレーン.strレーン名.Equals( strレーン名 ) )
				{
					return num;
				}
				num++;
			}
			return -1;
		}
		public int n現在の最大の小節番号を返す()
		{
			int num = -1;
			foreach( KeyValuePair<int, C小節> pair in this.dic小節 )
			{
				C小節 c小節 = pair.Value;
				if( c小節.n小節番号0to3599 > num )
				{
					num = c小節.n小節番号0to3599;
				}
			}
			return num;
		}
		public int n全小節の高さdotの合計を返す()
		{
			int num = 0;
			foreach( KeyValuePair<int, C小節> pair in this.dic小節 )
			{
				num += pair.Value.n小節長倍率を考慮した現在の小節の高さdot;
			}
			return num;
		}
		public int n全小節の高さgridの合計を返す()
		{
			int num = 0;
			foreach( KeyValuePair<int, C小節> pair in this.dic小節 )
			{
				num += pair.Value.n小節長倍率を考慮した現在の小節の高さgrid;
			}
			return num;
		}
		public int n譜面先頭からの位置gridから描画領域内のY座標dotを返す( int n譜面先頭からの位置grid, Size sz描画領域dot )
		{
			int num = n譜面先頭からの位置grid - this.n現在の譜面表示下辺の譜面先頭からの位置grid;
			return ( sz描画領域dot.Height - ( num * C小節.n１グリッドの高さdot ) );
		}
		public int n譜面先頭からみた小節先頭の位置gridを返す( int n小節番号0to3599 )
		{
			if( ( n小節番号0to3599 < 0 ) || ( n小節番号0to3599 > this.n現在の最大の小節番号を返す() ) )
			{
				return -1;
			}
			int num = 0;
			for( int i = 0; i < n小節番号0to3599; i++ )
			{
				C小節 c小節 = this.p小節を返す( i );
				if( c小節 != null )
				{
					num += c小節.n小節長倍率を考慮した現在の小節の高さgrid;
				}
			}
			return num;
		}
		public C小節 pチップの存在する小節を返す( Cチップ cc )
		{
			foreach( KeyValuePair<int, C小節> pair in this.dic小節 )
			{
				C小節 c小節 = pair.Value;
				if( c小節.listチップ.Contains( cc ) )
				{
					return c小節;
				}
			}
			return null;
		}
		public Cチップ p指定された座標dotにあるチップを返す( int x, int y )
		{
			int num = this.nX座標dotが位置するレーン番号を返す( x );
			if( num >= 0 )
			{
				int num2 = this.nY座標dotが位置するgridを返す・最高解像度( y );
				C小節 c小節 = this.p譜面先頭からの位置gridを含む小節を返す( num2 );
				if( c小節 == null )
				{
					return null;
				}
				int num3 = this.n譜面先頭からみた小節先頭の位置gridを返す( c小節.n小節番号0to3599 );
				foreach( Cチップ cチップ in c小節.listチップ )
				{
					int num4 = num3 + cチップ.n位置grid;
					int num5 = C小節.n位置変換dot2grid( Cチップ.nチップの高さdot );
					if( ( ( cチップ.nレーン番号0to == num ) && ( num4 <= num2 ) ) && ( num2 <= ( num4 + num5 ) ) )
					{
						return cチップ;
					}
				}
			}
			return null;
		}
		public Cチップ p指定位置にあるチップを返す( int n小節番号0to, int nレーン番号0to, int n小節内の位置grid )
		{
			C小節 c小節 = this.p小節を返す( n小節番号0to );
			if( c小節 != null )
			{
				for( int i = 0; i < c小節.listチップ.Count; i++ )
				{
					Cチップ cチップ = c小節.listチップ[ i ];
					if( ( cチップ.n位置grid == n小節内の位置grid ) && ( cチップ.nレーン番号0to == nレーン番号0to ) )
					{
						return cチップ;
					}
				}
			}
			return null;
		}
		public C小節 p小節を返す( int n小節番号0to3599 )
		{
			C小節 c小節 = null;
			if( this.dic小節.TryGetValue( n小節番号0to3599, out c小節 ) )
			{
				return c小節;
			}
			return null;
		}
		public C小節 p譜面先頭からの位置gridを含む小節を返す( int n譜面先頭からの位置grid )
		{
			int num = 0;
			int num2 = this.n現在の最大の小節番号を返す();
			for( int i = 0; i <= num2; i++ )
			{
				C小節 c小節 = this.p小節を返す( i );
				if( c小節 != null )
				{
					num += c小節.n小節長倍率を考慮した現在の小節の高さgrid;
					if( n譜面先頭からの位置grid < num )
					{
						return c小節;
					}
				}
			}
			return null;
		}
		public void tチップを削除する( int nレーン番号0to, int n譜面先頭からの位置grid )
		{
			C小節 c小節 = this.p譜面先頭からの位置gridを含む小節を返す( n譜面先頭からの位置grid );
			if( c小節 != null )
			{
				bool flag3;
				int num = n譜面先頭からの位置grid - this.n譜面先頭からみた小節先頭の位置gridを返す( c小節.n小節番号0to3599 );
				this._Form.mgrUndoRedo管理者.tトランザクション記録を開始する();
				do
				{
					Cレーン cレーン = this.listレーン[ nレーン番号0to ];
					bool flag = ( ( cレーン.eレーン種別 == Cレーン.E種別.GtR ) || ( cレーン.eレーン種別 == Cレーン.E種別.GtG ) ) || ( cレーン.eレーン種別 == Cレーン.E種別.GtB );
					bool flag2 = ( ( cレーン.eレーン種別 == Cレーン.E種別.BsR ) || ( cレーン.eレーン種別 == Cレーン.E種別.BsG ) ) || ( cレーン.eレーン種別 == Cレーン.E種別.BsB );
					flag3 = true;
					foreach( Cチップ cチップ in c小節.listチップ )
					{
						if( cチップ.n位置grid == num )
						{
							if( cチップ.nレーン番号0to == nレーン番号0to )
							{
								Cチップ cc = new Cチップ();
								cc.tコピーfrom( cチップ );
								Cチップ配置用UndoRedo redo = new Cチップ配置用UndoRedo( c小節.n小節番号0to3599, cc );
								this._Form.mgrUndoRedo管理者.tノードを追加する( new CUndoRedoセル<Cチップ配置用UndoRedo>( null, new DGUndoを実行する<Cチップ配置用UndoRedo>( this.tチップ削除のUndo ), new DGRedoを実行する<Cチップ配置用UndoRedo>( this.tチップ削除のRedo ), redo, redo ) );
								c小節.listチップ.Remove( cチップ );
								flag3 = false;
								this._Form.b未保存 = true;
								break;
							}
							if( ( flag && ( this.listレーン[ cチップ.nレーン番号0to ].eレーン種別 == Cレーン.E種別.GtR ) ) && ( cチップ.n値・整数1to1295 == 2 ) )
							{
								Cチップ cチップ3 = new Cチップ();
								cチップ3.tコピーfrom( cチップ );
								Cチップ配置用UndoRedo redo2 = new Cチップ配置用UndoRedo( c小節.n小節番号0to3599, cチップ3 );
								this._Form.mgrUndoRedo管理者.tノードを追加する( new CUndoRedoセル<Cチップ配置用UndoRedo>( null, new DGUndoを実行する<Cチップ配置用UndoRedo>( this.tチップ削除のUndo ), new DGRedoを実行する<Cチップ配置用UndoRedo>( this.tチップ削除のRedo ), redo2, redo2 ) );
								c小節.listチップ.Remove( cチップ );
								flag3 = false;
								this._Form.b未保存 = true;
								break;
							}
							if( ( flag2 && ( this.listレーン[ cチップ.nレーン番号0to ].eレーン種別 == Cレーン.E種別.BsR ) ) && ( cチップ.n値・整数1to1295 == 2 ) )
							{
								Cチップ cチップ4 = new Cチップ();
								cチップ4.tコピーfrom( cチップ );
								Cチップ配置用UndoRedo redo3 = new Cチップ配置用UndoRedo( c小節.n小節番号0to3599, cチップ4 );
								this._Form.mgrUndoRedo管理者.tノードを追加する( new CUndoRedoセル<Cチップ配置用UndoRedo>( null, new DGUndoを実行する<Cチップ配置用UndoRedo>( this.tチップ削除のUndo ), new DGRedoを実行する<Cチップ配置用UndoRedo>( this.tチップ削除のRedo ), redo3, redo3 ) );
								c小節.listチップ.Remove( cチップ );
								flag3 = false;
								this._Form.b未保存 = true;
								break;
							}
						}
					}
				}
				while( !flag3 );
				this._Form.tUndoRedo用GUIの有効・無効を設定する();
				this._Form.mgrUndoRedo管理者.tトランザクション記録を終了する();
			}
		}
		public void tチップを配置または置換する( int nレーン番号0to, int n譜面先頭からの位置grid, int nチップ値・整数, float fチップ値・浮動小数, bool b裏として配置 )
		{
			C小節 c小節 = this.p譜面先頭からの位置gridを含む小節を返す( n譜面先頭からの位置grid );
			if( c小節 != null )
			{
				this._Form.mgrUndoRedo管理者.tトランザクション記録を開始する();
				this.tチップを削除する( nレーン番号0to, n譜面先頭からの位置grid );
				if( ( ( this.listレーン[ nレーン番号0to ].eレーン種別 == Cレーン.E種別.GtR ) || ( this.listレーン[ nレーン番号0to ].eレーン種別 == Cレーン.E種別.BsR ) ) && ( nチップ値・整数 == 2 ) )
				{
					this.tチップを削除する( nレーン番号0to + 1, n譜面先頭からの位置grid );
					this.tチップを削除する( nレーン番号0to + 2, n譜面先頭からの位置grid );
				}
				Cレーン cレーン = this.listレーン[ nレーン番号0to ];
				Cチップ item = new Cチップ();
				item.nチャンネル番号00toFF = b裏として配置 ? cレーン.nチャンネル番号・裏00toFF : cレーン.nチャンネル番号・表00toFF;
				item.nレーン番号0to = nレーン番号0to;
				item.n位置grid = n譜面先頭からの位置grid - this.n譜面先頭からみた小節先頭の位置gridを返す( c小節.n小節番号0to3599 );
				item.n値・整数1to1295 = nチップ値・整数;
				item.f値・浮動小数 = fチップ値・浮動小数;
				item.b裏 = b裏として配置;
				c小節.listチップ.Add( item );
				Cチップ cc = new Cチップ();
				cc.tコピーfrom( item );
				Cチップ配置用UndoRedo redo = new Cチップ配置用UndoRedo( c小節.n小節番号0to3599, cc );
				this._Form.mgrUndoRedo管理者.tノードを追加する( new CUndoRedoセル<Cチップ配置用UndoRedo>( null, new DGUndoを実行する<Cチップ配置用UndoRedo>( this.tチップ配置のUndo ), new DGRedoを実行する<Cチップ配置用UndoRedo>( this.tチップ配置のRedo ), redo, redo ) );
				int num = this.n現在の最大の小節番号を返す();
				if( c小節.n小節番号0to3599 == num )
				{
					for( int i = num + 1; i <= ( num + 4 ); i++ )
					{
						this.dic小節.Add( i, new C小節( i ) );
						this._Form.mgrUndoRedo管理者.tノードを追加する( new CUndoRedoセル<int>( null, new DGUndoを実行する<int>( this.t小節挿入のUndo ), new DGRedoを実行する<int>( this.t小節挿入のRedo ), i, i ) );
					}
				}
				this._Form.mgrUndoRedo管理者.tトランザクション記録を終了する();
				this._Form.tUndoRedo用GUIの有効・無効を設定する();
				this._Form.b未保存 = true;
			}
		}
		public void tチップ削除のRedo( Cチップ配置用UndoRedo ur変更前, Cチップ配置用UndoRedo ur変更後 )
		{
			C小節 c小節 = this.p小節を返す( ur変更前.n小節番号0to );
			if( c小節 != null )
			{
				for( int i = 0; i < c小節.listチップ.Count; i++ )
				{
					Cチップ cチップ = c小節.listチップ[ i ];
					if( ( cチップ.n位置grid == ur変更前.cc.n位置grid ) && ( cチップ.nレーン番号0to == ur変更前.cc.nレーン番号0to ) )
					{
						c小節.listチップ.RemoveAt( i );
						this._Form.b未保存 = true;
						return;
					}
				}
			}
		}
		public void tチップ削除のUndo( Cチップ配置用UndoRedo ur変更前, Cチップ配置用UndoRedo ur変更後 )
		{
			C小節 c小節 = this.p小節を返す( ur変更前.n小節番号0to );
			if( c小節 != null )
			{
				Cチップ item = new Cチップ();
				item.tコピーfrom( ur変更前.cc );
				c小節.listチップ.Add( item );
				this._Form.b未保存 = true;
			}
		}
		public void tチップ選択のRedo( Cチップ位置用UndoRedo ur変更前, Cチップ位置用UndoRedo ur変更後 )
		{
			Cチップ cチップ = this.p指定位置にあるチップを返す( ur変更前.n小節番号0to, ur変更前.nレーン番号0to, ur変更前.n位置grid );
			if( cチップ != null )
			{
				cチップ.b確定選択中 = true;
			}
		}
		public void tチップ選択のUndo( Cチップ位置用UndoRedo ur変更前, Cチップ位置用UndoRedo ur変更後 )
		{
			Cチップ cチップ = this.p指定位置にあるチップを返す( ur変更前.n小節番号0to, ur変更前.nレーン番号0to, ur変更前.n位置grid );
			if( cチップ != null )
			{
				cチップ.b確定選択中 = false;
			}
		}
		public void tチップ選択解除のRedo( Cチップ位置用UndoRedo ur変更前, Cチップ位置用UndoRedo ur変更後 )
		{
			Cチップ cチップ = this.p指定位置にあるチップを返す( ur変更前.n小節番号0to, ur変更前.nレーン番号0to, ur変更前.n位置grid );
			if( cチップ != null )
			{
				cチップ.b確定選択中 = false;
			}
		}
		public void tチップ選択解除のUndo( Cチップ位置用UndoRedo ur変更前, Cチップ位置用UndoRedo ur変更後 )
		{
			Cチップ cチップ = this.p指定位置にあるチップを返す( ur変更前.n小節番号0to, ur変更前.nレーン番号0to, ur変更前.n位置grid );
			if( cチップ != null )
			{
				cチップ.b確定選択中 = true;
			}
		}
		public void tチップ配置のRedo( Cチップ配置用UndoRedo ur変更前, Cチップ配置用UndoRedo ur変更後 )
		{
			C小節 c小節 = this.p小節を返す( ur変更前.n小節番号0to );
			if( c小節 != null )
			{
				Cチップ item = new Cチップ();
				item.tコピーfrom( ur変更前.cc );
				c小節.listチップ.Add( item );
				this._Form.b未保存 = true;
			}
		}
		public void tチップ配置のUndo( Cチップ配置用UndoRedo ur変更前, Cチップ配置用UndoRedo ur変更後 )
		{
			C小節 c小節 = this.p小節を返す( ur変更前.n小節番号0to );
			if( c小節 != null )
			{
				for( int i = 0; i < c小節.listチップ.Count; i++ )
				{
					Cチップ cチップ = c小節.listチップ[ i ];
					if( ( cチップ.n位置grid == ur変更前.cc.n位置grid ) && ( cチップ.nレーン番号0to == ur変更前.cc.nレーン番号0to ) )
					{
						c小節.listチップ.RemoveAt( i );
						this._Form.b未保存 = true;
						return;
					}
				}
			}
		}
		public void tチップ番号置換のRedo( Cチップ位置用UndoRedo ur変更前, Cチップ位置用UndoRedo ur変更後 )
		{
			Cチップ cチップ = this.p指定位置にあるチップを返す( ur変更前.n小節番号0to, ur変更前.nレーン番号0to, ur変更前.n位置grid );
			if( cチップ != null )
			{
				cチップ.n値・整数1to1295 = ur変更後.n値・整数1to1295;
				this._Form.b未保存 = true;
			}
		}
		public void tチップ番号置換のUndo( Cチップ位置用UndoRedo ur変更前, Cチップ位置用UndoRedo ur変更後 )
		{
			Cチップ cチップ = this.p指定位置にあるチップを返す( ur変更後.n小節番号0to, ur変更後.nレーン番号0to, ur変更後.n位置grid );
			if( cチップ != null )
			{
				cチップ.n値・整数1to1295 = ur変更前.n値・整数1to1295;
				this._Form.b未保存 = true;
			}
		}
		public void tチップ表裏反転のRedo( Cチップ位置用UndoRedo ur変更前, Cチップ位置用UndoRedo ur変更後 )
		{
			Cチップ cチップ = this.p指定位置にあるチップを返す( ur変更前.n小節番号0to, ur変更前.nレーン番号0to, ur変更前.n位置grid );
			if( cチップ != null )
			{
				if( cチップ.b裏 )
				{
					cチップ.nチャンネル番号00toFF = this.listレーン[ cチップ.nレーン番号0to ].nチャンネル番号・表00toFF;
					cチップ.b裏 = false;
				}
				else
				{
					cチップ.nチャンネル番号00toFF = this.listレーン[ cチップ.nレーン番号0to ].nチャンネル番号・裏00toFF;
					cチップ.b裏 = true;
				}
				this._Form.b未保存 = true;
			}
		}
		public void tチップ表裏反転のUndo( Cチップ位置用UndoRedo ur変更前, Cチップ位置用UndoRedo ur変更後 )
		{
			Cチップ cチップ = this.p指定位置にあるチップを返す( ur変更前.n小節番号0to, ur変更前.nレーン番号0to, ur変更前.n位置grid );
			if( cチップ != null )
			{
				if( cチップ.b裏 )
				{
					cチップ.nチャンネル番号00toFF = this.listレーン[ cチップ.nレーン番号0to ].nチャンネル番号・表00toFF;
					cチップ.b裏 = false;
				}
				else
				{
					cチップ.nチャンネル番号00toFF = this.listレーン[ cチップ.nレーン番号0to ].nチャンネル番号・裏00toFF;
					cチップ.b裏 = true;
				}
				this._Form.b未保存 = true;
			}
		}
		public void t初期化()
		{
			this.t初期化・listレーンの生成();
			this.t初期化・小節を１０個まで作成する();
			this.t初期化・背景画像を生成する();
			this.t初期化・スクロールバーを初期設定する();
		}
		public void t小節削除のRedo( int nダミー, int n削除する小節番号0to )
		{
			this.dic小節.Remove( n削除する小節番号0to );
			int num = this.n現在の最大の小節番号を返す();
			for( int i = n削除する小節番号0to + 1; i <= num; i++ )
			{
				C小節 c小節 = this.p小節を返す( i );
				if( c小節 != null )
				{
					this.dic小節.Remove( i );
					c小節.n小節番号0to3599 = i - 1;
					this.dic小節.Add( c小節.n小節番号0to3599, c小節 );
				}
			}
			this._Form.b未保存 = true;
		}
		public void t小節削除のUndo( int n削除された小節番号0to, int nダミー )
		{
			for( int i = this.n現在の最大の小節番号を返す(); i >= n削除された小節番号0to; i-- )
			{
				C小節 c小節 = this.p小節を返す( i );
				if( c小節 != null )
				{
					this.dic小節.Remove( i );
					c小節.n小節番号0to3599 = i + 1;
					this.dic小節.Add( c小節.n小節番号0to3599, c小節 );
				}
			}
			this.dic小節.Add( n削除された小節番号0to, new C小節( n削除された小節番号0to ) );
			this._Form.b未保存 = true;
		}
		public void t小節挿入のRedo( int nダミー, int n挿入する小節番号0to )
		{
			for( int i = this.n現在の最大の小節番号を返す(); i >= n挿入する小節番号0to; i-- )
			{
				C小節 c小節 = this.p小節を返す( i );
				if( c小節 != null )
				{
					this.dic小節.Remove( i );
					c小節.n小節番号0to3599 = i + 1;
					this.dic小節.Add( c小節.n小節番号0to3599, c小節 );
				}
			}
			this.dic小節.Add( n挿入する小節番号0to, new C小節( n挿入する小節番号0to ) );
			this._Form.b未保存 = true;
		}
		public void t小節挿入のUndo( int n挿入された小節番号0to, int nダミー )
		{
			this.dic小節.Remove( n挿入された小節番号0to );
			int num = this.n現在の最大の小節番号を返す();
			for( int i = n挿入された小節番号0to + 1; i <= num; i++ )
			{
				C小節 c小節 = this.p小節を返す( i );
				if( c小節 != null )
				{
					this.dic小節.Remove( i );
					c小節.n小節番号0to3599 = i - 1;
					this.dic小節.Add( c小節.n小節番号0to3599, c小節 );
				}
			}
			this._Form.b未保存 = true;
		}
		public void t小節長変更のRedo( C小節用UndoRedo ur変更前, C小節用UndoRedo ur変更後 )
		{
			C小節 c小節 = this.p小節を返す( ur変更後.n小節番号0to );
			if( c小節 != null )
			{
				c小節.f小節長倍率 = ur変更後.f倍率;
				this._Form.b未保存 = true;
			}
		}
		public void t小節長変更のUndo( C小節用UndoRedo ur変更前, C小節用UndoRedo ur変更後 )
		{
			C小節 c小節 = this.p小節を返す( ur変更前.n小節番号0to );
			if( c小節 != null )
			{
				c小節.f小節長倍率 = ur変更前.f倍率;
				this._Form.b未保存 = true;
			}
		}
		public void t垂直スクロールバーと譜面可視領域の上下位置の調整()
		{
			VScrollBar bar = this._Form.vScrollBar譜面用垂直スクロールバー;
			PictureBox box = this._Form.pictureBox譜面パネル;
			bar.Minimum = 0;
			bar.Maximum = this.n全小節の高さgridの合計を返す() - 1;
			bar.SmallChange = C小節.n基準の高さgrid / 0x10;
			bar.LargeChange = C小節.n基準の高さgrid;
			bar.Enabled = bar.Maximum > bar.LargeChange;
			if( bar.Enabled )
			{
				this.n現在の譜面表示下辺の譜面先頭からの位置grid = ( ( bar.Maximum + 1 ) - bar.LargeChange ) - bar.Value;
				box.Refresh();
			}
		}
		public void t水平スクロールバーと譜面パネル左右位置の調整()
		{
			HScrollBar bar = this._Form.hScrollBar譜面用水平スクロールバー;
			SplitContainer container = this._Form.splitContainerタブと譜面を分割;
			PictureBox box = this._Form.pictureBox譜面パネル;
			bar.Minimum = 0;
			bar.Maximum = box.Width - 1;
			bar.SmallChange = 5;
			bar.LargeChange = container.Panel2.Width;
			bar.Enabled = bar.Maximum > bar.LargeChange;
			if( bar.Enabled )
			{
				int num = bar.Value;
				if( ( num + bar.LargeChange ) > bar.Maximum )
				{
					num = ( bar.Maximum - bar.LargeChange ) + 1;
				}
				bar.Value = num;
				box.Location = new Point( -num, 0 );
				box.Refresh();
			}
		}
		public void t譜面を描画する( Graphics g, Size sz描画対象サイズdot, Rectangle rc可視領域dot )
		{
			this.strfmt小節番号文字フォーマット.LineAlignment = StringAlignment.Center;
			this.strfmt小節番号文字フォーマット.Alignment = StringAlignment.Center;
			this.strfmtレーン名文字フォーマット.LineAlignment = StringAlignment.Near;
			this.strfmtレーン名文字フォーマット.Alignment = StringAlignment.Center;
			this.strfmtレーン割付チップ番号文字フォーマット.LineAlignment = StringAlignment.Near;
			this.strfmtレーン割付チップ番号文字フォーマット.Alignment = StringAlignment.Near;
			int num = 0;
			int num2 = 0;
			int num3 = this.n現在の譜面表示下辺の譜面先頭からの位置grid * C小節.n１グリッドの高さdot;
			int num4 = num3 + rc可視領域dot.Height;
			int num5 = this.n現在の最大の小節番号を返す();
			int num6 = 0;
			while( num6 <= num5 )
			{
				C小節 cs = this.p小節を返す( num6 );
				if( cs != null )
				{
					num2 = num + cs.n小節長倍率を考慮した現在の小節の高さdot;
					if( num >= num4 )
					{
						break;
					}
					if( num2 > num3 )
					{
						Rectangle rectangle = new Rectangle();
						rectangle.X = 0;
						rectangle.Y = sz描画対象サイズdot.Height - ( num2 - num3 );
						rectangle.Width = sz描画対象サイズdot.Width;
						rectangle.Height = num2 - num;
						Rectangle rectangle2 = new Rectangle();
						rectangle2.X = rc可視領域dot.X;
						rectangle2.Y = rectangle.Y;
						rectangle2.Width = rc可視領域dot.Width;
						rectangle2.Height = rectangle.Height;
						this.t譜面を描画する・１小節を描画する( g, cs, rectangle, rectangle2 );
					}
				}
				num6++;
				num = num2;
			}
			Rectangle rectangle3 = new Rectangle();
			rectangle3.X = 0;
			rectangle3.Y = 0;
			rectangle3.Width = sz描画対象サイズdot.Width;
			rectangle3.Height = nレーン割付チップ番号表示高さdot;
			this.t譜面を描画する・レーン割付チップを描画する( g, rectangle3 );
			rectangle3.X = 0;
			rectangle3.Y = 10;
			rectangle3.Width = sz描画対象サイズdot.Width;
			rectangle3.Height = nレーン番号表示高さdot;
			this.t譜面を描画する・レーン名を描画する( g, rectangle3 );
		}

		#region [ private ]
		//-----------------
		private Cメインフォーム _Form;
		private Brush brレーン割付番号文字ブラシ = new SolidBrush( Color.White );
		private Brush brレーン割付番号文字ブラシ影 = new SolidBrush( Color.Black );
		private Brush brレーン名文字ブラシ = new SolidBrush( Color.FromArgb( 0xff, 220, 220, 220 ) );
		private Brush brレーン名文字ブラシ影 = new SolidBrush( Color.Black );
		private Brush br小節番号文字ブラシ = new SolidBrush( Color.FromArgb( 80, 0xff, 0xff, 0xff ) );
		private Font ftレーン割付チップ番号文字フォント = new Font( "MS UI Gothic", 7f, FontStyle.Regular );
		private Font ftレーン番号文字フォント = new Font( "MS US Gothic", 8f, FontStyle.Regular );
		private Font ft小節番号文字フォント = new Font( "MS UI Gothic", 50f, FontStyle.Regular );
		private Pen penガイド線ペン = new Pen( Color.FromArgb( 50, 50, 50 ) );
		private Pen penレーン区分線ペン細 = new Pen( Color.Gray );
		private Pen penレーン区分線ペン太 = new Pen( Color.White, 2f );
		private Pen pen小節線ペン = new Pen( Color.White, 2f );
		private Pen pen拍線ペン = new Pen( Color.Gray );
		private StringFormat strfmtレーン割付チップ番号文字フォーマット = new StringFormat();
		private StringFormat strfmtレーン名文字フォーマット = new StringFormat();
		private StringFormat strfmt小節番号文字フォーマット = new StringFormat();

		private void t初期化・listレーンの生成()
		{
			this.listレーン.Clear();
			int num = 0;
			int num2 = 30;
			int alpha = 0x19;
			Cレーン item = new Cレーン();
			item.eレーン種別 = Cレーン.E種別.BPM;
			item.strレーン名 = "BPM";
			item.nチャンネル番号・表00toFF = 8;
			item.nチャンネル番号・裏00toFF = 3;
			item.b右側の線が太線 = true;
			item.col背景色 = Color.FromArgb( alpha, 160, 160, 160 );
			item.n位置Xdot = num;
			item.n幅dot = num2;
			this.listレーン.Add( item );
			num += item.n幅dot;
			item = new Cレーン();
			item.eレーン種別 = Cレーン.E種別.WAV;
			item.strレーン名 = "LC";
			item.nチャンネル番号・表00toFF = 0x1a;
			item.nチャンネル番号・裏00toFF = 0x1a;
			item.b右側の線が太線 = true;
			item.col背景色 = Color.FromArgb( alpha, 0, 0xff, 0xff );
			item.n位置Xdot = num;
			item.n幅dot = num2;
			this.listレーン.Add( item );
			num += item.n幅dot;
			item = new Cレーン();
			item.eレーン種別 = Cレーン.E種別.WAV;
			item.strレーン名 = "HH";
			item.nチャンネル番号・表00toFF = 0x11;
			item.nチャンネル番号・裏00toFF = 0x18;
			item.b右側の線が太線 = false;
			item.col背景色 = Color.FromArgb( alpha, 0, 0xff, 0xff );
			item.n位置Xdot = num;
			item.n幅dot = num2;
			this.listレーン.Add( item );
			num += item.n幅dot;
			item = new Cレーン();
			item.eレーン種別 = Cレーン.E種別.WAV;
			item.strレーン名 = "SD";
			item.nチャンネル番号・表00toFF = 0x12;
			item.nチャンネル番号・裏00toFF = 0x12;
			item.b右側の線が太線 = false;
			item.col背景色 = Color.FromArgb( alpha, 0xff, 0xff, 0 );
			item.n位置Xdot = num;
			item.n幅dot = num2;
			this.listレーン.Add( item );
			num += item.n幅dot;
			item = new Cレーン();
			item.eレーン種別 = Cレーン.E種別.WAV;
			item.strレーン名 = "BD";
			item.nチャンネル番号・表00toFF = 0x13;
			item.nチャンネル番号・裏00toFF = 0x13;
			item.b右側の線が太線 = false;
			item.col背景色 = Color.FromArgb( alpha, 0xff, 0x7f, 0x7f );
			item.n位置Xdot = num;
			item.n幅dot = num2;
			this.listレーン.Add( item );
			num += item.n幅dot;
			item = new Cレーン();
			item.eレーン種別 = Cレーン.E種別.WAV;
			item.strレーン名 = "HT";
			item.nチャンネル番号・表00toFF = 20;
			item.nチャンネル番号・裏00toFF = 20;
			item.b右側の線が太線 = false;
			item.col背景色 = Color.FromArgb( alpha, 0, 0xff, 0 );
			item.n位置Xdot = num;
			item.n幅dot = num2;
			this.listレーン.Add( item );
			num += item.n幅dot;
			item = new Cレーン();
			item.eレーン種別 = Cレーン.E種別.WAV;
			item.strレーン名 = "LT";
			item.nチャンネル番号・表00toFF = 0x15;
			item.nチャンネル番号・裏00toFF = 0x15;
			item.b右側の線が太線 = false;
			item.col背景色 = Color.FromArgb( alpha, 0xff, 0, 0 );
			item.n位置Xdot = num;
			item.n幅dot = num2;
			this.listレーン.Add( item );
			num += item.n幅dot;
			item = new Cレーン();
			item.eレーン種別 = Cレーン.E種別.WAV;
			item.strレーン名 = "FT";
			item.nチャンネル番号・表00toFF = 0x17;
			item.nチャンネル番号・裏00toFF = 0x17;
			item.b右側の線が太線 = false;
			item.col背景色 = Color.FromArgb( alpha, 0xff, 0, 0xff );
			item.n位置Xdot = num;
			item.n幅dot = num2;
			this.listレーン.Add( item );
			num += item.n幅dot;
			item = new Cレーン();
			item.eレーン種別 = Cレーン.E種別.WAV;
			item.strレーン名 = "CY";
			item.nチャンネル番号・表00toFF = 0x16;
			item.nチャンネル番号・裏00toFF = 0x19;
			item.b右側の線が太線 = false;
			item.col背景色 = Color.FromArgb( alpha, 0, 0xff, 0xff );
			item.n位置Xdot = num;
			item.n幅dot = num2;
			this.listレーン.Add( item );
			num += item.n幅dot;
			item = new Cレーン();
			item.eレーン種別 = Cレーン.E種別.FI;
			item.strレーン名 = "FI";
			item.nチャンネル番号・表00toFF = 0x53;
			item.nチャンネル番号・裏00toFF = 0x53;
			item.b右側の線が太線 = true;
			item.col背景色 = Color.FromArgb( alpha, 0xff, 0xff, 0 );
			item.n位置Xdot = num;
			item.n幅dot = num2;
			this.listレーン.Add( item );
			num += item.n幅dot;
			item = new Cレーン();
			item.eレーン種別 = Cレーン.E種別.WAV;
			item.strレーン名 = "BGM";
			item.nチャンネル番号・表00toFF = 1;
			item.nチャンネル番号・裏00toFF = 1;
			item.b右側の線が太線 = true;
			item.col背景色 = Color.FromArgb( alpha, 160, 160, 160 );
			item.n位置Xdot = num;
			item.n幅dot = num2;
			this.listレーン.Add( item );
			num += item.n幅dot;
			item = new Cレーン();
			item.eレーン種別 = Cレーン.E種別.WAV;
			item.strレーン名 = "SE1";
			item.nチャンネル番号・表00toFF = 0x61;
			item.nチャンネル番号・裏00toFF = 0x61;
			item.b右側の線が太線 = false;
			item.col背景色 = Color.FromArgb( alpha, 160, 160, 160 );
			item.n位置Xdot = num;
			item.n幅dot = num2;
			this.listレーン.Add( item );
			num += item.n幅dot;
			item = new Cレーン();
			item.eレーン種別 = Cレーン.E種別.WAV;
			item.strレーン名 = "SE2";
			item.nチャンネル番号・表00toFF = 0x62;
			item.nチャンネル番号・裏00toFF = 0x62;
			item.b右側の線が太線 = false;
			item.col背景色 = Color.FromArgb( alpha, 160, 160, 160 );
			item.n位置Xdot = num;
			item.n幅dot = num2;
			this.listレーン.Add( item );
			num += item.n幅dot;
			item = new Cレーン();
			item.eレーン種別 = Cレーン.E種別.WAV;
			item.strレーン名 = "SE3";
			item.nチャンネル番号・表00toFF = 0x63;
			item.nチャンネル番号・裏00toFF = 0x63;
			item.b右側の線が太線 = false;
			item.col背景色 = Color.FromArgb( alpha, 160, 160, 160 );
			item.n位置Xdot = num;
			item.n幅dot = num2;
			this.listレーン.Add( item );
			num += item.n幅dot;
			item = new Cレーン();
			item.eレーン種別 = Cレーン.E種別.WAV;
			item.strレーン名 = "SE4";
			item.nチャンネル番号・表00toFF = 100;
			item.nチャンネル番号・裏00toFF = 100;
			item.b右側の線が太線 = false;
			item.col背景色 = Color.FromArgb( alpha, 160, 160, 160 );
			item.n位置Xdot = num;
			item.n幅dot = num2;
			this.listレーン.Add( item );
			num += item.n幅dot;
			item = new Cレーン();
			item.eレーン種別 = Cレーン.E種別.WAV;
			item.strレーン名 = "SE5";
			item.nチャンネル番号・表00toFF = 0x65;
			item.nチャンネル番号・裏00toFF = 0x65;
			item.b右側の線が太線 = false;
			item.col背景色 = Color.FromArgb( alpha, 160, 160, 160 );
			item.n位置Xdot = num;
			item.n幅dot = num2;
			this.listレーン.Add( item );
			num += item.n幅dot;
			item = new Cレーン();
			item.eレーン種別 = Cレーン.E種別.GtV;
			item.strレーン名 = "GtV";
			item.nチャンネル番号・表00toFF = 0;
			item.nチャンネル番号・裏00toFF = 0;
			item.b右側の線が太線 = true;
			item.col背景色 = Color.FromArgb( alpha, 160, 160, 160 );
			item.n位置Xdot = num;
			item.n幅dot = num2;
			this.listレーン.Add( item );
			num += item.n幅dot;
			item = new Cレーン();
			item.eレーン種別 = Cレーン.E種別.GtR;
			item.strレーン名 = "GtR";
			item.nチャンネル番号・表00toFF = 0;
			item.nチャンネル番号・裏00toFF = 0;
			item.b右側の線が太線 = false;
			item.col背景色 = Color.FromArgb( alpha, 0xff, 0, 0 );
			item.n位置Xdot = num;
			item.n幅dot = num2;
			this.listレーン.Add( item );
			num += item.n幅dot;
			item = new Cレーン();
			item.eレーン種別 = Cレーン.E種別.GtG;
			item.strレーン名 = "GtG";
			item.nチャンネル番号・表00toFF = 0;
			item.nチャンネル番号・裏00toFF = 0;
			item.b右側の線が太線 = false;
			item.col背景色 = Color.FromArgb( alpha, 0, 0xff, 0 );
			item.n位置Xdot = num;
			item.n幅dot = num2;
			this.listレーン.Add( item );
			num += item.n幅dot;
			item = new Cレーン();
			item.eレーン種別 = Cレーン.E種別.GtB;
			item.strレーン名 = "GtB";
			item.nチャンネル番号・表00toFF = 0;
			item.nチャンネル番号・裏00toFF = 0;
			item.b右側の線が太線 = false;
			item.col背景色 = Color.FromArgb( alpha, 0, 0x80, 0xff );
			item.n位置Xdot = num;
			item.n幅dot = num2;
			this.listレーン.Add( item );
			num += item.n幅dot;
			item = new Cレーン();
			item.eレーン種別 = Cレーン.E種別.GtW;
			item.strレーン名 = "GtW";
			item.nチャンネル番号・表00toFF = 40;
			item.nチャンネル番号・裏00toFF = 40;
			item.b右側の線が太線 = true;
			item.col背景色 = Color.FromArgb( alpha, 160, 160, 160 );
			item.n位置Xdot = num;
			item.n幅dot = num2;
			this.listレーン.Add( item );
			num += item.n幅dot;
			item = new Cレーン();
			item.eレーン種別 = Cレーン.E種別.BsV;
			item.strレーン名 = "BsV";
			item.nチャンネル番号・表00toFF = 0;
			item.nチャンネル番号・裏00toFF = 0;
			item.b右側の線が太線 = true;
			item.col背景色 = Color.FromArgb( alpha, 160, 160, 160 );
			item.n位置Xdot = num;
			item.n幅dot = num2;
			this.listレーン.Add( item );
			num += item.n幅dot;
			item = new Cレーン();
			item.eレーン種別 = Cレーン.E種別.BsR;
			item.strレーン名 = "BsR";
			item.nチャンネル番号・表00toFF = 0;
			item.nチャンネル番号・裏00toFF = 0;
			item.b右側の線が太線 = false;
			item.col背景色 = Color.FromArgb( alpha, 0xff, 0, 0 );
			item.n位置Xdot = num;
			item.n幅dot = num2;
			this.listレーン.Add( item );
			num += item.n幅dot;
			item = new Cレーン();
			item.eレーン種別 = Cレーン.E種別.BsG;
			item.strレーン名 = "BsG";
			item.nチャンネル番号・表00toFF = 0;
			item.nチャンネル番号・裏00toFF = 0;
			item.b右側の線が太線 = false;
			item.col背景色 = Color.FromArgb( alpha, 0, 0xff, 0 );
			item.n位置Xdot = num;
			item.n幅dot = num2;
			this.listレーン.Add( item );
			num += item.n幅dot;
			item = new Cレーン();
			item.eレーン種別 = Cレーン.E種別.BsB;
			item.strレーン名 = "BsB";
			item.nチャンネル番号・表00toFF = 0;
			item.nチャンネル番号・裏00toFF = 0;
			item.b右側の線が太線 = false;
			item.col背景色 = Color.FromArgb( alpha, 0, 0x80, 0xff );
			item.n位置Xdot = num;
			item.n幅dot = num2;
			this.listレーン.Add( item );
			num += item.n幅dot;
			item = new Cレーン();
			item.eレーン種別 = Cレーン.E種別.BsW;
			item.strレーン名 = "BsW";
			item.nチャンネル番号・表00toFF = 0xa8;
			item.nチャンネル番号・裏00toFF = 0xa8;
			item.b右側の線が太線 = true;
			item.col背景色 = Color.FromArgb( alpha, 160, 160, 160 );
			item.n位置Xdot = num;
			item.n幅dot = num2;
			this.listレーン.Add( item );
			num += item.n幅dot;
			item = new Cレーン();
			item.eレーン種別 = Cレーン.E種別.AVI;
			item.strレーン名 = "AVI";
			item.nチャンネル番号・表00toFF = 0x54;
			item.nチャンネル番号・裏00toFF = 0x54;
			item.b右側の線が太線 = true;
			item.col背景色 = Color.FromArgb( alpha, 160, 160, 160 );
			item.n位置Xdot = num;
			item.n幅dot = num2;
			this.listレーン.Add( item );
			num += item.n幅dot;
			item = new Cレーン();
			item.eレーン種別 = Cレーン.E種別.BMP;
			item.strレーン名 = "BG1";
			item.nチャンネル番号・表00toFF = 4;
			item.nチャンネル番号・裏00toFF = 0xc4;
			item.b右側の線が太線 = true;
			item.col背景色 = Color.FromArgb( alpha, 160, 160, 160 );
			item.n位置Xdot = num;
			item.n幅dot = num2;
			this.listレーン.Add( item );
			num += item.n幅dot;
			item = new Cレーン();
			item.eレーン種別 = Cレーン.E種別.BMP;
			item.strレーン名 = "BG2";
			item.nチャンネル番号・表00toFF = 7;
			item.nチャンネル番号・裏00toFF = 0xc7;
			item.b右側の線が太線 = false;
			item.col背景色 = Color.FromArgb( alpha, 160, 160, 160 );
			item.n位置Xdot = num;
			item.n幅dot = num2;
			this.listレーン.Add( item );
			num += item.n幅dot;
			item = new Cレーン();
			item.eレーン種別 = Cレーン.E種別.BMP;
			item.strレーン名 = "BG3";
			item.nチャンネル番号・表00toFF = 0x55;
			item.nチャンネル番号・裏00toFF = 0xd5;
			item.b右側の線が太線 = false;
			item.col背景色 = Color.FromArgb( alpha, 160, 160, 160 );
			item.n位置Xdot = num;
			item.n幅dot = num2;
			this.listレーン.Add( item );
			num += item.n幅dot;
			item = new Cレーン();
			item.eレーン種別 = Cレーン.E種別.BMP;
			item.strレーン名 = "BG4";
			item.nチャンネル番号・表00toFF = 0x56;
			item.nチャンネル番号・裏00toFF = 0xd6;
			item.b右側の線が太線 = false;
			item.col背景色 = Color.FromArgb( alpha, 160, 160, 160 );
			item.n位置Xdot = num;
			item.n幅dot = num2;
			this.listレーン.Add( item );
			num += item.n幅dot;
			item = new Cレーン();
			item.eレーン種別 = Cレーン.E種別.BMP;
			item.strレーン名 = "BG5";
			item.nチャンネル番号・表00toFF = 0x57;
			item.nチャンネル番号・裏00toFF = 0xd7;
			item.b右側の線が太線 = false;
			item.col背景色 = Color.FromArgb( alpha, 160, 160, 160 );
			item.n位置Xdot = num;
			item.n幅dot = num2;
			this.listレーン.Add( item );
			num += item.n幅dot;
		}
		private void t初期化・スクロールバーを初期設定する()
		{
			this._Form.hScrollBar譜面用水平スクロールバー.Value = 0;
			this._Form.vScrollBar譜面用垂直スクロールバー.Value = 0;
			this.t水平スクロールバーと譜面パネル左右位置の調整();
			this.t垂直スクロールバーと譜面可視領域の上下位置の調整();
			this._Form.vScrollBar譜面用垂直スクロールバー.Value = ( this._Form.vScrollBar譜面用垂直スクロールバー.Maximum + 1 ) - this._Form.vScrollBar譜面用垂直スクロールバー.LargeChange;
		}
		private void t初期化・小節を１０個まで作成する()
		{
			for( int i = 0; i < 10; i++ )
			{
				this.dic小節.Add( i, new C小節( i ) );
			}
		}
		private void t初期化・背景画像を生成する()
		{
			int width = 0;
			foreach( Cレーン cレーン in this.listレーン )
			{
				width += cレーン.n幅dot;
			}
			this.bmp背景画像 = new Bitmap( width, 1 );
			Graphics graphics = Graphics.FromImage( this.bmp背景画像 );
			int x = 0;
			foreach( Cレーン cレーン2 in this.listレーン )
			{
				graphics.FillRectangle( new SolidBrush( cレーン2.col背景色 ), x, 0, cレーン2.n幅dot, this.bmp背景画像.Height );
				x += cレーン2.n幅dot;
			}
			graphics.Dispose();
			this._Form.pictureBox譜面パネル.Width = this.bmp背景画像.Width;
			this._Form.pictureBox譜面パネル.BackgroundImage = this.bmp背景画像;
			this._Form.pictureBox譜面パネル.BackgroundImageLayout = ImageLayout.Tile;
		}
		private void t譜面を描画する・１小節を描画する( Graphics g, C小節 cs, Rectangle rc小節のPicBox内描画領域, Rectangle rc小節のPicBox内可視領域 )
		{
			this.t譜面を描画する・１小節を描画する・小節番号を描画する( g, cs, rc小節のPicBox内可視領域 );
			this.t譜面を描画する・１小節を描画する・ガイド線を描画する( g, cs, rc小節のPicBox内描画領域 );
			this.t譜面を描画する・１小節を描画する・拍線を描画する( g, cs, rc小節のPicBox内描画領域 );
			this.t譜面を描画する・１小節を描画する・レーン区分線を描画する( g, cs, rc小節のPicBox内描画領域 );
			this.t譜面を描画する・１小節を描画する・小節線を描画する( g, cs, rc小節のPicBox内描画領域 );
			this.t譜面を描画する・１小節を描画する・チップを描画する( g, cs, rc小節のPicBox内描画領域 );
		}
		private void t譜面を描画する・１小節を描画する・ガイド線を描画する( Graphics g, C小節 cs, Rectangle rc小節のPicBox内描画領域 )
		{
			if( this.n現在のガイド幅grid > 1 )
			{
				int num = cs.n小節長倍率を考慮した現在の小節の高さgrid;
				for( int i = 0; i < num; i += this.n現在のガイド幅grid )
				{
					int num3 = rc小節のPicBox内描画領域.Bottom - ( i * C小節.n１グリッドの高さdot );
					g.DrawLine( this.penガイド線ペン, rc小節のPicBox内描画領域.X, num3, rc小節のPicBox内描画領域.Right, num3 );
				}
			}
		}
		private void t譜面を描画する・１小節を描画する・チップを描画する( Graphics g, C小節 cs, Rectangle rc小節のPicBox内描画領域 )
		{
			cs.listチップ.Sort();
			Rectangle rectangle = new Rectangle( 0, 0, 0, 0 );
			foreach( Cチップ cチップ in cs.listチップ )
			{
				if( cチップ.n枠外レーン数 != 0 )
				{
					continue;
				}
				int num = cチップ.n位置grid;
				Cレーン cレーン = this.listレーン[ cチップ.nレーン番号0to ];
				rectangle.X = cレーン.n位置Xdot;
				rectangle.Y = ( rc小節のPicBox内描画領域.Bottom - ( num * C小節.n１グリッドの高さdot ) ) - Cチップ.nチップの高さdot;
				rectangle.Width = cレーン.n幅dot;
				rectangle.Height = Cチップ.nチップの高さdot;
				if( !cチップ.b裏 )
				{
					switch( cレーン.eレーン種別 )
					{
						case Cレーン.E種別.GtR:
						case Cレーン.E種別.GtG:
						case Cレーン.E種別.GtB:
						case Cレーン.E種別.BsR:
						case Cレーン.E種別.BsG:
						case Cレーン.E種別.BsB:
							if( ( ( cレーン.eレーン種別 != Cレーン.E種別.GtR ) || ( cチップ.n値・整数1to1295 != 2 ) ) && ( ( cレーン.eレーン種別 != Cレーン.E種別.BsR ) || ( cチップ.n値・整数1to1295 != 2 ) ) )
							{
								break;
							}
							rectangle.Width = cレーン.n幅dot * 3;
							Cチップ.tOPENチップを描画する( g, rectangle );
							goto Label_0188;

						case Cレーン.E種別.BPM:
							Cチップ.t表チップを描画する( g, rectangle, cチップ.f値・浮動小数, cレーン.col背景色 );
							goto Label_0188;

						default:
							Cチップ.t表チップを描画する( g, rectangle, cチップ.n値・整数1to1295, cレーン.col背景色 );
							goto Label_0188;
					}
					Cチップ.t表チップを描画する( g, rectangle, -1, cレーン.col背景色 );
				}
				else if( cレーン.eレーン種別 == Cレーン.E種別.BPM )
				{
					Cチップ.t裏チップを描画する( g, rectangle, cチップ.f値・浮動小数, cレーン.col背景色 );
				}
				else
				{
					Cチップ.t裏チップを描画する( g, rectangle, cチップ.n値・整数1to1295, cレーン.col背景色 );
				}
			Label_0188:
				if( cチップ.bドラッグで選択中 || cチップ.b確定選択中 )
				{
					Cチップ.tチップの周囲の太枠を描画する( g, rectangle );
				}
			}
		}
		private void t譜面を描画する・１小節を描画する・レーン区分線を描画する( Graphics g, C小節 cs, Rectangle rc小節のPicBox内描画領域 )
		{
			int num = 0;
			foreach( Cレーン cレーン in this.listレーン )
			{
				Pen pen = cレーン.b右側の線が太線 ? this.penレーン区分線ペン太 : this.penレーン区分線ペン細;
				g.DrawLine( pen, num, rc小節のPicBox内描画領域.Top, num, rc小節のPicBox内描画領域.Bottom );
				num += cレーン.n幅dot;
			}
		}
		private void t譜面を描画する・１小節を描画する・小節線を描画する( Graphics g, C小節 cs, Rectangle rc小節のPicBox内描画領域 )
		{
			Rectangle rectangle = rc小節のPicBox内描画領域;
			g.DrawLine( this.pen小節線ペン, rectangle.X, rectangle.Bottom, rectangle.Right, rectangle.Bottom );
			g.DrawLine( this.pen小節線ペン, rectangle.X, rectangle.Top, rectangle.Right, rectangle.Top );
		}
		private void t譜面を描画する・１小節を描画する・小節番号を描画する( Graphics g, C小節 cs, Rectangle rc小節のPicBox内可視領域 )
		{
			g.DrawString( C変換.str小節番号を文字列3桁に変換して返す( cs.n小節番号0to3599 ), this.ft小節番号文字フォント, this.br小節番号文字ブラシ, rc小節のPicBox内可視領域, this.strfmt小節番号文字フォーマット );
		}
		private void t譜面を描画する・１小節を描画する・拍線を描画する( Graphics g, C小節 cs, Rectangle rc小節のPicBox内描画領域 )
		{
			int num = cs.n小節長倍率を考慮した現在の小節の高さgrid;
			for( int i = 0; i < num; i += C小節.n基準の高さgrid / 4 )
			{
				int num3 = rc小節のPicBox内描画領域.Bottom - ( i * C小節.n１グリッドの高さdot );
				g.DrawLine( this.pen拍線ペン, rc小節のPicBox内描画領域.X, num3, rc小節のPicBox内描画領域.Right, num3 );
			}
		}
		private void t譜面を描画する・レーン割付チップを描画する( Graphics g, Rectangle rc小節のPicBox内描画領域 )
		{
			LinearGradientBrush brush = new LinearGradientBrush( rc小節のPicBox内描画領域, Color.FromArgb( 0xff, 0, 0, 100 ), Color.FromArgb( 0xff, 100, 100, 0xff ), LinearGradientMode.Vertical );
			g.FillRectangle( brush, rc小節のPicBox内描画領域 );
			brush.Dispose();
			Rectangle layoutRectangle = new Rectangle( 0, 0, 0, 0 );
			foreach( Cレーン cレーン in this.listレーン )
			{
				layoutRectangle.X = ( rc小節のPicBox内描画領域.X + cレーン.n位置Xdot ) + 1;
				layoutRectangle.Y = rc小節のPicBox内描画領域.Y + 1;
				layoutRectangle.Width = cレーン.n幅dot;
				layoutRectangle.Height = rc小節のPicBox内描画領域.Height;
				if( cレーン.nレーン割付チップ・表0or1to1295 > 0 )
				{
					string s = C変換.str数値を36進数2桁に変換して返す( cレーン.nレーン割付チップ・表0or1to1295 );
					g.DrawString( s, this.ftレーン割付チップ番号文字フォント, this.brレーン割付番号文字ブラシ影, layoutRectangle, this.strfmtレーン割付チップ番号文字フォーマット );
					layoutRectangle.X--;
					layoutRectangle.Y--;
					g.DrawString( s, this.ftレーン割付チップ番号文字フォント, this.brレーン割付番号文字ブラシ, layoutRectangle, this.strfmtレーン割付チップ番号文字フォーマット );
					layoutRectangle.X++;
					layoutRectangle.Y++;
				}
				layoutRectangle.X += cレーン.n幅dot / 2;
				if( cレーン.nレーン割付チップ・裏0or1to1295 > 0 )
				{
					string str2 = C変換.str数値を36進数2桁に変換して返す( cレーン.nレーン割付チップ・裏0or1to1295 );
					g.DrawString( str2, this.ftレーン割付チップ番号文字フォント, this.brレーン割付番号文字ブラシ影, layoutRectangle, this.strfmtレーン割付チップ番号文字フォーマット );
					layoutRectangle.X--;
					layoutRectangle.Y--;
					g.DrawString( str2, this.ftレーン割付チップ番号文字フォント, this.brレーン割付番号文字ブラシ, layoutRectangle, this.strfmtレーン割付チップ番号文字フォーマット );
				}
			}
		}
		private void t譜面を描画する・レーン名を描画する( Graphics g, Rectangle rcレーン名のPicBox内描画領域 )
		{
			LinearGradientBrush brush = new LinearGradientBrush( rcレーン名のPicBox内描画領域, Color.FromArgb( 0xff, 100, 100, 0xff ), Color.FromArgb( 0, 0, 0, 0xff ), LinearGradientMode.Vertical );
			g.FillRectangle( brush, rcレーン名のPicBox内描画領域 );
			brush.Dispose();
			Rectangle layoutRectangle = new Rectangle( 0, 0, 0, 0 );
			foreach( Cレーン cレーン in this.listレーン )
			{
				layoutRectangle.X = ( rcレーン名のPicBox内描画領域.X + cレーン.n位置Xdot ) + 2;
				layoutRectangle.Y = rcレーン名のPicBox内描画領域.Y + 2;
				layoutRectangle.Width = cレーン.n幅dot;
				layoutRectangle.Height = 0x18;
				g.DrawString( cレーン.strレーン名, this.ftレーン番号文字フォント, this.brレーン名文字ブラシ影, layoutRectangle, this.strfmtレーン名文字フォーマット );
				layoutRectangle.X -= 2;
				layoutRectangle.Y -= 2;
				g.DrawString( cレーン.strレーン名, this.ftレーン番号文字フォント, this.brレーン名文字ブラシ, layoutRectangle, this.strfmtレーン名文字フォーマット );
			}
		}
		//-----------------
		#endregion
	}
}
