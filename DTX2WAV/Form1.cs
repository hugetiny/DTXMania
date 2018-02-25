using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DTX2WAV
{
	public partial class Main : Form
	{
		Form_Recording formRecording;
		Process p_DTXMania = null;
		bool bOpenedEncodingSettingTab = false;

		public Main()
		{
			InitializeComponent();
			bindingSource_BGM.DataSource = new VolumeSlider();
			bindingSource_SE.DataSource = new VolumeSlider();
			bindingSource_Drums.DataSource = new VolumeSlider();
			bindingSource_Guitar.DataSource = new VolumeSlider();
			bindingSource_Bass.DataSource = new VolumeSlider();
			bindingSource_Master.DataSource = new VolumeSlider();
			bindingSource_Ogg_Q.DataSource = new VolumeSlider();
		}

		/// <summary>
		/// メインウインドウの表示時に実行
		/// タイトルバーに、アプリ名とリリース番号を表示する
		/// 設定値の復元
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Main_Shown(object sender, EventArgs e)
		{
			#region [ タイトルバーの設定 ]
			System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
			int ver_asm_major = asm.GetName().Version.Major;
			this.Text = "DTX2WAV Rel" + ver_asm_major.ToString("D3");
			#endregion

			#region [ 設定値の復元 ]
			// アプリのverup時は旧バージョンの設定を引き継ぐ。
			// さもなくば、前回終了時の設定値を引き継ぐ。
			if (Properties.Settings.Default.IsUpgrade == false)
			{
				// Upgradeを実行する
				Properties.Settings.Default.Upgrade();

				// 「Upgradeを実行した」という情報を設定する
				Properties.Settings.Default.IsUpgrade = true;

				// 現行バージョンの設定を保存する
				Properties.Settings.Default.Save();
			}
			else
			{
				// 設定値の復元
				Properties.Settings.Default.Reload();
			}
			#endregion

			#region [ 復元した設定値を、Formに反映する ]
			numericUpDown_BGM.Value            = Properties.Settings.Default.nVol_BGM;
			numericUpDown_SE.Value             = Properties.Settings.Default.nVol_SE;
			numericUpDown_Drums.Value          = Properties.Settings.Default.nVol_Drums;
			numericUpDown_Guitar.Value         = Properties.Settings.Default.nVol_Guitar;
			numericUpDown_Bass.Value           = Properties.Settings.Default.nVol_Bass;
			numericUpDown_Master.Value         = Properties.Settings.Default.nVol_Master;
			checkBox_MonitorSound.Checked      = Properties.Settings.Default.bMonitorSound;
			comboBox_AudioFormat.SelectedIndex = Properties.Settings.Default.nAudioFormat;

			numericUpDown_Ogg_Q.Value          = Properties.Settings.Default.nOgg_Q;        // この設定は後でもう一度実施する。tabControl1_SelectedIndexChanged()へ。
			comboBox_MP3_bps.SelectedIndex     = Properties.Settings.Default.nMP3_bps;
			#endregion
		}

		private void button_browseDTX_Click(object sender, EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();

			ofd.Filter = Properties.Resources.ofdFilter;
			ofd.FilterIndex = 1;
			ofd.Title = Properties.Resources.ofdTitle;

			ofd.RestoreDirectory = false;

			ofd.CheckFileExists = true;
			ofd.CheckPathExists = true;

			if (ofd.ShowDialog() == DialogResult.OK)
			{
				textBox_BrowseDTX.Text = ofd.FileName;

				string outpath = Path.Combine(
									Path.GetDirectoryName(ofd.FileName),
									Path.GetFileNameWithoutExtension(ofd.FileName) + "." + comboBox_AudioFormat.Text.ToLower()
				);
				textBox_BrowseAudio.Text = outpath;
			}

		}

		private void button_browseWAV_Click(object sender, EventArgs e)
		{
			SaveFileDialog sfd = new SaveFileDialog();

			string filter = "";
			switch (comboBox_AudioFormat.Text)
			{
				case "WAV":
					filter = Properties.Resources.sfdFilterWAV;
					break;
				case "OGG":
					filter = Properties.Resources.sfdFilterOGG;
					break;
				case "MP3":
					filter = Properties.Resources.sfdFilterMP3;
					break;
				default:
					filter = Properties.Resources.sfdFilterALL;
					break;
			}
			sfd.Filter = filter;
			sfd.FilterIndex = 1;
			sfd.DefaultExt = "." + comboBox_AudioFormat.Text.ToLower();
			sfd.Title = Properties.Resources.sfdTitle;

			sfd.RestoreDirectory = false;
			sfd.AddExtension = true;

			sfd.OverwritePrompt = true;
			sfd.CheckFileExists = false;
			sfd.CheckPathExists = false;

			if (sfd.ShowDialog() == DialogResult.OK)
			{
				textBox_BrowseAudio.Text = sfd.FileName;
			}
		}


		/// <summary>
		/// 変換を実行。DTXManiaGRをDTX2WAVモードで呼び出す。
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button_Convert_Click(object sender, EventArgs e)
		{
			#region [ in/outファイル名など、必要な設定がなされているかをチェック ]
			if (!File.Exists(textBox_BrowseDTX.Text))
			{
				MessageBox.Show(Properties.Resources.errNoDTXFileText, Properties.Resources.errNoDTXFileCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			if (textBox_BrowseAudio.Text == "")
			{
				MessageBox.Show(Properties.Resources.errNoOutFileText, Properties.Resources.errNoOutFileCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			string inext = Path.GetExtension(textBox_BrowseAudio.Text).ToLower();
			if (inext == ".dtx" || inext == ".gda" || inext == ".g2d")
			{
				MessageBox.Show(Properties.Resources.errIllegalExtentionText, Properties.Resources.errIllegalExtentionCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			#endregion

			#region [ 録音用にDTXManiaプロセスを起動する ]
			p_DTXMania = new System.Diagnostics.Process();

			//イベントハンドラがフォームを作成したスレッドで実行されるようにする
			p_DTXMania.SynchronizingObject = this;
			//イベントハンドラの追加
			p_DTXMania.Exited += new EventHandler(p_Exited);
			p_DTXMania.EnableRaisingEvents = true;

			//アプリ名と引数の情報を設定
			p_DTXMania.StartInfo.FileName = "DTXManiaGR.exe";
			p_DTXMania.StartInfo.Arguments  = $"-E{comboBox_AudioFormat.Text.ToUpper()},";

			switch (comboBox_AudioFormat.Text.ToUpper())
			{
				case "WAV":
					p_DTXMania.StartInfo.Arguments +=  "48000,192,";	// freqとbitrate、DTXMania側ではいずれも無視される
					break;
				case "OGG":
					p_DTXMania.StartInfo.Arguments += $"48000,{numericUpDown_Ogg_Q.Value},";
					break;
				case "MP3":
					p_DTXMania.StartInfo.Arguments += $"48000,{comboBox_MP3_bps.Text},";
					break;
				default:
					p_DTXMania.StartInfo.Arguments +=  "48000,192,";
					break;
			}

			p_DTXMania.StartInfo.Arguments += $"{numericUpDown_BGM.Value},{numericUpDown_SE.Value},{numericUpDown_Drums.Value},{numericUpDown_Guitar.Value},{numericUpDown_Bass.Value},{numericUpDown_Master.Value},";
			p_DTXMania.StartInfo.Arguments += $"\"{textBox_BrowseAudio.Text}\",\"{textBox_BrowseDTX.Text}\"";

			//起動する
			try
			{
				p_DTXMania.Start();
			}
			catch (Exception)
			{
				MessageBox.Show(Properties.Resources.errFailedLaunchingDTXManiaText, Properties.Resources.errFailedLaunchingDTXManiaCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
				p_DTXMania.Close();
				p_DTXMania.Dispose();
				p_DTXMania = null;
				return;
			}
			#endregion

			//モーダルで変換中ダイアログを表示して、処理をいったん止める(キャンセル or 正常終了イベント待ち)
			formRecording = new Form_Recording();
			//formRecording.StartPosition = FormStartPosition.CenterParent;
			formRecording.ShowDialog(this);

			if (formRecording != null)
			{
				//フォームでCancelボタンが押されると、ここに来る
				//変換終了時のイベントで正常終了する場合は、p_Exited()で終了して、fがnullになって、ここはスキップされる
				formRecording.Dispose();
				formRecording = null;
			}
		}

		private void p_Exited(object sender, EventArgs e)
		{
			if (formRecording != null)
			{
				formRecording.Dispose();
				formRecording = null;
			}
			
			// DTXManiaプロセスの返り値を確認し、それに応じたダイアログを表示する
			if (p_DTXMania != null)
			{
				//MessageBox.Show(p_DTXMania.ExitCode.ToString());

				switch (p_DTXMania.ExitCode)
				{
					case 0:		// 正常終了
						using (var f = new Form_Finished_OK())
						{
							f.ShowDialog(this);
						}
						break;

					case 10010:		// Cancel
						using (var f = new Form_Finished_Fail())
						{
							f.ShowDialog(this);
						}
						break;

					default:		// DTXMania本体を強制終了した、など
						break;

				}
			}
			p_DTXMania.Close();
			p_DTXMania.Dispose();
			p_DTXMania = null;
		}

		/// <summary>
		/// Cancel押下時は、アプリ終了
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button_Cancel_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}


		/// <summary>
		/// 出力フォーマットを変更した場合は、出力パスの拡張子も更新する
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void comboBox_AudioFormat_SelectedIndexChanged(object sender, EventArgs e)
		{
			string outpath = textBox_BrowseAudio.Text;
			if (outpath == "")
			{
				return;
			}

			textBox_BrowseAudio.Text =
				Path.Combine(
					Path.GetDirectoryName(outpath),
					Path.GetFileNameWithoutExtension(outpath) + "." + comboBox_AudioFormat.Text.ToLower()
			);
		}

		
		
		
		/// <summary>
		/// NumericUpDown とTrackBar にバインドされるデータ
		/// </summary>
		public class VolumeSlider : INotifyPropertyChanged
		{
			/// <summary>
			/// INotifyPropertyChanged から継承したイベントデリゲート
			/// </summary>
			public event PropertyChangedEventHandler PropertyChanged;
			/// <summary>
			/// イベント通知
			/// </summary>
			/// <param name="info"></param>
			private void NotifyPropertyChanged(String info)
			{
				if (PropertyChanged != null)
				{
					PropertyChanged(this, new PropertyChangedEventArgs(info));
				}
			}
			int _value;
			public int Value
			{
				get { return _value; }
				set
				{
					if (value != _value)
					{
						_value = value;
						// このプロパティ名を渡してイベント通知
						NotifyPropertyChanged("Value");
					}
				}
			}

		}

		/// <summary>
		/// アプリ終了時に、設定値を保存
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Main_FormClosing(object sender, FormClosingEventArgs e)
		{

			#region [ Formの設定値を、Propertiesに退避する ]
			Properties.Settings.Default.nVol_BGM      = (int)numericUpDown_BGM.Value;
			Properties.Settings.Default.nVol_SE       = (int)numericUpDown_SE.Value;
			Properties.Settings.Default.nVol_Drums    = (int)numericUpDown_Drums.Value;
			Properties.Settings.Default.nVol_Guitar   = (int)numericUpDown_Guitar.Value;
			Properties.Settings.Default.nVol_Bass     = (int)numericUpDown_Bass.Value;
			Properties.Settings.Default.nVol_Master   = (int)numericUpDown_Master.Value;
			Properties.Settings.Default.bMonitorSound = checkBox_MonitorSound.Checked;
			Properties.Settings.Default.nAudioFormat  = comboBox_AudioFormat.SelectedIndex;
			Properties.Settings.Default.nOgg_Q        = (int)numericUpDown_Ogg_Q.Value;
			Properties.Settings.Default.nMP3_bps      = comboBox_MP3_bps.SelectedIndex;
			#endregion

			Properties.Settings.Default.Save();
		}


		#region #28821 2014.1.23 yyagi add: 外部からの文字列メッセージ送受信 定数定義
		[StructLayout(LayoutKind.Sequential)]
		public struct COPYDATASTRUCT
		{
			public IntPtr dwData;
			public UInt32 cbData;
			public IntPtr lpData;
		}
		#endregion
		/// <summary>
		/// メッセージを受信する
		/// </summary>
		/// <param name="m"></param>
		protected override void WndProc(ref Message m)
		{
			if (m.Msg == 0x004A) //WM_COPYDATA
			{
				COPYDATASTRUCT cds = (COPYDATASTRUCT)Marshal.PtrToStructure(m.LParam, typeof(COPYDATASTRUCT));
				string strMessage = Marshal.PtrToStringUni(cds.lpData);
				formRecording.label_state.Text = strMessage;	// Form_Recordingにメッセージの内容を伝える
			}
			base.WndProc(ref m);
		}



		private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
		{
			// Encoding Settingsタブにある、numericUpDownかTrackbarの値は、
			// Form_Shown()で初期化した値が、
			// タブの初回オープン時にゼロクリアされてしまう模様。
			// そのため、この値だけは、タブの初回オープン時に初期化する。
			if (tabControl1.SelectedIndex == 1 && bOpenedEncodingSettingTab == false)
			{
				bOpenedEncodingSettingTab = true;
				numericUpDown_Ogg_Q.Value = Properties.Settings.Default.nOgg_Q;
			}
		}
	}
	
}

