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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Globalization;
using System.Windows.Media;
using OpenTK;

namespace OpenTKLib
{
    public class PointUtils
    {
        public static List<Vector3d> CopyVector3(List<Vector3d> pointsTarget)
        {
            List<Vector3d> tempPoints = new List<Vector3d>();


            for (int i = (pointsTarget.Count - 1); i >= 0; i--)
            {
                Vector3d point1 = pointsTarget[i];
                tempPoints.Add(point1); 

            }
            return tempPoints;

        }
        public static List<Vertex> CopyVertices(List<Vertex> pointsTarget)
        {
            List<Vertex> tempPoints = new List<Vertex>();


            for (int i = (pointsTarget.Count - 1); i >= 0; i--)
            {
                Vertex point1 = pointsTarget[i];
                tempPoints.Add(point1);

            }
            return tempPoints;

        }

        public static void RemoveVector3d(ref List<Vector3d> pointsTarget, ref List<Vector3d> pointsSource, List<int> indices)
        {
            List<Vector3d> temp1 = new List<Vector3d>();
            List<Vector3d> temp2 = new List<Vector3d>();

            //temp.ShallowCopy(this.PointsTarget.GetPoints());

            indices.Sort();
            int indexNew = -1;
            for (int iPoint = (pointsTarget.Count - 1); iPoint >= 0; iPoint--)
            {
                Vector3d point1 = pointsTarget[iPoint];
                Vector3d point2 = pointsSource[iPoint];
                bool bfound = false;
                for (int i = (indices.Count - 1); i >= 0; i--)
                {
                    if (indices[i] == iPoint)
                    {
                        bfound = true;
                        break;
                    }
                }
                if (!bfound)
                {
                    indexNew++;
                    temp1.Add(point1);
                    temp2.Add(point2);
                    
                }
            }
            pointsTarget = temp1;
            pointsSource = temp2;

        }
        public static void RemoveVertex(ref List<Vertex> pointsTarget, ref List<Vertex> pointsSource, List<int> indices)
        {
            List<Vertex> temp1 = new List<Vertex>();
            List<Vertex> temp2 = new List<Vertex>();

            //temp.ShallowCopy(this.PointsTarget.GetPoints());

            indices.Sort();
            int indexNew = -1;
            for (int iPoint = (pointsTarget.Count - 1); iPoint >= 0; iPoint--)
            {
                Vertex point1 = pointsTarget[iPoint];
                Vertex point2 = pointsSource[iPoint];
                bool bfound = false;
                for (int i = (indices.Count - 1); i >= 0; i--)
                {
                    if (indices[i] == iPoint)
                    {
                        bfound = true;
                        break;
                    }
                }
                if (!bfound)
                {
                    indexNew++;
                    temp1.Add(point1);
                    temp2.Add(point2);

                }
            }
            pointsTarget = temp1;
            pointsSource = temp2;

        }
        public static double CalculateTotalDistance(List<Vertex> a, List<Vertex> b)
        {

            double totaldist = 0;
            for (int i = 0; i < a.Count; i++)
            {
                Vertex p1 = a[i];
                Vertex p2 = b[i];
                double dist = (Vector3d.Subtract(p1.Vector, p2.Vector)).Length;
                
                totaldist += dist;

            }

            return totaldist;
        }
    }
}
