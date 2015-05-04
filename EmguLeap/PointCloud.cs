using Emgu.CV;
using Emgu.CV.Structure;

namespace EmguLeap
{
	public class PointCloud
	{
		private readonly MCvPoint3D32f[] Map3D;
		private readonly int Height;

		public PointCloud(Image<Gray, byte> disparity, Matrix<double> matrixQ)
		{
			Map3D = PointCollection.ReprojectImageTo3D(disparity, matrixQ);
			Height = disparity.Height;
		}

		private double GetPoint(int x, int y)
		{
			return Map3D[x*Height + y].z;
		}

		public MeasuredDistance this[int x, int y]
		{
			get { return new MeasuredDistance(GetPoint(x, y)); }
		}
	}
}
