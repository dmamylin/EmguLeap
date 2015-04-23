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
using OpenCLTemplate;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Reflection;
using OpenTK;


namespace OpenTKLib
{
    public partial class Model3D
    {
        #region members

       
        public delegate float[] CoordFuncXYZ(float u, float v);

        private Vector3d centerOfGravity;
        public static bool HardwareSupportsBufferObjects = true;
        private static float rad2deg = 57.29578f;
        public string Name = "";
        public List<Part> Parts = new List<Part>();
        
        public List<Vector3d> Normals = new List<Vector3d>();
        public List<float[]> TextureCoords = new List<float[]>();
        public int GLTexture = 0;
        
        public Vertex MaxPoint = new Vertex(0, 0, 0);
        public Vertex MinPoint = new Vertex(0, 0, 0);
        //private string separadorDecimal = 1.5.ToString().Substring(1, 1);
        

        public bool ForceRedraw = false;
        public bool ShowModel = true;
        public Vector3d vetTransl = new Vector3d(0, 0, 0);
        public Vector3d vetRot = new Vector3d(0, 0, 0);
        public CLEnum.CLModelMovement modelMovement = CLEnum.CLModelMovement.Static;
        private System.Drawing.Bitmap TextureBitmap;
        private static CLCalc.Program.Kernel kernelGenUV;
        private CLCalc.Program.Kernel kernelGen3DModel;
        private CLCalc.Program.Variable varVertCoords;
        private CLCalc.Program.Variable varNormals;
        private CLCalc.Program.Variable varColors;
        private CLCalc.Program.Variable varTexCoords;
        private CLCalc.Program.Variable varu;
        private CLCalc.Program.Variable varv;
        private int uPts;
        private int vPts;
        private System.Drawing.Color ModelColor;
        private CLEnum.CLModelType modelType;
        public CLEnum.CLRenderStyle ModelRenderStyle;
        private int[] max;

        #endregion

        public Model3D()
        {
        }

        public Model3D(string Name, Model3D.CoordFuncXYZ F, float umin, float umax, float vmin, float vmax, int uPts, int vPts, Vector3d cor)
        {
            this.Create3DModel(Name, F, umin, umax, vmin, vmax, uPts, vPts, cor);
        }

        public Model3D(string Name, Model3D.CoordFuncXYZ F, float umin, float umax, float vmin, float vmax, int uPts, int vPts, Vector3d cor, System.Drawing.Bitmap Texture)
        {
            this.TextureBitmap = Texture;
            this.Create3DModel(Name, F, umin, umax, vmin, vmax, uPts, vPts, cor);
        }

        public Model3D(string Name, Model3D.CoordFuncXYZ F, float umin, float umax, int uPts, Vector3d cor)
        {
            this.Create3DLine(Name, F, umin, umax, uPts, cor);
        }
        public Model3D(string Name, List<Vertex> myVertexList)
        {
            this.Name = Name;
            
            Model3D.AssignModelDataFromVertices(this, myVertexList);
        }
        public Vector3d CenterOfGravity
        {
            get
            {
                if(centerOfGravity == Vector3d.Zero)
                    centerOfGravity = Vertices.CalculateCenterOfGravityVector(this.vertexList);
                return centerOfGravity;
            }
        }
        

        public Model3D(string file)
        {
            string str = Path.GetExtension(file).ToLower();
            if (str == ".obj")
                readOBJfile(file, this);
            else if (str == ".dxf")
                this.readDXFfile(file);
            else if (str == ".xyz")
                readXYZfile(file, this);

            else if (str == ".lab3d")
            {
                try
                {
                    DataSet dataSet = new DataSet();
                    int num = (int)dataSet.ReadXml(file);
                    DataTable OpenTKLibTbl = dataSet.Tables["Model"];
                    DataRow dataRow = OpenTKLibTbl.Rows[0];
                    System.Drawing.Bitmap PictureTexture = (System.Drawing.Bitmap)null;
                    if ((int)dataRow["intTexType"] == 2)
                    {
                        try
                        {
                            PictureTexture = new System.Drawing.Bitmap(Path.GetDirectoryName(file) + "\\" + Path.GetFileNameWithoutExtension(file) + ".JPG");
                        }
                        catch
                        {
                            dataRow["intTexType"] = (object)CLEnum.CLModelTextureType.None;
                        }
                    }
                    List<string> BuildLogs;
                    this.CreateFromOpenTKLibTbl(OpenTKLibTbl, PictureTexture, out BuildLogs);
                }
                catch
                {
                    throw new Exception("Invalid OpenTKLib file");
                }
            }
            if (!(this.Name == ""))
                return;
            this.Name = Path.GetFileNameWithoutExtension(file);
        }
        private List<Vertex> vertexList = new List<Vertex>();
        
        public List<Vertex> VertexList
        {
            get
            {
                return vertexList;


            }
            set
            {
                vertexList = value;
                for (int i = 0; i < vertexList.Count; i++)
                    vertexList[i].IndexInModel = i;
            }
        }
        public void CalculateNormals_Triangulation()
        {
            Model3D.TriangulateVertices_Rednaxela(this);
            Helper_NormalsFromTriangles();
            Helper_AdaptNormalsForEachVertex();

        }
        public static void TriangulateVertices_Rednaxela(Model3D myModel)
        {

            List<Vertex> vertices = myModel.VertexList;
            KDTreeVertex kv = new KDTreeVertex();
            kv.NumberOfNeighboursToSearch = 3;
            kv.BuildKDTree_Rednaxela(vertices);
            kv.InitVertices(vertices);
            //the result is in each Vertex as VertexIndex 
            kv.FindNearest_Points_Rednaxela(vertices);
           

            CreateTrianglesByNearestVertices(myModel);
            


        }
        public static void TriangulateVertices_Stark(Model3D myModel)
        {
            List<Vertex> vertices = myModel.VertexList;
            KDTreeVertex kv = new KDTreeVertex();
            kv.BuildKDTree_Stark(vertices);
            kv.InitVertices(vertices);
            //the result is in each Vertex as VertexIndex 
            kv.FindNearest_Points_Stark(vertices);


            CreateTrianglesByNearestVertices(myModel);
           


        }
        

  
    
        public static List<Vertex> CreateVertexList(List<Vector3d> vectors, List<float[]> colorInfo)
        {
            List<Vertex> myVertexList = new List<Vertex>();
            for (int i = 0; i < vectors.Count; i++)
            {
                Vertex vertex = new Vertex();
                vertex.Vector = vectors[i];
                vertex.IndTriangles = new List<int>();
                vertex.IndPart = new List<int>();
                vertex.IndexNormals = new List<int>();
                myVertexList.Add(vertex);

            }


            //have to add a new vertex list
            if (colorInfo != null)
            {
                List<Vertex> verticesWithColor = new List<Vertex>();
                for (int i = 0; i < myVertexList.Count; i++)
                {
                    Vertex vertex = myVertexList[i];
                    vertex.Color = new float[4] { 1f, 1f, 1f, 1f };
                    //vertex.VertexColor = new float[4];

                    vertex.Color[0] = colorInfo[i][0];
                    vertex.Color[1] = colorInfo[i][1];
                    vertex.Color[2] = colorInfo[i][2];
                    vertex.Color[3] = colorInfo[i][3];
                    verticesWithColor.Add(vertex);

                }
                myVertexList = verticesWithColor;
            }

            return myVertexList;
        }
      
