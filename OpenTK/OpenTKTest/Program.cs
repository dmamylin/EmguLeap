

using System;
using System.Windows.Forms;

using OpenTKLib;

namespace OpenTKTest
{
  internal static class Program
  {
    [STAThread]
    public static void Main(string[] args)
    {
    
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run((Form)new OpenTKTestForm());
      //Application.Run((Form)new Form3D());

    }
  }
}
