using System;
using System.Collections.Generic;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;
using OpenTKLib;
using Image = Leap.Image;

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

			Buffer = new List<Image<Gray, byte>>();

			provider.AddNewAction(ChangeImages);
			OnNewDisparityImage += CalculateDistance;
			OnNewDisparityImage += UpdateBuffer;
			//OnNewDisparityImage += UpdatePointCloud;

			imageForm.Visible = true;
			settingsForm.Visible = true;
			distanceForm.Visible = true;
			//GLTestForm.Visible = true;
		}

		private void ChangeImages(Bitmap[] images)
		{
			Console.WriteLine("Changing images.");
			var leftIm = images[0];
			var rightIm = images[1];
			var options = settingsForm.Options;
			var disparityIm = generator.CalculateDisparity(leftIm, rightIm, settingsForm.Options);

			imageForm.ChangeImages(new[] { leftIm, rightIm, disparityIm });
			Console.WriteLine("Images changed.");

			OnNewDisparityImage((Bitmap)disparityIm.Clone());
		}

		private void CalculateDistance(Bitmap disparityIm)
		{

			var middleX = disparityIm.Width / 2;
			var middleY = disparityIm.Height / 2;

			calculator = new DistanceCalculator(disparityIm);
			distanceForm.UpdateDistanceToCenter(calculator.GetDistance(middleX, middleY));
		}

		private void UpdateBuffer(Bitmap disparityIm)
		{
			Buffer.Add(new Image<Gray, byte>(disparityIm));

			if (Buffer.Count >= N)
			{
				Average = GetAverage(Buffer);
				Buffer = new List<Image<Gray, byte>>();

				CvInvoke.cvShowImage("Average",Average);
				CvInvoke.cvWaitKey(600);
			}
		}

		private void UpdatePointCloud(Bitmap disparityIm)
		{
			var vertices = calculator.ToVertexList();
			var colors = new byte[vertices.Count];
			for (var i = 0; i < colors.Length; i++)
				colors[i] = 240;
			GLTestForm.ShowListOfVertices(vertices, colors);
		}

		private Image<Gray, byte> GetAverage(List<Image<Gray, byte>> images)
		{
			var height = images[0].Height;
			var width = images[0].Width;
			var res = new byte[height, width, 1];
			foreach (var image in images)
			{
				for (int j = 0; j < height; j++)
				{
					for (int i = 0; i < width; i++)
					{
						res[j, i, 0] += (byte)(image.Data[j, i, 0] / N);
					}
				}
			}
			return new Image<Gray, byte>(res);
		}

		private Action<Bitmap> OnNewDisparityImage;

		private List<Image<Gray, byte>> Buffer;
		private Image<Gray, byte> Average;

		private Settings settingsForm;
		private ImageForm imageForm;
		private Distances distanceForm;
		private OpenTKTestForm GLTestForm;

		private DisparityGenerator generator;
		private ImageProvider provider;
		private DistanceCalculator calculator;

		private const int N = 2;
	}
}
