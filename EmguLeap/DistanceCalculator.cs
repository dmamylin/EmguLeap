using System;
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

		private float DistanceInCm(float rawDistance)
		{
			return rawDistance*20.408f + 4.592f;
		}

		public float GetDistance(int x, int y)
		{
			//return Map3D[y*ImageWidth + x].z*20.408f + 4.592f;
			return DistanceInCm(Map3D[y*ImageWidth + x].z);
		}

		public float GetDistance(Point point)
		{
			return GetDistance(point.X, point.Y);
		}

		public float GetDistanceToRectangle(Point middlePoint, int radius)
		{
			var upperLeftCorner = new Point(middlePoint.X - radius, middlePoint.Y - radius);
			var bottomRightCorner = new Point(middlePoint.X + radius, middlePoint.Y + radius);

			return GetDistanceToRectangle(upperLeftCorner, bottomRightCorner);
		}

		public float GetDistanceToRectangle(Point upperLeftCorner, Point bottomRightCorner)
		{
			var sumOfDistances = 0.0f;

			var startX = Math.Max(upperLeftCorner.X, 0);
			var startY = Math.Max(upperLeftCorner.Y, 0);

			var endX = Math.Min(bottomRightCorner.X, ImageWidth);
			var endY = Math.Min(bottomRightCorner.Y, ImageHeight);

			var totalCount = (endX - startX)*(endY - startY);

			for (var x = startX; x < endX; x++)
				for (var y = startY; y < endY; y++)
					sumOfDistances += GetDistance(x, y);

			// Filtering by average
			return DistanceInCm(sumOfDistances/totalCount);
		}
	}
}
