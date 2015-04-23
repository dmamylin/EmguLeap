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
    public partial class Model3D
    {
        public static void SetModelTriangles(Model3D myModel, List<Triangle> listTriangle)
        {

            Part p = new Part();
            p.Triangles = listTriangle;
            myModel.Parts.Add(p);
            myModel.CalculateBoundingBox(true);

            //TimeCalc.ShowLastTimeSpan("Bounding Box");


         
        }
        private static void CreateTrianglesByNearestVertices(Model3D myModel)
        {
            List<Vertex> vertices = myModel.vertexList;
            List<Triangle> listTriangle = CreateTrianglesByNearestVertices(vertices);
            //TimeCalc.ShowLastTimeSpan("Triangles");

            if (GeneralSettings.DebugMode)
            {
                System.Diagnostics.Debug.WriteLine("Info--- Number of vertices: " + vertices.Count.ToString() + "; Number of triangles : " + listTriangle.Count.ToString());

                //Triangle.SortIndexVerticesWithinAllTriangles(listTriangle);
                //TimeCalc.ShowLastTimeSpan("Sort");

                //listTriangle.Sort(new TriangleComparerVertices());
                //TimeCalc.ShowLastTimeSpan("Sort");

                //Triangle.CheckForDuplicates(listTriangle);
                //TimeCalc.ShowLastTimeSpan("Check Duplicates");


            }
            SetModelTriangles(myModel, listTriangle);


        }
        /// <summary>
        /// Based upon the info of the nearest vertex (of each vertex), the triangles are created
        /// </summary>
        /// <param name="myModel"></param>
        private static List<Triangle> CreateTrianglesByNearestVertices(List<Vertex> vertices)
        {
                   
            List<Triangle> listTriangle = new List<Triangle>();

            //create triangles
            //for (int i = vertices.Count - 1; i >= 0; i--)
            for (int i = 0; i < vertices.Count; i++)
            {
                Vertex v = vertices[i];
               
                if (v.DistanceNeighbours.Count >= 2)
                {
                    Triangle.AddTriangleToList(v.IndexInModel, v.IndexNeighbours[0], v.IndexNeighbours[1], listTriangle);


                }
                if (v.DistanceNeighbours.Count >= 3)
                {
                    Triangle.AddTriangleToList(v.IndexInModel, v.IndexNeighbours[0], v.IndexNeighbours[2], listTriangle);
                    Triangle.AddTriangleToList(v.IndexInModel, v.IndexNeighbours[1], v.IndexNeighbours[2], listTriangle);


                }
                if (v.DistanceNeighbours.Count >= 4)
                {
                    Triangle.AddTriangleToList(v.IndexInModel, v.IndexNeighbours[0], v.IndexNeighbours[3], listTriangle);
                    Triangle.AddTriangleToList(v.IndexInModel, v.IndexNeighbours[1], v.IndexNeighbours[3], listTriangle);
                    Triangle.AddTriangleToList(v.IndexInModel, v.IndexNeighbours[2], v.IndexNeighbours[3], listTriangle);

                }
                if (v.DistanceNeighbours.Count >= 5)
                {
                    Triangle.AddTriangleToList(v.IndexInModel, v.IndexNeighbours[0], v.IndexNeighbours[4], listTriangle);
                    Triangle.AddTriangleToList(v.IndexInModel, v.IndexNeighbours[1], v.IndexNeighbours[4], listTriangle);
                    Triangle.AddTriangleToList(v.IndexInModel, v.IndexNeighbours[2], v.IndexNeighbours[4], listTriangle);
                    Triangle.AddTriangleToList(v.IndexInModel, v.IndexNeighbours[3], v.IndexNeighbours[4], listTriangle);

                }



            }

            return listTriangle;


           

        }

        /// <summary>
        /// calculates normals for each triangle, then updates the normal list, and puts also the info in each vertex
        /// </summary>
        public void Helper_NormalsFromTriangles()
        {
            //clear all normals
            this.Normals.Clear();
            for (int i = 0; i < this.vertexList.Count; i++)
            {
                this.vertexList[i].IndexNormals.Clear();
            }


            for (int i = 0; i < this.Parts.Count; i++)
            {
                for (int j = 0; j < this.Parts[i].Triangles.Count; j++)
                {
                    Triangle t = this.Parts[i].Triangles[j];
                    t.IndNormals.Clear();
                    Triangle.CalculateNormal_UpdateNormalList(this, t);

                }
            }
        }
        /// <summary>
        /// TODO: the normals for the triangles now point to wrong data
        /// </summary>
        public void Helper_AdaptNormalsForEachVertex()
        {
            List<Vector3d> newNormals = new List<Vector3d>();
            int indexNormal = -1;
            for (int i = 0; i < this.vertexList.Count; i++)
            {
                Vertex vertex = this.vertexList[i];
                Vector3d v = Vector3d.Zero;
                if (vertex.IndexNormals.Count != 0)
                {

                    Vector3d sumOfNormals = new Vector3d(0.0, 0.0, 0.0);
                    //calculate sumOfNormals
                    foreach (int index in vertex.IndexNormals)
                    {
                        if (index < this.Normals.Count)
                            sumOfNormals += this.Normals[index];
                    }
                    v = sumOfNormals * (1.0 / (double)vertex.IndexNormals.Count);
                    v = v.NormalizeNew();
                    vertex.IndexNormals.Clear();

                   
                }
                newNormals.Add(v);
                indexNormal++;
                vertex.IndexNormals.Add(indexNormal);

            }
            this.Normals = newNormals;

            if (this.Normals.Count > 0)
            {
                for (int i = 0; i < this.Parts.Count; i++)
                {
                    for (int j = 0; j < this.Parts[i].Triangles.Count; j++)
                    {
                        Triangle t = this.Parts[i].Triangles[j];
                        t.IndNormals.Clear();
                        Vertex v = vertexList[t.IndVertices[0]];
                        t.IndNormals.Add(v.IndexNormals[0]);

                    }
                }
            }
        }
    }
}
