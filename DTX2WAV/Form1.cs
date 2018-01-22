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

namespace DTX2WAV
{
	public partial class Main : Form
	{
		Form_Converting f;

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
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Main_Shown(object sender, EventArgs e)
		{
			System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
			int ver_asm_major = asm.GetName().Version.Major;

			this.Text = "DTX2WAV Rel" + ver_asm_major.ToString("D3");
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

		private void button2_Click(object sender, EventArgs e)
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
			p.StartInfo.Arguments = $"-E{comboBox_AudioFormat.Text.ToUpper()},48000,192,\"{textBox_BrowseAudio.Text}\",\"{textBox_BrowseDTX.Text}\"";

			//起動する
			p.Start();

			//モーダルで変換中ダイアログを表示して、処理をいったん止める(キャンセル or 正常終了イベント待ち)
			f = new Form_Converting();
			f.ShowDialog(this);

			if (f != null)
			{
				//フォームでCancelボタンが押されると、ここに来る
				//変換終了時のイベントで正常終了する場合は、p_Exited()で終了して、fがnullになって、ここはスキップされる
				f.Dispose();
				f = null;
			}
		}

		private void p_Exited(object sender, EventArgs e)
		{
			if (f != null)
			{
				f.Dispose();
				f = null;
			}

			MessageBox.Show(
				"変換が正常に終了しました。",
				"変換終了",
				MessageBoxButtons.OK,
				MessageBoxIcon.Information
			);
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
	}
	
}

