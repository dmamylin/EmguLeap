using System.Windows.Forms;

namespace EmguLeap
{
	public static class Extensions
	{
		public static int GetData(this TrackBar trackBar)
		{
			int res = 0;
			if (trackBar.InvokeRequired)
			{
				trackBar.Invoke(new MethodInvoker(delegate { res = trackBar.Value; }));
			}
			else
			{
				res = trackBar.Value;
			}
			return res;
		}
	}
}
