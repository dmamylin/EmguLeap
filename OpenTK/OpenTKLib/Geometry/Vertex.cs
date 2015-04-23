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
using System.Globalization;
using System.IO;
using System.Reflection;
using OpenTK;

namespace OpenTKLib
{
    public class Vertex
    {
        public List<int> IndTriangles = new List<int>();
        public List<int> IndPart = new List<int>();
        public List<int> IndexNormals = new List<int>();

        public List<int> IndexNeighbours = new List<int>();
        public List<double> DistanceNeighbours = new List<double>();

        public Vector3d Vector;
        public float[] Color;
        

        //necessary for triangulation
        public int IndexInModel;
        public bool Marked;
        public double LengthSquared;

        public Vertex()
        {
        }
        public Vertex(Vector3d v)
        {
            Vector = v;
        }
        public Vertex(Vertex v)
        {

            Vector = new Vector3d(v.Vector.X, v.Vector.Y, v.Vector.Z);
            this.IndexInModel = v.IndexInModel;
            this.Color = v.Color;
            LengthSquared = v.LengthSquared;

        }
        
        public Vertex(double x, double y, double z)
        {
            Vector = new Vector3d(x, y, z);
            LengthSquared = Vector.LengthSquared;
        }
        public Vertex(Vector3d v, float[] color)
        {
            Vector = new Vector3d(v.X, v.Y, v.Z);
            if (color != null)
               Color = new float[4] { color[0], color[1], color[2], color[3] };
            LengthSquared = Vector.LengthSquared;
        }
        public Vertex(int myindexInModel, double x, double y, double z)
        {
            IndexInModel = myindexInModel;
            Vector = new Vector3d(x, y, z);
            LengthSquared = Vector.LengthSquared;

        }
        public Vertex(int indexInModel, Vertex v)
        {
            IndexInModel = indexInModel;
            Vector = new Vector3d(v.Vector.X, v.Vector.Y, v.Vector.Z);
            LengthSquared = v.LengthSquared;
        }
        public Vertex(int indexInModel, Vector3d v)
        {
            IndexInModel = indexInModel;
            Vector = new Vector3d(v.X, v.Y, v.Z);
            LengthSquared = v.LengthSquared;
        }
      
        public override string ToString()
        {

            string returnString = Vector.X.ToString("F1") + "  " + Vector.Y.ToString("F1") + "  " + Vector.Z.ToString("F1");

            if (Color != null && Color.Length > 0)
            {
                returnString += " :Color: " + Color[0].ToString("0.0") + ";" + Color[1].ToString("0.0") + ";" + Color[2].ToString("0.0") + ";" + Color[3].ToString("0.0");
            }
            return returnString;
        }
        
     

    }


}
