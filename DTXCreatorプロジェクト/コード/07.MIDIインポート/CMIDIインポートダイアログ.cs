using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.IO;
using System.Drawing;
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
                    case 46 : str楽器名 = "Open Hi-hat"; strレーン名 = "HH"; break;
                    case 47 : str楽器名 = "Mid Tom 1"; strレーン名 = "LT"; break;
                    case 48 : str楽器名 = "High Tom 2"; strレーン名 = "HT"; break;
                    case 49 : str楽器名 = "Crash Cymbal 1"; strレーン名 = "CY"; break;
                    case 50 : str楽器名 = "High Tom 1"; strレーン名 = "HT"; break;
                    case 51 : str楽器名 = "Ride Cymbal 1"; strレーン名 = "CY"; break;
                    case 52 : str楽器名 = "Chinese Cymbal"; strレーン名 = "CY"; break;
                    case 53 : str楽器名 = "Ride Bell"; strレーン名 = "CY"; break;
                    case 54 : str楽器名 = "Tambourine"; strレーン名 = "SE1"; break;
                    case 55 : str楽器名 = "Splash Cymbal"; strレーン名 = "LC"; break;
                    case 56 : str楽器名 = "Cowbell"; strレーン名 = "SE1"; break;
                    case 57 : str楽器名 = "Crash Cymbal 2"; strレーン名 = "LC"; break;
                    case 58 : str楽器名 = "Vibra Slap"; strレーン名 = "SE1"; break;
                    case 59 : str楽器名 = "Ride Cymbal 2"; strレーン名 = "CY"; break;
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
                this.dataGridView1.Rows.Add( i, strキー名[i%12], strレーン名, 0, str楽器名 );
                if ( i%12 == 1 || i%12 == 3 || i%12 == 6 || i%12 == 8 || i%12 == 10 ) this.dataGridView1.Rows[127-i].DefaultCellStyle.BackColor = Color.FromArgb( 240, 248, 255 );
                if ( i%12 == 0 ) this.dataGridView1.Rows[127-i].DefaultCellStyle.BackColor = Color.FromArgb( 255, 224, 224 );
                tMIDI割り当て一覧のレーン名の背景色を変更する( this.dataGridView1.RowCount-1 );

            }
            this.dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dataGridView1.Columns[2].DefaultCellStyle.Font = new Font( "meiryo", 8f, FontStyle.Bold );
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

        private void tMIDIファイルを開く( string strファイル名 )
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
                this.dataGridView1.Rows[127-i].Cells[3].Value = cMIDI.nドラム各ノート数[i];
            }
			
            // MIDI解析内容をテキストボックスに出力する
            string str文字列 = "";
            str文字列 += "File:" + strファイル名 + "\r\n";
            str文字列 += "BPM:" + cMIDI.dBPM + "\r\n";
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
			string strレーン名 = (string)this.dataGridView1.Rows[RowIndex].Cells[2].Value;
            int nレーン番号 = this.formメインフォーム.mgr譜面管理者.nレーン名に対応するレーン番号を返す( strレーン名 );
            if ( nレーン番号 > 1 )
            {
                Color color = this.formメインフォーム.mgr譜面管理者.listレーン[nレーン番号].col背景色;
                color = Color.FromArgb( color.R/2+128, color.G/2+128, color.B/2+128 );
                this.dataGridView1.Rows[RowIndex].Cells[2].Style.BackColor = color;
            }
			else if ( strレーン名 == "* Disuse *" )
			{
                Color color = Color.FromArgb( 128, 128, 128 );
                this.dataGridView1.Rows[RowIndex].Cells[2].Style.BackColor = color;
			}
        }

        public void tMIDIインポート結果を反映する()
        {
            if ( cMIDI != null && cMIDI.lチップ.Count > 0 )
            {
				// チップリストで、ベロシティをDTX向けに調整する
				foreach ( CMIDIチップ vMIDIチップ in cMIDI.lチップ )
				{
					int num3 = vMIDIチップ.nベロシティ;
					if ( this.checkBox2.Checked ) num3 = (int)(num3 / 1.27);
					if ( this.checkBox1.Checked ) num3 = (int)( Math.Pow( num3, 1.5 ) / Math.Pow( 100, 0.5 ) );
					num3 = ( num3 / (int)this.numericUpDown1.Value ) * (int)this.numericUpDown1.Value;
					num3 = ( num3 > 100 ) ? 100 : ( ( num3 == 0 ) ? 1 : num3 );
					vMIDIチップ.nベロシティ_DTX変換後 = num3;
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
				
				foreach ( CMIDIチップ vチップWAV in cMIDI.lMIDIWAV )
				{
					if ( nWAVCount > 4 && nレーン番号before != vチップWAV.nレーン番号 ) nWAVCount++;
					nレーン番号before = vチップWAV.nレーン番号;

					cwav = this.formメインフォーム.mgrWAVリスト管理者.tWAVをキャッシュから検索して返す_なければ新規生成する( nWAVCount );
					cwav.strファイル名 = vチップWAV.nキー.ToString() + ".ogg";

					cwav.n音量0to100 = vチップWAV.nベロシティ_DTX変換後;
					cwav.strラベル名 = this.formメインフォーム.mgr譜面管理者.listレーン[vチップWAV.nレーン番号].strレーン名 + " " +vチップWAV.strコメント;

					Color color = this.formメインフォーム.mgr譜面管理者.listレーン[vチップWAV.nレーン番号].col背景色;
					cwav.col背景色 = Color.FromArgb( color.R/2 + 128, color.G/2 + 128, color.B/2 + 128 );

					// 配置予定全チップのWAVを指定する
					foreach ( CMIDIチップ vMIDIチップ in cMIDI.lチップ )
					{
						if ( vチップWAV.strWAV重複チェック == vMIDIチップ.strWAV重複チェック ) vMIDIチップ.nWAV = nWAVCount;
					}
					nWAVCount ++;
				}
				
				// WAVリスト強制更新
				this.formメインフォーム.listViewWAVリスト.Refresh();

				// BPM他情報
                this.formメインフォーム.numericUpDownBPM.Value = (decimal)cMIDI.dBPM;
                this.formメインフォーム.textBox曲名.Text = Path.GetFileName( cMIDI.strファイル名 );
                //this.formメインフォーム.textBox自由入力欄.Text = "; DTXC MI "+Resources.DTXC_VERSION;
                if ( cMIDI.nMIDI重複チップ数を返す() > 0 ) this.formメインフォーム.textBoxコメント.Text = "重複チップ : "+cMIDI.nMIDI重複チップ数を返す();
				
				// 小節付加
				int num = this.formメインフォーム.mgr譜面管理者.n現在の最大の小節番号を返す();
				int num2 = cMIDI.pMIDIチップで一番遅い時間のチップを返す().n時間 / ( cMIDI.n分解能 / 96 ) / 384;
				for( int i = num + 1; i <= num2 ; i++ )
				{
					this.formメインフォーム.mgr譜面管理者.dic小節.Add( i, new C小節( i ) );
				}
				
				// チップ配置
                foreach ( CMIDIチップ vMIDIチップ in cMIDI.lチップ )
                {
					if ( vMIDIチップ.b入力 )
					{
						this.formメインフォーム.mgr譜面管理者.tチップを配置または置換する( vMIDIチップ.nレーン番号, vMIDIチップ.n時間 / (cMIDI.n分解能 / 96 ), vMIDIチップ.nWAV, 0f, false );
					}
                }
				this.formメインフォーム.mgr譜面管理者.tチップを配置または置換する( this.formメインフォーム.mgr譜面管理者.nレーン名に対応するレーン番号を返す( "BGM" ), 0, 2, 0f, false );
				

            }
        }
		
		// lMIDIWAVソート用
		static int nMIDIWAVSort( CMIDIチップ a, CMIDIチップ b )
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
        
    }
}
