using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using OpenTKLib;
using OpenTK;
using KDTreeRednaxela;

using MIConvexHull;
using VoronoiFortune;


namespace UnitTestsOpenTK
{
    [TestFixture]
    [Category("UnitTest")]
    public class TriangulateTest : TestBase
    {
        [Test]
        public void Face_VoronoiFortune_AsTriangles()
        {
            List<PointFortune> listPointsFortune = new List<PointFortune>();
            string fileNameLong = path + "\\KinectFace1.obj";

            vertices = IOUtils.ReadObjFile_ToVertices(fileNameLong);

            for (int i = 0; i < vertices.Count; i++)
            {
                PointFortune v = new PointFortune(vertices[i].Vector.X, vertices[i].Vector.Y, i);
                listPointsFortune.Add(v);

            }

            List<EdgeFortune> listEdges;

            Voronoi voronoi = new Voronoi(0.1);

            listEdges = voronoi.GenerateVoronoi(listPointsFortune);
            
            List<Triangle> listTriangle = new List<Triangle>();
            for (int i = 0; i < listEdges.Count; i+=3)
            {
                EdgeFortune edge = listEdges[i];


                Triangle t = new Triangle();
               
                //t.IndVertices.Add(cell.Vertices[0].IndexInModel);
                //t.IndVertices.Add(cell.Vertices[1].IndexInModel);
                //t.IndVertices.Add(cell.Vertices[2].IndexInModel);
                listTriangle.Add(t);

                //myLinesFrom.Add(vertices[edge.PointIndex1]);
                //myLinesTo.Add(vertices[edge.PointIndex2]);

            }

            //-------------------
            Model3D myModel = new Model3D("Face");
            Model3D.AssignModelDataFromVertices(myModel, vertices);


            ShowModel(myModel, true);

        }

         [Test]
        public void Face_VoronoiFortune()
        {
            List<PointFortune> listPointsFortune = new List<PointFortune>();
            string fileNameLong = path + "\\KinectFace1.obj";
            
            vertices = IOUtils.ReadObjFile_ToVertices(fileNameLong);

            
            for (int i = 0; i < vertices.Count; i++)
            {
                PointFortune v = new PointFortune(vertices[i].Vector.X, vertices[i].Vector.Y, i);
                listPointsFortune.Add(v);

            }

            List<EdgeFortune> listEdges;
            
          
            Voronoi voronoi = new Voronoi(0.1);

            listEdges = voronoi.GenerateVoronoi(listPointsFortune);
            List<Vertex> myLinesFrom = new List<Vertex>();
            List<Vertex> myLinesTo = new List<Vertex>();

             
            for (int i = 0; i < listEdges.Count; i++)
            {
                EdgeFortune edge = listEdges[i];

                myLinesFrom.Add(vertices[edge.PointIndex1]);
                myLinesTo.Add(vertices[edge.PointIndex2]);

            }

            //-------------------
            Model3D myModel = new Model3D("Face");
            Model3D.AssignModelDataFromVertices(myModel, vertices);


            ShowModel_WithLines(myModel, true, myLinesFrom, myLinesTo);
            
        }
       
        [Test]
        public void Face_Delaunay_MIConvexHull()
        {
            
            string fileNameLong = path + "\\KinectFace1.obj";
            vertices = IOUtils.ReadObjFile_ToVertices(fileNameLong);

            //string fileNameLong = path + "\\1.xyz";
            //vertices = IOUtils.ReadXYZFile_ToVertices(fileNameLong, false);

            List<Vertex2D> verticesDelaunay = new List<Vertex2D>();

            for (int i = 0; i < vertices.Count; i++)
            {
                Vertex2D v = new Vertex2D(vertices[i].Vector.X, vertices[i].Vector.Y, i);
                verticesDelaunay.Add(v);

            }
            
            VoronoiMesh<Vertex2D, Cell2D, VoronoiEdge<Vertex2D, Cell2D>> voronoiMesh;
            voronoiMesh = VoronoiMesh.Create<Vertex2D, Cell2D>(verticesDelaunay);
            List<Triangle> listTriangle = new List<Triangle>();
            foreach (Cell2D cell in voronoiMesh.Cells)
            {
                Triangle t = new Triangle();
               
                t.IndVertices.Add(cell.Vertices[0].IndexInModel);
                t.IndVertices.Add(cell.Vertices[1].IndexInModel);
                t.IndVertices.Add(cell.Vertices[2].IndexInModel);
                listTriangle.Add(t);
                //Vertex2D[] vert = cell.Vertices;
            }

            

            //-------------------
            Model3D myModel = new Model3D("Face");
            Model3D.AssignModelDataFromVertices(myModel, vertices);
            Model3D.SetModelTriangles(myModel, listTriangle);

            
            //Model3D.Save_OBJ(myModel, path, "1_triangulated.obj");
            ShowModel(myModel, true);

        }
     
     
     
    }
}
