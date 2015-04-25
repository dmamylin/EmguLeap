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

		public void ChangeDistance(float distance)
		{
			if (amount.InvokeRequired)
			{
				amount.Invoke(new MethodInvoker(delegate
				{
					amount.Text = distance.ToString("F");
				}));
			}
			else
			{
				amount.Text = distance.ToString("F");
			}
		}

		public double GetAngle()
		{
			var rawData = Angle.GetData();
			return rawData;
		}
	}
}
