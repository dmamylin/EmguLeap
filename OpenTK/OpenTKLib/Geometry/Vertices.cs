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

using OpenCLTemplate;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Reflection;
using OpenTK;

namespace OpenTKLib
{
    public class Vertices : List<Vertex>
    {
      

        public Vertices()
        {
        }
      
      
        public override string ToString()
        {

            string returnString = this.Count.ToString();

         
            return returnString;
        }

        public static Vertex GetVertexMax(List<Vertex> vertices)
        {
            Vertex centerOfGravity = new Vertex();
            Vertex maxPoint = new Vertex();
            Vertex minPoint = new Vertex();

            CalculateBoundingBox(vertices, ref maxPoint, ref minPoint);
           
            return maxPoint;

        }
        public static void ResetVerticesTo(List<Vertex> vertices, Vertex newOrigin)
        {
            //reset vertex so that it starts from 0,0,0
            for (int i = 0; i < vertices.Count; i++)
            {
                Vector3d v = vertices[i].Vector;
                v.X -= newOrigin.Vector.X;
                v.Y -= newOrigin.Vector.Y;
                v.Z -= newOrigin.Vector.Z;
                vertices[i].Vector = v;

            }

        }
        public static void AddVector(List<Vertex> vertices, Vertex newOrigin)
        {
            //reset vertex so that it starts from 0,0,0
            for (int i = 0; i < vertices.Count; i++)
            {
                Vector3d v = vertices[i].Vector;
                v.X += newOrigin.Vector.X;
                v.Y += newOrigin.Vector.Y;
                v.Z += newOrigin.Vector.Z;
                vertices[i].Vector = v;

            }

        }
        private static Vertex CalculateCenterOfGravity(List<Vertex> vertices)
        {
            Vertex centerOfGravity = new Vertex();


            foreach (Vertex vr in vertices)
                centerOfGravity.Vector += vr.Vector;
            centerOfGravity.Vector /= (double)vertices.Count;
            return centerOfGravity;
        }
        public static Vector3d CalculateCenterOfGravityVector(List<Vertex> vertices)
        {
            Vector3d centerOfGravity = new Vector3d();


            foreach (Vertex vr in vertices)
                centerOfGravity += vr.Vector;
            centerOfGravity /= (double)vertices.Count;
            return centerOfGravity;
        }
        public static void CalculateBoundingBox(List<Vertex> vertices, ref Vertex maxPoint, ref Vertex minPoint)
        {
           
            if (vertices.Count < 1)
                return;

            maxPoint = new Vertex();
            minPoint = new Vertex();

            double xMax = vertices[0].Vector.X;
            double yMax = vertices[0].Vector.Y;
            double zMax = vertices[0].Vector.Z;
            double xMin = vertices[0].Vector.X;
            double yMin = vertices[0].Vector.Y;
            double zMin = vertices[0].Vector.Z;
            foreach (Vertex ver in vertices)
            {
                if (ver.Vector.X > xMax)
                    xMax = ver.Vector.X;
                if (ver.Vector.Y > yMax)
                    yMax = ver.Vector.Y;
                if (ver.Vector.Z > zMax)
                    zMax = ver.Vector.Z;
                if (ver.Vector.X < xMin)
                    xMin = ver.Vector.X;
                if (ver.Vector.Y < yMin)
                    yMin = ver.Vector.Y;
                if (ver.Vector.Z < zMin)
                    zMin = ver.Vector.Z;
            }
            maxPoint.Vector.X = xMax;
            maxPoint.Vector.Y = yMax;
            maxPoint.Vector.Z = zMax;
           
            minPoint.Vector.X = xMin;
            minPoint.Vector.Y = yMin;
            minPoint.Vector.Z = zMin;

        }

