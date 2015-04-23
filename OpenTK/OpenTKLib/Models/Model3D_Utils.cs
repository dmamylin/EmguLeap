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
using OpenCLTemplate;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Reflection;
using OpenTK;


namespace OpenTKLib
{
    public partial class Model3D
    {
       
        /// <summary>Returns a Bitmap containing a text drawn. Useful to set as texture.</summary>
        /// <param name="s">String to be written</param>
        /// <param name="TextFont">Font to use</param>
        /// <param name="TextLeftColor">Left color of Text.</param>
        /// <param name="TextRightColor">Right color of Text.</param>
        /// <param name="BackgroundLeftColor">Left color of Background.</param>
        /// <param name="BackgroundRightColor">Right color of Background.</param>
        public static System.Drawing.Bitmap DrawString(string s, Font TextFont,System.Drawing.Color TextLeftColor,System.Drawing.Color TextRightColor,System.Drawing.Color BackgroundLeftColor,System.Drawing.Color BackgroundRightColor)
        {
            if (s == "")
                return (System.Drawing.Bitmap)null;
            System.Drawing.Bitmap bitmap1 = new System.Drawing.Bitmap(10, 10);
            SizeF sizeF = Graphics.FromImage((Image)bitmap1).MeasureString(s, TextFont);
            System.Drawing.Bitmap bitmap2 = new System.Drawing.Bitmap((int)sizeF.Width, (int)sizeF.Height);
            Graphics graphics = Graphics.FromImage((Image)bitmap2);
            Brush brush1 = (Brush)new LinearGradientBrush(new PointF(0.0f, 0.0f), new PointF(sizeF.Width, sizeF.Height), BackgroundLeftColor, BackgroundRightColor);
            graphics.FillRectangle(brush1, 0, 0, bitmap2.Width, bitmap2.Height);
            Brush brush2 = (Brush)new LinearGradientBrush(new PointF(0.0f, 0.0f), new PointF(sizeF.Width, sizeF.Height), TextLeftColor, TextRightColor);
            graphics.DrawString(s, TextFont, brush2, 0.0f, 0.0f);
            bitmap1.Dispose();
            return bitmap2;
        }

        public static System.Drawing.Bitmap DrawString(string s, Font TextFont,System.Drawing.Color TextColor,System.Drawing.Color BackgroundColor)
        {
            return Model3D.DrawString(s, TextFont, TextColor, TextColor, BackgroundColor, BackgroundColor);
        }

        public static System.Drawing.Bitmap DrawString(string s, Font TextFont)
        {
            return Model3D.DrawString(s, TextFont,System.Drawing.Color.Black,System.Drawing.Color.Black,System.Drawing.Color.White,System.Drawing.Color.White);
        }

        public static void AdjustCompatibility(DataTable OpenTKLibTbl)
        {
            if (!(OpenTKLibTbl.Rows[0]["OpenTKLibVersion"].ToString() != ((object)Assembly.GetExecutingAssembly().GetName().Version).ToString()))
                return;
            if (OpenTKLibTbl.Columns.IndexOf("intWireframe") >= 0)
                OpenTKLibTbl.Columns["intWireframe"].ColumnName = "intRenderStyle";
            if (OpenTKLibTbl.Columns.IndexOf("ModelName") < 0)
            {
                DataColumn column = new DataColumn("ModelName", Type.GetType("System.String"));
                OpenTKLibTbl.Columns.Add(column);
                OpenTKLibTbl.Rows[0]["ModelName"] = (object)"";
            }
        }

       
    }
}
