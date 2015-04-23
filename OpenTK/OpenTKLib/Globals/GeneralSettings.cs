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
using System.Globalization;
namespace OpenTKLib
{
    public class GeneralSettings
    {
        public static CultureInfo CurrentCulture = new CultureInfo("en-US");
        public static double AbsoluteTolerance;
        public static bool DebugMode = true;
        private static string separatorDecimal = ".";

        public static string TreatLanguageSpecifics(string language)
        {
            language = language.Replace("    ", " ");
            language = language.Replace("   ", " ");
            language = language.Replace("  ", " ");
            language = language.Replace(".", separatorDecimal);
            language = language.Replace(",", separatorDecimal);
            return language;
        }
    }
}
