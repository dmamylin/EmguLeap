using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Emgu.CV;
using Emgu.CV.Structure;
using OpenTKLib;

namespace EmguLeap
{
	public class PointCloud
	{
		private readonly MCvPoint3D32f[] Points;
		private readonly int Width;
		private readonly int Height;

		public PointCloud(Bitmap disparityMap, Matrix<double> matrixQ)
		{
			var disparityImage = new Image<Gray, byte>(disparityMap);
			Width = disparityImage.Width;
			Height = disparityImage.Height;

			Points = PointCollection.ReprojectImageTo3D(disparityImage, matrixQ);
		}

		public List<Vertex> ToVertexList()
		{
			return Points.Select(point => new Vertex(point.x, point.y, point.z)).ToList();
		}

		public MCvPoint3D32f this[int i, int j]
		{
			get { return Points[j*Width + i]; }
		}
	}
}
