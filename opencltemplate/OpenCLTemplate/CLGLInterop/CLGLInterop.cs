﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Windows.Forms;
using System.Drawing;
using Cloo;
using System.Runtime.InteropServices;

namespace OpenCLTemplate.CLGLInterop
{

    /// <summary>OpenGL render control</summary>
    public class GLRender
    {
        [DllImport("opengl32.dll")]
        extern static IntPtr wglGetCurrentDC();

        [DllImport("opengl32.dll")]
        extern static IntPtr wglGetCurrentContext();

        #region Initializations
        /// <summary>Parent form</summary>
        private System.Windows.Forms.Form ParentForm;

        /// <summary>OpenGL control</summary>
        public GLControl GLCtrl;

        /// <summary>Sets CL GL shared variables</summary>
        /// <param name="DeviceNumber">Index of device to use from ComputePlatform.Platforms[0].Devices. Use -1 for default</param>
        private void CreateCLGLContext(int DeviceNumber)
        {
            IntPtr curDC = wglGetCurrentDC();

            OpenTK.Graphics.IGraphicsContextInternal ctx = (OpenTK.Graphics.IGraphicsContextInternal)OpenTK.Graphics.GraphicsContext.CurrentContext;

            IntPtr raw_context_handle = ctx.Context.Handle; //wglGetCurrentContext();

            ComputeContextProperty p1 = new ComputeContextProperty(ComputeContextPropertyName.CL_GL_CONTEXT_KHR, raw_context_handle);
            ComputeContextProperty p2 = new ComputeContextProperty(ComputeContextPropertyName.CL_WGL_HDC_KHR, curDC);
            ComputeContextProperty p3 = new ComputeContextProperty(ComputeContextPropertyName.Platform, ComputePlatform.Platforms[0].Handle.Value);
            List<ComputeContextProperty> props = new List<ComputeContextProperty>() { p1, p2, p3 };

            ComputeContextPropertyList Properties = new ComputeContextPropertyList(props);

            List<ComputeDevice> GLDevices = null;
            if (DeviceNumber >= 0 && ComputePlatform.Platforms[0].Devices.Count > 1)
            {
                GLDevices = new List<ComputeDevice>() { ComputePlatform.Platforms[0].Devices[1] };
                CLGLCtx = new ComputeContext(GLDevices, Properties, null, IntPtr.Zero);
                CQ = new ComputeCommandQueue(CLGLCtx, GLDevices[0], ComputeCommandQueueFlags.None);
            }
            else
            {
                CLGLCtx = new ComputeContext(ComputeDeviceTypes.Gpu, Properties, null, IntPtr.Zero);
                CQ = new ComputeCommandQueue(CLGLCtx, CLGLCtx.Devices[0], ComputeCommandQueueFlags.None);
            }


        }


        /// <summary>Constructor. Adds a OpenGL Control to desired form</summary>
        /// <param name="ParentForm">OpenGL control parent form</param>
        /// <param name="CreateCLGLCtx">Create OpenGL/OpenCL shared context?</param>
        /// <param name="DeviceNumber">Index of device to use from ComputePlatform.Platforms[0].Devices. Use -1 for default</param>
        public GLRender(System.Windows.Forms.Form ParentForm, bool CreateCLGLCtx, int DeviceNumber)
        {
            this.ParentForm = ParentForm;

            InitGL(CreateCLGLCtx, DeviceNumber);

        }

        /// <summary>Typical OpenGL initialization</summary>
        private void InitGL(bool CreateCLGLCtx, int deviceNumber)
        {
            
            #region OpenGL Control creation with stereo capabilities

            OpenTK.Graphics.ColorFormat cf = new OpenTK.Graphics.ColorFormat();
            OpenTK.Graphics.GraphicsMode gm =
                new OpenTK.Graphics.GraphicsMode(32, 24, 8, 4, cf, 4, true);

            this.GLCtrl = new OpenTK.GLControl(gm);
            ParentForm.Controls.Add(GLCtrl);

            // 
            // sOGL
            // 
            this.GLCtrl.BackColor = System.Drawing.Color.Black;
            this.GLCtrl.Name = "sOGL";
            this.GLCtrl.VSync = false;
            this.GLCtrl.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.sOGL_MouseWheel);
            this.GLCtrl.Paint += new System.Windows.Forms.PaintEventHandler(this.sOGL_Paint);
            this.GLCtrl.MouseMove += new System.Windows.Forms.MouseEventHandler(this.sOGL_MouseMove);
            this.GLCtrl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.sOGL_MouseDown);
            this.GLCtrl.Resize += new System.EventHandler(this.sOGL_Resize);
            this.GLCtrl.MouseUp += new System.Windows.Forms.MouseEventHandler(this.sOGL_MouseUp);
            this.GLCtrl.KeyDown += new System.Windows.Forms.KeyEventHandler(this.sOGL_KeyDown);

            ParentForm.Resize += new EventHandler(sOGL_Resize);

            GLCtrl.Top = 0; GLCtrl.Left = 0;
            GLCtrl.Width = ParentForm.Width; GLCtrl.Height = ParentForm.Height;
            GLCtrl.Cursor = System.Windows.Forms.Cursors.Cross;

            #endregion

            if (CreateCLGLCtx)
            {
                CreateCLGLContext(deviceNumber);
                CLCalc.InitCL(ComputeDeviceTypes.Gpu, CLGLCtx, CQ, Cloo.ComputePlatform.Platforms.IndexOf(CLGLCtx.Platform));
            }


            GLCtrl.MakeCurrent();

