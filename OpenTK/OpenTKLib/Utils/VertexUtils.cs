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
    public class VertexUtils
    {
        public static void ScaleByVector(List<Vertex> vectorlList, Vertex v)
        {
            
           
            Matrix3d R = Matrix3d.Identity;
            R[0, 0] = v.Vector.X;
            R[1, 1] = v.Vector.Y;
            R[2, 2] = v.Vector.Z;

            MatrixUtilsOpenTK.RotateVertices(vectorlList, R);

        }
        public static void ScaleByFactor(List<Vertex> vectorlList, double scale)
        {
            Vector3d scaleVector = new Vector3d(scale, scale, scale);

            Matrix3d R = Matrix3d.Identity;
            R[0, 0] = scaleVector[0];
            R[1, 1] = scaleVector[1];
            R[2, 2] = scaleVector[2];

            MatrixUtilsOpenTK.RotateVertices(vectorlList, R);

        }
        public static void PerspectiveTransform(List<Vertex> vectorlList, double d)
        {
            //Vector3d scaleVector = new Vector3d(x, y, z);
            for (int i = 0; i < vectorlList.Count; i++)
            {
                Vertex v = vectorlList[i];
                v.Vector.X = v.Vector.X * d / v.Vector.Z;
                v.Vector.Y = v.Vector.Y * d / v.Vector.Z;
                v.Vector.Z = d;
                vectorlList[i] = v;
            }
        
        }

        public static void RotateVertices30Degrees(List<Vertex> vectorlList)
        {
            Matrix3d R = Matrix3d.Identity;
            //rotation 30 degrees
            R[0, 0] = 1F;
            R[1, 1] = R[2, 2] = 0.86603F;
            R[1, 2] = -0.5F;
            R[2, 1] = 0.5F;


            MatrixUtilsOpenTK.RotateVertices(vectorlList, R);


        }
      
   

        public static void RotateVertices(List<Vertex> vertexList, Matrix3d R)
        {
            List<Vector3d> listVectors = Vertices.ConvertToVector3dList(vertexList);

            MatrixUtilsOpenTK.RotateVectors(listVectors, R);
            Vertices.AssignNewVectorList(vertexList, listVectors);

            
        }
        public static void TranslateVertices(List<Vertex> vectorlList, double x, double y, double z)
        {
            for (int i = 0; i < vectorlList.Count; i++)
            {
                Vector3d translation = new Vector3d(x, y, z);
                Vertex v = vectorlList[i];
                Vector3d translatedV = Vector3d.Add(v.Vector, translation);
                v.Vector = translatedV;
                vectorlList[i] = v;
            }

        }
        public static void CreateOutliers(List<Vertex> vertices, int numberOfOutliers)
        {
            int numberIterate = 0;
            for (int i = vertices.Count -1; i >=0 ; i--)
            {
                //Vector3d p = vectors[i].Vector;
                Vector3d perturb = new Vector3d(vertices[i].Vector);

                numberIterate++;
                if (numberIterate > numberOfOutliers)
                    return;

                
                if (i % 3 == 0)
                {
                    perturb.X *= 1.2; perturb.Y *= 1.3; perturb.Z *= 1.05;
                }
                else if (i % 3 == 1)
                {
                    perturb.X *= 1.4; perturb.Y *=  0.9; perturb.Z *= 1.2;
                }
                else
                {
                    perturb.X *= 0.9; perturb.Y *= 1.2; perturb.Z *= 1.1;
                }
                vertices.Add(new Vertex(perturb));
                
            }



        }
        public static void PerturbVectors(List<Vector3d> vectors)
        {

            for (int i = 0; i < vectors.Count; i++)
            {
                Vector3d p = vectors[i];


                Vector3d perturb = new Vector3d();
                if (i % 3 == 0)
                {
                    perturb.X = 2; perturb.Y = 0; perturb.Z = 1;
                }
                else if (i % 3 == 1)
                {
                    perturb.X = 0; perturb.Y = 2; perturb.Z = 2;
                }
                else
                {
                    perturb.X = 1; perturb.Y = 3; perturb.Z = 4;
                }
                p = Vector3d.Add(p, perturb);


                vectors[i] = p;
               
            }



        }
        public static List<Vector3d> CloneListVector3d(List<Vector3d> myOldList)
        {
            List<Vector3d> myListNew = new List<Vector3d>();
               for(int i = 0; i < myOldList.Count; i++)
            {
                Vector3d v = myOldList[i];
                Vector3d vNew = new Vector3d(v);
                myListNew.Add(vNew);
            }
               return myListNew;
        }
        public static List<Vertex> CloneListVertex(List<Vertex> myOldList)
        {
            List<Vertex> myListNew = new List<Vertex>();
            for (int i = 0; i < myOldList.Count; i++)
            {
                Vertex v = myOldList[i];
                Vertex vNew = new Vertex(v.Vector);
                if (v.Color != null)
                    vNew.Color = v.Color;
                myListNew.Add(vNew);
            }
            return myListNew;
        }

        public static Matrix3d CreateARotationMatrix()
        {
            Matrix3d R = Matrix3d.CreateRotationX(90);

            Matrix3d temp = Matrix3d.CreateRotationY(130);
            R = Matrix3d.Mult(R, temp);
            temp = Matrix3d.CreateRotationZ(320);
            R = Matrix3d.Mult(R, temp);
            return R;

        }
        public static ushort[] CreatePointArrayOneDim_FromVertixes(List<Vertex> listVertices, int width, int height)
        {
            List<Vector3d> listVectors = Vertices.ConvertToVector3dList(listVertices);
            ushort[,] points = CreatePointMatrixFromPoint3DList(listVectors, width, height);
            ushort[] pointArr = CreatePointArrayOneDim(points, width, height);


            return pointArr;


        }
        public static ushort[] CreatePointArrayOneDim_FromVectors3d(List<Vector3d> listVectors, int width, int height)
        {
            ushort[,] points = CreatePointMatrixFromPoint3DList(listVectors, width, height);
            ushort[] pointArr = CreatePointArrayOneDim(points, width, height);


            return pointArr;


        }
        public static ushort[,] CreatePointMatrixFromPoint3DList(List<Vector3d> listVectors, int width, int height)
        {
            ushort[,] points = new ushort[width, height];


            for (int i = 0; i < listVectors.Count; i++)
            {

                Vector3d p3D = listVectors[i];
                points[Convert.ToInt32(listVectors[i].X), Convert.ToInt32(listVectors[i].Y)] = Convert.ToUInt16(listVectors[i].Z);

            }
            return points;


        }
        public static double[] SplitVertices(List<Vertex> listVectors, ref byte[] myColorPixels, int width, int height)
        {
            double[] pointArray = new double[width * height];

            int BYTES_PER_PIXEL = (PixelFormats.Bgr32.BitsPerPixel + 7) / 8;
            myColorPixels = new byte[width * height * BYTES_PER_PIXEL];

            for (int i = 0; i < listVectors.Count; i++)
            {
                int x = Convert.ToInt32(listVectors[i].Vector.X);
                int y = Convert.ToInt32(listVectors[i].Vector.Y);
                int depthIndex = (y * width) + x;
                Vertex p3D = listVectors[i];
                pointArray[depthIndex] = listVectors[i].Vector.Z;

                myColorPixels[depthIndex + 0] = Convert.ToByte(listVectors[i].Color[0] * 255); //colorInfoR[x, y];
                myColorPixels[depthIndex + 1] = Convert.ToByte(listVectors[i].Color[1] * 255);
                myColorPixels[depthIndex + 2] = Convert.ToByte(listVectors[i].Color[2] * 255);
                myColorPixels[depthIndex + 3] = Convert.ToByte(listVectors[i].Color[3] * 255);

                //colorArray2D[Convert.ToInt32(listVectors[i].Vector.X), Convert.ToInt32(listVectors[i].Vector.Y)] = Convert.ToUInt16(listVectors[i].Vector.Z);

            }



            return pointArray;


        }
        public static ushort[] CreatePointMatrixFromVertexData(List<Vertex> listVectors, ref byte[] myColorPixels, int width, int height)
        {
            ushort[] pointArray = new ushort[width * height];
            
            int BYTES_PER_PIXEL = (PixelFormats.Bgr32.BitsPerPixel + 7) / 8;
            myColorPixels = new byte[width * height * BYTES_PER_PIXEL];

            for (int i = 0; i < listVectors.Count; i++)
            {
                int x = Convert.ToInt32(listVectors[i].Vector.X);
                int y = Convert.ToInt32(listVectors[i].Vector.Y);
                int depthIndex = (y * width) + x;
                Vertex p3D = listVectors[i];
                pointArray[depthIndex] = Convert.ToUInt16(listVectors[i].Vector.Z);

                myColorPixels[depthIndex + 0] = Convert.ToByte(listVectors[i].Color[0] * 255); //colorInfoR[x, y];
                myColorPixels[depthIndex + 1] = Convert.ToByte(listVectors[i].Color[1] * 255);
                myColorPixels[depthIndex + 2] = Convert.ToByte(listVectors[i].Color[2] * 255);
                myColorPixels[depthIndex + 3] = Convert.ToByte(listVectors[i].Color[3] * 255);

                //colorArray2D[Convert.ToInt32(listVectors[i].Vector.X), Convert.ToInt32(listVectors[i].Vector.Y)] = Convert.ToUInt16(listVectors[i].Vector.Z);
                
            }
            

            
            return pointArray;


        }
      
        public static ushort[] CreatePointArrayOneDim(ushort[,] pointMatrix, int width, int height)
        {
            ushort[] pointArray = new ushort[width * height];

            //int nIndex = -1;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int depthIndex = (y * width) + x;
                    pointArray[depthIndex] = pointMatrix[x, y];

                }


            }
            return pointArray;


        }
     
        public static List<Vector3d> CreateVectorsFromPoint3D(List<Point3D> listPoints)
        {
            List<Vector3d> listOfVectors = new List<Vector3d>();
            for (int i = 0; i < listPoints.Count; i++)
            {
                Point3D myPoint = listPoints[i];

                listOfVectors.Add(new Vector3d(myPoint.X, myPoint.Y, myPoint.Z));
            }


            return listOfVectors;
        }
 
        public static List<Vector3d> CreateVector3DFromPoint3D(List<Point3D> listPoints)
        {
            List<Vector3d> listOfVectors = new List<Vector3d>();
            for (int i = 0; i < listPoints.Count; i++)
            {
                Point3D myPoint = listPoints[i];

                listOfVectors.Add(new Vector3d(myPoint.X, myPoint.Y, myPoint.Z));
            }


            return listOfVectors;
        }
   

   
    }
}
