using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace EmguLeap
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			settings = new Settings();
			distances = new Distances();
			generator = new DisparityGenerator();
			provider = new ImageProvider();
			provider.AddNewAction(ChangeImages);

			InitializeComponent();
			settings.Visible = true;
			distances.Visible = true;
		}

		public void ChangeImages(Bitmap[] images)
		{
			Console.WriteLine("images changed.");
			var leftIm = images[0];
			var rightIm = images[1];
			if (IsHandleCreated)
				Invoke(new Action(() =>
				{
					left.Image = leftIm;
					right.Image = rightIm;
					disparity.Image = generator.CalculateDisparity(leftIm, rightIm, settings.Options);

					var middleX = disparity.Image.Width / 2;
					var middleY = disparity.Image.Height / 2;
					var distanceCalculator = new DistanceCalculator((Bitmap)disparity.Image);

					distances.UpdateDistanceToCenter(distanceCalculator.GetRawDistance(middleX, middleY));

				}));
		}

		private ImageProvider provider;
		private readonly DisparityGenerator generator;
		private readonly Settings settings;
		private readonly Distances distances;
		private byte[] Colors;
	}
}
