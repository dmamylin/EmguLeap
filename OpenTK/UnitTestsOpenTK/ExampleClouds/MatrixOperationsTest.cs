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
    public class MatrixOperationsTest : TestBase
    {
      
      
        [Test]
        public void TranslateCuboid()
        {
            this.vertices = Vertices.CreateCuboid(5, 8, 60);
            verticesTransformed = VertexUtils.CloneListVertex(vertices);
            VertexUtils.TranslateVertices(verticesTransformed, 30, -20, 12);
            ShowVerticesInWindow(new byte[4] { 255, 255, 255, 255 }, new byte[4] { 255, 0, 0, 255 });
                      
        }
        [Test]
        public void RotateCuboid()
        {
            this.vertices = Vertices.CreateCuboid(5, 8, 60);
            verticesTransformed = VertexUtils.CloneListVertex(vertices);

            Matrix3d R = VertexUtils.CreateARotationMatrix();
            VertexUtils.RotateVertices(verticesTransformed, R);

            ShowVerticesInWindow(new byte[4] { 255, 255, 255, 255 }, new byte[4] { 255, 0, 0, 255 });
        }
        [Test]
        public void ScaleCuboid()
        {
            this.vertices = Vertices.CreateCuboid(5, 8, 60);
            verticesTransformed = VertexUtils.CloneListVertex(vertices);

            VertexUtils.ScaleByVector(verticesTransformed, new Vertex(1, 2, 3));
            ShowVerticesInWindow(new byte[4] { 255, 255, 255, 255 }, new byte[4] { 255, 0, 0, 255 });
        }

        [Test]
        public void RotateScaleTranslate()
        {
            this.vertices = Vertices.CreateCuboid(5, 8, 60);

            verticesTransformed = VertexUtils.CloneListVertex(vertices);
            Matrix3d R = VertexUtils.CreateARotationMatrix();
            VertexUtils.RotateVertices(verticesTransformed, R);
            VertexUtils.TranslateVertices(verticesTransformed, 30, -20, 12);
            VertexUtils.ScaleByVector(verticesTransformed, new Vertex(1, 2, 3));
            ShowVerticesInWindow(new byte[4] { 255, 255, 255, 255 }, new byte[4] { 255, 0, 0, 255 });
        }
     
    }
}
