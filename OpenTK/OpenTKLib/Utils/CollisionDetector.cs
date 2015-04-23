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
using System;
using System.Windows.Forms;

namespace OpenTKLib
{
  public class CollisionDetector
  {
    private CLCalc.Program.Kernel kernelCalcRotacoes;
    private CLCalc.Program.Kernel kernelCalcTransl;
    private CLCalc.Program.Kernel kernelCalcExactCollision;
    private CLCalc.Program.Kernel kernelCalcVertexCollision;

    public CollisionDetector()
    {
      if (CLCalc.CLAcceleration == CLCalc.CLAccelerationType.Unknown)
      {
        try
        {
          CLCalc.InitCL();
        }
        catch
        {
        }
      }
      try
      {
        if (CLCalc.CLAcceleration != CLCalc.CLAccelerationType.UsingCL)
          return;
        CollisionDetector.DisplacementSource displacementSource = new CollisionDetector.DisplacementSource();
        CollisionDetector.ExactCollisionSource exactCollisionSource = new CollisionDetector.ExactCollisionSource();
        CollisionDetector.VertexCollisionSource vertexCollisionSource = new CollisionDetector.VertexCollisionSource();
        CLCalc.Program.Compile(new string[4]
        {
          displacementSource.srcCalcRotacoes,
          displacementSource.srcCalcTransl,
          exactCollisionSource.srcExactCollision,
          vertexCollisionSource.srcVertexCollision
        });
        this.kernelCalcRotacoes = new CLCalc.Program.Kernel("CalcRotacoes");
        this.kernelCalcTransl = new CLCalc.Program.Kernel("CalcTransl");
        this.kernelCalcExactCollision = new CLCalc.Program.Kernel("CalcExactCollision");
        this.kernelCalcVertexCollision = new CLCalc.Program.Kernel("CalcVertexCollision");
      }
      catch (Exception ex)
      {
        int num = (int) MessageBox.Show(ex.ToString());
      }
    }

    private void CalcRotMatrix(float[] RotMatrix, float[] Displacement)
    {
      float num1 = Displacement[5];
      float num2 = Displacement[4];
      float num3 = Displacement[3];
      double num4 = Math.Cos((double) num2);
      double num5 = Math.Sin((double) num2);
      double num6 = Math.Cos((double) num3);
      double num7 = Math.Sin((double) num3);
      double num8 = Math.Cos((double) num1);
      double num9 = Math.Sin((double) num1);
      RotMatrix[0] = (float) (num4 * num8);
      RotMatrix[3] = (float) (-num6 * num9 + num7 * num5 * num8);
      RotMatrix[6] = (float) (num7 * num9 + num6 * num5 * num8);
      RotMatrix[1] = (float) (num4 * num9);
      RotMatrix[4] = (float) (num6 * num8 + num7 * num5 * num9);
      RotMatrix[7] = (float) (-num7 * num8 + num6 * num5 * num9);
      RotMatrix[2] = (float) -num5;
      RotMatrix[5] = (float) (num7 * num4);
      RotMatrix[8] = (float) (num6 * num4);
    }

    private void CalcRotacoes(float[] rotM, float[] V, float[] DisplacedVertexes)
    {
      for (int index1 = 0; index1 < V.Length; ++index1)
      {
        int num = index1 / 3;
        int index2 = index1 - 3 * num;
        DisplacedVertexes[index1] = (float) ((double) V[3 * num] * (double) rotM[index2] + (double) V[3 * num + 1] * (double) rotM[index2 + 3] + (double) V[3 * num + 2] * (double) rotM[index2 + 6]);
      }
    }

    private void CalcTransl(float[] Displacement, float[] DisplacedVertexes)
    {
      for (int index1 = 0; index1 < DisplacedVertexes.Length; ++index1)
      {
        int num = index1 / 3;
        int index2 = index1 - 3 * num;
        DisplacedVertexes[index1] += Displacement[index2];
      }
    }

    private float[] CalcVertexDisplacement(float[] ObjVertex, float[] ObjDisplacement)
    {
      float[] DisplacedVertexes = new float[ObjVertex.Length];
      float[] numArray = new float[9];
      for (int index = 0; index < ObjVertex.Length; ++index)
        DisplacedVertexes[index] = ObjVertex[index];
      if ((double) ObjDisplacement[3] != 0.0 || (double) ObjDisplacement[4] != 0.0 || (double) ObjDisplacement[5] != 0.0)
      {
        this.CalcRotMatrix(numArray, ObjDisplacement);
        this.CalcRotacoes(numArray, ObjVertex, DisplacedVertexes);
      }
      this.CalcTransl(ObjDisplacement, DisplacedVertexes);
      return DisplacedVertexes;
    }

