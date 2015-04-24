using System;
using System.Drawing;
using System.Windows.Forms;
using OpenTKLib;

namespace EmguLeap
{
	class Model
	{
		public Model()
		{
			imageForm = new ImageForm();
			settingsForm = new Settings();
			//distancesForm = new Distances();
			//GLTestForm = new OpenTKTestForm();

			provider = new ImageProvider();
			generator = new DisparityGenerator();

			provider.AddNewAction(ChangeImages);

			imageForm.Visible = true;
			settingsForm.Visible = true;
		}

		public void ChangeImages(Bitmap[] images)
		{
			Console.WriteLine("Changing images.");
			var leftIm = images[0];
			var rightIm = images[1];
			var options = settingsForm.Options;
			var disparityIm = generator.CalculateDisparity(leftIm, rightIm, settingsForm.Options);

			imageForm.ChangeImages(new[] { leftIm, rightIm, disparityIm });
			Console.WriteLine("Images changed.");
			//if (IsHandleCreated)
			//	Invoke(new Action(() =>
			//	{
			//		left.Image = leftIm;
			//		right.Image = rightIm;
			//		disparity.Image = generator.CalculateDisparity(leftIm, rightIm, settings.GetOptions());

			//		var middleX = disparity.Image.Width / 2;
			//		var middleY = disparity.Image.Height / 2;

			//		var distanceCalculator = new DistanceCalculator((Bitmap)disparity.Image);
			//		distances.UpdateDistanceToCenter(distanceCalculator.GetDistance(middleX, middleY));

			//		var vertices = distanceCalculator.ToVertexList();
			//		var colors = new byte[vertices.Count];
			//		for (var i = 0; i < colors.Length; i++)
			//			colors[i] = 240;
			//		GLTestForm.ShowListOfVertices(vertices, colors);
			//	}));
		}

		private Settings settingsForm;
		private ImageForm imageForm;
		private Distances distancesForm;
		private OpenTKTestForm GLTestForm;

		private DisparityGenerator generator;
		private ImageProvider provider;

	}
}
