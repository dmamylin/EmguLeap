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
    public class NormalsKDTree : TestBase
    {
      
         [Test]
        public void Cube_ShowNormals()
        {

            vertices = Vertices.CreateCube_Corners(5);
            Vertices.SetColorOfListTo(vertices, 1, 0, 0, 1);

            Model3D myModel = new Model3D("Cube");
            myModel.VertexList = vertices;

            myModel.CalculateNormals_Triangulation();

        
            this.CreateLinesForNormals(myModel);
            ShowModel_WithLines(myModel, true);
        }
        [Test]
        public void Bunny_ShowNormals()
        {
            string fileNameLong = path + "\\bunny.xyz";
            vertices = IOUtils.ReadXYZFile_ToVertices(fileNameLong, false);
            Vertices.SetColorOfListTo(vertices, new Vector3d(1, 0, 0));


            Model3D myModel = new Model3D("Bunny");
            myModel.VertexList = vertices;

            myModel.CalculateNormals_Triangulation();

            this.CreateLinesForNormals(myModel);
            

            ShowModel_WithLines(myModel, true);
            Model3D.Save_OBJ(myModel, path, "Bunny_Triangulated.obj");
            
        }
    
     
    }
}
