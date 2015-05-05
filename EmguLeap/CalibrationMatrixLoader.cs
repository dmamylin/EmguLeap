using System.Xml.Linq;
using Emgu.CV;
using Emgu.Util;

namespace EmguLeap
{
	public class CalibrationMatrixLoader
	{
		public Matrix<double> Q { get; private set; }
		public Matrix<double> C1 { get; private set; }
		public Matrix<double> C2 { get; private set; }
		public Matrix<double> D1 { get; private set; }
		public Matrix<double> D2 { get; private set; }
		public Matrix<double> R1 { get; private set; }
		public Matrix<double> R2 { get; private set; }
		public Matrix<double> P1 { get; private set; }
		public Matrix<double> P2 { get; private set; }

		public CalibrationMatrixLoader()
		{
			var xDoc = XDocument.Load("..\\..\\CalibrationData\\Q.xml");
			Q = Toolbox.XmlDeserialize<Matrix<double>>(xDoc);

			xDoc = XDocument.Load("..\\..\\CalibrationData\\C1.xml");
			C1 = Toolbox.XmlDeserialize<Matrix<double>>(xDoc);

			xDoc = XDocument.Load("..\\..\\CalibrationData\\C2.xml");
			C2 = Toolbox.XmlDeserialize<Matrix<double>>(xDoc);
			
			xDoc = XDocument.Load("..\\..\\CalibrationData\\D1.xml");
			D1 = Toolbox.XmlDeserialize<Matrix<double>>(xDoc);

			xDoc = XDocument.Load("..\\..\\CalibrationData\\D2.xml");
			D2 = Toolbox.XmlDeserialize<Matrix<double>>(xDoc);

			xDoc = XDocument.Load("..\\..\\CalibrationData\\R1.xml");
			R1 = Toolbox.XmlDeserialize<Matrix<double>>(xDoc);

			xDoc = XDocument.Load("..\\..\\CalibrationData\\R2.xml");
			R2 = Toolbox.XmlDeserialize<Matrix<double>>(xDoc);

			xDoc = XDocument.Load("..\\..\\CalibrationData\\P1.xml");
			P1 = Toolbox.XmlDeserialize<Matrix<double>>(xDoc);

			xDoc = XDocument.Load("..\\..\\CalibrationData\\P2.xml");
			P2 = Toolbox.XmlDeserialize<Matrix<double>>(xDoc);
		}
	}
}