        public static void AssignModelDataFromVertices(Model3D myNewModel, List<Vertex> vertexList)
        {


            myNewModel.VertexList = vertexList;

            myNewModel.Parts = new List<Part>();
            AssignTriangleAndPartFromVertex(myNewModel);
            
            
            myNewModel.CalculateBoundingBox(true);

        }
   
    
        /// <summary>
        /// creates one single triangle which contains all vertices
        /// this is added in one single part
        /// </summary>
        /// <param name="myNewModel"></param>
        public static void AssignTriangleAndPartFromVertex(Model3D myNewModel)
        {
            Part p = new Part();
            Triangle a = new Triangle();
          

            for (int i = 0; i < myNewModel.VertexList.Count; i++)
            {
                a.IndVertices.Add(i);

                myNewModel.VertexList[i].IndTriangles.Add(i);
                myNewModel.VertexList[i].IndPart.Add(i);
                a.IndTextures.Add(i);

            }
            
            p.Triangles.Add(a);
            if (p.Triangles.Count > 0)
            {
                p.ColorOverall = new Vector3d(1.0, 1.0, 1.0);
                myNewModel.Parts.Add(p);
            }


        }
        public float[,][] CreateVerticesFromFunction(Model3D.CoordFuncXYZ F, float umin, float umax, float vmin, float vmax, int uPts, int vPts)
        {
            float num1 = 1f / (float)(uPts - 1);
            float num2 = 1f / (float)(vPts - 1);
            float[,][] VertENormal = new float[uPts, vPts][];
            float[] numArray1 = new float[uPts];
            float[] numArray2 = new float[vPts];
            
            for (int i = 0; i < uPts; ++i)
                numArray1[i] = umin + (umax - umin) * (float)i * num1;

            for (int i = 0; i < vPts; ++i)
                numArray2[i] = vmin + (vmax - vmin) * (float)i * num2;

            for (int i = 0; i < uPts; ++i)
            {
                for (int j = 0; j < vPts; ++j)
                {
                    VertENormal[i, j] = F(numArray1[i], numArray2[j]);
                    if (VertENormal[i, j].Length == 6)
                    {
                        float num3 = (float)((double)VertENormal[i, j][3] * (double)VertENormal[i, j][3] + (double)VertENormal[i, j][4] * (double)VertENormal[i, j][4] + (double)VertENormal[i, j][5] * (double)VertENormal[i, j][5]);
                        if ((double)num3 != 0.0 && (double)num3 != 1.0)
                        {
                            float num4 = 1f / (float)Math.Sqrt((double)num3);
                            VertENormal[i, j][3] *= num4;
                            VertENormal[i, j][4] *= num4;
                            VertENormal[i, j][5] *= num4;
                        }
                    }
                }
            }
            return VertENormal;

        }
        private float[,][] CreateTexture()
        {
            float num1 = 1f / (float)(uPts - 1);
            float num2 = 1f / (float)(vPts - 1);

            float[,][] TexCoords = (float[,][])null;

            if (this.TextureBitmap != null)
            {
                TexCoords = new float[uPts, vPts][];
                for (int i = 0; i < uPts; ++i)
                {
                    float num3 = (float)i * num1;
                    for (int j = 0; j < vPts; ++j)
                    {
                        float num4 = (float)j * num2;
                        TexCoords[i, j] = new float[2]{num3,num4};
                    }
                }
            }
            return TexCoords;

        }
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
        public void Create3DModel(string Name, Model3D.CoordFuncXYZ F, float umin, float umax, float vmin, float vmax, int uPts, int vPts, Vector3d cor)
        {
            float[,][] VertENormal = CreateVerticesFromFunction(F, umin, umax, vmin, vmax, uPts, vPts);

            float[,][] textureCoords = CreateTexture();
           
            this.Create3DModel(Name, VertENormal, textureCoords, (float[,][])null, cor, false);
        }
        public void Create3DModel(string Name, List<Vertex> myVertexList)
        {
            this.Name = Name;
            Model3D.AssignModelDataFromVertices(this, myVertexList);
        }
       

        private void SetModelData(string Name, float[,][] VertENormal, float[,][] TexCoords, float[,][] VertexColors)
        {
            this.Name = Name;
           
            
            this.Parts = new List<Part>();
            this.vertexList = new List<Vertex>();
            this.Normals = new List<Vector3d>();
            this.TextureCoords = new List<float[]>();
            
            
            int Length1 = VertENormal.GetLength(0);
            int Length2 = VertENormal.GetLength(1);

           

            float num1 = 1f / (float)(Length1 - 1);
            float num2 = 1f / (float)(Length2 - 1);
            int indexInModel = -1;
            for (int i = 0; i < Length1; ++i)
            {
                for (int j = 0; j < Length2; ++j)
                {
                    Vertex vertex = new Vertex();
                    vertex.Vector = new Vector3d((double)VertENormal[i, j][0], (double)VertENormal[i, j][1], (double)VertENormal[i, j][2]);
                    vertex.IndexNormals = new List<int>();
                    vertex.IndPart = new List<int>();
                    vertex.IndTriangles = new List<int>();
                    vertex.IndPart.Add(0);
                    indexInModel++;
                    vertex.IndexInModel = indexInModel;
                    if (VertexColors != null)
                        vertex.Color = new float[4]{VertexColors[i, j][0],VertexColors[i, j][1],VertexColors[i, j][2],VertexColors[i, j][3]};

                    this.vertexList.Add(vertex);
                    if (VertENormal[i, j].Length == 6)
                        this.Normals.Add(new Vector3d((double)VertENormal[i, j][3], (double)VertENormal[i, j][4], (double)VertENormal[i, j][5]));
                    if (TexCoords != null)
                        this.TextureCoords.Add(new float[2]{TexCoords[i, j][0],TexCoords[i, j][1]});
                }
            }
        }

