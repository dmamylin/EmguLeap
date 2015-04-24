using System.Drawing;
using Emgu.CV;

namespace EmguLeap
{
	public class DistanceCalculator
	{
		private readonly PointCloud PointCloud;

		public DistanceCalculator(Bitmap disparityMap, Matrix<double> matrixQ)
		{
			PointCloud = new PointCloud(disparityMap, matrixQ);
		}

		public float GetDistanceToPoint(Point point)
		{
			return PointCloud[point.X, point.Y].z;
		}
	}
}
