using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EmguLeap
{
	public partial class Distances : Form
	{
		public Distances()
		{
			InitializeComponent();
		}

		public void UpdateDistanceToCenter(float newDistance)
		{
			label2.Text = newDistance.ToString("F");
		}
	}
}
