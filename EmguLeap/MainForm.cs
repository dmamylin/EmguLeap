using System;
using System.Drawing;
using System.Windows.Forms;
using OpenTKLib;

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
			GLTestForm = new OpenTKTestForm();
			provider.AddNewAction(ChangeImages);

			InitializeComponent();
			settings.Visible = true;
			distances.Visible = true;
			GLTestForm.Visible = true;
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
					disparity.Image = generator.CalculateDisparity(leftIm, rightIm, settings.GetOptions());

					var middleX = disparity.Image.Width/2;
					var middleY = disparity.Image.Height/2;

					var distance = new DistanceCalculator((Bitmap)disparity.Image).GetDistance(middleX, middleY);
					distances.UpdateDistanceToCenter(distance);
				}));
		}

		private ImageProvider provider;
		private readonly DisparityGenerator generator;
		private readonly Settings settings;
		private readonly Distances distances;
		private readonly OpenTKTestForm GLTestForm;
	}
}
