using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DTXCreator.コード._06.CDialog
{
	public partial class CDialog : Form
	{
		//public CDialog()
		//{
		//	InitializeComponent();
		//}

		public CDialog(string title, string message)
		{
			InitializeComponent();
			this.Text = title;
			this.label_Dialog.Text = message;
		}

		private void button_ok_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
			this.Close();
		}
	}
}
