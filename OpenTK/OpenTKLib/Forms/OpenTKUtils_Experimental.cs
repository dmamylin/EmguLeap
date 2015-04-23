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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
using System.Threading;
using OpenTKLib;
using System.Windows.Media.Media3D;
using OpenTK;


namespace OpenTKLib
{
    public partial class OpenTKTestForm 
    {
     
        List<Vector3d> lastPointCloud;
        List<float[]> lastColors;

        public void ShowPointCloud(byte[] mycolorInfo, ushort[] depthInfo, int width, int height)
        {

            List<Vector3d> myVectors = Vertices.ConvertToVector3DList_FromArray(depthInfo, width, height);
            List<float[]> myColors = PointCloudUtils.CreateColorInfo(mycolorInfo, depthInfo, width, height);
            this.lastPointCloud = myVectors;
            this.lastColors = myColors;

            this.OpenGLControl.ShowPointCloud("Color Point Cloud", myVectors, myColors);

            //ColorUtils.SetColorFromColorArray2Dim(mycolorInfo, depthInfo, width, height, polydata);

            ////PointCloudUtils.Colorize(polydata);
            //ShowPointCloud(polydata);

        }
      

   
        //private void ResetModelsToOrigin()
        //{
            
        //    List<List<Vertex>> myTempList = new List<List<Vertex>>();
        //    List<string> myNames = new List<string>();
        //    for (int i = 0; i < OpenGLControl.GLrender.Models3D.Count; i++)
        //    {
        //        List<Vertex> myVertices = OpenGLControl.GLrender.Models3D[i].VertexList;
        //        myTempList.Add(myVertices);
        //        myNames.Add(OpenGLControl.GLrender.Models3D[i].Name);
        //    }
        //    OpenGLControl.RemoveAllModels();


        //    for (int i = 0; i < myTempList.Count; i++)
        //    {
                
        //        List<Vertex> myVertices = myTempList[i];
        //        Vertices.GetVertexMax(myVertices);
        //        this.OpenGLControl.ShowPointCloud(myNames[i], myVertices);
                
        //    }
            
        //}
   
     
    }
}
