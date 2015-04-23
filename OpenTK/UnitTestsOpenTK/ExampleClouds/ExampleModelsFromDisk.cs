using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using OpenTKLib;
using OpenTK;

namespace UnitTestsOpenTK
{
    [TestFixture]
    [Category("UnitTest")]
    public class ExampleModelsFromDisk : TestBase
    {
       
        public ExampleModelsFromDisk() : base()
        {
          
        }

  
        [Test]
        public void OpenXYZ()
        {
            string fileNameLong = path + "\\1.xyz";
            OpenTKTestForm fOTK = new OpenTKTestForm();
            fOTK.OpenGLControl.LoadModelFromFile(fileNameLong, true);
            fOTK.ShowDialog();


        }
        [Test]
        public void OpenObjFile()
        {
            string fileNameLong = path + "\\KinectFace1.obj";
            OpenTKTestForm fOTK = new OpenTKTestForm();
            fOTK.OpenGLControl.LoadModelFromFile(fileNameLong, true);
            fOTK.ShowDialog();


        }
        [Test]
        public void OpenBunnySample_Triangulated()
        {
            string fileNameLong = path + "\\Bunny.obj";
            OpenTKTestForm fOTK = new OpenTKTestForm();
            fOTK.OpenGLControl.LoadModelFromFile(fileNameLong, false);
            fOTK.ShowDialog();


        }
        [Test]
        public void OpenBunnySample_OnlyPoints()
        {
            string fileNameLong = path + "\\Bunny.xyz";
            OpenTKTestForm fOTK = new OpenTKTestForm();
            fOTK.OpenGLControl.LoadModelFromFile(fileNameLong, false);
            fOTK.ShowDialog();


        }
        public void ShowDialog()
        {
            OpenTKTestForm fOTK = new OpenTKTestForm();
            fOTK.ShowDialog();

        }
      
     
    }
}
