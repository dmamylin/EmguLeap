using System.Windows.Forms;

namespace EmguLeap
{
	class Program
	{
		static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			var model = new DistanceModel();
			Application.Run();
		}
	}
}