    private CLCalc.Program.Variable CLCalcVertexDisplacement(float[] ObjVertex, float[] ObjDisplacement)
    {
      CLCalc.Program.Variable variable1 = new CLCalc.Program.Variable(ObjVertex);
      CLCalc.Program.Variable variable2 = new CLCalc.Program.Variable(ObjDisplacement);
      CLCalc.Program.Variable variable3 = new CLCalc.Program.Variable(ObjVertex);
      float[] numArray = new float[9];
      CLCalc.Program.Variable variable4 = new CLCalc.Program.Variable(numArray);
      int[] GlobalWorkSize = new int[1]
      {
        ObjVertex.Length
      };
      if ((double) ObjDisplacement[3] != 0.0 || (double) ObjDisplacement[4] != 0.0 || (double) ObjDisplacement[5] != 0.0)
      {
        this.CalcRotMatrix(numArray, ObjDisplacement);
        variable4.WriteToDevice(numArray);
        this.kernelCalcRotacoes.Execute((CLCalc.Program.MemoryObject[]) new CLCalc.Program.Variable[3]
        {
          variable4,
          variable1,
          variable3
        }, GlobalWorkSize);
      }
      this.kernelCalcTransl.Execute((CLCalc.Program.MemoryObject[]) new CLCalc.Program.Variable[2]
      {
        variable2,
        variable3
      }, GlobalWorkSize);
      return variable3;
    }

    public int[] PerVertexDetectCollisions(CollisionDetector.CollisionObject MovingObj, CollisionDetector.CollisionObject Obj, float CollisionDistance, out float[] MovingObjDistances)
    {
      if ((double) CollisionDistance < 0.0)
        throw new Exception("Collision distance should be positive");
      float[] numArray1 = new float[MovingObj.Vertex.Length];
      float[] numArray2 = new float[Obj.Vertex.Length];
      float[] numArray3 = new float[9];
      for (int index = 0; index < MovingObj.Vertex.Length; ++index)
        numArray1[index] = MovingObj.Vertex[index];
      for (int index = 0; index < Obj.Vertex.Length; ++index)
        numArray2[index] = Obj.Vertex[index];
      if ((double) MovingObj.Displacement[3] != 0.0 || (double) MovingObj.Displacement[4] != 0.0 || (double) MovingObj.Displacement[5] != 0.0)
      {
        this.CalcRotMatrix(numArray3, MovingObj.Displacement);
        this.CalcRotacoes(numArray3, MovingObj.Vertex, numArray1);
      }
      if ((double) Obj.Displacement[3] != 0.0 || (double) Obj.Displacement[4] != 0.0 || (double) Obj.Displacement[5] != 0.0)
      {
        this.CalcRotMatrix(numArray3, Obj.Displacement);
        this.CalcRotacoes(numArray3, Obj.Vertex, numArray2);
      }
      this.CalcTransl(MovingObj.Displacement, numArray1);
      this.CalcTransl(Obj.Displacement, numArray2);
      int[] MovingObjCollisions = new int[MovingObj.Vertex.Length / 3];
      MovingObjDistances = new float[MovingObj.Vertex.Length / 3];
      for (int index = 0; index < MovingObjCollisions.Length; ++index)
      {
        MovingObjCollisions[index] = -1;
        MovingObjDistances[index] = CollisionDistance * 2f;
      }
      float[] CollisionDistance1 = new float[1]
      {
        CollisionDistance
      };
      float[] N = new float[Obj.Triangle.Length];
      this.CalcVectors(numArray2, Obj.Triangle, N);
      this.CalcCollision(CollisionDistance1, numArray1, numArray2, Obj.Triangle, N, MovingObjCollisions, MovingObjDistances);
      return MovingObjCollisions;
    }

    private void CalcVectors(float[] ObjVertexes, int[] ObjTriangles, float[] N)
    {
      int num = ObjTriangles.Length / 3;
      for (int index1 = 0; index1 < num; ++index1)
      {
        int index2 = 3 * index1;
        floatVector floatVector1 = new floatVector(ObjVertexes[3 * ObjTriangles[index2]], ObjVertexes[3 * ObjTriangles[index2] + 1], ObjVertexes[3 * ObjTriangles[index2] + 2]);
        floatVector floatVector2 = new floatVector(ObjVertexes[3 * ObjTriangles[index2 + 1]], ObjVertexes[3 * ObjTriangles[index2 + 1] + 1], ObjVertexes[3 * ObjTriangles[index2 + 1] + 2]);
        floatVector floatVector3d = new floatVector(ObjVertexes[3 * ObjTriangles[index2 + 2]], ObjVertexes[3 * ObjTriangles[index2 + 2] + 1], ObjVertexes[3 * ObjTriangles[index2 + 2] + 2]);
        floatVector v1 = floatVector2 - floatVector1;
        floatVector v2 = floatVector3d - floatVector1;
        floatVector floatVector4 = floatVector.CrossProduct(v1, v2);
        floatVector4.normalize();
        v1.normalize();
        v2.normalize();
        N[index2] = floatVector4.x;
        N[index2 + 1] = floatVector4.y;
        N[index2 + 2] = floatVector4.z;
      }
    }

