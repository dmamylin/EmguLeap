using System;
using System.Collections;
using System.Collections.Generic;



namespace OpenTKLib
{

    /// <summary>
    /// A data item which is stored in each kd node.
    /// </summary>
    public class EllipseWrapper
    {
        //public bool Filled;
        public double x;
        public double y;
        public double z;
        public Vertex Vertex;
        public EllipseWrapper(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.Vertex = new Vertex(x, y, z);
            //this.Filled = false;
        }
        public EllipseWrapper(Vertex myVertex)
        {
            this.x = myVertex.Vector.X;
            this.y = myVertex.Vector.Y;
            this.z = myVertex.Vector.Z;
            this.Vertex = myVertex;
            //this.Filled = false;
        }
    }
    public enum KDTreeMode
    {
        Rednaxala,
        Stark,
        Numerics,
        BruteForce
    }
}