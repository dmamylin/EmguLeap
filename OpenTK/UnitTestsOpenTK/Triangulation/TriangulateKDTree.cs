using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using OpenTKLib;
using OpenTK;
using KDTreeRednaxela;

using MIConvexHull;
namespace UnitTestsOpenTK
{
    [TestFixture]
    [Category("UnitTest")]
    public class TriangulateKDTree : TestBase
    {
        [Test]
        public void Bunny_Show()
        {
            string fileNameLong = path + "\\bunny.xyz";
            vertices = IOUtils.ReadXYZFile_ToVertices(fileNameLong, false);
            Vertices.SetColorOfListTo(vertices, new Vector3d(1, 0, 0));


            Model3D myModel = new Model3D("Bunny");
            myModel.VertexList = vertices;
            
            myModel.Parts = new List<Part>();
            Model3D.AssignTriangleAndPartFromVertex(myModel);
            myModel.CalculateBoundingBox(true);


           
            ShowModel(myModel, true);

        }
        [Test]
        public void PointCloud_TriangulateRednaxela()
        {
            string fileNameLong = path + "\\KinectFace1.obj";
            vertices = IOUtils.ReadObjFile_ToVertices(fileNameLong);

            
            Model3D myModel = new Model3D("PointCloud");
            myModel.VertexList = vertices;
            

            Model3D.TriangulateVertices_Rednaxela(myModel);
            
            ShowModel(myModel, true);

        }
        [Test]
        public void Face_TriangulateRednaxela()
        {
            string fileNameLong = path + "\\1.xyz";
            vertices = IOUtils.ReadXYZFile_ToVertices(fileNameLong, false);
            //Vertices.SetColorOfListTo(vertices, new Vector3d(1, 0, 0));
           



            Model3D myModel = new Model3D("Face");
            myModel.VertexList = vertices;
            
            Model3D.TriangulateVertices_Rednaxela(myModel);

            ShowModel(myModel, true);

        }
      
        [Test]
        public void Bunny_TriangulateAndSave()
        {
            string fileNameLong = path + "\\bunny.xyz";
            vertices = IOUtils.ReadXYZFile_ToVertices(fileNameLong, false);
            Vertices.SetColorOfListTo(vertices, new Vector3d(1, 0, 0));


            Model3D myModel = new Model3D("Bunny");
            myModel.VertexList = vertices;

            Model3D.TriangulateVertices_Rednaxela(myModel);
            myModel.Helper_NormalsFromTriangles();
            Model3D.Save_OBJ(myModel, path, "Bunny_Triangulated.obj");
            ShowModel(myModel, true);

        }
     
        [Test]
        public void Face_TriangulateAndSave()
        {
            string fileNameLong = path + "\\1.xyz";
            vertices = IOUtils.ReadXYZFile_ToVertices(fileNameLong, false);
           

            Model3D myModel = new Model3D("Face");
            myModel.VertexList = vertices;

            Model3D.TriangulateVertices_Rednaxela(myModel);
            Model3D.Save_OBJ(myModel, path, "1_triangulated.obj");
            ShowModel(myModel, true);

        }
        [Test]
        public void Cube_TriangulateRednaxela()
        {

            vertices = Vertices.CreateCube_Corners(2);
            Vertices.SetColorOfListTo(vertices, 1, 0, 0, 1);
           
            Model3D myModel = new Model3D("Cube");
            myModel.VertexList = vertices;
            Model3D.TriangulateVertices_Rednaxela(myModel);

            ShowModel(myModel, true);
        }
        [Test]
        public void Cube_TriangulateStark()
        {

            vertices = Vertices.CreateCube_Corners(2);
            Vertices.SetColorOfListTo(vertices, 1, 0, 0, 1);

            Model3D myModel = new Model3D("Cube");
            myModel.VertexList = vertices;
            Model3D.TriangulateVertices_Stark(myModel);

            ShowModel(myModel, true);
        }

    
        [Test]
        public void Bunny_TriangulateRednaxela()
        {
            string fileNameLong = path + "\\bunny.xyz";
            vertices = IOUtils.ReadXYZFile_ToVertices(fileNameLong, false);
            Vertices.SetColorOfListTo(vertices, new Vector3d(1, 0, 0));

           
            Model3D myModel = new Model3D("Cube");
            myModel.VertexList = vertices;
            Model3D.TriangulateVertices_Rednaxela(myModel);


            ShowModel(myModel, true);

        }

    
      
       
       
     
    }
}
