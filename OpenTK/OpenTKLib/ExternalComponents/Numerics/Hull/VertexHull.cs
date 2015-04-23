using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTKLib;


namespace NLinear
{
    public class VertexHull
    {
        public Vector3<double> Vector;
        public int Condition = -1;
        public double Radius = 0;
        public VertexHull() { }
        public VertexHull(Vector3<double> pt, double radius)
        {
            this.Vector = pt; 
            Radius = radius;
        }
    }
  
   
}