    private void CalcCollision(float[] CollisionDistance, float[] MovingObjVertexes, float[] ObjVertexes, int[] ObjTriangles, float[] N, int[] MovingObjCollisions, float[] MovingObjDistances)
    {
      int num1 = MovingObjVertexes.Length / 3;
      int num2 = ObjTriangles.Length / 3;
      for (int index1 = 0; index1 < num1; ++index1)
      {
        for (int index2 = 0; index2 < num2; ++index2)
        {
          floatVector floatVector1 = new floatVector(MovingObjVertexes[3 * index1], MovingObjVertexes[3 * index1 + 1], MovingObjVertexes[3 * index1 + 2]);
          int index3 = 3 * index2;
          floatVector floatVector2 = new floatVector(ObjVertexes[3 * ObjTriangles[index3]], ObjVertexes[3 * ObjTriangles[index3] + 1], ObjVertexes[3 * ObjTriangles[index3] + 2]);
          floatVector v1 = floatVector1 - floatVector2;
          floatVector v2 = new floatVector(N[index3], N[index3 + 1], N[index3 + 2]);
          if ((double) v2.x != 0.0 || (double) v2.y != 0.0 || (double) v2.y != 0.0)
          {
            float num3 = floatVector.DotProduct(v1, v2);
            if ((double) Math.Abs(num3) <= (double) CollisionDistance[0] && MovingObjCollisions[index1] < 0)
            {
              floatVector floatVector3d = new floatVector(ObjVertexes[3 * ObjTriangles[index3 + 1]], ObjVertexes[3 * ObjTriangles[index3 + 1] + 1], ObjVertexes[3 * ObjTriangles[index3 + 1] + 2]);
              floatVector floatVector4 = new floatVector(ObjVertexes[3 * ObjTriangles[index3 + 2]], ObjVertexes[3 * ObjTriangles[index3 + 2] + 1], ObjVertexes[3 * ObjTriangles[index3 + 2] + 2]);
              floatVector floatVector5 = floatVector3d - floatVector2;
              floatVector floatVector6 = floatVector4 - floatVector2;
              floatVector floatVector7 = v1 - -num3 * v2;
              float num4;
              float num5;
              float num6;
              float num7;
              float num8;
              float num9;
              if ((double) Math.Abs(v2.x) >= (double) Math.Abs(v2.y) && (double) Math.Abs(v2.x) >= (double) Math.Abs(v2.z))
              {
                num4 = floatVector7.z;
                num5 = floatVector7.y;
                num6 = floatVector5.z;
                num7 = floatVector5.y;
                num8 = floatVector6.z;
                num9 = floatVector6.y;
              }
              else if ((double) Math.Abs(v2.y) >= (double) Math.Abs(v2.x) && (double) Math.Abs(v2.y) >= (double) Math.Abs(v2.z))
              {
                num4 = floatVector7.x;
                num5 = floatVector7.z;
                num6 = floatVector5.x;
                num7 = floatVector5.z;
                num8 = floatVector6.x;
                num9 = floatVector6.z;
              }
              else
              {
                num4 = floatVector7.x;
                num5 = floatVector7.y;
                num6 = floatVector5.x;
                num7 = floatVector5.y;
                num8 = floatVector6.x;
                num9 = floatVector6.y;
              }
              float num10 = (float) (1.0 / ((double) num6 * (double) num9 - (double) num7 * (double) num8));
              float num11 = (float) ((double) num9 * (double) num4 - (double) num8 * (double) num5);
              float num12 = (float) (-(double) num7 * (double) num4 + (double) num6 * (double) num5);
              float num13 = num11 * num10;
              float num14 = num12 * num10;
              if ((double) num13 + (double) num14 <= 1.0 && (double) num13 >= 0.0 && (double) num14 >= 0.0)
              {
                MovingObjCollisions[index1] = index2;
                MovingObjDistances[index1] = num3;
              }
            }
          }
          if (MovingObjCollisions[index1] >= 0)
            index2 = num2;
        }
      }
    }

