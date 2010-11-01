using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using FDK;

namespace DTXMania
{
	internal class CActOptionList : CActivity
	{
		// プロパティ

		public int n現在の選択項目;
		public bool b現在選択されている項目はReturnToMenuである
		{
			get
			{
				CItemBase base2 = this.list項目リスト[ this.n現在の選択項目 ];
				if( ( ( base2 != this.iCommonReturnToMenu ) && ( base2 != this.iDrumsReturnToMenu ) ) && ( base2 != this.iGuitarReturnToMenu ) )
				{
					return ( base2 == this.iBassReturnToMenu );
				}
				return true;
			}
		}
		public CItemBase ib現在の選択項目
		{
			get
			{
				return this.list項目リスト[ this.n現在の選択項目 ];
			}
		}


		// メソッド

		public void t項目リストの設定・Bass()
		{
			this.tConfigIniへ記録する();
			this.list項目リスト.Clear();
			this.iBassReturnToMenu = new CItemBase( "<< ReturnTo Menu", CItemBase.Eパネル種別.その他 );
			this.iBassReturnToMenu.str説明文 = "左側のメニューに戻ります。";
			this.list項目リスト.Add( this.iBassReturnToMenu );
			this.iBassAutoPlay = new CItemToggle( "AutoPlay", CDTXMania.ConfigIni.bAutoPlay.Bass );
			this.iBassAutoPlay.str説明文 = "ベースパートを自動で演奏します。";
			this.list項目リスト.Add( this.iBassAutoPlay );
			this.iBassScrollSpeed = new CItemInteger( "ScrollSpeed", 0, 0x7cf, CDTXMania.ConfigIni.n譜面スクロール速度.Bass );
			this.iBassScrollSpeed.str説明文 = "演奏時のベース譜面のスクロールの\n速度を指定します。\nx0.5 ～ x1000.0 までを指定可能です。";
			this.list項目リスト.Add( this.iBassScrollSpeed );
			this.iBassSudden = new CItemToggle( "Sudden", CDTXMania.ConfigIni.bSudden.Bass );
			this.iBassSudden.str説明文 = "ベースチップがヒットバー付近にくる\nまで表示されなくなります。";
			this.list項目リスト.Add( this.iBassSudden );
			this.iBassHidden = new CItemToggle( "Hidden", CDTXMania.ConfigIni.bHidden.Bass );
			this.iBassHidden.str説明文 = "ベースチップがヒットバー付近で表示\nされなくなります。";
			this.list項目リスト.Add( this.iBassHidden );
			this.iBassReverse = new CItemToggle( "Reverse", CDTXMania.ConfigIni.bReverse.Bass );
			this.iBassReverse.str説明文 = "ベースチップが譜面の上から下に流\nれるようになります。";
			this.list項目リスト.Add( this.iBassReverse );
			this.iBassPosition = new CItemList( "Position", CItemBase.Eパネル種別.通常, (int) CDTXMania.ConfigIni.判定文字表示位置.Bass, new string[] { "P-A", "P-B", "OFF" } );
			this.iBassPosition.str説明文 = "ベースの判定文字の表示位置を指定\nします。\n  P-A: レーン上\n  P-B: COMBO の下\n  OFF: 表示しない";
			this.list項目リスト.Add( this.iBassPosition );
			this.iBassRandom = new CItemList( "Random", CItemBase.Eパネル種別.通常, (int) CDTXMania.ConfigIni.eRandom.Bass, new string[] { "OFF", "Part", "Super", "Hyper" } );
			this.iBassRandom.str説明文 = "ベースのチップがランダムに降ってき\nます。\n  Part: 小節・レーン単位で交換\n  Super: チップ単位で交換\n  Hyper: 全部完全に変更";
			this.list項目リスト.Add( this.iBassRandom );
			this.iBassLight = new CItemToggle( "Light", CDTXMania.ConfigIni.bLight.Bass );
			this.iBassLight.str説明文 = "ベースチップのないところでピッキン\nグしても BAD になりません。";
			this.list項目リスト.Add( this.iBassLight );
			this.iBassLeft = new CItemToggle( "Left", CDTXMania.ConfigIni.bLeft.Bass );
			this.iBassLeft.str説明文 = "ベースの RGB の並びが左右反転し\nます。（左利きモード）";
			this.list項目リスト.Add( this.iBassLeft );
			this.n現在の選択項目 = 0;
			this.eメニュー種別 = Eメニュー種別.Bass;
		}
		public void t項目リストの設定・Common()
		{
			this.tConfigIniへ記録する();
			this.list項目リスト.Clear();
			this.iCommonReturnToMenu = new CItemBase( "<< ReturnTo Menu", CItemBase.Eパネル種別.その他 );
			this.iCommonReturnToMenu.str説明文 = "左側のメニューに戻ります。";
			this.list項目リスト.Add( this.iCommonReturnToMenu );
			this.iCommonDark = new CItemList( "DARK", CItemBase.Eパネル種別.通常, (int) CDTXMania.ConfigIni.eDark, new string[] { "OFF", "HALF", "FULL" } );
			this.iCommonDark.str説明文 = "HALF: 背景、レーン、ゲージが表示\nされなくなります。\nFULL: さらに小節線、拍線、判定ラ\nイン、パッドも表示されなくなります。";
			this.list項目リスト.Add( this.iCommonDark );
			this.iCommonPlaySpeed = new CItemInteger( "PlaySpeed", 5, 40, CDTXMania.ConfigIni.n演奏速度 );
			this.iCommonPlaySpeed.str説明文 = "曲の演奏速度を、速くしたり遅くした\nりすることができます。\n（※一部のサウンドカードでは正しく\n 再生できない可能性があります。）";
			this.list項目リスト.Add( this.iCommonPlaySpeed );
			this.n現在の選択項目 = 0;
			this.eメニュー種別 = Eメニュー種別.Common;
		}
		public void t項目リストの設定・Drums()
		{
			this.tConfigIniへ記録する();
			this.list項目リスト.Clear();
			this.iDrumsReturnToMenu = new CItemBase( "<< ReturnTo Menu", CItemBase.Eパネル種別.その他 );
			this.iDrumsReturnToMenu.str説明文 = "左側のメニューに戻ります。";
			this.list項目リスト.Add( this.iDrumsReturnToMenu );
			this.iDrumsAutoPlayAll = new CItemThreeState( "AutoPlay (All)", CItemThreeState.E状態.不定 );
			this.iDrumsAutoPlayAll.str説明文 = "全パッドの自動演奏の ON/OFF を\nまとめて切り替えます。";
			this.list項目リスト.Add( this.iDrumsAutoPlayAll );
			this.iDrumsLeftCymbal = new CItemToggle( "    LeftCymbal", CDTXMania.ConfigIni.bAutoPlay.LC );
			this.iDrumsLeftCymbal.str説明文 = "左シンバルを自動で演奏します。";
			this.list項目リスト.Add( this.iDrumsLeftCymbal );
			this.iDrumsHiHat = new CItemToggle( "    HiHat", CDTXMania.ConfigIni.bAutoPlay.HH );
			this.iDrumsHiHat.str説明文 = "ハイハットを自動で演奏します。\n（クローズ、オープンとも）";
			this.list項目リスト.Add( this.iDrumsHiHat );
			this.iDrumsSnare = new CItemToggle( "    Snare", CDTXMania.ConfigIni.bAutoPlay.SD );
			this.iDrumsSnare.str説明文 = "スネアを自動で演奏します。";
			this.list項目リスト.Add( this.iDrumsSnare );
			this.iDrumsBass = new CItemToggle( "    BassDrum", CDTXMania.ConfigIni.bAutoPlay.BD );
			this.iDrumsBass.str説明文 = "バスドラムを自動で演奏します。";
			this.list項目リスト.Add( this.iDrumsBass );
			this.iDrumsHighTom = new CItemToggle( "    HighTom", CDTXMania.ConfigIni.bAutoPlay.HT );
			this.iDrumsHighTom.str説明文 = "ハイタムを自動で演奏します。";
			this.list項目リスト.Add( this.iDrumsHighTom );
			this.iDrumsLowTom = new CItemToggle( "    LowTom", CDTXMania.ConfigIni.bAutoPlay.LT );
			this.iDrumsLowTom.str説明文 = "ロータムを自動で演奏します。";
			this.list項目リスト.Add( this.iDrumsLowTom );
			this.iDrumsFloorTom = new CItemToggle( "    FloorTom", CDTXMania.ConfigIni.bAutoPlay.FT );
			this.iDrumsFloorTom.str説明文 = "フロアタムを自動で演奏します。";
			this.list項目リスト.Add( this.iDrumsFloorTom );
			this.iDrumsCymbalRide = new CItemToggle( "    Cym/Ride", CDTXMania.ConfigIni.bAutoPlay.CY );
			this.iDrumsCymbalRide.str説明文 = "右シンバルとライドシンバルを自動で\n演奏します。";
			this.list項目リスト.Add( this.iDrumsCymbalRide );
			this.iDrumsScrollSpeed = new CItemInteger( "ScrollSpeed", 0, 0x7cf, CDTXMania.ConfigIni.n譜面スクロール速度.Drums );
			this.iDrumsScrollSpeed.str説明文 = "演奏時のドラム譜面のスクロールの\n速度を指定します。\nx0.5 ～ x1000.0 を指定可能です。";
			this.list項目リスト.Add( this.iDrumsScrollSpeed );
			this.iDrumsComboPosition = new CItemList( "ComboPosition", CItemBase.Eパネル種別.通常, (int) CDTXMania.ConfigIni.ドラムコンボ文字の表示位置, new string[] { "Left", "Center", "Right", "OFF" } );
			this.iDrumsComboPosition.str説明文 = "演奏時のドラムコンボ文字列の位置\nを指定します。";
			this.list項目リスト.Add( this.iDrumsComboPosition );
			this.iDrumsSudden = new CItemToggle( "Sudden", CDTXMania.ConfigIni.bSudden.Drums );
			this.iDrumsSudden.str説明文 = "ドラムチップが譜面の下の方から表\n示されるようになります。";
			this.list項目リスト.Add( this.iDrumsSudden );
			this.iDrumsHidden = new CItemToggle( "Hidden", CDTXMania.ConfigIni.bHidden.Drums );
			this.iDrumsHidden.str説明文 = "ドラムチップが譜面の下の方で表示\nされなくなります。";
			this.list項目リスト.Add( this.iDrumsHidden );
			this.iDrumsReverse = new CItemToggle( "Reverse", CDTXMania.ConfigIni.bReverse.Drums );
			this.iDrumsReverse.str説明文 = "ドラムチップが譜面の下から上に流\nれるようになります。";
			this.list項目リスト.Add( this.iDrumsReverse );
			this.iDrumsPosition = new CItemList( "Position", CItemBase.Eパネル種別.通常, (int) CDTXMania.ConfigIni.判定文字表示位置.Drums, new string[] { "P-A", "P-B", "OFF" } );
			this.iDrumsPosition.str説明文 = "ドラムの判定文字の表示位置を指定\nします。\n  P-A: レーン上\n  P-B: 判定ライン下\n  OFF: 表示しない";
			this.list項目リスト.Add( this.iDrumsPosition );
			this.iDrumsTight = new CItemToggle( "Tight", CDTXMania.ConfigIni.bTight );
			this.iDrumsTight.str説明文 = "ドラムチップのないところでパッドを\n叩くとミスになります。";
			this.list項目リスト.Add( this.iDrumsTight );
			this.n現在の選択項目 = 0;
			this.eメニュー種別 = Eメニュー種別.Drums;
		}
		public void t項目リストの設定・Exit()
		{
			this.tConfigIniへ記録する();
			this.eメニュー種別 = Eメニュー種別.Unknown;
		}
		public void t項目リストの設定・Guitar()
		{
			this.tConfigIniへ記録する();
			this.list項目リスト.Clear();
			this.iGuitarReturnToMenu = new CItemBase( "<< ReturnTo Menu", CItemBase.Eパネル種別.その他 );
			this.iGuitarReturnToMenu.str説明文 = "左側のメニューに戻ります。";
			this.list項目リスト.Add( this.iGuitarReturnToMenu );
			this.iGuitarAutoPlay = new CItemToggle( "AutoPlay", CDTXMania.ConfigIni.bAutoPlay.Guitar );
			this.iGuitarAutoPlay.str説明文 = "ギターパートを自動で演奏します。";
			this.list項目リスト.Add( this.iGuitarAutoPlay );
			this.iGuitarScrollSpeed = new CItemInteger( "ScrollSpeed", 0, 0x7cf, CDTXMania.ConfigIni.n譜面スクロール速度.Guitar );
			this.iGuitarScrollSpeed.str説明文 = "演奏時のギター譜面のスクロールの\n速度を指定します。\nx0.5 ～ x1000.0 までを指定可能です。";
			this.list項目リスト.Add( this.iGuitarScrollSpeed );
			this.iGuitarSudden = new CItemToggle( "Sudden", CDTXMania.ConfigIni.bSudden.Guitar );
			this.iGuitarSudden.str説明文 = "ギターチップがヒットバー付近にくる\nまで表示されなくなります。";
			this.list項目リスト.Add( this.iGuitarSudden );
			this.iGuitarHidden = new CItemToggle( "Hidden", CDTXMania.ConfigIni.bHidden.Guitar );
			this.iGuitarHidden.str説明文 = "ギターチップがヒットバー付近で表示\nされなくなります。";
			this.list項目リスト.Add( this.iGuitarHidden );
			this.iGuitarReverse = new CItemToggle( "Reverse", CDTXMania.ConfigIni.bReverse.Guitar );
			this.iGuitarReverse.str説明文 = "ギターチップが譜面の上から下に流\nれるようになります。";
			this.list項目リスト.Add( this.iGuitarReverse );
			this.iGuitarPosition = new CItemList( "Position", CItemBase.Eパネル種別.通常, (int) CDTXMania.ConfigIni.判定文字表示位置.Guitar, new string[] { "P-A", "P-B", "OFF" } );
			this.iGuitarPosition.str説明文 = "ギターの判定文字の表示位置を指定\nします。\n  P-A: レーン上\n  P-B: COMBO の下\n  OFF: 表示しない";
			this.list項目リスト.Add( this.iGuitarPosition );
			this.iGuitarRandom = new CItemList( "Random", CItemBase.Eパネル種別.通常, (int) CDTXMania.ConfigIni.eRandom.Guitar, new string[] { "OFF", "Part", "Super", "Hyper" } );
			this.iGuitarRandom.str説明文 = "ギターのチップがランダムに降ってき\nます。\n  Part: 小節・レーン単位で交換\n  Super: チップ単位で交換\n  Hyper: 全部完全に変更";
			this.list項目リスト.Add( this.iGuitarRandom );
			this.iGuitarLight = new CItemToggle( "Light", CDTXMania.ConfigIni.bLight.Guitar );
			this.iGuitarLight.str説明文 = "ギターチップのないところでピッキン\nグしても BAD になりません。";
			this.list項目リスト.Add( this.iGuitarLight );
			this.iGuitarLeft = new CItemToggle( "Left", CDTXMania.ConfigIni.bLeft.Guitar );
			this.iGuitarLeft.str説明文 = "ギターの RGB の並びが左右反転し\nます。（左利きモード）";
			this.list項目リスト.Add( this.iGuitarLeft );
			this.n現在の選択項目 = 0;
			this.eメニュー種別 = Eメニュー種別.Guitar;
		}
		public void tEnter押下()
		{
			if( this.b要素値にフォーカス中 )
			{
				this.b要素値にフォーカス中 = false;
			}
			else if( this.list項目リスト[ this.n現在の選択項目 ].e種別 == CItemBase.E種別.整数 )
			{
				this.b要素値にフォーカス中 = true;
			}
			else if( this.b現在選択されている項目はReturnToMenuである )
			{
				this.tConfigIniへ記録する();
			}
			else
			{
				this.list項目リスト[ this.n現在の選択項目 ].tEnter押下();
				if( this.list項目リスト[ this.n現在の選択項目 ] == this.iDrumsAutoPlayAll )
				{
					this.t全部のドラムパッドのAutoを切り替える( this.iDrumsAutoPlayAll.e現在の状態 == CItemThreeState.E状態.ON );
				}
			}
		}
		public void t次に移動()
		{
			if( this.b要素値にフォーカス中 )
			{
				this.list項目リスト[ this.n現在の選択項目 ].t項目値を前へ移動();
			}
			else
			{
				this.n目標のスクロールカウンタ += 100;
			}
		}
		public void t前に移動()
		{
			if( this.b要素値にフォーカス中 )
			{
				this.list項目リスト[ this.n現在の選択項目 ].t項目値を次へ移動();
			}
			else
			{
				this.n目標のスクロールカウンタ -= 100;
			}
		}

		
		// CActivity 実装

