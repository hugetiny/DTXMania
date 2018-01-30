using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace DTX2WAV
{
	public partial class Form_Recording : Form
	{
		public Form_Recording()
		{
			InitializeComponent();
		}

		/// <summary>
		/// DTXMania本体に、録音中止のメッセージを送信
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button_CancelConverting_Click(object sender, EventArgs e)
		{
			CSendMessageToDTXMania.SendMessage("-C");
		}

		private void Form_Recording_Load(object sender, EventArgs e)
		{
			progressBar_Recording.Value = 0;
		}


		/// <summary>
		/// DTXMania本体からDTX2WAVのメインForm経由でメッセージを受信する
		/// (メインFormのWndProcでメッセージを受信し、Form_Recordingのlabel_state経由でForm_Recordingが受け取る)
		/// そして、進捗表示をする
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void label_state_TextChanged(object sender, EventArgs e)
		{
			switch (label_state.Text.Substring(0,4).ToUpper())
			{
				case "BOOT":
					label_boot_check.Text = "→";
					label_loading_check.Text = "";
					label_playing_check.Text = "";
					label_exit_check.Text = "";
					break;
				case "LOAD":
					label_boot_check.Text = "✔";
					label_loading_check.Text = "→";
					label_playing_check.Text = "";
					label_exit_check.Text = "";
					break;
				case "PLAY":
					label_boot_check.Text = "✔";
					label_loading_check.Text = "✔";
					label_playing_check.Text = "→";
					label_exit_check.Text = "";
					break;
				case "TERM":
					label_boot_check.Text = "✔";
					label_loading_check.Text = "✔";
					label_playing_check.Text = "✔";
					label_exit_check.Text = "→";
					break;
				case "TIME":
					string[] s = label_state.Text.Split(new char[] { ',' });
					int nEstimateTimeMs = Convert.ToInt32(s[2]);
					int nCurrentTimeMs = Convert.ToInt32(s[1]);
//Debug.WriteLine(label_state.Text + ": " + nCurrentTimeMs + " : " + nEstimateTimeMs);

					if (nCurrentTimeMs > nEstimateTimeMs)
					{
						nCurrentTimeMs = nEstimateTimeMs;
					}
					progressBar_Recording.Value = (int)(((double)nCurrentTimeMs / (double)nEstimateTimeMs) * 10000);

					//int nEstimateTimeMs = (CDTXMania.Instance.DTX.listChip.Count > 0) ? CDTXMania.Instance.DTX.listChip[CDTXMania.Instance.DTX.listChip.Count - 1].n発声時刻ms : 0;
					string strEstimateTime = (((double)nEstimateTimeMs) / 1000.0).ToString("####0.00");
					string strCurrentTime = (((double)nCurrentTimeMs) / 1000.0).ToString("####0.00");

					label_currentTime.Text = strCurrentTime;
					label_estimateTime.Text = strEstimateTime;

					break;

				default:
					break;
			}
		}
	}
}

