using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;
using Leap;

namespace EmguLeap
{
	public class DistanceCalculator
	{
		private readonly MCvPoint3D32f[] Map3D;
		private readonly int ImageHeight;
		private readonly int ImageWidth;
		private const float MaxZ = 2.0f;

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

		private float RawDistanceToCm(float rawDistance)
		{
			return rawDistance*20.408f + 4.592f;
		}

		public float GetRawDistance(int x, int y)
		{
			return Map3D[y*ImageWidth + x].z;
		}

		public float GetRawDistance(Point point)
		{
			return GetRawDistance(point.X, point.Y);
		}

		public float GetCmDistance(int x, int y)
		{
			return RawDistanceToCm(GetRawDistance(x, y));
		}

		public float GetCmDistance(Point point)
		{
			return RawDistanceToCm(GetRawDistance(point));
		}

		public float GetDistanceToRectangleAverageFilter(Point middlePoint, int radius)
		{
			var upperLeftCorner = new Point(middlePoint.X - radius, middlePoint.Y - radius);
			var bottomRightCorner = new Point(middlePoint.X + radius, middlePoint.Y + radius);

			return GetDistanceToRectangle(upperLeftCorner, bottomRightCorner, AverageFilter);
		}

		public float GetDistanceToRectangle(Point upperLeftCorner, Point bottomRightCorner, Func<IterationRange2D, float> filter)
		{
			var startX = Math.Max(upperLeftCorner.X, 0);
			var startY = Math.Max(upperLeftCorner.Y, 0);

			var endX = Math.Min(bottomRightCorner.X, ImageWidth);
			var endY = Math.Min(bottomRightCorner.Y, ImageHeight);

			var distance = filter(new IterationRange2D(new Point(startX, endX), new Point(startY, endY)));

			return RawDistanceToCm(distance);
		}

		public float AverageFilter(IterationRange2D iterationRange)
		{
			var sumOfDistances = 0.0f;

			for (var x = iterationRange.StartX; x < iterationRange.EndX; x++)
				for (var y = iterationRange.StartY; y < iterationRange.EndY; y++)
					sumOfDistances += Math.Min(GetRawDistance(x, y), MaxZ);

			return sumOfDistances/iterationRange.TotalCount;
		}

		float GetDistanceByAngles(double latitude, double longitude)
		{
			// TODO: validate angles

			var midX = ImageWidth/2;
			var midY = ImageHeight/2;
			var dx = (int)Math.Floor(midX*latitude);
			var dy = (int)Math.Floor(midY*longitude);

			return GetCmDistance(new Point(midX - dx, midY + dy));
		}

		public class IterationRange2D
		{
			private readonly Point RangeX;
			private readonly Point RangeY;

			public int StartX { get { return RangeX.X; } }
			public int EndX { get { return RangeX.Y; } }
			public int StartY { get { return RangeY.X; } }
			public int EndY { get { return RangeY.Y;} }

			public int TotalCount { get { return Math.Abs((EndX - StartX)*(EndY - StartY)); } }

			public IterationRange2D(Point rangeX, Point rangeY)
			{
				RangeX = rangeX;
				RangeY = rangeY;
			}
		}
	}
}
