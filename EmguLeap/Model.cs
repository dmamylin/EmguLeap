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
			distanceForm = new Distances();
			//GLTestForm = new OpenTKTestForm();

			provider = new ImageProvider();
			generator = new DisparityGenerator();

			provider.AddNewAction(ChangeImages);
			OnNewDisparityImage += CalculateDistance;

			imageForm.Visible = true;
			settingsForm.Visible = true;
			distanceForm.Visible = true;
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

			OnNewDisparityImage((Bitmap)disparityIm.Clone());

			//		var middleX = disparity.Image.Width / 2;
			//		var middleY = disparity.Image.Height / 2;

			//		var distanceCalculator = new DistanceCalculator((Bitmap)disparity.Image);
			//		distances.UpdateDistanceToCenter(distanceCalculator.GetDistance(middleX, middleY));

			//		var vertices = distanceCalculator.ToVertexList();
			//		var colors = new byte[vertices.Count];
			//		for (var i = 0; i < colors.Length; i++)
			//			colors[i] = 240;
			//		GLTestForm.ShowListOfVertices(vertices, colors);
		}

		private void CalculateDistance(Bitmap disparityIm)
		{

			var middleX = disparityIm.Width / 2;
			var middleY = disparityIm.Height / 2;

			var distanceCalculator = new DistanceCalculator(disparityIm);
			distanceForm.UpdateDistanceToCenter(distanceCalculator.GetDistance(middleX, middleY));
		}

		private Action<Bitmap> OnNewDisparityImage;

		private Settings settingsForm;
		private ImageForm imageForm;
		private Distances distanceForm;
		private OpenTKTestForm GLTestForm;

		private DisparityGenerator generator;
		private ImageProvider provider;

	}
}