            //AntiAliasing e blend
            GL.Enable(EnableCap.LineSmooth);
            GL.Hint(HintTarget.LineSmoothHint, HintMode.DontCare);
            GL.Enable(EnableCap.PolygonSmooth);
            GL.Hint(HintTarget.PolygonSmoothHint, HintMode.DontCare);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);


            //Z-Buffer
            GL.ClearDepth(1.0f);
            GL.DepthFunc(DepthFunction.Lequal);
            GL.Enable(EnableCap.DepthTest);
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.DontCare);

            //Materiais, funcoes para habilitar cor
            GL.ColorMaterial(MaterialFace.FrontAndBack, ColorMaterialParameter.AmbientAndDiffuse); //tem q vir antes do enable
            GL.Enable(EnableCap.ColorMaterial);

            // Create light components
            float[] ambientLight = { 0.5f, 0.5f, 0.5f, 1.0f };
            float[] diffuseLight = { 2.3f, 2.3f, 2.3f, 1.0f };
            float[] specularLight = { 0.3f, 0.3f, 0.3f, 1.0f };
            float[] position = { 0.0f, -40.0f, 0.0f, 1.0f };

            // Assign created components to GL_LIGHT1
            GL.Light(LightName.Light1, LightParameter.Ambient, ambientLight);
            GL.Light(LightName.Light1, LightParameter.Diffuse, diffuseLight);
            GL.Light(LightName.Light1, LightParameter.Specular, specularLight);
            GL.Light(LightName.Light1, LightParameter.Position, position);

            GL.Enable(EnableCap.Light1);// Enable Light One

            GL.ShadeModel(ShadingModel.Smooth);
            GL.Enable(EnableCap.Lighting);

            //Normals recalculation
            GL.Enable(EnableCap.Normalize);

            //Textura
            GL.Enable(EnableCap.Texture2D);

            //Line and point sizes
            GL.LineWidth(2);
            //GL.PointSize(2);

            Create3DMouseModel(new float[] { 1.0f, 0.0f, 0.0f });
        }


        #endregion

        #region Event handling / mouse rotation, translation variables


        private void sOGL_Paint(object sender, PaintEventArgs e)
        {
            //Draws once more after animation stops
            Draw();
        }


        #region Mouse rotation
        bool clicado = false;
        bool clicDireito = false;
        int originalX, origXDireito;
        int originalY, origYDireito;
        /// <summary>Mouse rotation mode</summary>
        public MouseMoveMode MouseMode = MouseMoveMode.RotateModel;
        #endregion

        #region Mouse 3D events

        /// <summary>Mouse 3D event</summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">2D events raised in this instance (picture X, Y and buttons)</param>
        /// <param name="Mouse3DPos">Mouse 3D position</param>
        /// <param name="Mouse3DRadius">Mouse 3D radius</param>
        public delegate void Mouse3DEvent(object sender, MouseEventArgs e, Vector Mouse3DPos, float Mouse3DRadius);

        /// <summary>Raised when user clicks mouse button in environment using Mouse3D mode</summary>
        public event Mouse3DEvent Mouse3DDown;
        /// <summary>Raised when user releases mouse button in environment using Mouse3D mode</summary>
        public event Mouse3DEvent Mouse3DUp;
        /// <summary>Raised when user moves 3D mouse in environment using Mouse3D mode</summary>
        public event Mouse3DEvent Mouse3DMove;

        #endregion

        private void sOGL_Resize(object sender, EventArgs e)
        {
            if (GLCtrl != null)
            {

                GLCtrl.Width = ParentForm.Width; GLCtrl.Height = ParentForm.Height;
                if (GLCtrl.Width < 0) GLCtrl.Width = 1;
                if (GLCtrl.Height < 0) GLCtrl.Height = 1;

                GLCtrl.MakeCurrent();
                GL.Viewport(0, 0, GLCtrl.Width, GLCtrl.Height);
                GLCtrl.Invalidate();
            }
        }

        private void sOGL_MouseDown(object sender, MouseEventArgs e)
        {
            //if (MouseMode == MouseMoveMode.TranslateModel || MouseMode == MouseMoveMode.RotateModel)
            //{
            if (e.Button == MouseButtons.Left)
            {
                if (!clicado)
                {
                    clicado = true;
                    originalX = e.X;
                    originalY = e.Y;
                }
            }

            if (e.Button == MouseButtons.Right)
            {
                if (!clicDireito)
                {
                    clicDireito = true;
                    origXDireito = e.X;
                    origYDireito = e.Y;
                }
            }

            if (Mouse3D != null)
            {
                if (MousePosAnt == null) MousePosAnt = new Vector();
                MousePosAnt.x = Mouse3D.vetTransl.x;
                MousePosAnt.y = Mouse3D.vetTransl.y;
                MousePosAnt.z = Mouse3D.vetTransl.z;
                if (Mouse3DDown != null && this.MouseMode == MouseMoveMode.Mouse3D) Mouse3DDown(sender, e, new Vector(Mouse3D.vetTransl), Mouse3D.Scale[0]);
            }

            //}
            //else if (MouseMode == MouseMoveMode.Mouse3D)
            //{
            //}
        }

        private void sOGL_MouseUp(object sender, MouseEventArgs e)
        {
            if (MouseMode == MouseMoveMode.TranslateModel || MouseMode == MouseMoveMode.RotateModel)
            {
                if (e.Button == MouseButtons.Left)
                {
                    clicado = false;
                    this.ConsolidateRepositioning();
                }
            }
            else if (MouseMode == MouseMoveMode.Mouse3D)
            {
                if (Mouse3DUp != null) Mouse3DUp(sender, e, new Vector(Mouse3D.vetTransl), Mouse3D.Scale[0]);
                else Process3DMouseHit(e);
            }

            if (e.Button == MouseButtons.Left)
            {
                clicado = false;
            }
            if (e.Button == MouseButtons.Right)
            {
                clicDireito = false;
            }
        }
        private Point mousePos = new Point(0, 0);

        //private int MouseMoveRedrawCount = 0;
        private void sOGL_MouseMove(object sender, MouseEventArgs e)
        {
            //MouseMoveRedrawCount++;
            //if (MouseMoveRedrawCount == 5) MouseMoveRedrawCount = 0;
            if (MouseMode == MouseMoveMode.TranslateModel || MouseMode == MouseMoveMode.RotateModel)
            {
                mousePos.X = e.X; mousePos.Y = e.Y;
                if (e.Button == MouseButtons.Left)
                {
                    if (clicado)
                    {
                        this.RepositionCamera((float)e.X - (float)originalX, (float)e.Y - (float)originalY, MouseMode);
                        //if (MouseMoveRedrawCount==4) 
                            GLCtrl.Refresh();
                    }
                }
                Mouse3D.ShowModel = false;
            }
            else if (MouseMode == MouseMoveMode.Mouse3D)
            {

                if (MousePosAnt == null) MousePosAnt = new Vector();
                MousePosAnt.x = Mouse3D.vetTransl.x;
                MousePosAnt.y = Mouse3D.vetTransl.y;
                MousePosAnt.z = Mouse3D.vetTransl.z;

                float x = (float)e.X / (float)GLCtrl.Width;
                float y = (float)e.Y / (float)GLCtrl.Height;
                Translate3DMouseXY(x, y, 0);
                //if (MouseMoveRedrawCount == 4) 
                    GLCtrl.Refresh();
                Mouse3D.ShowModel = true;

                if (Mouse3DMove != null) Mouse3DMove(sender, e, new Vector(Mouse3D.vetTransl), Mouse3D.Scale[0]);
                else
                {
                    Process3DMouseHit(e);
                }
            }
        }

        //sOGL_MouseWheel
        private void sOGL_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (MouseMode == MouseMoveMode.TranslateModel || MouseMode == MouseMoveMode.RotateModel)
            {
                this.distEye *= 1 - ((float)e.Delta * 0.001);
                RecalcZnearZFar();
            }
            else if (MouseMode == MouseMoveMode.Mouse3D)
            {
                float x = (float)e.X / (float)GLCtrl.Width;
                float y = (float)e.Y / (float)GLCtrl.Height;
                this.Translate3DMouseXY(x, y, -e.Delta);
                GLCtrl.Refresh();

                if (Mouse3DMove != null) Mouse3DMove(sender, e, new Vector(Mouse3D.vetTransl), Mouse3D.Scale[0]);
                else Process3DMouseHit(e);
            }
        }

        /// <summary>Automatically recalculates zNear and zFar values</summary>
        private void RecalcZnearZFar()
        {
            //this.zFar = ((float)this.distEye+40) * 4;
            this.zFar = ((float)this.distEye) * 5;
            this.zNear = this.zFar * 1e-3f;
            this.RepositionCamera(0, 0, MouseMode);
            //status("View distance set to " + this.distEye.ToString(), -1);
            GLCtrl.Refresh();
        }

        private void sOGL_KeyDown(object sender, KeyEventArgs e)
        {

            //movimento com teclado
            bool invalidar = false;

            #region 3D mouseing commands
            if (e.KeyCode == Keys.Q && Mouse3D != null)
            {
                //Go into 3D mousing mode
                MouseMode = MouseMoveMode.Mouse3D;
                Reset3DMousePos();
            }
            else if (e.KeyCode == Keys.R)
            {
                MouseMode = MouseMoveMode.RotateModel;
                Mouse3D.ShowModel = false;
                invalidar = true;
            }
            else if (e.KeyCode == Keys.Subtract)
            {
                Mouse3D.Scale[0] *= 0.97f;
                Mouse3D.Scale[1] *= 0.97f;
                Mouse3D.Scale[2] *= 0.97f;
                invalidar = true;
            }
            else if (e.KeyCode == Keys.Add)
            {
                Mouse3D.Scale[0] *= 1.02f;
                Mouse3D.Scale[1] *= 1.02f;
                Mouse3D.Scale[2] *= 1.02f;
                invalidar = true;
            }


            #endregion

            if (e.KeyCode == Keys.W)
            {
                this.center -= this.zFar * 0.002 * this.front;
                this.eye -= this.zFar * 0.002 * this.front;
                invalidar = true;
            }
            if (e.KeyCode == Keys.S)
            {
                this.center += this.zFar * 0.002 * this.front;
                this.eye += this.zFar * 0.002 * this.front;
                invalidar = true;
            }
            if (e.KeyCode == Keys.A)
            {
                this.center -= this.zFar * 0.001 * this.esq;
                this.eye -= this.zFar * 0.001 * this.esq;
                invalidar = true;
            }
            if (e.KeyCode == Keys.D)
            {
                this.center += this.zFar * 0.001 * this.esq;
                this.eye += this.zFar * 0.001 * this.esq;
                invalidar = true;
            }

            //Rotacao com teclado
            double cos = Math.Cos(0.01);
            double sin = Math.Sin(0.01);
            if (e.KeyCode == Keys.NumPad4)
            {
                Vector front = new Vector(this.front);
                Vector esq = new Vector(this.esq);

                this.front = cos * front + sin * esq;
                this.esq = -sin * front + cos * esq;

                this.center = this.eye - this.front * this.distEye;

                invalidar = true;
            }
            if (e.KeyCode == Keys.NumPad6)
            {
                Vector front = new Vector(this.front);
                Vector esq = new Vector(this.esq);

                this.front = cos * front - sin * esq;
                this.esq = sin * front + cos * esq;

                this.center = this.eye - this.front * this.distEye;

                invalidar = true;
            }

            if (e.KeyCode == Keys.NumPad2)
            {
                Vector front = new Vector(this.front);
                Vector up = new Vector(this.up);

                this.front = cos * front + sin * up;
                this.up = -sin * front + cos * up;

                this.center = this.eye - this.front * this.distEye;

                invalidar = true;
            }
            if (e.KeyCode == Keys.NumPad8)
            {
                Vector front = new Vector(this.front);
                Vector up = new Vector(this.up);

                this.front = cos * front - sin * up;
                this.up = sin * front + cos * up;

                this.center = this.eye - this.front * this.distEye;

                invalidar = true;
            }


            if (invalidar)
            {
                GLCtrl.Invalidate();
            }
        }

        #endregion

        #region Variables and methods for mouse movement control

        /// <summary>Mouse action when used to move the 3D Model</summary>
        public enum MouseMoveMode
        {
            /// <summary>Mouse rotation mode index.</summary>
            RotateModel,
            /// <summary>Mouse translation mode index.</summary>
            TranslateModel,
            /// <summary>Enter 3D mousing mode</summary>
            Mouse3D,
            /// <summary>No mouse movement.</summary>
            None
        }

        /// <summary>Point where camera is looking at</summary>
        private Vector center = new Vector(0, 0, 0);
        /// <summary>Point where camera is standing</summary>
        private Vector eye = new Vector(0, 0, 215);
        /// <summary>Front vector</summary>
        private Vector front = new Vector(0, 0, 1);
        /// <summary>Up vector</summary>
        private Vector up = new Vector(0, 1, 0);
        /// <summary>Left vector</summary>
        private Vector esq = new Vector(1, 0, 0);
        /// <summary>Camera eye distance.</summary>
        private double distEye = 215;
        /// <summary>Far distance to clip at.</summary>
        public float zFar = 1000.0f;
        /// <summary>Near distance to clip at</summary>
        public float zNear = 1.0f;

        Vector frontCpy = new Vector(0, 0, 1);
        Vector upCpy = new Vector(0, 1, 0);
        Vector esqCpy = new Vector(1, 0, 0);
        Vector centerCpy = new Vector(0, 0, 0);

        //vetor angs = new vetor(0,0,0);
        //vetor angsCpy=new vetor(0,0,0);

        /// <summary>Repositions camera.</summary>
        /// <param name="mouseDX">X mouse displacement.</param>
        /// <param name="mouseDY">Y mouse displacement.</param>
        /// <param name="modo">Mouse displacement mode (user wants translation or rotation?)</param>
        private void RepositionCamera(float mouseDX, float mouseDY, MouseMoveMode modo)
        {
            if (modo == MouseMoveMode.RotateModel)
            {
                //Faz com que pegar o mouse em uma ponta e levar ate a outra
                //gire a cena 360 graus
                double ang2 = -3 * Math.PI * mouseDX / (float)GLCtrl.Width;
                double ang1 = -3 * Math.PI * mouseDY / (float)GLCtrl.Height;

                Console.Write(ang2.ToString());

                //Calcula a rotacao do mouse
                double c1, s1, c2, s2;
                c1 = Math.Cos(ang1);
                s1 = Math.Sin(ang1);
                c2 = Math.Cos(ang2);
                s2 = Math.Sin(ang2);

                front = frontCpy * c1 + upCpy * -s1;
                up = s1 * frontCpy + upCpy * c1;

                Vector temp = new Vector(front);

                front = temp * c2 + s2 * esqCpy;
                esq = -s2 * temp + esqCpy * c2;
            }
            else if (modo == MouseMoveMode.TranslateModel)
            {
                double dx = -distEye * mouseDX / (float)GLCtrl.Width;
                double dy = distEye * mouseDY / (float)GLCtrl.Height;

                center = centerCpy + esqCpy * dx + upCpy * dy;

            }
            //Olho: centro, deslocado na direcao em FRENTE de distEye
            eye = center + front * distEye;
            RepositionLight();
        }

        /// <summary>Function to advance view and allow "fly" simulations.</summary>
        /// <param name="Distance">Distance to fly</param>
        public void Fly(Vector Distance)
        {
            center += Distance.x * front + Distance.y * esq + Distance.z * up;
            eye += Distance.x * front + Distance.y * esq + Distance.z * up;
        }

        /// <summary>Updates temporary displacement vectors to internal drawing vectors.</summary>
        private void ConsolidateRepositioning()
        {
            frontCpy = new Vector(front);
            upCpy = new Vector(up);
            esqCpy = new Vector(esq);
            centerCpy = new Vector(center);
        }

        /// <summary>Repositions light.</summary>
        private void RepositionLight()
        {
            GL.LoadIdentity(); //para evitar pegar matriz de rotacao residual
            float[] position = { 0.0f, -(float)distEye, 0.0f, 1.0f }; //reposiciona a luz dinamicamente
            GL.Light(LightName.Light1, LightParameter.Position, position);
        }


        #endregion

        #region 3D mousing

        /// <summary>Mouse 3D model</summary>
        public GLVBOModel Mouse3D;

        /// <summary>Show mouse to center distance in this label if not null</summary>
        public ToolStripStatusLabel lblMouseToCenterDist;

        /// <summary>Creates a 3D Model for the mouse</summary>
        /// <param name="Color">Desired color</param>
        public void Create3DMouseModel(float[] Color)
        {
            if (Mouse3D == null)
            {
                Mouse3D = new GLVBOModel(BeginMode.Triangles);
            }

            #region Creates sphere
            int N = 20;

            float[] Vertex = new float[3 * N * N];
            float[] Normals = new float[3 * N * N];
            float[] Colors = new float[4 * N * N];

            for (int u = 0; u < N; u++)
            {
                float uu = 2.0f * (float)Math.PI * (float)u / (float)(N - 1);
                for (int v = 0; v < N; v++)
                {
                    float vv = (float)Math.PI * ((float)v / (float)(N - 1) - 0.5f);
                    Vertex[3 * (u + N * v)] = (float)(Math.Cos(uu) * Math.Cos(vv));
                    Vertex[1 + 3 * (u + N * v)] = (float)(Math.Sin(uu) * Math.Cos(vv));
                    Vertex[2 + 3 * (u + N * v)] = (float)(Math.Sin(vv));
                    Normals[3 * (u + N * v)] = Vertex[3 * (u + N * v)];
                    Normals[1 + 3 * (u + N * v)] = Vertex[1 + 3 * (u + N * v)];
                    Normals[2 + 3 * (u + N * v)] = Vertex[2 + 3 * (u + N * v)];

                    Colors[4 * (u + N * v)] = Color[0]; Colors[1 + 4 * (u + N * v)] = Color[1]; Colors[2 + 4 * (u + N * v)] = Color[2] + 0.5f * (1.0f + Vertex[2 + 3 * (u + N * v)]); Colors[3 + 4 * (u + N * v)] = 0.3f;
                }
            }

            int[] Elems = new int[6 * (N - 1) * (N - 1)];
            for (int u = 0; u < N - 1; u++)
            {
                for (int v = 0; v < N - 1; v++)
                {
                    Elems[6 * (u + (N - 1) * v)] = u + N * v;
                    Elems[1 + 6 * (u + (N - 1) * v)] = 1 + u + N * v;
                    Elems[2 + 6 * (u + (N - 1) * v)] = 1 + u + N * (1 + v);

                    Elems[3 + 6 * (u + (N - 1) * v)] = u + N * v;
                    Elems[4 + 6 * (u + (N - 1) * v)] = 1 + u + N * (1 + v);
                    Elems[5 + 6 * (u + (N - 1) * v)] = u + N * (1 + v);
                }
            }


            #endregion

            Mouse3D.Name = "3D Mouse"; BufferUsageHint h = BufferUsageHint.StaticDraw;
            Mouse3D.ShowModel = false;
            Mouse3D.SetVertexData(Vertex, h);
            Mouse3D.SetNormalData(Normals, h);
            Mouse3D.SetColorData(Colors, h);
            Mouse3D.SetElemData(Elems, h);

            Reset3DMousePos();
            ReDraw();
        }

        /// <summary>Resets 3D mouse position to center of view</summary>
        public void Reset3DMousePos()
        {
            Mouse3D.vetTransl.x = center.x; Mouse3D.vetTransl.y = center.y; Mouse3D.vetTransl.z = center.z;
            Mouse3D.Scale[0] = (float)distEye * 0.1f;
            Mouse3D.Scale[1] = (float)distEye * 0.1f;
            Mouse3D.Scale[2] = (float)distEye * 0.1f;
            ReDraw();
        }

        /// <summary>Translates 3D mouse to a given Left - Top</summary>
        /// <param name="x">Left relative value, 0 to 1</param>
        /// <param name="y">Up relative value, 0 to 1</param>
        /// <param name="dz">Z (depth) relative value</param>
        public void Translate3DMouseXY(float x, float y, float dz)
        {
            double mouseZ = Vector.DotProduct(front, Mouse3D.vetTransl - center);
            float scaleFac = ((float)mouseZ + (float)distEye) * 0.00012f;
            Mouse3D.vetTransl += scaleFac * front * dz;

            mouseZ = Vector.DotProduct(front, Mouse3D.vetTransl - center);
            scaleFac = ((float)distEye - (float)mouseZ) * 1.4f;
            Mouse3D.vetTransl = center + scaleFac * (esq * (x - 0.5f) - up * (y - 0.5f)) + mouseZ * front;

            if (lblMouseToCenterDist != null)
            {
                double dist = (Mouse3D.vetTransl - center).norm();
                lblMouseToCenterDist.Text = Math.Round(dist, 3).ToString();
            }
        }

        /// <summary>Increments current mouse position</summary>
        /// <param name="dx">Left relative value</param>
        /// <param name="dy">Up relative value</param>
        /// <param name="dz">Z (depth) relative value</param>
        public void Increment3DMousePos(float dx, float dy, float dz)
        {
            double mouseZ = Vector.DotProduct(front, Mouse3D.vetTransl);
            float scaleFac = ((float)mouseZ + (float)distEye) * 0.0006f;

            Mouse3D.vetTransl += scaleFac * (esq * dx + up * dy + front * dz);
        }

        #region OpenCL displacement/hide objects

        /// <summary>Processes 3D mouse event</summary>
        private void Process3DMouseHit(MouseEventArgs e)
        {
            if (clicDireito) HideElements();
            if (clicado) DisplaceElements();
        }

        /// <summary>OpenGL/CL shared context</summary>
        private ComputeContext CLGLCtx;
        /// <summary>OpenGL/CL shared command queue</summary>
        private ComputeCommandQueue CQ;

        /// <summary>Mouse position in GPU memory</summary>
        private ComputeBuffer<float> CLMousePos;

        /// <summary>Previous mouse position (when clicked)</summary>
        private Vector MousePosAnt;

        /// <summary>Previous mouse position in GPU memory</summary>
        private ComputeBuffer<float> CLMousePosAnt;

        /// <summary>Mouse radius in GPU memory</summary>
        private ComputeBuffer<float> CLMouseRadius;

        /// <summary>Hide/show kernel</summary>
        private ComputeKernel kernelHide, kernelShowAll, kernelHideLines;

        /// <summary>Displacement kernel</summary>
        private ComputeKernel kernelDisplace;

        /// <summary>Initializes OpenCL kernels to calculate displacement and hide objects</summary>
        private void InitCLDisp()
        {
            //Kernels
            OpenCLDispSrc src = new OpenCLDispSrc();
            ComputeProgram prog = new ComputeProgram(this.CLGLCtx, src.src);
            prog.Build(CLGLCtx.Devices, "", null, IntPtr.Zero);

            kernelHide = prog.CreateKernel("HideElems");
            kernelShowAll = prog.CreateKernel("ShowAllElems");
            kernelDisplace = prog.CreateKernel("DisplaceElems");
            kernelHideLines = prog.CreateKernel("HideLineElems");

            //Mouse arguments
            CLMousePos = new ComputeBuffer<float>(CLGLCtx, ComputeMemoryFlags.ReadWrite, 3);
            CLMousePosAnt = new ComputeBuffer<float>(CLGLCtx, ComputeMemoryFlags.ReadWrite, 3);
            CLMouseRadius = new ComputeBuffer<float>(CLGLCtx, ComputeMemoryFlags.ReadWrite, 1);


            kernelHide.SetMemoryArgument(2, CLMousePos);
            kernelHide.SetMemoryArgument(3, CLMouseRadius);

            kernelHideLines.SetMemoryArgument(2, CLMousePos);
            kernelHideLines.SetMemoryArgument(3, CLMouseRadius);

            kernelDisplace.SetMemoryArgument(1, CLMousePosAnt);
            kernelDisplace.SetMemoryArgument(2, CLMousePos);
            kernelDisplace.SetMemoryArgument(3, CLMouseRadius);
        }

        /// <summary>Writes information to a buffer</summary>
        /// <param name="buffer">Buffer object</param>
        /// <param name="Values">Values to write</param>
        private void CQWrite(ComputeBuffer<float> buffer, float[] Values)
        {
            unsafe
            {
                fixed (void* ponteiro = Values)
                {
                    CQ.Write<float>(buffer, true, 0, Values.Length, (IntPtr)ponteiro, null);
                }
            }
        }

        /// <summary>Undoes all hide operations and shows all elements</summary>
        public void ShowAllElements()
        {
            if (kernelHide == null) InitCLDisp();
            GL.Finish();

            CQWrite(CLMousePos, new float[] { (float)Mouse3D.vetTransl.x, (float)Mouse3D.vetTransl.y, (float)Mouse3D.vetTransl.z });

            CQWrite(CLMouseRadius, new float[] { (float)Mouse3D.Scale[0] });

            foreach (GLVBOModel model in this.Models)
            {
                if (model != Mouse3D)
                {
                    lock (model)
                    {
                        //Create from GL buffers
                        ComputeBuffer<int> CLGLElems = ComputeBuffer<int>.CreateFromGLBuffer<int>(CLGLCtx, ComputeMemoryFlags.ReadWrite, model.GLElemBuffer);

                        //Acquire
                        List<ComputeMemory> c = new List<ComputeMemory>() { CLGLElems };
                        CQ.AcquireGLObjects(c, null);


                        //Use
                        kernelShowAll.SetMemoryArgument(0, CLGLElems);
                        CQ.Execute(kernelShowAll, null, new long[] { model.ElemLength }, null, null);

                        //Release and dispose
                        CQ.ReleaseGLObjects(c, null);

                        CLGLElems.Dispose();
                    }
                }
            }
            CQ.Finish();

            ReDraw();
        }

        /// <summary>Hides elements in this GLRender which are close to the 3D mouse</summary>
        private void HideElements()
        {
            if (kernelHide == null) InitCLDisp();
            GL.Finish();

            CQWrite(CLMousePos, new float[] { (float)Mouse3D.vetTransl.x, (float)Mouse3D.vetTransl.y, (float)Mouse3D.vetTransl.z });
            CQWrite(CLMouseRadius, new float[] { (float)Mouse3D.Scale[0] });

            foreach (GLVBOModel model in this.Models)
            {
                if (model != Mouse3D && model.ShowModel)
                {
                    lock (model)
                    {
                        //Create from GL buffers
                        ComputeBuffer<int> CLGLElems = ComputeBuffer<int>.CreateFromGLBuffer<int>(CLGLCtx, ComputeMemoryFlags.ReadWrite, model.GLElemBuffer);
                        ComputeBuffer<float> CLGLVertexes = ComputeBuffer<float>.CreateFromGLBuffer<float>(CLGLCtx, ComputeMemoryFlags.ReadWrite, model.GLVertexBuffer);

                        //Acquire
                        List<ComputeMemory> c = new List<ComputeMemory>() { CLGLElems, CLGLVertexes };
                        CQ.AcquireGLObjects(c, null);

                        if (model.DrawMode == BeginMode.Triangles)
                        {
                            //Use
                            kernelHide.SetMemoryArgument(0, CLGLElems);
                            kernelHide.SetMemoryArgument(1, CLGLVertexes);
                            CQ.Execute(kernelHide, null, new long[] { model.ElemLength / 3 }, null, null);
                        }
                        else if (model.DrawMode == BeginMode.Lines)
                        {
                            //Use
                            kernelHideLines.SetMemoryArgument(0, CLGLElems);
                            kernelHideLines.SetMemoryArgument(1, CLGLVertexes);
                            CQ.Execute(kernelHideLines, null, new long[] { model.ElemLength / 2 }, null, null);
                        }

                        //Release and dispose
                        CQ.ReleaseGLObjects(c, null);


                        CLGLElems.Dispose();
                        CLGLVertexes.Dispose();
                    }
                }
            }
            CQ.Finish();

            ReDraw();
        }

        /// <summary>Displace elements according to mouse command</summary>
        private void DisplaceElements()
        {
            if (kernelHide == null) InitCLDisp();
            GL.Finish();

            CQWrite(CLMousePos, new float[] { (float)Mouse3D.vetTransl.x, (float)Mouse3D.vetTransl.y, (float)Mouse3D.vetTransl.z });
            CQWrite(CLMousePosAnt, new float[] { (float)MousePosAnt.x, (float)MousePosAnt.y, (float)MousePosAnt.z });
            CQWrite(CLMouseRadius, new float[] { (float)Mouse3D.Scale[0] });

            foreach (GLVBOModel model in this.Models)
            {
                if (model != Mouse3D)
                {
                    lock (model)
                    {
                        if (model.ShowModel)
                        {
                            //Create from GL buffers
                            ComputeBuffer<float> CLGLVertexes = ComputeBuffer<float>.CreateFromGLBuffer<float>(CLGLCtx, ComputeMemoryFlags.ReadWrite, model.GLVertexBuffer);

                            //Acquire
                            List<ComputeMemory> c = new List<ComputeMemory>() { CLGLVertexes };
                            CQ.AcquireGLObjects(c, null);


                            //Use
                            kernelDisplace.SetMemoryArgument(0, CLGLVertexes);
                            CQ.Execute(kernelDisplace, null, new long[] { model.numVertexes }, null, null);

                            //Release and dispose
                            CQ.ReleaseGLObjects(c, null);

                            CLGLVertexes.Dispose();
                        }
                    }
                }
            }
            CQ.Finish();

            ReDraw();
        }

        private class OpenCLDispSrc
        {
            public string src = @"
//global_size(0) = number of elements = elems.Length/3

__kernel void HideElems(__global int   * elems,
                        __global float * VertexCoords,
                        __global float * mouseCoords,
                        __global float * mouseRadius)
                        
 {
    int i3 = 3*get_global_id(0);
    if (elems[i3]>=0)
    {
        //Mouse coords
        float4 mousePos = (float4)(mouseCoords[0], mouseCoords[1], mouseCoords[2],0);
        float r = mouseRadius[0];
        
        //Triangle vertexes
        float4 v1 = (float4)(VertexCoords[3*elems[i3  ]], VertexCoords[3*elems[i3  ]+1], VertexCoords[3*elems[i3  ]+2], 0);
        float4 v2 = (float4)(VertexCoords[3*elems[i3+1]], VertexCoords[3*elems[i3+1]+1], VertexCoords[3*elems[i3+1]+2], 0);
        float4 v3 = (float4)(VertexCoords[3*elems[i3+2]], VertexCoords[3*elems[i3+2]+1], VertexCoords[3*elems[i3+2]+2], 0);
        
        float dist1, dist2, dist3;

        dist1 = fast_distance(mousePos, v1);
        
        if (dist1 <= 3*r)
        {
            dist2 = fast_distance(mousePos, v2);
            dist3 = fast_distance(mousePos, v3);

            if (dist1 <= r || dist2 <= r || dist3 <= r)
            {
                 elems[i3]=-elems[i3]-1;
                 elems[i3+1]=-elems[i3+1]-1;
                 elems[i3+2]=-elems[i3+2]-1;
            }
        }
    }
 }
 
__kernel void HideLineElems(__global int   * elems,
                            __global float * VertexCoords,
                            __global float * mouseCoords,
                            __global float * mouseRadius)
                        
 {
    int i2 = 2*get_global_id(0);
    if (elems[i2]>=0)
    {
        //Mouse coords
        float4 mousePos = (float4)(mouseCoords[0], mouseCoords[1], mouseCoords[2],0);
        float r = mouseRadius[0];
        
        //Triangle vertexes
        float4 v1 = (float4)(VertexCoords[3*elems[i2  ]], VertexCoords[3*elems[i2  ]+1], VertexCoords[3*elems[i2  ]+2], 0);
        float4 v2 = (float4)(VertexCoords[3*elems[i2+1]], VertexCoords[3*elems[i2+1]+1], VertexCoords[3*elems[i2+1]+2], 0);
        
        float dist1, dist2;

        dist1 = fast_distance(mousePos, v1);
        
        if (dist1 <= 3*r)
        {
            dist2 = fast_distance(mousePos, v2);

            if (dist1 <= r || dist2 <= r)
            {
                 elems[i2]=-elems[i2]-1;
                 elems[i2+1]=-elems[i2+1]-1;
            }
        }
    }
 }

 __kernel void ShowAllElems(__global int * elems)
 {
    
    int i = get_global_id(0);
    if (elems[i]<0)
    {
         elems[i]=-elems[i]-1;
    }
 }

//global_size(0) = number of vertexes = vertexes.Length/3

__kernel void DisplaceElems(__global float * VertexCoords,
                            __global float * mouseCoords0,
                            __global float * mouseCoordsf,
                            __global float * mouseRadius)
                        
 {
    int i3 = 3*get_global_id(0);
    
    //Mouse coords
    float4 mousePos0 = (float4)(mouseCoords0[0], mouseCoords0[1], mouseCoords0[2],0);
    float4 mousePosf = (float4)(mouseCoordsf[0], mouseCoordsf[1], mouseCoordsf[2],0);
    float invr = native_recip(mouseRadius[0]);
    
    //Vertex coordinates
    float4 v1 = (float4)(VertexCoords[i3], VertexCoords[i3+1], VertexCoords[i3+2], 0);
    
    float dist1 = fast_distance(mousePos0, v1);
    
    //Displacement vector
    mousePosf -= mousePos0;
    
    //Displacement intensity

    float temp = 0.707f*dist1*invr;
//    dist1 = 0.7978845608f*invr*native_exp(-temp*temp);
    dist1 = 7.978845608f*invr*native_exp(-temp*temp);
    
    //Final vertex coord
    v1 += mousePosf*dist1;
    
    VertexCoords[i3] = v1.x;    VertexCoords[i3+1] = v1.y;    VertexCoords[i3+2] = v1.z;
 }

";
        }

        #endregion

        #endregion

        #region OpenGL scene drawing
        /// <summary>List of OpenGL VBOs to draw</summary>
        public List<GLVBOModel> Models = new List<GLVBOModel>();

        /// <summary>Draw in stereographic projection?</summary>
        public bool StereoscopicDraw = false;

        /// <summary>Stereographic distance.</summary>
        public float StereoDistance = -0.005f;

        /// <summary>Background color</summary>
        public float[] ClearColor = new float[3] { 0.0f, 0.0f, 0.0f };

        /// <summary>Forces the control to redraw its contents</summary>
        public void ReDraw()
        {
            GLCtrl.Invalidate();
        }

        /// <summary>Void function delegate</summary>
        public delegate void VoidFunc();
        /// <summary>Function to be invoked prior to every drawing</summary>
        public VoidFunc PreDrawFunc;

        /// <summary>Controls the camera positioning for the OpenGL scene</summary>
        private void Draw()
        {
            //Prevents strange zFars from happening
            if (zFar < 0) zFar = 300;
            if (zFar > 1e35f) zFar = 1e35f;

            //Invoke pre draw function
            if (PreDrawFunc != null) PreDrawFunc();

            if (StereoscopicDraw)
            {
                #region Left eye
                Vector eyeStereo = eye - distEye * StereoDistance * esq;

                GL.DrawBuffer(DrawBufferMode.BackRight);
                GL.ClearColor(ClearColor[0], ClearColor[1], ClearColor[2], 0.0f);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                GL.LoadIdentity();

                OpenTK.Matrix4d m1 = OpenTK.Matrix4d.CreatePerspectiveFieldOfView(Math.PI * 0.25f,
                        (double)GLCtrl.Width / (double)GLCtrl.Height, (double)zNear, (double)zFar);
                GL.LoadMatrix(ref m1);
                OpenTK.Matrix4d m2 = OpenTK.Matrix4d.LookAt(eyeStereo.x, eyeStereo.y, eyeStereo.z, center.x, center.y, center.z,
                    up.x, up.y, up.z);
                GL.MultMatrix(ref m2);

                DoDraw();
                #endregion
                #region Right eye
                eyeStereo = eye + distEye * StereoDistance * esq;
                GL.DrawBuffer(DrawBufferMode.BackLeft);
                GL.ClearColor(ClearColor[0], ClearColor[1], ClearColor[2], 0.0f);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                GL.LoadIdentity();


                OpenTK.Matrix4d m10 = OpenTK.Matrix4d.CreatePerspectiveFieldOfView(Math.PI * 0.25f,
                        (double)GLCtrl.Width / (double)GLCtrl.Height, (double)zNear, (double)zFar);
                GL.LoadMatrix(ref m10);
                OpenTK.Matrix4d m20 = OpenTK.Matrix4d.LookAt(eyeStereo.x, eyeStereo.y, eyeStereo.z, center.x, center.y, center.z,
                    up.x, up.y, up.z);
                GL.MultMatrix(ref m20);


                DoDraw();
                #endregion

                GLCtrl.SwapBuffers();
            }
            else
            {
                GL.DrawBuffer(DrawBufferMode.Back);

                GL.ClearColor(ClearColor[0], ClearColor[1], ClearColor[2], 0.0f);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                GL.LoadIdentity();

                OpenTK.Matrix4d m1 = OpenTK.Matrix4d.CreatePerspectiveFieldOfView(Math.PI * 0.25f,
                    (double)GLCtrl.Width / (double)GLCtrl.Height, (double)zNear, (double)zFar);
                GL.LoadMatrix(ref m1);
                OpenTK.Matrix4d m2 = OpenTK.Matrix4d.LookAt(eye.x, eye.y, eye.z, center.x, center.y, center.z,
                    up.x, up.y, up.z);
                GL.MultMatrix(ref m2);

                DoDraw();


                //Material
                float[] specular = { 0.9f, 0.9f, 0.9f, 1 };

                GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Shininess, specular);

                GLCtrl.SwapBuffers();
            }
        }

        /// <summary>Draw axes at center?</summary>
        public bool DrawAxes = true;

        /// <summary>Draws OpenGL scene</summary>
        private void DoDraw()
        {
            GL.Begin(BeginMode.Lines);
            
            //Unbinds textures
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.Color4(1.0f, 1.0f, 1.0f, 1.0f);

            Vector v = new Vector(center);
            if (DrawAxes)
            {
                GL.Vertex3((float)v.x, (float)v.y, (float)v.z);
                GL.Vertex3((float)v.x + 60f, (float)v.y, (float)v.z);
                GL.Vertex3((float)v.x, (float)v.y, (float)v.z);
                GL.Vertex3((float)v.x, (float)v.y + 60f, (float)v.z);
                GL.Vertex3((float)v.x, (float)v.y, (float)v.z);
                GL.Vertex3((float)v.x, (float)v.y, (float)v.z + 60f);
            }
            if (lblMouseToCenterDist != null && MouseMode == MouseMoveMode.Mouse3D)
            {
                GL.Vertex3((float)center.x, (float)center.y, (float)center.z);

                GL.Vertex3((float)Mouse3D.vetTransl.x, (float)Mouse3D.vetTransl.y, (float)Mouse3D.vetTransl.z);
            }
            GL.End();

            lock (Models)
            {
                foreach (GLVBOModel model in Models)
                {
                    lock (model)
                    {
                        model.DrawModel();
                    }
                }
            }


            if (Mouse3D != null) Mouse3D.DrawModel();
        }

        #endregion

        #region Camera repositioning

        /// <summary>Sets center of camera and recalculates appropriate vectors</summary>
        /// <param name="NewCenter">Desired center</param>
        public void SetCenter(Vector NewCenter)
        {
            try
            {
                //Centraliza no modelo
                center = new Vector(NewCenter);

                this.RepositionCamera(0.0f, 0.0f, MouseMoveMode.RotateModel);

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Exception: " + ex.ToString(), "Select Model");
            }
        }

        /// <summary>Sets the camera distance from the center of where it's looking</summary>
        /// <param name="Distance">New distance</param>
        public void SetDistance(double Distance)
        {
            distEye = Distance;
            RecalcZnearZFar();
        }

        /// <summary>Gets camera center (the center of where the camera is looking at)</summary>
        public Vector GetCenter()
        {
            return new Vector(center);
        }
        /// <summary>Gets camera distance from the center</summary>
        public double GetDistance()
        {
            return distEye;
        }


        #endregion


        /// <summary>OpenGL Model created from vertex buffer objects</summary>
        public class GLVBOModel
        {
            /// <summary>Model name</summary>
            public string Name;
            /// <summary>Use draw elements? Just for compatibility with Lab3D models</summary>
            private bool UseDrawElements = true;

            #region Constructor/destructor
            /// <summary>Constructor. Receives draw mode of the model. REMINDER: Vertex, color and element data are necessary for drawing.</summary>
            /// <param name="DrawMode">OpenGL Draw model</param>
            public GLVBOModel(BeginMode DrawMode)
            {
                this.DrawMode = DrawMode;
            }

            /// <summary>Constructor. Reuses the same Vertex Buffer Elements of an existing 3D model.</summary>
            /// <param name="Source">Source model to reuse buffer elements</param>
            public GLVBOModel(GLVBOModel Source)
            {
                this.DrawMode = Source.DrawMode;
                this.CLColorBuffer = Source.CLColorBuffer;
                this.CLElemBuffer = Source.CLElemBuffer;
                this.CLNormalBuffer = Source.CLNormalBuffer;
                this.CLTexCoordBuffer = Source.CLTexCoordBuffer;
                this.CLVertexBuffer = Source.CLVertexBuffer;

                this.GLColorBuffer = Source.GLColorBuffer;
                this.GLElemBuffer = Source.GLElemBuffer;
                this.GLNormalBuffer = Source.GLNormalBuffer;
                this.GLTexCoordBuffer = Source.GLTexCoordBuffer;
                this.GLVertexBuffer = Source.GLVertexBuffer;
                this.ElemLength = Source.ElemLength;
            }

            /// <summary>Creates 3D model from a given file</summary>
            /// <param name="FileName">Source file</param>
            public static List<GLVBOModel> LoadFile(string FileName)
            {
                try
                {
                    Lab3DModelHolder model = new Lab3DModelHolder(FileName);

                    //Copies buffer objects to this instance 
                    List<GLVBOModel> vboModels = new List<GLVBOModel>();

                    for (int i = 0; i < model._partes.Count; i++)
                    {
                        object[] o;
                        model.geraPoligonosParte(model._partes[i], out o);

                        List<float> VertCoordsData = (List<float>)o[0];
                        List<float> ColorData = (List<float>)o[1];
                        List<float> TexCoordsData = (List<float>)o[2];
                        List<int> ElementData = (List<int>)o[3];
                        List<float> NormalsData = (List<float>)o[4];



                        GLVBOModel m = new GLVBOModel(BeginMode.Triangles);
                        m.Name = model.Name;
                        m.UseDrawElements = false;


                        m.SetColorData(ColorData.ToArray());
                        m.SetVertexData(VertCoordsData.ToArray());
                        m.SetNormalData(NormalsData.ToArray());
                        m.SetElemData(ElementData.ToArray());

                        if (model.TextureBitmap != null)
                        {
                            m.SetTexCoordData(TexCoordsData.ToArray());
                            if (i == 0)
                            {
                                //Creates texture for 1st
                                m.SetTexture(new Bitmap(model.TextureBitmap));
                            }
                            else
                            {
                                //Just copies texture pointer
                                m.GLTextureBuffer = vboModels[0].GLTextureBuffer;
                            }
                        }

                        vboModels.Add(m);
                    }

                    //Frees memory
                    model = null;
                    GC.Collect();

                    return vboModels;
                }
                catch
                {
                    throw new Exception("Could not read 3D model file");
                }
                
            }

            /// <summary>Disposes buffer objects</summary>
            public void Dispose()
            {
                if (GLVertexBuffer != 0) GL.DeleteBuffers(1, ref GLVertexBuffer);
                if (GLColorBuffer != 0) GL.DeleteBuffers(1, ref GLColorBuffer);
                if (GLNormalBuffer != 0) GL.DeleteBuffers(1, ref GLNormalBuffer);
                if (GLTexCoordBuffer != 0) GL.DeleteBuffers(1, ref GLTexCoordBuffer);
                if (GLElemBuffer != 0) GL.DeleteBuffers(1, ref GLElemBuffer);

                GLVertexBuffer = 0;
                GLColorBuffer = 0;
                GLNormalBuffer = 0;
                GLTexCoordBuffer = 0;
                GLElemBuffer = 0;
            }
            #endregion

            #region Buffers creation

            /// <summary>VBO draw mode</summary>
            public BeginMode DrawMode;

            /// <summary>GL Vertex VBO (xyz)</summary>
            public int GLVertexBuffer = 0;
            /// <summary>GL Color VBO (RGBA)</summary>
            public int GLColorBuffer = 0;
            /// <summary>GL Normals VBO (xyz)</summary>
            public int GLNormalBuffer = 0;
            /// <summary>GL Tex Coords VBO (xy)</summary>
            public int GLTexCoordBuffer = 0;
            /// <summary>GL Element buffer VBO (v1 v2 v3)</summary>
            public int GLElemBuffer = 0;
            /// <summary>OpenGL texture buffer object</summary>
            public int GLTextureBuffer = 0;

            /// <summary>Length of elements vector (total triangles = ElemLength/3)</summary>
            public int ElemLength = 1;

            /// <summary>How many vertexes are there?</summary>
            public int numVertexes = 1;

            #region Set VBO Data and texture
            /// <summary>Sets vertex data information</summary>
            /// <param name="VertexData">Vertex data information. v[3i] = x component of i-th vector, x[3i+1] = y component, x[3i+2] = z component</param>
            public void SetVertexData(float[] VertexData)
            {
                /*
                 * http://www.songho.ca/opengl/gl_vbo.html
                 * "Static" means the data in VBO will not be changed (specified once and used many times), 
                 * "dynamic" means the data will be changed frequently (specified and used repeatedly), and "stream" 
                 * means the data will be changed every frame (specified once and used once). "Draw" means the data will 
                 * be sent to GPU in order to draw (application to GL), "read" means the data will be read by the client's 
                 * application (GL to application), and "copy" means the data will be used both drawing and reading (GL to GL). 
                 */
                SetVertexData(VertexData, BufferUsageHint.DynamicDraw);
            }

            /// <summary>Sets vertex data information</summary>
            /// <param name="VertexData">Vertex data information. v[3i] = x component of i-th vector, x[3i+1] = y component, x[3i+2] = z component</param>
            /// <param name="Hint">OpenGL buffer usage hint</param>
            public void SetVertexData(float[] VertexData, BufferUsageHint Hint)
            {
                lock (this)
                {
                    if (GLVertexBuffer == 0)
                    {
                        GL.GenBuffers(1, out GLVertexBuffer);
                    }

                    GL.BindBuffer(BufferTarget.ArrayBuffer, GLVertexBuffer);
                    GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(VertexData.Length * sizeof(float)), VertexData, Hint);

                    GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                    numVertexes = VertexData.Length / 3;
                }
            }

            /// <summary>Sets vertex normals data information</summary>
            /// <param name="NormalVertexData">Normals data information. v[3i] = x component of i-th vector normal, x[3i+1] = y component, x[3i+2] = z component</param>
            public void SetNormalData(float[] NormalVertexData)
            {
                SetNormalData(NormalVertexData, BufferUsageHint.DynamicDraw);
            }

            /// <summary>Sets vertex normals data information</summary>
            /// <param name="NormalVertexData">Normals data information. v[3i] = x component of i-th vector normal, x[3i+1] = y component, x[3i+2] = z component</param>
            /// <param name="Hint">OpenGL buffer usage hint</param>
            public void SetNormalData(float[] NormalVertexData, BufferUsageHint Hint)
            {
                lock (this)
                {

                    if (GLNormalBuffer == 0)
                    {
                        GL.GenBuffers(1, out GLNormalBuffer);
                    }

                    GL.BindBuffer(BufferTarget.ArrayBuffer, GLNormalBuffer);
                    GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(NormalVertexData.Length * sizeof(float)), NormalVertexData, Hint);

                    GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

                }
            }


            /// <summary>Sets texture coordinate data information</summary>
            /// <param name="TexData">Vertex data information. v[2i] = x texture coord, x[2i+1] = y texture coordinate</param>
            public void SetTexCoordData(float[] TexData)
            {
                SetTexCoordData(TexData, BufferUsageHint.DynamicDraw);
            }

            /// <summary>Sets texture coordinate data information</summary>
            /// <param name="TexData">Vertex data information. v[2i] = x texture coord, x[2i+1] = y texture coordinate</param>
            /// <param name="Hint">OpenGL buffer usage hint</param>
            public void SetTexCoordData(float[] TexData, BufferUsageHint Hint)
            {
                lock (this)
                {

                    if (GLTexCoordBuffer == 0)
                    {
                        GL.GenBuffers(1, out GLTexCoordBuffer);
                    }

                    GL.BindBuffer(BufferTarget.ArrayBuffer, GLTexCoordBuffer);
                    GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(TexData.Length * sizeof(float)), TexData, Hint);

                    GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

                }
            }

            /// <summary>Sets color information</summary>
            /// <param name="ColorData">Vertex data information. v[4i] = R, x[4i+1] = G, x[4i+2]=B, x[4i+3]=alpha</param>
            public void SetColorData(float[] ColorData)
            {
                SetColorData(ColorData, BufferUsageHint.DynamicDraw);
            }

            /// <summary>Sets color information</summary>
            /// <param name="ColorData">Vertex data information. v[4i] = R, x[4i+1] = G, x[4i+2]=B, x[4i+3]=alpha</param>
            /// <param name="Hint">OpenGL buffer usage hint</param>
            public void SetColorData(float[] ColorData, BufferUsageHint Hint)
            {
                lock (this)
                {

                    if (GLColorBuffer == 0)
                    {
                        GL.GenBuffers(1, out GLColorBuffer);
                    }

                    GL.BindBuffer(BufferTarget.ArrayBuffer, GLColorBuffer);
                    GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(ColorData.Length * sizeof(float)), ColorData, Hint);

                    GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

                }
            }

            /// <summary>Sets vertex normals data information</summary>
            /// <param name="ElemData">Element data information. v[3i]  v[3i+1] and v[3i+2] are indexes of vertexes that will be drawn</param>
            public void SetElemData(int[] ElemData)
            {
                SetElemData(ElemData, BufferUsageHint.DynamicDraw);
            }

            /// <summary>Sets vertex normals data information</summary>
            /// <param name="ElemData">Element data information. v[3i]  v[3i+1] and v[3i+2] are indexes of vertexes that will be drawn</param>
            /// <param name="Hint">OpenGL buffer usage hint</param>
            public void SetElemData(int[] ElemData, BufferUsageHint Hint)
            {
                lock (this)
                {

                    if (GLElemBuffer == 0)
                    {
                        GL.GenBuffers(1, out GLElemBuffer);
                    }

                    GL.BindBuffer(BufferTarget.ArrayBuffer, GLElemBuffer);
                    GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(ElemData.Length * sizeof(int)), ElemData, Hint);

                    GL.BindBuffer(BufferTarget.ArrayBuffer, 0);


                    ElemLength = ElemData.Length;
                }
            }

            /// <summary>Creates a texture from a bitmap and associates with this model. Note: texture is flipped in Y axis, needs to correct texture coordinates.
            /// Models created from equations are already corrected</summary>
            /// <param name="bmp">Bitmap to create texture from</param>
            public void SetTexture(Bitmap bmp)
            {
                CLGLInteropFunctions.ApplyTexture(bmp, ref this.GLTextureBuffer);
                this.CLTexture2D = null;
            }
            #endregion

            #endregion

            #region Model draw

            /// <summary>Radian to degree conversion</summary>
            private static float rad2deg = (float)180 / (float)Math.PI;

            /// <summary>Show this model?</summary>
            public bool ShowModel = true;

            /// <summary>Object translation vector from origin.</summary>
            public Vector vetTransl = new Vector(0, 0, 0);
            /// <summary>Object rotation vector in Euler angles (psi-theta-phi).</summary>
            public Vector vetRot = new Vector(0, 0, 0); //em angulos de Euler psi theta phi
            /// <summary>Model scaling {ScaleX, ScaleY, ScaleZ}</summary>
            public float[] Scale = new float [] {1.0f, 1.0f,1.0f};

            /// <summary>This can be used to set model color if color buffer is not being used. Order: RGBA</summary>
            public float[] ModelColor = new float[] { 1.0f, 1.0f, 1.0f, 1.0f };

            /// <summary>Draws this model</summary>
            public void DrawModel()
            {
                if (!this.ShowModel) return;

                GL.PushMatrix();
                GL.Translate((float)vetTransl.x, (float)vetTransl.y, (float)vetTransl.z);

                GL.Rotate((float)vetRot.z * rad2deg, 0.0f, 0.0f, 1.0f);
                GL.Rotate((float)vetRot.y * rad2deg, 0.0f, 1.0f, 0.0f);
                GL.Rotate((float)vetRot.x * rad2deg, 1.0f, 0.0f, 0.0f);

                GL.Scale(Scale[0], Scale[1], Scale[2]);

                GL.Color4(ModelColor[0], ModelColor[1], ModelColor[2], ModelColor[3]);

                //Applies texture associated with this object or zero if it was neither initialized nor set
                GL.BindTexture(TextureTarget.Texture2D, this.GLTextureBuffer);

                //Draws Vertex Buffer Objects onto the screen
                DrawModelVBOs();


                GL.BindTexture(TextureTarget.Texture2D, 0);
                GL.PopMatrix();
            }

            private void DrawModelVBOs()
            {
                #region Draw buffer objects

                if (GLVertexBuffer != 0)
                {
                    GL.BindBuffer(BufferTarget.ArrayBuffer, GLVertexBuffer);
                    GL.VertexPointer(3, VertexPointerType.Float, 0, 0);

                    GL.EnableClientState(ArrayCap.VertexArray);
                }

                if (GLNormalBuffer != 0)
                {
                    GL.BindBuffer(BufferTarget.ArrayBuffer, GLNormalBuffer);
                    GL.NormalPointer(NormalPointerType.Float, 0, 0);

                    GL.EnableClientState(ArrayCap.NormalArray);
                }

                if (GLColorBuffer != 0)
                {
                    GL.BindBuffer(BufferTarget.ArrayBuffer, GLColorBuffer);
                    GL.ColorPointer(4, ColorPointerType.Float, 0, 0);

                    GL.EnableClientState(ArrayCap.ColorArray);
                }


                if (GLTexCoordBuffer != 0 && GLTextureBuffer > 0)
                {
                    GL.BindBuffer(BufferTarget.ArrayBuffer, GLTexCoordBuffer);
                    GL.TexCoordPointer(2, TexCoordPointerType.Float, 0, 0);

                    GL.EnableClientState(ArrayCap.TextureCoordArray);
                }

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, GLElemBuffer);
                
                if (UseDrawElements) GL.DrawElements(DrawMode, ElemLength, DrawElementsType.UnsignedInt, 0);
                else GL.DrawArrays(DrawMode, 0, ElemLength);

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

                if (GLVertexBuffer != 0)
                {
                    GL.DisableClientState(ArrayCap.VertexArray);
                    GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                }
                if (GLColorBuffer != 0)
                {
                    GL.DisableClientState(ArrayCap.ColorArray);
                    GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                }
                if (GLNormalBuffer != 0)
                {
                    GL.DisableClientState(ArrayCap.NormalArray);
                    GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                }
                if (GLTexCoordBuffer != 0 && GLTextureBuffer > 0)
                {
                    GL.DisableClientState(ArrayCap.TextureCoordArray);
                    GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                }

                #endregion

            }
            #endregion

            #region Retrieving OpenCL buffers from OpenGL buffers

            /// <summary>Local storage of element data buffer</summary>
            private CLCalc.Program.Variable CLElemBuffer;
            /// <summary>Retrieves an OpenCL float buffer from this object's OpenGL elements VBO (3 ints per element)</summary>
            public CLCalc.Program.Variable GetCLElemBuffer()
            {
                if (CLElemBuffer == null)
                {
                    CLElemBuffer = new CLCalc.Program.Variable(GLElemBuffer,typeof(int));
                }
                return CLElemBuffer;
            }

            /// <summary>Local storage of color buffer</summary>
            private CLCalc.Program.Variable CLColorBuffer;
            /// <summary>Retrieves an OpenCL float buffer from this object's OpenGL color data VBO (4 floats per vertex)</summary>
            public CLCalc.Program.Variable GetCLColorBuffer()
            {
                if (CLColorBuffer == null)
                {
                    CLColorBuffer = new CLCalc.Program.Variable(GLColorBuffer, typeof(float));
                }
                return CLColorBuffer;
            }

            /// <summary>Local storage of texture coordinates buffer</summary>
            private CLCalc.Program.Variable CLTexCoordBuffer;
            /// <summary>Retrieves an OpenCL float buffer from this object's OpenGL texture coordinate data VBO (2 floats per vertex)</summary>
            public CLCalc.Program.Variable GetCLTexCoordBuffer()
            {
                if (CLTexCoordBuffer == null)
                {
                    CLTexCoordBuffer = new CLCalc.Program.Variable(GLTexCoordBuffer, typeof(float));
                }
                return CLTexCoordBuffer;
            }

            /// <summary>Local storage of vertex normals buffer</summary>
            private CLCalc.Program.Variable CLNormalBuffer;
            /// <summary>Retrieves an OpenCL float buffer from this object's OpenGL vertex normals data VBO (3 floats per vertex)</summary>
            public CLCalc.Program.Variable GetCLNormalBuffer()
            {
                if (CLNormalBuffer == null)
                {
                    CLNormalBuffer = new CLCalc.Program.Variable(GLNormalBuffer, typeof(float));
                }
                return CLNormalBuffer;
            }

            /// <summary>Local storage of vertex buffer</summary>
            private CLCalc.Program.Variable CLVertexBuffer;
            /// <summary>Retrieves an OpenCL float buffer from this object's OpenGL vertex data VBO (3 floats per vertex)</summary>
            public CLCalc.Program.Variable GetCLVertexBuffer()
            {
                if (CLVertexBuffer == null)
                {
                    CLVertexBuffer = new CLCalc.Program.Variable(GLVertexBuffer, typeof(float));
                }
                return CLVertexBuffer;
            }

            /// <summary>Local storage of vertex buffer</summary>
            private CLCalc.Program.Image2D CLTexture2D;
            /// <summary>Retrieves an OpenCL float buffer from this object's OpenGL vertex data VBO (3 floats per vertex)
            /// IMPORTANT: The data type of this object from the HOST standpoint is BYTE in the Alpha Blue Green Red order.
            /// The data type in OpenCL C99 is FLOAT4 (write_imagef) and in OpenGL the display order is BLUE GREEN RED ALPHA when modifying from OpenCL</summary>
            public CLCalc.Program.Image2D GetCLTexture2D()
            {
                if (this.GLTextureBuffer <= 0) throw new Exception("No texture is associated with this model");
                if (CLTexture2D == null)
                {
                    CLTexture2D = new CLCalc.Program.Image2D(GLTextureBuffer);
                }
                return CLTexture2D;
            }

            #endregion

            #region Creating buffers from equations
            /// <summary>Creates a surface from given equations. Parameters are u and v (strings). Eg: vertexEqs[0] = "u+v"</summary>
            /// <param name="uParams">U coordinate parameters: [0] - uMin, [1] - uMax, [2] - number of points</param>
            /// <param name="vParams">V coordinate parameters: [0] - vMin, [1] - vMax, [2] - number of points</param>
            /// <param name="vertexEqs">Array containing 3 strings that will define vertex positions. [0] x(u,v), [1] y(u,v), [2] z(u,v)</param>
            /// <param name="colorEqs">Array containing 4 strings that will define vertex colors R(u,v), G(u,v), B(u,v), A(u,v). May contain only RGB</param>
            /// <param name="normalsEqs">Array containing strings that will define vertex normals</param>
            public static GLVBOModel CreateSurface(float[] uParams, float[] vParams, string[] vertexEqs, string[] colorEqs, string[] normalsEqs)
            {
                GLVBOModel model = new GLVBOModel(BeginMode.Triangles);

                int uPts = (int)uParams[2];
                int vPts = (int)vParams[2];

                float[] vertexes = new float[3 * uPts * vPts];
                float[] normals = new float[3 * uPts * vPts];
                float[] colors = new float[4 * uPts * vPts];
                float[] texCoords = new float[2 * uPts * vPts];

                int[] elems = new int[6 * (uPts - 1) * (vPts - 1)]; //3 vertices per element, 2 triangles to make a square

                model.SetColorData(colors);
                model.SetNormalData(normals);
                model.SetVertexData(vertexes);
                model.SetElemData(elems);
                model.SetTexCoordData(texCoords);

                //Reads GL buffers
                CLCalc.Program.Variable CLvertex = model.GetCLVertexBuffer();
                CLCalc.Program.Variable CLnormal = model.GetCLNormalBuffer();
                CLCalc.Program.Variable CLcolor = model.GetCLColorBuffer();
                CLCalc.Program.Variable CLelem = model.GetCLElemBuffer();
                CLCalc.Program.Variable CLTexCoords = model.GetCLTexCoordBuffer();

                CLCalc.Program.Variable[] args = new CLCalc.Program.Variable[] { CLvertex, CLnormal, CLcolor, CLelem, CLTexCoords };


                //Creates source
                #region Assembles OpenCL source
                string src = @"

//enqueue with dimensions uPts-1, vPts-1
__kernel void CreateElems(__global int* elems)
{
  int i = get_global_id(0);
  int w = get_global_size(0);
  
  int j = get_global_id(1);
  
  int ind = 6*(i+w*j);
  w++;
  elems[ind] = i+w*j;
  elems[ind+1] = i+1+w*j;
  elems[ind+2] = i+1+w*j+w;
  
  elems[ind+3] = i+w*j;
  elems[ind+4] = i+1+w*j+w;
  elems[ind+5] = i+w*j+w;
}

__kernel void f(__global float* vertex,
                __global float* normal,
                __global float* colors,
                __global float* texCoords,
                __constant float* uvMinStep)

{
   int i = get_global_id(0);
   int w = get_global_size(0); //Matrix width
   
   int j = get_global_id(1);
   int h = get_global_size(1);
   
   float uMin = uvMinStep[0];
   float uStep = uvMinStep[1];

   float vMin = uvMinStep[2];
   float vStep = uvMinStep[3];
   
   float u = uMin + uStep*(float)i;
   float v = vMin + vStep*(float)j;
   
   //Texture coordinates
   int ind = 2*(i+w*j);
   texCoords[ind] = (float)i/((float)w - 1.0f);
   texCoords[ind+1] = (float)(h-1-j)/((float)h - 1.0f);

   //Vertexes
   ind = 3*(i+w*j);
   vertex[ind] = " + vertexEqs[0] + @";
   vertex[ind+1] = " + vertexEqs[1] + @";
   vertex[ind+2] = " + vertexEqs[2] + @";
   
   //Normals
   float4 n;
   n.x = " + normalsEqs[0] + @";
   n.y = " + normalsEqs[1] + @";
   n.z = " + normalsEqs[2] + @";
   n.w = 0;
   n = normalize(n);
   normal[ind] = n.x;
   normal[ind+1] = n.y;
   normal[ind+2] = n.z;
   
   
   //Colors
   ind = (i+w*j)<<2;
   colors[ind] = " + colorEqs[0] + @";
   colors[ind+1] = " + colorEqs[1] + @";
   colors[ind+2] = " + colorEqs[2] + @";
   colors[ind+3] = " + (colorEqs.Length >= 4 ? colorEqs[4] : "1") + @";
   
}
";
                #endregion

                //Creates kernel
                CLCalc.Program.Compile(src);

                CLCalc.Program.Kernel kernelEquations = new CLCalc.Program.Kernel("f");
                CLCalc.Program.Kernel createElems = new CLCalc.Program.Kernel("CreateElems");

                //Information vector
                float[] uvminStep = new float[] { uParams[0], (uParams[1] - uParams[0]) / (uPts-1), vParams[0], (vParams[1] - vParams[0]) / (vPts-1)};
                CLCalc.Program.Variable CLuvminStep = new CLCalc.Program.Variable(uvminStep);

                //CLCalc.Program.Variable CLElem2 = new CLCalc.Program.Variable(elems);

                //Acquires to OpenCL
                CLGLInterop.CLGLInteropFunctions.AcquireGLElements(args);

                //Runs kernels
                createElems.Execute(new CLCalc.Program.Variable[] {CLelem}, new int[] { uPts - 1, vPts - 1 });
                kernelEquations.Execute(new CLCalc.Program.Variable[] 
                {
                    CLvertex, CLnormal, CLcolor, CLTexCoords, CLuvminStep
                }
                , new int[] { uPts, vPts });



                //Releases from OpenCL
                CLGLInterop.CLGLInteropFunctions.ReleaseGLElements(args);

                return model;
            }

            /// <summary>Creates a surface from given equations. Parameters are u and v (strings). Eg: vertexEqs[0] = "u+v"</summary>
            /// <param name="uParams">U coordinate parameters: [0] - uMin, [1] - uMax, [2] - number of points</param>
            /// <param name="vParams">V coordinate parameters: [0] - vMin, [1] - vMax, [2] - number of points</param>
            /// <param name="vertexEqs">Array containing 3 strings that will define vertex positions. [0] x(u,v), [1] y(u,v), [2] z(u,v)</param>
            /// <param name="normalsEqs">Array containing strings that will define vertex normals</param>
            public static GLVBOModel CreateSurface(float[] uParams, float[] vParams, string[] vertexEqs, string[] normalsEqs)
            {
                GLVBOModel model = new GLVBOModel(BeginMode.Triangles);

                int uPts = (int)uParams[2];
                int vPts = (int)vParams[2];

                float[] vertexes = new float[3 * uPts * vPts];
                float[] normals = new float[3 * uPts * vPts];
                float[] texCoords = new float[2 * uPts * vPts];

                int[] elems = new int[6 * (uPts - 1) * (vPts - 1)]; //3 vertices per element, 2 triangles to make a square

                model.SetNormalData(normals);
                model.SetVertexData(vertexes);
                model.SetElemData(elems);
                model.SetTexCoordData(texCoords);

                //Reads GL buffers
                CLCalc.Program.Variable CLvertex = model.GetCLVertexBuffer();
                CLCalc.Program.Variable CLnormal = model.GetCLNormalBuffer();
                CLCalc.Program.Variable CLelem = model.GetCLElemBuffer();
                CLCalc.Program.Variable CLTexCoords = model.GetCLTexCoordBuffer();

                CLCalc.Program.Variable[] args = new CLCalc.Program.Variable[] { CLvertex, CLnormal, CLelem, CLTexCoords };


                //Creates source
                #region Assembles OpenCL source
                string src = @"

//enqueue with dimensions uPts-1, vPts-1
__kernel void CreateElems(__global int* elems)
{
  int i = get_global_id(0);
  int w = get_global_size(0);
  
  int j = get_global_id(1);
  
  int ind = 6*(i+w*j);
  w++;
  elems[ind] = i+w*j;
  elems[ind+1] = i+1+w*j;
  elems[ind+2] = i+1+w*j+w;
  
  elems[ind+3] = i+w*j;
  elems[ind+4] = i+1+w*j+w;
  elems[ind+5] = i+w*j+w;
}

__kernel void f(__global float* vertex,
                __global float* normal,
                __global float* texCoords,
                __constant float* uvMinStep)

{
   int i = get_global_id(0);
   int w = get_global_size(0); //Matrix width
   
   int j = get_global_id(1);
   int h = get_global_size(1);
   
   float uMin = uvMinStep[0];
   float uStep = uvMinStep[1];

   float vMin = uvMinStep[2];
   float vStep = uvMinStep[3];
   
   float u = uMin + uStep*(float)i;
   float v = vMin + vStep*(float)j;
   
   //Texture coordinates
   int ind = 2*(i+w*j);
   texCoords[ind] = (float)i/((float)w - 1.0f);
   texCoords[ind+1] = (float)(h-1-j)/((float)h - 1.0f);

   //Vertexes
   ind = 3*(i+w*j);
   vertex[ind] = " + vertexEqs[0] + @";
   vertex[ind+1] = " + vertexEqs[1] + @";
   vertex[ind+2] = " + vertexEqs[2] + @";
   
   //Normals
   float4 n;
   n.x = " + normalsEqs[0] + @";
   n.y = " + normalsEqs[1] + @";
   n.z = " + normalsEqs[2] + @";
   n.w = 0;
   n = normalize(n);
   normal[ind] = n.x;
   normal[ind+1] = n.y;
   normal[ind+2] = n.z;

   
}
";
                #endregion

                //Creates kernel
                CLCalc.Program.Compile(src);

                CLCalc.Program.Kernel kernelEquations = new CLCalc.Program.Kernel("f");
                CLCalc.Program.Kernel createElems = new CLCalc.Program.Kernel("CreateElems");

                //Information vector
                float[] uvminStep = new float[] { uParams[0], (uParams[1] - uParams[0]) / (uPts - 1), vParams[0], (vParams[1] - vParams[0]) / (vPts - 1) };
                CLCalc.Program.Variable CLuvminStep = new CLCalc.Program.Variable(uvminStep);

                //CLCalc.Program.Variable CLElem2 = new CLCalc.Program.Variable(elems);

                //Acquires to OpenCL
                CLGLInterop.CLGLInteropFunctions.AcquireGLElements(args);

                //Runs kernels
                createElems.Execute(new CLCalc.Program.Variable[] { CLelem }, new int[] { uPts - 1, vPts - 1 });
                kernelEquations.Execute(new CLCalc.Program.Variable[] 
                {
                    CLvertex, CLnormal, CLTexCoords, CLuvminStep
                }
                , new int[] { uPts, vPts });



                //Releases from OpenCL
                CLGLInterop.CLGLInteropFunctions.ReleaseGLElements(args);

                return model;
            }
            #endregion



            #region Lab3D Model holder, used to load .DXF and .OBJ files

            /// <summary>3D Models handler from Lab3D software. Used to create 3D models from .OBJ and .DXF</summary>
            private class Lab3DModelHolder
            {
                /// <summary>Checks if hardware supports Buffer Objects</summary>
                public static bool HardwareSupportsBufferObjects = true;

                /// <summary>3D Model name.</summary>
                public string Name = "";

                #region Propriedades geometricas / cor: areas, vertices, CG, dimensoes
                /// <summary>Struct to hold a polygon area.</summary>
                public struct area
                {
                    /// <summary>List of indexes of vertexes that create this area.</summary>
                    public List<int> IndVertices; //area contem quais vertices?
                    /// <summary>List of indexes of normal vectors of this area's vertexes.</summary>
                    public List<int> IndNormais;
                    /// <summary>List of indexes of texture vectors of this area's vertexes.</summary>
                    public List<int> IndTexVertexes;

                }
                /// <summary>Struct to hold a polygon vertex.</summary>
                public struct Vertex
                {
                    /// <summary>List of indexes of areas that use this vertex.</summary>
                    public List<int> _indAreas; //vertice pertence a quais areas?
                    /// <summary>List of indexes of 3D model parts that use this vertex.</summary>
                    public List<int> _indParte; //de qual parte do desenho?
                    /// <summary>List of indexes of areas that use this vertex.</summary>
                    public List<int> _indNormais; //quais sao os vetores normais associados a ele?

                    /// <summary>Vertex coordinates.</summary>
                    public Vector Coords;

                    /// <summary>Vertex color, RGBA</summary>
                    public float[] VertexColor;
                }

                /// <summary>3D Model parts holder.</summary>
                public class Parte
                {
                    /// <summary>Part name.</summary>
                    public string Name;
                    /// <summary>List of areas that belong to this part.</summary>
                    public List<area> Areas;
                    /// <summary>Color of this area.</summary>
                    public Vector Cor;
                    /// <summary>Transparency of this area.</summary>
                    public float Transparencia;
                    /// <summary>Display this part as selected?</summary>
                    public bool Selecionar;

                    /// <summary>OpenGL list number of this part.</summary>
                    public int GLListNumber; //numero da lista da GLList a chamar

                    /// <summary>Buffer objects to use if the hardware supports buffer objects</summary>
                    public int[] GLBuffers = null;
                    /// <summary>Number of elements to draw using buffer objects</summary>
                    public int GLNumElements;


                    #region Construtor
                    /// <summary>Constructor. Copies a given part.</summary>
                    /// <param name="p">Part to copy from.</param>
                    public Parte(Parte p)
                    {
                        this.Name = p.Name;
                        this.Areas = new List<area>(p.Areas);
                        this.Cor = new Vector(p.Cor);
                        this.Transparencia = p.Transparencia;
                        this.Selecionar = p.Selecionar;
                        this.GLListNumber = p.GLListNumber;
                    }
                    /// <summary>Empty constructor.</summary>
                    public Parte()
                    {
                        this.Name = "";
                        this.Areas = new List<area>();
                        this.Cor = new Vector();
                        this.Transparencia = 0.0f;
                        this.Selecionar = false;
                        this.GLListNumber = -1;
                    }

                    #endregion

                }

                /// <summary>List of parts that construct this 3D Model.</summary>
                public List<Parte> _partes = new List<Parte>();
                /// <summary>List of this 3D Model's vertexes.</summary>
                public List<Vertex> _vertices = new List<Vertex>();
                /// <summary>List of this 3D Model's normal vectors.</summary>
                public List<Vector> _vetNormais = new List<Vector>();
                /// <summary>List of this 3D Model's texture vectors coordinates.</summary>
                public List<float[]> _vetTextureCoords = new List<float[]>();

                /// <summary>Texture bitmap to use</summary>
                public System.Drawing.Bitmap TextureBitmap;
                /// <summary>OpenGL texture of this model</summary>
                public int GLTexture = 0;


                /// <summary>Vertexes average coordinate (center of gravity).</summary>
                public Vector CG = new Vector(0, 0, 0);
                /// <summary>Size of rectangle that contains this 3D Model.</summary>
                public Vector Dimens = new Vector(0, 0, 0); //dimensao do retangulo que contem o objeto
                /// <summary>Minimum Z coordinate of this model. Useful for reading terrain.</summary>
                public Vector minPoint = new Vector(0, 0, 0); //menor Z
                #endregion

                #region Construtores

                /// <summary>Empty constructor.</summary>
                public Lab3DModelHolder()
                {
                }

                /// <summary>Constructor. Creates a 3D Model from parameterized 3D equations</summary>
                /// <param name="Name">Model name.</param>
                /// <param name="F">Coordinate function F. Has to return double[3] {x, y, z} or double[6] {x,y,z,nx,ny,nz}, n? = normals</param>
                /// <param name="umin">Minimum value of u coordinate.</param>
                /// <param name="umax">Maximum value of u coordinate.</param>
                /// <param name="vmin">Minimum value of v coordinate.</param>
                /// <param name="vmax">Maximum value of v coordinate.</param>
                /// <param name="uPts">Number of points in u partition.</param>
                /// <param name="vPts">Number of points in v partition.</param>
                /// <param name="cor">Color vector. x=red, y=green, z=blue. Goes from 0 to 1.</param>
                public Lab3DModelHolder(string Name, CoordFuncXYZ F,
                    float umin, float umax, float vmin, float vmax, int uPts, int vPts, Vector cor)
                {
                    Create3DModel(Name, F, umin, umax, vmin, vmax, uPts, vPts, cor);
                }

                /// <summary>Constructor. Creates a 3D Model from parameterized 3D equations with texture.</summary>
                /// <param name="Name">Model name.</param>
                /// <param name="F">Coordinate function F. Has to return double[3] {x, y, z} or double[6] {x,y,z,nx,ny,nz}, n? = normals</param>
                /// <param name="umin">Minimum value of u coordinate.</param>
                /// <param name="umax">Maximum value of u coordinate.</param>
                /// <param name="vmin">Minimum value of v coordinate.</param>
                /// <param name="vmax">Maximum value of v coordinate.</param>
                /// <param name="uPts">Number of points in u partition.</param>
                /// <param name="vPts">Number of points in v partition.</param>
                /// <param name="cor">Color vector. x=red, y=green, z=blue. Goes from 0 to 1.</param>
                /// <param name="Texture">Texture bitmap.</param>
                public Lab3DModelHolder(string Name, CoordFuncXYZ F,
                    float umin, float umax, float vmin, float vmax, int uPts, int vPts, Vector cor, System.Drawing.Bitmap Texture)
                {
                    this.TextureBitmap = Texture;
                    Create3DModel(Name, F, umin, umax, vmin, vmax, uPts, vPts, cor);
                }

                /// <summary>Constructor. Creates a 3D Model from parameterized 3D line equations.</summary>
                /// <param name="Name">Model name.</param>
                /// <param name="F">Coordinate function F. Has to return double[3] {x, y, z} or double[6] {x,y,z,nx,ny,nz}, n? = normals.
                /// Notice only parameter u is passed (v=0)</param>
                /// <param name="umin">Minimum value of u coordinate.</param>
                /// <param name="umax">Maximum value of u coordinate.</param>
                /// <param name="uPts">Number of points in u partition.</param>
                /// <param name="cor">Color vector. x=red, y=green, z=blue. Goes from 0 to 1.</param>
                public Lab3DModelHolder(string Name, CoordFuncXYZ F,
                    float umin, float umax, int uPts, Vector cor)
                {
                    Create3DLine(Name, F, umin, umax, uPts, cor);
                }

                /// <summary>Delegate to create coordinates in the new 3D Model.</summary>
                /// <param name="u">Independent parameter u.</param>
                /// <param name="v">Independent parameter v.</param>
                public delegate float[] CoordFuncXYZ(float u, float v);



                private string separadorDecimal = (1.5).ToString().Substring(1, 1);

                /// <summary>Removes spaces from line and understands , and . as decimal separators.</summary>
                /// <param name="linha">Line to be rewritten.</param>
                private string trataLinha(string linha)
                {
                    linha = linha.Replace("    ", " ");
                    linha = linha.Replace("   ", " ");
                    linha = linha.Replace("  ", " ");
                    linha = linha.Replace(".", separadorDecimal);
                    linha = linha.Replace(",", separadorDecimal);
                    return linha;
                }


                /// <summary>Constructor. Creates 3D Model from file.</summary>
                /// <param name="file">File to read 3D data.</param>
                public Lab3DModelHolder(string file)
                {
                    string ext = System.IO.Path.GetExtension(file).ToLower();
                    if (ext == ".obj") lerOBJ(file);
                    else if (ext == ".dxf") lerDXF(file);


                    if (this.Name == "")
                        this.Name = System.IO.Path.GetFileNameWithoutExtension(file);
                }

                /// <summary>Reads 3D model from Wavefront .OBJ</summary>
                /// <param name="fileOBJ">File to read</param>
                private void lerOBJ(string fileOBJ)
                {
                    using (System.IO.StreamReader sr = new System.IO.StreamReader(fileOBJ))
                    {
                        // Read and display lines from the file until the end of 
                        // the file is reached.
                        Parte part = new Parte(); Vertex v = new Vertex(); area a; int indV;

                        _partes = new List<Parte>();

                        string[] texto;
                        string line;

                        while (!sr.EndOfStream)
                        {

                            line = (sr.ReadLine()).Trim();

                            while (line.EndsWith("\\")) //caracter \ indica que continua na outra linha
                            {
                                line = line.Substring(0, line.Length - 1);
                                line = line + (sr.ReadLine()).Trim();
                            }

                            line = trataLinha(line);

                            texto = line.Split();

                            if (texto.Length >= 0) //existem caracteres
                            {

                                switch (texto[0].ToLower())
                                {
                                    case "mtllib":
                                        //check texture file
                                        GetTexture(texto[1], fileOBJ);

                                        break;
                                    case "vt":
                                        //Texture coord
                                        Vector vv2 = new Vector(0, 0, 0);
                                        double.TryParse(texto[1], (System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands), null, out vv2.x);
                                        double.TryParse(texto[2], (System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands), null, out vv2.y);

                                        _vetTextureCoords.Add(new float[] { (float)vv2.x, (float)vv2.y });

                                        break;
                                    case "vn":
                                        Vector vv = new Vector(0, 0, 0);
                                        double.TryParse(texto[1], (System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands), null, out vv.x);
                                        double.TryParse(texto[2], (System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands), null, out vv.y);
                                        double.TryParse(texto[3], (System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands), null, out vv.z);

                                        vv.normalize();

                                        _vetNormais.Add(vv);

                                        break;
                                    case "v": //vertice
                                        v.Coords = new Vector(0, 0, 0);
                                        v._indAreas = new List<int>();
                                        v._indParte = new List<int>();
                                        v._indNormais = new List<int>();
                                        double.TryParse(texto[1], (System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands), null, out v.Coords.x);
                                        double.TryParse(texto[2], (System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands), null, out v.Coords.y);
                                        double.TryParse(texto[3], (System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands), null, out v.Coords.z);

                                        _vertices.Add(v);
                                        break;
                                    case "f": //area
                                        a.IndVertices = new List<int>();
                                        a.IndNormais = new List<int>();
                                        a.IndTexVertexes = new List<int>();
                                        foreach (string txtVert in texto)
                                        {
                                            if (txtVert.ToLower() != "f")
                                            {
                                                //formato: num/num/num
                                                //so interessa o primeiro, o vertice
                                                string[] temp;
                                                temp = txtVert.Split('/');

                                                int.TryParse(temp[0], out indV);
                                                a.IndVertices.Add(indV - 1);

                                                //guarda as areas nos vertices, com sua respectiva parte
                                                int xx = a.IndVertices.Count - 1;
                                                int zz = _partes.Count - 1;
                                                _vertices[a.IndVertices[xx]]._indAreas.Add(xx);
                                                _vertices[a.IndVertices[xx]]._indParte.Add(zz);

                                                //guarda o vetor normal
                                                if (temp.Length > 2) //formato "v/vt/vn"
                                                {
                                                    int.TryParse(temp[temp.Length - 1], out indV);
                                                    int yy = indV - 1;

                                                    //se o vetor normal estiver nulo, calcula
                                                    if (!(double.IsNaN(_vetNormais[yy].x)))
                                                    {
                                                        a.IndNormais.Add(yy);
                                                        int kk = indV - 1;
                                                        _vertices[a.IndVertices[xx]]._indNormais.Add(kk);
                                                    }

                                                    //guarda texture coord
                                                    if (temp[temp.Length - 2] != "")
                                                    {
                                                        int.TryParse(temp[temp.Length - 2], out indV);
                                                        yy = indV - 1;

                                                        a.IndTexVertexes.Add(yy);
                                                    }
                                                }

                                                //formato "v/vt" , calcular normal

                                            }
                                        }

                                        //calcula o vetor normal
                                        //mas ele normalmente e' fornecido....
                                        //se o vetor normal estiver nulo, calcula
                                        if (a.IndVertices.Count != a.IndNormais.Count)
                                        {
                                            calcNormais(a);
                                        }

                                        part.Areas.Add(a);

                                        break;
                                    case "g": //novo grupo
                                        if (part.Areas.Count > 0) //nova regiao
                                        {
                                            if (TextureBitmap == null)
                                                part.Cor = new Vector(0.3 * Math.Cos(23 * _partes.Count) + 0.5, 0.5 * Math.Cos(17 * _partes.Count + 1) + 0.5, 0.5 * Math.Cos(_partes.Count) + 0.5);
                                            else
                                                part.Cor = new Vector(1, 1, 1);

                                            Parte novaParte = new Parte(part);
                                            _partes.Add(novaParte);
                                        }
                                        if (texto.Length > 1) part.Name = line.Replace(texto[1], "");
                                        part.Areas.Clear();

                                        break;
                                }
                            }
                        }
                        if (part.Areas.Count > 0) //salva a ultima parte
                        {
                            part.Cor = new Vector(1, 1, 1);
                            _partes.Add(part);
                        }

                        sr.Close();

                        FinishReadFile();
                    }
                }

                /// <summary>Does standard endfile operations (smooth normals, CG calculation)</summary>
                private void FinishReadFile()
                {
                    suavizaNormais();
                    calcCGBox();
                }

                /// <summary>Reads 3D model from Autodesk DXF</summary>
                /// <param name="fileDXF">File to read</param>
                private void lerDXF(string fileDXF)
                {
                    using (System.IO.StreamReader sr = new System.IO.StreamReader(fileDXF))
                    {
                        // Read and display lines from the file until the end of 
                        // the file is reached.
                        Parte part = new Parte();
                        part.Areas = new List<area>();
                        part.Cor = new Vector(0.5, 0.5, 0.5);
                        _partes = new List<Parte>();
                        _partes.Add(part);

                        string line;

                        while (!sr.EndOfStream)
                        {
                            line = (sr.ReadLine()).Trim();
                            line = trataLinha(line);

                            if (line.ToUpper() == "3DFACE" || line.ToUpper() == "SOLID")
                            {
                                ler3DFace(sr);
                            }
                            else if (line.ToUpper() == "POLYLINE")
                            {
                                lerPolyline(sr);
                            }
                        }

                        sr.Close();

                        FinishReadFile();
                    }
                }

                /// <summary>Reads 3DFACE element</summary>
                /// <param name="sr">Stream</param>
                private void ler3DFace(System.IO.StreamReader sr)
                {
                    Vertex[] v = lerCoords(sr);

                    area a = new area();
                    a.IndNormais = new List<int>(); a.IndVertices = new List<int>();
                    for (int i = 0; i < 4; i++)
                    {
                        if (i < 3 || (v[2].Coords.x != v[3].Coords.x || v[2].Coords.y != v[3].Coords.y || v[2].Coords.z != v[3].Coords.z))
                        {
                            v[i]._indAreas.Add(_partes[_partes.Count - 1].Areas.Count);
                            v[i]._indParte.Add(_partes.Count - 1);

                            a.IndVertices.Add(_vertices.Count);
                            _vertices.Add(v[i]);
                        }
                    }

                    _partes[_partes.Count - 1].Areas.Add(a);

                    calcNormais(a);

                }

                /// <summary>Reads POLYLINE element</summary>
                /// <param name="sr">Stream</param>
                private void lerPolyline(System.IO.StreamReader sr)
                {
                    string line = "";

                    //vertices para referenciar corretamente
                    int numVerts = _vertices.Count;


                    while (line.ToUpper() != "SEQEND" && !sr.EndOfStream)
                    {
                        line = (sr.ReadLine()).Trim(); line = trataLinha(line);
                        if (line.ToUpper() == "VERTEX")
                        {
                            Vertex v = lerCoord(sr);

                            while (line != "0" && !sr.EndOfStream)
                            {
                                line = (sr.ReadLine()).Trim(); line = trataLinha(line);

                                //Verifica que tipo de vertice é: vértice ou face
                                if (line == "70")
                                {
                                    line = (sr.ReadLine()).Trim(); line = trataLinha(line);

                                    if (line == "128") //face
                                    {
                                        area a = new area(); a.IndVertices = new List<int>(); a.IndNormais = new List<int>();
                                        bool LinhaPar = true;
                                        while (line != "0")
                                        {
                                            LinhaPar = !LinhaPar;
                                            line = (sr.ReadLine()).Trim();
                                            if (LinhaPar && line != "0")
                                            {
                                                line = trataLinha(line);

                                                int i;
                                                int.TryParse(line, out i);
                                                if (i > 0)
                                                {
                                                    a.IndVertices.Add(i - 1 + numVerts);
                                                    _vertices[i - 1]._indParte.Add(_partes.Count - 1);
                                                    _vertices[i - 1]._indAreas.Add(_partes[_partes.Count - 1].Areas.Count);
                                                }
                                            }
                                        }
                                        if (a.IndVertices.Count >= 3)
                                        {
                                            _partes[_partes.Count - 1].Areas.Add(a);
                                            calcNormais(a);
                                        }

                                    }
                                    else if (line == "32")
                                    {
                                        _vertices.Add(v);
                                        if (_vertices.Count - numVerts > 3)
                                        {
                                            area a = new area(); a.IndVertices = new List<int>(); a.IndNormais = new List<int>();
                                            a.IndVertices.Add(_vertices.Count - 1);
                                            a.IndVertices.Add(_vertices.Count - 2);
                                            a.IndVertices.Add(_vertices.Count - 3);
                                            _partes[_partes.Count - 1].Areas.Add(a);
                                            calcNormais(a);
                                        }
                                    }
                                    else //if (line == "192") //vertice
                                    {
                                        _vertices.Add(v);
                                    }
                                }
                            }

                        }
                    }
                }

                /// <summary>Reads 4 vertexes from a streamreader (10 20 30, 11 21 31, 12 22 32, 13 23 33)</summary>
                /// <param name="sr">Streamreader</param>
                private Vertex[] lerCoords(System.IO.StreamReader sr)
                {
                    int lidos = 0;
                    string line = "";

                    Vertex[] v = new Vertex[4]; for (int i = 0; i < 4; i++)
                    {
                        v[i] = new Vertex(); v[i].Coords = new Vector(); v[i]._indParte = new List<int>(); v[i]._indNormais = new List<int>(); v[i]._indAreas = new List<int>();
                    }

                    while (lidos < 12 && !sr.EndOfStream)
                    {
                        line = (sr.ReadLine()).Trim();
                        line = trataLinha(line);
                        #region Componente x
                        if (line == "10")
                        {
                            line = (sr.ReadLine()).Trim(); line = trataLinha(line);
                            double.TryParse(line, out v[0].Coords.x); lidos++;
                        }
                        else if (line == "11")
                        {
                            line = (sr.ReadLine()).Trim(); line = trataLinha(line);
                            double.TryParse(line, out v[1].Coords.x); lidos++;
                        }
                        else if (line == "12")
                        {
                            line = (sr.ReadLine()).Trim(); line = trataLinha(line);
                            double.TryParse(line, out v[2].Coords.x); lidos++;
                        }
                        else if (line == "13")
                        {
                            line = (sr.ReadLine()).Trim(); line = trataLinha(line);
                            double.TryParse(line, out v[3].Coords.x); lidos++;
                        }
                        #endregion
                        #region Componente y
                        if (line == "20")
                        {
                            line = (sr.ReadLine()).Trim(); line = trataLinha(line);
                            double.TryParse(line, out v[0].Coords.y); lidos++;
                        }
                        else if (line == "21")
                        {
                            line = (sr.ReadLine()).Trim(); line = trataLinha(line);
                            double.TryParse(line, out v[1].Coords.y); lidos++;
                        }
                        else if (line == "22")
                        {
                            line = (sr.ReadLine()).Trim(); line = trataLinha(line);
                            double.TryParse(line, out v[2].Coords.y); lidos++;
                        }
                        else if (line == "23")
                        {
                            line = (sr.ReadLine()).Trim(); line = trataLinha(line);
                            double.TryParse(line, out v[3].Coords.y); lidos++;
                        }
                        #endregion
                        #region Componente z

                        if (line == "30")
                        {
                            line = (sr.ReadLine()).Trim(); line = trataLinha(line);
                            double.TryParse(line, out v[0].Coords.z); lidos++;
                        }
                        else if (line == "31")
                        {
                            line = (sr.ReadLine()).Trim(); line = trataLinha(line);
                            double.TryParse(line, out v[1].Coords.z); lidos++;
                        }
                        else if (line == "32")
                        {
                            line = (sr.ReadLine()).Trim(); line = trataLinha(line);
                            double.TryParse(line, out v[2].Coords.z); lidos++;
                        }
                        else if (line == "33")
                        {
                            line = (sr.ReadLine()).Trim(); line = trataLinha(line);
                            double.TryParse(line, out v[3].Coords.z); lidos++;
                        }
                        #endregion
                    }
                    return v;
                }

                /// <summary>Reads a single vertex (10 20 30)</summary>
                /// <param name="sr">Streamreader</param>
                private Vertex lerCoord(System.IO.StreamReader sr)
                {
                    int lidos = 0;
                    string line = "";
                    Vertex v = new Vertex();
                    v.Coords = new Vector(); v._indParte = new List<int>(); v._indNormais = new List<int>(); v._indAreas = new List<int>();

                    while (lidos < 3 && !sr.EndOfStream)
                    {
                        line = (sr.ReadLine()).Trim();
                        line = trataLinha(line);
                        if (line == "10")
                        {
                            line = (sr.ReadLine()).Trim(); line = trataLinha(line);
                            double.TryParse(line, out v.Coords.x); lidos++;
                        }
                        else if (line == "20")
                        {
                            line = (sr.ReadLine()).Trim(); line = trataLinha(line);
                            double.TryParse(line, out v.Coords.y); lidos++;
                        }
                        else if (line == "30")
                        {
                            line = (sr.ReadLine()).Trim(); line = trataLinha(line);
                            double.TryParse(line, out v.Coords.z); lidos++;
                        }
                    }

                    return v;
                }

                /// <summary>Attempts to load texture</summary>
                /// <param name="TexFile">Picture file containing texture</param>
                /// <param name="OBJFile">OBJ file to read path from.</param>
                private void GetTexture(string TexFile, string OBJFile)
                {
                    try
                    {
                        string[] texto = OBJFile.Split('\\');
                        string path = "";

                        //try to read texture from given file
                        TexFile = TexFile.Replace(",", ".");

                        for (int i = texto.Length - 2; i >= 0; i--)
                        {
                            path = texto[i] + "\\" + path;
                        }

                        using (System.IO.StreamReader sr = new System.IO.StreamReader(path + TexFile))
                        {
                            while (!sr.EndOfStream)
                            {

                                string line = (sr.ReadLine()).Trim();

                                while (line.EndsWith("\\")) //caracter \ indica que continua na outra linha
                                {
                                    line = line.Substring(0, line.Length - 1);
                                    line = line + (sr.ReadLine()).Trim();
                                }

                                line = trataLinha(line);

                                texto = line.Split();
                                if (texto[0].ToLower() == "map_kd")
                                {
                                    TextureBitmap = new System.Drawing.Bitmap(path + texto[1].Replace(",", "."));
                                }

                            }

                            sr.Close();
                        }
                    }
                    catch { }
                }


                #endregion

                #region Criacao por equacoes e calculos relacionados ao modelo

                /// <summary>Recreates 3D Model from parameterized 3D equations</summary>
                /// <param name="Name">Model name.</param>
                /// <param name="F">Coordinate function F. Has to return double[3] {x, y, z}
                /// OR double[6] {x,y,z, normalX, normalY, normalZ}</param>
                /// <param name="umin">Minimum value of u coordinate.</param>
                /// <param name="umax">Maximum value of u coordinate.</param>
                /// <param name="vmin">Minimum value of v coordinate.</param>
                /// <param name="vmax">Maximum value of v coordinate.</param>
                /// <param name="uPts">Number of points in u partition.</param>
                /// <param name="vPts">Number of points in v partition.</param>
                /// <param name="cor">Color vector. x=red, y=green, z=blue. Goes from 0 to 1.</param>
                public void Create3DModel(string Name, CoordFuncXYZ F,
                    float umin, float umax, float vmin, float vmax, int uPts, int vPts, Vector cor)
                {

                    float tempi = 1 / (float)(uPts - 1);
                    float tempj = 1 / (float)(vPts - 1);

                    //vertice e normal
                    float[,][] VertENormal = new float[uPts, vPts][];

                    //vertices
                    float[] uVals = new float[uPts];
                    float[] vVals = new float[vPts];
                    //Calcula uma única vez quais pontos devem ser transformados
                    for (int i = 0; i < uPts; i++)
                    {
                        uVals[i] = umin + (umax - umin) * (float)i * tempi;
                    }
                    for (int j = 0; j < vPts; j++)
                    {
                        vVals[j] = vmin + (vmax - vmin) * (float)j * tempj;
                    }

                    //Calcula os valores da função
                    for (int i = 0; i < uPts; i++)
                    {
                        //int i = get_global_id(0);
                        //int vPts = maxJ[0];
                        for (int j = 0; j < vPts; j++)
                        {
                            VertENormal[i, j] = F(uVals[i], vVals[j]);

                            //normaliza o vetor normal
                            if (VertENormal[i, j].Length == 6)
                            {
                                float temp = VertENormal[i, j][3] * VertENormal[i, j][3] +
                                    VertENormal[i, j][4] * VertENormal[i, j][4] +
                                    VertENormal[i, j][5] * VertENormal[i, j][5];

                                if (temp != 0 && temp != 1)
                                {
                                    temp = 1f / (float)Math.Sqrt(temp);
                                    VertENormal[i, j][3] *= temp; VertENormal[i, j][4] *= temp; VertENormal[i, j][5] *= temp;
                                }
                            }
                        }
                    }


                    //Texture coordinates
                    float[,][] TexCoords = null;
                    if (TextureBitmap != null)
                    {
                        TexCoords = new float[uPts, vPts][];
                        for (int i = 0; i < uPts; i++)
                        {
                            float uu = (float)i * tempi;
                            for (int j = 0; j < vPts; j++)
                            {
                                float vv = (float)j * tempj;
                                TexCoords[i, j] = new float[] { uu, vv };
                            }
                        }
                    }

                    Create3DModel(Name, VertENormal, TexCoords, null, cor, false);
                }

                /// <summary>Recreates 3D Model from calculated data</summary>
                /// <param name="Name">Model name.</param>
                /// <param name="VertENormal">Vertexes and normals to use to build model. float[,][3] - Only vertexes. float[6] - vertexes and normals</param>
                /// <param name="TexCoords">Texture coordinates. null - don't use. float[,][2] - coords</param>
                /// <param name="VertexColors">Vertex colors. null - don't use. float[,][4] - RGBA color</param>
                /// <param name="GlobalColor">Color vector. x=red, y=green, z=blue. Goes from 0 to 1.</param>
                /// <param name="LineModel">Is this a curve? (and not a surface)</param>
                public void Create3DModel(string Name, float[,][] VertENormal, float[,][] TexCoords, float[,][] VertexColors, Vector GlobalColor, bool LineModel)
                {
                    _partes = new List<Parte>();
                    _vertices = new List<Vertex>();
                    _vetNormais = new List<Vector>();
                    _vetTextureCoords = new List<float[]>();


                    Parte part = new Parte();

                    this.Name = Name;
                    part.Name = Name;

                    int uPts = VertENormal.GetLength(0);
                    int vPts = VertENormal.GetLength(1);

                    if (LineModel && vPts != 1) throw new Exception("Line model should have v dimension equal to 1");


                    float tempi = 1 / (float)(uPts - 1);
                    float tempj = 1 / (float)(vPts - 1);


                    //vertices
                    for (int i = 0; i < uPts; i++)
                    {
                        for (int j = 0; j < vPts; j++)
                        {
                            Vertex vert = new Vertex();
                            vert.Coords = new Vector(VertENormal[i, j][0], VertENormal[i, j][1], VertENormal[i, j][2]);

                            vert._indNormais = new List<int>();
                            vert._indParte = new List<int>();
                            vert._indAreas = new List<int>();

                            vert._indParte.Add(0);

                            //Colors
                            if (VertexColors != null)
                            {
                                vert.VertexColor = new float[4] { VertexColors[i, j][0], VertexColors[i, j][1], VertexColors[i, j][2], VertexColors[i, j][3] };
                            }
                            _vertices.Add(vert);

                            //Normals
                            if (VertENormal[i, j].Length == 6)
                            {
                                Vector normal = new Vector(VertENormal[i, j][3], VertENormal[i, j][4], VertENormal[i, j][5]);
                                _vetNormais.Add(normal);
                            }

                            //Texture coordinates
                            if (TexCoords != null)
                            {
                                _vetTextureCoords.Add(new float[] { TexCoords[i, j][0], TexCoords[i, j][1] });
                            }


                        }
                    }

                    //areas
                    for (int i = 0; i < uPts - 1; i++)
                    {
                        if (LineModel) //Model is a line
                        {
                            area a = new area();
                            a.IndVertices = new List<int>();
                            a.IndNormais = new List<int>();

                            a.IndVertices.Add(i);
                            a.IndVertices.Add(i + 1);

                            //Se a funcao calculou normais
                            if (VertENormal[i, 0].Length == 6)
                            {
                                a.IndNormais.Add(i);
                                a.IndNormais.Add(i + 1);
                            }
                            else
                            {
                                Vector n1 = new Vector(_vertices[i].Coords);
                                Vector n2 = new Vector(_vertices[i + 1].Coords);
                                n2 -= n1;
                                _vetNormais.Add(n2);
                                if (n2.x == 0 && n2.y == 0 && n2.z == 0) n2.z = 1;
                                a.IndNormais.Add(_vetNormais.Count - 1);
                                a.IndNormais.Add(_vetNormais.Count - 1);
                            }

                            part.Areas.Add(a);

                            _vertices[i]._indAreas.Add(part.Areas.Count - 1);
                            _vertices[i + 1]._indAreas.Add(part.Areas.Count - 1);

                        }

                        else //Model is a surface
                        {
                            for (int j = 0; j < vPts - 1; j++)
                            {
                                area a = new area();
                                a.IndVertices = new List<int>();
                                a.IndNormais = new List<int>();

                                a.IndVertices.Add(i * vPts + j);
                                a.IndVertices.Add((i + 1) * vPts + j);
                                a.IndVertices.Add((i + 1) * vPts + j + 1);
                                a.IndVertices.Add(i * vPts + j + 1);

                                if (TextureBitmap != null)
                                {
                                    a.IndTexVertexes = new List<int>();
                                    a.IndTexVertexes.Add(i * vPts + j);
                                    a.IndTexVertexes.Add((i + 1) * vPts + j);
                                    a.IndTexVertexes.Add((i + 1) * vPts + j + 1);
                                    a.IndTexVertexes.Add(i * vPts + j + 1);
                                }

                                //Se a funcao calculou normais
                                if (VertENormal[i, j].Length == 6)
                                {
                                    a.IndNormais.Add(i * vPts + j);
                                    a.IndNormais.Add((i + 1) * vPts + j);
                                    a.IndNormais.Add((i + 1) * vPts + j + 1);
                                    a.IndNormais.Add(i * vPts + j + 1);
                                } //se nao
                                else this.calcNormais(a);

                                part.Areas.Add(a);

                                _vertices[i * vPts + j]._indAreas.Add(part.Areas.Count - 1);
                                _vertices[i * vPts + j + 1]._indAreas.Add(part.Areas.Count - 1);
                                _vertices[(i + 1) * vPts + j + 1]._indAreas.Add(part.Areas.Count - 1);
                                _vertices[(i + 1) * vPts + j]._indAreas.Add(part.Areas.Count - 1);
                            }
                        }
                    }

                    part.Cor = new Vector(GlobalColor);

                    _partes.Add(part);

                    //Nao suaviza se recebeu os vetores, so se calculou
                    if (VertENormal[0, 0].Length == 3) suavizaNormais();
                    calcCGBox();
                }

                /// <summary>Creates a 3D Line from parameterized equations</summary>
                /// <param name="Name">Line name</param>
                /// <param name="F">Coordinate function F. Has to return double[3] {x, y, z}
                /// OR double[6] {x,y,z, normalX, normalY, normalZ}. Notice that only u coordinate is passed (v=0).</param>
                /// <param name="umin">Minimum value of u coordinate.</param>
                /// <param name="umax">Maximum value of u coordinate.</param>
                /// <param name="uPts">Number of points in u partition.</param>
                /// <param name="cor">Color vector. x=red, y=green, z=blue. Goes from 0 to 1.</param>
                public void Create3DLine(string Name, CoordFuncXYZ F, float umin, float umax, int uPts, Vector cor)
                {
                    _partes = new List<Parte>();
                    _vertices = new List<Vertex>();
                    _vetNormais = new List<Vector>();
                    _vetTextureCoords = new List<float[]>();

                    Parte part = new Parte();
                    this.ModelRenderStyle = Lab3DModelHolder.CLEnum.CLRenderStyle.Wireframe;

                    this.Name = Name;
                    part.Name = Name;

                    float tempi = 1 / (float)(uPts - 1);

                    //vertice e normal
                    float[][] VertENormal = new float[uPts][];

                    //vertices
                    float[] uVals = new float[uPts];
                    //Calcula uma única vez quais pontos devem ser transformados
                    for (int i = 0; i < uPts; i++)
                    {
                        uVals[i] = umin + (umax - umin) * (float)i * tempi;
                    }

                    //Calcula os valores da função
                    for (int i = 0; i < uPts; i++)
                    {
                        //int i = get_global_id(0);
                        //int vPts = maxJ[0];
                        VertENormal[i] = F(uVals[i], 0);

                        //normaliza o vetor normal
                        if (VertENormal[i].Length == 6)
                        {
                            float temp = VertENormal[i][3] * VertENormal[i][3] +
                                VertENormal[i][4] * VertENormal[i][4] +
                                VertENormal[i][5] * VertENormal[i][5];

                            if (temp != 0 && temp != 1)
                            {
                                temp = 1f / (float)Math.Sqrt(temp);
                                VertENormal[i][3] *= temp; VertENormal[i][4] *= temp; VertENormal[i][5] *= temp;
                            }
                        }
                    }

                    //Prossegue com a criação
                    Create3DLine(Name, VertENormal, cor);
                }

                /// <summary>Creates a 3D Line from parameterized equations</summary>
                /// <param name="Name">Line name</param>
                /// <param name="VertENormal">Vertex and normal coordinates of curve</param>
                /// <param name="cor">Color vector. x=red, y=green, z=blue. Goes from 0 to 1.</param>
                public void Create3DLine(string Name, float[][] VertENormal, Vector cor)
                {
                    _partes = new List<Parte>();
                    _vertices = new List<Vertex>();
                    _vetNormais = new List<Vector>();
                    _vetTextureCoords = new List<float[]>();

                    Parte part = new Parte();
                    this.ModelRenderStyle = Lab3DModelHolder.CLEnum.CLRenderStyle.Wireframe;

                    this.Name = Name;
                    part.Name = Name;

                    int uPts = VertENormal.Length;

                    //vertices
                    for (int i = 0; i < uPts; i++)
                    {
                        Vertex vert = new Vertex();
                        vert.Coords = new Vector(VertENormal[i][0], VertENormal[i][1], VertENormal[i][2]);

                        vert._indNormais = new List<int>();
                        vert._indParte = new List<int>();
                        vert._indAreas = new List<int>();

                        vert._indParte.Add(0);

                        _vertices.Add(vert);

                        Vector normal;
                        if (VertENormal[i].Length == 6)
                        {
                            normal = new Vector(VertENormal[i][3], VertENormal[i][4], VertENormal[i][5]);
                        }
                        else
                        {
                            normal = new Vector(0, 0, 1);
                            if (i > 0)
                            {
                                Vector v1 = new Vector(VertENormal[i - 1][0], VertENormal[i - 1][1], VertENormal[i - 1][2]);
                                Vector v2 = new Vector(VertENormal[i][0], VertENormal[i][1], VertENormal[i][2]);
                                v2 -= v1;
                                normal.x = v2.x; normal.y = v2.y; normal.z = v2.z;
                            }
                        }

                        _vetNormais.Add(normal);
                    }

                    //areas
                    for (int i = 0; i < uPts - 1; i++)
                    {

                        area a = new area();
                        a.IndVertices = new List<int>();
                        a.IndNormais = new List<int>();

                        a.IndVertices.Add(i);
                        a.IndVertices.Add(i + 1);


                        a.IndNormais.Add(i);
                        a.IndNormais.Add(i + 1);


                        part.Areas.Add(a);

                        _vertices[i]._indAreas.Add(part.Areas.Count - 1);
                        _vertices[i + 1]._indAreas.Add(part.Areas.Count - 1);

                    }

                    part.Cor = new Vector(cor);

                    _partes.Add(part);

                    calcCGBox();
                }

                /// <summary>Calculates normal vectors of a given area.</summary>
                /// <param name="a">Area to calculate normal vectors</param>
                /// <remarks>It is not possible to know for sure the area orientation. 
                /// It would be better to give the normal vector.</remarks>
                private void calcNormais(area a)
                {
                    Vector vA = new Vector(_vertices[a.IndVertices[0]].Coords);
                    Vector vB = new Vector(_vertices[a.IndVertices[1]].Coords);
                    Vector vC = new Vector(_vertices[a.IndVertices[2]].Coords);

                    Vector vAB = new Vector(vB - vA);
                    Vector vAC = new Vector(vC - vA);

                    vAB = Vector.CrossProduct(vAB, vAC);
                    vAB.normalize();

                    foreach (int ind in a.IndVertices)
                    {
                        //adiciona normal
                        _vetNormais.Add(vAB);
                        //registra
                        int yy = _vetNormais.Count - 1;
                        a.IndNormais.Add(yy);

                        int kk = _vetNormais.Count - 1;
                        _vertices[ind]._indNormais.Add(kk);
                    }
                }

                /// <summary>Calculates vertexes average (center of gravity) and lowest Z coordinate.</summary>
                public void calcCGBox()
                {
                    lock (this)
                    {
                        //Calculo do CG dos vertices
                        foreach (Vertex vv in _vertices)
                        {
                            CG += vv.Coords;
                        }
                        CG /= (double)_vertices.Count;

                        #region "Calculo do box"
                        double maxX, maxY, maxZ;
                        double minX, minY, minZ;

                        maxX = _vertices[0].Coords.x;
                        maxY = _vertices[0].Coords.y;
                        maxZ = _vertices[0].Coords.z;
                        minX = _vertices[0].Coords.x;
                        minY = _vertices[0].Coords.y;
                        minZ = _vertices[0].Coords.z;

                        foreach (Vertex vv in _vertices)
                        {
                            if (vv.Coords.x > maxX) maxX = vv.Coords.x;
                            if (vv.Coords.y > maxY) maxY = vv.Coords.y;
                            if (vv.Coords.z > maxZ) maxZ = vv.Coords.z;

                            if (vv.Coords.x < minX) minX = vv.Coords.x;
                            if (vv.Coords.y < minY) minY = vv.Coords.y;
                            if (vv.Coords.z < minZ) minZ = vv.Coords.z;
                        }
                        Dimens.x = maxX - minX;
                        Dimens.y = maxY - minY;
                        Dimens.z = maxZ - minZ;
                        minPoint.x = minX;
                        minPoint.y = minY;
                        minPoint.z = minZ;

                        #endregion
                    }
                }

                /// <summary>Smooths normal vectors in order to get per-pixel normals
                /// and allow OpenGL to use Gouraud shading.</summary>
                private void suavizaNormais()
                {
                    //suavizacao de normais
                    foreach (Vertex vv in _vertices)
                    {
                        Vector normalMedia = new Vector(0, 0, 0);
                        foreach (int ind in vv._indNormais)
                        {
                            normalMedia += _vetNormais[ind];
                        }
                        normalMedia *= 1 / (double)vv._indNormais.Count;
                        normalMedia.normalize();

                        foreach (int ind in vv._indNormais)
                        {
                            _vetNormais[ind] = new Vector(normalMedia);
                        }
                    }
                }





                #endregion

                #region Desenho e posicao do Objeto, função para aplicar outra textura

                /// <summary>Force element redraw?</summary>
                public bool ForceRedraw = false;
                /// <summary>Show this model?</summary>
                public bool ShowModel = true;

                /// <summary>Object translation vector from origin.</summary>
                public Vector vetTransl = new Vector(0, 0, 0);
                /// <summary>Object rotation vector in Euler angles (psi-theta-phi).</summary>
                public Vector vetRot = new Vector(0, 0, 0); //em angulos de Euler psi theta phi

                /// <summary>Radian to degree conversion</summary>
                private static float rad2deg = (float)180 / (float)Math.PI;

                /// <summary>Renders this 3D Model. Returns next OpenGL list number to use
                /// if it was necessary to generate new lists.</summary>
                /// <param name="DesenharTransparentes">True to draw only transparent object. This is used to draw solid objects last.</param>
                /// <returns>Returns next OpenGL list number to use
                /// if it was necessary to generate new lists.</returns>
                public void Render(bool DesenharTransparentes)
                {
                    if (this.ShowModel)
                    {
                        GL.PushMatrix();
                        GL.Translate((float)vetTransl.x, (float)vetTransl.y, (float)vetTransl.z);

                        GL.Rotate((float)vetRot.z * rad2deg, 0.0f, 0.0f, 1.0f);
                        GL.Rotate((float)vetRot.y * rad2deg, 0.0f, 1.0f, 0.0f);
                        GL.Rotate((float)vetRot.x * rad2deg, 1.0f, 0.0f, 0.0f);

                        desenhaPartes(DesenharTransparentes); //desenha partes transparentes?

                        GL.PopMatrix();
                    }
                }

                /// <summary>Renders the parts of this 3D model.</summary>
                /// <param name="DesenharTransparentes">True to draw only transparent object. This is used to draw solid objects last.</param>
                public void desenhaPartes(bool DesenharTransparentes)
                {
                    //AplicaTextura();

                    foreach (Parte parte in _partes)
                    {
                        object[] o;
                        geraPoligonosParte(parte, out o);
                    }
                }

                /// <summary>Generates polygons for a given 3D Model part.</summary>
                /// <param name="p">Part to generate models to.</param>
                /// <param name="o">Object to hold information content o = new object[] { VertCoordsData, ColorData, TexCoordsData, ElementData, NormalsData }</param>
                public void geraPoligonosParte(Parte p, out object[] o)
                {
                    o = null;
                    try
                    #region Draw using buffer objects
                    {
                        //if (!HardwareSupportsBufferObjects) throw new Exception("Buffer objects not supported");

                        //if (p.GLBuffers == null)
                        //{
                        //    p.GLBuffers = new int[5];
                        //    GL.GenBuffers(5, p.GLBuffers);
                        //}

                        //Creates population
                        List<float> VertCoordsData = new List<float>();
                        List<float> ColorData = new List<float>();
                        List<float> TexCoordsData = new List<float>();
                        List<int> ElementData = new List<int>();
                        List<float> NormalsData = new List<float>();
                        o = new object[] { VertCoordsData, ColorData, TexCoordsData, ElementData, NormalsData };

                        //The bad thing here is that the areas contain triangle fans
                        //but from one area to the other you can't pass a triangle fan
                        //For lines, it's ok, it's a line strip

                        if (this.ModelRenderStyle == Lab3DModelHolder.CLEnum.CLRenderStyle.Wireframe)
                        {
                            #region Line strip case
                            foreach (area a in p.Areas)
                            {
                                for (int i = 1; i < a.IndVertices.Count; i++)
                                {
                                    try
                                    {
                                        if (TextureBitmap != null && a.IndTexVertexes.Count > 0)
                                        {
                                            //Previous
                                            TexCoordsData.Add(_vetTextureCoords[a.IndTexVertexes[i - 1]][0]);
                                            TexCoordsData.Add(_vetTextureCoords[a.IndTexVertexes[i - 1]][1]);
                                            //Current
                                            TexCoordsData.Add(_vetTextureCoords[a.IndTexVertexes[i]][0]);
                                            TexCoordsData.Add(_vetTextureCoords[a.IndTexVertexes[i]][1]);
                                        }
                                    }
                                    catch
                                    { }

                                    //Previous
                                    NormalsData.Add((float)_vetNormais[a.IndNormais[i - 1]].x);
                                    NormalsData.Add((float)_vetNormais[a.IndNormais[i - 1]].y);
                                    NormalsData.Add((float)_vetNormais[a.IndNormais[i - 1]].z);
                                    //Current
                                    NormalsData.Add((float)_vetNormais[a.IndNormais[i]].x);
                                    NormalsData.Add((float)_vetNormais[a.IndNormais[i]].y);
                                    NormalsData.Add((float)_vetNormais[a.IndNormais[i]].z);

                                    if (_vertices[i].VertexColor != null)
                                    {
                                        //Previous
                                        ColorData.Add(_vertices[a.IndVertices[i - 1]].VertexColor[0]);
                                        ColorData.Add(_vertices[a.IndVertices[i - 1]].VertexColor[1]);
                                        ColorData.Add(_vertices[a.IndVertices[i - 1]].VertexColor[2]);
                                        ColorData.Add(_vertices[a.IndVertices[i - 1]].VertexColor[3]);
                                        //Current
                                        ColorData.Add(_vertices[a.IndVertices[i]].VertexColor[0]);
                                        ColorData.Add(_vertices[a.IndVertices[i]].VertexColor[1]);
                                        ColorData.Add(_vertices[a.IndVertices[i]].VertexColor[2]);
                                        ColorData.Add(_vertices[a.IndVertices[i]].VertexColor[3]);
                                    }
                                    else
                                    {
                                        for (int k = 0; k < 2; k++)
                                        {
                                            ColorData.Add((float)p.Cor.x);
                                            ColorData.Add((float)p.Cor.y);
                                            ColorData.Add((float)p.Cor.z);
                                            ColorData.Add(1.0f - p.Transparencia);
                                        }
                                    }
                                    //Previous
                                    VertCoordsData.Add((float)_vertices[a.IndVertices[i - 1]].Coords.x);
                                    VertCoordsData.Add((float)_vertices[a.IndVertices[i - 1]].Coords.y);
                                    VertCoordsData.Add((float)_vertices[a.IndVertices[i - 1]].Coords.z);
                                    //Current
                                    VertCoordsData.Add((float)_vertices[a.IndVertices[i]].Coords.x);
                                    VertCoordsData.Add((float)_vertices[a.IndVertices[i]].Coords.y);
                                    VertCoordsData.Add((float)_vertices[a.IndVertices[i]].Coords.z);

                                    ElementData.Add(VertCoordsData.Count - 2);
                                    ElementData.Add(VertCoordsData.Count - 1);
                                }
                            }
                            #endregion
                        }
                        else if (this.ModelRenderStyle == Lab3DModelHolder.CLEnum.CLRenderStyle.Solid)
                        {
                            #region Triangle fan case
                            foreach (area a in p.Areas)
                            {
                                for (int i = 2; i < a.IndVertices.Count; i++)
                                {
                                    try
                                    {
                                        if (TextureBitmap != null && a.IndTexVertexes.Count > 0)
                                        {
                                            //First element
                                            TexCoordsData.Add(_vetTextureCoords[a.IndTexVertexes[0]][0]);
                                            TexCoordsData.Add(_vetTextureCoords[a.IndTexVertexes[0]][1]);

                                            //Previous element
                                            TexCoordsData.Add(_vetTextureCoords[a.IndTexVertexes[i - 1]][0]);
                                            TexCoordsData.Add(_vetTextureCoords[a.IndTexVertexes[i - 1]][1]);

                                            //Current element
                                            TexCoordsData.Add(_vetTextureCoords[a.IndTexVertexes[i]][0]);
                                            TexCoordsData.Add(_vetTextureCoords[a.IndTexVertexes[i]][1]);
                                        }
                                    }
                                    catch
                                    { }

                                    //First
                                    NormalsData.Add((float)_vetNormais[a.IndNormais[0]].x);
                                    NormalsData.Add((float)_vetNormais[a.IndNormais[0]].y);
                                    NormalsData.Add((float)_vetNormais[a.IndNormais[0]].z);
                                    //Previous
                                    NormalsData.Add((float)_vetNormais[a.IndNormais[i - 1]].x);
                                    NormalsData.Add((float)_vetNormais[a.IndNormais[i - 1]].y);
                                    NormalsData.Add((float)_vetNormais[a.IndNormais[i - 1]].z);
                                    //Current
                                    NormalsData.Add((float)_vetNormais[a.IndNormais[i]].x);
                                    NormalsData.Add((float)_vetNormais[a.IndNormais[i]].y);
                                    NormalsData.Add((float)_vetNormais[a.IndNormais[i]].z);

                                    if (_vertices[i].VertexColor != null)
                                    {
                                        //First
                                        ColorData.Add(_vertices[a.IndVertices[0]].VertexColor[0]);
                                        ColorData.Add(_vertices[a.IndVertices[0]].VertexColor[1]);
                                        ColorData.Add(_vertices[a.IndVertices[0]].VertexColor[2]);
                                        ColorData.Add(_vertices[a.IndVertices[0]].VertexColor[3]);
                                        //Previous
                                        ColorData.Add(_vertices[a.IndVertices[i - 1]].VertexColor[0]);
                                        ColorData.Add(_vertices[a.IndVertices[i - 1]].VertexColor[1]);
                                        ColorData.Add(_vertices[a.IndVertices[i - 1]].VertexColor[2]);
                                        ColorData.Add(_vertices[a.IndVertices[i - 1]].VertexColor[3]);
                                        //Current
                                        ColorData.Add(_vertices[a.IndVertices[i]].VertexColor[0]);
                                        ColorData.Add(_vertices[a.IndVertices[i]].VertexColor[1]);
                                        ColorData.Add(_vertices[a.IndVertices[i]].VertexColor[2]);
                                        ColorData.Add(_vertices[a.IndVertices[i]].VertexColor[3]);
                                    }
                                    else
                                    {
                                        for (int k = 0; k < 3; k++)
                                        {
                                            ColorData.Add((float)p.Cor.x);
                                            ColorData.Add((float)p.Cor.y);
                                            ColorData.Add((float)p.Cor.z);
                                            ColorData.Add(1.0f - p.Transparencia);
                                        }
                                    }

                                    //First
                                    VertCoordsData.Add((float)_vertices[a.IndVertices[0]].Coords.x);
                                    VertCoordsData.Add((float)_vertices[a.IndVertices[0]].Coords.y);
                                    VertCoordsData.Add((float)_vertices[a.IndVertices[0]].Coords.z);
                                    //Previous
                                    VertCoordsData.Add((float)_vertices[a.IndVertices[i - 1]].Coords.x);
                                    VertCoordsData.Add((float)_vertices[a.IndVertices[i - 1]].Coords.y);
                                    VertCoordsData.Add((float)_vertices[a.IndVertices[i - 1]].Coords.z);
                                    //Current
                                    VertCoordsData.Add((float)_vertices[a.IndVertices[i]].Coords.x);
                                    VertCoordsData.Add((float)_vertices[a.IndVertices[i]].Coords.y);
                                    VertCoordsData.Add((float)_vertices[a.IndVertices[i]].Coords.z);

                                    //Adds all 3 vertexes
                                    for (int k = 3; k >= 1; k--)
                                        ElementData.Add(VertCoordsData.Count - k);
                                }
                            }
                            #endregion
                        }
                        else if (this.ModelRenderStyle == Lab3DModelHolder.CLEnum.CLRenderStyle.Point)
                        {
                            #region Point case
                            foreach (area a in p.Areas)
                            {
                                for (int i = 0; i < a.IndVertices.Count; i++)
                                {
                                    try
                                    {
                                        if (TextureBitmap != null && a.IndTexVertexes.Count > 0)
                                        {
                                            //Current
                                            TexCoordsData.Add(_vetTextureCoords[a.IndTexVertexes[i]][0]);
                                            TexCoordsData.Add(_vetTextureCoords[a.IndTexVertexes[i]][1]);
                                        }
                                    }
                                    catch
                                    { }

                                    //Current
                                    NormalsData.Add((float)_vetNormais[a.IndNormais[i]].x);
                                    NormalsData.Add((float)_vetNormais[a.IndNormais[i]].y);
                                    NormalsData.Add((float)_vetNormais[a.IndNormais[i]].z);

                                    if (_vertices[i].VertexColor != null)
                                    {

                                        //Current
                                        ColorData.Add(_vertices[a.IndVertices[i]].VertexColor[0]);
                                        ColorData.Add(_vertices[a.IndVertices[i]].VertexColor[1]);
                                        ColorData.Add(_vertices[a.IndVertices[i]].VertexColor[2]);
                                        ColorData.Add(_vertices[a.IndVertices[i]].VertexColor[3]);
                                    }
                                    else
                                    {
                                        ColorData.Add((float)p.Cor.x);
                                        ColorData.Add((float)p.Cor.y);
                                        ColorData.Add((float)p.Cor.z);
                                        ColorData.Add(1.0f - p.Transparencia);
                                    }

                                    //Current
                                    VertCoordsData.Add((float)_vertices[a.IndVertices[i]].Coords.x);
                                    VertCoordsData.Add((float)_vertices[a.IndVertices[i]].Coords.y);
                                    VertCoordsData.Add((float)_vertices[a.IndVertices[i]].Coords.z);

                                    ElementData.Add(VertCoordsData.Count - 1);
                                }
                            }
                            #endregion
                        }

                        p.GLNumElements = ElementData.Count;

                        //BufferUsageHint Hint;
                        //if (this.modelMovement == CLEnum.CLModelMovement.Dynamic) Hint = BufferUsageHint.StreamDraw;
                        //else Hint = BufferUsageHint.StaticDraw;

                        ////Stores data in the buffers
                        //GL.BindBuffer(BufferTarget.ArrayBuffer, p.GLBuffers[0]);
                        //GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(ColorData.Count * sizeof(float)), ColorData.ToArray(), Hint);

                        //GL.BindBuffer(BufferTarget.ArrayBuffer, p.GLBuffers[1]);
                        //GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(VertCoordsData.Count * sizeof(float)), VertCoordsData.ToArray(), Hint);

                        //GL.BindBuffer(BufferTarget.ArrayBuffer, p.GLBuffers[2]);
                        //GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(NormalsData.Count * sizeof(float)), NormalsData.ToArray(), Hint);

                        //if (TextureBitmap != null)
                        //{
                        //    GL.BindBuffer(BufferTarget.ArrayBuffer, p.GLBuffers[3]);
                        //    GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(TexCoordsData.Count * sizeof(float)), TexCoordsData.ToArray(), Hint);
                        //}

                        //GL.BindBuffer(BufferTarget.ElementArrayBuffer, p.GLBuffers[4]);
                        //GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(ElementData.Count * sizeof(int)), ElementData.ToArray(), Hint);

                    }
                    #endregion
                    catch
                    #region OpenGL Display Lists, for hardwares that don't support Buffer Objects
                    {
                        Lab3DModelHolder.HardwareSupportsBufferObjects = false;

                        GL.NewList(p.GLListNumber, ListMode.Compile);

                        foreach (area a in p.Areas)
                        {
                            if (this.ModelRenderStyle == Lab3DModelHolder.CLEnum.CLRenderStyle.Solid)
                            {
                                GL.Begin(BeginMode.TriangleFan);
                                for (int i = 0; i < a.IndVertices.Count; i++)
                                {
                                    try
                                    {
                                        if (TextureBitmap != null && a.IndTexVertexes.Count > 0)
                                        {
                                            GL.TexCoord2(_vetTextureCoords[a.IndTexVertexes[i]][0], _vetTextureCoords[a.IndTexVertexes[i]][1]);
                                        }
                                    }
                                    catch
                                    { }

                                    GL.Normal3((float)_vetNormais[a.IndNormais[i]].x, (float)_vetNormais[a.IndNormais[i]].y, (float)_vetNormais[a.IndNormais[i]].z);

                                    if (_vertices[i].VertexColor != null)
                                    {
                                        GL.Color4(_vertices[a.IndVertices[i]].VertexColor[0], _vertices[a.IndVertices[i]].VertexColor[1],
                                            _vertices[a.IndVertices[i]].VertexColor[2], _vertices[a.IndVertices[i]].VertexColor[3]);
                                    }

                                    GL.Vertex3((float)_vertices[a.IndVertices[i]].Coords.x, (float)_vertices[a.IndVertices[i]].Coords.y, (float)_vertices[a.IndVertices[i]].Coords.z);
                                }
                                GL.End();
                            }
                            else if (this.ModelRenderStyle == Lab3DModelHolder.CLEnum.CLRenderStyle.Wireframe)
                            {
                                GL.Begin(BeginMode.LineStrip);
                                {
                                    for (int i = 0; i < a.IndVertices.Count; i++)
                                    {
                                        try
                                        {
                                            if (TextureBitmap != null && a.IndTexVertexes.Count > 0)
                                            {
                                                GL.TexCoord2(_vetTextureCoords[a.IndTexVertexes[i]][0], _vetTextureCoords[a.IndTexVertexes[i]][1]);
                                            }
                                        }
                                        catch
                                        { }

                                        if (_vertices[i].VertexColor != null)
                                        {
                                            GL.Color4(_vertices[a.IndVertices[i]].VertexColor[0], _vertices[a.IndVertices[i]].VertexColor[1],
                                                _vertices[a.IndVertices[i]].VertexColor[2], _vertices[a.IndVertices[i]].VertexColor[3]);
                                        }

                                        GL.Normal3((float)_vetNormais[a.IndNormais[i]].x, (float)_vetNormais[a.IndNormais[i]].y, (float)_vetNormais[a.IndNormais[i]].z);
                                        GL.Vertex3((float)_vertices[a.IndVertices[i]].Coords.x, (float)_vertices[a.IndVertices[i]].Coords.y, (float)_vertices[a.IndVertices[i]].Coords.z);
                                    }
                                }
                                GL.End();
                            }

                            else if (this.ModelRenderStyle == Lab3DModelHolder.CLEnum.CLRenderStyle.Point)
                            {
                                GL.Begin(BeginMode.Points);
                                {
                                    for (int i = 0; i < a.IndVertices.Count; i++)
                                    {
                                        try
                                        {
                                            if (TextureBitmap != null && a.IndTexVertexes.Count > 0)
                                            {
                                                GL.TexCoord2(_vetTextureCoords[a.IndTexVertexes[i]][0], _vetTextureCoords[a.IndTexVertexes[i]][1]);
                                            }
                                        }
                                        catch
                                        { }

                                        if (_vertices[i].VertexColor != null)
                                        {
                                            GL.Color4(_vertices[a.IndVertices[i]].VertexColor[0], _vertices[a.IndVertices[i]].VertexColor[1],
                                                _vertices[a.IndVertices[i]].VertexColor[2], _vertices[a.IndVertices[i]].VertexColor[3]);
                                        }

                                        GL.Normal3((float)_vetNormais[a.IndNormais[i]].x, (float)_vetNormais[a.IndNormais[i]].y, (float)_vetNormais[a.IndNormais[i]].z);
                                        GL.Vertex3((float)_vertices[a.IndVertices[i]].Coords.x, (float)_vertices[a.IndVertices[i]].Coords.y, (float)_vertices[a.IndVertices[i]].Coords.z);
                                    }
                                }
                                GL.End();
                            }
                        }
                        GL.EndList();
                    }
                    #endregion
                }


                #region Texture
                /// <summary>Changes the texture of this object</summary>
                /// <param name="Bmp">New bitmap to apply</param>
                public void ApplyTexture(Bitmap Bmp)
                {
                    GL.DeleteTexture(this.GLTexture);
                    this.GLTexture = 0;
                    TextureBitmap = Bmp;

                    //There is an invalid pointer to bitmap lying around
                    GC.Collect();
                }

                /// <summary>Applies texture if possible</summary>
                private void AplicaTextura()
                {
                    try
                    {
                        if (TextureBitmap != null)
                        {
                            if (this.GLTexture <= 0)
                            {
                                //texture, if there is one
                                System.Drawing.Bitmap image = new System.Drawing.Bitmap(TextureBitmap);


                                //Guarantee power of 2 images
                                image = ResizeToPowerOf2(image);

                                image.RotateFlip(System.Drawing.RotateFlipType.RotateNoneFlipY);
                                System.Drawing.Imaging.BitmapData bitmapdata;
                                System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, image.Width, image.Height);

                                bitmapdata = image.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly,
                                    System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                                GL.GenTextures(1, out this.GLTexture);
                                GL.BindTexture(TextureTarget.Texture2D, this.GLTexture);

                                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb8, image.Width, image.Height,
                                    0, (PixelFormat)(int)All.BgrExt, PixelType.UnsignedByte, bitmapdata.Scan0);

                                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);		// Linear Filtering
                                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);		// Linear Filtering

                                image.UnlockBits(bitmapdata);
                                image.Dispose();
                            }
                            else GL.BindTexture(TextureTarget.Texture2D, this.GLTexture);
                        }
                        else
                        {
                            GL.BindTexture(TextureTarget.Texture2D, 0);
                        }
                    }
                    catch
                    {
                        int i = 0;
                        i = i + 1;
                    }
                }

                /// <summary>Method for resizing an image</summary>
                /// <param name="img">Image to resize</param>
                private Bitmap ResizeToPowerOf2(Image img)
                {
                    //get the height and width of the image
                    int originalW = img.Width;
                    int originalH = img.Height;

                    //get the new size based on the percentage change
                    int resizedW = (int)Math.Pow(2, Math.Ceiling(Math.Log((double)originalW, 2)));
                    int resizedH = (int)Math.Pow(2, Math.Ceiling(Math.Log((double)originalH, 2)));

                    //create a new Bitmap the size of the new image
                    Bitmap bmp = new Bitmap(resizedW, resizedH);

                    //create a new graphic from the Bitmap
                    Graphics graphic = Graphics.FromImage((Image)bmp);
                    graphic.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                    //draw the newly resized image
                    graphic.DrawImage(img, 0, 0, resizedW, resizedH);

                    //dispose and free up the resources
                    graphic.Dispose();

                    //return the image
                    return bmp;
                }
                #endregion

                #region Model displacement and Selection
                /// <summary>Displaces 3D model in a certain way.</summary>
                /// <param name="modo">Displacement model. 0=X axis, 1=Y axis, 2=Z axis,
                /// 3=X rotation, 4=Y rotation, 5=Z rotation</param>
                /// <param name="desl">Displacement value. Distance or angle in radians.</param>
                public void Desloca(int modo, double desl)
                {
                    switch (modo)
                    {
                        case 0: //Transl X
                            vetTransl.x += desl;
                            break;
                        case 1: //Transl Y
                            vetTransl.y += desl;
                            break;
                        case 2: //Transl Z
                            vetTransl.z += desl;
                            break;
                        case 3: //Rot X
                            vetRot.x += desl;
                            break;
                        case 4: //Rot Y
                            vetRot.y += desl;
                            break;
                        case 5: //Rot Z
                            vetRot.z += desl;
                            break;

                    }
                }

                /// <summary>Draws desired part as selected.</summary>
                /// <param name="indParte">Part to select.</param>
                public void Seleciona(int indParte)
                {
                    _partes[indParte].Selecionar = true;
                    Render(_partes[indParte].Transparencia != 0.0f);
                }

                /// <summary>Stops drawing desired part as selected.</summary>
                /// <param name="indParte">Part to unselect.</param>
                public void DesSeleciona(int indParte)
                {
                    _partes[indParte].Selecionar = false;
                    Render(_partes[indParte].Transparencia != 0.0f);
                }
                #endregion

                #endregion

                #region Exemplos de Modelos 3D
                /// <summary>Example 3D Models generated from equations.</summary>
                public static class Example3DModels
                {
                    /// <summary>Generates a 3D Model for a cylinder.</summary>
                    /// <param name="Name">Model name.</param>
                    /// <param name="Radius">Cylinder radius.</param>
                    /// <param name="Height">Cylinder height.</param>
                    /// <param name="numPoints">Number of points for circular section.</param>
                    /// <param name="Color">Color vector.</param>
                    /// <param name="Texture">Texture bitmap. Null uses no texture</param>
                    public static Lab3DModelHolder Cylinder(string Name, float Radius, float Height, int numPoints, Vector Color, System.Drawing.Bitmap Texture)
                    {
                        rCyl = Radius;
                        return new Lab3DModelHolder(Name, CylFunction, 0, (float)(2 * Math.PI), 0, Height, numPoints, 2, Color, Texture);
                    }
                    private static float rCyl = 1;
                    private static float[] CylFunction(float u, float v)
                    {
                        float[] resp = new float[6];
                        //direcao e vetor normal
                        resp[3] = (float)Math.Cos(u);
                        resp[4] = (float)Math.Sin(u);
                        resp[5] = 0;
                        //ponto
                        resp[0] = rCyl * resp[3];
                        resp[1] = rCyl * resp[4];
                        resp[2] = v;
                        return resp;
                    }


                    /// <summary>Generates a 3D Model for a cone.</summary>
                    /// <param name="Name">Model name.</param>
                    /// <param name="Radius">Cone outer radius.</param>
                    /// <param name="Height">Cone height.</param>
                    /// <param name="numPoints">Number of points for circular section.</param>
                    /// <param name="Color">Color vector.</param>
                    /// <param name="Texture">Texture bitmap. Null uses no texture</param>
                    public static Lab3DModelHolder Cone(string Name, float Radius, float Height, int numPoints, Vector Color, System.Drawing.Bitmap Texture)
                    {
                        rCon = Radius / Height;
                        return new Lab3DModelHolder(Name, ConeFunc, 0, (float)(2 * Math.PI), 0, -Height, numPoints, 2, Color, Texture);
                    }
                    private static float rCon = 1;
                    private static float[] ConeFunc(float u, float v)
                    {
                        float[] resp = new float[6];
                        //ponto
                        resp[0] = rCon * (float)(Math.Sin(u)) * v;
                        resp[1] = rCon * (float)(Math.Cos(u)) * v;
                        resp[2] = v;
                        //normal
                        resp[3] = v * (float)(Math.Sin(u));
                        resp[4] = v * (float)(Math.Cos(u));
                        resp[5] = -rCon * v;

                        float invnorma = (float)(1 / Math.Sqrt(resp[3] * resp[3] + resp[4] * resp[4] + resp[5] * resp[5]));
                        resp[3] *= invnorma;
                        resp[4] *= invnorma;
                        resp[5] *= invnorma;

                        return resp;
                    }

                    /// <summary>Generates a 3D Model for a sphere.</summary>
                    /// <param name="Name">Model name.</param>
                    /// <param name="numPoints">Number of points to use for each coordinate. At least 4.</param>
                    /// <param name="Radius">Sphere radius.</param>
                    /// <param name="Color">Color vector.</param>
                    /// <param name="Texture">Texture bitmap. Null uses no texture</param>
                    public static Lab3DModelHolder Sphere(string Name, float Radius, int numPoints, Vector Color, System.Drawing.Bitmap Texture)
                    {
                        rSph = Radius;
                        if (numPoints < 4) numPoints = 4;
                        return new Lab3DModelHolder(Name, SphereFunction, 0, (float)(2 * Math.PI), -0.5f * (float)(Math.PI), (float)(0.5 * Math.PI), numPoints, numPoints, Color, Texture);
                    }
                    private static float rSph = 1;
                    private static float[] SphereFunction(float u, float v)
                    {
                        float[] resp = new float[6];
                        //direcao e vetor normal
                        resp[3] = (float)(Math.Cos(u) * Math.Cos(v));
                        resp[4] = (float)(Math.Sin(u) * Math.Cos(v));
                        resp[5] = (float)(Math.Sin(v));
                        //ponto
                        resp[0] = rSph * resp[3];
                        resp[1] = rSph * resp[4];
                        resp[2] = rSph * resp[5];
                        return resp;
                    }

                    /// <summary>Generates a 3D Model for a disk.</summary>
                    /// <param name="Name">Model name.</param>
                    /// <param name="numPoints">Number of points to use in circumference.</param>
                    /// <param name="InnerRadius">Inner disk radius.</param>
                    /// <param name="OuterRadius">Outer disk radius.</param>
                    /// <param name="Color">Color vector.</param>
                    /// <param name="Texture">Texture bitmap. Null uses no texture</param>
                    public static Lab3DModelHolder Disk(string Name, float InnerRadius, float OuterRadius, int numPoints, Vector Color, System.Drawing.Bitmap Texture)
                    {
                        if (numPoints < 4) numPoints = 4;
                        return new Lab3DModelHolder(Name, DiskFunction, 0, (float)(2 * Math.PI), InnerRadius, OuterRadius, numPoints, 2, Color, Texture);
                    }
                    private static float[] DiskFunction(float u, float v)
                    {
                        float[] resp = new float[6];
                        //direcao e vetor normal
                        resp[3] = 0;
                        resp[4] = 0;
                        resp[5] = 1;
                        //ponto
                        resp[0] = v * (float)(Math.Cos(u));
                        resp[1] = v * (float)(Math.Sin(u));
                        resp[2] = 0;
                        return resp;
                    }
                }

                #endregion



                #region Geração com OpenCL, equações e arquivos Lab3D

                /// <summary>Enumerations for Lab3D file format</summary>
                public static class CLEnum
                {
                    /// <summary>Model type</summary>
                    public enum CLModelType
                    {
                        /// <summary>Model contains a curve</summary>
                        Curve,
                        /// <summary>Model contains a surface</summary>
                        Surface
                    }

                    /// <summary>Model movement type</summary>
                    public enum CLModelMovement
                    {
                        /// <summary>Static model</summary>
                        Static,
                        /// <summary>Vertex coords of the model change with time</summary>
                        Dynamic
                    }

                    /// <summary>Lab3D model texture</summary>
                    public enum CLModelTextureType
                    {
                        /// <summary>No texture</summary>
                        None,
                        /// <summary>Texture from text</summary>
                        FromText,
                        /// <summary>Texture from image</summary>
                        FromImage
                    }

                    /// <summary>Model number of points in u and v direction</summary>
                    public enum CLModelQuality
                    {
                        /// <summary>Low 30x30 model</summary>
                        Low,
                        /// <summary>Medium 100x100 model</summary>
                        Medium,
                        /// <summary>High 200x200 model</summary>
                        High,
                        /// <summary>Very high 400x400 model</summary>
                        VeryHigh,
                        /// <summary>Custom resolution</summary>
                        Custom
                    }

                    /// <summary>Model render style</summary>
                    public enum CLRenderStyle
                    {
                        /// <summary>Renders this model Solid</summary>
                        Solid,
                        /// <summary>Renders this model Wireframe</summary>
                        Wireframe,
                        /// <summary>Renders this model as Point</summary>
                        Point
                    }
                }





                #region Model properties

                /// <summary>Model vertexex move with time?</summary>
                public CLEnum.CLModelMovement modelMovement = CLEnum.CLModelMovement.Static;

                /// <summary>Render style</summary>
                public CLEnum.CLRenderStyle ModelRenderStyle;
                #endregion




                #endregion
            }

            #endregion

        }


    }

    /// <summary>OpenGL 3D font creator</summary>
    public class GLFont
    {
        /// <summary>Stores 3D character models</summary>
        public GLRender.GLVBOModel[] GLchars;

        /// <summary>Reference width of letter O</summary>
        private float referenceWidth;

        /// <summary>Width of characters in OpenGL scale</summary>
        private float[] WidthInGLScale;

        #region 3D font constructor and loader from file.
        /// <summary>Creates a new 3D font from specified font</summary>
        /// <param name="f">Font prototype to use in this 3D font</param>
        /// <param name="GLNormalizationScale">Character reference width in OpenGL scale</param>
        public GLFont(Font f, float GLNormalizationScale)
        {
            byte[] b = new byte[1];
            Graphics g = Graphics.FromImage(new Bitmap(1, 1));
            string s;

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            System.Diagnostics.Stopwatch sw2 = new System.Diagnostics.Stopwatch();

            //Preliminary size
            SizeF sizePrelim = g.MeasureString("O", f);
            this.referenceWidth = sizePrelim.Width;
            int w = (int)(2.0f * sizePrelim.Width);
            int h = (int)(sizePrelim.Height);

            float[, ,] bmpVals = new float[w, h, 3];
            OpenCLTemplate.Isosurface.MarchingCubes mc = new OpenCLTemplate.Isosurface.MarchingCubes(bmpVals);

            //GLchars[i].Scale = size.Width * 0.4f;
            //Centers in Z
            float temp = GLNormalizationScale / referenceWidth;
            mc.Increments = new float[] { temp, temp, GLNormalizationScale * 0.1f };
            mc.InitValues = new float[] { 0.0f, 0.0f, -GLNormalizationScale * 0.1f };

            //Creates 3D models for each character
            GLchars = new GLRender.GLVBOModel[256];
            WidthInGLScale = new float[256];
            for (int i = 0; i < 256; i++)
            {
                sw.Start();
                b[0] = (byte)i;
                s = System.Text.ASCIIEncoding.Default.GetString(b);
                //Measures string size
                SizeF size = g.MeasureString(s, f);

                //Creates a bitmap to store the letter and draws it onto the bitmap
                Bitmap bmp = new Bitmap(1 + (int)size.Width, 1 + (int)size.Height);
                Graphics g2 = Graphics.FromImage(bmp);

                WidthInGLScale[i] = 0.7f * GLNormalizationScale * (float)size.Width / (float)sizePrelim.Width;

                g2.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                g2.FillRectangle(Brushes.Black, 0, 0, bmp.Width, bmp.Height);
                g2.DrawString(s, f, Brushes.White, 0, 0);

                //Creates a float array to store bitmap values (input to isoSurface generator)

                for (int x = 0; x < w; x++)
                {
                    for (int y = 0; y < h; y++)
                    {
                        bmpVals[x, y, 0] = 0;
                        bmpVals[x, y, 1] = 0;
                        bmpVals[x, y, 2] = 0;
                    }
                }

                bool intensitiesAdded = false;

                #region Reads bitmap to float values array
                System.Drawing.Imaging.BitmapData bmdbmp = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                           System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                unsafe
                {
                    for (int y = 0; y < bmdbmp.Height; y++)
                    {
                        byte* row = (byte*)bmdbmp.Scan0 + (y * bmdbmp.Stride);

                        for (int x = 0; x < bmdbmp.Width; x++)
                        {
                            if (row[x << 2] != 0)
                            {
                                bmpVals[x, h - y - 1, 1] = row[(x << 2)];
                                intensitiesAdded = true;
                            }
                        }
                    }
                }


                #endregion

                bmp.UnlockBits(bmdbmp);

                //p.Image = bmp;
                bmp.Dispose();

                //If there's data to create a 3D model do so
                if (intensitiesAdded)
                {

                    GLchars[i] = new GLRender.GLVBOModel(BeginMode.Triangles);
                    mc.SetFuncVals(bmpVals);

                    sw2.Start();
                    mc.CalcIsoSurface(027.0f);
                    List<float> Vertex, Normals; List<int> Elems;
                    mc.GetEdgeInfo(out Vertex, out Normals, out Elems);
                    sw2.Stop();

                    //float[] Colors = new float[(Vertex.Count / 3) << 2];
                    //for (int k = 0; k < Colors.Length; k++) Colors[k] = 1.0f;

                    //GLchars[i].vetRot.z = Math.PI / 2;
                    //GLchars[i].vetRot.x = -Math.PI / 2;
                    //GLchars[i].Scale = 100.0f;

                    GLchars[i].SetNormalData(Normals.ToArray());
                    GLchars[i].SetVertexData(Vertex.ToArray());
                    GLchars[i].SetElemData(Elems.ToArray());
                    //GLchars[i].SetColorData(Colors);
                    sw.Stop();
                }
                else WidthInGLScale[i] = 0.6f * GLNormalizationScale;

                sw.Stop();
            }

        }

        /// <summary>Loads a 3D font from a file. Does NOT require OpenCL/GL interop</summary>
        /// <param name="filename">3D Font file</param>
        public GLFont(string filename)
        {
            //Reads file information
            byte[] Data;
            using (System.IO.BinaryReader b = new System.IO.BinaryReader(System.IO.File.Open(filename, System.IO.FileMode.Open)))
            {
                long length = b.BaseStream.Length;

                Data = new byte[length];

                long pos = 0;
                int bytesToRead = 1000;
                while (pos < length)
                {
                    byte[] bt = b.ReadBytes(bytesToRead);

                    for (int i = 0; i < bt.Length; i++) Data[pos + i] = bt[i];

                    pos += bytesToRead;
                    if (length - pos < bytesToRead) bytesToRead = (int)(length - pos);

                }

                b.Close();
            }

            //Creates 3D models for each character
            GLchars = new GLRender.GLVBOModel[256];
            WidthInGLScale = new float[256];

            //Last stored char. So far, it has to be 255
            byte nChars = Data[0];
            int DataPos = 1;
            for (int indChar = 0; indChar <= nChars; indChar++)
            {
                //Current char
                byte CurChar = Data[DataPos]; DataPos++;

                //Width of this char
                WidthInGLScale[indChar] = BitConverter.ToSingle(Data, DataPos); DataPos += 4;

                //number of vertexes/normals
                int nVert = BitConverter.ToInt32(Data, DataPos); DataPos += 4;

                //number of elements
                int nElem = BitConverter.ToInt32(Data, DataPos); DataPos += 4;

                //Only creates character if there are elements
                if (nVert > 0)
                {
                    float[] verts = new float[3 * nVert];
                    float[] normals = new float[3 * nVert];
                    int[] elems = new int[3 * nElem];

                    for (int p = 0; p < 3 * nVert; p += 3)
                    {
                        verts[p] = BitConverter.ToSingle(Data, DataPos); DataPos += 4;
                        verts[1 + p] = BitConverter.ToSingle(Data, DataPos); DataPos += 4;
                        verts[2 + p] = BitConverter.ToSingle(Data, DataPos); DataPos += 4;
                    }
                    for (int p = 0; p < 3 * nVert; p += 3)
                    {
                        normals[p] = BitConverter.ToSingle(Data, DataPos); DataPos += 4;
                        normals[1 + p] = BitConverter.ToSingle(Data, DataPos); DataPos += 4;
                        normals[2 + p] = BitConverter.ToSingle(Data, DataPos); DataPos += 4;
                    }
                    for (int p = 0; p < 3 * nElem; p += 3)
                    {
                        elems[p] = BitConverter.ToInt32(Data, DataPos); DataPos += 4;
                        elems[1 + p] = BitConverter.ToInt32(Data, DataPos); DataPos += 4;
                        elems[2 + p] = BitConverter.ToInt32(Data, DataPos); DataPos += 4;
                    }
                    //Creates the model
                    GLchars[indChar] = new GLRender.GLVBOModel(BeginMode.Triangles);
                    GLchars[indChar].SetNormalData(normals);
                    GLchars[indChar].SetVertexData(verts);
                    GLchars[indChar].SetElemData(elems);
                }
                else
                {
                }


            }
        }
        #endregion

        #region Drawing 3D strings
        /// <summary>Creates an array of 3D models containing the given string. If target!=null adds them to target`s display list</summary>
        /// <param name="s">String to write</param>
        /// <param name="target">Target GLWindow to write</param>
        public List<GLRender.GLVBOModel> Draw3DString(string s, GLRender target)
        {
            List<GLRender.GLVBOModel> GLstr = new List<GLRender.GLVBOModel>();
            byte[] sb = System.Text.ASCIIEncoding.Default.GetBytes(s);

            float curX = 0;
            for (int i = 0; i < sb.Length; i++)
            {
                if (GLchars[sb[i]] != null)
                {
                    GLRender.GLVBOModel m = new GLRender.GLVBOModel(GLchars[sb[i]]);

                    m.vetTransl.x = curX;
                    GLstr.Add(m);

                    if (target != null) target.Models.Add(m);
                }

                curX += this.WidthInGLScale[sb[i]];
            }

            return GLstr;
        }


        /// <summary>Creates an array of 3D models containing the given string</summary>
        /// <param name="s">String to write</param>
        public List<GLRender.GLVBOModel> Draw3DString(string s)
        {
            return Draw3DString(s, null);
        }
        #endregion

        #region 3D font save

        /// <summary>Saves this 3D font to a file. Requires OpenCL/GL interoperation.</summary>
        /// <param name="file">File to save to.</param>
        public void Save(string file)
        {
            //Stores all font information
            List<byte> Data = new List<byte>();
            byte[] b;

            //Stores data for 256 characters. Last is 255
            Data.Add(255);

            for (int i = 0; i < 256; i++)
            {
                //Dealing with the i-th character
                Data.Add((byte)i);

                //Stores its width
                b = BitConverter.GetBytes(WidthInGLScale[i]);
                for (int k = 0; k < b.Length; k++) Data.Add(b[k]);

                if (GLchars[i] != null)
                {
                    //Stores the number of vertexes. Vertex bytes written afterwards will be 3*numVertexes*(4 bytes per float)
                    b = BitConverter.GetBytes(GLchars[i].numVertexes);
                    for (int k = 0; k < b.Length; k++) Data.Add(b[k]);

                    //Stores the number of elements. Element bytes written afterwards will be 3*number of elements*(4 bytes per int)
                    b = BitConverter.GetBytes(GLchars[i].ElemLength / 3);
                    for (int k = 0; k < b.Length; k++) Data.Add(b[k]);

                    CLCalc.Program.Variable CLvertex = GLchars[i].GetCLVertexBuffer();
                    CLCalc.Program.Variable CLnormals = GLchars[i].GetCLNormalBuffer();
                    CLCalc.Program.Variable CLelems = GLchars[i].GetCLElemBuffer();
                    CLCalc.Program.Variable[] vars = new CLCalc.Program.Variable[] { CLvertex, CLnormals, CLelems };

                    CLGLInteropFunctions.AcquireGLElements(vars);
                    float[] vertex = new float[CLvertex.OriginalVarLength];
                    float[] normals = new float[CLnormals.OriginalVarLength];
                    int[] elems = new int[CLelems.OriginalVarLength];

                    CLvertex.ReadFromDeviceTo(vertex);
                    CLnormals.ReadFromDeviceTo(normals);
                    CLelems.ReadFromDeviceTo(elems);

                    CLGLInteropFunctions.ReleaseGLElements(vars);

                    //Stores each vertex
                    for (int p = 0; p < vertex.Length; p++)
                    {
                        b = BitConverter.GetBytes(vertex[p]);
                        for (int k = 0; k < b.Length; k++) Data.Add(b[k]);
                    }

                    //Stores each normal
                    for (int p = 0; p < normals.Length; p++)
                    {
                        b = BitConverter.GetBytes(normals[p]);
                        for (int k = 0; k < b.Length; k++) Data.Add(b[k]);
                    }

                    //Stores each element
                    for (int p = 0; p < elems.Length; p++)
                    {
                        b = BitConverter.GetBytes(elems[p]);
                        for (int k = 0; k < b.Length; k++) Data.Add(b[k]);
                    }
                }
                else
                {
                    //Stores zero as number of vertexes
                    b = BitConverter.GetBytes((int)0);
                    for (int k = 0; k < b.Length; k++) Data.Add(b[k]);

                    //Stores zero as number of elements
                    b = BitConverter.GetBytes((int)0);
                    for (int k = 0; k < b.Length; k++) Data.Add(b[k]);
                }
            }

            System.IO.FileStream fs = new System.IO.FileStream(file, System.IO.FileMode.Create);

            using (System.IO.BinaryWriter bw = new System.IO.BinaryWriter(fs))
            {
                bw.Write(Data.ToArray());

                bw.Close();
            }
            fs.Close();
        }

        #endregion

        #region Generate texture from string

        /// <summary>Returns a Bitmap containing a text drawn. Useful to set as texture.</summary>
        /// <param name="s">String to be written</param>
        /// <param name="TextFont">Font to use</param>
        /// <param name="TextLeftColor">Left color of Text.</param>
        /// <param name="TextRightColor">Right color of Text.</param>
        /// <param name="BackgroundLeftColor">Left color of Background.</param>
        /// <param name="BackgroundRightColor">Right color of Background.</param>
        public static Bitmap DrawString(string s, Font TextFont, Color TextLeftColor, Color TextRightColor,
            Color BackgroundLeftColor, Color BackgroundRightColor)
        {
            if (s == "") return null;

            Bitmap dum = new Bitmap(10, 10);
            Graphics g = Graphics.FromImage(dum);

            SizeF size = g.MeasureString(s, TextFont);

            Bitmap bmp = new Bitmap((int)size.Width, (int)size.Height);
            Graphics gbmp = Graphics.FromImage(bmp);

            Brush bBckg = new System.Drawing.Drawing2D.LinearGradientBrush(new PointF(0, 0), new PointF(size.Width, size.Height), BackgroundLeftColor, BackgroundRightColor);
            gbmp.FillRectangle(bBckg, 0, 0, bmp.Width, bmp.Height);

            Brush bTexto = new System.Drawing.Drawing2D.LinearGradientBrush(new PointF(0, 0), new PointF(size.Width, size.Height), TextLeftColor, TextRightColor);
            gbmp.DrawString(s, TextFont, bTexto, 0, 0);

            dum.Dispose();

            return bmp;
        }

        /// <summary>Returns a Bitmap containing a text drawn. Useful to set as texture.</summary>
        /// <param name="s">String to be written</param>
        /// <param name="TextFont">Font to use</param>
        /// <param name="TextColor">Text color.</param>
        /// <param name="BackgroundColor">Background color.</param>
        public static Bitmap DrawString(string s, Font TextFont, Color TextColor, Color BackgroundColor)
        {
            return DrawString(s, TextFont, TextColor, TextColor, BackgroundColor, BackgroundColor);
        }

        /// <summary>Returns a Bitmap containing a text drawn. Useful to set as texture.</summary>
        /// <param name="s">String to be written</param>
        /// <param name="TextFont">Font to use</param>
        public static Bitmap DrawString(string s, Font TextFont)
        {
            return DrawString(s, TextFont, Color.Black, Color.Black, Color.White, Color.White);
        }

        #endregion
    }

    /// <summary>Encapsulates functions needed to acquire and release OpenCL/GL shared objects</summary>
    public static class CLGLInteropFunctions
    {
        /// <summary>Acquires one OpenCL variable created from GL buffers in order to use it. Ignores variables not created from OpenGL buffer</summary>
        /// <param name="CLGLVar">Variable to be acquired</param>
        public static void AcquireGLElements(CLCalc.Program.MemoryObject CLGLVar)
        {
            AcquireGLElements(new CLCalc.Program.MemoryObject[] { CLGLVar });
        }

        /// <summary>Acquires OpenCL variables created from GL buffers in order to use them. Ignores variables not created from OpenGL buffer</summary>
        /// <param name="CLGLVars">Variables to be acquired</param>
        public static void AcquireGLElements(CLCalc.Program.MemoryObject[] CLGLVars)
        {
            GL.Finish();

            List<ComputeMemory> ClooCLGLBuffers = new List<ComputeMemory>();
            foreach (CLCalc.Program.MemoryObject var in CLGLVars)
            {
                if (var.CreatedFromGLBuffer && (!var.AcquiredInOpenCL))
                {
                    ClooCLGLBuffers.Add(var.VarPointer);
                    var.AcquiredInOpenCL = true;
                }
            }

            CLCalc.Program.CommQueues[CLCalc.Program.DefaultCQ].AcquireGLObjects(ClooCLGLBuffers, null);
        }

        /// <summary>Releases one OpenCL variable created from GL buffers. Ignores variables not created from OpenGL buffer</summary>
        /// <param name="CLGLVar">Variable to be released</param>
        public static void ReleaseGLElements(CLCalc.Program.MemoryObject CLGLVar)
        {
            ReleaseGLElements(new CLCalc.Program.MemoryObject[] { CLGLVar });
        }

        /// <summary>Releases OpenCL variables created from GL buffers. Ignores variables not created from OpenGL buffer</summary>
        /// <param name="CLGLVars">Variables to be acquired</param>
        public static void ReleaseGLElements(CLCalc.Program.MemoryObject[] CLGLVars)
        {
            GL.Finish();

            List<ComputeMemory> ClooCLGLBuffers = new List<ComputeMemory>();
            foreach (CLCalc.Program.MemoryObject var in CLGLVars)
            {
                if (var.CreatedFromGLBuffer && var.AcquiredInOpenCL)
                {
                    ClooCLGLBuffers.Add(var.VarPointer);
                    var.AcquiredInOpenCL = false;
                }
            }

            CLCalc.Program.CommQueues[CLCalc.Program.DefaultCQ].ReleaseGLObjects(ClooCLGLBuffers, null);
            CLCalc.Program.CommQueues[CLCalc.Program.DefaultCQ].Finish();
        }

        /// <summary>Copies bitmap data to a OpenGL texture. Note: texture is flipped in Y axis, needs to correct texture coordinates</summary>
        /// <param name="TextureBitmap">Bitmap to be copied to OpenGL memory</param>
        /// <param name="ind">A valid OpenGL texture generated with GLGenTexture. If less than zero a new OpenGL texture is created and stored in ind</param>
        public static void ApplyTexture(Bitmap TextureBitmap, ref int ind)
        {
            if (TextureBitmap != null)
            {
                if (ind <= 0) ind = GL.GenTexture();

                //texture, if there is one

                //System.Drawing.Bitmap image = new System.Drawing.Bitmap(TextureBitmap);
                System.Drawing.Bitmap image = TextureBitmap;

                //image.RotateFlip(System.Drawing.RotateFlipType.RotateNoneFlipY); //this takes too long
                System.Drawing.Imaging.BitmapData bitmapdata;
                System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, image.Width, image.Height);

                bitmapdata = image.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                
                //Acquire texture
                GL.BindTexture(TextureTarget.Texture2D, ind);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Nearest);

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, image.Width, image.Height,
                    0, (OpenTK.Graphics.OpenGL.PixelFormat)(int)All.BgrExt, PixelType.UnsignedByte, bitmapdata.Scan0);

                image.UnlockBits(bitmapdata);
                
                //image.Dispose();

                //Release texture
                GL.BindTexture(TextureTarget.Texture2D, 0);
            }
        }

    }

    /// <summary>Vector class with math operations and dot / cross products.</summary>
    public class Vector : IComparable<Vector>
    {
        /// <summary>Vector X component.</summary>
        public double x;
        /// <summary>Vector Y component.</summary>
        public double y;
        /// <summary>Vector Z component.</summary>
        public double z;

        #region "Construtores e ToString"
        /// <summary>Constructor. Initializes zero vector.</summary>
        public Vector()
        {
            x = 0; y = 0; z = 0;
        }

        /// <summary>Construtor. Initializes given values.</summary>
        /// <param name="xComponent">Vector X component.</param>
        /// <param name="yComponent">Vector Y component.</param>
        /// <param name="zComponent">Vector Z component.</param>
        public Vector(double xComponent, double yComponent, double zComponent)
        {
            this.x = xComponent;
            this.y = yComponent;
            this.z = zComponent;
        }

        /// <summary>Construtor. Copies a given vector.</summary>
        /// <param name="v">Vector to copy.</param>
        public Vector(Vector v)
        {
            this.x = v.x;
            this.y = v.y;
            this.z = v.z;
        }

        /// <summary>Returns a string that represents this vector.</summary>
        public override string ToString()
        {
            return "(" + this.x.ToString() + ";" + this.y.ToString() + ";" + this.z.ToString() + ")";
        }
        #endregion

        #region "Operadores aritméticos e comparação de igualdade"
        /// <summary>Vector sum.</summary>
        /// <param name="v1">First vector to sum.</param>
        /// <param name="v2">Second vector to sum.</param>
        public static Vector operator +(Vector v1, Vector v2)
        {
            Vector resp = new Vector();
            resp.x = v1.x + v2.x;
            resp.y = v1.y + v2.y;
            resp.z = v1.z + v2.z;
            return resp;
        }
        /// <summary>Vector subtraction.</summary>
        /// <param name="v1">Vector to subtract from.</param>
        /// <param name="v2">Vector to be subtracted.</param>
        public static Vector operator -(Vector v1, Vector v2)
        {
            Vector resp = new Vector();
            resp.x = v1.x - v2.x;
            resp.y = v1.y - v2.y;
            resp.z = v1.z - v2.z;
            return resp;
        }
        /// <summary>Vector scalar product.</summary>
        /// <param name="num">Scalar to multiply.</param>
        /// <param name="v">Vector to multiply.</param>
        public static Vector operator *(double num, Vector v)
        {
            Vector resp = new Vector();
            resp.x = v.x * num;
            resp.y = v.y * num;
            resp.z = v.z * num;
            return resp;
        }
        /// <summary>Vector scalar product.</summary>
        /// <param name="num">Scalar to multiply.</param>
        /// <param name="v">Vector to multiply.</param>
        public static Vector operator *(Vector v, double num)
        {
            Vector resp = new Vector();
            resp.x = v.x * num;
            resp.y = v.y * num;
            resp.z = v.z * num;
            return resp;
        }
        /// <summary>Vector scalar division.</summary>
        /// <param name="num">Scalar to divide by.</param>
        /// <param name="v">Vector to be divided.</param>
        public static Vector operator /(double num, Vector v)
        {
            Vector resp = new Vector();
            resp.x = v.x / num;
            resp.y = v.y / num;
            resp.z = v.z / num;
            return resp;
        }
        /// <summary>Vector scalar division.</summary>
        /// <param name="num">Scalar to divide by.</param>
        /// <param name="v">Vector to be divided.</param>
        public static Vector operator /(Vector v, double num)
        {
            Vector resp = new Vector();
            resp.x = v.x / num;
            resp.y = v.y / num;
            resp.z = v.z / num;
            return resp;
        }

        /// <summary>Equality comparison.</summary>
        /// <param name="v">Vector to compare to.</param>
        public int CompareTo(Vector v)
        {
            if (this.x == v.x && this.y == v.y && this.z == v.z)
            {
                return 0;
            }
            else
            {
                return 1;
            }

        }
        #endregion

        #region "Produtos escalar e vetorial"
        /// <summary>Returns vector dot product.</summary>
        /// <param name="v1">First vector of Dot Product.</param>
        /// <param name="v2">Second vector of Dot Product.</param>
        public static double DotProduct(Vector v1, Vector v2)
        {
            return v1.x * v2.x + v1.y * v2.y + v1.z * v2.z;
        }

        /// <summary>Returns vector cross product.</summary>
        /// <param name="v1">First vector of Cross Product.</param>
        /// <param name="v2">Second vector of Cross Product.</param>
        public static Vector CrossProduct(Vector v1, Vector v2)
        {
            //i    j    k
            //this
            //v

            Vector resp = new Vector();
            resp.x = v1.y * v2.z - v2.y * v1.z;
            resp.y = -v1.x * v2.z + v2.x * v1.z;
            resp.z = v1.x * v2.y - v2.x * v1.y;
            return resp;
        }
        #endregion

        #region "Normalização para comprimento unitário"
        /// <summary>Returns vector norm.</summary>
        public double norm()
        {
            return Math.Sqrt(Vector.DotProduct(this, this));
        }

        /// <summary>Normalizes this vector.</summary>
        public void normalize()
        {
            double invNorma = 1 / this.norm();
            this.x *= invNorma;
            this.y *= invNorma;
            this.z *= invNorma;
        }
        #endregion

    }



}