    public bool DetectCollisions(CollisionDetector.CollisionObject Obj1, CollisionDetector.CollisionObject Obj2)
    {
      if (CLCalc.CLAcceleration == CLCalc.CLAccelerationType.UsingCL)
      {
        CLCalc.Program.Variable variable1 = this.CLCalcVertexDisplacement(Obj1.Vertex, Obj1.Displacement);
        CLCalc.Program.Variable variable2 = this.CLCalcVertexDisplacement(Obj2.Vertex, Obj2.Displacement);
        int[] Values = new int[1];
        CLCalc.Program.Variable variable3 = new CLCalc.Program.Variable(Values);
        CLCalc.Program.Variable variable4 = new CLCalc.Program.Variable(Obj1.Triangle);
        CLCalc.Program.Variable variable5 = new CLCalc.Program.Variable(Obj2.Triangle);
        int[] GlobalWorkSize = new int[2]
        {
          Obj1.Triangle.Length / 3,
          Obj2.Triangle.Length / 3
        };
        this.kernelCalcExactCollision.Execute((CLCalc.Program.MemoryObject[]) new CLCalc.Program.Variable[5]
        {
          variable1,
          variable4,
          variable2,
          variable5,
          variable3
        }, GlobalWorkSize);
        variable3.ReadFromDeviceTo(Values);
        return Values[0] > 0;
      }
      else
      {
        float[] Obj1Vertexes = this.CalcVertexDisplacement(Obj1.Vertex, Obj1.Displacement);
        float[] Obj2Vertexes = this.CalcVertexDisplacement(Obj2.Vertex, Obj2.Displacement);
        int[] numCollisions = new int[1];
        this.CalcExactCollision(Obj1Vertexes, Obj1.Triangle, Obj2Vertexes, Obj2.Triangle, numCollisions);
        return numCollisions[0] > 0;
      }
    }

    private void CalcExactCollision(float[] Obj1Vertexes, int[] Obj1Triangles, float[] Obj2Vertexes, int[] Obj2Triangles, int[] numCollisions)
    {
      int num1 = Obj1Triangles.Length / 3;
      int num2 = Obj2Triangles.Length / 3;
      for (int index1 = 0; index1 < num1; ++index1)
      {
        for (int index2 = 0; index2 < num2; ++index2)
        {
          int index3 = 3 * index1;
          int index4 = 3 * index2;
          floatVector floatVector1 = new floatVector(Obj1Vertexes[3 * Obj1Triangles[index3]], Obj1Vertexes[3 * Obj1Triangles[index3] + 1], Obj1Vertexes[3 * Obj1Triangles[index3] + 2]);
          floatVector floatVector2 = new floatVector(Obj1Vertexes[3 * Obj1Triangles[index3 + 1]], Obj1Vertexes[3 * Obj1Triangles[index3 + 1] + 1], Obj1Vertexes[3 * Obj1Triangles[index3 + 1] + 2]);
          floatVector floatVector3 = new floatVector(Obj1Vertexes[3 * Obj1Triangles[index3 + 2]], Obj1Vertexes[3 * Obj1Triangles[index3 + 2] + 1], Obj1Vertexes[3 * Obj1Triangles[index3 + 2] + 2]);
          floatVector floatVector4 = new floatVector(Obj2Vertexes[3 * Obj2Triangles[index4]], Obj2Vertexes[3 * Obj2Triangles[index4] + 1], Obj2Vertexes[3 * Obj2Triangles[index4] + 2]);
          floatVector floatVector5 = new floatVector(Obj2Vertexes[3 * Obj2Triangles[index4 + 1]], Obj2Vertexes[3 * Obj2Triangles[index4 + 1] + 1], Obj2Vertexes[3 * Obj2Triangles[index4 + 1] + 2]);
          floatVector floatVector6 = new floatVector(Obj2Vertexes[3 * Obj2Triangles[index4 + 2]], Obj2Vertexes[3 * Obj2Triangles[index4 + 2] + 1], Obj2Vertexes[3 * Obj2Triangles[index4 + 2] + 2]);
          if (numCollisions[0] <= 0 && this.RetaCruzaTriangulo(floatVector1, floatVector2, floatVector4, floatVector5, floatVector6))
            ++numCollisions[0];
          if (numCollisions[0] <= 0 && this.RetaCruzaTriangulo(floatVector1, floatVector3, floatVector4, floatVector5, floatVector6))
            ++numCollisions[0];
          if (numCollisions[0] <= 0 && this.RetaCruzaTriangulo(floatVector2, floatVector3, floatVector4, floatVector5, floatVector6))
            ++numCollisions[0];
          if (numCollisions[0] <= 0 && this.RetaCruzaTriangulo(floatVector4, floatVector5, floatVector1, floatVector2, floatVector3))
            ++numCollisions[0];
          if (numCollisions[0] <= 0 && this.RetaCruzaTriangulo(floatVector4, floatVector6, floatVector1, floatVector2, floatVector3))
            ++numCollisions[0];
        }
      }
    }

