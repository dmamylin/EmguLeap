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
    public class TestBase
    {
        protected static string path;
        protected List<Vertex> vertices = null;
        protected List<Vertex> verticesTransformed = null;

        protected List<Vertex> linesFrom = null;
        protected List<Vertex> linesTo = null;

        public TestBase()
        {
            path = AppDomain.CurrentDomain.BaseDirectory + "TestData";

        }

        protected void ShowVerticesInWindow(byte[] colorModel1, byte[] colorModel2)
        {
            OpenTKTestForm fOTK = new OpenTKTestForm();

            fOTK.ShowListOfVertices(vertices, colorModel1);
            
            if(verticesTransformed != null && verticesTransformed.Count > 0)
                fOTK.ShowListOfVertices(verticesTransformed, colorModel2);
            
            fOTK.ShowDialog();
            
        }
        protected void ShowModel(Model3D myModel, bool removeAllOthers)
        {
            OpenTKTestForm fOTK = new OpenTKTestForm();

            fOTK.ShowModel(myModel, removeAllOthers);
            

            TimeCalc.ShowLastTimeSpan("Show Model");
            fOTK.ShowDialog();

        }
         protected void ShowModel_WithLines(Model3D myModel, bool removeAllOthers, List<Vertex> myLinesFrom, List<Vertex> myLinesTo)
        {
            OpenTKTestForm fOTK = new OpenTKTestForm();
            fOTK.OpenGLControl.GLrender.LinesFrom = myLinesFrom;
            fOTK.OpenGLControl.GLrender.LinesTo = myLinesTo;

            fOTK.ShowModel(myModel, removeAllOthers);
           


            TimeCalc.ShowLastTimeSpan("Show Model");
            fOTK.ShowDialog();

        }


        protected void ShowModel_WithLines(Model3D myModel, bool removeAllOthers)
        {
            OpenTKTestForm fOTK = new OpenTKTestForm();

            fOTK.SetLineData(this.linesFrom, this.linesTo);

            fOTK.ShowModel(myModel, removeAllOthers);
        

            TimeCalc.ShowLastTimeSpan("Show Model");
            fOTK.ShowDialog();

           


        }
        protected void CreateLinesForNormals(Model3D myModel)
        {
            if (myModel.Normals == null || myModel.VertexList.Count != myModel.Normals.Count)
            {
                System.Windows.Forms.MessageBox.Show("Normals not calculated right ");
                return;
            }
            linesFrom = new List<Vertex>();
            linesTo = new List<Vertex>();

            for (int i = 0; i < myModel.VertexList.Count; i++ )
            {
                linesFrom.Add(myModel.VertexList[i]);
                linesTo.Add(new Vertex(myModel.Normals[i]));


            }
              
        }
        protected void ResetLists()
        {
            vertices = null;
            verticesTransformed = null;
        }


        private void UpateModel_Faces(Model3D myModel, List<cFace> listFaces)
        {
            List<Triangle> listTriangle = new List<Triangle>();

            System.Diagnostics.Debug.WriteLine("Number of faces " + listFaces.Count.ToString());
            for (int i = 0; i < listFaces.Count; i++)
            {
                cFace face = listFaces[i];
                Triangle a = new Triangle();
               
                for (int j = 0; j < face.Vertices.Length; j++)
                {
                    a.IndVertices.Add(face.Vertices[j].IndexInModel);

                }
               
                listTriangle.Add(a);
            }


            Part p = new Part();
            p.Triangles = listTriangle;
            myModel.Parts.Add(p);

            myModel.Helper_AdaptNormalsForEachVertex();
            myModel.CalculateBoundingBox(true);
        }
        protected Model3D CreateModel(string modelName, List<Vertex> myListVertices, List<cFace> listFaces)
        {
            Model3D myModel = new Model3D();
            myModel.Name = modelName;
            myModel.VertexList = myListVertices;

            UpateModel_Faces(myModel, listFaces);


           

            return myModel;
        }
    
      

       
    }
}
