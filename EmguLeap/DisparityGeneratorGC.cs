using System;
using System.Diagnostics;
using System.Drawing;
using System.Xml.Linq;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;

namespace EmguLeap
{
	public class DisparityGeneratorGC
	{
		public DisparityGeneratorGC()
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

		public Tuple<Bitmap, Bitmap> CalculateDisparity(Bitmap leftRaw, Bitmap rightRaw)
		{
			var sw = new Stopwatch();
			sw.Start();
			var left = new Image<Gray, byte>(new Size(640, 240));
			var right = new Image<Gray, byte>(new Size(640, 240));
			CvInvoke.cvRemap(new Image<Gray, byte>(leftRaw), left, mapx1, mapy1, 1, new MCvScalar(0));
			CvInvoke.cvRemap(new Image<Gray, byte>(rightRaw), right, mapx2, mapy2, 1, new MCvScalar(0));

			Size size = left.Size;
			var disparityMapLeft = new Image<Gray, short>(size);
			var disparityMapRight = new Image<Gray, short>(size);

			using (var solver = new StereoGC(40,3))
			{
				solver.FindStereoCorrespondence(new Image<Gray, byte>(leftRaw), new Image<Gray, byte>(rightRaw), disparityMapLeft, disparityMapRight);
			}
			sw.Stop();
			Console.WriteLine("GC time: {0}",sw.ElapsedMilliseconds);
			return Tuple.Create(disparityMapLeft.ToBitmap(), disparityMapRight.ToBitmap());
		}

		private Matrix<float> mapx1;
		private Matrix<float> mapy1;
		private Matrix<float> mapx2;
		private Matrix<float> mapy2;
	}
}
