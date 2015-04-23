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
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;
 
 

namespace OpenTKLib
{
    public partial class OpenGLControl
    {
        public OpenGLRenderer GLrender;
        private Model3D modelOld;
        public OpenFileDialog openModel;


        public void LoadModelFromFile(string fileName, bool resetModelToOrigin)
        {
            string errorText = string.Empty;
            Model3D model = this.GLrender.LoadModel(fileName, errorText);
            if (model != null)
            {
                if (resetModelToOrigin)
                    model.ResetModelToOrigin();
                AddModel(model);
                ShowModels();
            }
        }

        private void LoadFileDialog()
        {
            this.openModel = new OpenFileDialog();
            if (this.openModel.ShowDialog() != DialogResult.OK)
                return;
            if (Path.GetExtension(this.openModel.FileName) != ".OpenTKLibSim")
            {
               
                loadFile();
                
             
            }
            else
            {
               
                LoadModelFromFile(this.openModel.FileName, true);

            }

        }

        public void ShowModel(Model3D myModel)
        {
            AddModel(myModel);
            ShowModels();


        }

        public void AddModel(Model3D myModel)
        {
            lock (myModel) 
                this.GLrender.Models3D.Add(myModel);
            this.GLrender.SelectModel(this.GLrender.Models3D.Count - 1);
            this.comboModels.Items.Add((object)myModel.Name);
            this.comboModels.SelectedIndex = this.comboModels.Items.Count - 1;
            myModel.ModelRenderStyle = this.modelRenderStyle;
        }
        public void ShowModels()
        {
            if (this.GLrender.Models3D == null)
                return;
            if (this.GLrender.Models3D.Count > 1)
                this.modelOld = this.GLrender.Models3D[this.GLrender.Models3D.Count - 1];
            this.RefreshView_MakeCurrent();

        }
   
        public void RemoveModel(int indModel)
        {
            if (indModel < 0 || this.GLrender.Models3D[indModel].Name.StartsWith("|"))
                return;
            for (int index = 0; index < this.GLrender.Models3D[indModel].Parts.Count; ++index)
                GL.DeleteLists(this.GLrender.Models3D[indModel].Parts[index].GLListNumber, 1);
            lock (this.GLrender.Models3D)
                this.GLrender.RemoveModel(indModel);
            this.comboModels.Items.RemoveAt(indModel);
            //this.comboModels.SelectedText = "";

            if(comboModels.Items.Count > 0)
                comboModels.SelectedIndex = 0;
            GC.Collect();

            if(GLrender.Models3D.Count > 0)
            {
                Model3D model3D = GLrender.Models3D[0];
                ShowModels();
                GLrender.SelectModel(0);
            }
            this.glControl1.Refresh();
        }
        private void WriteModelName(int modelInd)
        {
            this.comboModels.Items.Add((object)this.GLrender.Models3D[modelInd].Name);
        }

  

        private int[] readTriangles(int indModelo)
        {
            int num1 = 0;
            int num2 = 0;
            for (int index1 = 0; index1 < this.GLrender.Models3D[indModelo].Parts.Count; ++index1)
            {
                for (int index2 = 0; index2 < this.GLrender.Models3D[indModelo].Parts[index1].Triangles.Count; ++index2)
                {
                    num1 += this.GLrender.Models3D[indModelo].Parts[index1].Triangles[index2].IndVertices.Count - 2;
                    ++num2;
                }
            }
            if (num1 != 0)
            {
                int[] numArray = new int[3 * num1];
                int index1 = 0;
                for (int index2 = 0; index2 < this.GLrender.Models3D[indModelo].Parts.Count; ++index2)
                {
                    for (int index3 = 0; index3 < this.GLrender.Models3D[indModelo].Parts[index2].Triangles.Count; ++index3)
                    {
                        for (int index4 = 0; index4 < this.GLrender.Models3D[indModelo].Parts[index2].Triangles[index3].IndVertices.Count - 2; ++index4)
                        {
                            numArray[index1] = this.GLrender.Models3D[indModelo].Parts[index2].Triangles[index3].IndVertices[index4];
                            numArray[index1 + 1] = this.GLrender.Models3D[indModelo].Parts[index2].Triangles[index3].IndVertices[index4 + 1];
                            numArray[index1 + 2] = this.GLrender.Models3D[indModelo].Parts[index2].Triangles[index3].IndVertices[index4 + 2];
                            index1 += 3;
                        }
                    }
                }
                return numArray;
            }
            else
            {
                int[] numArray = new int[num2 * 3];
                int index1 = 0;
                for (int index2 = 0; index2 < this.GLrender.Models3D[indModelo].Parts.Count; ++index2)
                {
                    for (int index3 = 0; index3 < this.GLrender.Models3D[indModelo].Parts[index2].Triangles.Count; ++index3)
                    {
                        numArray[index1] = this.GLrender.Models3D[indModelo].Parts[index2].Triangles[index3].IndVertices[0];
                        numArray[index1 + 1] = this.GLrender.Models3D[indModelo].Parts[index2].Triangles[index3].IndVertices[1];
                        numArray[index1 + 2] = this.GLrender.Models3D[indModelo].Parts[index2].Triangles[index3].IndVertices[1];
                        index1 += 3;
                    }
                }
                return numArray;
            }
        }

        private void loadFile()
        {
            Model3D model3D;
            for (int index = 0; index < this.openModel.FileNames.Length; ++index)
            {
                string errorText = string.Empty;
                model3D = this.GLrender.LoadModel(this.openModel.FileNames[index], errorText);
                if (model3D != null)
                {
                    AddModel(model3D);
                    ShowModels();

                }
            }
        }
       


    }
}
