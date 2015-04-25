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
			if (amount.InvokeRequired)
			{
				amount.Invoke(new MethodInvoker(delegate
								{
									amount.Text = newDistance.ToString("F");
								}));
			}
			else
			{
				amount.Text = newDistance.ToString("F");
			}
		}
	}
}
