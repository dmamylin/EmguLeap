using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace OpenTKLib
{
    public class PlaneOld
    {

        Vector3d normal;
        double w;

        public PlaneOld(Vector3d normal, double w)
        {
            this.normal = normal;
            this.w = w;
        }
        public static PlaneOld PlaneFromPoints(Vector3d a, Vector3d b, Vector3d c)
        {
            Vector3d n = b - a;
            Vector3d temp = c - a;
            n = Vector3d.Cross(n, temp);
            n /= n.Length;
            double d = Vector3d.Dot(n, a);

            //var n = b.Minus(a).cross(c.Minus(a)).unit();
            return new PlaneOld(n, d);
        }

   
        public void flip() 
        {
            this.normal = this.normal.Negate();
            this.w = -this.w;
        }
    }
}
