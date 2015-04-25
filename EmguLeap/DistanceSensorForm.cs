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

		public void ChangeDistance(float cm,float raw)
		{
			if (amount.InvokeRequired)
			{
				amount.Invoke(new MethodInvoker(delegate
				{
					amount.Text = cm.ToString("F");
				}));
			}
			else
			{
				amount.Text = cm.ToString("F");
			}

			if (rawAmount.InvokeRequired)
			{
				rawAmount.Invoke(new MethodInvoker(delegate
				{
					rawAmount.Text = raw.ToString("F");
				}));
			}
			else
			{
				rawAmount.Text = raw.ToString("F");
			}
		}

		public double GetAngle()
		{
			var rawData = Angle.GetData();
			return rawData;
		}
	}
}
