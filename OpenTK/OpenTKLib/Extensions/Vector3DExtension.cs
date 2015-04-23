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
// be liable for any loss, expense or damage, of any type or nature ariMath.Sing out of the use of, or inability to use this software or program, including, but not
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
    //Extensios attached to the object which folloes the "this" 
    public static class Vector3dExtension
    {

        /// <summary>Transform a direction vector by the given Matrix
        /// Assumes the matrix has a bottom row of (0,0,0,1), that is the translation part is ignored.
        /// </summary>
        /// <param name="vec">The vector to transform</param>
        /// <param name="mat">The desired transformation</param>
        /// <returns>The transformed vector</returns>
        public static Vector3d Clone(this Vector3d vector)
        {
            Vector3d vNew = new Vector3d(vector.X, vector.Y, vector.Z);
            return vNew;

        }
        public static Vector3d Negate(this Vector3d vector)
        {
            Vector3d vNew = new Vector3d(-vector.X, -vector.Y, -vector.Z);
            return vNew;

        }
        public static Vector3d LinearInterpolate(this Vector3d vector, Vector3d otherVector, double d)
        {
            Vector3d temp = vector + (otherVector - vector) * d;
            return temp;
        }

        public static double Norm(this Vector3d vector)
        {
            return Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z);
        }
        public static Vector3d NormalizeNew(this Vector3d vector)
        {
            double den = vector.Norm();
            if (den != 0.0)
            {
                vector.X /= den;
                vector.Y /= den;
                vector.Z /= den;

            }
            return vector;

        }
        public static void Normalize(this Vertex vertex)
        {
            double den = vertex.Vector.Norm();
            if (den != 0.0)
            {
                vertex.Vector.X /= den;
                vertex.Vector.Y /= den;
                vertex.Vector.Z /= den;

            }

        }

     
     // converts cartesion to polar coordinates
     // result:
     // [0] = length
     // [1] = angle with z-axis
     // [2] = angle of projection into x,y, plane with x-axis
     //
        public static Vector3d CartesianToPolar(this Vector3d v)
        {
            Vector3d polar = new Vector3d();

            polar.X = v.Length;

            if (v[2] > 0.0f)
            {
                polar.Y = (float)Math.Atan(Math.Sqrt(v[0] * v[0] + v[1] * v[1]) / v[2]);
            }
            else if (v[2] < 0.0f)
            {
                polar[1] = (float)Math.Atan(Math.Sqrt(v[0] * v[0] + v[1] * v[1]) / v[2]) + Math.PI;
            }
            else
            {
                polar[1] = Math.PI * 0.5f;
            }


            if (v[0] > 0.0f)
            {
                polar[2] = (float)Math.Atan(v[1] / v[0]);
            }
            else if (v[0] < 0.0f)
            {
                polar[2] = (float)Math.Atan(v[1] / v[0]) + Math.PI;
            }
            else if (v[1] > 0)
            {
                polar[2] = Math.PI * 0.5f;
            }
            else
            {
                polar[2] = -Math.PI * 0.5;
            }
            return polar;
        }



        ///
        //  converts polar to cartesion coordinates
        //  input:
        //  [0] = length
        //  [1] = angle with z-axis
        //  [2] = angle of projection into x,y, plane with x-axis
        // 
        public static Vector3d PolarToCartesian(this Vector3d v)
        {
            Vector3d cart = new Vector3d();
            cart[0] = v[0] * Math.Sin(v[1]) * (float)Math.Cos(v[2]);
            cart[1] = v[0] * Math.Sin(v[1]) * (float)Math.Sin(v[2]);
            cart[2] = v[0] * Math.Cos(v[1]);
            return cart;
        }


        /////////////////////////////////////////////////////////////////
        /// <summary>
        /// projects Vector v1 on v2 , return value is projection
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        static Vector3d ProjectOntoVector(this Vector3d v1, Vector3d v2)
        {
            return v2 * Vector3d.Dot(v1, v2);
        }


        public static Vector3d ProjectVectorIntoPlane(this Vector3d v1, Vector3d normalOfPlane)
        {
            return v1 - ProjectOntoVector(v1, normalOfPlane);
        }


        public static Vector3d ProjectPointOntoPlane(this Vector3d point, Vector3d anchor, Vector3d normal)
        {
            Vector3d temp = point - anchor;
            return point - ProjectOntoVector(temp, normal);
        }
        public static double AngleInDegrees(this Vector3d a, Vector3d b)
        {
            if(a.Equals(Vector3d.Zero) || b.Equals(Vector3d.Zero))
                return 360;

            Vector3d v = Vector3d.Cross(a, b);
            double d1 = v.Norm();

            double d2 = Vector3d.Dot(a, b);
            double angle = Math.Atan2(d1, d2);
            angle = angle * 180 / Math.PI;

            ////-----------------------------------
            ////alternative
            //double dot = Vector3d.Dot(a, b);
            //// Divide the dot by the product of the magnitudes of the vectors
            //dot = dot / (a.Norm() * b.Norm());
            ////Get the arc cosin of the angle, you now have your angle in radians 
            //double acos = Math.Acos(dot);
            ////Multiply by 180/Mathf.PI to convert to degrees
            //double angleCheck = acos * 180 / Math.PI;
            ////-----------------------------

            //if (angle - angleCheck > 0.1)
            //    System.Windows.Forms.MessageBox.Show("SW Check Angle");

            return angle;


           

        }



    }
}
