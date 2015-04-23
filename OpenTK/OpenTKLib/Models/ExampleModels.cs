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
    public static class Example3DModels
    {
        private static float rCyl = 1f;
        private static float rCon = 1f;
        private static float rSph = 1f;

        /// <summary>Generates a 3D Model for a cylinder.</summary>
        /// <param name="Name">Model name.</param>
        /// <param name="Radius">Cylinder radius.</param>
        /// <param name="Height">Cylinder height.</param>
        /// <param name="numPoints">Number of points for circular section.</param>
        /// <param name="Color">Color vector.</param>
        /// <param name="Texture">Texture bitmap. Null uses no texture</param>
        public static Model3D Cylinder(string Name, float Radius, float Height, int numPoints, Vector3d Color, System.Drawing.Bitmap Texture)
        {
            Example3DModels.rCyl = Radius;
            return new Model3D(Name, new Model3D.CoordFuncXYZ(Example3DModels.CylFunction), 0.0f, 6.283185f, 0.0f, Height, numPoints, 2, Color, Texture);
        }

        private static float[] CylFunction(float u, float v)
        {
            float[] listOfPoints = new float[6]{0.0f,0.0f,0.0f,(float) Math.Cos((double) u),(float) Math.Sin((double) u),0.0f};
            listOfPoints[0] = Example3DModels.rCyl * listOfPoints[3];
            listOfPoints[1] = Example3DModels.rCyl * listOfPoints[4];
            listOfPoints[2] = v;
            return listOfPoints;
        }
        /// <summary>Generates a 3D Model for a cone.</summary>
        /// <param name="Name">Model name.</param>
        /// <param name="Radius">Cone outer radius.</param>
        /// <param name="Height">Cone height.</param>
        /// <param name="numPoints">Number of points for circular section.</param>
        /// <param name="Color">Color vector.</param>
        /// <param name="Texture">Texture bitmap. Null uses no texture</param>
        public static Model3D Cone(string Name, float Radius, float Height, int numPoints, Vector3d Color, System.Drawing.Bitmap Texture)
        {
            Example3DModels.rCon = Radius / Height;
            return new Model3D(Name, new Model3D.CoordFuncXYZ(Example3DModels.ConeFunc), 0.0f, 6.283185f, 0.0f, -Height, numPoints, 2, Color , Texture);
        }

        private static float[] ConeFunc(float u, float v)
        {

            float[] listOfPoints = new float[6]{ Example3DModels.rCon * (float) Math.Sin((double) u) * v,
                Example3DModels.rCon * (float) Math.Cos((double) u) * v,v,v * (float) Math.Sin((double) u),
                    v * (float) Math.Cos((double) u),-Example3DModels.rCon * v};

            float num = (float)(1.0 / Math.Sqrt((double)listOfPoints[3] * (double)listOfPoints[3] + (double)listOfPoints[4] * (double)listOfPoints[4] + (double)listOfPoints[5] * (double)listOfPoints[5]));
            listOfPoints[3] *= num;
            listOfPoints[4] *= num;
            listOfPoints[5] *= num;
            return listOfPoints;
        }
        /// <summary>Generates a 3D Model for a sphere.</summary>
        /// <param name="Name">Model name.</param>
        /// <param name="numPoints">Number of points to use for each coordinate. At least 4.</param>
        /// <param name="Radius">Sphere radius.</param>
        /// <param name="Color">Color vector.</param>
        /// <param name="Texture">Texture bitmap. Null uses no texture</param>
        public static Model3D Sphere(string Name, float Radius, int numPoints, Vector3d Color, System.Drawing.Bitmap Texture)
        {
            Example3DModels.rSph = Radius;
            if (numPoints < 4)
                numPoints = 4;
            return new Model3D(Name, new Model3D.CoordFuncXYZ(Example3DModels.SphereFunction), 0.0f, 6.283185f, -1.570796f, 1.570796f, numPoints, numPoints, Color, Texture);
        }

        private static float[] SphereFunction(float u, float v)
        {
            float[] listOfPoints = new float[6]{0.0f,0.0f,0.0f,(float) (Math.Cos((double) u) * Math.Cos((double) v)),(float) (Math.Sin((double) u) * Math.Cos((double) v)),
          (float) Math.Sin((double) v)};
            listOfPoints[0] = Example3DModels.rSph * listOfPoints[3];
            listOfPoints[1] = Example3DModels.rSph * listOfPoints[4];
            listOfPoints[2] = Example3DModels.rSph * listOfPoints[5];
            return listOfPoints;
        }
        /// <summary>Generates a 3D Model for a disk.</summary>
        /// <param name="Name">Model name.</param>
        /// <param name="numPoints">Number of points to use in circumference.</param>
        /// <param name="InnerRadius">Inner disk radius.</param>
        /// <param name="OuterRadius">Outer disk radius.</param>
        /// <param name="Color">Color vector.</param>
        /// <param name="Texture">Texture bitmap. Null uses no texture</param>
        public static Model3D Disk(string Name, float InnerRadius, float OuterRadius, int numPoints, Vector3d Color, System.Drawing.Bitmap Texture)
        {
            if (numPoints < 4)
                numPoints = 4;
            return new Model3D(Name, new Model3D.CoordFuncXYZ(Example3DModels.DiskFunction), 0.0f, 6.283185f, InnerRadius, OuterRadius, numPoints, 2, Color, Texture);
        }

        private static float[] DiskFunction(float u, float v)
        {
            float[] listOfPoints = new float[6]{0.0f,0.0f,0.0f,0.0f,0.0f,1f};

            listOfPoints[0] = v * (float)Math.Cos((double)u);
            listOfPoints[1] = v * (float)Math.Sin((double)u);
            listOfPoints[2] = 0.0f;
            return listOfPoints;
        }
        /// <summary>
        /// Generates a 3D Model for a cuboid
        /// </summary>
        /// <param name="Name">Model name</param>
        /// <param name="u">Length of the lower part</param>
        /// <param name="v">Length of the high part</param>
        /// <param name="numberOfPoints">Number of points to use in circumference</param>
        /// <param name="Color">Color vector</param>
        /// <param name="Texture">Texture bitmap. Null uses no texture</param>
        /// <returns></returns>
        public static Model3D Cuboid(string Name, float u, float v, int numberOfPoints, Vector3d Color, System.Drawing.Bitmap Texture)
        {

            List<Vertex> points = Vertices.CreateCuboid(u, v, numberOfPoints);

            Vertices.SetColorOfListTo(points, Color);

            Model3D myModel = new Model3D();
            myModel.Create3DModel("Cube", points);

            return myModel;

        }
        /// <summary>
        /// Generates a 3D Model for a cuboid, by setting all lines with points
        /// </summary>
        /// <param name="Name">Model name</param>
        /// <param name="u">Length of the lower part</param>
        /// <param name="v">Length of the high part</param>
        /// <param name="numberOfPoints">Number of points to use in circumference</param>
        /// <param name="Color">Color vector</param>
        /// <param name="Texture">Texture bitmap. Null uses no texture</param>
        /// <returns></returns>
        public static Model3D Cuboid_AllLines(string Name, float u, float v, int numberOfPoints, Vector3d Color, System.Drawing.Bitmap Texture)
        {

            List<Vertex> points = new List<Vertex>();

            float u0 = 0f;
            float v0 = 0f;

            for (int i = 0; i < numberOfPoints; i++)
            {
                points.Add(new Vertex(u0, 0, 0));
                points.Add(new Vertex(0, 0, u0));
                points.Add(new Vertex(u0, 0, u));
                points.Add(new Vertex(u, 0, u0));

                points.Add(new Vertex(0, v0, 0));
                points.Add(new Vertex(0, v0, u));
                points.Add(new Vertex(u, v0, u));
                points.Add(new Vertex(u, v0, 0));

                points.Add(new Vertex(u0, v, 0));
                points.Add(new Vertex(0, v, u0));
                points.Add(new Vertex(u0, v, u));
                points.Add(new Vertex(u, v, u0));

                u0 += u / 100;
                v0 += v / 100;


            }

            Vertices.SetColorOfListTo(points, Color);

            Model3D myModel = new Model3D();
            myModel.Create3DModel("Cube", points);

            return myModel;

        }

    }
}
