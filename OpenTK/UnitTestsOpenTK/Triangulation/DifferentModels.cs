using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.Text;
using System.Drawing;


//using GeoAPI.Geometries;


using OpenTKLib;
using OpenTK;

namespace UnitTestsOpenTK
{
    [TestFixture]
    [Category("UnitTest")]
    public class DifferentModels : TestBase
    {
        private static readonly Random RND = new Random(998715632);
        private const double SideLen = 10.0;

      
        public void ShowModel()
        {
            Model3D myModel = Example3DModels.Sphere("Sphere", 20f,100, new Vector3d(1, 1, 1), null);
            ShowModel(myModel, true);
        }
        [Test]
        public void ShowCuboid()
        {
            this.vertices = Vertices.CreateCuboid(5, 8, 60);
            verticesTransformed = VertexUtils.CloneListVertex(vertices);
            VertexUtils.TranslateVertices(verticesTransformed, 30, -20, 12);
            ShowVerticesInWindow(new byte[4] { 255, 255, 255, 255 }, new byte[4] { 255, 0, 0, 255 });

        }
        //private void ConvertToVertices(IGeometry myGeometry)
        //{
        //    this.vertices = new List<Vertex>();
        //    for (int i = 0; i < myGeometry.Coordinates.Length; i++)
        //    {
        //        Coordinate c = myGeometry.Coordinates[i];
        //        Vertex v = new Vertex(c.X, c.Y, c.Z);
        //    }
        //}
     

     
        
        //private static ICollection<Coordinate> RandomPoints(int nPts)
        //{
        //    List<Coordinate> pts = new List<Coordinate>();

        //    for (int i = 0; i < nPts; i++)
        //    {
        //        double x = SideLen * RND.NextDouble();
        //        double y = SideLen * RND.NextDouble();
        //        double z = SideLen * RND.NextDouble();

        //        pts.Add(new Coordinate(x, y, z));
        //    }
        //    return pts;
        //}
    }
}
