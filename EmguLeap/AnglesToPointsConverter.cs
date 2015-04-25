using System.Drawing;

namespace EmguLeap
{
	public class AnglesToPointsConverter
	{
		private readonly double Longitude;
		private readonly double Latitude;

		public Point ResultedPoint { get; private set; }

		public AnglesToPointsConverter(double longitude, double latitude)
		{
			Longitude = longitude;
			Latitude = latitude;

			ResultedPoint = CalculateCoordinates();
		}

		private Point CalculateCoordinates()
		{
			
		}

		public Point GetPoint()
		{
			return ResultedPoint;
		}
	}
}