        /// <summary>Recreates 3D Model from calculated data</summary>
        /// <param name="Name">Model name.</param>
        /// <param name="VertENormal">Vertexes and normals to use to build model. float[,][3] - Only vertexes. float[6] - vertexes and normals</param>
        /// <param name="TexCoords">Texture coordinates. null - don't use. float[,][2] - coords</param>
        /// <param name="VertexColors">Vertex colors. null - don't use. float[,][4] - RGBA color</param>
        /// <param name="GlobalColor">Color vector. x=red, y=green, z=blue. Goes from 0 to 1.</param>
        /// <param name="LineModel">Is this a curve? (and not a surface)</param>
        public void Create3DModel(string Name, float[,][] VertENormal, float[,][] textureCoords, float[,][] VertexColors, Vector3d GlobalColor, bool LineModel)
        {
            SetModelData(Name, VertENormal, textureCoords, VertexColors);

            Part parts = SetPart(Name, VertENormal, GlobalColor, LineModel);

            this.Parts.Add(parts);
            if (VertENormal[0, 0].Length == 3)
                this.Helper_AdaptNormalsForEachVertex();
            this.CalculateBoundingBox(true);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="VertENormal"></param>
        /// <param name="GlobalColor"></param>
        /// <param name="LineModel"></param>
        /// <returns></returns>
        private Part SetPart(string Name, float[,][] VertENormal, Vector3d GlobalColor, bool LineModel)
        {
            int Length1 = VertENormal.GetLength(0);
            int Length2 = VertENormal.GetLength(1);


            Part parts = new Part();
            parts.Name = Name;

            if (LineModel && Length2 != 1)
                throw new Exception("Line model should have v dimension equal to 1");

            for (int i = 0; i < Length1 - 1; ++i)
            {
                Triangle a;
                if (LineModel)
                {
                    a = new Triangle();
                   
                    a.IndVertices.Add(i);
                    a.IndVertices.Add(i + 1);
                    if (VertENormal[i, 0].Length == 6)
                    {
                        a.IndNormals.Add(i);
                        a.IndNormals.Add(i + 1);
                    }
                    else
                    {
                        Vector3d vector1 = new Vector3d(this.vertexList[i].Vector);
                        Vector3d vector2 = new Vector3d(this.vertexList[i + 1].Vector) - vector1;
                        this.Normals.Add(vector2);
                        if (vector2.X == 0.0 && vector2.Y == 0.0 && vector2.Z == 0.0)
                            vector2.Z = 1.0;
                        a.IndNormals.Add(this.Normals.Count - 1);
                        a.IndNormals.Add(this.Normals.Count - 1);
                    }
                    parts.Triangles.Add(a);
                    this.vertexList[i].IndTriangles.Add(parts.Triangles.Count - 1);
                    this.vertexList[i + 1].IndTriangles.Add(parts.Triangles.Count - 1);
                }
                else
                {
                    for (int j = 0; j < Length2 - 1; ++j)
                    {
                        a = new Triangle();
                      
                        a.IndVertices.Add(i * Length2 + j);
                        a.IndVertices.Add((i + 1) * Length2 + j);
                        a.IndVertices.Add((i + 1) * Length2 + j + 1);
                        a.IndVertices.Add(i * Length2 + j + 1);
                        if (this.TextureBitmap != null)
                        {
                            a.IndTextures = new List<int>();
                            a.IndTextures.Add(i * Length2 + j);
                            a.IndTextures.Add((i + 1) * Length2 + j);
                            a.IndTextures.Add((i + 1) * Length2 + j + 1);
                            a.IndTextures.Add(i * Length2 + j + 1);
                        }
                        if (VertENormal[i, j].Length == 6)
                        {
                            a.IndNormals.Add(i * Length2 + j);
                            a.IndNormals.Add((i + 1) * Length2 + j);
                            a.IndNormals.Add((i + 1) * Length2 + j + 1);
                            a.IndNormals.Add(i * Length2 + j + 1);
                        }
                        parts.Triangles.Add(a);
                        this.vertexList[i * Length2 + j].IndTriangles.Add(parts.Triangles.Count - 1);
                        this.vertexList[i * Length2 + j + 1].IndTriangles.Add(parts.Triangles.Count - 1);
                        this.vertexList[(i + 1) * Length2 + j + 1].IndTriangles.Add(parts.Triangles.Count - 1);
                        this.vertexList[(i + 1) * Length2 + j].IndTriangles.Add(parts.Triangles.Count - 1);
                    }
                }
            }
            parts.ColorOverall = new Vector3d(GlobalColor);
            return parts;
        }
        /// <summary>Creates a 3D Line from parameterized equations</summary>
        /// <param name="Name">Line name</param>
        /// <param name="F">Coordinate function F. Has to return double[3] {x, y, z}
        /// OR double[6] {x,y,z, normalX, normalY, normalZ}. Notice that only u coordinate is passed (v=0).</param>
        /// <param name="umin">Minimum value of u coordinate.</param>
        /// <param name="umax">Maximum value of u coordinate.</param>
        /// <param name="uPts">Number of points in u partition.</param>
        /// <param name="cor">Color vector. x=red, y=green, z=blue. Goes from 0 to 1.</param>
        public void Create3DLine(string Name, Model3D.CoordFuncXYZ F, float umin, float umax, int uPts, Vector3d cor)
        {
            this.Parts = new List<Part>();
            this.vertexList = new List<Vertex>();
            this.Normals = new List<Vector3d>();
            this.TextureCoords = new List<float[]>();
            Part parts = new Part();
            this.ModelRenderStyle = CLEnum.CLRenderStyle.Wireframe;
            this.Name = Name;
            parts.Name = Name;
            float num1 = 1f / (float)(uPts - 1);
            float[][] VertENormal = new float[uPts][];
            float[] numArray = new float[uPts];
            for (int index = 0; index < uPts; ++index)
                numArray[index] = umin + (umax - umin) * (float)index * num1;
            for (int index = 0; index < uPts; ++index)
            {
                VertENormal[index] = F(numArray[index], 0.0f);
                if (VertENormal[index].Length == 6)
                {
                    float num2 = (float)((double)VertENormal[index][3] * (double)VertENormal[index][3] + (double)VertENormal[index][4] * (double)VertENormal[index][4] + (double)VertENormal[index][5] * (double)VertENormal[index][5]);
                    if ((double)num2 != 0.0 && (double)num2 != 1.0)
                    {
                        float num3 = 1f / (float)Math.Sqrt((double)num2);
                        VertENormal[index][3] *= num3;
                        VertENormal[index][4] *= num3;
                        VertENormal[index][5] *= num3;
                    }
                }
            }
            this.Create3DLine(Name, VertENormal, cor);
        }

        /// <summary>Creates a 3D Line from parameterized equations</summary>
        /// <param name="Name">Line name</param>
        /// <param name="VertENormal">Vertex and normal coordinates of curve</param>
        /// <param name="cor">Color vector. x=red, y=green, z=blue. Goes from 0 to 1.</param>
        public void Create3DLine(string Name, float[][] VertENormal, Vector3d cor)
        {
            this.Parts = new List<Part>();
            this.vertexList = new List<Vertex>();
            this.Normals = new List<Vector3d>();
            this.TextureCoords = new List<float[]>();
            Part parts = new Part();
            this.ModelRenderStyle = CLEnum.CLRenderStyle.Wireframe;
            this.Name = Name;
            parts.Name = Name;
            int Length = VertENormal.Length;
            for (int index = 0; index < Length; ++index)
            {
                Vertex vertex = new Vertex();
                vertex.Vector = new Vector3d((double)VertENormal[index][0], (double)VertENormal[index][1], (double)VertENormal[index][2]);
                vertex.IndexNormals = new List<int>();
                vertex.IndPart = new List<int>();
                vertex.IndTriangles = new List<int>();
                vertex.IndPart.Add(0);
                this.vertexList.Add(vertex);
                Vector3d vector;
                if (VertENormal[index].Length == 6)
                {
                    vector = new Vector3d((double)VertENormal[index][3], (double)VertENormal[index][4], (double)VertENormal[index][5]);
                }
                else
                {
                    vector = new Vector3d(0.0, 0.0, 1.0);
                    if (index > 0)
                    {
                        floatVector floatVector1 = new floatVector(VertENormal[index - 1][0], VertENormal[index - 1][1], VertENormal[index - 1][2]);
                        floatVector floatVector2 = new floatVector(VertENormal[index][0], VertENormal[index][1], VertENormal[index][2]) - floatVector1;
                        vector.X = (double)floatVector2.x;
                        vector.Y = (double)floatVector2.y;
                        vector.Z = (double)floatVector2.z;
                    }
                }
                this.Normals.Add(vector);
            }
            for (int index = 0; index < Length - 1; ++index)
            {
                Triangle triangle = new Triangle();
              
                triangle.IndVertices.Add(index);
                triangle.IndVertices.Add(index + 1);
                triangle.IndNormals.Add(index);
                triangle.IndNormals.Add(index + 1);
                parts.Triangles.Add(triangle);
                this.vertexList[index].IndTriangles.Add(parts.Triangles.Count - 1);
                this.vertexList[index + 1].IndTriangles.Add(parts.Triangles.Count - 1);
            }
            parts.ColorOverall = new Vector3d(cor);
            this.Parts.Add(parts);
            this.CalculateBoundingBox(true);
        }

        //public void AdaptNormalsOfTriangle(Triangle a)
        //{
        //    try
        //    {
        //        Vector3d vector4 = new Vector3d(0, 0, 0);
        //        if (a.IndVertices.Count > 1)
        //        {
        //            Vector3d vector1 = new Vector3d(this.VertexList[a.IndVertices[0]].Vector);
        //            Vector3d vector2 = new Vector3d(this.VertexList[a.IndVertices[1]].Vector);
        //            Vector3d vector3 = new Vector3d(this.VertexList[a.IndVertices[2]].Vector);

        //            vector4 = MatrixUtilsOpenTK.CrossProduct(new Vector3d(vector2 - vector1), new Vector3d(vector3 - vector1));
                    
        //        }
        //        vector4.NormalizeNew();
        //        //this.Normals.Add(vector4);
        //        //int indexNormal = this.Normals.Count - 1;
        //        //a.IndNormals.Add(indexNormal);
        //        this.Normals.Add(vector4);
        //        int indexNormal = this.Normals.Count - 1;
        //        a.IndNormals.Add(indexNormal);

        //        foreach (int index in a.IndVertices)
        //        {
        //            //this.Normals.Add(vector4);
        //           this.VertexList[index].IndexNormals.Add(indexNormal);
        //        }
        //    }
        //    catch (Exception err)
        //    {
        //        System.Windows.Forms.MessageBox.Show("Error calculating normals: " + this.Normals.Count.ToString() + "; " + err.Message);
        //    }
        //}
     
        public void CalculateBoundingBox(bool resetModelToOrigin)
        {
            lock (this)
            {

                Vertices.CalculateBoundingBox(this.vertexList, ref this.MaxPoint, ref this.MinPoint);

            }
        }
      
        public void ResetModelToOrigin()
        {

            Vertices.ResetVertexToOrigin(this.vertexList);

            
        }
      
        /// <summary>
        /// 
        /// </summary>
        /// <param name="drawTransparent"></param>
        public void Render(bool drawTransparent)
        {
            if (!this.ShowModel)
                return;
            GL.PushMatrix();
            GL.Translate((float)this.vetTransl.X, (float)this.vetTransl.Y, (float)this.vetTransl.Z);
            GL.Rotate((float)this.vetRot.Z * Model3D.rad2deg, 0.0f, 0.0f, 1f);
            GL.Rotate((float)this.vetRot.Y * Model3D.rad2deg, 0.0f, 1f, 0.0f);
            GL.Rotate((float)this.vetRot.X * Model3D.rad2deg, 1f, 0.0f, 0.0f);
            if (this.Parts == null || this.Parts.Count == 0)
            {
            }
            else
            {
                this.drawPart(drawTransparent);
            }
            GL.PopMatrix();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="drawTransparent"></param>
        private void drawPart(bool drawTransparent)
        {
            this.ApplyTexture();
            foreach (Part p in this.Parts)
            {
                if ((double)p.Transparency == 0.0 != drawTransparent || this.ForceRedraw)
                {
                    if (p.Selected)
                        GL.Color4(1f, 0.0f, 0.0f, 1f);
                    if (!p.Selected)
                      GL.Color4((float)p.ColorOverall.X, (float)p.ColorOverall.Y, (float)p.ColorOverall.Z, 1f - p.Transparency);
                    //GL.Color4(1, 0, 0, 1);
                    if (p.GLListNumber <= 0 || this.ForceRedraw)
                    {
                        if (!this.ForceRedraw)
                            p.GLListNumber = GL.GenLists(1);
                        this.drawPolygonPart(p);
                    }
                    if (Model3D.HardwareSupportsBufferObjects)
                    {
                        helperBindBufferToOpenGL(p);
                    }
                    else
                        GL.CallList(p.GLListNumber);
                }
            }
            this.ForceRedraw = false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <param name="listVertices"></param>
        /// <param name="listColor"></param>
        /// <param name="listTexture"></param>
        /// <param name="listNumberOfVertices"></param>
        /// <param name="listNormals"></param>
        private void helperSetWireframe(Part p, List<float> listVertices , List<float> listColor, List<float> listTexture ,
            List<float> listNumberOfVertices , List<float> listNormals )
        {
            foreach (Triangle triangle in p.Triangles)
            {
                for (int index1 = 1; index1 < triangle.IndVertices.Count; ++index1)
                {
                    try
                    {
                        if (this.TextureBitmap != null && triangle.IndTextures.Count > 0)
                        {
                            listTexture.Add(this.TextureCoords[triangle.IndTextures[index1 - 1]][0]);
                            listTexture.Add(this.TextureCoords[triangle.IndTextures[index1 - 1]][1]);
                            listTexture.Add(this.TextureCoords[triangle.IndTextures[index1]][0]);
                            listTexture.Add(this.TextureCoords[triangle.IndTextures[index1]][1]);
                        }
                    }
                    catch
                    {
                    }
                    if (triangle.IndNormals.Count > (index1 -1))
                    {
                        listNormals.Add((float)this.Normals[triangle.IndNormals[index1 - 1]].X);
                        listNormals.Add((float)this.Normals[triangle.IndNormals[index1 - 1]].Y);
                        listNormals.Add((float)this.Normals[triangle.IndNormals[index1 - 1]].Z);
                    }
                    else
                    {
                        listNormals.Add(0f);
                        listNormals.Add(0f);
                        listNormals.Add(0f);
                    } 
                    if (triangle.IndNormals.Count > index1 )
                    {
                        listNormals.Add((float)this.Normals[triangle.IndNormals[index1]].X);
                        listNormals.Add((float)this.Normals[triangle.IndNormals[index1]].Y);
                        listNormals.Add((float)this.Normals[triangle.IndNormals[index1]].Z);
                    }
                    {
                        listNormals.Add(0f);
                        listNormals.Add(0f);
                        listNormals.Add(0f);
                    } 
                    if (this.vertexList[index1].Color != null)
                    {
                        listColor.Add(this.vertexList[triangle.IndVertices[index1 - 1]].Color[0]);
                        listColor.Add(this.vertexList[triangle.IndVertices[index1 - 1]].Color[1]);
                        listColor.Add(this.vertexList[triangle.IndVertices[index1 - 1]].Color[2]);
                        listColor.Add(this.vertexList[triangle.IndVertices[index1 - 1]].Color[3]);
                        listColor.Add(this.vertexList[triangle.IndVertices[index1]].Color[0]);
                        listColor.Add(this.vertexList[triangle.IndVertices[index1]].Color[1]);
                        listColor.Add(this.vertexList[triangle.IndVertices[index1]].Color[2]);
                        listColor.Add(this.vertexList[triangle.IndVertices[index1]].Color[3]);
                    }
                    else
                    {
                        for (int index2 = 0; index2 < 2; ++index2)
                        {
                            listColor.Add((float)p.ColorOverall.X);
                            listColor.Add((float)p.ColorOverall.Y);
                            listColor.Add((float)p.ColorOverall.Z);
                            listColor.Add(1f - p.Transparency);
                        }
                    }
                    listVertices.Add((float)this.vertexList[triangle.IndVertices[index1 - 1]].Vector.X);
                    listVertices.Add((float)this.vertexList[triangle.IndVertices[index1 - 1]].Vector.Y);
                    listVertices.Add((float)this.vertexList[triangle.IndVertices[index1 - 1]].Vector.Z);
                    listVertices.Add((float)this.vertexList[triangle.IndVertices[index1]].Vector.X);
                    listVertices.Add((float)this.vertexList[triangle.IndVertices[index1]].Vector.Y);
                    listVertices.Add((float)this.vertexList[triangle.IndVertices[index1]].Vector.Z);
                    listNumberOfVertices.Add((float)(listVertices.Count - 2));
                    listNumberOfVertices.Add((float)(listVertices.Count - 1));
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <param name="listVertices"></param>
        /// <param name="listColor"></param>
        /// <param name="listTexture"></param>
        /// <param name="listNumberOfVertices"></param>
        /// <param name="listNormals"></param>
        private void helperSetSolid(Part p, List<float> listVertices, List<float> listColor, List<float> listTexture,
           List<float> listNumberOfVertices, List<float> listNormals)
        {
            foreach (Triangle triangle in p.Triangles)
            {
                for (int i = 2; i < triangle.IndVertices.Count; ++i)
                {
                    try
                    {
                        if (this.TextureBitmap != null && triangle.IndTextures.Count > 0)
                        {
                            listTexture.Add(this.TextureCoords[triangle.IndTextures[0]][0]);
                            listTexture.Add(this.TextureCoords[triangle.IndTextures[0]][1]);
                            listTexture.Add(this.TextureCoords[triangle.IndTextures[i - 1]][0]);
                            listTexture.Add(this.TextureCoords[triangle.IndTextures[i - 1]][1]);
                            listTexture.Add(this.TextureCoords[triangle.IndTextures[i]][0]);
                            listTexture.Add(this.TextureCoords[triangle.IndTextures[i]][1]);
                        }
                    }
                    catch
                    {
                    }
                    if (triangle.IndNormals.Count > 0)
                    {

                        listNormals.Add((float)this.Normals[triangle.IndNormals[0]].X);
                        listNormals.Add((float)this.Normals[triangle.IndNormals[0]].Y);
                        listNormals.Add((float)this.Normals[triangle.IndNormals[0]].Z);
                    }
                    else 
                    {
                        listNormals.Add(0f);
                        listNormals.Add(0f);
                        listNormals.Add(0f);
                    } 
                    if (triangle.IndNormals.Count > i)
                    {
                        listNormals.Add((float)this.Normals[triangle.IndNormals[i - 1]].X);
                        listNormals.Add((float)this.Normals[triangle.IndNormals[i - 1]].Y);
                        listNormals.Add((float)this.Normals[triangle.IndNormals[i - 1]].Z);
                        listNormals.Add((float)this.Normals[triangle.IndNormals[i]].X);
                        listNormals.Add((float)this.Normals[triangle.IndNormals[i]].Y);
                        listNormals.Add((float)this.Normals[triangle.IndNormals[i]].Z);
                    }
                    else
                    {
                        listNormals.Add(0f);
                        listNormals.Add(0f);
                        listNormals.Add(0f);
                        listNormals.Add(0f);
                        listNormals.Add(0f);
                        listNormals.Add(0f);
                    } 
                    if (this.vertexList[i].Color != null)
                    {
                        listColor.Add(this.vertexList[triangle.IndVertices[0]].Color[0]);
                        listColor.Add(this.vertexList[triangle.IndVertices[0]].Color[1]);
                        listColor.Add(this.vertexList[triangle.IndVertices[0]].Color[2]);
                        listColor.Add(this.vertexList[triangle.IndVertices[0]].Color[3]);
                        listColor.Add(this.vertexList[triangle.IndVertices[i - 1]].Color[0]);
                        listColor.Add(this.vertexList[triangle.IndVertices[i - 1]].Color[1]);
                        listColor.Add(this.vertexList[triangle.IndVertices[i - 1]].Color[2]);
                        listColor.Add(this.vertexList[triangle.IndVertices[i - 1]].Color[3]);
                        listColor.Add(this.vertexList[triangle.IndVertices[i]].Color[0]);
                        listColor.Add(this.vertexList[triangle.IndVertices[i]].Color[1]);
                        listColor.Add(this.vertexList[triangle.IndVertices[i]].Color[2]);
                        listColor.Add(this.vertexList[triangle.IndVertices[i]].Color[3]);
                    }
                    else
                    {
                        for (int index2 = 0; index2 < 3; ++index2)
                        {
                            listColor.Add((float)p.ColorOverall.X);
                            listColor.Add((float)p.ColorOverall.Y);
                            listColor.Add((float)p.ColorOverall.Z);
                            listColor.Add(1f - p.Transparency);
                        }
                    }
                    listVertices.Add((float)this.vertexList[triangle.IndVertices[0]].Vector.X);
                    listVertices.Add((float)this.vertexList[triangle.IndVertices[0]].Vector.Y);
                    listVertices.Add((float)this.vertexList[triangle.IndVertices[0]].Vector.Z);
                    listVertices.Add((float)this.vertexList[triangle.IndVertices[i - 1]].Vector.X);
                    listVertices.Add((float)this.vertexList[triangle.IndVertices[i - 1]].Vector.Y);
                    listVertices.Add((float)this.vertexList[triangle.IndVertices[i - 1]].Vector.Z);
                    listVertices.Add((float)this.vertexList[triangle.IndVertices[i]].Vector.X);
                    listVertices.Add((float)this.vertexList[triangle.IndVertices[i]].Vector.Y);
                    listVertices.Add((float)this.vertexList[triangle.IndVertices[i]].Vector.Z);
                    for (int index2 = 3; index2 >= 1; --index2)
                        listNumberOfVertices.Add((float)(listVertices.Count - index2));
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <param name="listVertices"></param>
        /// <param name="listColor"></param>
        /// <param name="listTexture"></param>
        /// <param name="listNumberOfVertices"></param>
        /// <param name="listNormals"></param>
        private void helperSetPoints(Part p, List<float> listVertices, List<float> listColor, List<float> listTexture,
         List<float> listNumberOfVertices, List<float> listNormals)
        {
            try
            {
                foreach (Triangle triangle in p.Triangles)
                {
                    for (int index = 0; index < triangle.IndVertices.Count; ++index)
                    {
                        try
                        {
                            if (this.TextureBitmap != null && triangle.IndTextures.Count > 0)
                            {
                                listTexture.Add(this.TextureCoords[triangle.IndTextures[index]][0]);
                                listTexture.Add(this.TextureCoords[triangle.IndTextures[index]][1]);
                            }
                        }
                        catch
                        {
                        }
                        if (triangle.IndNormals.Count > index)
                        {
                            listNormals.Add((float)this.Normals[triangle.IndNormals[index]].X);
                            listNormals.Add((float)this.Normals[triangle.IndNormals[index]].Y);
                            listNormals.Add((float)this.Normals[triangle.IndNormals[index]].Z);
                        }
                        else
                        {
                            listNormals.Add(0f);
                            listNormals.Add(0f);
                            listNormals.Add(0f);
                        }
                        if (this.vertexList[index].Color != null)
                        {
                            listColor.Add(this.vertexList[triangle.IndVertices[index]].Color[0]);
                            listColor.Add(this.vertexList[triangle.IndVertices[index]].Color[1]);
                            listColor.Add(this.vertexList[triangle.IndVertices[index]].Color[2]);
                            listColor.Add(this.vertexList[triangle.IndVertices[index]].Color[3]);
                        }
                        else
                        {
                            listColor.Add((float)p.ColorOverall.X);
                            listColor.Add((float)p.ColorOverall.Y);
                            listColor.Add((float)p.ColorOverall.Z);
                            listColor.Add(1f - p.Transparency);
                        }
                        listVertices.Add((float)this.vertexList[triangle.IndVertices[index]].Vector.X);
                        listVertices.Add((float)this.vertexList[triangle.IndVertices[index]].Vector.Y);
                        listVertices.Add((float)this.vertexList[triangle.IndVertices[index]].Vector.Z);
                        listNumberOfVertices.Add((float)(listVertices.Count - 1));
                    }
                }
            }
            catch(Exception err)
            {
                System.Windows.Forms.MessageBox.Show("SW Error in helperSetPoints: " + err.Message);

            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <param name="listVertices"></param>
        /// <param name="listColor"></param>
        /// <param name="listTexture"></param>
        /// <param name="listNumberOfVertices"></param>
        /// <param name="listNormals"></param>
        private void helperBindBuffersToPart(Part p, List<float> listVertices, List<float> listColor, List<float> listTexture,
         List<float> listNumberOfVertices, List<float> listNormals)
        {
            p.GLNumElements = listNumberOfVertices.Count;

            BufferUsageHint usage = this.modelMovement != CLEnum.CLModelMovement.Dynamic ? BufferUsageHint.StaticDraw : BufferUsageHint.StreamDraw;
            GL.BindBuffer(BufferTarget.ArrayBuffer, p.GLBuffers[0]);
            GL.BufferData<float>(BufferTarget.ArrayBuffer, (IntPtr)(listColor.Count * 4), listColor.ToArray(), usage);
            GL.BindBuffer(BufferTarget.ArrayBuffer, p.GLBuffers[1]);
            GL.BufferData<float>(BufferTarget.ArrayBuffer, (IntPtr)(listVertices.Count * 4), listVertices.ToArray(), usage);
            GL.BindBuffer(BufferTarget.ArrayBuffer, p.GLBuffers[2]);
            if (listNormals.Count > 0)
                GL.BufferData<float>(BufferTarget.ArrayBuffer, (IntPtr)(listNormals.Count * 4), listNormals.ToArray(), usage);

            if (this.TextureBitmap != null)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, p.GLBuffers[3]);
                GL.BufferData<float>(BufferTarget.ArrayBuffer, (IntPtr)(listTexture.Count * 4), listTexture.ToArray(), usage);
            }
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, p.GLBuffers[4]);
            GL.BufferData<float>(BufferTarget.ElementArrayBuffer, (IntPtr)(listNumberOfVertices.Count * 4), listNumberOfVertices.ToArray(), usage);
        }

        private void helperBindBufferToOpenGL(Part p)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, p.GLBuffers[0]);
            GL.ColorPointer(4, ColorPointerType.Float, 0, 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, p.GLBuffers[1]);
            GL.VertexPointer(3, VertexPointerType.Float, 0, 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, p.GLBuffers[2]);
            GL.NormalPointer(NormalPointerType.Float, 0, 0);
            if (this.TextureBitmap != null)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, p.GLBuffers[3]);
                GL.TexCoordPointer(2, TexCoordPointerType.Float, 0, 0);
            }
            else
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, p.GLBuffers[4]);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.ColorArray);
            GL.EnableClientState(ArrayCap.NormalArray);
            if (this.TextureBitmap != null)
                GL.EnableClientState(ArrayCap.TextureCoordArray);

            //draw object :
            if (this.ModelRenderStyle == CLEnum.CLRenderStyle.Point)
            {
                GL.PointSize(GLSettings.PointSize);
                GL.DrawArrays(PrimitiveType.Points, 0, p.GLNumElements);
            }
            else if (this.ModelRenderStyle == CLEnum.CLRenderStyle.Solid)
                GL.DrawArrays(PrimitiveType.Triangles, 0, p.GLNumElements);
            else if (this.ModelRenderStyle == CLEnum.CLRenderStyle.Wireframe)
                GL.DrawArrays(PrimitiveType.Lines, 0, p.GLNumElements);
            
            GL.DisableClientState(ArrayCap.VertexArray);
            GL.DisableClientState(ArrayCap.ColorArray);
            GL.DisableClientState(ArrayCap.NormalArray);
            if (this.TextureBitmap != null)
                GL.DisableClientState(ArrayCap.TextureCoordArray);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        private void helperBindBuffersToPart_WithoutHardwareSupport(Part p)
        {
            Model3D.HardwareSupportsBufferObjects = false;
            GL.NewList(p.GLListNumber, ListMode.Compile);
            foreach (Triangle triangle in p.Triangles)
            {
                if (this.ModelRenderStyle == CLEnum.CLRenderStyle.Point)
                {
                    GL.Begin(PrimitiveType.Points);
                    for (int index = 0; index < triangle.IndVertices.Count; ++index)
                    {
                        try
                        {
                            if (this.TextureBitmap != null && triangle.IndTextures.Count > 0)
                                GL.TexCoord2(this.TextureCoords[triangle.IndTextures[index]][0], this.TextureCoords[triangle.IndTextures[index]][1]);
                        }
                        catch
                        {
                        }
                        if (this.vertexList[index].Color != null)
                            GL.Color4(this.vertexList[triangle.IndVertices[index]].Color[0], this.vertexList[triangle.IndVertices[index]].Color[1], this.vertexList[triangle.IndVertices[index]].Color[2], this.vertexList[triangle.IndVertices[index]].Color[3]);
                         if (triangle.IndNormals.Count > 0)
                            GL.Normal3((float)this.Normals[triangle.IndNormals[0]].X, (float)this.Normals[triangle.IndNormals[0]].Y, (float)this.Normals[triangle.IndNormals[0]].Z);
                        GL.Vertex3((float)this.vertexList[triangle.IndVertices[index]].Vector.X, (float)this.vertexList[triangle.IndVertices[index]].Vector.Y, (float)this.vertexList[triangle.IndVertices[index]].Vector.Z);
                    }
                    GL.End();
                    GL.PointSize(GLSettings.PointSize);
                }
                else if (this.ModelRenderStyle == CLEnum.CLRenderStyle.Wireframe)
                {
                    GL.Begin(PrimitiveType.LineStrip);
                    for (int index = 0; index < triangle.IndVertices.Count; ++index)
                    {
                        try
                        {
                            if (this.TextureBitmap != null && triangle.IndTextures.Count > 0)
                                GL.TexCoord2(this.TextureCoords[triangle.IndTextures[index]][0], this.TextureCoords[triangle.IndTextures[index]][1]);
                        }
                        catch
                        {
                        }
                        if (this.vertexList[index].Color != null)
                            GL.Color4(this.vertexList[triangle.IndVertices[index]].Color[0], this.vertexList[triangle.IndVertices[index]].Color[1], this.vertexList[triangle.IndVertices[index]].Color[2], this.vertexList[triangle.IndVertices[index]].Color[3]);
                        if (triangle.IndNormals != null && triangle.IndNormals.Count > 0)
                            GL.Normal3((float)this.Normals[triangle.IndNormals[0]].X, (float)this.Normals[triangle.IndNormals[0]].Y, (float)this.Normals[triangle.IndNormals[0]].Z);
                        //GL.Normal3((float)this.Normals[triangle.IndNormals[index]].X, (float)this.Normals[triangle.IndNormals[index]].Y, (float)this.Normals[triangle.IndNormals[index]].Z);

                        GL.Vertex3((float)this.vertexList[triangle.IndVertices[index]].Vector.X, (float)this.vertexList[triangle.IndVertices[index]].Vector.Y, (float)this.vertexList[triangle.IndVertices[index]].Vector.Z);
                    }
                    GL.End();
                }
                else if (this.ModelRenderStyle == CLEnum.CLRenderStyle.Solid)
                {
                    GL.Begin(PrimitiveType.TriangleFan);
                    for (int index = 0; index < triangle.IndVertices.Count; ++index)
                    {
                        try
                        {
                            if (this.TextureBitmap != null && triangle.IndTextures.Count > 0)
                                GL.TexCoord2(this.TextureCoords[triangle.IndTextures[index]][0], this.TextureCoords[triangle.IndTextures[index]][1]);
                        }
                        catch
                        {
                        }
                        if (triangle.IndNormals.Count > 0)
                            GL.Normal3((float)this.Normals[triangle.IndNormals[0]].X, (float)this.Normals[triangle.IndNormals[0]].Y, (float)this.Normals[triangle.IndNormals[0]].Z);
                            //GL.Normal3((float)this.Normals[triangle.IndNormals[index]].X, (float)this.Normals[triangle.IndNormals[index]].Y, (float)this.Normals[triangle.IndNormals[index]].Z);
                        
                        if (this.vertexList[index].Color != null)
                            GL.Color4(this.vertexList[triangle.IndVertices[index]].Color[0], this.vertexList[triangle.IndVertices[index]].Color[1], this.vertexList[triangle.IndVertices[index]].Color[2], this.vertexList[triangle.IndVertices[index]].Color[3]);
                        GL.Vertex3((float)this.vertexList[triangle.IndVertices[index]].Vector.X, (float)this.vertexList[triangle.IndVertices[index]].Vector.Y, (float)this.vertexList[triangle.IndVertices[index]].Vector.Z);
                    }
                    GL.End();
                }
               
               
            }
            GL.EndList();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        private void drawPolygonPart(Part p)
        {
            try
            {
                if (!Model3D.HardwareSupportsBufferObjects)
                {
                    // old: throw new Exception("Buffer objects not supported");
                    helperBindBuffersToPart_WithoutHardwareSupport(p);
                    return;
                }
                  
                if (p.GLBuffers == null)
                {
                    p.GLBuffers = new int[5];
                    GL.GenBuffers(5, p.GLBuffers);
                }
                List<float> listVertices = new List<float>();
                List<float> listColor = new List<float>();
                List<float> listTexture = new List<float>();
                List<float> listNumberOfVertices = new List<float>();
                List<float> listNormals = new List<float>();

                if (this.ModelRenderStyle == CLEnum.CLRenderStyle.Point)
                {
                    helperSetPoints(p, listVertices, listColor, listTexture, listNumberOfVertices, listNormals);
                }
                else if (this.ModelRenderStyle == CLEnum.CLRenderStyle.Wireframe)
                {
                    helperSetWireframe(p, listVertices, listColor, listTexture, listNumberOfVertices, listNormals);

                }
                else if (this.ModelRenderStyle == CLEnum.CLRenderStyle.Solid)
                {
                    helperSetSolid(p, listVertices, listColor, listTexture, listNumberOfVertices, listNormals);
                }
                
                

                helperBindBuffersToPart(p, listVertices, listColor, listTexture, listNumberOfVertices, listNormals);
            }
            catch(Exception err)
            {
                System.Windows.Forms.MessageBox.Show("SW Error - to check: " + err.Message);
                helperBindBuffersToPart_WithoutHardwareSupport(p);
                
            }
        }

        public void ApplyTexture(System.Drawing.Bitmap Bmp)
        {
            GL.DeleteTexture(this.GLTexture);
            this.GLTexture = 0;
            this.TextureBitmap = Bmp;
            GC.Collect();
        }

        private void ApplyTexture()
        {
            try
            {
                if (this.TextureBitmap != null)
                {
                    if (this.GLTexture <= 0)
                    {
                        System.Drawing.Bitmap bitmap = ResizeToPowerOf2((Image)new System.Drawing.Bitmap((Image)this.TextureBitmap));
                        bitmap.RotateFlip(RotateFlipType.Rotate180FlipX);
                        Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
                        BitmapData bitmapdata = bitmap.LockBits(rect, ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                        GL.GenTextures(1, out this.GLTexture);
                        GL.BindTexture(TextureTarget.Texture2D, this.GLTexture);
                        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb8, bitmap.Width, bitmap.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgr, PixelType.UnsignedByte, bitmapdata.Scan0);
                        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, 9729);
                        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, 9729);
                        bitmap.UnlockBits(bitmapdata);
                        bitmap.Dispose();
                    }
                    else
                        GL.BindTexture(TextureTarget.Texture2D, this.GLTexture);
                }
                else
                    GL.BindTexture(TextureTarget.Texture2D, 0);
            }
            catch
            {
               
            }
        }

        private static System.Drawing.Bitmap ResizeToPowerOf2(Image img)
        {
            int width1 = img.Width;
            int height1 = img.Height;
            int width2 = (int)Math.Pow(2.0, Math.Ceiling(Math.Log((double)width1, 2.0)));
            int height2 = (int)Math.Pow(2.0, Math.Ceiling(Math.Log((double)height1, 2.0)));
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(width2, height2);
            Graphics graphics = Graphics.FromImage((Image)bitmap);
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.DrawImage(img, 0, 0, width2, height2);
            graphics.Dispose();
            return bitmap;
        }

        public void Translate(int mode, double offset)
        {
            switch (mode)
            {
                case 0:
                    this.vetTransl.X += offset;
                    break;
                case 1:
                    this.vetTransl.Y += offset;
                    break;
                case 2:
                    this.vetTransl.Z += offset;
                    break;
                case 3:
                    this.vetRot.X += offset;
                    break;
                case 4:
                    this.vetRot.Y += offset;
                    break;
                case 5:
                    this.vetRot.Z += offset;
                    break;
            }
        }

        public void SelectPart(int indPart)
        {
            this.Parts[indPart].Selected = true;
            this.Render((double)this.Parts[indPart].Transparency != 0.0);
        }

        public void DeSelectPart(int indPart)
        {
            this.Parts[indPart].Selected = false;
            this.Render((double)this.Parts[indPart].Transparency != 0.0);
        }

     

        public void CreateFromOpenTKLibTbl(DataTable OpenTKLibTbl, System.Drawing.Bitmap PictureTexture, out List<string> BuildLogs)
        {
            Model3D.AdjustCompatibility(OpenTKLibTbl);
            if (CLCalc.CLAcceleration == CLCalc.CLAccelerationType.Unknown)
                CLCalc.InitCL();
            if (CLCalc.CLAcceleration != CLCalc.CLAccelerationType.UsingCL)
                throw new Exception("OpenCLDisabledException");
            string str1;
            string str2;
            try
            {
                DataRow dataRow = OpenTKLibTbl.Rows[0];
                this.Name = dataRow["ModelName"].ToString();
                str1 = dataRow["Equation"].ToString();
                str2 = dataRow["AdvancedFunctions"].ToString();
                this.modelType = (CLEnum.CLModelType)dataRow["intModelType"];
                CLEnum.CLModelTextureType modelTextureType = (CLEnum.CLModelTextureType)dataRow["intTexType"];
                CLEnum.CLModelQuality clModelQuality = (CLEnum.CLModelQuality)dataRow["intModelQuality"];
                this.modelMovement = (CLEnum.CLModelMovement)dataRow["intModelMovement"];
                this.uPts = (int)dataRow["intUPts"];
                this.vPts = this.modelType != CLEnum.CLModelType.Surface ? 1 : (int)dataRow["intVPts"];
                this.ModelColor =System.Drawing.Color.FromArgb((int)dataRow["intColorR"], (int)dataRow["intColorG"], (int)dataRow["intColorB"]);
                if (modelTextureType == CLEnum.CLModelTextureType.FromText)
                    PictureTexture = Model3D.DrawString(dataRow["TextureText"].ToString(), new Font(dataRow["fontFamily"].ToString(), (float)(double)dataRow["dblFontSize"], (FontStyle)dataRow["intFontStyle"]),System.Drawing.Color.FromArgb((int)dataRow["intTextLeftR"], (int)dataRow["intTextLeftG"], (int)dataRow["intTextLeftB"]),System.Drawing.Color.FromArgb((int)dataRow["intTextRightR"], (int)dataRow["intTextRightG"], (int)dataRow["intTextRightB"]),System.Drawing.Color.FromArgb((int)dataRow["intBackLeftR"], (int)dataRow["intBackLeftG"], (int)dataRow["intBackLeftB"]),System.Drawing.Color.FromArgb((int)dataRow["intBackRightR"], (int)dataRow["intBackRightG"], (int)dataRow["intBackRightB"]));
                this.ModelRenderStyle = (CLEnum.CLRenderStyle)dataRow["intRenderStyle"];
            }
            catch
            {
                throw new Exception("Invalid OpenTKLib format");
            }
            string str3 = "\r\n\r\n                //#pragma OPENCL EXTENSION cl_khr_fp64 : enable\r\n\r\n                __kernel void\r\n                GenerateUV( __global write_only float * uv)\r\n                {\r\n                    int i = get_global_id(0);\r\n                    int n = get_global_size(0);\r\n                    uv[i] = (float)i/(float)(n-1);\r\n                }";
            string str4 = "\r\n\r\n   __kernel void\r\n   Create3DModel(global float4 * Model_VertCoords,\r\n                 global float4 * Model_Normals,\r\n                 global float4 * Model_Colors,\r\n                 global float2 * Model_TexCoords,\r\n                 global float  * Vals_u,\r\n                 global float  * Vals_v,\r\n                 global float  * Vals_tempo)\r\n                 \r\n   {\r\n       int Var_i = get_global_id(0);\r\n       int Var_j = get_global_id(1);\r\n       int Var_iPts = get_global_size(0);\r\n\r\n       float uu = Vals_u[Var_i];\r\n       float vv = Vals_v[Var_j];\r\n       float t  = Vals_tempo[0];\r\n       \r\n       float4 Coords, Normal,System.Drawing.Color;\r\n       float2 TexCoords;\r\n       \r\n       float u, v, uMin, uMax, vMin, vMax;\r\n\r\n            ";
            string str5 = "\r\n       //Retrieve data\r\n       Model_VertCoords[Var_i+Var_iPts*Var_j] = Coords;\r\n       Model_Normals[Var_i+Var_iPts*Var_j]    = Normal;\r\n       Model_Colors[Var_i+Var_iPts*Var_j]     =System.Drawing.Color;\r\n       Model_TexCoords[Var_i+Var_iPts*Var_j]  = TexCoords;\r\n   }\r\n";
            CLCalc.Program.Compile(new string[3]
      {
        str3,
        str2,
        str4 + str1 + str5
      }, out BuildLogs);
            BuildLogs = (List<string>)null;
            if (Model3D.kernelGenUV == null)
                Model3D.kernelGenUV = new CLCalc.Program.Kernel("GenerateUV");
            this.kernelGen3DModel = new CLCalc.Program.Kernel("Create3DModel");
            float[] Values1 = new float[this.uPts];
            float[] Values2 = new float[this.vPts];
            this.varu = new CLCalc.Program.Variable(Values1);
            this.varv = new CLCalc.Program.Variable(Values2);
            float[] Values3 = new float[this.uPts * this.vPts * 4];
            float[] Values4 = new float[this.uPts * this.vPts * 4];
            float[] Values5 = new float[this.uPts * this.vPts * 4];
            float[] Values6 = new float[this.uPts * this.vPts * 2];
            this.varVertCoords = new CLCalc.Program.Variable(Values3);
            this.varNormals = new CLCalc.Program.Variable(Values4);
            this.varColors = new CLCalc.Program.Variable(Values5);
            this.varTexCoords = new CLCalc.Program.Variable(Values6);
            int[] GlobalWorkSize = new int[1]
      {
        this.uPts
      };
            CLCalc.Program.Variable[] variableArray1 = new CLCalc.Program.Variable[1]
      {
        this.varu
      };
            Model3D.kernelGenUV.Execute((CLCalc.Program.MemoryObject[])variableArray1, GlobalWorkSize);
            CLCalc.Program.Variable[] variableArray2 = new CLCalc.Program.Variable[1]
      {
        this.varv
      };
            GlobalWorkSize[0] = this.vPts;
            Model3D.kernelGenUV.Execute((CLCalc.Program.MemoryObject[])variableArray2, GlobalWorkSize);
            this.ApplyTexture(PictureTexture);
            this.Redraw3DModel(new CLCalc.Program.Variable(new int[1]));
        }

        public void Redraw3DModel(CLCalc.Program.Variable vart)
        {
            if (this.max == null)
                this.max = new int[2];
            this.max[0] = this.uPts;
            this.max[1] = this.vPts;
            this.kernelGen3DModel.Execute((CLCalc.Program.MemoryObject[])new CLCalc.Program.Variable[7]{this.varVertCoords,this.varNormals,this.varColors,
                this.varTexCoords,this.varu,this.varv,vart}, this.max);
            float[] Values1 = new float[this.uPts * this.vPts * 4];
            float[] Values2 = new float[this.uPts * this.vPts * 4];
            float[] Values3 = new float[this.uPts * this.vPts * 4];
            float[] Values4 = new float[this.uPts * this.vPts * 2];
            this.varVertCoords.ReadFromDeviceTo(Values1);
            this.varNormals.ReadFromDeviceTo(Values2);
            this.varColors.ReadFromDeviceTo(Values3);
            this.varTexCoords.ReadFromDeviceTo(Values4);
            float[,][] VertENormal = new float[this.uPts, this.vPts][];
            float[,][] TexCoords = new float[this.uPts, this.vPts][];
            float[,][] VertexColors = (float[,][])null;
            bool flag = (double)Values2[0] != 0.0 || (double)Values2[1] != 0.0 || (double)Values2[2] != 0.0;
            if ((double)Values3[0] != 0.0 || (double)Values3[1] != 0.0 || (double)Values3[2] != 0.0 || (double)Values3[3] != 0.0)
                VertexColors = new float[this.uPts, this.vPts][];
            for (int index1 = 0; index1 < this.uPts; ++index1)
            {
                for (int index2 = 0; index2 < this.vPts; ++index2)
                {
                    int index3 = 4 * (index1 + this.uPts * index2);
                    if (flag)
                    {
                        VertENormal[index1, index2] = new float[6];
                        VertENormal[index1, index2][3] = Values2[index3];
                        VertENormal[index1, index2][4] = Values2[1 + index3];
                        VertENormal[index1, index2][5] = Values2[2 + index3];
                    }
                    else
                        VertENormal[index1, index2] = new float[3];
                    VertENormal[index1, index2][0] = Values1[index3];
                    VertENormal[index1, index2][1] = Values1[1 + index3];
                    VertENormal[index1, index2][2] = Values1[2 + index3];
                    if ((double)Values3[0] != 0.0 || (double)Values3[1] != 0.0 || (double)Values3[2] != 0.0 || (double)Values3[3] != 0.0)
                    {
                        VertexColors[index1, index2] = new float[4];
                        VertexColors[index1, index2][0] = Values3[index3];
                        VertexColors[index1, index2][1] = Values3[1 + index3];
                        VertexColors[index1, index2][2] = Values3[2 + index3];
                        VertexColors[index1, index2][3] = Values3[3 + index3];
                    }
                    TexCoords[index1, index2] = new float[2];
                    TexCoords[index1, index2][0] = Values4[2 * (index1 + this.uPts * index2)];
                    TexCoords[index1, index2][1] = Values4[1 + 2 * (index1 + this.uPts * index2)];
                }
            }
            double num = 1.0 / (double)byte.MaxValue;
            Vector3d GlobalColor = new Vector3d((double)this.ModelColor.R * num, (double)this.ModelColor.G * num, (double)this.ModelColor.B * num);
            this.Create3DModel(this.Name, VertENormal, TexCoords, VertexColors, GlobalColor, this.modelType == CLEnum.CLModelType.Curve);
            this.ForceRedraw = true;
        }

      

       
    }
}
