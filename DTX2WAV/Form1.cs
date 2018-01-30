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

		public Main()
		{
			InitializeComponent();
			bindingSource_BGM.DataSource = new VolumeSlider();
			bindingSource_SE.DataSource = new VolumeSlider();
			bindingSource_Drums.DataSource = new VolumeSlider();
			bindingSource_Guitar.DataSource = new VolumeSlider();
			bindingSource_Bass.DataSource = new VolumeSlider();
			bindingSource_Master.DataSource = new VolumeSlider();
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
			numericUpDown_BGM.Value    = Properties.Settings.Default.nVol_BGM;
			numericUpDown_SE.Value     = Properties.Settings.Default.nVol_SE;
			numericUpDown_Drums.Value  = Properties.Settings.Default.nVol_Drums;
			numericUpDown_Guitar.Value = Properties.Settings.Default.nVol_Guitar;
			numericUpDown_Bass.Value   = Properties.Settings.Default.nVol_Bass;
			numericUpDown_Master.Value = Properties.Settings.Default.nVol_Master;
			checkBox_MonitorSound.Checked = Properties.Settings.Default.bMonitorSound;
			#endregion
		}

		private void button_browseDTX_Click(object sender, EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();

			ofd.Filter = "DTXファイル(*.dtx)|*.dtx|GDAファイル(*.gda;*.g2d)|*.gda;*.g2d)|すべてのファイル(*.*)|*.*";
			ofd.FilterIndex = 1;
			ofd.Title = "DTXファイルを選択してください";

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
				Debug.WriteLine("out:" + outpath);
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
					filter = "WAVファイル(*.wav)|*.wav|すべてのファイル(*.*)|*.*";
					break;
				case "OGG":
					filter = "OGGファイル(*.ogg)|*.ogg|すべてのファイル(*.*)|*.*";
					break;
				case "MP3":
					filter = "MP3ファイル(*.mp3)|*.mp3|すべてのファイル(*.*)|*.*";
					break;
				default:
					filter = "すべてのファイル(*.*)|*.*";
					break;
			}
			sfd.Filter = filter;
			sfd.FilterIndex = 1;
			sfd.DefaultExt = "." + comboBox_AudioFormat.Text.ToLower();
			sfd.Title = "出力ファイル名を選択してください";

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
			if (!File.Exists(textBox_BrowseDTX.Text))
			{
				MessageBox.Show("DTXファイルがありません。", "ファイルが見つかりません", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			if (textBox_BrowseAudio.Text == "")
			{
				MessageBox.Show("出力ファイルが指定されていません。", "ファイルが見つかりません", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			string inext = Path.GetExtension(textBox_BrowseAudio.Text).ToLower();
			if (inext == ".dtx" || inext == ".gda" || inext == ".g2d")
			{
				MessageBox.Show("出力ファイルとして、DTX/GDA/G2Dファイルを指定しています。", "出力ファイル指定エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			System.Diagnostics.Process p = new System.Diagnostics.Process();

			//イベントハンドラがフォームを作成したスレッドで実行されるようにする
			p.SynchronizingObject = this;
			//イベントハンドラの追加
			p.Exited += new EventHandler(p_Exited);
			p.EnableRaisingEvents = true;

			//アプリ名と引数の情報を設定
			p.StartInfo.FileName = "DTXManiaGR.exe";
			p.StartInfo.Arguments = $"-E{comboBox_AudioFormat.Text.ToUpper()},48000,192,";
			p.StartInfo.Arguments += $"{numericUpDown_BGM.Value},{numericUpDown_SE.Value},{numericUpDown_Drums.Value},{numericUpDown_Guitar.Value},{numericUpDown_Bass.Value},{numericUpDown_Master.Value},";
			p.StartInfo.Arguments += $"\"{textBox_BrowseAudio.Text}\",\"{textBox_BrowseDTX.Text}\"";

			//起動する
			p.Start();

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

			//MessageBox.Show(
			//	"録音が正常に終了しました。",
			//	"録音終了",
			//	MessageBoxButtons.OK,
			//	MessageBoxIcon.Information
			//);

			using (Form_Finished f = new Form_Finished())
			{
				f.ShowDialog();
			}
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
//Debug.WriteLine("Msg received: " + strMessage);
				formRecording.label_state.Text = strMessage;	// Form_Recordingにメッセージの内容を伝える
			}
			base.WndProc(ref m);
		}
	}
	
}

