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
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Globalization;
using System.Windows.Media;
using OpenTK;


namespace OpenTKLib
{
    
    public class MathBase
    {
        public static float Pi_Float = 3.14159265358979f;
        public static float DegreesToRadians_Float = 0.017453292f;
        public static float RadiansToDegrees_Float = 57.2957795131f;
        public static double DegreesToRadians = 0.017453292519943295;
        public static double Pi = 3.1415926535897932384626;
        public static double RadiansToDegrees = 57.29577951308232;

       
         public static double DistanceBetweenVectors(Vector3d v1, Vector3d v2)
        {

            return (Vector3d.Subtract(v1, v2)).Length;
        }
        
    }
}