    private bool RetaCruzaTriangulo(floatVector A, floatVector B, floatVector C, floatVector D, floatVector E)
    {
      floatVector floatVector1 = B - A;
      floatVector floatVector2 = D - C;
      floatVector floatVector3 = E - C;
      float num1 = (float) ((double) floatVector2.x * (double) floatVector3.y * (double) floatVector1.z - (double) floatVector2.x * (double) floatVector1.y * (double) floatVector3.z - (double) floatVector2.y * (double) floatVector3.x * (double) floatVector1.z + (double) floatVector2.y * (double) floatVector1.x * (double) floatVector3.z + (double) floatVector2.z * (double) floatVector3.x * (double) floatVector1.y - (double) floatVector2.z * (double) floatVector1.x * (double) floatVector3.y);
      if ((double) num1 == 0.0)
        return false;
      float num2 = -1f / num1;
      float[,] numArray = new float[3, 3]
      {
        {
          (float) (-(double) floatVector3.y * (double) floatVector1.z + (double) floatVector3.z * (double) floatVector1.y),
          (float) (-(double) floatVector3.z * (double) floatVector1.x + (double) floatVector1.z * (double) floatVector3.x),
          (float) ((double) floatVector3.y * (double) floatVector1.x - (double) floatVector3.x * (double) floatVector1.y)
        },
        {
          (float) ((double) floatVector2.y * (double) floatVector1.z - (double) floatVector2.z * (double) floatVector1.y),
          (float) ((double) floatVector2.z * (double) floatVector1.x - (double) floatVector1.z * (double) floatVector2.x),
          (float) (-(double) floatVector2.y * (double) floatVector1.x + (double) floatVector2.x * (double) floatVector1.y)
        },
        {
          (float) ((double) floatVector2.y * (double) floatVector3.z - (double) floatVector2.z * (double) floatVector3.y),
          (float) ((double) floatVector2.z * (double) floatVector3.x - (double) floatVector3.z * (double) floatVector2.x),
          (float) (-(double) floatVector2.y * (double) floatVector3.x + (double) floatVector2.x * (double) floatVector3.y)
        }
      };
      float num3 = (float) ((double) numArray[0, 0] * ((double) A.x - (double) C.x) + (double) numArray[0, 1] * ((double) A.y - (double) C.y) + (double) numArray[0, 2] * ((double) A.z - (double) C.z));
      float num4 = (float) ((double) numArray[1, 0] * ((double) A.x - (double) C.x) + (double) numArray[1, 1] * ((double) A.y - (double) C.y) + (double) numArray[1, 2] * ((double) A.z - (double) C.z));
      float num5 = (float) ((double) numArray[2, 0] * ((double) A.x - (double) C.x) + (double) numArray[2, 1] * ((double) A.y - (double) C.y) + (double) numArray[2, 2] * ((double) A.z - (double) C.z));
      float num6 = num3 * num2;
      float num7 = num4 * num2;
      float num8 = num5 * num2;
      return 0.0 <= (double) num8 && (double) num8 <= 1.0 && ((double) num6 >= 0.0 && (double) num7 >= 0.0) && (double) num6 + (double) num7 <= 1.0;
    }

    public int[] VertexCollision(float[] v1, float[] displacement1, float[] v2, float[] displacement2, float CollisionDistance, out float[] VertexDistances)
    {
      if ((double) CollisionDistance < 0.0)
        throw new Exception("Collision distance should be positive");
      int[] numArray1 = new int[v1.Length / 3];
      VertexDistances = new float[v1.Length / 3];
      for (int index = 0; index < numArray1.Length; ++index)
      {
        numArray1[index] = -1;
        VertexDistances[index] = -1f;
      }
      float[] numArray2 = new float[1]
      {
        CollisionDistance
      };
      if (CLCalc.CLAcceleration == CLCalc.CLAccelerationType.UsingCL && (long) v1.Length * (long) v2.Length > 10000000L)
      {
        CLCalc.Program.Variable variable1 = this.CLCalcVertexDisplacement(v1, displacement1);
        CLCalc.Program.Variable variable2 = this.CLCalcVertexDisplacement(v2, displacement2);
        CLCalc.Program.Variable variable3 = new CLCalc.Program.Variable(numArray2);
        CLCalc.Program.Variable variable4 = new CLCalc.Program.Variable(numArray1);
        CLCalc.Program.Variable variable5 = new CLCalc.Program.Variable(VertexDistances);
        this.kernelCalcVertexCollision.Execute((CLCalc.Program.MemoryObject[]) new CLCalc.Program.Variable[5]
        {
          variable3,
          variable1,
          variable2,
          variable4,
          variable5
        }, new int[2]
        {
          v1.Length / 3,
          v2.Length / 3
        });
        variable4.ReadFromDeviceTo(numArray1);
        variable5.ReadFromDeviceTo(VertexDistances);
      }
      else
      {
        float[] v1_1 = this.CalcVertexDisplacement(v1, displacement1);
        float[] v2_1 = this.CalcVertexDisplacement(v2, displacement2);
        this.CalcVertexCollisions(numArray2, v1_1, v2_1, numArray1, VertexDistances);
      }
      return numArray1;
    }

