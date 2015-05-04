using Leap;

namespace EmguLeap
{
	// TODO: inherit from DoubleBasedScalar
	public class MeasuredDistance
	{
		public double Raw { get; private set; }
		public double Centimeters { get { return RawToCm(Raw); } }

		public MeasuredDistance(double rawDistance)
		{
			Raw = rawDistance;
		}

		// TODO: approximate a function
		private double RawToCm(double raw)
		{
			return 0.0;
		}
	}
}
