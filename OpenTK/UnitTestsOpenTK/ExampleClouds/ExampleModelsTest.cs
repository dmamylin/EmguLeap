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
    public class ExampleModelsTest : TestBase
    {

        [Test]
        public void Cuboid()
        {
            Model3D myModel = Example3DModels.Cuboid("Cuboid", 20f, 40f, 100, new Vector3d(1, 1, 1), null);
            ShowModel(myModel, true);
        }
        [Test]
        public void Cylinder()
        {
            Model3D myModel = Example3DModels.Cylinder("cylinder", 3f, 5f, 100, new Vector3d(1,1,1), null);
            ShowModel(myModel, true);
        }
        [Test]
        public void Disk()
        {
            Model3D myModel = Example3DModels.Disk("Disk", 30f, 50f, 100, new Vector3d(1, 1, 1), null);
            ShowModel(myModel, true);
        }
        [Test]
        public void Cone()
        {
            Model3D myModel = Example3DModels.Cone("Cone", 30f, 100f, 100, new Vector3d(1, 1, 1), null);
            ShowModel(myModel, true);
        }
         [Test]
        public void Sphere()
        {
            Model3D myModel = Example3DModels.Sphere("Sphere", 20f,100, new Vector3d(1, 1, 1), null);
            ShowModel(myModel, true);
        }
       

     
     
    }
}
