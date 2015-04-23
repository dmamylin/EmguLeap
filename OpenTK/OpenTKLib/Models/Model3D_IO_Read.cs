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
using System.Text;

namespace OpenTKLib
{
    public partial class Model3D
    {

        private static void readXYZfile(string fileName, Model3D myNewModel)
        {

            List<Vector3d> vectors = IOUtils.ReadXYZFile_ToVector3d(fileName, false);
            //List<byte[]> colorInfo;
            List<Vertex> myVertexList = CreateVertexList(vectors, null);
            AssignModelDataFromVertices(myNewModel, myVertexList);


        }
        private static Model3D readOBJfile(string fileOBJ)
        {
            Model3D myNewModel = new Model3D();
            readOBJfile(fileOBJ, myNewModel);
            return myNewModel;

        }
        private void readDXFfile(string fileDXF)
        {
            using (StreamReader sr = new StreamReader(fileDXF))
            {
                Part parts = new Part();
                parts.Triangles = new List<Triangle>();
                parts.ColorOverall = new Vector3d(0.5, 0.5, 0.5);
                this.Parts = new List<Part>();
                this.Parts.Add(parts);
                while (!sr.EndOfStream)
                {
                    string str = GeneralSettings.TreatLanguageSpecifics(sr.ReadLine().Trim());
                    if (str.ToUpper() == "3DFACE" || str.ToUpper() == "SOLID")
                        this.read3DFace(sr);
                    else if (str.ToUpper() == "POLYLINE")
                        this.readPolyline(sr);
                }
                sr.Close();
                Helper_AdaptNormalsForEachVertex();
                CalculateBoundingBox(true);
            }
        }
      
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileOBJ"></param>
        /// <param name="myNewModel"></param>
        private static void readOBJfile(string fileOBJ, Model3D myNewModel)
        {
            string line = string.Empty;
            int indexInModel = -1;
            try
            {

                using (StreamReader streamReader = new StreamReader(fileOBJ))
                {
                    Part p = new Part();
                    Vertex vertex = new Vertex();
                    myNewModel.Parts = new List<Part>();
                    while (!streamReader.EndOfStream)
                    {
                        line = streamReader.ReadLine().Trim();
                        while (line.EndsWith("\\"))
                            line = line.Substring(0, line.Length - 1) + streamReader.ReadLine().Trim();
                        string str1 = GeneralSettings.TreatLanguageSpecifics(line);
                        string[] strArrayRead = str1.Split();
                        if (strArrayRead.Length >= 0)
                        {
                            switch (strArrayRead[0].ToLower())
                            {
                                case "mtllib":
                                    if (strArrayRead.Length < 2)
                                    {
                                        System.Windows.Forms.MessageBox.Show("Error reading obj file (mtllib) in line : " + line);
                                    }

                                    myNewModel.GetTexture(strArrayRead[1], fileOBJ);
                                    break;
                                case "v"://Vertex
                                    vertex = IOUtils.HelperReadVertex(strArrayRead);
                                    indexInModel++;
                                    vertex.IndexInModel = indexInModel;
                                    myNewModel.vertexList.Add(vertex);
                                    break;
                                case "vt"://Texture
                                    if (strArrayRead.Length < 3)
                                    {
                                        System.Windows.Forms.MessageBox.Show("Error reading obj file (Vertex) in line : " + line);
                                    }
                                    Vector3d vector1 = new Vector3d(0.0, 0.0, 0.0);
                                    double.TryParse(strArrayRead[1], NumberStyles.Float | NumberStyles.AllowThousands, (IFormatProvider)null, out vector1.X);
                                    double.TryParse(strArrayRead[2], NumberStyles.Float | NumberStyles.AllowThousands, (IFormatProvider)null, out vector1.Y);
                                    myNewModel.TextureCoords.Add(new float[2] { (float)vector1.X, (float)vector1.Y });
                                    break;
                                case "vn"://Normals
                                    if (strArrayRead.Length < 4)
                                    {
                                        System.Windows.Forms.MessageBox.Show("Error reading obj file (Normals) in line : " + line);
                                    }
                                    Vector3d vector2 = new Vector3d(0.0, 0.0, 0.0);
                                    double.TryParse(strArrayRead[1], NumberStyles.Float | NumberStyles.AllowThousands, (IFormatProvider)null, out vector2.X);
                                    double.TryParse(strArrayRead[2], NumberStyles.Float | NumberStyles.AllowThousands, (IFormatProvider)null, out vector2.Y);
                                    double.TryParse(strArrayRead[3], NumberStyles.Float | NumberStyles.AllowThousands, (IFormatProvider)null, out vector2.Z);
                                    //vector2.NormalizeNew();
                                    myNewModel.Normals.Add(vector2);
                                    break;

                                case "f":
                                    Triangle a = helper_ReadTriangle(strArrayRead, myNewModel);
                                    p.Triangles.Add(a);
                                    break;
                                case "g":
                                    if (p.Triangles.Count > 0)
                                    {
                                        p.ColorOverall = myNewModel.TextureBitmap != null ? new Vector3d(1.0, 1.0, 1.0) : new Vector3d(0.3 * Math.Cos((double)(23 * myNewModel.Parts.Count)) + 0.5, 0.5 * Math.Cos((double)(17 * myNewModel.Parts.Count + 1)) + 0.5, 0.5 * Math.Cos((double)myNewModel.Parts.Count) + 0.5);
                                        myNewModel.Parts.Add(new Part(p));
                                    }
                                    if (strArrayRead.Length > 1)
                                        p.Name = str1.Replace(strArrayRead[1], "");
                                    p.Triangles.Clear();
                                    break;
                            }
                        }
                    }
                    if (p.Triangles.Count > 0)
                    {
                        p.ColorOverall = new Vector3d(1.0, 1.0, 1.0);
                        myNewModel.Parts.Add(p);
                    }
                    else
                    {
                        AssignTriangleAndPartFromVertex(myNewModel);
                    }
                    streamReader.Close();
                    myNewModel.Helper_AdaptNormalsForEachVertex();
                    myNewModel.CalculateBoundingBox(true);
                }
            }
            catch (Exception err)
            {
                System.Windows.Forms.MessageBox.Show("Error reading obj file (general): " + line + " ; " + err.Message);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strArrayRead"></param>
        /// <param name="myNewModel"></param>
        /// <returns></returns>
        private static Triangle helper_ReadTriangle(string[] strArrayRead, Model3D myNewModel)
        {
            try
            {
                Triangle a = new Triangle();
                
                foreach (string str2 in strArrayRead)
                {
                    if (str2.ToLower() != "f")
                    {
                        try
                        {
                            // the index vertex starts with 1 !!
                            string[] strArray2 = str2.Split('/');
                            int result;
                            int.TryParse(strArray2[0], out result);
                            a.IndVertices.Add(result - 1);
                            //a.IndVertices.Add(result);
                            int indVertex = a.IndVertices.Count - 1;
                            int indPart = myNewModel.Parts.Count - 1;
                            if (indPart < 0)
                                indPart = 0;
                            myNewModel.vertexList[a.IndVertices[indVertex]].IndTriangles.Add(indVertex);
                            myNewModel.vertexList[a.IndVertices[indVertex]].IndPart.Add(indPart);
                            if (strArray2.Length > 2)
                            {
                                int.TryParse(strArray2[strArray2.Length - 1], out result);
                                int index2 = result - 1;
                                if (!double.IsNaN(myNewModel.Normals[index2].X))
                                {
                                    a.IndNormals.Add(index2);
                                    int num2 = result - 1;
                                    myNewModel.vertexList[a.IndVertices[indVertex]].IndexNormals.Add(num2);
                                }
                                if (strArray2[strArray2.Length - 2] != "")
                                {
                                    int.TryParse(strArray2[strArray2.Length - 2], out result);
                                    int num2 = result - 1;
                                    a.IndTextures.Add(num2);
                                }
                            }
                        }
                        catch(Exception err1)
                        {
                            System.Windows.Forms.MessageBox.Show("Error reading obj file (triangles)  " + err1.Message);
                            return new Triangle();
                        }
                    }
                }

                return a;
            }
            catch(Exception err)
            {
                System.Windows.Forms.MessageBox.Show("Error reading obj file (triangles)  " + err.Message);
                return new Triangle();
            }

        }

     
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sr"></param>
        private void read3DFace(StreamReader sr)
        {
            Vertex[] vertexArray = this.readCoordinates(sr);
            Triangle a = new Triangle();
           
            for (int index = 0; index < 4; ++index)
            {
                if (index < 3 || (vertexArray[2].Vector.X != vertexArray[3].Vector.X || vertexArray[2].Vector.Y != vertexArray[3].Vector.Y || vertexArray[2].Vector.Z != vertexArray[3].Vector.Z))
                {
                    vertexArray[index].IndTriangles.Add(this.Parts[this.Parts.Count - 1].Triangles.Count);
                    vertexArray[index].IndPart.Add(this.Parts.Count - 1);
                    a.IndVertices.Add(this.vertexList.Count);
                    this.vertexList.Add(vertexArray[index]);
                }
            }
            this.Parts[this.Parts.Count - 1].Triangles.Add(a);
            
        }

        private void readPolyline(StreamReader sr)
        {
            string str = "";
            int count = this.vertexList.Count;
            while (str.ToUpper() != "SEQEND" && !sr.EndOfStream)
            {
                str = GeneralSettings.TreatLanguageSpecifics(sr.ReadLine().Trim());
                if (str.ToUpper() == "VERTEX")
                {
                    Vertex vertex = this.lerCoord(sr);
                    while (str != "0" && !sr.EndOfStream)
                    {
                        str = GeneralSettings.TreatLanguageSpecifics(sr.ReadLine().Trim());
                        if (str == "70")
                        {
                            str = GeneralSettings.TreatLanguageSpecifics(sr.ReadLine().Trim());
                            Triangle a;
                            if (str == "128")
                            {
                                a = new Triangle();
                                
                                bool flag = true;
                                while (str != "0")
                                {
                                    flag = !flag;
                                    str = sr.ReadLine().Trim();
                                    if (flag && str != "0")
                                    {
                                        str = GeneralSettings.TreatLanguageSpecifics(str);
                                        int result;
                                        int.TryParse(str, out result);
                                        if (result > 0)
                                        {
                                            a.IndVertices.Add(result - 1 + count);
                                            this.vertexList[result - 1].IndPart.Add(this.Parts.Count - 1);
                                            this.vertexList[result - 1].IndTriangles.Add(this.Parts[this.Parts.Count - 1].Triangles.Count);
                                        }
                                    }
                                }
                                if (a.IndVertices.Count >= 3)
                                {
                                    this.Parts[this.Parts.Count - 1].Triangles.Add(a);
                                    
                                }
                            }
                            else if (str == "32")
                            {
                                this.vertexList.Add(vertex);
                                if (this.vertexList.Count - count > 3)
                                {
                                    a = new Triangle();
                                  
                                    a.IndVertices.Add(this.vertexList.Count - 1);
                                    a.IndVertices.Add(this.vertexList.Count - 2);
                                    a.IndVertices.Add(this.vertexList.Count - 3);
                                    this.Parts[this.Parts.Count - 1].Triangles.Add(a);
                                    
                                }
                            }
                            else
                                this.vertexList.Add(vertex);
                        }
                    }
                }
            }
        }

        private Vertex[] readCoordinates(StreamReader sr)
        {
            int num = 0;
            Vertex[] vertexArray = new Vertex[4];
            for (int index = 0; index < 4; ++index)
            {
                vertexArray[index] = new Vertex();
                vertexArray[index].Vector = new Vector3d();
                vertexArray[index].IndPart = new List<int>();
                vertexArray[index].IndexNormals = new List<int>();
                vertexArray[index].IndTriangles = new List<int>();
            }
            while (num < 12 && !sr.EndOfStream)
            {
                string s = GeneralSettings.TreatLanguageSpecifics(sr.ReadLine().Trim());
                if (s == "10")
                {
                    s = GeneralSettings.TreatLanguageSpecifics(sr.ReadLine().Trim());
                    double.TryParse(s, out vertexArray[0].Vector.X);
                    ++num;
                }
                else if (s == "11")
                {
                    s = GeneralSettings.TreatLanguageSpecifics(sr.ReadLine().Trim());
                    double.TryParse(s, out vertexArray[1].Vector.X);
                    ++num;
                }
                else if (s == "12")
                {
                    s = GeneralSettings.TreatLanguageSpecifics(sr.ReadLine().Trim());
                    double.TryParse(s, out vertexArray[2].Vector.X);
                    ++num;
                }
                else if (s == "13")
                {
                    s = GeneralSettings.TreatLanguageSpecifics(sr.ReadLine().Trim());
                    double.TryParse(s, out vertexArray[3].Vector.X);
                    ++num;
                }
                if (s == "20")
                {
                    s = GeneralSettings.TreatLanguageSpecifics(sr.ReadLine().Trim());
                    double.TryParse(s, out vertexArray[0].Vector.Y);
                    ++num;
                }
                else if (s == "21")
                {
                    s = GeneralSettings.TreatLanguageSpecifics(sr.ReadLine().Trim());
                    double.TryParse(s, out vertexArray[1].Vector.Y);
                    ++num;
                }
                else if (s == "22")
                {
                    s = GeneralSettings.TreatLanguageSpecifics(sr.ReadLine().Trim());
                    double.TryParse(s, out vertexArray[2].Vector.Y);
                    ++num;
                }
                else if (s == "23")
                {
                    s = GeneralSettings.TreatLanguageSpecifics(sr.ReadLine().Trim());
                    double.TryParse(s, out vertexArray[3].Vector.Y);
                    ++num;
                }
                if (s == "30")
                {
                    double.TryParse(GeneralSettings.TreatLanguageSpecifics(sr.ReadLine().Trim()), out vertexArray[0].Vector.Z);
                    ++num;
                }
                else if (s == "31")
                {
                    double.TryParse(GeneralSettings.TreatLanguageSpecifics(sr.ReadLine().Trim()), out vertexArray[1].Vector.Z);
                    ++num;
                }
                else if (s == "32")
                {
                    double.TryParse(GeneralSettings.TreatLanguageSpecifics(sr.ReadLine().Trim()), out vertexArray[2].Vector.Z);
                    ++num;
                }
                else if (s == "33")
                {
                    double.TryParse(GeneralSettings.TreatLanguageSpecifics(sr.ReadLine().Trim()), out vertexArray[3].Vector.Z);
                    ++num;
                }
            }
            return vertexArray;
        }

        private Vertex lerCoord(StreamReader sr)
        {
            int num = 0;
            Vertex vertex = new Vertex();
            vertex.Vector = new Vector3d();
            vertex.IndPart = new List<int>();
            vertex.IndexNormals = new List<int>();
            vertex.IndTriangles = new List<int>();
            while (num < 3 && !sr.EndOfStream)
            {
                string str = GeneralSettings.TreatLanguageSpecifics(sr.ReadLine().Trim());
                if (str == "10")
                {
                    double.TryParse(GeneralSettings.TreatLanguageSpecifics(sr.ReadLine().Trim()), out vertex.Vector.X);
                    ++num;
                }
                else if (str == "20")
                {
                    double.TryParse(GeneralSettings.TreatLanguageSpecifics(sr.ReadLine().Trim()), out vertex.Vector.Y);
                    ++num;
                }
                else if (str == "30")
                {
                    double.TryParse(GeneralSettings.TreatLanguageSpecifics(sr.ReadLine().Trim()), out vertex.Vector.Z);
                    ++num;
                }
            }
            return vertex;
        }

        private void GetTexture(string TexFile, string OBJFile)
        {
            try
            {
                string[] strArray1 = OBJFile.Split('\\');
                string str = "";
                TexFile = TexFile.Replace(",", ".");
                for (int index = strArray1.Length - 2; index >= 0; --index)
                    str = strArray1[index] + "\\" + str;
                using (StreamReader streamReader = new StreamReader(str + TexFile))
                {
                    while (!streamReader.EndOfStream)
                    {
                        string linha = streamReader.ReadLine().Trim();
                        while (linha.EndsWith("\\"))
                            linha = linha.Substring(0, linha.Length - 1) + streamReader.ReadLine().Trim();
                        string[] strArray2 = GeneralSettings.TreatLanguageSpecifics(linha).Split();
                        if (strArray2[0].ToLower() == "map_kd")
                            this.TextureBitmap = new System.Drawing.Bitmap(str + strArray2[1].Replace(",", "."));
                    }
                    streamReader.Close();
                }
            }
            catch
            {
            }
        }
    }
}
