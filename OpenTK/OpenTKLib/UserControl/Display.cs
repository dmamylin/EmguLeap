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
        public bool DrawAtZero = false;
        CLEnum.CLRenderStyle modelRenderStyle; // pont, wireframe etc.

        private void initGLControl()
        {
            
            this.glControl1.BackColor =System.Drawing.Color.Black;
            
            this.glControl1.Name = "glControl1";
            this.glControl1.VSync = false;
            this.glControl1.Top = 0;
            this.glControl1.Left = 0;
            this.glControl1.Width = this.Width;
            this.glControl1.Height = this.Height;
            this.glControl1.Cursor = Cursors.Cross;

            initEventHandlers();
            
            this.GLrender = new OpenGLRenderer(this.glControl1);
            this.GLrender.Draw();

            
            int num = 0;
            while (num <= 100)
            {
                this.comboTransparency.Items.Add((object)num.ToString());
                num += 10;
            }


            InitialSettingsOnLoad();

        }
        private void SetComboSelection(ToolStripComboBox combo , string selection)
        {
            for (int i = 0; i < combo.Items.Count; i++)
            {
                if (combo.Items[i].ToString() == selection)
                {
                    combo.SelectedIndex = i;
                    break;
                }
            }
        }
        private void InitialSettingsOnLoad()
        {
            this.comboRightMouse.SelectedIndex = 0;
            this.comboTransparency.SelectedIndex = 0;
            
            comboCamera.SelectedIndex = 0;

            if (GLSettings.ShowAxis)
                showAxesToolStripMenuItem.Text = "Hide Axis";
            else
                showAxesToolStripMenuItem.Text = "Show Axis";

            SetComboSelection(this.comboViewMode, GLSettings.ViewMode);

        }
        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            this.glControl1.MakeCurrent();
            if (this.DrawAtZero)
            {
                this.DrawAtZero = false;
                this.GLrender.Draw();
            }
            else
                this.GLrender.Draw();
        }

        private void glControl1_Resize(object sender, EventArgs e)
        {
            if (this.glControl1 == null)
                return;
            this.glControl1.Width = this.Width;
            this.glControl1.Height = this.Height;
            if (this.glControl1.Width < 0)
                this.glControl1.Width = 1;
            if (this.glControl1.Height < 0)
                this.glControl1.Height = 1;
            this.glControl1.MakeCurrent();
            GL.Viewport(0, 0, this.glControl1.Width, this.glControl1.Height);
            this.glControl1.Invalidate();
        }
      
    }
}
