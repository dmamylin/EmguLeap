using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;

namespace EmguLeap
{
	class Program
	{
		static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			var model = new Model();
			Application.Run();

			//var generator = new DisparityGeneratorGC();
			//for (int i = 0; i < 20; i++)
			//{
			//	var leftRaw = new Bitmap(string.Format("..\\..\\img\\left{0}.bmp", i));
			//	var rightRaw = new Bitmap(string.Format("..\\..\\img\\right{0}.bmp", i));
			//	var res = generator.CalculateDisparity(leftRaw, rightRaw);
			//	CvInvoke.cvShowImage("leftRaw", new Image<Gray, byte>(leftRaw));
			//	CvInvoke.cvShowImage("rightRaw", new Image<Gray, byte>(rightRaw));
			//	CvInvoke.cvShowImage("left", new Image<Gray, byte>(res.Item1));
			//	CvInvoke.cvShowImage("right", new Image<Gray, byte>(res.Item2));
			//	CvInvoke.cvWaitKey(-1);
			//}
		}
	}
}
