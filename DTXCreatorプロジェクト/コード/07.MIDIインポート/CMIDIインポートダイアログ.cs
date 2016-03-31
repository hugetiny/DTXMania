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
        private bool b一覧準備完了;
		private System.Resources.ResourceManager resource;
		
        public CMIDIインポートダイアログ()
        {
            InitializeComponent();
			this.b一覧準備完了 = false;
			resource = new System.Resources.ResourceManager( this.GetType() );
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

		private void dgvチャンネル一覧_CurrentCellDirtyStateChanged(object sender, EventArgs e)
		{
			if ( dgvチャンネル一覧.IsCurrentCellDirty )
			{
				dgvチャンネル一覧.CommitEdit( DataGridViewDataErrorContexts.Commit );
			}
		}
		private void dgvチャンネル一覧_CellValueChanged( object sender, DataGridViewCellEventArgs e )
		{
			if ( cMIDI != null && this.b一覧準備完了 ) this.tMIDIファイルを開く( cMIDI.strファイル名 );
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
                    case 35 : str楽器名 = "Bass Drum 2"; strレーン名 = "LP"; b裏チャンネル = true; break;
                    case 36 : str楽器名 = "Bass Drum 1"; strレーン名 = "BD"; break;
                    case 37 : str楽器名 = "Side Stick"; strレーン名 = "SE1"; break;
                    case 38 : str楽器名 = "Snare Drum 1"; strレーン名 = "SD"; break;
                    case 39 : str楽器名 = "Hand Clap"; strレーン名 = "SE1"; break;
                    case 40 : str楽器名 = "Snare Drum 2"; strレーン名 = "SD"; break;
                    case 41 : str楽器名 = "Low Tom 2"; strレーン名 = "FT"; break;
                    case 42 : str楽器名 = "Closed Hi-hat"; strレーン名 = "HH"; break;
                    case 43 : str楽器名 = "Low Tom 1"; strレーン名 = "FT"; break;
                    case 44 : str楽器名 = "Pedal Hi-hat"; strレーン名 = "LP"; break;
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
                this.dgv割り当て一覧.Rows.Add( i, strキー名[i%12], strレーン名, b裏チャンネル, 0, str楽器名 );
                if ( i%12 == 1 || i%12 == 3 || i%12 == 6 || i%12 == 8 || i%12 == 10 ) this.dgv割り当て一覧.Rows[127-i].DefaultCellStyle.BackColor = Color.FromArgb( 240, 248, 255 );
                if ( i%12 == 0 ) this.dgv割り当て一覧.Rows[127-i].DefaultCellStyle.BackColor = Color.FromArgb( 255, 224, 224 );
                tMIDI割り当て一覧のレーン名の背景色を変更する( this.dgv割り当て一覧.RowCount-1 );

            }
            this.dgv割り当て一覧.Columns["MIDI_Key"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dgv割り当て一覧.Columns["DTX_Lane"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dgv割り当て一覧.Columns["DTX_Lane"].DefaultCellStyle.Font = new Font( "meiryo", 8f, FontStyle.Bold );
            this.dgv割り当て一覧.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            this.dgv割り当て一覧.FirstDisplayedScrollingRowIndex = 81;//key35=LBDが表示される位置

        }

        public void tMIDIチャンネル一覧を作成する()
        {
			for (int i = 1; i <= 16; i++)
			{
				this.dgvチャンネル一覧.Rows.Add( i, 0, (i==10) );
				this.dgvチャンネル一覧.Rows[i-1].DefaultCellStyle.BackColor = (i==10) ? Color.FromArgb( 255, 224, 224 ) : Color.FromArgb( 255, 255, 255 );
			}
			this.b一覧準備完了 = true;
		}

        public void tMIDIファイルを選択する()
        {
            #region [ ファイル選択 ]
            //-----------------

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "MIDIファイルを選択";
            dialog.Filter = "MIDIファイル (*.mid,*.midi)|*.mid;*.midi|すべてのファイル (*.*)|*.*";
            dialog.FilterIndex = 1;
            dialog.InitialDirectory = this.formメインフォーム.strMIDIインポートフォルダ;
            DialogResult result = dialog.ShowDialog();

            if (result != DialogResult.OK) return;

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
			this.formメインフォーム.strMIDIインポートフォルダ = Path.GetDirectoryName( strファイル名 ) + @"\";
            //-----------------
            #endregion

            #region [ MIDIファイル解析 ]
            //-----------------
            cMIDI = new CMIDI( strファイル名 );
            cMIDI.formメインフォーム = this.formメインフォーム;
			cMIDI.dgvチャンネル一覧 = this.dgvチャンネル一覧;
            cMIDI.tMIDIを解析する();
			cMIDI.tMIDIチップをレーンに割り当てる( this.dgv割り当て一覧 );

			this.label重複チップ数.Text = resource.GetString("label重複チップ数.Text") + " : " + cMIDI.n重複チップ数;
			
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
                this.dgv割り当て一覧.Rows[127-i].Cells["Notes"].Value = cMIDI.nドラム各ノート数[i];
			
            // MIDI解析内容をテキストボックスに出力する
            string str文字列 = "";
            str文字列 += "File:" + strファイル名 + "\r\n";
            str文字列 += "BPM:" + cMIDI.f先頭BPM + "\r\n";
            str文字列 += "TimeBase:" + cMIDI.n分解能 + "\r\n";
            str文字列 += "\r\n";
            
            foreach ( CMIDIトラック value in cMIDI.lMIDIトラック )
            {
                str文字列 += "Track " + value.nトラック数;
                str文字列 += " : " + value.strトラック名 + "\r\n";
                str文字列 += value.str解析内容 + "\r\n";
            }
            
            this.textBox1.Text = str文字列;

			for ( int i = 1 ; i <= 16 ; i++ )
                this.dgvチャンネル一覧.Rows[i-1].Cells["ChNotes"].Value = this.cMIDI.lチャンネル毎のノート数1to16[i];
            //-----------------
            #endregion

        }
        
        // レーン名をワンクリックで開く用
        private void dgv割り当て一覧_CellEnter( object sender, DataGridViewCellEventArgs e )
        {
            DataGridView dgv割り当て一覧 = (DataGridView) sender;

            if ( dgv割り当て一覧.Columns[e.ColumnIndex].Name == "DTX_Lane" && dgv割り当て一覧.Columns[e.ColumnIndex] is DataGridViewComboBoxColumn )
                SendKeys.Send("{F4}");
        }

		// レーン名を変更したら
        private void dgv割り当て一覧_CellEndEdit( object sender, DataGridViewCellEventArgs e )
        {
            DataGridView dgv割り当て一覧 = (DataGridView) sender;

            if ( dgv割り当て一覧.Columns[e.ColumnIndex].Name == "DTX_Lane" )
                tMIDI割り当て一覧のレーン名の背景色を変更する( e.RowIndex );
			
            if ( cMIDI != null && this.b一覧準備完了 )
			{
				cMIDI.tMIDIチップをレーンに割り当てる( dgv割り当て一覧 );
				this.label重複チップ数.Text = resource.GetString("label重複チップ数.Text") + " : " + cMIDI.n重複チップ数;
				
			}

        }
		
		/// <summary>
		/// レーン名変更時に呼び出される
		/// </summary>
        private void tMIDI割り当て一覧のレーン名の背景色を変更する( int RowIndex )
        {
			string strレーン名 = (string)this.dgv割り当て一覧.Rows[RowIndex].Cells["DTX_Lane"].Value;
            int nレーン番号 = this.formメインフォーム.mgr譜面管理者.nレーン名に対応するレーン番号を返す( strレーン名 );

            if ( nレーン番号 >= this.formメインフォーム.mgr譜面管理者.nレーン名に対応するレーン番号を返す( "LC" ) )
            {
                Color color = this.formメインフォーム.mgr譜面管理者.listレーン[nレーン番号].col背景色;
                color = Color.FromArgb( color.R/2+128, color.G/2+128, color.B/2+128 );
                this.dgv割り当て一覧.Rows[RowIndex].Cells["DTX_Lane"].Style.BackColor = color;
            }
			else if ( strレーン名 == "* Disuse *" )
			{
                Color color = Color.FromArgb( 128, 128, 128 );
                this.dgv割り当て一覧.Rows[RowIndex].Cells["DTX_Lane"].Style.BackColor = color;
			}
        }

        public void tMIDIインポート結果を反映する()
        {
            if ( cMIDI != null && cMIDI.lMIDIイベント.Count > 0 )
            {
				
				#region [ チップリストで、ベロシティをDTX向けに調整する ]
				foreach ( CMIDIイベント vMIDIイベント in cMIDI.lMIDIイベント )
				{
					int velo = vMIDIイベント.nベロシティ;
					if ( this.checkBoxベロシティ最大値127.Checked ) velo = (int)(velo / 1.27);//127を最大値
					if ( this.checkBoxベロシティカーブ調整.Checked ) velo = (int)( Math.Pow( velo, 1.5 ) / Math.Pow( 100, 0.5 ) );//ベロシティカーブ
					velo = ( velo / (int)this.numericUpDownVOLUME間隔.Value ) * (int)this.numericUpDownVOLUME間隔.Value;
					velo = ( velo > 100 ) ? 100 : ( ( velo == 0 ) ? 1 : velo );
					vMIDIイベント.nベロシティ_DTX変換後 = velo;
				}
				#endregion

				#region [ 配置予定チップを割り当て一覧に沿ってレーンを割り当てる ]
				cMIDI.tMIDIチップをレーンに割り当てる( this.dgv割り当て一覧 );
				#endregion
				
				#region [ WAVリスト出力 ]

				#region [ #WAV02 BGM仮置き用 ]
				CWAV cwav = this.formメインフォーム.mgrWAVリスト管理者.tWAVをキャッシュから検索して返す_なければ新規生成する( 2 );
				cwav.strラベル名 = "BGM";
				cwav.bBGMとして使用 = true;
				cwav.col背景色 = Color.FromArgb( 224, 255, 224 );
				#endregion
				
				#region [ #WAV04 以降をWAVリスト割り当てに使う ]
				// WAVリストをソートして見やすくする
				cMIDI.lMIDIWAV.Sort( nMIDIWAVSort );

				int nWAVCount = 4;
				int nレーン番号before = 0;
				
				foreach ( CMIDIイベント vMIDIWAV in cMIDI.lMIDIWAV )
				{
					// ノートチップ以外をWAVリストに表示させない
					if ( vMIDIWAV.eイベントタイプ != CMIDIイベント.Eイベントタイプ.NoteOnOff ) continue;

					// レーン毎に1行空ける
					if ( nWAVCount > 4 && nレーン番号before != vMIDIWAV.nレーン番号 ) nWAVCount++;
					nレーン番号before = vMIDIWAV.nレーン番号;

					// WAVリストに配置
					cwav = this.formメインフォーム.mgrWAVリスト管理者.tWAVをキャッシュから検索して返す_なければ新規生成する( nWAVCount );
					cwav.strファイル名 = vMIDIWAV.nキー.ToString() + ".ogg";
					cwav.n音量0to100 = vMIDIWAV.nベロシティ_DTX変換後;
					cwav.strラベル名 = this.formメインフォーム.mgr譜面管理者.listレーン[vMIDIWAV.nレーン番号].strレーン名
						+ ( vMIDIWAV.b裏チャンネル ? "*" : "" )
						+ " " + vMIDIWAV.strコメント;

					// WAVリストの背景色を、レーンの色に合わせる
					Color color = this.formメインフォーム.mgr譜面管理者.listレーン[vMIDIWAV.nレーン番号].col背景色;
					cwav.col背景色 = Color.FromArgb( color.R/2 + 128, color.G/2 + 128, color.B/2 + 128 );

					// 配置予定全チップにWAV番号を指定する
					foreach ( CMIDIイベント vMIDIイベント in cMIDI.lMIDIイベント )
					{
						if ( vMIDIWAV.strWAV重複チェック == vMIDIイベント.strWAV重複チェック )
							vMIDIイベント.nWAV = nWAVCount;
					}
					nWAVCount ++;
				}
				#endregion

				#region [ WAVリスト強制更新 ]
				this.formメインフォーム.listViewWAVリスト.Refresh();
				#endregion

				#endregion
				
				#region [ 小節付加＋変拍子設定 ]
				tMIDIイベントリストから小節リストを構成する( cMIDI.lMIDIイベント, cMIDI.n分解能 );
				#endregion
				
				#region [ チップ配置 ]
				// 複数トラックへの対応のため
				cMIDI.lMIDIイベント.Sort( ( ( a, b ) => (int) a.n時間 - (int) b.n時間 ) );

				// 配置予定チップを実際に配置する
                foreach ( CMIDIイベント vMIDIイベント in cMIDI.lMIDIイベント )
                {
					if ( vMIDIイベント.b入力 )
					{
						vMIDIイベント.挿入( this.formメインフォーム, cMIDI.n分解能 );
					}
                }

				// BGMチップを仮置き
				this.formメインフォーム.mgr譜面管理者.tチップを配置または置換する( this.formメインフォーム.mgr譜面管理者.nレーン名に対応するレーン番号を返す( "BGM" ), 0, 2, 0f, false );
				#endregion

				#region [ 情報を入力 ]
                if ( cMIDI.f先頭BPM > 0.0 ) this.formメインフォーム.numericUpDownBPM.Value = (decimal)cMIDI.f先頭BPM;
                this.formメインフォーム.textBox曲名.Text = Path.GetFileName( cMIDI.strファイル名 );
                if ( cMIDI.n重複チップ数 > 0 ) this.formメインフォーム.textBoxコメント.Text = resource.GetString("label重複チップ数.Text") + " : "+cMIDI.n重複チップ数;
				#endregion

            }
		}
		
		/// <summary>
		/// WAVリストを順番にソートする
		/// ・レーン番号：昇順
		/// ・裏チャンネル：昇順(表が0、裏が1)
		/// ・(MIDIの)キー：昇順
		/// ・音量：降順
		/// </summary>
		static int nMIDIWAVSort( CMIDIイベント a, CMIDIイベント b )
		{
			// 昇順
			if ( a.nレーン番号 > b.nレーン番号 ) return 1;
			else if ( a.nレーン番号 < b.nレーン番号 ) return -1;
			else
			{
				// 昇順
				if ( (a.b裏チャンネル?1:0) > (b.b裏チャンネル?1:0) ) return 1;
				else if ( (a.b裏チャンネル?1:0) < (b.b裏チャンネル?1:0) ) return -1;
				else
				{
					// 昇順
					if ( a.nキー > b.nキー ) return 1;
					else if ( a.nキー < b.nキー ) return -1;
					else
					{
						// 降順
						if ( a.nベロシティ > b.nベロシティ ) return -1;
						else if ( a.nベロシティ < b.nベロシティ ) return 1;
						else return 0;
					}
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
