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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;


namespace OpenTKLib
{
    public partial class OpenGLControl : System.Windows.Forms.UserControl
    {
        int vertexPointer;
        int colorPointer;
        int verticesLength;

        public OpenGLControl()
        {
            InitializeComponent();
            initEventHandlers();


            for (int i = 0; i < Enum.GetValues(typeof(CLEnum.CLRenderStyle)).GetLength(0); i++)
            {
                string strVal = Enum.GetValues(typeof(CLEnum.CLRenderStyle)).GetValue(i).ToString();

               
                comboViewMode.Items.Add(strVal);
            }
            this.modelRenderStyle = CLEnum.CLRenderStyle.Point;
            comboViewMode.SelectedIndex = comboViewMode.Items.Count - 1;
            

            this.initGLControl();
           

        }
        private void OpenGLControl_Load(object sender, EventArgs e)
        {

           // this.initGLControl();

        }


        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Vector3d colorVector = new Vector3d(1, 0, 0);

            Model3D Model1 = Example3DModels.Sphere("Sphere", 2, 8, colorVector, (System.Drawing.Bitmap)null);
            AddModel(Model1);
            this.glControl1.Refresh();
        }

        private void newModelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Model3D Model = new Model3D();
            this.GLrender.Models3D.Add(Model);
            this.comboModels.Items.Add((object)Model.Name);
        }

        private void testPointCloudToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TestData.CreateTestData(out vertexPointer, out colorPointer, out verticesLength);

        }

    

     
        private void loadModelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadFileDialog();
        }

     

        

        private void comboViewMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            //do not change during load control
            if (this.GLrender == null)
                return;

            string strDisplay = Enum.GetValues(typeof(CLEnum.CLRenderStyle)).GetValue(comboViewMode.SelectedIndex).ToString();
            
            GLSettings.ViewMode = strDisplay;

            modelRenderStyle = CLEnum.CLRenderStyle.Point;
            for (int i = 0; i < Enum.GetValues(typeof(CLEnum.CLRenderStyle)).GetLength(0); i++)
            {

                string strVal = Enum.GetValues(typeof(CLEnum.CLRenderStyle)).GetValue(i).ToString();
                if (strVal == strDisplay)
                {
                    modelRenderStyle = (CLEnum.CLRenderStyle)Enum.GetValues(typeof(CLEnum.CLRenderStyle)).GetValue(i);
                    break;
                }

            }
            ChangeDisplayMode(modelRenderStyle);
        }
      
        private void toolStripButtonColor_Click(object sender, EventArgs e)
        {
            ChangeModelColor();

   
        }
        private void SetColor(string command)
        {
            string[] s = command.Split('|');
            s = s[1].Split(';');
            int ind;
                int R, G, B;
            int.TryParse(s[0], out ind);
            int.TryParse(s[1], out R);
            int.TryParse(s[2], out G);
            int.TryParse(s[3], out B);


            int indexModel = comboModels.SelectedIndex;
            for (int i = 0; i < GLrender.Models3D[ind].Parts.Count; i++)
            {
                Vertices.ColorDelete(GLrender.Models3D[indexModel].VertexList);
                GLrender.Models3D[indexModel].Parts[i].ColorOverall.X = (double)R / 255.0;
                GLrender.Models3D[indexModel].Parts[i].ColorOverall.Y = (double)G / 255.0;
                GLrender.Models3D[indexModel].Parts[i].ColorOverall.Z = (double)B / 255.0;
            }

            GLrender.Models3D[comboModels.SelectedIndex].ForceRedraw = true;
            GLrender.Draw();
            glControl1.Refresh();
        }

     
       
      
        private void RefreshView_MakeCurrent()
        {
            this.glControl1.MakeCurrent();

            this.GLrender.Draw();
            this.glControl1.Refresh();


        }
        private void RefreshView(bool forceRedraw)
        {
            for (int i = 0; i < GLrender.Models3D.Count; i++)
            {
                GLrender.Models3D[i].ForceRedraw = true;
                
            }
            this.glControl1.Invalidate();
            
           
        }
        private void comboTransparency_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            if (comboModels.SelectedIndex >= 0)
            {
                float transp;
                float.TryParse(comboTransparency.Text, out transp);
                transp *= 0.01f;
                float alpha = 1 - transp;
                for (int i = 0; i < GLrender.Models3D[comboModels.SelectedIndex].Parts.Count; i++)
                {
                    Vertices.ChangeTransparency(GLrender.Models3D[comboModels.SelectedIndex].VertexList, alpha);
                   
                }
            }

            RefreshView(true);

        }

        private void comboCamera_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboCamera.SelectedIndex == 0)
            {
                this.cameraMode = GLrender.MODE_ROT;
                glControl1.Cursor = Cursors.Cross;
            }
            else
            {
                cameraMode = GLrender.MODE_TRANSL;
                glControl1.Cursor = Cursors.NoMove2D;
            }
        }
        public void SaveSettings()
        {
            GLSettings.SaveSettings();

        }

    
       

        private void showAxesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (showAxesToolStripMenuItem.Text == "Show Axis")
            {
                GLSettings.ShowAxis = true;
                showAxesToolStripMenuItem.Text = "Hide Axis";
                glControl1.Refresh();
            }
            else if (showAxesToolStripMenuItem.Text == "Hide Axis")
            {
                GLSettings.ShowAxis = false;
                showAxesToolStripMenuItem.Text = "Show Axis";
                glControl1.Refresh();
            }
        }

    

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutForm sf = new AboutForm();
            sf.ShowDialog();

        }

        private void removeSelectedModelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            RemoveModel(comboModels.SelectedIndex);

        }

        private void togglefullscreenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ParentFormWindow != null)
            {
                if (ParentFormWindow.FormBorderStyle == FormBorderStyle.None)
                {
                    this.glControl1.SendToBack();
                    ParentFormWindow.FormBorderStyle = FormBorderStyle.Sizable;
                }
                else
                {
                    this.glControl1.BringToFront();
                    ParentFormWindow.FormBorderStyle = FormBorderStyle.None;
                    ParentFormWindow.WindowState = FormWindowState.Maximized;
                }
            }
        }

        private void convexHullToolStripMenuItem_Click(object sender, EventArgs e)
        {

            int indexModel = comboModels.SelectedIndex;
            Model3D myModel = GLrender.Models3D[comboModels.SelectedIndex];



            List<Vector3d> myListVectors = Vertices.ConvertToVector3dList(myModel.VertexList);
            ConvexHull3D convHull = new ConvexHull3D(myListVectors);

        }

        private void triangulateNearestNeighbourToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void saveNormalsOfCurrentModelToolStripMenuItem_Click(object sender, EventArgs e)
        {


            Model3D myModel = GLrender.Models3D[comboModels.SelectedIndex];
            CheckNormals(myModel);
            string path = AppDomain.CurrentDomain.BaseDirectory + "TestData";

                      
            Model3D.Save_OBJ(myModel, path, myModel.Name + ".obj");

        }
        private void CheckNormals(Model3D myModel)
        {
            if (myModel.Normals.Count == 0)
            {
                myModel.CalculateNormals_Triangulation();
            }
            
            
        }
        private void showNormalsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (showNormalsToolStripMenuItem.Text == "Show Normals")
            {
                GLSettings.ShowNormals = true;
                showNormalsToolStripMenuItem.Text = "Hide Normals";
                
                Model3D myModel = GLrender.Models3D[comboModels.SelectedIndex];
                CheckNormals(myModel);
                this.GLrender.CreateLinesForNormals(myModel);
                RefreshView(true);

            }
            else if (showNormalsToolStripMenuItem.Text == "Hide Normals")
            {
                GLSettings.ShowNormals = false;
                showNormalsToolStripMenuItem.Text = "Show Normals";
                this.GLrender.DeleteLinesForNormals();
                RefreshView(true);
                
            }

            

        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingsForm sf = new SettingsForm(this);
            if (sf.ShowDialog() == DialogResult.OK)
            {
                RefreshView(true);
                
            }
        }

     

      

     
       
    }
}
