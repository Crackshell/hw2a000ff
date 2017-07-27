using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace hw2a000ff
{
	public partial class FormWaiting : Form
	{
		public FormWaiting()
		{
			InitializeComponent();
		}

		public void SetStatus(string text)
		{
			Invoke(new Action(() => {
				labelStatus.Text = text;
			}));
		}
	}
}
