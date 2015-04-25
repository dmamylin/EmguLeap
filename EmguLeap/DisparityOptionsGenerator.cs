using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmguLeap
{
	static class DisparityOptionsGenerator
	{
		public static DisparityOptions GetOptions()
		{
			return new DisparityOptions(64, 20, 13, 1, 5, 15, 7 * 16, 48);
		}
	}
}
