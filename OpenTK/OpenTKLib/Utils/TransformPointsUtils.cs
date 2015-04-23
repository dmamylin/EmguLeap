// Pogramming by
//     Douglas Andrade ( http://www.cmsoft.com.br, email: cmsoft@cmsoft.com.br)
//               Implementation of most of the functionality
//     Edgar Maass: (email: maass@logisel.de)
//               Code adaption, changed to user control
//
//Software used: 
//    OpenGL : http://www.opengl.org
//    OpenTK : http://www.opentk.com
//
// DISCLAIMER: Users rely upon this software at their own risk, and assume the responsibility for the results. Should this software or program prove defective, 
// users assume the cost of all losses, including, but not limited to, any necessary servicing, repair or correction. In no event shall the developers or any person 
// be liable for any loss, expense or damage, of any type or nature arising out of the use of, or inability to use this software or program, including, but not
// limited to, claims, suits or causes of action involving alleged infringement of copyrights, patents, trademarks, trade secrets, or unfair competition. 
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace OpenTKLib
{
    public class TransformPointsUtils
    {
        public static List<Vector3d> CalculatePointsShiftedByCentroid(List<Vector3d> a)
        {
            Vector3d centroid = CalculateCentroid(a);
            List<Vector3d> b = CalculatePointsShiftedByCentroid(a, centroid);
            return b;
            
        }
        public static List<Vector3d> CalculatePointsShiftedByCentroid(List<Vector3d> a, Vector3d centroid)
        {
            
            List<Vector3d> b = new List<Vector3d>();
            for (int i = 0; i < a.Count; i++)
            {
                Vector3d v = a[i];
                Vector3d vNew = new Vector3d(v.X - centroid.X, v.Y - centroid.Y, v.Z - centroid.Z);
                b.Add(vNew);

            }
            return b;

        }
        public static Matrix3d CalculateCorrelationMatrix(List<Vector3d> b, List<Vector3d> a)
        {
            //consists of elementx 
            //axbx axby axbz
            //aybx ayby aybz
            //azbx azby azbz
            Matrix3d H = new Matrix3d();
            for (int i = 0; i < b.Count; i++)
            {
                //H[0, 0] += b[i].X * a[i].X;
                //H[1, 0] += b[i].X * a[i].Y;
                //H[2, 0] += b[i].X * a[i].Z;

                //H[0, 1] += b[i].Y * a[i].X;
                //H[1, 1] += b[i].Y * a[i].Y;
                //H[2, 1] += b[i].Y * a[i].Z;

                //H[0, 2] += b[i].Z * a[i].X;
                //H[1, 2] += b[i].Z * a[i].Y;
                //H[2, 2] += b[i].Z * a[i].Z;

                H[0, 0] += b[i].X * a[i].X;
                H[0, 1] += b[i].X * a[i].Y;
                H[0, 2] += b[i].X * a[i].Z;

                H[1, 0] += b[i].Y * a[i].X;
                H[1, 1] += b[i].Y * a[i].Y;
                H[1, 2] += b[i].Y * a[i].Z;

                H[2, 0] += b[i].Z * a[i].X;
                H[2, 1] += b[i].Z * a[i].Y;
                H[2, 2] += b[i].Z * a[i].Z;


            }
            H = MatrixUtilsOpenTK.MultiplyScalar3D(H, 1.0D / b.Count);
            return H;
        }
        public static double[,] DoubleArrayFromMatrix(Matrix3d m)
        {
            double[,] doubleArray = new double[3,3];
            for(int i = 0; i < 3; i++)
                for(int j = 0; j < 3; j++)
                    doubleArray[i,j] = m[i,j];

            return doubleArray;
        }
        public static double[,] DoubleArrayFromMatrix(Matrix2d m)
        {
            double[,] doubleArray = new double[2, 2];
            for (int i = 0; i < 2; i++)
                for (int j = 0; j < 2; j++)
                    doubleArray[i, j] = m[i, j];

            return doubleArray;
        }
        public static double[,] GetTransformMatrixAsDoubleArray(Matrix4d matrix)
        {
            double[,] matrixAsDouble = new double[4, 4];


            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    matrixAsDouble[i, j] = matrix[i, j];
                }
            }

            return matrixAsDouble;

        }
        public static float[,] GetTransformMatrixAsFloatArray(Matrix4d matrix)
        {
            float[,] matrixAsDouble = new float[4, 4];


            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    matrixAsDouble[i, j] = Convert.ToSingle(matrix[i, j]);
                }
            }

            return matrixAsDouble;

        }
     
        public static Vector3d CalculateCentroid(List<Vector3d> pointsTarget)
        {
            
            
            Vector3d centroid = new Vector3d();
            for(int i = 0; i < pointsTarget.Count; i++)
            {
                Vector3d v = pointsTarget[i];
                centroid.X += v.X;
                centroid.Y += v.Y;
                centroid.Z += v.Z;

                
            }
            centroid.X /= pointsTarget.Count;
            centroid.Y /= pointsTarget.Count;
            centroid.Z /= pointsTarget.Count;
            
            return centroid;

        }
    
    }
}
