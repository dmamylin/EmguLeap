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
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
 

namespace OpenTKLib
{
    public partial class OpenGLControl
    {
        private System.Drawing.Point mousePos = new System.Drawing.Point(0, 0);
        private System.Drawing.Point mousePosOriginal = new System.Drawing.Point(0, 0);
        private bool mouseClicked = false;
        private int cameraMode = 0;
        private bool clickedDirect = false;
        private int xOrigin = 0;
        private int yOrigin = 0;
        public Form ParentFormWindow;


        private void initEventHandlers()
        {

            this.glControl1.MouseMove += new MouseEventHandler(this.glControl1_MouseMove);
            this.glControl1.MouseDown += new MouseEventHandler(this.glControl1_MouseDown);
            this.glControl1.MouseUp += new MouseEventHandler(this.glControl1_MouseUp);
            this.glControl1.MouseWheel += new MouseEventHandler(this.glControl1_MouseWheel);
            this.glControl1.KeyDown += new KeyEventHandler(this.glControl1_KeyDown);

            this.glControl1.Paint += new PaintEventHandler(this.glControl1_Paint);
            this.glControl1.Resize += new EventHandler(this.glControl1_Resize);

        }
      
        private void glControl1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && !this.mouseClicked)
            {
                this.mouseClicked = true;
                mousePosOriginal.X = e.X;
                mousePosOriginal.Y = e.Y;
            }
            if (e.Button != MouseButtons.Right || this.clickedDirect)
                return;
            this.clickedDirect = true;
            this.xOrigin = e.X;
            this.yOrigin = e.Y;
        }

        private void glControl1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.mouseClicked = false;
                this.GLrender.ConsolidateMove();
            }
            if (e.Button != MouseButtons.Right || this.GLrender.Models3D.Count <= 0)
                return;
            this.clickedDirect = false;
            this.GLrender.ConsolidaMoveModel();
        }

        private void glControl1_MouseMove(object sender, MouseEventArgs e)
        {
            this.mousePos.X = e.X;
            this.mousePos.Y = e.Y;
            if (e.Button == MouseButtons.Left && this.mouseClicked)
            {
                this.GLrender.RepositionCamera((float)e.X - (float)this.mousePosOriginal.X, (float)e.Y - (float)this.mousePosOriginal.Y, this.cameraMode);
                this.glControl1.Refresh();
            }
            if (e.Button != MouseButtons.Right || this.comboModels.SelectedIndex < 0)
                return;
            this.GLrender.MoveModel(this.comboModels.SelectedIndex, this.comboRightMouse.SelectedIndex, e.X - e.Y - this.xOrigin + this.yOrigin);
            
        }

        private void glControl1_MouseWheel(object sender, MouseEventArgs e)
        {
            this.GLrender.distEye *= 1.0 - (double)e.Delta * 0.001;
            this.GLrender.zFar = (float)this.GLrender.distEye * 5f;
            this.GLrender.RepositionCamera(0.0f, 0.0f, this.cameraMode);

            //Debug.WriteLine("View distance set to " + this.GLrender.distEye.ToString());
            //this.status("View distance set to " + this.GLrender.distEye.ToString(), -1);
            this.glControl1.Refresh();
        }
        private void glControl1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.togglefullscreenToolStripMenuItem_Click(sender, new EventArgs());
            bool flag = false;
            if (e.KeyCode == Keys.W)
            {
                this.GLrender.center -= (double)this.GLrender.zFar * 0.002 * this.GLrender.front;
                this.GLrender.eye -= (double)this.GLrender.zFar * 0.002 * this.GLrender.front;
                flag = true;
            }
            if (e.KeyCode == Keys.S)
            {
                this.GLrender.center += (double)this.GLrender.zFar * 0.002 * this.GLrender.front;
                this.GLrender.eye += (double)this.GLrender.zFar * 0.002 * this.GLrender.front;
                flag = true;
            }
            if (e.KeyCode == Keys.A)
            {
                this.GLrender.center -= (double)this.GLrender.zFar * 0.001 * this.GLrender.esq;
                this.GLrender.eye -= (double)this.GLrender.zFar * 0.001 * this.GLrender.esq;
                flag = true;
            }
            if (e.KeyCode == Keys.D)
            {
                this.GLrender.center += (double)this.GLrender.zFar * 0.001 * this.GLrender.esq;
                this.GLrender.eye += (double)this.GLrender.zFar * 0.001 * this.GLrender.esq;
                flag = true;
            }
            double num1 = Math.Cos(0.01);
            double num2 = Math.Sin(0.01);
            if (e.KeyCode == Keys.NumPad4)
            {
                Vector3d vector1 = new Vector3d(this.GLrender.front);
                Vector3d vector2 = new Vector3d(this.GLrender.esq);
                this.GLrender.front = num1 * vector1 + num2 * vector2;
                this.GLrender.esq = -num2 * vector1 + num1 * vector2;
                this.GLrender.center = this.GLrender.eye - this.GLrender.front * this.GLrender.distEye;
                flag = true;
            }
            if (e.KeyCode == Keys.NumPad6)
            {
                Vector3d vector1 = new Vector3d(this.GLrender.front);
                Vector3d vector2 = new Vector3d(this.GLrender.esq);
                this.GLrender.front = num1 * vector1 - num2 * vector2;
                this.GLrender.esq = num2 * vector1 + num1 * vector2;
                this.GLrender.center = this.GLrender.eye - this.GLrender.front * this.GLrender.distEye;
                flag = true;
            }
            if (e.KeyCode == Keys.NumPad2)
            {
                Vector3d vector1 = new Vector3d(this.GLrender.front);
                Vector3d vector2 = new Vector3d(this.GLrender.up);
                this.GLrender.front = num1 * vector1 + num2 * vector2;
                this.GLrender.up = -num2 * vector1 + num1 * vector2;
                this.GLrender.center = this.GLrender.eye - this.GLrender.front * this.GLrender.distEye;
                flag = true;
            }
            if (e.KeyCode == Keys.NumPad8)
            {
                Vector3d vector1 = new Vector3d(this.GLrender.front);
                Vector3d vector2 = new Vector3d(this.GLrender.up);
                this.GLrender.front = num1 * vector1 - num2 * vector2;
                this.GLrender.up = num2 * vector1 + num1 * vector2;
                this.GLrender.center = this.GLrender.eye - this.GLrender.front * this.GLrender.distEye;
                flag = true;
            }
            if (!flag)
                return;
            this.glControl1.Invalidate();
        }
      
    }
}
