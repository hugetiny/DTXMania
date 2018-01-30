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
	public partial class Form_Finished : Form
	{
		public Form_Finished()
		{
			InitializeComponent();
		}

		private void button_OK_Click(object sender, EventArgs e)
		{
			this.Close();		}
	}
}
