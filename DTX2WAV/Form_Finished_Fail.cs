using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DTX2WAV
{
	public partial class Form_Finished_Fail : Form
	{
		public Form_Finished_Fail()
		{
			InitializeComponent();
		}

		private void button_OK_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void Form_Finished_Fail_Shown(object sender, EventArgs e)
		{
			Bitmap canvas = new Bitmap(pictureBox_FailIcon.Width, pictureBox_FailIcon.Height);
			Graphics g = Graphics.FromImage(canvas);

			g.DrawIcon(SystemIcons.Warning, 0, 0);
			g.Dispose();
			pictureBox_FailIcon.Image = canvas;
		}
	}
}
