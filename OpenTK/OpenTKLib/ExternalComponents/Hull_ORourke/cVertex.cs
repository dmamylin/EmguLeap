/*------------------------------------------------------------------
class cVertex.

This corresponds to the C struct cVertex -- type vertex.
It is one cell of a vertex list, holding the point v,
next and previous pointers, and Boolean flags (e.g., ear).
Its only methods besides constuctors are printing methods
and ResetVertexTo3D (which raises a point onto a paraboloid).
-------------------------------------------------------------------*/


using OpenTK;
using OpenTKLib;
using System;

namespace OpenTKLib
{

    public class cVertex
    {

        public cVertex PrevVertex, NextVertex;
        public cPointi Point;
        public bool IsEar = false;
        public int IndexInModel;
        public cEdge Edge;
        public bool IsOnHull;		/* T iff point on hull. */
        public bool IsProcessed;

        public cVertex()
        {
            PrevVertex = NextVertex = null;
            Point = new cPointi();
            IndexInModel = 0;
            Edge = null;
            IsOnHull = false;
            IsProcessed = false;
        }

        public cVertex(double i, double j)
        {
            Point = new cPointi();
            Point.X = i;
            Point.Y = j;
            Point.Z = i * i + j * j;
            PrevVertex = NextVertex = null;
        }

        public cVertex(double x, double y, double z)
        {
            Point = new cPointi();
            Point.X = x;
            Point.Y = y;
            Point.Z = z;
            PrevVertex = NextVertex = null;
        }

        /* Raises point to 3D by placing in on paraboloid */
        public void ResetVertex3D()
        {
            Point.Z = Point.X * Point.X + Point.Y * Point.Y;
        }

        public void PrintVertex(int index)
        {
            System.Diagnostics.Debug.WriteLine("V" + index + " = ");
            Point.PrintPoint();
        }

        public void PrintVertex()
        {
            Point.PrintPoint();
        }

        public void PrintVertex3D()
        {
            System.Diagnostics.Debug.WriteLine("V" + IndexInModel + " = (" + Point.X + ", " + Point.Y + ", " + Point.Z + "); ");
        }

        public void PrintVertex3D(int k)
        {
            System.Diagnostics.Debug.WriteLine("V" + k + " = (" + Point.X + ", " + Point.Y + ", " + Point.Z + "); ");
        }
        public override string ToString()
        {
            return this.Point.X.ToString() + "  " + this.Point.Y.ToString() + "  " + this.Point.Z.ToString() ;
        }
    }




}