    private void CalcVertexCollisions(float[] CollisionDistance, float[] v1, float[] v2, int[] Collisions, float[] VertexDistances)
    {
      int num1 = v1.Length / 3;
      int num2 = v2.Length / 3;
      for (int index1 = 0; index1 < num1; ++index1)
      {
        for (int index2 = 0; index2 < num2; ++index2)
        {
          int index3 = 3 * index1;
          if (Collisions[index3] < 0)
          {
            int index4 = 3 * index2;
            float num3 = (new floatVector(v1[index3], v1[index3 + 1], v1[index3 + 2]) - new floatVector(v2[index4], v2[index4 + 1], v2[index4 + 2])).norm();
            if ((double) num3 <= (double) CollisionDistance[0])
            {
              Collisions[index3] = index4;
              VertexDistances[index3] = num3;
            }
            if ((double) num3 < (double) VertexDistances[index3] || (double) VertexDistances[index3] < 0.0)
              VertexDistances[index3] = num3;
          }
        }
      }
    }

    public class CollisionObject
    {
      public float[] Vertex = new float[0];
      public int[] Triangle = new int[0];
      public float[] Displacement = new float[6];

      public CollisionObject(int nVertexes, float[] Vertexes, int nTriangles, int[] Triangles)
      {
        if (Vertexes.Length != 3 * nVertexes)
          throw new Exception("Vertexes Length should be 3*nVertexes");
        if (Triangles.Length != 3 * nTriangles)
          throw new Exception("Triangles Length should be 3*nTriangles");
        this.Vertex = Vertexes;
        this.Triangle = Triangles;
      }

      public void SetDisplacement(float dx, float dy, float dz, float phi, float theta, float psi)
      {
        this.Displacement[0] = dx;
        this.Displacement[1] = dy;
        this.Displacement[2] = dz;
        this.Displacement[3] = phi;
        this.Displacement[4] = theta;
        this.Displacement[5] = psi;
      }

      public static CollisionDetector.CollisionObject BoundingBox(float[] StartPoint, float[] Dimensions)
      {
        float[] Vertexes = new float[24];
        int[] Triangles = new int[36];
        float num1 = StartPoint[0];
        float num2 = StartPoint[1];
        float num3 = StartPoint[2];
        float num4 = StartPoint[0] + Dimensions[0];
        float num5 = StartPoint[1] + Dimensions[1];
        float num6 = StartPoint[2] + Dimensions[2];
        Vertexes[0] = num1;
        Vertexes[1] = num2;
        Vertexes[2] = num3;
        Vertexes[3] = num4;
        Vertexes[4] = num2;
        Vertexes[5] = num3;
        Vertexes[6] = num4;
        Vertexes[7] = num5;
        Vertexes[8] = num3;
        Vertexes[9] = num1;
        Vertexes[10] = num5;
        Vertexes[11] = num3;
        Vertexes[12] = num1;
        Vertexes[13] = num2;
        Vertexes[14] = num6;
        Vertexes[15] = num4;
        Vertexes[16] = num2;
        Vertexes[17] = num6;
        Vertexes[18] = num4;
        Vertexes[19] = num5;
        Vertexes[20] = num6;
        Vertexes[21] = num1;
        Vertexes[22] = num5;
        Vertexes[23] = num6;
        Triangles[0] = 0;
        Triangles[1] = 1;
        Triangles[2] = 2;
        Triangles[3] = 0;
        Triangles[4] = 2;
        Triangles[5] = 3;
        Triangles[6] = 0;
        Triangles[7] = 1;
        Triangles[8] = 5;
        Triangles[9] = 0;
        Triangles[10] = 5;
        Triangles[11] = 4;
        Triangles[12] = 1;
        Triangles[13] = 2;
        Triangles[14] = 6;
        Triangles[15] = 1;
        Triangles[16] = 6;
        Triangles[17] = 5;
        Triangles[18] = 3;
        Triangles[19] = 2;
        Triangles[20] = 6;
        Triangles[21] = 3;
        Triangles[22] = 6;
        Triangles[23] = 7;
        Triangles[24] = 0;
        Triangles[25] = 3;
        Triangles[26] = 7;
        Triangles[27] = 0;
        Triangles[28] = 7;
        Triangles[29] = 4;
        Triangles[30] = 4;
        Triangles[31] = 5;
        Triangles[32] = 6;
        Triangles[33] = 4;
        Triangles[34] = 6;
        Triangles[35] = 7;
        return new CollisionDetector.CollisionObject(8, Vertexes, 12, Triangles);
      }
    }

