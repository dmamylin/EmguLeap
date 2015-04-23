using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using OpenTKLib;
using OpenTK;

namespace UnitTestsOpenTK
{
    [TestFixture]
    [Category("UnitTest")]
    public class MathTest
    {
         private static string path;
         public MathTest()
        {
            path = AppDomain.CurrentDomain.BaseDirectory + "TestData";
            //string str = 

        }


         [Test]
         public void Matrices()
         {
             Matrix2d a = new Matrix2d(0.5,-.5, 1, -1);
             Matrix2d b = new Matrix2d(1, -1, 2,-2);
             Matrix2d c = Matrix2d.Mult(a, b);


             double[,] Harray = TransformPointsUtils.DoubleArrayFromMatrix(a);
             double[,] Uarray = new double[2, 2];
             double[,] VTarray = new double[2, 2];
             double[] eigenvalues = new double[2];


             //trial 3:
             alglib.svd.rmatrixsvd(Harray, 2, 2, 2, 2, 2, ref eigenvalues, ref Uarray, ref VTarray);


             Matrix2d U = MatrixUtilsOpenTK.DoubleArrayToMatrix2d(Uarray);
             Matrix2d UT = Matrix2d.Transpose(U);
             c = Matrix2d.Mult(U,UT);//should give I Matrix
             Matrix2d VT = MatrixUtilsOpenTK.DoubleArrayToMatrix2d(VTarray);
             Matrix2d V = Matrix2d.Transpose(VT);
             c = Matrix2d.Mult(V, VT);//should give I Matrix
             //check solution

             Matrix2d checkShouldGiveI = Matrix2d.Mult(U, VT);
             Matrix2d R = Matrix2d.Mult(U, VT);
             Matrix2d RT = Matrix2d.Transpose(R);

             c = Matrix2d.Mult(RT, R);//should give I Matrix

          
         }
     
    }
}