		public override void On活性化()
		{
			this.list項目リスト = new List<CItemBase>();
			this.eメニュー種別 = Eメニュー種別.Unknown;
			this.t項目リストの設定・Common();
			this.b要素値にフォーカス中 = false;
			this.n目標のスクロールカウンタ = 0;
			this.n現在のスクロールカウンタ = 0;
			this.nスクロール用タイマ値 = -1;
			this.ct三角矢印アニメ = new CCounter();
			base.On活性化();
		}
		public override void On非活性化()
		{
			this.tConfigIniへ記録する();
			this.list項目リスト.Clear();
			this.ct三角矢印アニメ = null;
			base.On非活性化();
		}
		public override void OnManagedリソースの作成()
		{
			if( !base.b活性化してない )
			{
				this.tx通常項目行パネル = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenOption itembox.png" ), false );
				this.txその他項目行パネル = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenOption itembox other.png" ), false );
				this.tx三角矢印 = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\ScreenOption triangle arrow.png" ), false );
				base.OnManagedリソースの作成();
			}
		}
		public override void OnManagedリソースの解放()
		{
			if( !base.b活性化してない )
			{
				CDTXMania.tテクスチャの解放( ref this.tx通常項目行パネル );
				CDTXMania.tテクスチャの解放( ref this.txその他項目行パネル );
				CDTXMania.tテクスチャの解放( ref this.tx三角矢印 );
				base.OnManagedリソースの解放();
			}
		}
		public override int On進行描画()
		{
			throw new InvalidOperationException( "t進行描画(bool)のほうを使用してください。" );
		}
		public int t進行描画( bool b項目リスト側にフォーカスがある )
		{
			if( !base.b活性化してない )
			{
				if( base.b初めての進行描画 )
				{
					this.nスクロール用タイマ値 = CDTXMania.Timer.n現在時刻;
					this.ct三角矢印アニメ.t開始( 0, 9, 50, CDTXMania.Timer );
					base.b初めての進行描画 = false;
				}
				this.b項目リスト側にフォーカスがある = b項目リスト側にフォーカスがある;
				long num = CDTXMania.Timer.n現在時刻;
				if( num < this.nスクロール用タイマ値 )
				{
					this.nスクロール用タイマ値 = num;
				}
				while( ( num - this.nスクロール用タイマ値 ) >= 2 )
				{
					int num2 = Math.Abs( (int) ( this.n目標のスクロールカウンタ - this.n現在のスクロールカウンタ ) );
					int num3 = 0;
					if( num2 <= 100 )
					{
						num3 = 2;
					}
					else if( num2 <= 300 )
					{
						num3 = 3;
					}
					else if( num2 <= 500 )
					{
						num3 = 4;
					}
					else
					{
						num3 = 8;
					}
					if( this.n現在のスクロールカウンタ < this.n目標のスクロールカウンタ )
					{
						this.n現在のスクロールカウンタ += num3;
						if( this.n現在のスクロールカウンタ > this.n目標のスクロールカウンタ )
						{
							this.n現在のスクロールカウンタ = this.n目標のスクロールカウンタ;
						}
					}
					else if( this.n現在のスクロールカウンタ > this.n目標のスクロールカウンタ )
					{
						this.n現在のスクロールカウンタ -= num3;
						if( this.n現在のスクロールカウンタ < this.n目標のスクロールカウンタ )
						{
							this.n現在のスクロールカウンタ = this.n目標のスクロールカウンタ;
						}
					}
					if( this.n現在のスクロールカウンタ >= 100 )
					{
						this.n現在の選択項目 = this.t次の項目( this.n現在の選択項目 );
						this.n現在のスクロールカウンタ -= 100;
						this.n目標のスクロールカウンタ -= 100;
						if( this.n目標のスクロールカウンタ == 0 )
						{
							CDTXMania.stageオプション.t項目変更通知();
						}
					}
					else if( this.n現在のスクロールカウンタ <= -100 )
					{
						this.n現在の選択項目 = this.t前の項目( this.n現在の選択項目 );
						this.n現在のスクロールカウンタ += 100;
						this.n目標のスクロールカウンタ += 100;
						if( this.n目標のスクロールカウンタ == 0 )
						{
							CDTXMania.stageオプション.t項目変更通知();
						}
					}
					this.nスクロール用タイマ値 += 2;
				}
				if( this.b項目リスト側にフォーカスがある && ( this.n目標のスクロールカウンタ == 0 ) )
				{
					this.ct三角矢印アニメ.t進行Loop();
				}
				this.ptパネルの基本座標[ 4 ].X = this.b項目リスト側にフォーカスがある ? 0x114 : 0x12d;
				int nItem = this.n現在の選択項目;
				for( int i = 0; i < 4; i++ )
				{
					nItem = this.t前の項目( nItem );
				}
				for( int j = -4; j < 6; j++ )
				{
					if( ( ( j == -4 ) && ( this.n現在のスクロールカウンタ > 0 ) ) || ( ( j == 5 ) && ( this.n現在のスクロールカウンタ < 0 ) ) )
					{
						nItem = this.t次の項目( nItem );
						continue;
					}
					int index = j + 4;
					int num8 = ( this.n現在のスクロールカウンタ <= 0 ) ? ( ( ( j + 4 ) + 1 ) % 10 ) : ( ( ( ( j + 4 ) - 1 ) + 10 ) % 10 );
					int x = this.ptパネルの基本座標[ index ].X + ( (int) ( ( this.ptパネルの基本座標[ num8 ].X - this.ptパネルの基本座標[ index ].X ) * ( ( (double) Math.Abs( this.n現在のスクロールカウンタ ) ) / 100.0 ) ) );
					int y = this.ptパネルの基本座標[ index ].Y + ( (int) ( ( this.ptパネルの基本座標[ num8 ].Y - this.ptパネルの基本座標[ index ].Y ) * ( ( (double) Math.Abs( this.n現在のスクロールカウンタ ) ) / 100.0 ) ) );
					switch( this.list項目リスト[ nItem ].eパネル種別 )
					{
						case CItemBase.Eパネル種別.通常:
							if( this.tx通常項目行パネル != null )
							{
								this.tx通常項目行パネル.t2D描画( CDTXMania.app.Device, x, y );
							}
							break;

						case CItemBase.Eパネル種別.その他:
							if( this.txその他項目行パネル != null )
							{
								this.txその他項目行パネル.t2D描画( CDTXMania.app.Device, x, y );
							}
							break;
					}
					CDTXMania.stageオプション.actFont.t文字列描画( x + 0x12, y + 12, this.list項目リスト[ nItem ].str項目名 );
					switch( this.list項目リスト[ nItem ].e種別 )
					{
						case CItemBase.E種別.ONorOFFトグル:
							CDTXMania.stageオプション.actFont.t文字列描画( x + 210, y + 12, ( (CItemToggle) this.list項目リスト[ nItem ] ).bON ? "ON" : "OFF" );
							break;

						case CItemBase.E種別.ONorOFFor不定スリーステート:
							switch( ( (CItemThreeState) this.list項目リスト[ nItem ] ).e現在の状態 )
							{
								case CItemThreeState.E状態.ON:
									CDTXMania.stageオプション.actFont.t文字列描画( x + 210, y + 12, "ON" );
									break;

								case CItemThreeState.E状態.不定:
									CDTXMania.stageオプション.actFont.t文字列描画( x + 210, y + 12, "- -" );
									break;

								default:
									CDTXMania.stageオプション.actFont.t文字列描画( x + 210, y + 12, "OFF" );
									break;
							}
							break;

						case CItemBase.E種別.整数:
							{
								if( this.list項目リスト[ nItem ] != this.iCommonPlaySpeed )
								{
									if( ( ( this.list項目リスト[ nItem ] == this.iDrumsScrollSpeed ) || ( this.list項目リスト[ nItem ] == this.iGuitarScrollSpeed ) ) || ( this.list項目リスト[ nItem ] == this.iBassScrollSpeed ) )
									{
										float num15 = ( ( (CItemInteger) this.list項目リスト[ nItem ] ).n現在の値 + 1 ) * 0.5f;
										CDTXMania.stageオプション.actFont.t文字列描画( x + 210, y + 12, num15.ToString( "x0.0" ), ( j == 0 ) && this.b要素値にフォーカス中 );
									}
									else
									{
										CDTXMania.stageオプション.actFont.t文字列描画( x + 210, y + 12, ( (CItemInteger) this.list項目リスト[ nItem ] ).n現在の値.ToString(), ( j == 0 ) && this.b要素値にフォーカス中 );
									}
									break;
								}
								double num14 = ( (double) ( (CItemInteger) this.list項目リスト[ nItem ] ).n現在の値 ) / 20.0;
								CDTXMania.stageオプション.actFont.t文字列描画( x + 210, y + 12, num14.ToString( "0.000" ), ( j == 0 ) && this.b要素値にフォーカス中 );
								break;
							}
						case CItemBase.E種別.リスト:
							{
								CItemList list = (CItemList) this.list項目リスト[ nItem ];
								CDTXMania.stageオプション.actFont.t文字列描画( x + 210, y + 12, list.list項目値[ list.n現在選択されている項目番号 ] );
								break;
							}
					}
					nItem = this.t次の項目( nItem );
				}
				if( this.b項目リスト側にフォーカスがある && ( this.n目標のスクロールカウンタ == 0 ) )
				{
					int num11;
					int num12;
					int num13;
					if( !this.b要素値にフォーカス中 )
					{
						num11 = 0x114;
						num12 = 0xba - this.ct三角矢印アニメ.n現在の値;
						num13 = 0xfe + this.ct三角矢印アニメ.n現在の値;
					}
					else
					{
						num11 = 0x210;
						num12 = 0xc6 - this.ct三角矢印アニメ.n現在の値;
						num13 = 0xf2 + this.ct三角矢印アニメ.n現在の値;
					}
					if( this.tx三角矢印 != null )
					{
						this.tx三角矢印.t2D描画( CDTXMania.app.Device, num11, num12, new Rectangle( 0, 0, 0x20, 0x10 ) );
						this.tx三角矢印.t2D描画( CDTXMania.app.Device, num11, num13, new Rectangle( 0, 0x10, 0x20, 0x10 ) );
					}
				}
			}
			return 0;
		}


