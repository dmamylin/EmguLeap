using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;

namespace EmguLeap
{
	public class DistanceCalculator
	{
		private readonly MCvPoint3D32f[] Map3D;
		private readonly int ImageHeight;
		private readonly int ImageWidth;

		public DistanceCalculator(Bitmap disparityMap)
		{
			var xDoc = XDocument.Load("..\\..\\CalibrationData\\Q.xml");
			var matrixQ = Toolbox.XmlDeserialize<Matrix<double>>(xDoc);
			var disparityImage = new Image<Gray, byte>(disparityMap);

			ImageHeight = disparityImage.Height;
			ImageWidth = disparityImage.Width;

			Map3D = PointCollection.ReprojectImageTo3D(disparityImage, matrixQ);
		}

		public void ConvertToObj(string filename)
		{
			var outFile = new StreamWriter(filename);

			foreach (var point in Map3D)
				outFile.WriteLine("v " + point.x + " " + point.y + " " + point.z);

			outFile.Close();
		}

		public float GetDistance(int x, int y)
		{
			return Map3D[y*ImageWidth + x].z;
		}
	}
}
