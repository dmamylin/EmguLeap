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
    public class TriangulationORourke_Cube : TestBase
    {

    
         [Test]
         public void Cube_Delaunay_RandomPoints()
         {
             //vertices = Vertices.CreateCube_RandomPointsOnPlanes(1, 10);
             vertices = Vertices.CreateCube_Corners(2);
             
             Vertices.SetColorOfListTo(vertices, 1, 0, 0, 1);
             List<Vector3d> myListVectors = Vertices.ConvertToVector3dList(vertices);

             DelaunayTri delaunay = new DelaunayTri(myListVectors);
             
             Model3D myModel = CreateModel("Cube Delaunay", vertices, delaunay.Faces.ListFaces);

             ShowModel(myModel, true);


         }
         [Test]
         public void Cube_DelaunayOLD_RandomPoints()
         {
             //vertices = Vertices.CreateCube_RandomPointsOnPlanes(1, 10);
             vertices = Vertices.CreateCube_Corners(2);

             //vertices = Vertices.CreateCube_Corners(0.1);
             Vertices.SetColorOfListTo(vertices, 1, 0, 0, 1);
             List<Vector3d> myListVectors = Vertices.ConvertToVector3dList(vertices);

             DelaunayTri_Old delaunay = new DelaunayTri_Old(myListVectors);

             Model3D myModel = CreateModel("Cube Delaunay", vertices, delaunay.Faces.ListFaces);

             ShowModel(myModel, true);


         }
         [Test]
         public void Cube_Voronoi_RandomPoints()
         {
             vertices = Vertices.CreateCube_RandomPointsOnPlanes(1, 10);

             //vertices = Vertices.CreateCube_Corners(0.1);
             Vertices.SetColorOfListTo(vertices, 1, 0, 0, 1);
             List<Vector3d> myListVectors = Vertices.ConvertToVector3dList(vertices);

             DelaunayTri delaunay = new DelaunayTri();
             delaunay.Voronoi(myListVectors);

             Model3D myModel = CreateModel("Cube Voronoi", vertices, delaunay.Faces.ListFaces);

             ShowModel(myModel, true);


         }
    
    }
}
