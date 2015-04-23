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
    public partial class OpenTKTestForm : Form
    {
       
        
        public OpenGLControl OpenGLControl;

        public OpenTKTestForm()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(CultureInfo.CurrentCulture.LCID);
            InitializeComponent();
            GLSettings.InitFromSettings();
            AddOpenGLControl();
            

        }
        private void AddOpenGLControl()
        {
            this.OpenGLControl = new OpenGLControl();
            this.SuspendLayout();
            // 
            // openGLControl1
            // 
            this.OpenGLControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OpenGLControl.Location = new System.Drawing.Point(0, 0);
            this.OpenGLControl.Name = "openGLControl1";
            this.OpenGLControl.Size = new System.Drawing.Size(854, 453);
            this.OpenGLControl.TabIndex = 0;

            panelOpenKinect.Controls.Add(this.OpenGLControl);
            //this.Controls.Add(this.openGLControl1);
           
            this.ResumeLayout(false);
        }
        public void ShowPointCloud(ushort[] depthInfo, int width, int height)
        {
            List<Vector3d> myVectors = Vertices.ConvertToVector3DList_FromArray(depthInfo, width, height);
            this.OpenGLControl.ShowPointCloud("Depth Point Cloud", myVectors, null);
           

        }
   

        protected override void OnClosed(EventArgs e)
        {
            GLSettings.SaveSettings();
            base.OnClosed(e);
        }

        public void ShowListOfVertices(List<Vertex> myVerticesList, byte[] color)
        {
            if (color != null)
            {
                List<float[]> myColors = PointCloudUtils.CreateColorList(myVerticesList.Count, color[0], color[1], color[2], color[3]);
                Vertices.SetColorToList(myVerticesList, myColors);

            }

            this.OpenGLControl.ShowPointCloud("Point Cloud", myVerticesList);

        }
        public void ShowModel(Model3D myModel, bool removeAllOthers)
        {
            if(removeAllOthers)
                this.OpenGLControl.RemoveAllModels();
            this.OpenGLControl.ShowModel(myModel);

        }
        public void SetLineData(List<Vertex> myLinesFrom, List<Vertex> myLinesTo)
        {
                 
            this.OpenGLControl.GLrender.LinesFrom = myLinesFrom;
            this.OpenGLControl.GLrender.LinesTo = myLinesTo;



        
        }
     
    }
}
