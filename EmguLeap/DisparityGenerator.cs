using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.Util;

namespace EmguLeap
{
	class DisparityGenerator
	{
		public DisparityGenerator()
		{
			XDocument xDoc;
			xDoc = XDocument.Load("..\\..\\CalibrationData\\Q.xml");
			var Q = Toolbox.XmlDeserialize<Matrix<double>>(xDoc);
			xDoc = XDocument.Load("..\\..\\CalibrationData\\C1.xml");
			var C1 = Toolbox.XmlDeserialize<Matrix<double>>(xDoc);
			xDoc = XDocument.Load("..\\..\\CalibrationData\\C2.xml");
			var C2 = Toolbox.XmlDeserialize<Matrix<double>>(xDoc);
			xDoc = XDocument.Load("..\\..\\CalibrationData\\D1.xml");
			var D1 = Toolbox.XmlDeserialize<Matrix<double>>(xDoc);
			xDoc = XDocument.Load("..\\..\\CalibrationData\\D2.xml");
			var D2 = Toolbox.XmlDeserialize<Matrix<double>>(xDoc);
			xDoc = XDocument.Load("..\\..\\CalibrationData\\R1.xml");
			var R1 = Toolbox.XmlDeserialize<Matrix<double>>(xDoc);
			xDoc = XDocument.Load("..\\..\\CalibrationData\\R2.xml");
			var R2 = Toolbox.XmlDeserialize<Matrix<double>>(xDoc);
			xDoc = XDocument.Load("..\\..\\CalibrationData\\P1.xml");
			var P1 = Toolbox.XmlDeserialize<Matrix<double>>(xDoc);
			xDoc = XDocument.Load("..\\..\\CalibrationData\\P2.xml");
			var P2 = Toolbox.XmlDeserialize<Matrix<double>>(xDoc);

			mapx1 = new Matrix<float>(240, 640);
			mapy1 = new Matrix<float>(240, 640);
			mapx2 = new Matrix<float>(240, 640);
			mapy2 = new Matrix<float>(240, 640);
			CvInvoke.cvInitUndistortRectifyMap(C1, D1, R1, P1, mapx1, mapy1);
			CvInvoke.cvInitUndistortRectifyMap(C2, D2, R2, P2, mapx2, mapy2);
		}

		public Bitmap CalculateDisparity(Bitmap leftRaw, Bitmap rightRaw, DisparityOptions options)
		{
			var left = new Image<Gray, byte>(new Size(640, 240));
			var right = new Image<Gray, byte>(new Size(640, 240));
			CvInvoke.cvRemap(new Image<Gray, byte>(leftRaw), left, mapx1, mapy1,1, new MCvScalar(0));
			CvInvoke.cvRemap(new Image<Gray, byte>(rightRaw), right, mapx2, mapy2,1, new MCvScalar(0));
			
			Size size = left.Size;
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
			//using (var stereoSolver = new StereoSGBM(16, 160, 7, 1600, 2200, 96, 48, 0, 0, 4, StereoSGBM.Mode.SGBM))
			//{
			//	stereoSolver.FindStereoCorrespondence(left, right, disparityMap);
			//}

			//normalize(disp, disp8, 0, 255, CV_MINMAX, CV_8U);

			var newDisp = disparityMap.Clone();
			CvInvoke.cvNormalize(disparityMap, newDisp, 0, 255, NORM_TYPE.CV_MINMAX, IntPtr.Zero);
			//return disparityMap.ToBitmap();
			return newDisp.ToBitmap();


			//using (StereoSGBM solver = new StereoSGBM(-64,
			//	192,
			//	5,
			//	600,
			//	2400,
			//	10,
			//	4,
			//	1,
			//	150,
			//	2,
			//	StereoSGBM.Mode.SGBM))
		}

		private Matrix<float> mapx1;
		private Matrix<float> mapy1;
		private Matrix<float> mapx2;
		private Matrix<float> mapy2;
	}

	public class DisparityOptions
	{
		public DisparityOptions(int numD, int minD, int sad, int disp12, int preFilter, int uniqRatio, int speckle,
			int speckleRange)
		{

			numDisparities = numD;
			minDispatities = minD;
			SAD = sad;
			P1 = 8 * 1 * SAD * SAD;
			P2 = 32 * 1 * SAD * SAD;
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
