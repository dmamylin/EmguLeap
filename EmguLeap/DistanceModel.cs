using System;
using System.Collections.Generic;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;

namespace EmguLeap
{
	class DistanceModel
	{
		private readonly Action<Bitmap> OnNewDisparityImage;
		private readonly Action OnNewAverageImage;

		private List<Image<Gray, byte>> Buffer = new List<Image<Gray, byte>>();
		private Image<Gray, byte> Average;

		private readonly DistanceSensorForm DistanceForm;

		private readonly DisparityGenerator Generator;
		private readonly DistanceCalculator Calculator;
		private readonly CalibrationMatrixLoader MatrixLoader;
		private readonly ImageProvider Provider;

		private const int N = 1;
		
		public DistanceModel()
		{
			DistanceForm = new DistanceSensorForm();

			MatrixLoader = new CalibrationMatrixLoader();
			Provider = new ImageProvider();
			Generator = new DisparityGenerator(MatrixLoader);
			Calculator = new DistanceCalculator();

			Provider.AddNewAction(UpdateDisparity);
			OnNewDisparityImage += UpdateBuffer;
			OnNewAverageImage += CalculateDistance;

			DistanceForm.Visible = true;
		}

		private void UpdateDisparity(Bitmap[] images)
		{
			var leftIm = images[0];
			var rightIm = images[1];
			var disparityIm = Generator.CalculateDisparity(leftIm, rightIm, DisparityOptionsGenerator.GetOptions());

			OnNewDisparityImage((Bitmap)disparityIm.Clone());
		}

		private void CalculateDistance()
		{
			var angle = DistanceForm.GetAngle();
			var average = Average.ToBitmap();
			var imageWithLine = DrawThinRedLine(average, angle);
			DistanceForm.ChangeImage(imageWithLine);
			Calculator.UpdateImage(average);
			var cmDistance = Calculator.GetCmDistanceByAngle(angle, Calculator.AverageFilterAdaptive);
			var rawDistance = Calculator.GetRawDistanceByAngle(angle, Calculator.AverageFilterAdaptive);

			DistanceForm.ChangeDistance(cmDistance, rawDistance);
		}

		private Bitmap DrawThinRedLine(Bitmap image, double angle)
		{
			var dx = (int)Math.Floor(image.Width * angle / 150);
			var x = image.Width / 2 + dx;
			var tempBitmap = new Bitmap(image.Width, image.Height);
			using (Graphics g = Graphics.FromImage(tempBitmap))
			{
				g.DrawImage(image, 0, 0);
				g.DrawLine(Pens.Red, x, 0, x, image.Height);
			}
			return tempBitmap;
		}

		private void UpdateBuffer(Bitmap disparityIm)
		{
			Buffer.Add(new Image<Gray, byte>(disparityIm));

			if (Buffer.Count >= N)
			{
				Average = GetAverage(Buffer);
				Buffer = new List<Image<Gray, byte>>();
				OnNewAverageImage();
			}
		}

		// TODO: WTF? Divided by 1? Seriously???
		private Image<Gray, byte> GetAverage(List<Image<Gray, byte>> images)
		{
			var height = images[0].Height;
			var width = images[0].Width;
			var res = new byte[height, width, 1];
			foreach (var image in images)
			{
				for (var j = 0; j < height; j++)
				{
					for (var i = 0; i < width; i++)
					{
						res[j, i, 0] += (byte)(image.Data[j, i, 0] / N); // Hey, TODO! Yeah, exactly!
					}
				}
			}
			return new Image<Gray, byte>(res);
		}
	}
}
