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

namespace OpenTKLib
{
    public partial class SettingsForm : Form
    {
        public OpenGLControl Parent;
        public SettingsForm(OpenGLControl myParent)
        {
            this.Parent = myParent;
            InitializeComponent();
            this.textBoxPointSize.Text = GLSettings.PointSize.ToString("0.00" );
            this.textBoxPointSizeAxis.Text = GLSettings.PointSizeAxis.ToString("0.00");
          
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            GLSettings.PointSize = Convert.ToSingle(this.textBoxPointSize.Text);
            GLSettings.PointSizeAxis = Convert.ToSingle(this.textBoxPointSizeAxis.Text);
            Parent.RedrawModels();

            this.Close();
        }

        private void buttonColorBack_Click(object sender, EventArgs e)
        {
            ColorDialog backColor = new ColorDialog();

            // Update the text box color if the user clicks OK 
            if (backColor.ShowDialog() == DialogResult.OK)
            {
                Parent.ChangeColorOfModels(backColor.Color.R, backColor.Color.G, backColor.Color.B);
                ////Execute scripted command
                //ExecCommand("BackColor|" + backColor.Color.R.ToString()
                //    + ";" + backColor.Color.G.ToString() + ";" + backColor.Color.B.ToString());
            }
        }

        private void buttonColorBackground_Click(object sender, EventArgs e)
        {
            Parent.ChangeModelColor();

        }

        private void buttonBackColor_Click(object sender, EventArgs e)
        {
            Parent.ChangeBackColor();
        }
    }
}
