using System.Drawing;
using System.Windows.Forms;

namespace EmguLeap
{
	public partial class ImageFormA : Form
	{
		public ImageFormA()
		{
			InitializeComponent();
		}

		public void ChangeImages(Bitmap[] images)
		{
			if (images.Length == 3)
			{
				left.Image = images[0];
				right.Image = images[1];
				disparity.Image = images[2];
			}
			if (images.Length == 2)
			{
				left.Image = images[0];
				right.Image = images[1];
			}
			else
			{
				return;
			}
		}
	}
}