		// その他

		#region [ private ]
		//-----------------
		private enum Eメニュー種別
		{
			Common,
			Drums,
			Guitar,
			Bass,
			Unknown
		}

		private bool b項目リスト側にフォーカスがある;
		private bool b要素値にフォーカス中;
		private CCounter ct三角矢印アニメ;
		private Eメニュー種別 eメニュー種別;
		private CItemToggle iBassAutoPlay;
		private CItemToggle iBassHidden;
		private CItemToggle iBassLeft;
		private CItemToggle iBassLight;
		private CItemList iBassPosition;
		private CItemList iBassRandom;
		private CItemBase iBassReturnToMenu;
		private CItemToggle iBassReverse;
		private CItemInteger iBassScrollSpeed;
		private CItemToggle iBassSudden;
		private CItemList iCommonDark;
		private CItemInteger iCommonPlaySpeed;
		private CItemBase iCommonReturnToMenu;
		private CItemThreeState iDrumsAutoPlayAll;
		private CItemToggle iDrumsBass;
		private CItemList iDrumsComboPosition;
		private CItemToggle iDrumsCymbalRide;
		private CItemToggle iDrumsFloorTom;
		private CItemToggle iDrumsHidden;
		private CItemToggle iDrumsHighTom;
		private CItemToggle iDrumsHiHat;
		private CItemToggle iDrumsLeftCymbal;
		private CItemToggle iDrumsLowTom;
		private CItemList iDrumsPosition;
		private CItemBase iDrumsReturnToMenu;
		private CItemToggle iDrumsReverse;
		private CItemInteger iDrumsScrollSpeed;
		private CItemToggle iDrumsSnare;
		private CItemToggle iDrumsSudden;
		private CItemToggle iDrumsTight;
		private CItemToggle iGuitarAutoPlay;
		private CItemToggle iGuitarHidden;
		private CItemToggle iGuitarLeft;
		private CItemToggle iGuitarLight;
		private CItemList iGuitarPosition;
		private CItemList iGuitarRandom;
		private CItemBase iGuitarReturnToMenu;
		private CItemToggle iGuitarReverse;
		private CItemInteger iGuitarScrollSpeed;
		private CItemToggle iGuitarSudden;
		private List<CItemBase> list項目リスト;
		private long nスクロール用タイマ値;
		private int n現在のスクロールカウンタ;
		private int n目標のスクロールカウンタ;
		private Point[] ptパネルの基本座標 = new Point[] { new Point( 0x12d, 3 ), new Point( 0x12d, 0x35 ), new Point( 0x12d, 0x67 ), new Point( 0x12d, 0x99 ), new Point( 0x114, 0xcb ), new Point( 0x12d, 0xfd ), new Point( 0x12d, 0x12f ), new Point( 0x12d, 0x161 ), new Point( 0x12d, 0x193 ), new Point( 0x12d, 0x1c5 ) };
		private CTexture txその他項目行パネル;
		private CTexture tx三角矢印;
		private CTexture tx通常項目行パネル;