        public static Vertex ResetVertexToOrigin(List<Vertex> vertices)
        {
            Vertex newOrigin = CalculateCenterOfGravity(vertices);
            ResetVerticesTo(vertices, newOrigin);
            return newOrigin;

        }
        public static void ResizeVertixesTo1(List<Vertex> vertices, ref Vector3d centerOfGravity, ref Vector3d maxPoint, ref Vector3d minPoint)
        {
            double d = Math.Max(maxPoint.X, maxPoint.Y);
            d = Math.Max(d, maxPoint.Z);
            if (d > 0)
            {
                centerOfGravity.X /= d;
                centerOfGravity.Y /= d;
                centerOfGravity.Z /= d;
                for (int i = 0; i < vertices.Count; i++)
                {
                    vertices[i].Vector.X /= d;
                    vertices[i].Vector.Y /= d;
                    vertices[i].Vector.Z /= d;
                }
            }

          
        }
        public static List<Vector3d> ConvertToVector3dList(List<Vertex> listPoints)
        {
            List<Vector3d> listOfVectors = new List<Vector3d>();
            for (int i = 0; i < listPoints.Count; i++)
            {
                Vertex myPoint = listPoints[i];
                listOfVectors.Add(new Vector3d(myPoint.Vector.X, myPoint.Vector.Y, myPoint.Vector.Z));
            }

            return listOfVectors;
        }
        public static List<Vertex> ConvertVector3DListToVertexList(List<Vector3d> listPoints)
        {
            List<Vertex> listOfVeretixes = new List<Vertex>();
            for (int i = 0; i < listPoints.Count; i++)
            {
                Vector3d myPoint = listPoints[i];
                listOfVeretixes.Add(new Vertex(i, myPoint));
            }

            return listOfVeretixes;
        }
        public static void ColorDelete(List<Vertex> listPoints)
        {
            
            for (int i = 0; i < listPoints.Count; i++)
            {
                listPoints[i].Color = null;
                
            }

            
        }
        public static void ChangeTransparency(List<Vertex> listPoints, float f)
        {

            for (int i = 0; i < listPoints.Count; i++)
            {
                if (listPoints[i].Color != null)
                {
                    listPoints[i].Color[3] = f;
                }
                
                
            }


        }
        public static void AssignNewVectorList(List<Vertex> listVertices, List<Vector3d> listPoints)
        {

            for (int i = 0; i < listVertices.Count; i++)
            {
                listVertices[i].Vector = listPoints[i];

            }


        }
        public static void SetColorToList(List<Vertex> listVertices, List<float[]> colorList)
        {
            if (listVertices.Count != colorList.Count)
                return;

            
            for (int i = 0; i < listVertices.Count; i++)
            {
                Vertex myPoint = listVertices[i];
                myPoint.Color = colorList[i];

            }

        }
        public static void SetColorOfListTo(List<Vertex> listVertices, float r, float g, float b, float a)
        {
            float[] color = new float[4] { r, g, b, a };
            for (int i = 0; i < listVertices.Count; i++)
            {
                listVertices[i].Color = color;


            }

        }
        public static void SetColorOfListTo(List<Vertex> listVertices, Vector3d colorVector)
        {
            float[] color = new float[4] { Convert.ToSingle(colorVector[0]), Convert.ToSingle(colorVector[1]), Convert.ToSingle(colorVector[2]), 1f };
            for (int i = 0; i < listVertices.Count; i++)
            {
                listVertices[i].Color = color;


            }

        }
        public static List<Vertex> CopyVertices(List<Vertex> pointsTarget)
        {
            List<Vertex> tempPoints = new List<Vertex>();

            for (int i = 0; i < pointsTarget.Count; i++)
            {
                Vertex point1 = pointsTarget[i];
                tempPoints.Add(new Vertex(point1.Vector, point1.Color));

            }

         
            return tempPoints;

        }
        public static List<Vertex> CreateSomePoints()
        {

            List<Vertex> points = new List<Vertex>();
            // Create points
            double[] origin = { 0.0, 0.0, 0.0 };
            //points.Add(new Vector3d(0, 0, 0));
            points.Add(new Vertex(100, 0, 0));
            points.Add(new Vertex(0, 100, 0));
            points.Add(new Vertex(0, 0, 100));
            //points.Add(new Vector3d(100, 100, 0));
            //points.Add(new Vector3d(0, 200, 200));
            //points.Add(new Vector3d(200, 0, 100));
            //points.Add(new Vector3d(400, 200, 300));
            //points.Add(new Vector3d(200, 500, 1000));
            //points.Add(new Vector3d(500, 1, 600));




            return points;

        }
        public static List<Vertex> CreateCuboid(float u, float v, int numberOfPoints)
        {
            List<Vertex> points = new List<Vertex>();
            float v0 = 0f;
            int indexInModel = -1;
            for (int i = 0; i < numberOfPoints; i++)
            {
                indexInModel++;
                points.Add(new Vertex(indexInModel, 0, v0, 0));
                points.Add(new Vertex(indexInModel, 0, v0, u));
                points.Add(new Vertex(indexInModel, u, v0, u));
                points.Add(new Vertex(indexInModel, u, v0, 0));

                v0 += v / numberOfPoints;

            }
            return points;

        }
        public static List<Vertex> CreateCube_Corners(double cubeSize)
        {

            List<Vertex> points = new List<Vertex>();

            points.Add(new Vertex(0, -cubeSize / 2, -cubeSize / 2, -cubeSize / 2));
            points.Add(new Vertex(1, cubeSize / 2, -cubeSize / 2, -cubeSize / 2));
            points.Add(new Vertex(2, cubeSize / 2, cubeSize / 2, -cubeSize / 2));
            points.Add(new Vertex(3, -cubeSize / 2, cubeSize / 2, -cubeSize / 2));

            points.Add(new Vertex(4, -cubeSize / 2, -cubeSize / 2, cubeSize / 2));
            points.Add(new Vertex(5, cubeSize / 2, -cubeSize / 2, cubeSize / 2));
            points.Add(new Vertex(6, cubeSize / 2, cubeSize / 2, cubeSize / 2));
            points.Add(new Vertex(7, -cubeSize / 2, cubeSize / 2, cubeSize / 2));

         
            return points;

        }
        public static List<Vertex> CreateCube_RandomPoints(double cubeSize, int numberOfRandomPoints)
        {
            List<Vertex> points = new List<Vertex>();
            var r = new Random();
            for (var i = 0; i < numberOfRandomPoints; i++)
            {
                var vi = new Vertex(cubeSize * r.NextDouble() - cubeSize / 2,
                                    cubeSize * r.NextDouble() - cubeSize / 2,
                                    cubeSize * r.NextDouble() - cubeSize / 2);
                points.Add(vi);
                
            }
            return points;
        }
         public static List<Vertex> CreateCube_RegularGrid(double cubeSize, int numberOfRandomPoints)
        {
            List<Vertex> points = new List<Vertex>();
            var r = new Random();

            for (int i = 0; i <= 10; i++)
            {
                for (int j = 0; j <= 10; j++)
                {
                    for (int k = 0; k <= 10; k++)
                    {
                        var v = new Vertex(-cubeSize + 4 * i, -cubeSize + 4 * j, -cubeSize + 4 * k);
                        points.Add(v);
                        
                    }
                }
            }

            return points;
                     

        }
           