    private class DisplacementSource
    {
      public string srcCalcRotacoes = "\r\n                        __kernel void\r\n                        CalcRotacoes( __global read_only float * rotM,\r\n                                      __global read_only float * V,\r\n                                      __global           float * DisplacedVertexes)\r\n                        {\r\n                            int k = get_global_id(0);\r\n                            int i = k / 3;\r\n                            int r = k - 3 * i;\r\n                            DisplacedVertexes[k] = V[3 * i] * rotM[r] + V[3 * i + 1] * rotM[r + 3] + V[3 * i + 2] * rotM[r + 6];\r\n                        }\r\n";
      public string srcCalcTransl = "\r\n                            __kernel void\r\n                            CalcTransl( __global read_only float * Displacement,\r\n                                        __global           float * DisplacedVertexes)\r\n                            {\r\n                                int k = get_global_id(0);\r\n\r\n                                int i = k / 3;\r\n                                int r = k - 3 * i;\r\n                                DisplacedVertexes[k] += Displacement[r];\r\n                            }";
    }

    private class ExactCollisionSource
    {
      public string srcExactCollision = "\r\n                            bool RetaCruzaTriangulo(float4 A, float4 B, float4 C, float4 D, float4 E)\r\n                            {\r\n                                //checks if A + k*AB crosses CDE\r\n                                //vector calculations\r\n                                float4 AB = B - A;\r\n                                float4 CD = D - C;\r\n                                float4 CE = E - C;\r\n\r\n                                //match point between plane and line:\r\n                                //A+delta*AB = C+alfa*CD+beta*CE\r\n                                //[CD CE -AB]*[alfa beta delta]t=[A-C]\r\n                                //0<=delta<=1, alfa>=0, beta>=0, alfa+beta<=1\r\n\r\n                                float invdet;\r\n                                invdet = fma(CD.x, (fma(CE.y, AB.z, - AB.y * CE.z)), fma(CD.y, (fma(AB.x, CE.z, - CE.x * AB.z)), CD.z * (fma(CE.x, AB.y, - AB.x * CE.y))));\r\n\r\n                                if (invdet == 0)\r\n                                    return false;\r\n\r\n                                invdet = -1 / invdet;\r\n\r\n                                float M[9];\r\n\r\n                                M[0] = fma(-CE.y, AB.z, CE.z * AB.y); M[1] = fma(CD.y, AB.z, - CD.z * AB.y);M[2] = fma(CD.y, CE.z, - CD.z * CE.y);\r\n                                M[3] = fma(-CE.z, AB.x, AB.z * CE.x); M[4] = fma(CD.z, AB.x, - AB.z * CD.x);M[5] = fma(CD.z, CE.x, - CE.z * CD.x);\r\n                                M[6] = fma(CE.y, AB.x, - CE.x * AB.y);M[7] = fma(-CD.y, AB.x, CD.x * AB.y); M[8] = fma(-CD.y, CE.x, CD.x * CE.y);\r\n\r\n                                \r\n\r\n                                //[alfa=sol.x beta=sol.y delta=sol.z]=M*[A-C]\r\n                                float4 CA = A - C;\r\n                                float4 sol;\r\n                                sol.x = fma(M[0], CA.x, fma(M[3], CA.y, M[6] * CA.z));\r\n                                sol.y = fma(M[1], CA.x, fma(M[4], CA.y, M[7] * CA.z));\r\n                                sol.z = fma(M[2], CA.x, fma(M[5], CA.y, M[8] * CA.z));\r\n\r\n                                //Sistema sobredeterminado\r\n\r\n                                sol *= invdet;\r\n\r\n                                if (0 <= sol.z && sol.z <= 1 && sol.x >= 0 && sol.y >= 0 && sol.x + sol.y <= 1)\r\n                                    return true;\r\n                                else return false;\r\n                            }\r\n                \r\n                            __kernel void\r\n                            CalcExactCollision( __global           float * Obj1Vertexes,\r\n                                                __global           int * Obj1Triangles,\r\n                                                __global           float * Obj2Vertexes,\r\n                                                __global           int * Obj2Triangles,\r\n                                                __global           int * numCollisions)\r\n                            {\r\n                                int i = get_global_id(0);\r\n                                int j = get_global_id(1);\r\n                                int ii = 3 * i, jj = 3 * j;\r\n\r\n                                //Busca vértices\r\n                                float4 A0 = (float4)(Obj1Vertexes[3 * Obj1Triangles[ii]],\r\n                                    Obj1Vertexes[3 * Obj1Triangles[ii] + 1], Obj1Vertexes[3 * Obj1Triangles[ii] + 2], 0);\r\n                                float4 A1 = (float4)(Obj1Vertexes[3 * Obj1Triangles[ii + 1]],\r\n                                    Obj1Vertexes[3 * Obj1Triangles[ii + 1] + 1], Obj1Vertexes[3 * Obj1Triangles[ii + 1] + 2], 0);\r\n                                float4 A2 = (float4)(Obj1Vertexes[3 * Obj1Triangles[ii + 2]],\r\n                                    Obj1Vertexes[3 * Obj1Triangles[ii + 2] + 1], Obj1Vertexes[3 * Obj1Triangles[ii + 2] + 2], 0);\r\n\r\n                                float4 B0 = (float4)(Obj2Vertexes[3 * Obj2Triangles[jj]],\r\n                                    Obj2Vertexes[3 * Obj2Triangles[jj] + 1], Obj2Vertexes[3 * Obj2Triangles[jj] + 2], 0);\r\n                                float4 B1 = (float4)(Obj2Vertexes[3 * Obj2Triangles[jj + 1]],\r\n                                    Obj2Vertexes[3 * Obj2Triangles[jj + 1] + 1], Obj2Vertexes[3 * Obj2Triangles[jj + 1] + 2], 0);\r\n                                float4 B2 = (float4)(Obj2Vertexes[3 * Obj2Triangles[jj + 2]],\r\n                                    Obj2Vertexes[3 * Obj2Triangles[jj + 2] + 1], Obj2Vertexes[3 * Obj2Triangles[jj + 2] + 2], 0);\r\n\r\n                                //Verificacao de colisoes\r\n                                if (numCollisions[0] <= 0)\r\n                                    if (RetaCruzaTriangulo(A0, A1, B0, B1, B2))\r\n                                        numCollisions[0]++;\r\n\r\n                                if (numCollisions[0] <= 0)\r\n                                    if (RetaCruzaTriangulo(A0, A2, B0, B1, B2))\r\n                                        numCollisions[0]++;\r\n\r\n                                if (numCollisions[0] <= 0)\r\n                                    if (RetaCruzaTriangulo(A1, A2, B0, B1, B2))\r\n                                        numCollisions[0]++;\r\n\r\n                                if (numCollisions[0] <= 0)\r\n                                    if (RetaCruzaTriangulo(B0, B1, A0, A1, A2))\r\n                                        numCollisions[0]++;\r\n\r\n                                if (numCollisions[0] <= 0)\r\n                                    if (RetaCruzaTriangulo(B0, B2, A0, A1, A2))\r\n                                        numCollisions[0]++;\r\n\r\n                                //Not necessary. If none of the others have collided this one won't too \r\n                                //if (numCollisions[0] <= 0)\r\n                                //    if (RetaCruzaTriangulo(B1, B2, A0, A1, A2))\r\n                                //        numCollisions[0]++;\r\n                            }";
    }

