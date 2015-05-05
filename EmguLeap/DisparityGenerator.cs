using System;
using System.Diagnostics;
using System.Drawing;
using System.Xml.Linq;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;

namespace EmguLeap
{
	class DisparityGenerator
	{
		public DisparityGenerator(CalibrationMatrixLoader matrixLoader)
		{
			Mapx1 = new Matrix<float>(240, 640);
			Mapy1 = new Matrix<float>(240, 640);
			Mapx2 = new Matrix<float>(240, 640);
			Mapy2 = new Matrix<float>(240, 640);
			CvInvoke.cvInitUndistortRectifyMap(matrixLoader.C1, matrixLoader.D1, matrixLoader.R1, matrixLoader.P1, Mapx1, Mapy1);
			CvInvoke.cvInitUndistortRectifyMap(matrixLoader.C2, matrixLoader.D2, matrixLoader.R2, matrixLoader.P2, Mapx2, Mapy2);

			Lut = new Matrix<byte>(new Size(1, 256));
			for (var i = 0; i < 50; i++)
			{
				Lut[i, 0] = 0;
			}
			for (var i = 50; i < 256; i++)
			{
				Lut[i, 0] = (byte)i;
			}
		}

		public Bitmap CalculateDisparity(Bitmap leftRaw, Bitmap rightRaw, DisparityOptions options)
		{
			var sw = new Stopwatch();
			sw.Start();

			var left = new Image<Gray, byte>(new Size(640, 240));
			var right = new Image<Gray, byte>(new Size(640, 240));
			CvInvoke.cvRemap(new Image<Gray, byte>(leftRaw), left, Mapx1, Mapy1, 1, new MCvScalar(0));
			CvInvoke.cvRemap(new Image<Gray, byte>(rightRaw), right, Mapx2, Mapy2, 1, new MCvScalar(0));

			var size = left.Size;
			var disparityMap = new Image<Gray, short>(size);
			using (var stereoSolver = new StereoSGBM(options.minDispatities,
				options.numDisparities,
				options.SAD,
				options.P1,
				options.P2,
				options.disp12MaxDiff,
				options.PreFilterCap,
				options.UniquenessRatio,
				options.Speckle,
				options.SpeckleRange,
				StereoSGBM.Mode.SGBM))
			{
				stereoSolver.FindStereoCorrespondence(left, right, disparityMap);
			}



			sw.Stop();
			Console.WriteLine("{0} ms fo sgbm computation.", sw.ElapsedMilliseconds);

			var res = disparityMap.Convert<Gray, byte>();
			CvInvoke.cvLUT(res, res, Lut);
			return res.ToBitmap();

		}

		private readonly Matrix<float> Mapx1;
		private readonly Matrix<float> Mapy1;
		private readonly Matrix<float> Mapx2;
		private readonly Matrix<float> Mapy2;

		private readonly Matrix<byte> Lut;
	}

	public class DisparityOptions
	{
		public DisparityOptions(int numD, int minD, int sad, int disp12, int preFilter, int uniqRatio, int speckle,
			int speckleRange)
		{

			numDisparities = numD;
			minDispatities = minD;
			SAD = sad;
			P1 = SAD * SAD;
			P2 = 512 * 1 * SAD * SAD;
			disp12MaxDiff = disp12;
			PreFilterCap = preFilter;
			UniquenessRatio = uniqRatio;
			Speckle = speckle;
			SpeckleRange = speckleRange;
			fullDP = true;
		}

		public int numDisparities;
		public int minDispatities;
		public int SAD;
		public int P1;
		public int P2;
		public int disp12MaxDiff;
		public int PreFilterCap;
		public int UniquenessRatio;
		public int Speckle;
		public bool fullDP;
		public int SpeckleRange;
	}
}
