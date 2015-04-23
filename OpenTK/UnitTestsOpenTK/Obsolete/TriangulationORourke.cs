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
    public class TriangulationORourke : TestBase
    {

     
        [Test]
        public void Bunny_Delaunay()
        {
            string fileNameLong = path + "\\bunny.xyz";
            vertices = IOUtils.ReadXYZFile_ToVertices(fileNameLong, false);
            Vertices.SetColorOfListTo(vertices, new Vector3d(1, 0, 0));

            List<Vector3d> myListVectors = Vertices.ConvertToVector3dList(vertices);
           
            //DelaunayTri delaunay = new DelaunayTri(myListVectors);
            DelaunayTri delaunay = new DelaunayTri(myListVectors);

            Model3D myModel = CreateModel("Bunny Delaunay", vertices, delaunay.Faces.ListFaces);

            ShowModel(myModel, true);

        }
        [Test]
        public void Bunny_DelaunayOLD()
        {
            string fileNameLong = path + "\\bunny.xyz";
            vertices = IOUtils.ReadXYZFile_ToVertices(fileNameLong, false);
            Vertices.SetColorOfListTo(vertices, new Vector3d(1, 0, 0));

            List<Vector3d> myListVectors = Vertices.ConvertToVector3dList(vertices);

            //DelaunayTri delaunay = new DelaunayTri(myListVectors);
            DelaunayTri_Old delaunay = new DelaunayTri_Old(myListVectors);

            Model3D myModel = CreateModel("Bunny Delaunay", vertices, delaunay.Faces.ListFaces);

            ShowModel(myModel, true);

        }
       
     
    }
}
