﻿// Pogramming by
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTKLib
{
    public static class GLSettings
    {
        public static bool ShowAxis;
        public static bool ShowNormals;
        public static float PointSize = 1;
        public static float PointSizeAxis = 1;
        public static string ViewMode;


        public static void InitFromSettings()
        {
            ShowAxis = OpenTKLib.Properties.Settings.Default.ShowAxis;
            PointSize = OpenTKLib.Properties.Settings.Default.PointSize;
            PointSizeAxis = OpenTKLib.Properties.Settings.Default.PointSizeAxis;
            ViewMode = OpenTKLib.Properties.Settings.Default.ViewMode;
            

        }
        public static void SaveSettings()
        {
            OpenTKLib.Properties.Settings.Default.ShowAxis = ShowAxis;
            OpenTKLib.Properties.Settings.Default.PointSize = PointSize ;
            OpenTKLib.Properties.Settings.Default.PointSizeAxis = PointSizeAxis ;
            OpenTKLib.Properties.Settings.Default.ViewMode = ViewMode ;
            OpenTKLib.Properties.Settings.Default.Save();
        }
        public static void SetDefaultSettings()
        {
            
            SaveSettings();
            InitFromSettings();

        }
    }
}
