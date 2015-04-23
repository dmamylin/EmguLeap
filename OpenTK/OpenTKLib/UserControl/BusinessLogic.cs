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
using OpenTKLib.Properties;
using OpenCLTemplate;
using OpenTK;
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
        private void ChangeDisplayMode(CLEnum.CLRenderStyle displayMode)
        {
            for (int i = 0; i < this.GLrender.Models3D.Count; i++)
            {
                this.GLrender.Models3D[i].ModelRenderStyle = displayMode;
                
                
            }
            RefreshView(true);
            
        }
        public void ChangeBackColor()
        {
            ColorDialog backColor = new ColorDialog();

            // Update the text box color if the user clicks OK 
            if (backColor.ShowDialog() == DialogResult.OK)
            {
                //Execute scripted command
                //ExecCommand("BackColor|" + backColor.Color.R.ToString()
                //    + ";" + backColor.Color.G.ToString() + ";" + backColor.Color.B.ToString());

                this.GLrender.ClearColor[0] = backColor.Color.R;
                this.GLrender.ClearColor[1] = backColor.Color.G;
                this.GLrender.ClearColor[2] = backColor.Color.B;
                for (int index = 0; index < 3; ++index)
                    this.GLrender.ClearColor[index] /= (float)byte.MaxValue;
                this.glControl1.Invalidate();
            }
        }
        public void ChangeModelColor()
        {
            if (comboModels.SelectedIndex >= 0)
            {
                ColorDialog colDiag = new ColorDialog();
                // Sets the initial color select to the current text color.

                // Update the text box color if the user clicks OK 
                if (colDiag.ShowDialog() == DialogResult.OK)
                {
                    SetColor("SetColor|" + comboModels.SelectedIndex.ToString() + ";" + colDiag.Color.R.ToString()
                        + ";" + colDiag.Color.G.ToString() + ";" + colDiag.Color.B.ToString());
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Please load a 3D object first");

            }
        }
        public void RedrawModels()
        {
            for (int i = 0; i < this.GLrender.Models3D.Count; i++)
            {
                this.GLrender.Models3D[this.comboModels.SelectedIndex].ForceRedraw = true;
            }
            this.GLrender.Draw();
            this.glControl1.Refresh();
        }
        public void ChangeColorOfModels(int r, int g, int b)
        {
            for (int i = 0; i < this.GLrender.Models3D.Count; i++)
            {
                for (int index = 0; index < this.GLrender.Models3D[i].Parts.Count; ++index)
                {
                    this.GLrender.Models3D[this.comboModels.SelectedIndex].Parts[index].ColorOverall.X = (float)r / (float)byte.MaxValue;
                    this.GLrender.Models3D[this.comboModels.SelectedIndex].Parts[index].ColorOverall.Y = (float)g / (float)byte.MaxValue;
                    this.GLrender.Models3D[this.comboModels.SelectedIndex].Parts[index].ColorOverall.Z = (float)b / (float)byte.MaxValue;
                }
                
            }
            RedrawModels();
        }
        private void ExecCommand(string command)
        {
            string[] strArray1 = command.Split('|');
            if (strArray1[0].ToLower() == "loadmodel")
            {
                string file = strArray1[1];
                string errorMessage = string.Empty;
                if (this.GLrender.LoadModel(file, errorMessage) == null)
                    throw new Exception("Error loading file");
                string[] strArray2 = file.Split('\\');
                this.comboModels.Items.Add((object)strArray2[strArray2.Length - 1]);
                this.comboModels.SelectedIndex = this.comboModels.Items.Count - 1;
                //this.status("File " + file + " opened.", -1);
            }
            else if (strArray1[0].ToLower() == "setcolor")
            {
                string[] strArray2 = strArray1[1].Split(';');
                int result1;
                int.TryParse(strArray2[0], out result1);
                int result2;
                int.TryParse(strArray2[1], out result2);
                int result3;
                int.TryParse(strArray2[2], out result3);
                int result4;
                int.TryParse(strArray2[3], out result4);
                ChangeColorOfModels(result2, result3, result4);

                //for (int index = 0; index < this.GLrender.Models3D[result1].Parts.Count; ++index)
                //{
                //    this.GLrender.Models3D[this.comboModels.SelectedIndex].Parts[index].ColorOverall.X = (float)result2 / (float)byte.MaxValue;
                //    this.GLrender.Models3D[this.comboModels.SelectedIndex].Parts[index].ColorOverall.Y = (float)result3 / (float)byte.MaxValue;
                //    this.GLrender.Models3D[this.comboModels.SelectedIndex].Parts[index].ColorOverall.Z = (float)result4 / (float)byte.MaxValue;
                //}
                //this.GLrender.Models3D[this.comboModels.SelectedIndex].ForceRedraw = true;
                //this.GLrender.Draw();
                ////this.GLrender.Draw(this.varTime);
                //this.glControl1.Refresh();
            }
            else if (strArray1[0].ToLower() == "setdisplacement")
            {
                string[] strArray2 = strArray1[1].Split(';');
                int result1;
                int.TryParse(strArray2[0], out result1);
                float result2;
                float.TryParse(strArray2[1], out result2);
                float result3;
                float.TryParse(strArray2[2], out result3);
                float result4;
                float.TryParse(strArray2[3], out result4);
                float result5;
                float.TryParse(strArray2[4], out result5);
                float result6;
                float.TryParse(strArray2[5], out result6);
                float result7;
                float.TryParse(strArray2[6], out result7);
                this.GLrender.Models3D[result1].vetTransl.X = (float)result2;
                this.GLrender.Models3D[result1].vetTransl.Y = (float)result3;
                this.GLrender.Models3D[result1].vetTransl.Z = (float)result4;
                this.GLrender.Models3D[result1].vetRot.X = (float)result5;
                this.GLrender.Models3D[result1].vetRot.Y = (float)result6;
                this.GLrender.Models3D[result1].vetRot.Z = (float)result7;
                this.GLrender.Draw();
                this.glControl1.Refresh();
            }
            else if (strArray1[0].ToLower() == "backcolor")
            {
                string[] strArray2 = strArray1[1].Split(';');
                float.TryParse(strArray2[0], out this.GLrender.ClearColor[0]);
                float.TryParse(strArray2[1], out this.GLrender.ClearColor[1]);
                float.TryParse(strArray2[2], out this.GLrender.ClearColor[2]);
                for (int index = 0; index < 3; ++index)
                    this.GLrender.ClearColor[index] /= (float)byte.MaxValue;
                this.glControl1.Invalidate();
            }
           
        }

    }
}
