using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Xml.Linq;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;
using OpenTKLib;

namespace EmguLeap
{
	public class DistanceCalculator
	{
		private readonly MCvPoint3D32f[] Map3D;
		private readonly int ImageHeight;
		private readonly int ImageWidth;

		public DistanceCalculator(Bitmap disparityMap, MatrixLoader matrixLoader)
		{
			var matrixQ = matrixLoader.Q;
			var disparityImage = new Image<Gray, byte>(disparityMap);

			ImageHeight = disparityImage.Height;
			ImageWidth = disparityImage.Width;

			Map3D = PointCollection.ReprojectImageTo3D(disparityImage, matrixQ);
		}

		public void ConvertToObj(string filename)
		{
			var outFile = new System.IO.StreamWriter(filename);

			foreach (var point in Map3D)
				outFile.WriteLine("v " + point.x + " " + point.y + " " + point.z);

			outFile.Close();
		}

		public List<Vertex> ToVertexList()
		{
			return Map3D.Select(point => new Vertex(point.x, point.y, point.z)).ToList();
		}

		public float GetDistance(int x, int y)
		{
			return Map3D[y*ImageWidth + x].z;
		}
	}
}
