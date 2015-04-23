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
    public class KDTreeTest : TestBase
    {
        
        public KDTreeTest():base()
        {
                     
        }

        [Test]
        public void KDTreeTest_Stark()
        {
            TimeCalc.ResetTime();
            vertices = Vertices.CreateCube_RandomPointsOnPlanes(1, 1000);
            List<Vertex> result = new List<Vertex>();

            KDTree_Stark tree = KDTree_Stark.Build(vertices);

            for (int i = 0; i < vertices.Count; i++)
            {
                int indexNearest = tree.FindNearest_ExcludeTakenPoints(vertices[i]);
                result.Add(vertices[indexNearest]);
            }

            TimeCalc.ShowLastTimeSpan("KDTree RednaxelaTest");

        }
        [Test]
        public void KDTree3D_NumericsTest()
        {


        }
        
         [Test]
        public void KDTree_RednaxelaTest()
        {
            TimeCalc.ResetTime();

            vertices = Vertices.CreateCube_RandomPointsOnPlanes(1, 1000);

            KDTreeVertex kv = new KDTreeVertex();
            kv.BuildKDTree_Rednaxela(vertices);
            kv.InitVertices(vertices);
            kv.FindNearest_Points_Rednaxela(vertices);


            TimeCalc.ShowLastTimeSpan("KDTree RednaxelaTest");

        }
       
        [Test]
        public void KDTreeTest_Stark_Translation()
        {
            TimeCalc.ResetTime();
            vertices = Vertices.CreateCube_RandomPointsOnPlanes(1, 1000);

            List<Vertex> target = vertices;

            List<Vertex> source = Vertices.CopyVertices(vertices);

            VertexUtils.TranslateVertices(source, 100, 100, 100);



            List<Vertex> result = new List<Vertex>();
            KDTree_Stark tree = KDTree_Stark.Build(target);
            
            for (int i = 0; i < source.Count; i++)
            {
               
                int indexNearest = tree.FindNearest_ExcludeTakenPoints(source[i]);
                result.Add(target[indexNearest]);
                
            }
            TimeCalc.ShowLastTimeSpan("KDTree RednaxelaTest");


        }
         [Test]
         public void KDTreeTest_StarkBruteForce()
         {

             vertices = Vertices.CreateCube_RandomPointsOnPlanes(1, 1000);

             List<Vertex> target = vertices;

             List<Vertex> source = Vertices.CopyVertices(vertices);
             VertexUtils.RotateVertices30Degrees(source);

             List<Vertex> result = new List<Vertex>();
             

             for (int i = 0; i < source.Count; i++)
             {
                 KDTree_Stark tree = KDTree_Stark.Build(target);
                 int indexNearest = tree.FindNearest(source[i]);
                 result.Add(target[indexNearest]);
                 target.RemoveAt(indexNearest);

             }


         }
   
      
    }
}
