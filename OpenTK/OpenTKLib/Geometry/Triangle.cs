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
 

    public class Triangle
    {
        public List<int> IndVertices;
        public List<int> IndNormals;
        public List<int> IndTextures;
        
        public Triangle()
        {
            IndVertices = new List<int>();
            IndNormals = new List<int>();
            IndTextures = new List<int>();

        }
        public Triangle(int i, int j, int k) : this()
        {
            this.IndVertices.Add(i);
            this.IndVertices.Add(j);
            this.IndVertices.Add(k);
        }
       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public static Triangle CreateTriangle(int i, int j, int k)
        {
            Triangle a = new Triangle();
           
            a.IndVertices.Add(i);
            a.IndVertices.Add(j);
            a.IndVertices.Add(k);

            return a;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="k"></param>
        /// <param name="l"></param>
        /// <returns></returns>
        public static Triangle CreateTriangle(int i, int j, int k, int l)
        {
            Triangle a = CreateTriangle(i, j, k);
            a.IndVertices.Add(l);

            return a;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vi"></param>
        /// <param name="vj"></param>
        /// <param name="vk"></param>
        /// <returns></returns>
        private Triangle CreateTriangle(Vertex vi, Vertex vj, Vertex vk)
        {
            Triangle a = new Triangle();
           
            a.IndVertices.Add(vi.IndexInModel);
            a.IndVertices.Add(vj.IndexInModel);
            a.IndVertices.Add(vk.IndexInModel);

            return a;

        }

        /// <summary>
        /// Helper method
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="k"></param>
        /// <param name="listTriangle"></param>
        public static void AddTriangleToList(int i, int j, int k, List<Triangle> listTriangle)
        {
            Triangle a = Triangle.CreateTriangle(i, j, k);
           listTriangle.Add(a);
            
        }
   
        /// <summary>
        /// 
        /// </summary>
        /// <param name="areas"></param>
        public static void SortIndexVerticesWithinAllTriangles(List<Triangle> areas)
        {
            for (int i = 0; i < areas.Count; i++)
            {
                Triangle a = areas[i];
                a.IndVertices.Sort();

            }

        }
        /// <summary>
        /// Check by IndVertices. The areas are already sorted  (performing sort here would make the performance be bad)
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public bool Equals(Triangle b)
        {
            if (this.IndVertices.Count != b.IndVertices.Count)
                return false;
            for (int i = 0; i < this.IndVertices.Count; i++ )
            {
                if (this.IndVertices[i] != b.IndVertices[i])
                    return false;
            }
            return true;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            for (int i = 0; i < this.IndVertices.Count; i++)
            {
                sb.Append(" : " + this.IndVertices[i].ToString() );

            }

            
            return sb.ToString();
        }
        /// <summary>
        /// is a very time consuming method - use only with list size smaller than 10,000
        /// </summary>
        /// <param name="listTriangle"></param>
        public static void CheckForDuplicates(List<Triangle> listTriangle)
        {

            System.Diagnostics.Debug.WriteLine("Number of areas before check: " + listTriangle.Count.ToString());


            for (int i = listTriangle.Count - 1; i >= 0; i--)
            {
                Triangle ai = listTriangle[i];
                for (int j = 0; j < i; j++)
                {
                    if (ai.Equals(listTriangle[j]))
                    {
                        listTriangle.RemoveAt(i);
                        break;
                    }
                }

            }
            System.Diagnostics.Debug.WriteLine("Number of areas AFTER check: " + listTriangle.Count.ToString());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="myModel"></param>
        /// <param name="t"></param>
        public static void CalculateNormal_UpdateNormalList(Model3D myModel, Triangle t)
        {

            Vector3d normal = CalculateNormalForTriangle(myModel.VertexList, t);
            
            if (normal != null)
            {
                myModel.Normals.Add(normal);
                int indNewNormal = myModel.Normals.Count - 1;
                
                
                t.IndNormals.Add(indNewNormal);
                //adds the normal to each of the vertices in the triangle
                for (int i = 0; i < t.IndVertices.Count; i++ )
                {
                    int indVertex = t.IndVertices[i];
                    myModel.VertexList[indVertex].IndexNormals.Add(indNewNormal);
                }
                   
            }


        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vector3d CalculateNormalForTriangle(List<Vertex> vertices, Triangle t)
        {

            Vector3d a = vertices[t.IndVertices[0]].Vector;
            Vector3d b = vertices[t.IndVertices[1]].Vector;
            Vector3d c = vertices[t.IndVertices[2]].Vector;

            Vector3d normal = Vector3d.Cross(b - a, c - a);
            //alternative:
            //Vector3d normal = Vector3d.Cross(b - a, c - b);

            Vector3d temp = normal;
            normal = normal.NormalizeNew();
            //if (!CheckVector(normal))
            //{
            //    System.Windows.Forms.MessageBox.Show("SW Error calculating normal");
            //    return new Vector3d(0, 0, 0);

            //}
            
            return normal;
        }
        //public static Vector3d GetNormalForTriangle(Model3D myModel, Triangle t)
        //{
        //    if (t.IndNormals == null || t.IndNormals.Count == 0)
        //    {
        //        CalculateNormalForTriangle(myModel.VertexList, t);

        //    }
        //    return myModel.Normals[t.IndNormals[0]];
        //}
        public static Vector3d CalculateNormal(Model3D myModel, Triangle t)
        {
            if (t.IndNormals == null || t.IndNormals.Count == 0)
            {
                Vector3d a = myModel.VertexList[t.IndVertices[0]].Vector;
                Vector3d b = myModel.VertexList[t.IndVertices[1]].Vector;
                Vector3d c = myModel.VertexList[t.IndVertices[2]].Vector;

                Vector3d normal = Vector3d.Cross(b - a, c - a);
                //alternative:
                //Vector3d normal = Vector3d.Cross(b - a, c - b);

                Vector3d temp = normal;
                normal = normal.NormalizeNew();
                //if (!CheckVector(normal))
                //{
                //    System.Windows.Forms.MessageBox.Show("SW Error calculating normal");
                //    return new Vector3d(0, 0, 0);

                //}
                return normal;

            }
            return myModel.Normals[t.IndNormals[0]];
        }
        private static bool CheckVector(Vector3d v)
        {
            if (double.IsInfinity(v.X) || double.IsNaN(v.X) || double.IsInfinity(v.Y) || double.IsNaN(v.Y) || double.IsInfinity(v.Z) || double.IsNaN(v.Z))
                return false;

       

            return true;


        }
    }

    /// <summary>
    /// compares according to INDEX of first, second, third vertex
    /// </summary>
    public class TriangleComparerVertices : IComparer<Triangle>
    {

        public int Compare(Triangle a, Triangle b)
        {
            if (a.IndVertices.Count != b.IndVertices.Count)
                return 0;

            for (int i = 0; i < a.IndVertices.Count; i++)
            {
                int ai = a.IndVertices[i];
                int bi = b.IndVertices[i];
                if (ai < bi)
                    return -1;
                else if (ai > bi)
                    return 1;

            }
            return 0;

        }
    }
    //}

 
}