        public static List<Vertex> CreateCube_RandomPointsOnPlanes(double cubeSize, int numberOfRandomPoints)
        {
            List<Vertex> points = new List<Vertex>();
            points = CreateCube_Corners(cubeSize);

            var r = new Random();

            int indexInModel = points[points.Count - 1].IndexInModel;
            for (var i = 0; i < numberOfRandomPoints; i++)
            {
                indexInModel++;
                var vi = new Vertex(indexInModel, cubeSize / 2, cubeSize * r.NextDouble() - cubeSize / 2, cubeSize * r.NextDouble() - cubeSize / 2);
                points.Add(vi);

            }
            for (var i = 0; i < numberOfRandomPoints; i++)
            {
                indexInModel++;
                var vi = new Vertex(indexInModel , - cubeSize / 2, cubeSize * r.NextDouble() - cubeSize / 2, cubeSize * r.NextDouble() - cubeSize / 2);
                points.Add(vi);

            }

          
            for (var i = 0; i < numberOfRandomPoints; i++)
            {
                indexInModel++;
                var vi = new Vertex(indexInModel, cubeSize * r.NextDouble() - cubeSize / 2, cubeSize / 2, cubeSize * r.NextDouble() - cubeSize / 2);
                points.Add(vi);

            }
            for (var i = 0; i < numberOfRandomPoints; i++)
            {
                indexInModel++;
                var vi = new Vertex(indexInModel, cubeSize * r.NextDouble() - cubeSize / 2, -cubeSize / 2, cubeSize * r.NextDouble() - cubeSize / 2);
                points.Add(vi);

            }

           
            for (var i = 0; i < numberOfRandomPoints; i++)
            {
                indexInModel++;
                var vi = new Vertex(indexInModel, cubeSize * r.NextDouble() - cubeSize / 2, cubeSize * r.NextDouble() - cubeSize / 2, -cubeSize / 2);
                points.Add(vi);

            }
            for (var i = 0; i < numberOfRandomPoints; i++)
            {
                indexInModel++;
                var vi = new Vertex(indexInModel, cubeSize * r.NextDouble() - cubeSize / 2, cubeSize * r.NextDouble() - cubeSize / 2, cubeSize / 2);
                points.Add(vi);

            }
            return points;
        }
        public static List<Vertex> CreateSphere_RandomPoints(double cubeSize, int numberOfRandomPoints)
        {
            List<Vertex> points = new List<Vertex>();
            var r = new Random();
            /****** Random Vertices ******/
            for (var i = 0; i < numberOfRandomPoints; i++)
            {
                var radius = cubeSize * r.NextDouble();
                // if (i < NumberOfVertices / 2) radius /= 2;
                var theta = 2 * Math.PI * r.NextDouble();
                var azimuth = Math.PI * r.NextDouble();
                var x = radius * Math.Cos(theta) * Math.Sin(azimuth);
                var y = radius * Math.Sin(theta) * Math.Sin(azimuth);
                var z = radius * Math.Cos(azimuth);
                var vi = new Vertex(x, y, z);
                points.Add(vi);
            }
            return points;

        }
            
            
        public static List<Vector3d> ConvertToVector3DList_FromArray(ushort[] arrayDepth, int width, int height)
        {
            List<Vector3d> listOfVectors = new List<Vector3d>();
            
            for (ushort x = 0; x < width; ++x)
            {
                for (ushort y = 0; y < height; ++y)
                {

                    int depthIndex = (y * width) + x;
                    ushort z = arrayDepth[depthIndex];

                    if (z != 0)
                    {
                        
                        listOfVectors.Add(new Vector3d(Convert.ToDouble(x), Convert.ToDouble(y), Convert.ToDouble(z)));
                    }
                }
            }

            return listOfVectors;
        }
        public static List<double[]> ConvertToListDoubles(List<Vertex> vertices)
        {
            List<double[]> a = new List<double[]>();

            for (int i = 0; i < vertices.Count; ++i)
            {
                Vector3d v = vertices[i].Vector;
                double[] d = new double[3]{v.X, v.Y, v.Z};
                a.Add(d);
            }

            return a;
        }
        public static double[][] ConvertToDoubleArray(List<Vertex> vertices)
        {
           double[][] a = new double[vertices.Count][];

   
   
            for (int i = 0; i < vertices.Count; ++i)
            {
                Vector3d v = vertices[i].Vector;
                double[] d = new double[3]{v.X, v.Y, v.Z};
                a[i] = d;
            }

            return a;
        }
      
    }
   
    
}