    private class VertexCollisionSource
    {
      public string srcVertexCollision = "\r\n                \r\n                            __kernel void\r\n                            CalcVertexCollision( __global           float * CollisionDistance,\r\n                                                __global            float * v1,\r\n                                                __global            float * v2,\r\n                                                __global            int * Collisions,\r\n                                                __global            float * VertexDistances)\r\n                            {\r\n                                int i = get_global_id(0);\r\n                                int j = get_global_id(1);\r\n\r\n                                int ii = 3 * i;\r\n                                if (Collisions[ii] < 0)\r\n                                {\r\n                                    int jj = 3 * j;\r\n\r\n                                    float4 vec1 = (float4)(v1[ii], v1[ii + 1], v1[ii + 2], 0);\r\n                                    float4 vec2 = (float4)(v2[jj], v2[jj + 1], v2[jj + 2], 0);\r\n\r\n                                    float dist = distance(vec1, vec2);\r\n\r\n                                    if (dist <= CollisionDistance[0])\r\n                                    {\r\n                                        Collisions[ii] = jj;\r\n                                        VertexDistances[ii] = dist;\r\n                                    }\r\n\r\n                                    if (dist < VertexDistances[ii] || VertexDistances[ii] < 0)\r\n                                    {\r\n                                        VertexDistances[ii] = dist;\r\n                                    }\r\n                                }\r\n\r\n                            }";
    }
  }
}
