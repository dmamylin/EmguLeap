﻿using System;
using System.Collections.Generic;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;

namespace EmguLeap
{
	class DistanceModel
	{
		public DistanceModel()
		{
			distanceForm = new DistanceSensorForm();

			provider = new ImageProvider();
			generator = new DisparityGenerator();

			provider.AddNewAction(UpdateDisparity);
			OnNewDisparityImage += UpdateBuffer;
			OnNewAverageImage += CalculateDistance;

			distanceForm.Visible = true;
		}

		private void UpdateDisparity(Bitmap[] images)
		{
			var leftIm = images[0];
			var rightIm = images[1];
			var disparityIm = generator.CalculateDisparity(leftIm, rightIm, DisparityOptionsGenerator.GetOptions());

			OnNewDisparityImage((Bitmap)disparityIm.Clone());
		}

		private void CalculateDistance()
		{
			var angle = distanceForm.GetAngle();
			calculator.GetDistanceByAngle(angle, calculator.AverageFilter);
		}

		private void UpdateBuffer(Bitmap disparityIm)
		{
			Buffer.Add(new Image<Gray, byte>(disparityIm));

			if (Buffer.Count >= N)
			{
				Average = GetAverage(Buffer);
				Buffer = new List<Image<Gray, byte>>();
				distanceForm.ChangeImage(Average.ToBitmap());
				OnNewAverageImage();
			}
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
		private Action OnNewAverageImage;

		private List<Image<Gray, byte>> Buffer =  new List<Image<Gray, byte>>();
		private Image<Gray, byte> Average;

		private DistanceSensorForm distanceForm;

		private DisparityGenerator generator;
		private ImageProvider provider;
		private DistanceCalculator calculator;

		private const int N = 2;
	}
}
