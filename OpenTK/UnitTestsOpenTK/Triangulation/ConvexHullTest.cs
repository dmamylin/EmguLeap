using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using OpenTKLib;
using OpenTK;


using System.Linq;
namespace UnitTestsOpenTK
{
    [TestFixture]
    [Category("UnitTest")]
    public class ConvexHullTest : TestBase
    {


        [Test]
        public void Cube_ConvexHull()
        {

            vertices = Vertices.CreateCube_Corners(0.1);
            Vertices.SetColorOfListTo(vertices, 1, 0, 0, 1);
            List<Vector3d> myListVectors = Vertices.ConvertToVector3dList(vertices);

            ConvexHull3D convHull = new ConvexHull3D(myListVectors);

            Model3D myModel = CreateModel("Convex Hull", vertices, convHull.Faces.ListFaces);


            ShowModel(myModel, true);
            System.Diagnostics.Debug.WriteLine("Number of faces: " + convHull.Faces.ListFaces.Count.ToString());

        }
        [Test]
        public void Cube_ConvexHull_RandomPoints()
        {

            vertices = Vertices.CreateCube_RandomPointsOnPlanes(1, 10);

            Vertices.SetColorOfListTo(vertices, 1, 0, 0, 1);
            List<Vector3d> myListVectors = Vertices.ConvertToVector3dList(vertices);

            ConvexHull3D convHull = new ConvexHull3D(myListVectors);

            Model3D myModel = CreateModel("Convex Hull", vertices, convHull.Faces.ListFaces);

            ShowModel(myModel, true);

            System.Diagnostics.Debug.WriteLine("Number of faces: " + convHull.Faces.ListFaces.Count.ToString());

        }
     
        [Test]
        public void Bunny_Hull()
        {
            string fileNameLong = path + "\\bunny.xyz";
            vertices = IOUtils.ReadXYZFile_ToVertices(fileNameLong, false);
            Vertices.SetColorOfListTo(vertices, new Vector3d(1, 0, 0));

            List<Vector3d> myListVectors = Vertices.ConvertToVector3dList(vertices);

            ConvexHull3D cHull = new ConvexHull3D(myListVectors);

            Model3D myModel = CreateModel("Bunny Hull", vertices, cHull.Faces.ListFaces);

            ShowModel(myModel, true);

        }
      
     
    }
}