		private int t前の項目( int nItem )
		{
			if( --nItem < 0 )
			{
				nItem = this.list項目リスト.Count - 1;
			}
			return nItem;
		}
		private int t次の項目( int nItem )
		{
			if( ++nItem >= this.list項目リスト.Count )
			{
				nItem = 0;
			}
			return nItem;
		}
		private void t全部のドラムパッドのAutoを切り替える( bool bAutoON )
		{
			this.iDrumsLeftCymbal.bON = this.iDrumsHiHat.bON = this.iDrumsSnare.bON = this.iDrumsBass.bON = this.iDrumsHighTom.bON = this.iDrumsLowTom.bON = this.iDrumsFloorTom.bON = this.iDrumsCymbalRide.bON = bAutoON;
		}
		private void tConfigIniへ記録する()
		{
			switch( this.eメニュー種別 )
			{
				case Eメニュー種別.Common:
					this.tConfigIniへ記録する・Common();
					return;

				case Eメニュー種別.Drums:
					this.tConfigIniへ記録する・Drums();
					return;

				case Eメニュー種別.Guitar:
					this.tConfigIniへ記録する・Guitar();
					return;

				case Eメニュー種別.Bass:
					this.tConfigIniへ記録する・Bass();
					return;
			}
		}
		private void tConfigIniへ記録する・Bass()
		{
			CDTXMania.ConfigIni.bAutoPlay.Bass = this.iBassAutoPlay.bON;
			CDTXMania.ConfigIni.n譜面スクロール速度.Bass = this.iBassScrollSpeed.n現在の値;
			CDTXMania.ConfigIni.bSudden.Bass = this.iBassSudden.bON;
			CDTXMania.ConfigIni.bHidden.Bass = this.iBassHidden.bON;
			CDTXMania.ConfigIni.bReverse.Bass = this.iBassReverse.bON;
			CDTXMania.ConfigIni.判定文字表示位置.Bass = (E判定文字表示位置) this.iBassPosition.n現在選択されている項目番号;
			CDTXMania.ConfigIni.eRandom.Bass = (Eランダムモード) this.iBassRandom.n現在選択されている項目番号;
			CDTXMania.ConfigIni.bLight.Bass = this.iBassLight.bON;
			CDTXMania.ConfigIni.bLeft.Bass = this.iBassLeft.bON;
		}
		private void tConfigIniへ記録する・Common()
		{
			CDTXMania.ConfigIni.eDark = (Eダークモード) this.iCommonDark.n現在選択されている項目番号;
			CDTXMania.ConfigIni.n演奏速度 = this.iCommonPlaySpeed.n現在の値;
		}
		private void tConfigIniへ記録する・Drums()
		{
			CDTXMania.ConfigIni.bAutoPlay.LC = this.iDrumsLeftCymbal.bON;
			CDTXMania.ConfigIni.bAutoPlay.HH = this.iDrumsHiHat.bON;
			CDTXMania.ConfigIni.bAutoPlay.SD = this.iDrumsSnare.bON;
			CDTXMania.ConfigIni.bAutoPlay.BD = this.iDrumsBass.bON;
			CDTXMania.ConfigIni.bAutoPlay.HT = this.iDrumsHighTom.bON;
			CDTXMania.ConfigIni.bAutoPlay.LT = this.iDrumsLowTom.bON;
			CDTXMania.ConfigIni.bAutoPlay.FT = this.iDrumsFloorTom.bON;
			CDTXMania.ConfigIni.bAutoPlay.CY = this.iDrumsCymbalRide.bON;
			CDTXMania.ConfigIni.n譜面スクロール速度.Drums = this.iDrumsScrollSpeed.n現在の値;
			CDTXMania.ConfigIni.ドラムコンボ文字の表示位置 = (Eドラムコンボ文字の表示位置) this.iDrumsComboPosition.n現在選択されている項目番号;
			CDTXMania.ConfigIni.bSudden.Drums = this.iDrumsSudden.bON;
			CDTXMania.ConfigIni.bHidden.Drums = this.iDrumsHidden.bON;
			CDTXMania.ConfigIni.bReverse.Drums = this.iDrumsReverse.bON;
			CDTXMania.ConfigIni.判定文字表示位置.Drums = (E判定文字表示位置) this.iDrumsPosition.n現在選択されている項目番号;
			CDTXMania.ConfigIni.bTight = this.iDrumsTight.bON;
		}
		private void tConfigIniへ記録する・Guitar()
		{
			CDTXMania.ConfigIni.bAutoPlay.Guitar = this.iGuitarAutoPlay.bON;
			CDTXMania.ConfigIni.n譜面スクロール速度.Guitar = this.iGuitarScrollSpeed.n現在の値;
			CDTXMania.ConfigIni.bSudden.Guitar = this.iGuitarSudden.bON;
			CDTXMania.ConfigIni.bHidden.Guitar = this.iGuitarHidden.bON;
			CDTXMania.ConfigIni.bReverse.Guitar = this.iGuitarReverse.bON;
			CDTXMania.ConfigIni.判定文字表示位置.Guitar = (E判定文字表示位置) this.iGuitarPosition.n現在選択されている項目番号;
			CDTXMania.ConfigIni.eRandom.Guitar = (Eランダムモード) this.iGuitarRandom.n現在選択されている項目番号;
			CDTXMania.ConfigIni.bLight.Guitar = this.iGuitarLight.bON;
			CDTXMania.ConfigIni.bLeft.Guitar = this.iGuitarLeft.bON;
		}
		//-----------------
		#endregion
	}
}
