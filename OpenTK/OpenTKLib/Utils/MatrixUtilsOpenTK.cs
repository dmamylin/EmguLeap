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
    public class MatrixUtilsOpenTK
    {

        Matrix4 Matrix;


        public static Vector3d CrossProduct(Vector3d v1, Vector3d v2)
        {
            return new Vector3d()
            {
                X = v1.Y * v2.Z - v2.Y * v1.Z,
                Y = -v1.X * v2.Z + v2.X * v1.Z,
                Z = v1.X * v2.Y - v2.X * v1.Y
            };
        }

        public static double Norm(Vector3d v)
        {

            double val = v.X * v.X + v.Y * v.Y + v.Z * v.Z;

            return val;
        }

     
        public static Matrix3d MultiplyScalar3D(Matrix3d A, double val)
        {
            Matrix3d mReturn = new Matrix3d();
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    mReturn[i,j] = A[i, j] * val;
            return mReturn;
            
        }

        public static Matrix3d MultiplyScalar3F(Matrix3d A, float val)
        {
            Matrix3d matReturn = new Matrix3d( );
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    matReturn[i,j] = A[i, j] * val;
            return matReturn;
        }


        public static void RotateVectors(List<Vector3d> vectors, Matrix3d R)
        {
            for (int i = 0; i < vectors.Count; i++)
            {
                Vector3d v = vectors[i];
                vectors[i] = R.TransformVector(v);
               // Vector3d v1 = Multiply3x3(R, v);
                
            }
        }
        public static void RotateVertices(List<Vertex> vectors, Matrix3d R)
        {
            for (int i = 0; i < vectors.Count; i++)
            {
                Vertex v = vectors[i];
                vectors[i].Vector = R.TransformVector(v.Vector);

            }
        }
        public static double[] Multiply3x3(double[,] A, double[] v)
        {
            double[] u = new double[3];
            double x = A[0, 0] * v[0] + A[0, 1] * v[1] + A[0, 2] * v[2];
            double y = A[1, 0] * v[0] + A[1, 1] * v[1] + A[1, 2] * v[2];
            double z = A[2, 0] * v[0] + A[2, 1] * v[1] + A[2, 2] * v[2];

            u[0] = x;
            u[1] = y;
            u[2] = z;
            return u;
        }
        public static Vector3d Multiply3x3(Matrix3d A, Vector3d v)
        {
            Vector3d u = new Vector3d();
            u.X = A[0, 0] * v[0] + A[0, 1] * v[1] + A[0, 2] * v[2];
            u.Y = A[1, 0] * v[0] + A[1, 1] * v[1] + A[1, 2] * v[2];
            u.Z = A[2, 0] * v[0] + A[2, 1] * v[1] + A[2, 2] * v[2];

          
            return u;
        }
        public static Matrix3d DoubleArrayToMatrix3d( double[,] arr)
        {

            Matrix3d m = new Matrix3d( );
            for(int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    m[i,j] = arr[i,j];
           
            return m;
        }
        public static Matrix2d DoubleArrayToMatrix2d(double[,] arr)
        {

            Matrix2d m = new Matrix2d();
            for (int i = 0; i < 2; i++)
                for (int j = 0; j < 2; j++)
                    m[i, j] = arr[i, j];

            return m;
        }
        public static Matrix3d Matrix3dfromMatrix3d( Matrix3d a)
        {

            Matrix3d m = new Matrix3d();
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    m[i, j] = a[i, j];

            return m;
        }

        public static float[,] MatrixToFloatArray(Matrix4 myMatrix)
        {
            float[,] myMatrixArray = new float[4, 4];
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    myMatrixArray[i, j] = myMatrix[i, j];

            return myMatrixArray;
        }
        public static double[,] MatrixToDoubleArray(Matrix4 myMatrix)
        {
            double[,] myMatrixArray = new double[4, 4];
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    myMatrixArray[i, j] = myMatrix[i, j];

            return myMatrixArray;
        }
         public static double[,] MatrixToDoubleArray(Matrix3d myMatrix)
        {
            double[,] myMatrixArray = new double[3, 3];
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    myMatrixArray[i, j] = myMatrix[i, j];

            return myMatrixArray;
        }
   

        public void GetPosition(double[] position)
        {
            
            position[0] = this.Matrix[0, 3];
            position[1] = this.Matrix[1, 3];
            position[2] = this.Matrix[2, 3];
        }
  

  
        internal static T[] InitializeWithDefaultInstances<T>(int Length) where T : new()
        {
            T[] array = new T[Length];
            for (int i = 0; i < Length; i++)
            {
                array[i] = new T();
            }
            return array;
        }
   
        public void Translate(float x, float y, float z)
        {
            if (x == 0.0 && y == 0.0 && z == 0.0)
            {
                return;
            }

            Matrix4 matrix = Matrix4.Identity;
           
            matrix[0, 3] = x;
            matrix[1, 3] = y;
            matrix[2, 3] = z;
            Matrix4.Mult(ref Matrix, ref matrix, out Matrix);
            
        }
        public void Rotate(float angle, float x, float y, float z)
        {
            if (angle == 0.0 || (x == 0.0 && y == 0.0 && z == 0.0))
            {
                return;
            }

            // convert to radians
            angle = angle * MathBase.DegreesToRadians_Float;

            // make a normalized quaternion
            float w = Convert.ToSingle(Math.Cos(0.5 * angle));
            float f = Convert.ToSingle(Math.Sin(0.5 * angle) / Math.Sqrt(x * x + y * y + z * z));
            x *= f;
            y *= f;
            z *= f;

            // convert the quaternion to a matrix
            Matrix4 matrix = Matrix4.Identity;
            

            float ww = w * w;
            float wx = w * x;
            float wy = w * y;
            float wz = w * z;

            float xx = x * x;
            float yy = y * y;
            float zz = z * z;

            float xy = x * y;
            float xz = x * z;
            float yz = y * z;

            float s = ww - xx - yy - zz;

            matrix[0, 0] = xx * 2 + s;
            matrix[1, 0] = (xy + wz) * 2;
            matrix[2, 0] = (xz - wy) * 2;

            matrix[0, 1] = (xy - wz) * 2;
            matrix[1, 1] = yy * 2 + s;
            matrix[2, 1] = (yz + wx) * 2;

            matrix[0, 2] = (xz + wy) * 2;
            matrix[1, 2] = (yz - wx) * 2;
            matrix[2, 2] = zz * 2 + s;
           
            Matrix4.Mult(ref Matrix, ref matrix, out Matrix);
           
        }
        public void Scale(float x, float y, float z)
        {
            if (x == 1.0 && y == 1.0 && z == 1.0)
            {
                return;
            }

            Matrix4 matrix = Matrix4.Identity;

            matrix[0, 0] = x;
            matrix[1, 1] = y;
            matrix[2, 2] = z;

            Matrix4.Mult(ref Matrix, ref matrix, out Matrix);
           
        }
     
    }
}

  