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

		private List<Image<Gray, byte>> Buffer = new List<Image<Gray, byte>>();

		private readonly DistanceSensorForm DistanceForm;

		private readonly DisparityGenerator Generator;
		private readonly CalibrationMatrixLoader MatrixLoader;
		private readonly ImageProvider Provider;

		private const int N = 1;
		
		public DistanceModel()
		{
			DistanceForm = new DistanceSensorForm();

			MatrixLoader = new CalibrationMatrixLoader();
			Provider = new ImageProvider();
			Generator = new DisparityGenerator(MatrixLoader);

			Provider.AddNewAction(UpdateDisparity);
			OnNewDisparityImage += PresentDisparityImage;

			DistanceForm.Visible = true;
		}

		private void UpdateDisparity(Bitmap[] images)
		{
			var leftIm = images[0];
			var rightIm = images[1];
			var disparityIm = Generator.CalculateDisparity(leftIm, rightIm, DisparityOptionsGenerator.GetOptions());

			OnNewDisparityImage((Bitmap)disparityIm.Clone());
		}

		private void PresentDisparityImage(Bitmap disparityImage)
		{
			DistanceForm.ChangeImage(disparityImage);
		}
	}
}
