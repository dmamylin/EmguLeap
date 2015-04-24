using System;
using System.Drawing;

namespace EmguLeap
{
	class Model
	{
		private readonly Settings SettingsForm;
		private readonly ImageForm ImageForm;
		private readonly Distances DistancesForm;

		private readonly DisparityGenerator DisparityGenerator;
		private readonly ImageProvider ImageProvider;
		private readonly MatrixLoader MatrixLoader;

		public Model()
		{
			ImageForm = new ImageForm();
			SettingsForm = new Settings();
			DistancesForm = new Distances();

			ImageProvider = new ImageProvider();
			DisparityGenerator = new DisparityGenerator();
			MatrixLoader = new MatrixLoader();
		}

		public void Run()
		{
			ImageProvider.AddNewAction(ChangeImages);
			ImageProvider.AddNewAction(MeasureDistance);

			ImageForm.Visible = true;
			SettingsForm.Visible = true;
		}

		public void MeasureDistance(Bitmap[] images)
		{
			var disparity = DisparityGenerator.CalculateDisparity(images[0], images[1], SettingsForm.Options);
			var middlePoint = new Point(disparity.Width/2, disparity.Height/2);
			var distanceCalculator = new DistanceCalculator(disparity, MatrixLoader.Q);

			DistancesForm.UpdateDistanceToCenter(distanceCalculator.GetDistanceToPoint(middlePoint));

			/*var vertices = distanceCalculator.ToVertexList();
			var colors = new byte[vertices.Count];
			for (var i = 0; i < colors.Length; i++)
				colors[i] = 240;
			GLTestForm.ShowListOfVertices(vertices, colors);*/
		}

		public void ChangeImages(Bitmap[] images)
		{
			Console.WriteLine("Changing images.");
			var disparityIm = DisparityGenerator.CalculateDisparity(images[0], images[1], SettingsForm.Options);

			ImageForm.ChangeImages(new[] { images[0], images[1], disparityIm });
			Console.WriteLine("Images changed.");
		}
	}
}
