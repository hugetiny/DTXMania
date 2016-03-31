using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.IO;
using System.Drawing;
using System.Diagnostics;
using DTXCreator.譜面;
using DTXCreator.WAV_BMP_AVI;
using DTXCreator.Properties;

namespace DTXCreator.MIDIインポート
{
    public partial class CMIDIインポートダイアログ : Form
    {

        private CMIDI cMIDI;
        public Cメインフォーム formメインフォーム;
		
        public CMIDIインポートダイアログ()
        {
            InitializeComponent();
        }

        private void CMIDIインポートダイアログ_KeyDown(object sender, KeyEventArgs e)
        {
            if ( e.KeyCode == Keys.Return )
            {
                this.buttonOK.PerformClick();
            }
            else if ( e.KeyCode == Keys.Escape )
            {
                this.buttonCancel.PerformClick();
            }
        }

		private void numericUpDownCh_ValueChanged(object sender, EventArgs e)
		{
			if ( cMIDI != null ) this.tMIDIファイルを開く( cMIDI.strファイル名 );
		}

        private void buttonOpen_Click(object sender, EventArgs e)
        {
            this.tMIDIファイルを選択する();
        }

        public void tMIDI割り当て一覧を作成する()
        {
            // レーン一覧を作成
            this.DTX_Lane.Items.AddRange( "* Disuse *" );
            foreach ( Cレーン cレーン in this.formメインフォーム.mgr譜面管理者.listレーン )
            {
				if ( cレーン.eレーン種別 == Cレーン.E種別.WAV ) this.DTX_Lane.Items.AddRange( cレーン.strレーン名 );
            }
            
            // MIDIキー一覧を作成
            for ( int i = 127; i >= 0; i-- )
            {
                string str楽器名 = "";
                string[] strキー名 = new string[] { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B", };
                string strレーン名 = "* Disuse *";
				bool b裏チャンネル = false;
                switch ( i )
                {
                    case 35 : str楽器名 = "Bass Drum 2"; strレーン名 = "BD"; break;
                    case 36 : str楽器名 = "Bass Drum 1"; strレーン名 = "BD"; break;
                    case 37 : str楽器名 = "Side Stick"; strレーン名 = "SE1"; break;
                    case 38 : str楽器名 = "Snare Drum 1"; strレーン名 = "SD"; break;
                    case 39 : str楽器名 = "Hand Clap"; strレーン名 = "SE1"; break;
                    case 40 : str楽器名 = "Snare Drum 2"; strレーン名 = "SD"; break;
                    case 41 : str楽器名 = "Low Tom 2"; strレーン名 = "FT"; break;
                    case 42 : str楽器名 = "Closed Hi-hat"; strレーン名 = "HH"; break;
                    case 43 : str楽器名 = "Low Tom 1"; strレーン名 = "FT"; break;
                    case 44 : str楽器名 = "Pedal Hi-hat"; strレーン名 = "SE2"; break;
                    case 45 : str楽器名 = "Mid Tom 2"; strレーン名 = "LT"; break;
                    case 46 : str楽器名 = "Open Hi-hat"; strレーン名 = "HH"; b裏チャンネル = true; break;
                    case 47 : str楽器名 = "Mid Tom 1"; strレーン名 = "LT"; break;
                    case 48 : str楽器名 = "High Tom 2"; strレーン名 = "HT"; break;
                    case 49 : str楽器名 = "Crash Cymbal 1"; strレーン名 = "CY"; break;
                    case 50 : str楽器名 = "High Tom 1"; strレーン名 = "HT"; break;
                    case 51 : str楽器名 = "Ride Cymbal 1"; strレーン名 = "CY"; b裏チャンネル = true; break;
                    case 52 : str楽器名 = "Chinese Cymbal"; strレーン名 = "CY"; break;
                    case 53 : str楽器名 = "Ride Bell"; strレーン名 = "CY"; b裏チャンネル = true; break;
                    case 54 : str楽器名 = "Tambourine"; strレーン名 = "SE1"; break;
                    case 55 : str楽器名 = "Splash Cymbal"; strレーン名 = "LC"; break;
                    case 56 : str楽器名 = "Cowbell"; strレーン名 = "SE1"; break;
                    case 57 : str楽器名 = "Crash Cymbal 2"; strレーン名 = "LC"; break;
                    case 58 : str楽器名 = "Vibra Slap"; strレーン名 = "SE1"; break;
                    case 59 : str楽器名 = "Ride Cymbal 2"; strレーン名 = "CY"; b裏チャンネル = true; break;
                    case 60 : str楽器名 = "High Bongo"; break;
                    case 61 : str楽器名 = "Low Bongo"; break;
                    case 62 : str楽器名 = "Mute High Conga"; break;
                    case 63 : str楽器名 = "Open High Conga"; break;
                    case 64 : str楽器名 = "Low Conga"; break;
                    case 65 : str楽器名 = "High Timbale"; break;
                    case 66 : str楽器名 = "Low Timbale"; break;
                    case 67 : str楽器名 = "High Agogo"; break;
                    case 68 : str楽器名 = "Low Agogo"; break;
                    case 69 : str楽器名 = "Cabasa"; break;
                    case 70 : str楽器名 = "Maracas"; break;
                    case 71 : str楽器名 = "Short Whistle"; break;
                    case 72 : str楽器名 = "Long Whistle"; break;
                    case 73 : str楽器名 = "Short Guiro"; break;
                    case 74 : str楽器名 = "Long Guiro"; break;
                    case 75 : str楽器名 = "Claves"; break;
                    case 76 : str楽器名 = "High Wood Block"; break;
                    case 77 : str楽器名 = "Low Wood Block"; break;
                    case 78 : str楽器名 = "Mute Cuica"; break;
                    case 79 : str楽器名 = "Open Cuica"; break;
                    case 80 : str楽器名 = "Mute Triangle"; break;
                    case 81 : str楽器名 = "Open Triangle"; break;
                }
                this.dataGridView1.Rows.Add( i, strキー名[i%12], strレーン名, b裏チャンネル, 0, str楽器名 );
                if ( i%12 == 1 || i%12 == 3 || i%12 == 6 || i%12 == 8 || i%12 == 10 ) this.dataGridView1.Rows[127-i].DefaultCellStyle.BackColor = Color.FromArgb( 240, 248, 255 );
                if ( i%12 == 0 ) this.dataGridView1.Rows[127-i].DefaultCellStyle.BackColor = Color.FromArgb( 255, 224, 224 );
                tMIDI割り当て一覧のレーン名の背景色を変更する( this.dataGridView1.RowCount-1 );

            }
            this.dataGridView1.Columns["MIDI_Key"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dataGridView1.Columns["DTX_Lane"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dataGridView1.Columns["DTX_Lane"].DefaultCellStyle.Font = new Font( "meiryo", 8f, FontStyle.Bold );
            this.dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            this.dataGridView1.FirstDisplayedScrollingRowIndex = 80;

        }

        public void tMIDIファイルを選択する()
        {
            #region [ ファイル選択 ]
            //-----------------
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "MIDIファイルを選択";
            dialog.Filter = "MIDIファイル (*.mid,*.midi)|*.mid;*.midi|すべてのファイル (*.*)|*.*";
            dialog.FilterIndex = 1;
            dialog.InitialDirectory = this.formメインフォーム.str作業フォルダ名;
            DialogResult result = dialog.ShowDialog();

            if (result != DialogResult.OK)
                return;
            //-----------------
            #endregion

            this.tMIDIファイルを開く( dialog.FileName );
        }

        public void tMIDIファイルを開く( string strファイル名 )
        {
            #region [ ファイル確認 ]
            //-----------------
            if ( !File.Exists( strファイル名 ) )
            {
                MessageBox.Show(
                    "ファイルが見つかりません。",
                    "MIDIインポート",
                    MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
                return;
            }
            //-----------------
            #endregion

            #region [ 拡張子 ]
            //-----------------
            string str拡張子 = Path.GetExtension(strファイル名);

            if ( !str拡張子.Equals(".mid", StringComparison.OrdinalIgnoreCase ) && !str拡張子.Equals( ".midi", StringComparison.OrdinalIgnoreCase) )
            {
                MessageBox.Show(
                    "MIDIファイルではありません。",
                    "MIDIインポート",
                    MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
                return;
            }
            //-----------------
            #endregion

            #region [ 各設定 ]
            //-----------------
			this.formメインフォーム.str作業フォルダ名 = Path.GetDirectoryName( strファイル名 ) + @"\";
            //-----------------
            #endregion

            #region [ MIDIファイル解析 ]
            //-----------------
            cMIDI = new CMIDI( strファイル名 );
            cMIDI.formメインフォーム = this.formメインフォーム;
			cMIDI.n読み込みCh = (int)this.numericUpDownCh.Value;
            cMIDI.tMIDIを解析する();
			cMIDI.tMIDIチップをレーンに割り当てる( this.dataGridView1 );
			this.label3.Text = "重複チップ : " + cMIDI.nMIDI重複チップ数を返す();
            
            // ヘッダがMIDI以外なら中断
            if ( !cMIDI.bMIDIファイル )
            {
                MessageBox.Show(
                    "MIDIファイルではありません。",
                    "MIDIインポート",
                    MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
				cMIDI = null;
                return;
            }
            //-----------------
            #endregion
            
            #region [ 解析結果を出力・処理 ]
            //-----------------
            // 各キーのノート数を表に出力する
            for ( int i = 0 ; i < 128 ; i++ )
            {
                this.dataGridView1.Rows[127-i].Cells["Notes"].Value = cMIDI.nドラム各ノート数[i];
            }
			
            // MIDI解析内容をテキストボックスに出力する
            string str文字列 = "";
            str文字列 += "File:" + strファイル名 + "\r\n";
            str文字列 += "BPM:" + cMIDI.f先頭BPM + "\r\n";
            str文字列 += "TimeBase:" + cMIDI.n分解能 + "\r\n";
            str文字列 += "\r\n";
            
            foreach ( CMIDIトラック value in cMIDI.lMIDIトラック )
            {
                str文字列 += "Track " + value.nトラック数;
                str文字列 += " / Channel " + value.nチャンネル;
                str文字列 += " / Name " + value.strトラック名 + "\r\n";
                str文字列 += value.str解析内容 + "\r\n";
            }
            
            this.textBox1.Text = str文字列;
            //-----------------
            #endregion

        }
        
        // レーン名をワンクリックで開く用
        private void dataGridView1_CellEnter( object sender, DataGridViewCellEventArgs e )
        {
            DataGridView dgv = (DataGridView) sender;

            if ( dgv.Columns[e.ColumnIndex].Name == "DTX_Lane" && dgv.Columns[e.ColumnIndex] is DataGridViewComboBoxColumn )
            {
                SendKeys.Send("{F4}");
            }
        }

		// レーン名を変更したら
        private void dataGridView1_CellEndEdit( object sender, DataGridViewCellEventArgs e )
        {
            DataGridView dgv = (DataGridView) sender;

            if ( dgv.Columns[e.ColumnIndex].Name == "DTX_Lane" )
            {
                tMIDI割り当て一覧のレーン名の背景色を変更する( e.RowIndex );
            }
			
            if ( cMIDI != null )
			{
				cMIDI.tMIDIチップをレーンに割り当てる( this.dataGridView1 );
				this.label3.Text = "重複チップ : " + cMIDI.nMIDI重複チップ数を返す();
			}

        }

        private void tMIDI割り当て一覧のレーン名の背景色を変更する( int RowIndex )
        {
			string strレーン名 = (string)this.dataGridView1.Rows[RowIndex].Cells["DTX_Lane"].Value;
            int nレーン番号 = this.formメインフォーム.mgr譜面管理者.nレーン名に対応するレーン番号を返す( strレーン名 );
            if ( nレーン番号 > 1 )
            {
                Color color = this.formメインフォーム.mgr譜面管理者.listレーン[nレーン番号].col背景色;
                color = Color.FromArgb( color.R/2+128, color.G/2+128, color.B/2+128 );
                this.dataGridView1.Rows[RowIndex].Cells["DTX_Lane"].Style.BackColor = color;
            }
			else if ( strレーン名 == "* Disuse *" )
			{
                Color color = Color.FromArgb( 128, 128, 128 );
                this.dataGridView1.Rows[RowIndex].Cells["DTX_Lane"].Style.BackColor = color;
			}
        }

        public void tMIDIインポート結果を反映する()
        {
            if ( cMIDI != null && cMIDI.lチップ.Count > 0 )
            {
				// チップリストで、ベロシティをDTX向けに調整する
				foreach ( CMIDIイベント vMIDIチップ in cMIDI.lチップ )
				{
					int velo = vMIDIチップ.nベロシティ;
					if ( this.checkBox2.Checked ) velo = (int)(velo / 1.27);//127を最大値
					if ( this.checkBox1.Checked ) velo = (int)( Math.Pow( velo, 1.5 ) / Math.Pow( 100, 0.5 ) );//ベロシティカーブ
					velo = ( velo / (int)this.numericUpDown1.Value ) * (int)this.numericUpDown1.Value;
					velo = ( velo > 100 ) ? 100 : ( ( velo == 0 ) ? 1 : velo );
					vMIDIチップ.nベロシティ_DTX変換後 = velo;
				}

				// 配置予定チップをレーン指定に沿って割り当てる
				cMIDI.tMIDIチップをレーンに割り当てる( this.dataGridView1 );
				
				// #WAV02を仮BGM枠に
				CWAV cwav = this.formメインフォーム.mgrWAVリスト管理者.tWAVをキャッシュから検索して返す_なければ新規生成する( 2 );
				cwav.strラベル名 = "BGM";
				cwav.bBGMとして使用 = true;
				cwav.col背景色 = Color.FromArgb( 224, 255, 224 );
				
				// #WAV04以降を変換枠に
				// WAVリストをソートして見やすくする
				cMIDI.lMIDIWAV.Sort( nMIDIWAVSort );

				int nWAVCount = 4;
				int nレーン番号before = 0;
				
				foreach ( CMIDIイベント vチップWAV in cMIDI.lMIDIWAV )
				{
					if ( nWAVCount > 4 && nレーン番号before != vチップWAV.nレーン番号 ) nWAVCount++;
					if ( vチップWAV.eイベントタイプ != CMIDIイベント.Eイベントタイプ.NoteOnOff ) continue; // ノートチップ以外をWAVリストに表示させない

					nレーン番号before = vチップWAV.nレーン番号;

					cwav = this.formメインフォーム.mgrWAVリスト管理者.tWAVをキャッシュから検索して返す_なければ新規生成する( nWAVCount );
					cwav.strファイル名 = vチップWAV.nキー.ToString() + ".ogg";

					cwav.n音量0to100 = vチップWAV.nベロシティ_DTX変換後;
					cwav.strラベル名 = this.formメインフォーム.mgr譜面管理者.listレーン[vチップWAV.nレーン番号].strレーン名 + " " +vチップWAV.strコメント;

					Color color = this.formメインフォーム.mgr譜面管理者.listレーン[vチップWAV.nレーン番号].col背景色;
					cwav.col背景色 = Color.FromArgb( color.R/2 + 128, color.G/2 + 128, color.B/2 + 128 );

					// 配置予定全チップのWAVを指定する
					foreach ( CMIDIイベント vMIDIチップ in cMIDI.lチップ )
					{
						if ( vチップWAV.strWAV重複チェック == vMIDIチップ.strWAV重複チェック ) vMIDIチップ.nWAV = nWAVCount;
					}
					nWAVCount ++;
				}
				
				// WAVリスト強制更新
				this.formメインフォーム.listViewWAVリスト.Refresh();

				cMIDI.lチップ.Sort( ( ( a, b ) => (int) a.n時間 - (int) b.n時間 ) );	// 複数トラックへの対応のため

				// BPM他情報
                if ( cMIDI.f先頭BPM > 0.0 ) this.formメインフォーム.numericUpDownBPM.Value = (decimal)cMIDI.f先頭BPM;
                this.formメインフォーム.textBox曲名.Text = Path.GetFileName( cMIDI.strファイル名 );
                if ( cMIDI.nMIDI重複チップ数を返す() > 0 ) this.formメインフォーム.textBoxコメント.Text = "重複チップ : "+cMIDI.nMIDI重複チップ数を返す();
				
				// 小節付加＋変拍子設定
				tMIDIイベントリストから小節リストを構成する( cMIDI.lチップ, cMIDI.n分解能 );
				
				// チップ配置
                foreach ( CMIDIイベント vMIDIチップ in cMIDI.lチップ )
                {
					if ( vMIDIチップ.b入力 )
					{
						vMIDIチップ.挿入( this.formメインフォーム, cMIDI.n分解能 );
					}
                }
				// BGMチップを仮置きする
				this.formメインフォーム.mgr譜面管理者.tチップを配置または置換する( this.formメインフォーム.mgr譜面管理者.nレーン名に対応するレーン番号を返す( "BGM" ), 0, 2, 0f, false );
            }
		}
		
		// lMIDIWAVソート用
		static int nMIDIWAVSort( CMIDIイベント a, CMIDIイベント b )
		{
			if ( a.nレーン番号 > b.nレーン番号 ) return 1;
			else if ( a.nレーン番号 < b.nレーン番号 ) return -1;
			else
			{
				if ( a.nキー > b.nキー ) return -1;
				else if ( a.nキー < b.nキー ) return 1;
				else
				{
					if ( a.nベロシティ > b.nベロシティ ) return -1;
					else if ( a.nベロシティ < b.nベロシティ ) return 1;
					else return 0;
				}
			}
		}

		private struct barlen
		{
			public int n時間;
			public int n分子;
			public int n分母;

			public barlen( int _n時間, int _n分子, int _n分母 )
			{
				n時間 = _n時間;
				n分子 = _n分子;
				n分母 = _n分母;
			}
		}
        
		private void tMIDIイベントリストから小節リストを構成する( List<CMIDIイベント> cml, int n四分音符の分解能 )
		{
			if ( cml.Count <= 0 ) return;

			// 最終拍子イベント以降、曲最後までの小節について、この先のロジックで小節長を変更するために、ダミーで最後に拍子変更のイベントを入れる。

			int n最終分子 = 1;
			int n最終分母 = 1;
			int n最終時間 = (int)cml[ cml.Count - 1 ].n時間;

			cml.Reverse();
			foreach ( CMIDIイベント cm in cml )
			{
				if ( cm.eイベントタイプ == CMIDIイベント.Eイベントタイプ.BarLen )
				{
					n最終分子 = cm.n拍子分子;
					n最終分母 = cm.n拍子分母;
					break;
				}
			}
			cml.Reverse();

			if ( n最終時間 >= 0 )
			{
				cml.Add( new CMIDIBARLen( (UInt32)n最終時間, n最終分子, n最終分母 ) );
			}

			
			this.formメインフォーム.mgr譜面管理者.dic小節.Clear();
			foreach ( CMIDIイベント cm in cml )
			{
				if ( cm.eイベントタイプ == CMIDIイベント.Eイベントタイプ.BarLen )
				{
					// もし拍子変更イベントの絶対時間が、小節外にあれば、必要なだけ小節を追加する
					while ( true )
					{
						bool bExistBar = true;
						// 現在保持している小節リストの、nGridの最大値を取得する
						int nCurrentMaxBar = this.formメインフォーム.mgr譜面管理者.n現在の最大の小節番号を返す();
						int nCurremtMaxBar_FirstGrid = this.formメインフォーム.mgr譜面管理者.n譜面先頭からみた小節先頭の位置gridを返す( nCurrentMaxBar );
						if ( nCurremtMaxBar_FirstGrid < 0 ) nCurremtMaxBar_FirstGrid = 0;

						C小節 c最終小節 = this.formメインフォーム.mgr譜面管理者.p譜面先頭からの位置gridを含む小節を返す( nCurremtMaxBar_FirstGrid );
						float fCurrent小節倍率 = (c最終小節 == null) ? 1.0f : c最終小節.f小節長倍率;
						int nCurrentMaxGrid = nCurremtMaxBar_FirstGrid + (int) ( 192 * fCurrent小節倍率 ) - 1;
						if ( nCurrentMaxBar < 0 ) nCurrentMaxGrid = -1;

						// 拍子変更イベントの絶対時間が、小節外にあれば、新規に小節を一つ追加する。
						// 小節長は前の小節長を継承するか、MIDIイベント指定による新しい値にするか。
						// 小節を1つ追加しただけでは足りないのであれば、whileループで繰り返し追加し続ける。
						int nEvent時間 = (int)cm.n時間 * ( 192 / 4 ) / n四分音符の分解能;
						if ( nCurrentMaxGrid < (int) nEvent時間 )
						{
							++nCurrentMaxBar;

							C小節 c小節 = new C小節( nCurrentMaxBar );
							if ( c小節 != null )
							{
								c小節.f小節長倍率 = fCurrent小節倍率;
								this.formメインフォーム.mgr譜面管理者.dic小節.Add( nCurrentMaxBar, c小節 );
							}
							else
							{
								throw new Exception("C小節の作成に失敗しました。");
							}
						}
						else
						{
							// 小節追加whileループの最後か、または小節が既に存在する場合でも、拍子の変更があれば反映する。
							if (cm.eイベントタイプ == CMIDIイベント.Eイベントタイプ.BarLen)
							{
								C小節 c小節 = this.formメインフォーム.mgr譜面管理者.p譜面先頭からの位置gridを含む小節を返す( nEvent時間 );
								this.formメインフォーム.t小節長を変更する_小節単位( c小節.n小節番号0to3599, (float)cm.n拍子分子 / cm.n拍子分母 );
							}
							break;
						}
					}
				}
			}

		}

	}
}
