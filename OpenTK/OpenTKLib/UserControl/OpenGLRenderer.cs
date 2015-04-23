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
using System.Drawing;
using OpenCLTemplate;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace OpenTKLib
{
    public class OpenGLRenderer
    {
        public GLControl glControl1;
        public List<Model3D> Models3D;
        public int SelectedModelIndex;

        public bool DrawStereo;
        public float StereoDistance;
        public float[] ClearColor;
        public int MODE_ROT;
        public int MODE_TRANSL;
        public Vector3d center;
        public Vector3d eye;
        public Vector3d front;
        public Vector3d up;
        public Vector3d esq;
        public double distEye;
        public float zFar;
        private Vector3d frontCpy;
        private Vector3d upCpy;
        private Vector3d esqCpy;
        private Vector3d centerCpy;
        private float paramAnt;
        private float param;
        public List<Vertex> LinesFrom;
        public List<Vertex> LinesTo;

        TextRenderer textRenderer;
        Font serif = new Font(FontFamily.GenericSerif, 24);

        
        public OpenGLRenderer(GLControl openGlControl)
        {
            Models3D = new List<Model3D>();
            float[] numArray = new float[3];
            numArray[2] = 0.5f;
            this.ClearColor = numArray;
            this.MODE_ROT = 0;
            this.MODE_TRANSL = 3;
            this.center = new Vector3d(0, 0, 0);
            this.eye = new Vector3d(215, 0, 0);
            this.front = new Vector3d(1, 0, 0);
            this.up = new Vector3d(0, 0, 1);
            this.esq = new Vector3d(0, 1, 0);
            this.distEye = 215.0;
            this.zFar = 1000f;
            this.frontCpy = new Vector3d(1, 0, 0);
            this.upCpy = new Vector3d(0, 0, 1);
            this.esqCpy = new Vector3d(0, 1, 0);
            this.centerCpy = new Vector3d(0, 0, 0);
            this.paramAnt = 0.0f;
            this.param = 0.0f;
            // ISSUE: explicit constructor call
            this.glControl1 = openGlControl;
            this.glControl1.MakeCurrent();
            GL.Enable(EnableCap.LineSmooth);
            GL.Hint(HintTarget.LineSmoothHint, HintMode.DontCare);
            GL.Enable(EnableCap.PolygonSmooth);
            GL.Hint(HintTarget.PolygonSmoothHint, HintMode.DontCare);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.ClearDepth(1.0);
            GL.DepthFunc(DepthFunction.Lequal);
            GL.Enable(EnableCap.DepthTest);
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.DontCare);
            GL.ColorMaterial(MaterialFace.FrontAndBack,ColorMaterialParameter.AmbientAndDiffuse);
            GL.Enable(EnableCap.ColorMaterial);
            float[] params1 = new float[4] { 0.5f, 0.5f, 0.5f, 1f };
            float[] params2 = new float[4] { 0.3f, 0.3f, 0.3f, 1f };
            float[] params3 = new float[4] { 0.1f, 0.1f, 0.1f, 1f };
            float[] params4 = new float[4] { 0.0f, -400f, 0.0f, 1f };
            GL.Light(LightName.Light1, LightParameter.Ambient, params1);
            GL.Light(LightName.Light1, LightParameter.Diffuse, params2);
            GL.Light(LightName.Light1, LightParameter.Specular, params3);
            GL.Light(LightName.Light1, LightParameter.Position, params4);
            GL.Enable(EnableCap.Light1);
            GL.ShadeModel(ShadingModel.Smooth);
            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.Texture2D);
            try
            {
                GL.EnableClientState(ArrayCap.VertexArray);
                GL.DisableClientState(ArrayCap.VertexArray);
            }
            catch
            {
                Model3D.HardwareSupportsBufferObjects = false;
            }
            CreateTextRenderer();
        }

        public void CreateTextRenderer()
        {
            textRenderer = new TextRenderer(10, 10);
            PointF position = PointF.Empty;
            textRenderer.Clear(System.Drawing.Color.MidnightBlue);
            textRenderer.DrawString("MyText", serif, Brushes.White, position);

        }
        public void RemoveModel(int ind)
        {
            try
            {
                foreach (Part parts in this.Models3D[ind].Parts)
                {
                    if (Model3D.HardwareSupportsBufferObjects)
                    {
                        for (int index = 0; index < parts.GLBuffers.Length; ++index)
                            GL.DeleteBuffers(1, ref parts.GLBuffers[index]);
                    }
                    else
                        GL.DeleteLists(parts.GLListNumber, 1);
                }
                if (this.Models3D[ind].GLTexture > 0)
                    GL.DeleteTexture(this.Models3D[ind].GLTexture);
            }
            catch
            {
            }
            Model3D Model = this.Models3D[ind];
           
            this.Models3D.RemoveAt(ind);
        }

        public void RepositionCamera(float mouseDX, float mouseDY, int modo)
        {
            if (modo == this.MODE_ROT)
            {
                double num1 = -3.0 * Math.PI * (double)mouseDX / (double)this.glControl1.Width;
                double num2 = -3.0 * Math.PI * (double)mouseDY / (double)this.glControl1.Height;
                Console.Write(num1.ToString());
                double num3 = Math.Cos(num2);
                double num4 = Math.Sin(num2);
                double num5 = Math.Cos(num1);
                double num6 = Math.Sin(num1);
                this.front = this.frontCpy * num3 + this.upCpy * -num4;
                this.up = num4 * this.frontCpy + this.upCpy * num3;
                Vector3d vector = new Vector3d(this.front);
                this.front = vector * num5 + num6 * this.esqCpy;
                this.esq = -num6 * vector + this.esqCpy * num5;
            }
            else if (modo == this.MODE_TRANSL)
            {
                double dx = -distEye * mouseDX / (float)glControl1.Width;
                double dy = distEye * mouseDY / (float)glControl1.Height;

                center = centerCpy + esqCpy * dx + upCpy * dy;
            }
            this.eye = this.center + this.front * this.distEye;
            this.RepositionLight();
        }

        public void Fly(Vector3d Distance)
        {
            this.center += Distance.X * this.front + Distance.Y * this.esq + Distance.Z * this.up;
            this.eye += Distance.X * this.front + Distance.Y * this.esq + Distance.Z * this.up;
        }

        public void ConsolidateMove()
        {
            this.frontCpy = new Vector3d(this.front);
            this.upCpy = new Vector3d(this.up);
            this.esqCpy = new Vector3d(this.esq);
            this.centerCpy = new Vector3d(this.center);
        }

        private void RepositionLight()
        {
            try
            {
                GL.LoadIdentity();
                GL.Light(LightName.Light1, LightParameter.Position, new float[4] { (float)this.distEye, (float)this.distEye, 0.0f, 1f });
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show("Error in Reposition light : " + err.Message);
            }
        }
     
        public Model3D LoadModel(string file, string NoOpenCLMessage)
        {
            Model3D model3D = null;
            try
            {
               
                model3D = new Model3D(file);
                
                return model3D;
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower() == "opencldisabledexception")
                {
                    int num1 = (int)MessageBox.Show(NoOpenCLMessage, "OpenTKLib", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
                else
                {
                    int num2 = (int)MessageBox.Show("Exception: " + ex.ToString(), "OpenTKLib", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
                return model3D;
            }
        }
        private void DrawStereoImages()
        {
            Vector3d vectorLeftEye = this.eye - this.distEye * (double)this.StereoDistance * this.esq;
            GL.DrawBuffer(DrawBufferMode.BackRight);
            GL.ClearColor(this.ClearColor[0], this.ClearColor[1], this.ClearColor[2], 0.0f);
            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
            GL.LoadIdentity();
            Matrix4d perspectiveFieldOfView1 = Matrix4d.CreatePerspectiveFieldOfView(Math.PI / 4.0, (double)this.glControl1.Width / (double)this.glControl1.Height, (double)this.zFar * 0.001, (double)this.zFar + 1000.0);
            GL.LoadMatrix(ref perspectiveFieldOfView1);
            Matrix4d mat1 = Matrix4d.LookAt(vectorLeftEye.X, vectorLeftEye.Y, vectorLeftEye.Z, this.center.X, this.center.Y, this.center.Z, this.up.X, this.up.Y, this.up.Z);
            GL.MultMatrix(ref mat1);

            this.DrawAll3DObjects();
            
            Vector3d vectorRightEye = this.eye + this.distEye * (double)this.StereoDistance * this.esq;
            GL.DrawBuffer(DrawBufferMode.BackLeft);
            GL.ClearColor(this.ClearColor[0], this.ClearColor[1], this.ClearColor[2], 0.0f);
            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
            GL.LoadIdentity();
            Matrix4d perspectiveFieldOfView2 = Matrix4d.CreatePerspectiveFieldOfView(Math.PI / 4.0, (double)this.glControl1.Width / (double)this.glControl1.Height, (double)this.zFar * 0.001, (double)this.zFar + 1000.0);
            GL.LoadMatrix(ref perspectiveFieldOfView2);
            Matrix4d mat2 = Matrix4d.LookAt(vectorRightEye.X, vectorRightEye.Y, vectorRightEye.Z, this.center.X, this.center.Y, this.center.Z, this.up.X, this.up.Y, this.up.Z);
            GL.MultMatrix(ref mat2);

            this.DrawAll3DObjects();
            
            this.glControl1.SwapBuffers();
        }
        private void Draw3D()
        {
            GL.DrawBuffer(DrawBufferMode.Back);
            GL.ClearColor(this.ClearColor[0], this.ClearColor[1], this.ClearColor[2], 0.0f);
            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
            GL.LoadIdentity();
            Matrix4d perspectiveFieldOfView = Matrix4d.CreatePerspectiveFieldOfView(Math.PI / 4.0, (double)this.glControl1.Width / (double)this.glControl1.Height, (double)this.zFar * 0.001, (double)this.zFar + 1000.0);
            GL.LoadMatrix(ref perspectiveFieldOfView);
            Matrix4d mat = Matrix4d.LookAt(this.eye.X, this.eye.Y, this.eye.Z, this.center.X, this.center.Y, this.center.Z, this.up.X, this.up.Y, this.up.Z);
            GL.MultMatrix(ref mat);

            this.DrawAll3DObjects();
            
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Shininess, new float[4] { 0.9f, 0.9f, 0.9f, 1f });
            this.glControl1.SwapBuffers();
        }
        public void Draw()
        {

            if ((double)this.zFar <= 0.0)
                this.zFar = 300f;
            if ((double)this.zFar > 1.00000004091848E+35)
                this.zFar = 1E+35f;
            if (this.DrawStereo)
            {
                DrawStereoImages();
            }
            else
            {
                Draw3D();
            }
        }
        private void DrawPointCloud(int vertexPointer, int colorPointer, int verticesLength, Matrix4 projection)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit |
                       ClearBufferMask.DepthBufferBit |
                       ClearBufferMask.StencilBufferBit);
          


            Matrix4 lookat = Matrix4.LookAt(0, 128, 256, 0, 0, 0, 0, 1, 0);
            Vector3d scale = new Vector3d(4, 4, 4);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref lookat);
            GL.Scale(scale);

            GL.PointSize(GLSettings.PointSize);


            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.ColorArray);


            GL.VertexPointer(3, VertexPointerType.Float, Vector3.SizeInBytes, new IntPtr(0));

            GL.BindBuffer(BufferTarget.ArrayBuffer, colorPointer);
            GL.ColorPointer(3,ColorPointerType.UnsignedByte, 0, 0);
            //-------------

            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexPointer);


            GL.DrawArrays(PrimitiveType.Points, 0, verticesLength);

            glControl1.SwapBuffers();
            glControl1.Invalidate();

        }
      
        private void DrawAxis()
        {
            GL.Begin(PrimitiveType.Lines);

            GL.Color4(1f, 1f, 1f, 1f);
            GL.Vertex3(0.0f, 0.0f, 0.0f);
            GL.Vertex3(60f, 0.0f, 0.0f);
            GL.Vertex3(0.0f, 0.0f, 0.0f);
            GL.Vertex3(0.0f, 60f, 0.0f);
            GL.Vertex3(0.0f, 0.0f, 0.0f);
            GL.Vertex3(0.0f, 0.0f, 60f);

            GL.End();
            GL.LineWidth(GLSettings.PointSizeAxis);
            GL.PointSize(GLSettings.PointSize);

            
        }
        private void DrawAxisLabels()
        {
            PointF position = PointF.Empty;
            //textRenderer.Clear(System.Drawing.Color.Red);
            textRenderer.DrawString("----TestString", serif, Brushes.White, position);

            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, textRenderer.Texture);
            GL.Begin(PrimitiveType.Quads);

            GL.TexCoord2(0.0f, 1.0f); GL.Vertex2(-1f, -1f);
            GL.TexCoord2(1.0f, 1.0f); GL.Vertex2(1f, -1f);
            GL.TexCoord2(1.0f, 0.0f); GL.Vertex2(1f, 1f);
            GL.TexCoord2(0.0f, 0.0f); GL.Vertex2(-1f, 1f);

            GL.End();

            //SwapBuffers();
        }
        public void DeleteLinesForNormals()
        {
            LinesFrom = new List<Vertex>();
            LinesTo = new List<Vertex>();
        }
        public void CreateLinesForNormals(Model3D myModel)
        {
            if (myModel.Normals == null || myModel.VertexList.Count != myModel.Normals.Count)
            {
                System.Windows.Forms.MessageBox.Show("Normals not calculated right ");
                return;
            }
            LinesFrom = new List<Vertex>();
            LinesTo = new List<Vertex>();

            for (int i = 0; i < myModel.VertexList.Count; i++)
            {
                LinesFrom.Add(myModel.VertexList[i]);
                LinesTo.Add(new Vertex(myModel.Normals[i]));


            }

        }
        private void ShowLines()
        {
            

            if (this.LinesFrom != null)
            {
                GL.Begin(PrimitiveType.Lines);
               
                for (int i = 0; i < LinesFrom.Count; i++)
                {
                    Vertex vStart = LinesFrom[i];
                    Vertex vEnd = LinesTo[i];
                    if (vStart.Color == null)
                    {
                        GL.Color4(1f,1f,1f,1f);
                        //DrawLine(new float[]{1f,1f,1f,1f}, vStart, vEnd);
                    }
                    else
                    {
                        GL.Color4(vStart.Color[0], vStart.Color[1], vStart.Color[2], vStart.Color[3]);
                        //DrawLine(vStart.Color, vStart, vEnd);
                    }
                    GL.Vertex3(vStart.Vector.X, vStart.Vector.Y, vStart.Vector.Z);
                    GL.Vertex3(vEnd.Vector.X, vEnd.Vector.Y, vEnd.Vector.Z);
                }
                GL.End();
                GL.LineWidth(GLSettings.PointSizeAxis);
                GL.PointSize(GLSettings.PointSize);
            }
        }

        private void DrawLine(float[] color, Vertex vStart, Vertex vEnd)
        {
            GL.Begin(PrimitiveType.Lines);

            GL.Color4(color[0], color[1], color[2], color[3]);
            GL.Vertex3(vStart.Vector.X, vStart.Vector.Y, vStart.Vector.Z);
            GL.Vertex3(vEnd.Vector.X, vEnd.Vector.Y, vEnd.Vector.Z);


            GL.End();
            GL.LineWidth(GLSettings.PointSizeAxis);
            GL.PointSize(GLSettings.PointSize);
        }
   
        private void DrawAll3DObjects()
        {
            if(GLSettings.ShowAxis)
                DrawAxis();

            if (this.Models3D != null && this.Models3D.Count > 0)
            {
                ShowLines();
                lock (this.Models3D)
                {
                    foreach (Model3D model in this.Models3D)
                    {
                        lock (model)
                        {
                            model.Render(false);
                        }
                    }
                }
              
            }
        }

        public void SelectModel(int ind)
        {
            try
            {
                this.SelectedModelIndex = ind;
                this.center = new Vector3d(this.Models3D[ind].CenterOfGravity + this.Models3D[ind].vetTransl);
                this.distEye = this.Models3D[ind].MaxPoint.Vector.X;
                if (this.distEye < this.Models3D[ind].MaxPoint.Vector.Y)
                    this.distEye = this.Models3D[ind].MaxPoint.Vector.Y;
                if (this.distEye < this.Models3D[ind].MaxPoint.Vector.Z)
                    this.distEye = this.Models3D[ind].MaxPoint.Vector.Z;
                this.distEye *= 1.5;
                this.zFar = (float)((this.distEye + 30.0) * 3.0);
                this.RepositionCamera(0.0f, 0.0f, this.MODE_ROT);
            }
            catch (Exception ex)
            {
                int num = (int)MessageBox.Show("Exception: " + ex.ToString(), "Select Model");
            }
        }

        public void MoveModel(int ind, int modo, int mouseCount)
        {
            this.paramAnt = this.param;
            if (modo <= 2)
                this.param = (float)this.distEye * (float)mouseCount / (float)this.glControl1.Width;
            if (modo > 2)
                this.param = 9.424778f * (float)mouseCount / (float)this.glControl1.Width;
            this.Models3D[ind].Translate(modo, (double)this.param - (double)this.paramAnt);
            this.Draw();
            this.glControl1.Refresh();
        }

        public void ConsolidaMoveModel()
        {
            this.param = 0.0f;
        }

     
   
    }
}
