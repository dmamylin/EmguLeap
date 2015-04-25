using System.Drawing;
using System.Windows.Forms;

namespace EmguLeap
{
	public partial class DistanceSensorForm : Form
	{
		public DistanceSensorForm()
		{
			InitializeComponent();
		}

		public void ChangeImage(Bitmap image)
		{
			Image.Image = image;
		}

		public void ChangeDistance(float cmDistance, float rawDistance)
		{
			if (amount.InvokeRequired)
			{
				amount.Invoke(new MethodInvoker(delegate
				{
					amount.Text = cmDistance.ToString("F");
					label3.Text = rawDistance.ToString("F");
				}));
			}
			else
			{
				amount.Text = cmDistance.ToString("F");
				label3.Text = rawDistance.ToString("F");
			}
		}

		public double GetAngle()
		{
			var rawData = Angle.GetData();
			var invertedData = rawData;
			return invertedData;
		}
	}
}
