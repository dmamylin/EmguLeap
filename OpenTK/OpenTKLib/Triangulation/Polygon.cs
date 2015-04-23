//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using OpenTK;

//namespace OpenTKLib
//{
//    // # class Polygon

//    // Represents a convex polygon. The vertices used to initialize a polygon must
//    // be coplanar and form a convex loop. They do not have to be `CSG.Vertex`
//    // instances but they must behave similarly (duck typing can be used for
//    // customization).
//    // 
//    // Each convex polygon has a `shared` property, which is shared between all
//    // polygons that are clones of each other or were split from the same polygon.
//    // This can be used to define per-polygon properties (such as surface color).


//    public class Polygon
//    {
//        public List<Vector3d> vertices;
//        public Plane plane;
//        bool shared;

//        public Polygon(List<Vector3d> vertices, bool shared)
//        {
//            this.vertices = vertices;
//            this.shared = shared;
//            this.plane = Plane.PlaneFromPoints(vertices[0], vertices[1], vertices[2]);
//        }
        
//        public Polygon clone() 
//        {
//            return new Polygon(vertices, this.shared);
//        }
//        public void flip() 
//        {
//          //this.vertices.reverse().map(function(v) { v.flip(); });
//          this.plane.flip();
//        }
       

//    }

//}
