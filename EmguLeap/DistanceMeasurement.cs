using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmguLeap
{
	public class DistanceMeasurement
	{
		private readonly double RawDistance;
		public double Raw { get { return RawDistance; } }

		public double Centimeters
		{
			get { return RawDistance * 20.408f + 4.592f; }
		}

		public DistanceMeasurement(double rawDistance)
		{
			RawDistance = rawDistance;
		}


	}
}
