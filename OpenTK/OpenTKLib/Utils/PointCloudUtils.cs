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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTKLib;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using OpenTK;

namespace OpenTKLib
{
    public class PointCloudUtils
    {
        

        public static List<Vector3d> RotatePointCloudXY(List<Vector3d> oldList, int Width, int Height)
        {
           
            List<Vector3d> listOfVectors = new List<Vector3d>();


            for (int i = 0; i < oldList.Count; i++)
            {
                Vector3d v = oldList[i];
                
                double newX = Width - v.X;
                //double newX = Convert.ToDouble(p.GetValue(0));
                double newY = Height - v.Y;

                listOfVectors.Add(new Vector3d(newX, newY, v.Z));

            }

            return listOfVectors;

        }
    
     
        public static List<float[]> CreateColorInfo(byte[] arrayColor, ushort[] arrayDepth, int width, int height)
        {

            int BYTES_PER_PIXEL = (PixelFormats.Bgr32.BitsPerPixel + 7) / 8;

            List<float[]> listOfColors = new List<float[]>();
            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    int depthIndex = (y * width) + x;
                    int colorIndex = depthIndex * BYTES_PER_PIXEL;
                    ushort z = arrayDepth[depthIndex];
                    if (z > 0)
                    {
                        float[] color = new float[4]{0,0,0,0};
                        color[0] = arrayColor[colorIndex    ] / 255F  ;
                        color[1] = arrayColor[colorIndex +1 ] / 255F;
                        color[2] = arrayColor[colorIndex +2 ] / 255F;
                        color[3] = 1F;
                        listOfColors.Add(color);

                    }

                }
            }

            return listOfColors;
        }
        public static List<float[]> CreateColorList(int numberOfItems, byte red, byte green, byte blue, byte transparency)
        {

            int BYTES_PER_PIXEL = (PixelFormats.Bgr32.BitsPerPixel + 7) / 8;

            List<float[]> listOfColors = new List<float[]>();
            float[] color = new float[4] { 0, 0, 0, 0 };
            color[0] = red / 255F;
            color[1] = green / 255F;
            color[2] = blue / 255F;
            color[3] = transparency / 255F;

            for (int i = 0; i < numberOfItems; ++i)
            {
                listOfColors.Add(color);
            }

            return listOfColors;
        }

        public static List<Point3D> CreatePoint3DListFromVertices(List<Vertex> myListVertices)
        {
            List<Point3D> myListPoint3D = new List<Point3D>();
            for (int i = 0; i < myListVertices.Count; i++)
            {
                Vertex myVertex = myListVertices[i];
                Point3D p3D = new Point3D(myVertex.Vector.X, myVertex.Vector.Y, myVertex.Vector.Z);
                myListPoint3D.Add(p3D); 
            }
            return myListPoint3D;

        }
        public static List<Vertex> CreateVerticesFromDrawingPoints_IncludingCheck(List<System.Drawing.Point> pointList, List<Vertex> pointsTarget, List<System.Drawing.Point> pointOther)
        {

            List<Vertex> pointNew = new List<Vertex>();
            bool pointFound = false;

            for (int i = pointList.Count - 1; i >= 0; i--)
            {
                System.Drawing.Point pNew = pointList[i];
                for (int j = 0; j < pointsTarget.Count; j++)
                {
                    Vertex p = pointsTarget[j];
                    //add point only if it is found in the original point list
                    if (pNew.X == Convert.ToInt32(p.Vector[0]) && pNew.Y == Convert.ToInt32(p.Vector[1]))
                    {
                        pointFound = true;
                        pointNew.Add(p);
                        break;
                    }

                }
                //some error - have to check!
                if (!pointFound)
                {
                    MessageBox.Show("Error in identifying point from cloud with the stitched result: " + i.ToString());
                    pointOther.RemoveAt(i);
                }



            }
            return pointNew;

        }
    }
}
