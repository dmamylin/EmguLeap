﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using OpenCLTemplate;

namespace OpenCLTemplate.LinearAlgebra
{
    /// <summary>Encapsulates Linear Algebra functions, especially symmetric positive definite matrices.
    /// WARNING: Do not exceed linear systems of size 23000</summary>
    public partial class floatLinalg
    {
        /// <summary>Use OpenCL?</summary>
        public static bool UseOpenCLIfAvailable = true;

        #region Matrices and vector classes definitions and BLAS functions

        /// <summary>BLAS functions</summary>
        public static class BLAS
        {

            #region BLAS 1 - vector ops
            /// <summary>Computer a linear combination alpha*u+beta*v. Puts answer in ans. Creates ans if it is null</summary>
            public static floatVector LinearCombination(float alpha, floatVector u, float beta, floatVector v, ref floatVector ans)
            {
                if (ans == null) ans = new floatVector(new float[u.Length]);
                if (u.Length != v.Length) throw new Exception("Incompatible dimensions");
                if (ans.Length != u.Length) throw new Exception("Ans dimension should be equal to vectors dimension");

                if (UseOpenCLIfAvailable && CLCalc.CLAcceleration == CLCalc.CLAccelerationType.UsingCL)
                {
                    u.CLCoef.WriteToDevice(new float[] { alpha });
                    v.CLCoef.WriteToDevice(new float[] { beta });
                    kernelLinearComb.Execute(new CLCalc.Program.MemoryObject[] { u.CLCoef, v.CLCoef, u.CLValues, v.CLValues, ans.CLValues }, u.Length);
                }
                else
                {
                    for (int i = 0; i < u.Length; i++)
                    {
                        ans.Values[i] = alpha * u.Values[i] + beta * v.Values[i];
                    }
                }

                return ans;
            }

            /// <summary>Computer a linear combination alpha*u+beta*v. Puts answer in ans. Creates ans if it is null</summary>
            public static floatMatrix LinearCombination(float alpha, floatMatrix u, float beta, floatMatrix v, ref floatMatrix ans)
            {
                if (ans == null) ans = new floatMatrix(new float[u.Rows, u.Cols]);
                if (u.Rows != v.Rows || u.Cols != v.Cols) throw new Exception("Incompatible dimensions");
                if (ans.Rows != u.Rows || ans.Cols != u.Cols) throw new Exception("Ans dimension should be equal to vectors dimension");

                if (UseOpenCLIfAvailable && CLCalc.CLAcceleration == CLCalc.CLAccelerationType.UsingCL)
                {
                    u.CLCoef.WriteToDevice(new float[] { alpha });
                    v.CLCoef.WriteToDevice(new float[] { beta });
                    kernelLinearComb.Execute(new CLCalc.Program.MemoryObject[] { u.CLCoef, v.CLCoef, u.CLValues, v.CLValues, ans.CLValues }, u.Values.Length);
                }
                else
                {
                    for (int i = 0; i < u.Values.Length; i++)
                    {
                        ans.Values[i] = alpha * u.Values[i] + beta * v.Values[i];
                    }
                }

                return ans;
            }

            /// <summary>Computes vector dot product</summary>
            /// <param name="u">First vector</param>
            /// <param name="v">Second vector</param>
            /// <param name="temp">Temporary vector to store inner product</param>
            public static float Dot(floatVector u, floatVector v, ref floatVector temp)
            {
                if (u.Length != v.Length) throw new Exception("Vectors should have the same length");
                if (temp == null) temp = new floatVector(new float[u.Length]);
                if (u.Length != temp.Length) throw new Exception("Temp should have the same length as u and v");

                if (UseOpenCLIfAvailable && CLCalc.CLAcceleration == CLCalc.CLAccelerationType.UsingCL)
                {
                    kernelInnerProd.Execute(new CLCalc.Program.MemoryObject[] { u.CLValues, v.CLValues, temp.CLValues }, u.Length);
                }
                else
                {
                    for (int i = 0; i < u.Length; i++) temp.Values[i] = u.Values[i] * v.Values[i];
                }

                return temp.Sum();
            }

            /// <summary>Element-wise multiplication u .* u</summary>
            public static floatVector ElemWiseSquare(floatVector u, ref floatVector ans)
            {
                if (ans == null) ans = new floatVector(new float[u.Length]);
                if (ans.Length != u.Length) throw new Exception("ans and u should have same length");

                if (UseOpenCLIfAvailable && CLCalc.CLAcceleration == CLCalc.CLAccelerationType.UsingCL)
                {
                    kernelElemWiseProd.Execute(new CLCalc.Program.MemoryObject[] { u.CLValues, ans.CLValues }, u.Length);
                }
                else
                {
                    for (int i = 0; i < u.Length; i++)
                    {
                        ans.Values[i] = u.Values[i] * u.Values[i];
                    }
                }
                return ans;
            }

            /// <summary>Element-wise inversion 1 ./ u</summary>
            public static floatVector ElemWiseInv(floatVector u, ref floatVector ans)
            {
                if (ans == null) ans = new floatVector(new float[u.Length]);
                if (ans.Length != u.Length) throw new Exception("ans and u should have same length");

                if (UseOpenCLIfAvailable && CLCalc.CLAcceleration == CLCalc.CLAccelerationType.UsingCL)
                {
                    kernelElemWiseInv.Execute(new CLCalc.Program.MemoryObject[] { u.CLValues, ans.CLValues }, u.Length);
                }
                else
                {
                    for (int i = 0; i < u.Length; i++)
                    {
                        ans.Values[i] = 1.0f / u.Values[i];
                    }
                }
                return ans;
            }

            /// <summary>Element-wise inversion 1 ./ (u.*u)</summary>
            public static floatVector ElemWiseInv2(floatVector u, ref floatVector ans)
            {
                if (ans == null) ans = new floatVector(new float[u.Length]);
                if (ans.Length != u.Length) throw new Exception("ans and u should have same length");

                if (UseOpenCLIfAvailable && CLCalc.CLAcceleration == CLCalc.CLAccelerationType.UsingCL)
                {
                    kernelElemWiseInv2.Execute(new CLCalc.Program.MemoryObject[] { u.CLValues, ans.CLValues }, u.Length);
                }
                else
                {
                    for (int i = 0; i < u.Length; i++)
                    {
                        ans.Values[i] = 1.0f / (u.Values[i] * u.Values[i]);
                    }
                }
                return ans;
            }

            /// <summary>Sums the components of a vector using __local memory and coalesced access</summary>
            /// <param name="CLv">Vector whose components should be summed</param>
            public static float SumVectorElements(floatVector CLv)
            {
                float resp = 0;
                if (UseOpenCLIfAvailable && CLCalc.CLAcceleration == CLCalc.CLAccelerationType.UsingCL)
                {
                    /*
                     The idea here is to create a reduction in which the access pattern to the vectors is coalesced.
                     The first step is to reduce the number of non-summed items to a multiple of NWORKITEMS and then coalesce the access
                     */

                    int LOCALWORKSIZE = Math.Min(256, (int)CLCalc.Program.CommQueues[CLCalc.Program.DefaultCQ].Device.MaxWorkGroupSize);
                    int NWORKITEMS = 16 * LOCALWORKSIZE;

                    int n = CLv.Length;
                    float[] resps = new float[NWORKITEMS];
                    if (CLv.CLresps == null)
                    {
                        CLv.CLresps = new CLCalc.Program.Variable(resps);
                        CLv.CLn = new CLCalc.Program.Variable(new int[1]);
                    }

                    CLv.CLn.WriteToDevice(new int[] { n });
                    CLCalc.Program.Variable[] args = new CLCalc.Program.Variable[] { CLv.CLValues, CLv.CLresps, CLv.CLn };

                    //Write n = k*NWORKITEMS + p. Preprocess to eliminate p`s and leave summation only to a multiple of NWORKITEMS
                    int k = n / NWORKITEMS;
                    int p = n - k * NWORKITEMS;

                    //Clears partial responses
                    kernelClear.Execute(args, NWORKITEMS);

                    //Sums the p last elements into the p first elements
                    kernelPreSum.Execute(args, p);

                    //Use CLn to inform each work-item its workload. Each one will access and sum k numbers
                    CLv.CLn.WriteToDevice(new int[] { k });

                    kernelCoalLocalSum.Execute(args, new int[] { NWORKITEMS }, new int[] { LOCALWORKSIZE });

                    CLv.CLresps.ReadFromDeviceTo(resps);

                    //Serial part
                    int imax = NWORKITEMS / LOCALWORKSIZE;
                    for (int i = 0; i < imax; i++) resp += resps[i];

                }
                else
                {
                    double sum = 0;
                    for (int i = 0; i < CLv.Length; i++) sum += CLv.Values[i];
                    resp = (float)sum;
                }

                return resp;

            }

            /// <summary>Sums the components of a vector using __local memory and coalesced access</summary>
            /// <param name="CLv">Matrix whose components should be summed</param>
            public static float SumMatrixElements(floatMatrix CLv)
            {
                float resp = 0;
                if (UseOpenCLIfAvailable && CLCalc.CLAcceleration == CLCalc.CLAccelerationType.UsingCL)
                {
                    /*
                     The idea here is to create a reduction in which the access pattern to the vectors is coalesced.
                     The first step is to reduce the number of non-summed items to a multiple of NWORKITEMS and then coalesce the access
                     */

                    int LOCALWORKSIZE = Math.Min(256, (int)CLCalc.Program.CommQueues[CLCalc.Program.DefaultCQ].Device.MaxWorkGroupSize);
                    int NWORKITEMS = 16 * LOCALWORKSIZE;

                    int n = CLv.Values.Length;
                    float[] resps = new float[NWORKITEMS];
                    if (CLv.CLresps == null)
                    {
                        CLv.CLresps = new CLCalc.Program.Variable(resps);
                        CLv.CLn = new CLCalc.Program.Variable(new int[1]);
                    }

                    CLv.CLn.WriteToDevice(new int[] { n });
                    CLCalc.Program.Variable[] args = new CLCalc.Program.Variable[] { CLv.CLValues, CLv.CLresps, CLv.CLn };

                    //Write n = k*NWORKITEMS + p. Preprocess to eliminate p`s and leave summation only to a multiple of NWORKITEMS
                    int k = n / NWORKITEMS;
                    int p = n - k * NWORKITEMS;

                    //Clears partial responses
                    kernelClear.Execute(args, NWORKITEMS);

                    //Sums the p last elements into the p first elements
                    kernelPreSum.Execute(args, p);

                    //Use CLn to inform each work-item its workload. Each one will access and sum k numbers
                    CLv.CLn.WriteToDevice(new int[] { k });

                    kernelCoalLocalSum.Execute(args, new int[] { NWORKITEMS }, new int[] { LOCALWORKSIZE });

                    CLv.CLresps.ReadFromDeviceTo(resps);

                    //Serial part
                    int imax = NWORKITEMS / LOCALWORKSIZE;
                    for (int i = 0; i < imax; i++) resp += resps[i];

                }
                else
                {
                    double sum = 0;
                    for (int i = 0; i < CLv.Values.Length; i++) sum += CLv.Values[i];
                    resp = (float)sum;
                }

                return resp;

            }

            /// <summary>Copies src vector contents to dst</summary>
            public static void CopyVector(floatVector Src, floatVector Dst)
            {
                if (UseOpenCLIfAvailable && CLCalc.CLAcceleration == CLCalc.CLAccelerationType.UsingCL)
                {
                    kernelCopyBuffer.Execute(new CLCalc.Program.MemoryObject[] { Src.CLValues, Dst.CLValues }, Src.Length);
                }
                else
                {
                    for (int i = 0; i < Src.Length; i++) Dst.Values[i] = Src.Values[i];
                }
            }

            /// <summary>Copies src vector contents to dst</summary>
            public static void CopyMatrix(floatMatrix Src, floatMatrix Dst)
            {
                if (UseOpenCLIfAvailable && CLCalc.CLAcceleration == CLCalc.CLAccelerationType.UsingCL)
                {
                    kernelCopyBuffer.Execute(new CLCalc.Program.MemoryObject[] { Src.CLValues, Dst.CLValues }, Src.CLValues.OriginalVarLength);
                }
                else
                {
                    for (int i = 0; i < Src.Values.Length; i++) Dst.Values[i] = Src.Values[i];
                }
            }

            /// <summary>Returns true if v has any positive entries</summary>
            public static bool HasPositiveEntries(floatVector v)
            {
                if (UseOpenCLIfAvailable && CLCalc.CLAcceleration == CLCalc.CLAccelerationType.UsingCL)
                {
                    float[] resp = new float[1];
                    v.CLCoef.WriteToDevice(resp);
                    kernelHasPositiveEntry.Execute(new CLCalc.Program.MemoryObject[] { v.CLValues, v.CLCoef }, v.CLValues.OriginalVarLength);
                    v.CLCoef.ReadFromDeviceTo(resp);

                    return (resp[0] > 1);
                }
                else
                {
                    for (int i = 0; i < v.Values.Length; i++) if (v.Values[i] >= 0) return true;

                    return false;
                }
            }

            #endregion

            #region BLAS 2 - matrix - vector prod
            /// <summary>Computes M*(alpha*v). Creates ans if it is null</summary>
            public static floatVector MatrVecProd(floatMatrix M, floatVector v, float alpha, ref floatVector ans)
            {
                if (ans != null && ans.Length != M.Rows) throw new Exception("ans length should match M rows");
                if (v.Length != M.Cols) throw new Exception("v length should match M cols");
                if (ans == null) ans = new floatVector(new float[M.Rows]);

                if (UseOpenCLIfAvailable && CLCalc.CLAcceleration == CLCalc.CLAccelerationType.UsingCL)
                {
                    v.CLCoef.WriteToDevice(new float[] { alpha });
                    kernelMatrVecProd.Execute(new CLCalc.Program.MemoryObject[] { M.CLValues, M.CLDim, v.CLValues, v.CLCoef, ans.CLValues }, M.Rows);
                }
                else
                {
                    for (int i = 0; i < M.Rows; i++)
                    {
                        float temp = 0;
                        for (int j = 0; j < M.Cols; j++)
                        {
                            temp += M[i, j] * v.Values[j] * alpha;
                        }
                        ans.Values[i] = temp;
                    }
                }

                return ans;
            }

            /// <summary>Computes M*(alpha*v) + beta*u. Creates ans if it is null</summary>
            public static floatVector MatrVecProdSumVec(floatMatrix M, floatVector v, float alpha, floatVector u, float beta, ref floatVector ans)
            {
                if (ans != null && ans.Length != M.Rows) throw new Exception("ans length should match M rows");
                if (v.Length != M.Cols) throw new Exception("v length should match M cols");
                if (u.Length != M.Rows) throw new Exception("u length should match M rows");
                if (ans == null) ans = new floatVector(new float[M.Rows]);

                if (UseOpenCLIfAvailable && CLCalc.CLAcceleration == CLCalc.CLAccelerationType.UsingCL)
                {
                    v.CLCoef.WriteToDevice(new float[] { alpha });
                    u.CLCoef.WriteToDevice(new float[] { beta });
                    kernelMatrVecProdSumVec.Execute(new CLCalc.Program.MemoryObject[] { M.CLValues, M.CLDim, v.CLValues, v.CLCoef, u.CLValues, u.CLCoef, ans.CLValues }, M.Rows);
                }
                else
                {
                    for (int i = 0; i < M.Rows; i++)
                    {
                        float temp = 0;
                        for (int j = 0; j < M.Cols; j++)
                        {
                            temp += M[i, j] * v.Values[j] * alpha;
                        }
                        ans.Values[i] = temp + beta * u.Values[i];
                    }
                }

                return ans;
            }

            /// <summary>Computes the Matrix-vector product alpha*D*u</summary>
            public static floatVector DiagVecProd(floatDiag D, floatVector u, float alpha, ref floatVector ans)
            {
                if (ans != null && ans.Length != D.Rows) throw new Exception("ans length should match D dimension");
                if (u.Length != D.Rows) throw new Exception("u length should match D dimension");
                if (ans == null) ans = new floatVector(new float[D.Rows]);

                if (UseOpenCLIfAvailable && CLCalc.CLAcceleration == CLCalc.CLAccelerationType.UsingCL)
                {
                    u.CLCoef.WriteToDevice(new float[] { alpha });
                    kernelDiagVecProd.Execute(new CLCalc.Program.MemoryObject[] { D.CLValues, u.CLValues, u.CLCoef, ans.CLValues }, D.Rows);
                }
                else
                {
                    for (int i = 0; i < D.Rows; i++)
                    {
                        ans.Values[i] = alpha * D.Values[i] * u.Values[i];
                    }
                }

                return ans;
            }

            /// <summary>Computes the Matrix-matrix transpose product alpha*D*transpose(V)</summary>
            public static floatMatrix DiagTranspMatProd(floatDiag D, floatMatrix u, float alpha, ref floatMatrix ans)
            {
                if (ans != null && (ans.Rows != u.Cols || ans.Cols != u.Rows)) throw new Exception("ans length should match transpose(u)");
                if (u.Cols != D.Rows) throw new Exception("u Cols should match D dimension");
                if (ans == null) ans = new floatMatrix(new float[u.Cols, u.Rows]);

                if (UseOpenCLIfAvailable && CLCalc.CLAcceleration == CLCalc.CLAccelerationType.UsingCL)
                {
                    u.CLCoef.WriteToDevice(new float[] { alpha });
                    kernelDiagTranspMatProd.Execute(new CLCalc.Program.MemoryObject[] { D.CLValues, u.CLValues, u.CLCoef, ans.CLValues }, new int[] { u.Cols, u.Rows });
                }
                else
                {
                    int NN = u.Cols;
                    int MM = u.Rows;
                    for (int j = 0; j < u.Rows; j++)
                    {
                        for (int i = 0; i < D.Rows; i++)
                        {
                            ans.Values[j + MM * i] = alpha * D.Values[i] * u.Values[i + NN * j];
                        }
                    }
                }

                return ans;
            }

            /// <summary>Symmetric positive definite product with vector, Msym*v. resp gets constructed if ==null </summary>
            private static void MultiplyNoCL(floatSymPosDefMatrix M, floatVector v, ref floatVector resp)
            {
                for (int i = 0; i <  M.getN; i++)
                {
                    float val = 0;
                    for (int k = 0; k < M.getN; k++)
                    {
                        val += M[i, k] * v.Values[k];
                    }
                    resp.Values[i] = val;
                }
            }

            /// <summary>Symmetric positive definite product with vector, Msym*v. resp gets constructed if ==null </summary>
            public static floatVector SymPosDefSymMatrVecMultiply(floatSymPosDefMatrix M, floatVector v, ref floatVector ans)
            {
                if (ans == null) ans = new floatVector(new float[v.Length]);
                if (ans.Length != v.Length) throw new Exception("ans and v should have the same length");
                if (v.Length != M.getN) throw new Exception("Invalid vector length to multiply by this matrix");

                if (CLCalc.CLAcceleration != CLCalc.CLAccelerationType.UsingCL || !floatLinalg.UseOpenCLIfAvailable)
                {
                    MultiplyNoCL(M, v, ref ans);
                    return ans;
                }

                if (!M.IsMatrixInClMemoryUpdated)
                {
                    M.CLValues.WriteToDevice(M.Values);
                    M.IsMatrixInClMemoryUpdated = true;
                }

                kernelSymMatrVecMultiply.Execute(new CLCalc.Program.Variable[] { M.CLValues, v.CLValues, ans.CLValues }, M.getN);

                return ans;
            }


            #endregion

            #region BLAS 3 and Atb

            /// <summary>Computes A*inv(H)*A' and stores result in ans</summary>
            /// <param name="A">A matrix, mxn</param>
            /// <param name="H">H matrix, nxn</param>
            /// <param name="ans">answer, mxm</param>
            /// <param name="temp">Temporary matrix for the operation, size nxm</param>
            /// <param name="refine">Refine linear system solution?</param>
            public static floatSymPosDefMatrix ComputeAinvHTranspA(floatMatrix A, floatSymPosDefMatrix H, ref floatSymPosDefMatrix ans, ref floatMatrix temp, bool refine)
            {
                int m=A.Rows;
                int n=A.Cols;
                if (ans == null) ans = new floatSymPosDefMatrix(m);

                if (H.getN != n) throw new Exception("Matrix sizes not compatible");
                if (ans.getN != m) throw new Exception("Answer size not compatible");

                H.LinearSolve(A, refine, ref temp);

                //Go on to multiplying A*temp
                if (UseOpenCLIfAvailable && CLCalc.CLAcceleration == CLCalc.CLAccelerationType.UsingCL)
                {
                    kernelComputeAinvHAt.Execute(new CLCalc.Program.MemoryObject[] { A.CLValues, A.CLDim, temp.CLValues, ans.CLValues }, (m * (m + 1)) >> 1);
                    ans.IsCholeskyFactorized = false;
                }
                else
                {
                    for (int p = 0; p < m; p++)
                    {
                        int np = n * p;
                        for (int q = 0; q <= p; q++)
                        {
                            int nq = n * q;

                            float val = 0;
                            for (int k = 0; k < n; k++)
                            {
                                val += A.Values[k + np] * temp.Values[k + nq];
                            }
                            ans.Values[((p * (1 + p)) >> 1) + q] = val;
                        }
                    }
                }

                return ans;
            }

            /// <summary>Computes transpose(A)*A</summary>
            /// <param name="A">Original matrix</param>
            /// <param name="lambda">Regularization term</param>
            /// <param name="AtA">Answer, A transpose times A</param>
            public static floatSymPosDefMatrix MatrTranspMatrProd(floatMatrix A, floatVector lambda, ref floatSymPosDefMatrix AtA)
            {
                return MatrTranspMatrProd(A, null, lambda, ref AtA);
            }

            /// <summary>Symmetric positive definite product with matrix transpose, Msym*At. ans gets constructed if ==null </summary>
            public static floatMatrix SymPosDefMatrMatrMultiply(floatSymPosDefMatrix M, floatMatrix A, ref floatMatrix ans)
            {
                if (ans == null) ans = new floatMatrix(new float[A.Cols, M.getN]);

                if (UseOpenCLIfAvailable && CLCalc.CLAcceleration == CLCalc.CLAccelerationType.UsingCL)
                {
                    if (!M.IsMatrixInClMemoryUpdated) M.CLValues.WriteToDevice(M.Values);
                    kernelSymMatrMatrMultiply.Execute(new CLCalc.Program.MemoryObject[] { M.CLValues, A.CLValues, ans.CLValues }, new int[] { A.Rows, A.Cols });
                }
                else
                {
                    for (int j = 0; j < A.Cols; j++)
                    {
                        for (int i = 0; i < M.getN; i++)
                        {
                            float val = 0;
                            for (int k = 0; k < M.getN; k++)
                            {
                                val += M[i, k] * A.Values[k + j * A.Rows];
                            }
                            ans.Values[i + j * A.Rows] = val;
                        }
                    }
                }

                return ans;
            }

            /// <summary>Computes transpose(A)*A using weights W</summary>
            /// <param name="A">Original matrix</param>
            /// <param name="W">Measurement weight vector</param>
            /// <param name="lambda">Regularization term</param>
            /// <param name="AtA">Answer, A transpose times A</param>
            public static floatSymPosDefMatrix MatrTranspMatrProd(floatMatrix A, floatDiag W, floatVector lambda, ref floatSymPosDefMatrix AtA)
            {
                if (UseOpenCLIfAvailable && CLCalc.CLAcceleration == CLCalc.CLAccelerationType.UsingCL)
                {
                    return AuxLSAtACL(A, W, lambda, ref AtA);
                }
                else
                {
                    return AuxLeastSquaresAtAnoCL(A, W, lambda, ref AtA);
                }
            }

            /// <summary>Computes A*B' = A*transpose(B) and stores result in ans</summary>
            /// <param name="A">Matrix A</param>
            /// <param name="B">Matrix B</param>
            /// <param name="ans">Answer. If null, gets created.</param>
            public static void MatrTranspMatrProd(floatMatrix A, floatMatrix B, ref floatMatrix ans)
            {
                if (A.Cols != B.Cols) throw new Exception("Incompatible dimensions");
                if (ans == null) ans = new floatMatrix(new float[A.Rows, B.Rows]);
                if (ans.Rows != A.Rows || ans.Cols != B.Rows) throw new Exception("Invalid ans dimensions");

                if (UseOpenCLIfAvailable && CLCalc.CLAcceleration == CLCalc.CLAccelerationType.UsingCL)
                {
                    kernelRegularMatrTranspMatrProd.Execute(new CLCalc.Program.MemoryObject[] { A.CLValues, B.CLValues, ans.CLValues, A.CLDim }, new int[] { A.Rows, B.Rows });
                }
                else
                {
                    int n = A.Cols;
                    int p = B.Rows;
                    for (int i = 0; i < A.Rows; i++)
                    {
                        for (int j = 0; j < B.Rows; j++)
                        {
                            int ni = n * i;
                            int nj = n * j;

                            float temp = 0.0f;
                            for (int k = 0; k < n; k++)
                            {
                                temp += A.Values[k + ni] * B.Values[k + nj];
                            }

                            ans.Values[j + p * i] = temp;

                        }
                    }
                }
            }

            /// <summary>Computes transpose(A)*A and transpose(A)*b weighted by W</summary>
            /// <param name="A">Original matrix</param>
            /// <param name="W">Measurement weight vector</param>
            /// <param name="lambda">Regularization term</param>
            /// <param name="AtA">Answer, A transpose times A</param>
            private static floatSymPosDefMatrix AuxLeastSquaresAtAnoCL(floatMatrix A, floatDiag W, floatVector lambda, ref floatSymPosDefMatrix AtA)
            {
                //A (mxn), AtA (nxn) positive semidef symmetric
                int m = A.Rows;
                int n = A.Cols;

                if (AtA == null) AtA = new floatSymPosDefMatrix(new float[(n * (n + 1)) >> 1]);

                if (W != null)
                {
                    for (int i = 0; i < n; i++)
                    {
                        for (int j = 0; j <= i; j++)
                        {
                            double val = 0;
                            for (int k = 0; k < m; k++)
                            {
                                val += A[k, i] * A[k, j] * W.Values[k];
                            }
                            AtA.Values[((i * (i + 1)) >> 1) + j] = (float)val;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < n; i++)
                    {
                        for (int j = 0; j <= i; j++)
                        {
                            double val = 0;
                            for (int k = 0; k < m; k++)
                            {
                                val += A[k, i] * A[k, j];
                            }
                            AtA.Values[((i * (i + 1)) >> 1) + j] = (float)val;
                        }
                    }
                }

                //regularization term
                for (int i = 0; i < n; i++)
                {
                    AtA.Values[((i * (i + 1)) >> 1) + i] += lambda.Values[i];
                }

                return AtA;
            }



            /// <summary>Computes transpose(A)*A and transpose(A)*b weighted by W using OpenCL. Lambda is regularization term</summary>
            private static floatSymPosDefMatrix AuxLSAtACL(floatMatrix A, floatDiag W, floatVector lambda, ref floatSymPosDefMatrix AtA)
            {
                if (AtA == null || AtA.CLValues.OriginalVarLength != (A.Cols * (A.Cols + 1)) >> 1)
                {
                    AtA = new floatSymPosDefMatrix(new float[(A.Cols * (A.Cols + 1)) >> 1]);
                }

                CLCalc.Program.Variable[] args = new CLCalc.Program.Variable[] { A.CLValues, A.CLDim, W.CLValues, AtA.CLValues, lambda.CLValues };
                kernelComputeAtWA.Execute(args, AtA.CLValues.OriginalVarLength);
                
                //Just modified values in CL memory, matrix is no longer Cholesky factorized
                AtA.IsCholeskyFactorized = false;

                return AtA;
            }

            /// <summary>Computes transpose(A)*diag(W)*b*alpha</summary>
            /// <param name="A">Original matrix</param>
            /// <param name="b">Vector to multiply</param>
            /// <param name="W">Measurement weight vector</param>
            /// <param name="ans">Answer. If null, gets created</param>
            public static floatVector MatrTraspVecMult(floatMatrix A, floatDiag W, floatVector b, ref floatVector ans)
            {
                return MatrTraspVecMult(A, W, b, 1.0f, ref ans);
            }

            /// <summary>Computes transpose(A)*diag(W)*b*alpha</summary>
            /// <param name="A">Original matrix</param>
            /// <param name="b">Vector to multiply</param>
            /// <param name="W">Measurement weight vector</param>
            /// <param name="alpha">Multiplication constant</param>
            /// <param name="ans">Answer. If null, gets created</param>
            public static floatVector MatrTraspVecMult(floatMatrix A, floatDiag W, floatVector b, float alpha, ref floatVector ans)
            {
                int m = A.Rows;
                int n = A.Cols;
                if (ans == null) ans = new floatVector(new float[A.Cols]);

                if (A.Rows != W.Rows) throw new Exception("Incompatible A and W dimensions");
                if (A.Rows != b.Length) throw new Exception("Incompatible A and b dimensions");
                if (A.Cols != ans.Length) throw new Exception("Incompatible A and ans dimensions");


                if (UseOpenCLIfAvailable && CLCalc.CLAccelerationType.UsingCL == CLCalc.CLAcceleration)
                {
                    b.CLCoef.WriteToDevice(new float[] {alpha});
                    kernelTranspMatrVecProdW.Execute(new CLCalc.Program.MemoryObject[] { A.CLValues, A.CLDim, b.CLValues, b.CLCoef, W.CLValues, ans.CLValues }, A.Cols);
                }
                else
                {
                    for (int i = 0; i < n; i++)
                    {
                        double val = 0;
                        for (int k = 0; k < m; k++)
                        {
                            val += A[k, i] * b.Values[k] * W.Values[k] * alpha;
                        }
                        ans.Values[i] = (float)val;
                    }
                }

                return ans;
            }


            #endregion

        }

        /// <summary>Encapsulates functions to create a symmetric, positive definite matrix</summary>
        public class floatSymPosDefMatrix
        {

            #region Constructors
            /// <summary>Constructor.</summary>
            /// <param name="n">Number of matrix rows (n x n)</param>
            public floatSymPosDefMatrix(int n)
            {
                this.N = n;
                Values = new float[(n * (n + 1)) >> 1];

                LocalInitCL();
                this.IsCholeskyFactorized = false;
            }

            /// <summary>Constructor.</summary>
            /// <param name="vals">Matrix elements. Length should be n*(n+1)/2 where matrix is nxn</param>
            public floatSymPosDefMatrix(float[] vals)
            {
                int temp = (int)Math.Floor(Math.Sqrt(1 + (vals.Length << 3)));
                int n = temp * temp == 1 + (vals.Length << 3) ? (temp - 1) >> 1 : temp >> 1;

                if (vals.Length != (n * (n + 1)) >> 1) throw new Exception("Invalid vector length");

                Values = (float[])vals.Clone();
                this.N = n;

                LocalInitCL();
                this.IsCholeskyFactorized = false;
            }

            #endregion

            #region Matrix information

            /// <summary>Matrix dimension</summary>
            private int N;

            /// <summary>Gets matrix dimension</summary>
            public int getN
            {
                get { return N; }
            }

            /// <summary>Matrix values</summary>
            public float[] Values;

            /// <summary>Nuumber of rows</summary>
            public int Rows
            {
                get
                {
                    return N;
                }
            }
            /// <summary>Number of columns</summary>
            public int Cols
            {
                get
                {
                    return N;
                }
            }

            /// <summary>Access matrix elements</summary>
            /// <param name="i">Row index of element to access</param>
            /// <param name="j">Column index of element to access</param>
            public float this[int i, int j]
            {
                get
                {
                    if (i >= N || j >= N) throw new Exception("Index out of bounds");

                    if (i >= j) return Values[((i * (i + 1)) >> 1) + j];
                    else return Values[((j * (j + 1)) >> 1) + i];
                }
                set
                {
                    if (i >= N || j >= N) throw new Exception("Index out of bounds");

                    this.IsCholeskyFactorized = false;
                    this.IsMatrixInClMemoryUpdated = false;
                    if (i >= j) Values[((i * (i + 1)) >> 1) + j] = value;
                    else Values[((j * (j + 1)) >> 1) + i] = value;
                }
            }

            /// <summary>Returns a string representing this instance</summary>
            public override string ToString()
            {
                int maxN = 200;
                string s = "";

                for (int i = 0; i < Math.Min(maxN, this.N); i++)
                {
                    for (int j = 0; j < Math.Min(maxN, this.N); j++)
                    {
                        s += (this[i, j]).ToString() + "\t\t";
                    }
                    s += "\n";
                }

                return s;
            }

            #endregion

            #region Cholesky factorization
            /// <summary>Was the matrix cholesky factorized since last update?</summary>
            public bool IsCholeskyFactorized = false;
            /// <summary>Cholesky factorization</summary>
            public float[] cholDec;

            /// <summary>Computes Cholesky factorization of a matrix</summary>
            public void ComputeCholesky()
            {
                if (!UseOpenCLIfAvailable || CLCalc.CLAcceleration != CLCalc.CLAccelerationType.UsingCL/* || this.N < 120*/)
                {
                    NoCLCholesky();
                    if (UseOpenCLIfAvailable && CLCalc.CLAcceleration == CLCalc.CLAccelerationType.UsingCL) CLcholDec.WriteToDevice(cholDec);
                }
                else CLBlockCholesky();

                //float last;
                //if (UseOpenCLIfAvailable && CLCalc.CLAcceleration == CLCalc.CLAccelerationType.UsingCL)
                //{
                //    CLcholDec.ReadFromDeviceTo(cholDec);
                //    last = cholDec[((getN * (getN + 1)) >> 1) - 1];
                //}
            }

            /// <summary>Naive computation of the Cholesky factorization for very small systems or systems without OpenCL</summary>
            private void NoCLCholesky()
            {
                cholDec = new float[(N * (N + 1)) >> 1];
                float[] thisCp = (float[])this.Values.Clone();

                float[] prevVals = new float[N];

                float temp;
                float temp2;
                int indTemp;

                for (int i = 0; i < N; i++)
                {
                    //pivot
                    temp = 1.0f / (float)Math.Sqrt(thisCp[((i * (i + 1)) >> 1) + i]);

                    //Row elements
                    for (int j = i; j < N; j++)
                    {
                        indTemp = ((j * (j + 1)) >> 1) + i;
                        temp2 = temp * thisCp[indTemp];

                        cholDec[indTemp] = temp2;
                        prevVals[j] = temp2;
                    }

                    //Global update
                    for (int p = i + 1; p < N; p++)
                    {
                        int pp = ((p * (p + 1)) >> 1);
                        for (int q = i + 1; q <= p; q++)
                        {
                            indTemp = pp + q;
                            thisCp[indTemp] = thisCp[indTemp] - prevVals[p] * prevVals[q];
                        }
                    }
                }

                this.IsCholeskyFactorized = true;
            }

            /// <summary>DEBUG function. Returns true if Cholesky decomposition succeeded to 10*float.Epsilon precision</summary>
            private bool CheckDecomposition()
            {
                if (!IsCholeskyFactorized) throw new Exception("Matrix not factorized");

                double check = 0;
                for (int i = 0; i < N; i++)
                {
                    for (int j = 0; j <= i; j++)
                    {
                        double elem = 0;
                        for (int k = 0; k <= j; k++)
                            elem += cholDec[((i * (i + 1)) >> 1) + k] * cholDec[((j * (j + 1)) >> 1) + k];

                        double dif = (this.Values[((i * (i + 1)) >> 1) + j] - elem) / elem;
                        check += Math.Abs(dif);
                    }
                }
                check /= (double)(0.5 * N * (N + 1));
                return (check < 5E-5);
            }

            #region OpenCL Cholesky decomposition

            /// <summary>Submatrix inverse</summary>
            private float[] invL11;
            /// <summary>Submatrix inverse</summary>
            private CLCalc.Program.Variable CLinvl11;

            /// <summary>Cholesky decomposition in Device memory</summary>
            public CLCalc.Program.Variable CLcholDec;
            /// <summary>Copy of values of this matrix</summary>
            public CLCalc.Program.Variable CLValues;
            /// <summary>Is matrix in OpenCL memory updated?</summary>
            public bool IsMatrixInClMemoryUpdated = true;

            /// <summary>Cholesky elements computed in previous step</summary>
            CLCalc.Program.Variable CLprevVals;
            /// <summary>Offsets to perform calculations</summary>
            CLCalc.Program.Variable CLoffSet;

            /// <summary>B variable during back/forward subst</summary>
            CLCalc.Program.Variable CLb;
            /// <summary>Y variable during back/forward subst</summary>
            CLCalc.Program.Variable CLy;
            /// <summary>Size of matrix in CL memory</summary>
            CLCalc.Program.Variable CLn;


            /// <summary>Vector to hold M*current solution</summary>
            floatVector vecMx;
            /// <summary>Vector to hold residues</summary>
            floatVector vecResidues;
            /// <summary>Vector to hold residues absolute values</summary>
            floatVector vecResiduesAbs;
            /// <summary>Vector to hold refinement</summary>
            floatVector vecDeltax;

            /// <summary>Vector to hold M*current solution</summary>
            floatMatrix matMx;
            /// <summary>Vector to hold residues</summary>
            floatMatrix matResidues;
            /// <summary>Vector to hold residues absolute values</summary>
            floatMatrix matResiduesAbs;
            /// <summary>Vector to hold refinement</summary>
            floatMatrix matDeltax;

            private void LocalInitCL()
            {
                if (CLCalc.CLAcceleration == CLCalc.CLAccelerationType.Unknown) CLCalc.InitCL();

                if (UseOpenCLIfAvailable && CLCalc.CLAcceleration == CLCalc.CLAccelerationType.UsingCL)
                {
                    CLoffSet = new CLCalc.Program.Variable(new int[1]);
                    CLValues = new CLCalc.Program.Variable(this.Values);


                    invL11 = new float[(SUBMATRIXSIZE * (SUBMATRIXSIZE + 1)) >> 1];
                    CLinvl11 = new CLCalc.Program.Variable(invL11);

                    int NMultiple = N;
                    if (N % SUBMATRIXSIZE != 0)
                    {
                        NMultiple = N / SUBMATRIXSIZE;
                        NMultiple = SUBMATRIXSIZE * (NMultiple + 1);
                        cholDec = new float[(NMultiple * (NMultiple + 1)) >> 1];
                        for (int i = 0; i < Values.Length; i++) cholDec[i] = Values[i];
                    }
                    else
                    {
                        cholDec = (float[])this.Values.Clone();
                    }

                    CLcholDec = new CLCalc.Program.Variable(cholDec);
                    CLprevVals = new CLCalc.Program.Variable(new float[N]);

                    CLb = new CLCalc.Program.Variable(new float[N]);
                    CLy = new CLCalc.Program.Variable(new float[N]);
                    CLn = new CLCalc.Program.Variable(new int[] { N });
                }
            }

            /// <summary>Cholesky decomposition using OpenCL with Blocks</summary>
            public void CLBlockCholesky()
            {
                //If matrix dimension is not a multiple of SUBMATRIXSIZE
                //pad with zeros.

                int NMultiple = N;
                if (N % SUBMATRIXSIZE != 0)
                {
                    NMultiple = N / SUBMATRIXSIZE;
                    NMultiple = SUBMATRIXSIZE * (NMultiple + 1);
                }

                if (!IsMatrixInClMemoryUpdated)
                {
                    for (int i = 0; i < Values.Length; i++) cholDec[i] = Values[i];
                    CLcholDec.WriteToDevice(cholDec);
                }
                else
                {
                    kernelCopyBuffer.Execute(new CLCalc.Program.MemoryObject[] { CLValues, CLcholDec }, CLValues.OriginalVarLength);
                }


                int SubMatrixSize = SUBMATRIXSIZE;
                int GlobalSize;


                //Important. Set offset to zero
                CLoffSet.WriteToDevice(new int[] { 0 });
                CLCalc.Program.Variable[] args = new CLCalc.Program.Variable[] { CLcholDec, CLoffSet, CLinvl11 };

                GlobalSize = (SubMatrixSize * (SubMatrixSize + 1)) >> 1;
                for (int i = 0; i < NMultiple; i += SubMatrixSize)
                {
                    //Computes Cholesky factor L11 and its inverse
                    kernelCholeskyDiagBlock.Execute(args, new int[] { GlobalSize }, new int[] { GlobalSize });

                    //CLcholDec.ReadFromDeviceTo(cholDec);


                    //Computes column panel L21
                    //Note: offSet has been updated, kernel should use its value-1

                    //Number of submatrices to update: (N-i)/SubMatrixSize
                    int nSubMatrices = (NMultiple - i) / SubMatrixSize - 1;

                    if (nSubMatrices > 0)
                    {
                        //Computes panels and updates main diagonals
                        kernelCholeskyComputePanel.Execute(args, new int[] { nSubMatrices * SubMatrixSize, SubMatrixSize }, new int[] { SubMatrixSize, SubMatrixSize });

                        //CLcholDec.ReadFromDeviceTo(cholDec);

                        //Still need to update nSubMatrices*(nSubMatrices-1)/2 full matrices in the Cholesky decomposition
                        //They start at indexes [i+SubMatrixSize i], and they are the offdiagonal block matrices

                        int totalSubMatricesToUpdate = ((nSubMatrices - 1) * nSubMatrices) >> 1;
                        if (totalSubMatricesToUpdate > 0)
                        {
                            kernelCholeskyForwardProp.Execute(args, new int[] { totalSubMatricesToUpdate * SubMatrixSize, SubMatrixSize }, new int[] { SubMatrixSize, SubMatrixSize });
                        }
                    }

                    //CLcholDec.ReadFromDeviceTo(cholDec);
                }


                //CLcholDec.ReadFromDeviceTo(cholDec);
                this.IsCholeskyFactorized = true;
            }
            #endregion
            #endregion

            #region Linear system solving, determinant

            /// <summary>Solves system Ax = b and returns x</summary>
            /// <param name="b">b vector</param>
            public float[] LinearSolve(float[] b)
            {
                //Refining is basically free, so why not?
                return LinearSolve(b, true);
            }

            /// <summary>Solves system Ax = b and returns x</summary>
            /// <param name="b">b vector</param>
            /// <param name="refine">Refine solution? Recommended: true</param>
            public float[] LinearSolve(float[] b, bool refine)
            {
                floatVector CLbb = new floatVector(b);
                floatVector resp = null;
                LinearSolve(CLbb, refine, ref resp);
                if (UseOpenCLIfAvailable && CLCalc.CLAcceleration == CLCalc.CLAccelerationType.UsingCL)
                {
                    resp.CLValues.ReadFromDeviceTo(resp.Values);
                }
                return resp.Values;
            }

            /// <summary>Solves system Ax = b and returns x, where b is a right hand side matrix</summary>
            /// <param name="b">b vector</param>
            /// <param name="refine">Refine solution? Recommended: true</param>
            public float[,] LinearSolve(float[,] b, bool refine)
            {
                floatMatrix CLbb = new floatMatrix(b);
                floatMatrix resp = null;
                LinearSolve(CLbb, true, ref resp);
                if (UseOpenCLIfAvailable && CLCalc.CLAcceleration == CLCalc.CLAccelerationType.UsingCL)
                {
                    resp.CLValues.ReadFromDeviceTo(resp.Values);
                }
                float[,] vResp = new float[resp.Rows, resp.Cols];
                for (int i = 0; i < resp.Rows; i++) for (int j = 0; j < resp.Cols; j++) vResp[i, j] = resp[i, j];

                return vResp;
            }


            /// <summary>Solves system A*invHAt = Mt and returns invHAt solving system per column. Refine may considerably slow the method.</summary>
            /// <param name="M">Right-hand-size of linear system</param>
            /// <param name="refine">Refine solution? Recommended: true</param>
            /// <param name="invHAt">Answer A*invHAt</param>
            public floatMatrix LinearSolve(floatMatrix M, bool refine, ref floatMatrix invHAt)
            {
                //System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                //System.Diagnostics.Stopwatch sw1 = new System.Diagnostics.Stopwatch();
                //System.Diagnostics.Stopwatch sw2 = new System.Diagnostics.Stopwatch();

                if (invHAt == null) invHAt=new floatMatrix(new float[this.N, M.Rows]);
                if (this.N != M.Cols) throw new Exception("Dimensions not compatible");
                if (invHAt.Rows != this.N || invHAt.Cols != M.Rows) throw new Exception("Invalid matrix dimensions for invHAt");

                //OpenCLTemplate.CLCalc.Program.CommQueues[OpenCLTemplate.CLCalc.Program.DefaultCQ].Finish(); sw.Start();
                if (!this.IsCholeskyFactorized) ComputeCholesky();




                if (UseOpenCLIfAvailable && CLCalc.CLAcceleration == CLCalc.CLAccelerationType.UsingCL)
                {
                    M.CLValues.ReadFromDeviceTo(M.Values);
                    this.CLcholDec.ReadFromDeviceTo(this.cholDec);
                }
                linsolveMatrix(M, ref invHAt);
                if (UseOpenCLIfAvailable && CLCalc.CLAcceleration == CLCalc.CLAccelerationType.UsingCL) invHAt.CLValues.WriteToDevice(invHAt.Values);


                ////TO DO: OpenCL fwd/bksubs
                ////OpenCLTemplate.CLCalc.Program.CommQueues[OpenCLTemplate.CLCalc.Program.DefaultCQ].Finish(); sw1.Start();
                //LinsolveCLMatrix(M, ref invHAt);
                ////OpenCLTemplate.CLCalc.Program.CommQueues[OpenCLTemplate.CLCalc.Program.DefaultCQ].Finish(); sw1.Stop();



                if (!refine) return invHAt;

                double totalRes = 0;

                if (matResidues == null || matResidues.Values.Length != M.Values.Length)
                {
                    matResidues = new floatMatrix(new float[M.Rows, M.Cols]);
                    matMx = new floatMatrix(new float[M.Rows, M.Cols]);
                    matDeltax = new floatMatrix(new float[M.Rows, M.Cols]);
                    matResiduesAbs = new floatMatrix(new float[M.Rows, M.Cols]);
                }

                for (int iter = 0; iter < 8 && !double.IsNaN(totalRes); iter++)
                {
                    //OpenCLTemplate.CLCalc.Program.CommQueues[OpenCLTemplate.CLCalc.Program.DefaultCQ].Finish(); sw2.Start();
                    BLAS.SymPosDefMatrMatrMultiply(this, invHAt, ref matMx);
                    //OpenCLTemplate.CLCalc.Program.CommQueues[OpenCLTemplate.CLCalc.Program.DefaultCQ].Finish(); sw2.Stop();
                    BLAS.LinearCombination(1, matMx, -1, M, ref matResidues);


                    if (UseOpenCLIfAvailable && CLCalc.CLAcceleration == CLCalc.CLAccelerationType.UsingCL)
                        kernelElemWiseAbs.Execute(new CLCalc.Program.MemoryObject[] { matResidues.CLValues, matResiduesAbs.CLValues }, M.Values.Length);
                    else
                    {
                        for (int i = 0; i < M.Values.Length; i++) matResiduesAbs.Values[i] = Math.Abs(matResidues.Values[i]);
                    }

                    totalRes = matResiduesAbs.Sum() / (double)N;


                    if (totalRes < 1E-5)
                        iter = 8;
                    {
                        LinsolveCLMatrix(matResidues, ref matDeltax);

                        if (UseOpenCLIfAvailable && CLCalc.CLAcceleration == CLCalc.CLAccelerationType.UsingCL)
                            kernelInPlaceSubtract.Execute(new CLCalc.Program.MemoryObject[] { invHAt.CLValues, matDeltax.CLValues }, M.Values.Length);
                        else
                        {
                            for (int i = 0; i < M.Values.Length; i++) invHAt.Values[i] -= matDeltax.Values[i];
                        }
                    }


                }

                //swResto.Stop();
               // OpenCLTemplate.CLCalc.Program.CommQueues[OpenCLTemplate.CLCalc.Program.DefaultCQ].Finish(); sw.Stop();

                return invHAt;

            }

            /// <summary>Solves system Ax = b and returns x</summary>
            /// <param name="CLbb">b vector</param>
            /// <param name="refine">Refine solution? Recommended: true</param>
            /// <param name="resp">Holds answer</param>
            public floatVector LinearSolve(floatVector CLbb, bool refine, ref floatVector resp)
            {
                //System.Diagnostics.Stopwatch swChol = new System.Diagnostics.Stopwatch();
                //System.Diagnostics.Stopwatch swResto = new System.Diagnostics.Stopwatch();
                //System.Diagnostics.Stopwatch swLinSolve = new System.Diagnostics.Stopwatch();
                //System.Diagnostics.Stopwatch sw1 = new System.Diagnostics.Stopwatch();
                //System.Diagnostics.Stopwatch sw2 = new System.Diagnostics.Stopwatch();

                if (CLbb.Length != N) throw new Exception("Dimensions not compatible");

                //swChol.Start();
                if (!this.IsCholeskyFactorized) ComputeCholesky();
                //swChol.Stop();

                //float[] CholCP = (float[])cholDec.Clone();
                //this.NoCLCholesky();
                //double dif = 0;
                //for (int i = 0; i < cholDec.Length; i++) dif = Math.Max(dif, Math.Abs(cholDec[i] - CholCP[i]));
                //swResto.Start();

                //Computes preliminar solution
                //sw1.Start();
                if (resp != null && resp.Length != N) throw new Exception("Linear system resp should have dimension equal to matrix size");


                CLbb.ReadFromDevice();
                if (UseOpenCLIfAvailable && CLCalc.CLAcceleration == CLCalc.CLAccelerationType.UsingCL) this.CLcholDec.ReadFromDeviceTo(this.cholDec);
                linsolve(CLbb.Values, ref resp);
                if (UseOpenCLIfAvailable && CLCalc.CLAcceleration == CLCalc.CLAccelerationType.UsingCL) resp.CLValues.WriteToDevice(resp.Values);

                ////A FAZER: Fwd/bksubs com OpenCL
                //linsolveCL(CLbb, ref resp);


                //sw1.Stop(); sw2.Start();
                //float[] resp2 = linsolve(b);
                //sw2.Stop();
                //for (int i = 0; i < resp.Length; i++) dif = Math.Max(dif, Math.Abs(resp[i] - resp2[i]));
                //double tt = sw2.Elapsed.TotalSeconds - sw1.Elapsed.TotalSeconds ;

                if (!refine)
                {
                    //resp.ReadFromDevice();
                    //CLbb.ReadFromDevice();
                    //if (UseOpenCLIfAvailable && CLCalc.CLAcceleration == CLCalc.CLAccelerationType.UsingCL)
                    //{
                    //    this.CLValues.ReadFromDeviceTo(this.Values);
                    //    this.CLcholDec.ReadFromDeviceTo(this.cholDec);
                    //}

                    //swResto.Stop();
                    return resp;
                }

                double totalRes = 0;

                if (vecResidues == null)
                {
                    vecResidues = new floatVector(new float[N]);
                    vecMx = new floatVector(new float[N]);
                    vecDeltax = new floatVector(new float[N]);
                    vecResiduesAbs = new floatVector(new float[N]);
                }

                for (int iter = 0; iter < 8 && !double.IsNaN(totalRes); iter++)
                {
                    MultiplyCL(resp, ref vecMx);

                    BLAS.LinearCombination(1, vecMx, -1, CLbb, ref vecResidues);

                    if (UseOpenCLIfAvailable && CLCalc.CLAcceleration == CLCalc.CLAccelerationType.UsingCL)
                        kernelElemWiseAbs.Execute(new CLCalc.Program.MemoryObject[] { vecResidues.CLValues, vecResiduesAbs.CLValues }, N);
                    else
                    {
                        for (int i = 0; i < N; i++) vecResiduesAbs.Values[i] = Math.Abs(vecResidues.Values[i]);
                    }

                    totalRes = vecResiduesAbs.Sum()/(double)N;


                    if (totalRes < 1E-5) 
                        iter = 8;
                    {
                        linsolveCL(vecResidues, ref vecDeltax);

                        if (UseOpenCLIfAvailable && CLCalc.CLAcceleration == CLCalc.CLAccelerationType.UsingCL)
                            kernelInPlaceSubtract.Execute(new CLCalc.Program.MemoryObject[] { resp.CLValues, vecDeltax.CLValues }, N);
                        else
                        {
                            for (int i = 0; i < N; i++) resp.Values[i] -= vecDeltax.Values[i];
                        }
                    }
                }

                //swResto.Stop();


                return resp;
            }

            private float[] linsolveCL(float[] bb)
            {
                floatVector CLbb = new floatVector(bb);
                floatVector ans = null;
                linsolveCL(CLbb, ref ans);
                ans.CLValues.ReadFromDeviceTo(ans.Values);
                return ans.Values;
            }

            private void linsolveCL(floatVector CLbb, ref floatVector resp)
            {
                if (!UseOpenCLIfAvailable || CLCalc.CLAcceleration != CLCalc.CLAccelerationType.UsingCL)
                {
                    linsolve(CLbb.Values, ref resp);
                    return;
                }

                //int NMultiple = N;
                ////float[] bAugm;
                //if (N % SUBMATRIXSIZE != 0)
                //{
                //    NMultiple = N / SUBMATRIXSIZE;
                //    NMultiple = SUBMATRIXSIZE * (NMultiple + 1);
                //}
                ////bAugm = new float[NMultiple];
                ////for (int i = 0; i < bb.Length; i++) bAugm[i] = bb[i];

                if (resp == null) resp = new floatVector(new float[N]);


                //Copy elements to CLb
                if (CLb == null || CLb.OriginalVarLength < CLbb.Length) CLb = new CLCalc.Program.Variable(CLbb.Values);
                kernelCopyBuffer.Execute(new CLCalc.Program.MemoryObject[] { CLbb.CLValues, CLb }, CLbb.Length);


                CLCalc.Program.Variable[] args = new CLCalc.Program.Variable[] { CLcholDec, CLy, CLb, CLoffSet, CLn };
                int[] offset = new int[1];


                //float[] yDebug = new float[N];
                //float[] bDebug = new float[N];


                //Forward substitution
                int i;
                for (i = 0; i < N; i += SUBMATRIXSIZE)
                {
                    offset[0] = i;
                    CLoffSet.WriteToDevice(offset);

                    int size = Math.Min(SUBMATRIXSIZE, N - i);
                    kernelFwdUpperBackSubs.Execute(args, new int[] { size }, new int[] { size });


                    ////DEBUG
                    //CLy.ReadFromDeviceTo(yDebug);
                    //CLb.ReadFromDeviceTo(bDebug);

                    //propagation
                    if (i + SUBMATRIXSIZE < N)
                    {
                        kernelFwdPropag.Execute(args, N - i - SUBMATRIXSIZE);
                    }

                    ////DEBUG
                    //CLy.ReadFromDeviceTo(yDebug);
                    //CLb.ReadFromDeviceTo(bDebug);
                    //CLcholDec.ReadFromDeviceTo(cholDec);
                }

                //Backward subst. Stores answer in CLb
                args = new CLCalc.Program.Variable[] { CLcholDec, CLb, CLy, CLoffSet, CLn };
                //Backward substitution
                for (i = N - SUBMATRIXSIZE; i >= 0; i -= SUBMATRIXSIZE)
                {
                    offset[0] = i;
                    CLoffSet.WriteToDevice(offset);

                    int size = SUBMATRIXSIZE;
                    kernelBkLowerBackSubs.Execute(args, new int[] { size }, new int[] { size });

                    ////DEBUG
                    //CLy.ReadFromDeviceTo(yDebug);
                    //CLb.ReadFromDeviceTo(bDebug);


                    if (i > 0)
                    {
                        kernelBackPropag.Execute(args, i);
                    }

                    //CLy.ReadFromDeviceTo(yDebug);
                    //CLb.ReadFromDeviceTo(bDebug);
                }
                if (SUBMATRIXSIZE + i > 0)
                {
                    offset[0] = 0; CLoffSet.WriteToDevice(offset);
                    kernelBkLowerBackSubs.Execute(args, new int[] { SUBMATRIXSIZE + i }, new int[] { SUBMATRIXSIZE + i });
                }
                //CLy.ReadFromDeviceTo(yDebug);
                //CLb.ReadFromDeviceTo(bDebug);

                kernelCopyBuffer.Execute(new CLCalc.Program.Variable[] { CLb, resp.CLValues }, N);

            }

            /// <summary>Solves system Ax = b and returns x</summary>
            /// <param name="bb">b vector</param>
            /// <param name="resp">Solution</param>
            private void linsolve(float[] bb, ref floatVector resp)
            {
                float[] b = (float[])bb.Clone();
                float[] y = new float[N];

                if (resp == null) resp = new floatVector(new float[N]);

                //Forward substitution
                for (int i = 0; i < N; i++)
                {
                    y[i] = b[i] / cholDec[((i * (i + 1)) >> 1) + i];

                    for (int j = i + 1; j < N; j++)
                    {
                        b[j] -= cholDec[((j * (j + 1)) >> 1) + i] * y[i];
                    }
                }

                //Backward substitution
                for (int i = N - 1; i >= 0; i--)
                {
                    resp.Values[i] = y[i] / cholDec[((i * (i + 1)) >> 1) + i];

                    for (int j = 0; j < i; j++)
                    {
                        y[j] -= cholDec[((i * (i + 1)) >> 1) + j] * resp.Values[i];
                    }
                }
            }

            /// <summary>Backsubstitutes to solve a linear system with a matrix right hand size</summary>
            private void LinsolveCLMatrix(floatMatrix M, ref floatMatrix resp)
            {
                //System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                //System.Diagnostics.Stopwatch sw1 = new System.Diagnostics.Stopwatch();
                //sw.Start();


                //number of RHS as multiple of SUBMATRIXSIZE
                int nRHSMult = M.Rows / SUBMATRIXSIZE;
                int nRHSleftOver = M.Rows - SUBMATRIXSIZE*nRHSMult;


                if (!UseOpenCLIfAvailable || CLCalc.CLAcceleration != CLCalc.CLAccelerationType.UsingCL)
                {
                    linsolveMatrix(M, ref resp);
                    return;
                }

                //Copy elements to CLb
                if (CLb == null || CLb.OriginalVarLength < M.Values.Length)
                {
                    CLb = new CLCalc.Program.Variable(M.Values);
                    CLy = new CLCalc.Program.Variable(M.Values);
                }

                kernelCopyBuffer.Execute(new CLCalc.Program.MemoryObject[] { M.CLValues, CLb }, M.Values.Length);
                int nEqs = M.Rows;

                CLCalc.Program.Variable[] args = new CLCalc.Program.Variable[] { CLcholDec, CLy, CLb, CLoffSet, CLn };
                int[] offset = new int[1];

                //DEBUG
                //float[] yDebug = new float[M.Values.Length];
                //float[] bDebug = new float[M.Values.Length];
                //this.CLcholDec.ReadFromDeviceTo(cholDec);

                //Forward substitution
                int i;
                for (i = 0; i < N; i += SUBMATRIXSIZE)
                {
                    offset[0] = i;
                    CLoffSet.WriteToDevice(offset);

                    int size = Math.Min(SUBMATRIXSIZE, N - i);
                    kernelFwdUpperBackSubs.Execute(args, new int[] { size, nEqs }, new int[] { size, 1 });


                    ////DEBUG
                    //CLy.ReadFromDeviceTo(yDebug);
                    //CLb.ReadFromDeviceTo(bDebug);

                    //sw1.Start();
                    //propagation
                    if (i + SUBMATRIXSIZE < N)
                    {
                        if (nRHSMult > 0) kernelFwdPropag.Execute(args, new int[] { N - i - SUBMATRIXSIZE, nRHSMult * SUBMATRIXSIZE }, new int[] { 1, SUBMATRIXSIZE });
                        if (nRHSleftOver > 0) 
                            kernelFwdPropag2.Execute(args, new int[] { N - i - SUBMATRIXSIZE, nRHSleftOver }, new int[] { 1, nRHSleftOver }, new int[] { 0, nRHSMult * SUBMATRIXSIZE });
                    }
                    //OpenCLTemplate.CLCalc.Program.CommQueues[OpenCLTemplate.CLCalc.Program.DefaultCQ].Finish();
                    //sw1.Stop();


                    ////DEBUG
                    //CLy.ReadFromDeviceTo(yDebug);
                    //CLb.ReadFromDeviceTo(bDebug);
                }

                //Backward subst. Stores answer in CLb
                args = new CLCalc.Program.Variable[] { CLcholDec, CLb, CLy, CLoffSet, CLn };
                //Backward substitution
                for (i = N - SUBMATRIXSIZE; i >= 0; i -= SUBMATRIXSIZE)
                {
                    offset[0] = i;
                    CLoffSet.WriteToDevice(offset);

                    int size = SUBMATRIXSIZE;
                    kernelBkLowerBackSubs.Execute(args, new int[] { size, nEqs }, new int[] { size, 1 });

                    if (i > 0)
                    {
                        //Propagation using __local storage
                        if (nRHSMult > 0) kernelBackPropag.Execute(args, new int[] { i, nRHSMult * SUBMATRIXSIZE }, new int[] { 1, SUBMATRIXSIZE });

                        //leftovers (not multiples of SUBMATRIXSIZE)
                        if (nRHSleftOver > 0)
                            kernelBackPropag2.Execute(args, new int[] { i, nRHSleftOver }, new int[] { 1, nRHSleftOver }, new int[] { 0, nRHSMult * SUBMATRIXSIZE });

                    }


                }
                if (SUBMATRIXSIZE + i > 0)
                {
                    offset[0] = 0; CLoffSet.WriteToDevice(offset);
                    kernelBkLowerBackSubs.Execute(args, new int[] { SUBMATRIXSIZE + i, nEqs }, new int[] { SUBMATRIXSIZE + i, 1 });
                }

                kernelCopyBuffer.Execute(new CLCalc.Program.Variable[] { CLb, resp.CLValues }, resp.Values.Length);


                //OpenCLTemplate.CLCalc.Program.CommQueues[OpenCLTemplate.CLCalc.Program.DefaultCQ].Finish();
                //sw.Stop();
            }

            /// <summary>Solves system Ax = b' and returns x</summary>
            /// <param name="bb">b Matrix</param>
            /// <param name="resp">Answer</param>
            private void linsolveMatrix(floatMatrix bb, ref floatMatrix resp)
            {
                float[] b = (float[])bb.Values.Clone();
                float[] y = new float[bb.Values.Length];



                if (resp == null) resp = new floatMatrix(new float[bb.Cols, bb.Rows]);

                for (int k = 0; k < bb.Rows; k++)
                {

                    //Forward substitution
                    for (int i = 0; i < N; i++)
                    {
                        y[i + k * N] = b[i + k * N] / cholDec[((i * (i + 1)) >> 1) + i];

                        for (int j = i + 1; j < N; j++)
                        {
                            b[j + k * N] -= cholDec[((j * (j + 1)) >> 1) + i] * y[i + k * N];
                        }
                    }

                    //Backward substitution
                    for (int i = N - 1; i >= 0; i--)
                    {
                        resp.Values[i + k * N] = y[i + k * N] / cholDec[((i * (i + 1)) >> 1) + i];

                        for (int j = 0; j < i; j++)
                        {
                            y[j + k * N] -= cholDec[((i * (i + 1)) >> 1) + j] * resp.Values[i + k * N];
                        }
                    }
                }
            }

            /// <summary>Retrieves the Determinant of this matrix</summary>
            public float Determinant()
            {
                {
                    if (!this.IsCholeskyFactorized) this.ComputeCholesky();

                    if (UseOpenCLIfAvailable && CLCalc.CLAcceleration == CLCalc.CLAccelerationType.UsingCL) CLcholDec.ReadFromDeviceTo(cholDec);

                    float det = 1;
                    for (int i = 0; i < N; i++)
                    {
                        det *= cholDec[((i * (i + 1)) >> 1) + i];
                    }
                    return det * det;
                }
            }
            #endregion

            #region Operations (product, etc)

            /// <summary>Dot product</summary>
            public static float Dot(float[] a, float[] b)
            {
                float val = 0;
                if (a.Length != b.Length) throw new Exception("Incompatibe dimensions for inner product");

                int n = a.Length;

                for (int i = 0; i < n; i++) val += a[i] + b[i];

                return val;
            }

            /// <summary>Symmetric positive definite product with vector, Msym*v. resp gets constructed if ==null </summary>
            private floatVector MultiplyCL(floatVector v, ref floatVector ans)
            {
                return BLAS.SymPosDefSymMatrVecMultiply(this, v, ref ans);
            }

            #endregion

            #region Write to disk
            /// <summary>Writes this matrix to file in Octave format</summary>
            /// <param name="file">File to write to</param>
            public void Write(string file)
            {
                using (StreamWriter sw = new StreamWriter(file))
                {
                    if (UseOpenCLIfAvailable && CLCalc.CLAcceleration == CLCalc.CLAccelerationType.UsingCL) CLValues.ReadFromDeviceTo(Values);
                    sw.WriteLine("# Created by OpenCLTemplate " + DateTime.Now.ToString());
                    sw.WriteLine("# name: P");
                    sw.WriteLine("# type: matrix");
                    sw.WriteLine("# rows:" + this.getN.ToString());
                    sw.WriteLine("# columns:" + this.getN.ToString());

                    for (int i = 0; i < getN; i++)
                    {
                        string s = "";
                        for (int j = 0; j < getN; j++)
                        {
                            s = s + " " + this[i, j].ToString().Replace(",", ".");
                        }
                        sw.WriteLine(s);
                    }
                }
            }
            #endregion

            #region Preconstructed matrices

            /// <summary>Returns the identity matrix</summary>
            /// <param name="n">Matrix dimension nxn</param>
            public static floatSymPosDefMatrix Identity(int n)
            {
                floatSymPosDefMatrix M = new floatSymPosDefMatrix(n);

                for (int i = 0; i < n; i++) M[i, i] = 1;

                return M;
            }

            #endregion

            #region Nonlinear least squares, http://www.alkires.com/103/chap6.pdf

            /// <summary>Delegate to compute residues and gradients based on current estimate x [n]. Returns residues r [m] and gradients gradR [m , n], j-th component
            /// of gradient of residue r[i] = [i,j] = gradR[i,j] </summary>
            /// <param name="x">Current estimate</param>
            /// <param name="r">Residues</param>
            /// <param name="gradR">Gradient of residue functions</param>
            /// <param name="ComputeGrads">Compute gradients?</param>
            public delegate void ComputeResidueGrad(float[] x, ref float[] r, ref float[,] gradR, bool ComputeGrads);

            /// <summary>Computes nonlinear least squares using user functions to evaluate residues and their gradients</summary>
            /// <param name="f">Function that computes residues [m] and their gradients [grad r1; grad r2] m x n (each gradient in one line) [i,j] = gradR[i,j]</param>
            /// <param name="x">Intial guess</param>
            /// <param name="m">Number of residue equations</param>
            /// <param name="maxiter">Maximum number of iterations</param>
            /// <param name="err">Adjustment error</param>
            public static float[] NonLinearLS(ComputeResidueGrad f, float[] x, int m, int maxiter, ref double err)
            {
                int n = x.Length;
                float eps = 5e-5f * 0.5f;
                float alpha = 0.002f;

                float[,] A = new float[m, n];
                float[] r = new float[m];
                
                floatMatrix CLA = new floatMatrix(A);
                floatVector CLr = new floatVector(r);
                floatVector CLlambda = new floatVector(new float[CLA.Cols]);
                float[] ww = new float[CLA.Rows];
                for (int i = 0; i < ww.Length; i++) ww[i] = 1;
                floatDiag CLW = new floatDiag(ww);

                float[] v = new float[CLA.Cols];
                floatVector CLv = new floatVector(v);

                double errAnt = 0;

                for (int i = 0; i < maxiter; i++)
                {
                    //Computes residues and gradient
                    f(x, ref r, ref A, true);
                    CLA.SetValues(A);
                    if (UseOpenCLIfAvailable && CLCalc.CLAcceleration == CLCalc.CLAccelerationType.UsingCL) CLr.CLValues.WriteToDevice(r);
                    else
                    {
                        for (int kk = 0; kk < r.Length; kk++) CLr.Values[kk] = r[kk];
                    }

                    errAnt = err;
                    err = NormAtb(A, r, m, n);

                    //if (errAnt == err) it means algorithm is not converging at all
                    if (err < eps || errAnt == err || double.IsNaN(err)) i = maxiter;
                    else
                    {
                        floatSymPosDefMatrix AtA = null;
                        AtA = BLAS.MatrTranspMatrProd(CLA, CLW, CLlambda, ref AtA);

                        CLv = BLAS.MatrTraspVecMult(CLA, CLW, CLr, ref CLv);

                        if (UseOpenCLIfAvailable && CLCalc.CLAcceleration == CLCalc.CLAccelerationType.UsingCL) CLv.CLValues.ReadFromDeviceTo(CLv.Values);
                        v = AtA.LinearSolve(CLv.Values);

                        for (int k = 0; k < v.Length; k++) v[k] = -v[k];

                        //Line search

                        //||r||²
                        float normRSquared = 0;
                        for (int k = 0; k < r.Length; k++) normRSquared += r[k] * r[k];

                        //2transpose(r)Av
                        float transpRAv = 0;
                        for (int p = 0; p < m; p++)
                        {
                            float val = 0;
                            for (int q = 0; q < n; q++) val += A[p, q] * v[q];
                            transpRAv += r[p] * val;
                        }
                        transpRAv *= 2.0f;

                        float t = 2.0f;
                        //iterates while sum(ri*(x+tv)^2)>||r||²+alpha*2*transpose(r)*A*v*t
                        float lhs = 1;
                        float rhs = 0;

                        float[] newX = (float[])x.Clone();

                        while (lhs > rhs)
                        {
                            t *= 0.5f;

                            //Update x
                            for (int k = 0; k < x.Length; k++) newX[k] = x[k] + v[k] * t;

                            //Update r
                            f(newX, ref r, ref A, false);

                            lhs = 0;
                            for (int k = 0; k < m; k++) lhs += r[k] * r[k];
                            rhs = normRSquared + alpha * transpRAv * t;
                        }

                        x = newX;
                    }
                }

                return x;
            }

            private static double NormAtb(float[,] A, float[] r, int m, int n)
            {
                double resp = 0;
                for (int i = 0; i < n; i++)
                {
                    double val = 0;
                    for (int k = 0; k < m; k++)
                    {
                        val += A[k, i] * r[k];
                    }
                    resp += val * val;
                }
                resp = Math.Sqrt(resp);
                return resp;
            }

            private static void testeGenLS()
            {
                Random rnd = new Random();
                int m = 20;
                int n = 2;

                vals = new float[m, n];
                bs = new float[m];

                for (int i = 0; i < m; i++)
                {
                    float xx = 4 * (float)rnd.NextDouble();
                    float yy = 4 * (float)rnd.NextDouble();
                    vals[i, 0] = xx;
                    vals[i, 1] = yy;
                    bs[i] = (float)Math.Sqrt(1.0f + 0.2 * yy + 0.03 * xx * xx) - 3 * (xx - yy) + 1000 * 0.2f * 0.03f;
                }

                //double err = 0;
                //NonLinearLS(compGrad, x, m, 50,ref err);
            }

            private static float[,] vals;
            private static float[] bs;
            private static void compGrad(float[] x, ref float[] r, ref float[,] gradR, bool computeGrads)
            {
                int n = x.Length;
                int m = r.Length;

                for (int i = 0; i < m; i++)
                {
                    r[i] = (float)Math.Sqrt(x[0] + x[1] * vals[i, 1] + x[2] * vals[i, 0] * vals[i, 0]) + x[3] * (vals[i, 0] - vals[i, 1]) + 1000 * x[1] * x[2] - bs[i];

                    if (computeGrads)
                    {
                        float temp = 0.5f / (float)Math.Sqrt(x[0] + x[1] * vals[i, 1] + x[2] * vals[i, 0] * vals[i, 0]);
                        gradR[i, 0] = 1.0f * temp + 1000.0f * x[2];
                        gradR[i, 1] = vals[i, 1] * temp + 1000.0f * x[1];
                        gradR[i, 2] = vals[i, 0] * vals[i, 0] * temp;
                        gradR[i, 3] = (vals[i, 0] - vals[i, 1]);
                    }
                }
            }


            #endregion

            #region Nonlinear optimization problem constructor
            /// <summary>Nonlinear optimization problem</summary>
            public class NonlinearOptimizProblem
            {
                /// <summary>Generic residue function of type F(x) - y</summary>
                public abstract class ResidueFunction
                {
                    /// <summary>Desired value of this function</summary>
                    public float y;
                    /// <summary>Current values of local variables</summary>
                    public float[] X;
                    /// <summary>Mapping of local indexes to global indexes</summary>
                    public int[] GlobalIndex;

                    /// <summary>Computes residue and gradient of this Residue function</summary>
                    /// <param name="x">Global optimization variable</param>
                    /// <param name="ComputeGrad">Compute gradient?</param>
                    /// <param name="Gradient">Global gradient of this function</param>
                    public float ComputeResidueGradient(float[] x, bool ComputeGrad, out float[] Gradient)
                    {
                        for (int i = 0; i < X.Length; i++)
                        {
                            if (GlobalIndex[i] >= 0) X[i] = x[GlobalIndex[i]];
                        }

                        float res; float[] LocalGrad;
                        if (ComputeGrad)
                        {
                            res = F(true, out LocalGrad) - this.y;
                            Gradient = new float[x.Length];

                            for (int i = 0; i < X.Length; i++)
                            {
                                if (GlobalIndex[i] >= 0) Gradient[GlobalIndex[i]] = LocalGrad[i];
                            }

                        }
                        else
                        {
                            res = F(false, out LocalGrad) - this.y;
                            Gradient = null;
                        }


                        return res;
                    }

                    /// <summary>For simulation purposes, compute local Y as a function of a global optimization variable</summary>
                    /// <param name="x">Optimization variable</param>
                    public void InitY(float[] x)
                    {
                        this.y = 0;
                        float[] G;
                        this.y = ComputeResidueGradient(x, false, out G);
                    }

                    /// <summary>Computes function value and gradient using local information</summary>
                    /// <param name="ComputeGrad">Compute gradient?</param>
                    /// <param name="Gradient">Local gradient output</param>
                    public abstract float F(bool ComputeGrad, out float[] Gradient);
                }

                /// <summary>List of residue functions of this nonlinear optimization problem</summary>
                public List<ResidueFunction> ResidueFunctions = new List<ResidueFunction>();

                /// <summary>Computes residues and gradients of residue functions of this nonlinear optimization problem</summary>
                /// <param name="x">Global optimization vector</param>
                /// <param name="r">Residues</param>
                /// <param name="gradR">Gradient</param>
                /// <param name="ComputeGrads">Compute gradients?</param>
                public void ComputeResidueGrad(float[] x, ref float[] r, ref float[,] gradR, bool ComputeGrads)
                {
                    if (!ComputeGrads)
                    {
                        float[] Grad;
                        for (int i = 0; i < ResidueFunctions.Count; i++)
                        {
                            r[i] = ResidueFunctions[i].ComputeResidueGradient(x, false, out Grad);
                        }
                    }
                    else
                    {
                        float[] Grad;
                        for (int i = 0; i < ResidueFunctions.Count; i++)
                        {
                            r[i] = ResidueFunctions[i].ComputeResidueGradient(x, true, out Grad);
                            for (int j = 0; j < ResidueFunctions[i].GlobalIndex.Length; j++)
                            {
                                if (ResidueFunctions[i].GlobalIndex[j] >= 0) gradR[i, ResidueFunctions[i].GlobalIndex[j]] = Grad[ResidueFunctions[i].GlobalIndex[j]];
                            }
                        }

                    }
                }

                /// <summary>Solves this nonlinear optimization problem</summary>
                /// <param name="x">Global optimization vector, initial guess</param>
                /// <param name="MAXITER">Maximum number of iterations</param>
                public float[] Solve(float[] x, int MAXITER)
                {
                    double err = 0;
                    return floatSymPosDefMatrix.NonLinearLS(this.ComputeResidueGrad, x, ResidueFunctions.Count, MAXITER, ref err);
                }

                /// <summary>Example residue functions</summary>
                public static class SampleResidueFunctions
                {
                    /// <summary>Sample residues</summary>
                    public class ResidueExp : ResidueFunction
                    {
                        /// <summary>Constructor</summary>
                        /// <param name="T">Desired function value</param>
                        public ResidueExp(float T)
                        {
                            this.X = new float[2];
                            this.GlobalIndex = new int[2];
                            this.t = T;
                        }
                        /// <summary>Desired function value</summary>
                        public float t = 1.0f;

                        /// <summary>Function F</summary>
                        /// <param name="ComputeGrad">Compute gradient?</param>
                        /// <param name="Gradient">Returns gradient</param>
                        public override float F(bool ComputeGrad, out float[] Gradient)
                        {
                            float temp = t * X[0] * X[1] + X[1] * (float)Math.Exp(t * 0.1);
                            float resp = temp;

                            if (ComputeGrad)
                            {
                                Gradient = new float[2];
                                Gradient[0] = t * X[1];
                                Gradient[1] = (float)Math.Exp(t * 0.1) + t * X[0];
                            }
                            else Gradient = null;

                            return resp;
                        }
                    }
                }
            }
            #endregion

        }

        /// <summary>Functions to create and manipulate vectors</summary>
        public class floatVector
        {
            /// <summary>Vetor values</summary>
            public float[] Values;
            /// <summary>Vetor values in CL memory</summary>
            public CLCalc.Program.Variable CLValues;
            /// <summary>Vetor coefficient for combinations</summary>
            public CLCalc.Program.Variable CLCoef;

            #region Buffers for vector sum

            /// <summary>Vector dimension in CL memory</summary>
            public CLCalc.Program.Variable CLn;
            /// <summary>Partial sums in CL memory</summary>
            public CLCalc.Program.Variable CLresps;

            #endregion

            /// <summary>Vector dimension</summary>
            public int Length
            {
                get
                {
                    return Values.Length;
                }
            }
            #region Constructors
            /// <summary>OpenCL vector constructor</summary>
            /// <param name="Vals">Vector elements</param>
            public floatVector(float[] Vals)
            {
                this.Values = (float[])Vals.Clone();
                if (UseOpenCLIfAvailable && CLCalc.CLAcceleration == CLCalc.CLAccelerationType.UsingCL)
                {
                    CLValues = new CLCalc.Program.Variable(Values);
                    CLCoef = new CLCalc.Program.Variable(new float[1]);
                }
            }

            /// <summary>Creates vector from M elements sequentially</summary>
            /// <param name="symM">Symmetric matrix to use</param>
            public floatVector(floatSymPosDefMatrix symM)
            {
                this.CLValues = symM.CLValues;
                this.Values = symM.Values;

                //Since I'm probably going to modify the matrix, I want a new Cholesky factorization
                //if I ever call a LinearSolve
                symM.IsCholeskyFactorized = false;

                if (UseOpenCLIfAvailable && CLCalc.CLAcceleration == CLCalc.CLAccelerationType.UsingCL)
                {
                    CLCoef = new CLCalc.Program.Variable(new float[1]);
                }
            }
            #endregion

            #region Methods

            /// <summary>Returns the sum of components of this vector</summary>
            public float Sum()
            {
                return BLAS.SumVectorElements(this);
            }

            /// <summary>Computes the Euclidean norm of this  vector</summary>
            /// <param name="temp">Holds temporary operations</param>
            public float Norm(ref floatVector temp)
            {
                if (temp == null) temp = new floatVector(new float[this.Values.Length]);

                floatLinalg.BLAS.ElemWiseSquare(this, ref temp);

                return (float)Math.Sqrt(temp.Sum());
            }

            /// <summary>Computes square of the Euclidean norm of this  vector</summary>
            /// <param name="temp">Holds temporary operations</param>
            public float NormSquared(ref floatVector temp)
            {
                if (temp == null) temp = new floatVector(new float[this.Values.Length]);

                floatLinalg.BLAS.ElemWiseSquare(this, ref temp);

                return temp.Sum();
            }

            /// <summary>Returns true if this vector has any positive entries</summary>
            /// <returns></returns>
            public bool HasPositiveEntries()
            {
                return BLAS.HasPositiveEntries(this);
            }

            /// <summary>Reads from OpenCL if using OpenCL</summary>
            public void ReadFromDevice()
            {
                if (UseOpenCLIfAvailable && CLCalc.CLAcceleration == CLCalc.CLAccelerationType.UsingCL) CLValues.ReadFromDeviceTo(Values);
            }
            #endregion

            #region Write to disk

            /// <summary>Writes contents of this vector to disk</summary>
            /// <param name="file"></param>
            public void Write(string file)
            {
                this.ReadFromDevice();
                using (StreamWriter sw = new StreamWriter(file))
                {
                    if (UseOpenCLIfAvailable && CLCalc.CLAcceleration == CLCalc.CLAccelerationType.UsingCL) CLValues.ReadFromDeviceTo(Values);
                    sw.WriteLine("# Created by OpenCLTemplate " + DateTime.Now.ToString());
                    sw.WriteLine("# name: q");
                    sw.WriteLine("# type: matrix");
                    sw.WriteLine("# rows: " + this.Length.ToString());
                    sw.WriteLine("# columns: 1");

                    for (int i = 0; i < this.Length; i++)
                    {
                        string s = "";
                        s = s + " " + Values[i].ToString().Replace(",", ".");
                        sw.WriteLine(s);
                    }
                }
            }

            #endregion
        }

        /// <summary>Generic matrix</summary>
        public class floatMatrix
        {
            /// <summary>Vector representation of the matrix values</summary>
            public float[] Values;

            /// <summary>Matrix values in CL memory</summary>
            public CLCalc.Program.Variable CLValues;
            /// <summary>Coefficient for combinations</summary>
            public CLCalc.Program.Variable CLCoef;
            /// <summary>Matrix dimensions in CL memory</summary>
            public CLCalc.Program.Variable CLDim;

            #region Buffers for matrix sum

            /// <summary>Vector dimension in CL memory</summary>
            public CLCalc.Program.Variable CLn;
            /// <summary>Partial sums in CL memory</summary>
            public CLCalc.Program.Variable CLresps;

            #endregion

            /// <summary>Number of rows</summary>
            private int nRows;
            /// <summary>Number of columns</summary>
            private int nCols;

            /// <summary>Matrix dimension</summary>
            public int Rows
            {
                get
                {
                    return nRows;
                }
            }
            /// <summary>Matrix dimension</summary>
            public int Cols
            {
                get
                {
                    return nCols;
                }
            }

            /// <summary>Retrieve matrix elements</summary>
            /// <param name="i">Row</param>
            /// <param name="j">Column</param>
            public float this[int i, int j]
            {
                get
                {
                    return Values[j + nCols * i];
                }
            }

            /// <summary>New matrix constructor</summary>
            /// <param name="Vals">Matrix values</param>
            public floatMatrix(float[,] Vals)
            {
                nRows = Vals.GetLength(0);
                nCols = Vals.GetLength(1);
                Values = new float[nRows * nCols];

                if (UseOpenCLIfAvailable && CLCalc.CLAcceleration == CLCalc.CLAccelerationType.UsingCL)
                {
                    CLValues = new CLCalc.Program.Variable(Values);
                    CLDim = new CLCalc.Program.Variable(new int[] { nRows, nCols });
                    CLCoef = new CLCalc.Program.Variable(new float[1]);
                }

                SetValues(Vals);
            }

            /// <summary>Sets values for this matrix</summary>
            /// <param name="Vals">New values</param>
            public void SetValues(float[,] Vals)
            {
                if (nRows != Vals.GetLength(0) || nCols != Vals.GetLength(1)) throw new Exception("Invalid dimension");

                Values = new float[nRows * nCols];

                for (int i = 0; i < nRows; i++)
                    for (int j = 0; j < nCols; j++)
                        Values[j + nCols * i] = Vals[i, j];


                if (UseOpenCLIfAvailable && CLCalc.CLAcceleration == CLCalc.CLAccelerationType.UsingCL)
                    CLValues.WriteToDevice(Values);
            }

            /// <summary>Returns the sum of components of this matrix</summary>
            public float Sum()
            {
                return BLAS.SumMatrixElements(this);
            }

            #region Write to disk
            /// <summary>Writes this matrix to file in Octave format</summary>
            /// <param name="file">File to write to</param>
            public void Write(string file)
            {
                using (StreamWriter sw = new StreamWriter(file))
                {
                    if (UseOpenCLIfAvailable && CLCalc.CLAcceleration == CLCalc.CLAccelerationType.UsingCL) CLValues.ReadFromDeviceTo(Values);
                    sw.WriteLine("# Created by OpenCLTemplate " + DateTime.Now.ToString());
                    sw.WriteLine("# name: A");
                    sw.WriteLine("# type: matrix");
                    sw.WriteLine("# rows:" + this.Rows.ToString());
                    sw.WriteLine("# columns:" + this.Cols.ToString());

                    for (int i = 0; i < this.Rows; i++)
                    {
                        string s = "";
                        for (int j = 0; j < this.Cols; j++)
                        {
                            s = s + " " + this[i, j].ToString().Replace(",", ".");
                        }
                        sw.WriteLine(s);
                    }
                }
            }
            #endregion
        }

        /// <summary>Diagonal matrix</summary>
        public class floatDiag
        {
            /// <summary>Values of diagonal elements</summary>
            public float[] Values;
            /// <summary>Values of diagonal elements in OpenCL memory</summary>
            public CLCalc.Program.Variable CLValues;

            private int nRows, nCols;

            /// <summary>Matrix dimension</summary>
            public int Rows
            {
                get
                {
                    return nRows;
                }
            }
            /// <summary>Matrix dimension</summary>
            public int Cols
            {
                get
                {
                    return nCols;
                }
            }

            /// <summary>OpenCL diagonal matrix constructor</summary>
            /// <param name="Vals">Main diagonal elements</param>
            public floatDiag(float[] Vals)
            {
                this.Values = (float[])Vals.Clone();
                if (UseOpenCLIfAvailable && CLCalc.CLAcceleration == CLCalc.CLAccelerationType.UsingCL)
                {
                    CLValues = new CLCalc.Program.Variable(Values);
                }
                nRows = Vals.Length;
                nCols = Vals.Length;
            }

            /// <summary>Creates the diagonal matrix D(v) with elements associated to those of vector v. Uses the same objects.</summary>
            /// <param name="v">Reference vector</param>
            public floatDiag(floatVector v)
            {
                nRows = v.Length;
                nCols = v.Length;
                this.Values = v.Values;
                this.CLValues = v.CLValues;
            }
        }

        #endregion


    }

    /// <summary>Encapsulates optimization techniques based on Newton methods</summary>
    public class floatOptimization
    {
        #region Unconstrained minimization

        /// <summary>Unconstrained minimization driver</summary>
        public static class UnconstrainedMinimization
        {
            /// <summary>Configuration parameters (v. Boyd's Convex Optimization)</summary>
            public static class Config
            {
                private static float _alpha = 0.02f, _beta = 0.5f, _eps = 4E-6f;
                private static int MAXITER = 50;

                /// <summary>Backtrack line search alpha parameter</summary>
                public static float alpha
                {
                    get { return _alpha; }
                    set
                    {
                        if (value < 0 || value > 0.5f) throw new Exception("alpha value should be in range (0, 0.5)");
                        else _alpha = value;
                    }
                }

                /// <summary>Backtrack line search beta parameter</summary>
                public static float beta
                {
                    get { return _beta; }
                    set
                    {
                        if (value < 0 || value > 1.0f) throw new Exception("beta value should be in range (0, 1)");
                        else _beta = value;
                    }
                }

                /// <summary>Unconstrained minimization precision</summary>
                public static float eps
                {
                    get { return _eps; }
                    set { eps = Math.Abs(value); }
                }

                /// <summary>Maximum number of iterations</summary>
                public static int MaxIterations
                {
                    get { return MAXITER; }
                    set
                    {
                        MAXITER = Math.Abs(value);
                    }
                }
            }

            /// <summary>Function to be used in unconstrained minimization method. Should compute function value and search direction if requested
            /// (i.e., probably solve a Newton system)</summary>
            /// <param name="CLx">Current function point</param>
            /// <param name="ComputeGradHess">Compute gradient and hessian?</param>
            /// <param name="Grad">Gradientof F, if requested. Should not be computed if not requested</param>
            /// <param name="Hess">Hessian of F, if requested. Should not be computed if not requested</param>
            public delegate float C2Function(floatLinalg.floatVector CLx, bool ComputeGradHess, ref floatLinalg.floatVector Grad, ref floatLinalg.floatSymPosDefMatrix Hess);


            /// <summary>Computes the unconstrained minimum of function F in x0.Length dimensions</summary>
            /// <param name="x0">Start point. Problem dimension is assumed to be x0.Length</param>
            /// <param name="F">Function to minimize</param>
            /// <param name="iters">Number of iterations used to solve the problem</param>
            public static float[] Solve(float[] x0, C2Function F, out int iters)
            {
                floatLinalg.floatVector CLx = new floatLinalg.floatVector(x0);

                //Creates Hessian and Gradient
                floatLinalg.floatSymPosDefMatrix HessF = new floatLinalg.floatSymPosDefMatrix(x0.Length);
                floatLinalg.floatVector gradF = new floatLinalg.floatVector(x0);

                //Line search variables
                floatLinalg.floatVector temp = new floatLinalg.floatVector(x0);
                floatLinalg.floatVector deltaX = new floatLinalg.floatVector(x0);

                return Solve(CLx, F, out iters, HessF, gradF, temp, deltaX);
            }

            /// <summary>Computes the unconstrained minimum of function F in x0.Length dimensions subject to equality constraint Ax=b</summary>
            /// <param name="x0">Start point. Problem dimension is assumed to be x0.Length</param>
            /// <param name="F">Function to minimize</param>
            /// <param name="A">Equality constraint matrix</param>
            /// <param name="b">Equality constraint rhs</param>            
            /// <param name="iters">Number of iterations used to solve the problem</param>
            public static float[] Solve(float[] x0, C2Function F, float[,] A, float[] b, out int iters)
            {
                floatLinalg.floatVector CLx = new floatLinalg.floatVector(x0);

                //Creates Hessian and Gradient
                floatLinalg.floatSymPosDefMatrix HessF = new floatLinalg.floatSymPosDefMatrix(x0.Length);
                floatLinalg.floatVector gradF = new floatLinalg.floatVector(x0);

                //Line search variables
                floatLinalg.floatVector temp = new floatLinalg.floatVector(x0);
                floatLinalg.floatVector deltaX = new floatLinalg.floatVector(x0);

                
                //Equality constraint vars
                if (A != null)
                {
                    int c = A.GetLength(0);
                    int n = x0.Length;
                    floatLinalg.floatMatrix CLA = new floatLinalg.floatMatrix(A);
                    floatLinalg.floatVector CLb = new floatLinalg.floatVector(b);
                    floatLinalg.floatVector CLDeltaNu = new floatLinalg.floatVector(b);

                    floatLinalg.floatVector rPri = new floatLinalg.floatVector(new float[c]);
                    floatLinalg.floatVector Atnu = new floatLinalg.floatVector(new float[n]);
                    floatLinalg.floatVector tempC = new floatLinalg.floatVector(new float[c]);
                    floatLinalg.floatVector temp2 = new floatLinalg.floatVector(x0);

                    floatLinalg.floatSymPosDefMatrix AinvHAt = new floatLinalg.floatSymPosDefMatrix(c);
                    floatLinalg.floatVector AinvHrhs = new floatLinalg.floatVector(new float[c]);
                    floatLinalg.floatMatrix tempAinvMatrix = new floatLinalg.floatMatrix(new float[n, c]);

                    float[] fOnes = new float[c];
                    for (int i = 0; i < fOnes.Length; i++) fOnes[i] = 1;
                    floatLinalg.floatVector onesC = new floatLinalg.floatVector(fOnes);
                    return Solve(CLx, F, out iters, HessF, gradF, temp, deltaX, CLA, CLb, AinvHAt, tempAinvMatrix, temp2, AinvHrhs, rPri, tempC, CLDeltaNu, Atnu, onesC);
                }
                else return Solve(CLx, F, out iters, HessF, gradF, temp, deltaX, null, null, null, null, null, null, null, null, null, null, null);

            }

            /// <summary>Computes the unconstrained minimum of function F in x0.Length dimensions</summary>
            /// <param name="CLx">Solution x and initial guess</param>
            /// <param name="F">Function to be minimized</param>
            /// <param name="iters">Number of iterations</param>
            /// <param name="HessF">Holder of hessian of F</param>
            /// <param name="gradF">Holder of gradient of F</param>
            /// <param name="temp">Temporary vector of dimension n</param>
            /// <param name="deltaX">X search direction</param>
            public static float[] Solve(floatLinalg.floatVector CLx, C2Function F, out int iters,
                floatLinalg.floatSymPosDefMatrix HessF, floatLinalg.floatVector gradF, floatLinalg.floatVector temp, floatLinalg.floatVector deltaX)
            {
                return Solve(CLx, F, out iters, HessF, gradF, temp, deltaX, null, null, null, null, null, null, null, null, null, null, null);
            }


            /// <summary>Computes the unconstrained minimum of function F in x0.Length dimensions</summary>
            /// <param name="CLx">Solution x and initial guess</param>
            /// <param name="F">Function to be minimized</param>
            /// <param name="iters">Number of iterations</param>
            /// <param name="HessF">Holder of hessian of F</param>
            /// <param name="gradF">Holder of gradient of F</param>
            /// <param name="temp">Temporary vector of dimension n</param>
            /// <param name="deltaX">X search direction</param>
            /// <param name="A">Equality constraint matrix</param>
            /// <param name="b">Equality constraint rhs</param>
            /// <param name="AinvHAt">Holder of A*inv(H)*A'</param>
            /// <param name="tempAinvMatrix">Temporary matrix for computation of inv(H)*A'</param>
            /// <param name="temp2">Temporary vector of length n</param>
            /// <param name="AinvHrhs">Temporary vector</param>
            /// <param name="rPri">Primal residual</param>
            /// <param name="tempC">Temporary vector of length C</param>
            /// <param name="CLDeltaNu">Search direction in Nu</param>
            /// <param name="Atnu">A times nu</param>
            /// <param name="onesC">Vector of ones of dimension C</param>
            public static float[] Solve(floatLinalg.floatVector CLx, C2Function F, out int iters, 
                floatLinalg.floatSymPosDefMatrix HessF, floatLinalg.floatVector gradF, floatLinalg.floatVector temp, floatLinalg.floatVector deltaX,

                floatLinalg.floatMatrix A, floatLinalg.floatVector b, floatLinalg.floatSymPosDefMatrix AinvHAt, floatLinalg.floatMatrix tempAinvMatrix, 
                floatLinalg.floatVector temp2, floatLinalg.floatVector AinvHrhs, floatLinalg.floatVector rPri, floatLinalg.floatVector tempC, floatLinalg.floatVector CLDeltaNu,
                floatLinalg.floatVector Atnu, floatLinalg.floatVector onesC)
            {
                float lambda = 1E30f; //, lambdaPrev = 2E30f;
                iters = 0;

                while (lambda > Config.eps && iters < Config.MaxIterations)
                {
                    //Computes gradient and hessian
                    F(CLx, true, ref gradF, ref HessF);
                    HessF.IsCholeskyFactorized = false;

                    //Search direction is minus gradient
                    floatLinalg.BLAS.LinearCombination(0, CLx, -1, gradF, ref temp);

                    if (A == null)
                    {
                        //Finds Newton step without equality constraints
                        deltaX = HessF.LinearSolve(temp, false, ref deltaX);

                        //HessF.CLcholDec.ReadFromDeviceTo(HessF.cholDec);
                        //HessF.Write("C:\\oct\\PP.txt");
                        //temp.Write("C:\\oct\\q.txt");
                        //deltaX.ReadFromDevice();
                    }
                    else
                    {
                        //Solves KKT system [Hpd A'; A 0] = [temp; rPri]. Remember that rPri was computed as -Ax+b
                        //primal residual, -rPri
                        floatLinalg.BLAS.MatrVecProdSumVec(A, CLx, -1, b, 1, ref rPri);

                        //A inv(H) A'
                        floatLinalg.BLAS.ComputeAinvHTranspA(A, HessF, ref AinvHAt, ref tempAinvMatrix, false);
                        
                        //A inv(H) temp
                        HessF.LinearSolve(temp, false, ref temp2);
                        floatLinalg.BLAS.MatrVecProd(A, temp2, 1, ref AinvHrhs);

                        //Ainv(H)temp - rpri
                        floatLinalg.BLAS.LinearCombination(1, AinvHrhs, -1, rPri, ref tempC);

                        //Solves for deltaNu
                        AinvHAt.LinearSolve(tempC, false, ref CLDeltaNu);

                        //Procceeds to deltaX = invH * (deltaX - AtDeltanu)
                        floatLinalg.BLAS.MatrTraspVecMult(A, new floatLinalg.floatDiag(onesC), CLDeltaNu, ref Atnu);
                        floatLinalg.BLAS.LinearCombination(-1, Atnu, 1, temp, ref temp2);

                        HessF.LinearSolve(temp2, false, ref deltaX);
                    }

                    //Finds step that leads to decrement
                    BackTrack(F, CLx, deltaX, gradF, ref temp, ref HessF);


                    ////////DEBUG
                    //if (UseOpenCLIfAvailable && CLCalc.CLAcceleration == CLCalc.CLAccelerationType.UsingCL)
                    //{
                    //    HessF.CLValues.ReadFromDeviceTo(HessF.Values);
                    //    HessF.CLcholDec.ReadFromDeviceTo(HessF.cholDec);
                    //}

                    ////CLx.CLValues.ReadFromDeviceTo(CLx.Values);
                    ////deltaX.CLValues.ReadFromDeviceTo(deltaX.Values);
                    //gradF.ReadFromDevice();
                    //deltaX.ReadFromDevice();
                    //CLx.ReadFromDevice();

                    lambda = (float)Math.Sqrt(-floatLinalg.BLAS.Dot(gradF, deltaX, ref temp));

                    iters++;
                }

                if (floatLinalg.UseOpenCLIfAvailable && CLCalc.CLAcceleration == CLCalc.CLAccelerationType.UsingCL) CLx.CLValues.ReadFromDeviceTo(CLx.Values);
                return CLx.Values;
            }

            /// <summary>Line search algorithm</summary>
            /// <param name="f">Function to be searched</param>
            /// <param name="x">Current point</param>
            /// <param name="deltaX">Search direction</param>
            /// <param name="gradF">Gradient of F</param>
            /// <param name="temp">Temporary vector to store current direction</param>
            /// <param name="HessF">Hessian of F</param>
            private static void BackTrack(C2Function f, floatLinalg.floatVector x, floatLinalg.floatVector deltaX, 
                floatLinalg.floatVector gradF, ref floatLinalg.floatVector temp, ref floatLinalg.floatSymPosDefMatrix HessF)
            {
                float t = 1;

                float alphagradFdeltaX = floatLinalg.BLAS.Dot(gradF, deltaX, ref temp) * Config.alpha;

                floatLinalg.floatVector dum = null;
                float valF = f(x, false, ref dum, ref HessF);

                //temp holds f(x+t*deltax)
                floatLinalg.BLAS.LinearCombination(1, x, t, deltaX, ref temp);
                float valFupdt = f(temp, false, ref dum, ref HessF);

                while ((t > 0) && (float.IsNaN(valFupdt) || valFupdt > valF + t * alphagradFdeltaX))
                {
                    floatLinalg.BLAS.LinearCombination(1, x, t, deltaX, ref temp);
                    valFupdt = f(temp, false, ref dum, ref HessF);
                    t *= Config.beta;
                }

                if (t > 0)
                {
                    //Consolidates deltaX
                    floatLinalg.BLAS.LinearCombination(1, temp, -1, x, ref deltaX);

                    //Updates x
                    floatLinalg.BLAS.CopyVector(temp, x);
                }
            }

        }
        #endregion

        #region Curve fitting applications
        /// <summary>Curve fitting applications</summary>
        public static class CurveFitting
        {
            #region Solution of regularized least-squares problems

            /// <summary>Computes least squares fitting of Ax = b weighted by W and returns x</summary>
            /// <param name="A">Dependent variables measurements</param>
            /// <param name="b">Independent variables measurements</param>
            /// <param name="W">Weights</param>
            /// <param name="lambda">Regularization term</param>
            public static float[] LeastSquares(float[,] A, float[] b, float[] W, float lambda)
            {
                floatLinalg.floatMatrix CLA = new floatLinalg.floatMatrix(A);
                floatLinalg.floatVector CLb = new floatLinalg.floatVector(b);


                if (W != null && W.Length != CLA.Rows) throw new Exception("Incompatible Weight dimensions");

                floatLinalg.floatDiag CLW;
                if (W == null)
                {
                    float[] ww = new float[CLA.Rows];
                    for (int i = 0; i < ww.Length; i++) ww[i] = 1;
                    CLW = new floatLinalg.floatDiag(ww);
                }
                else CLW = new floatLinalg.floatDiag(W);

                float[] lambdas = new float[CLA.Cols];
                for (int i = 0; i < lambdas.Length; i++) lambdas[i] = lambda;
                floatLinalg.floatVector CLlambda = new floatLinalg.floatVector(lambdas);

                floatLinalg.floatSymPosDefMatrix AtA = null;
                AtA = floatLinalg.BLAS.MatrTranspMatrProd(CLA, CLW, CLlambda, ref AtA);

                //CLA.CLValues.ReadFromDeviceTo(CLA.Values);
                //AtA.CLValues.ReadFromDeviceTo(AtA.Values);

                floatLinalg.floatVector Atb = null;
                Atb = floatLinalg.BLAS.MatrTraspVecMult(CLA, CLW, CLb, ref Atb);

                //Atb.CLValues.WriteToDevice(Atb.Values);

                GC.Collect();
                floatLinalg.floatVector resp = null;
                AtA.LinearSolve(Atb, true, ref resp);

                if (floatLinalg.UseOpenCLIfAvailable && CLCalc.CLAcceleration == CLCalc.CLAccelerationType.UsingCL) resp.CLValues.ReadFromDeviceTo(resp.Values);
                return resp.Values;
            }

            /// <summary>Computes least squares fitting of Ax = b and returns x</summary>
            /// <param name="A">Dependent variables measurements</param>
            /// <param name="b">Independent variables measurements</param>
            public static float[] LeastSquares(float[,] A, float[] b)
            {
                return LeastSquares(A, b, null, 0);
            }

            /// <summary>Computes least squares fitting of Ax = b and returns x</summary>
            /// <param name="A">Dependent variables measurements</param>
            /// <param name="b">Independent variables measurements</param>
            /// <param name="lambda">Regularization term</param>            
            public static float[] LeastSquares(float[,] A, float[] b, float lambda)
            {
                return LeastSquares(A, b, null, lambda);
            }

            #endregion

            #region p-norm regularization

            /// <summary>Computes the p-norm minimization of Ax - b with weights w, using q-norm regularization with weights lambda on x</summary>
            /// <param name="x0">Start point</param>
            /// <param name="A">Dependent variables</param>
            /// <param name="b">Independent variables measurements</param>
            /// <param name="W">Weights of each equation</param>
            /// <param name="lambda">Regularization term of each component of x</param>
            /// <param name="p">Ax - b minimization exponent</param>
            /// <param name="q">x regularization exponent</param>
            public static float[] PNormMinimization(float[] x0, float[,] A, float[] b, float[] W, float[] lambda, float p, float q)
            {
                return PNormMinimization(x0, A, b, W, lambda, p, q, null, null);
            }

            /// <summary>Computes the p-norm minimization of Ax - b with weights w, using q-norm regularization with weights lambda on x</summary>
            /// <param name="x0">Start point</param>
            /// <param name="A">Dependent variables</param>
            /// <param name="b">Independent variables measurements</param>
            /// <param name="W">Weights of each equation</param>
            /// <param name="lambda">Regularization term of each component of x</param>
            /// <param name="p">Ax - b minimization exponent</param>
            /// <param name="q">x regularization exponent</param>
            /// <param name="AeqConstr">Equality constraint matrix AeqConstr * x = bConstr</param>
            /// <param name="bEqConstr">Equality constraint right hand side</param>
            public static float[] PNormMinimization(float[] x0, float[,] A, float[] b, float[] W, float[] lambda, float p, float q, float[,] AeqConstr, float[] bEqConstr)
            {
                //Dimensionality check
                if (A.GetLength(1) != x0.Length) throw new Exception("Number of columns of A should be x.Length");
                if (A.GetLength(0) != b.Length) throw new Exception("Number of rows of A should be b.Length");
                if (A.GetLength(0) != W.Length) throw new Exception("Number of weights should be equal to b.Length");
                if (A.GetLength(1) != lambda.Length) throw new Exception("Number of lambda weights should be equal to x.Length");
                if (p < 1 || q < 1) throw new Exception("p and q values should be greater than 1");
                if (AeqConstr != null && AeqConstr.GetLength(1) != x0.Length) throw new Exception("Number of columns of AeqConstr should be x.Length");
                if (AeqConstr != null && bEqConstr.Length != AeqConstr.GetLength(0)) throw new Exception("bEqConstr.Length not compatible with AeqConstr");

                float[] resp = null;

                floatLinalg.floatMatrix CLA = new floatLinalg.floatMatrix(A);
                //floatLinalg.floatVector CLx = new floatLinalg.floatVector(x0);
                floatLinalg.floatVector CLb = new floatLinalg.floatVector(b);
                floatLinalg.floatVector CLW = new floatLinalg.floatVector(W);
                floatLinalg.floatVector CLlambda = new floatLinalg.floatVector(lambda);
                floatLinalg.floatVector CLp = new floatLinalg.floatVector(new float[] { p });
                floatLinalg.floatVector CLq = new floatLinalg.floatVector(new float[] { q });

                PNormMinClass pnm = new PNormMinClass(CLA, CLb, CLW, CLlambda, CLp, CLq);

                floatLinalg.floatVector x = new floatLinalg.floatVector(x0);
                floatLinalg.floatVector g = new floatLinalg.floatVector(x0);
                floatLinalg.floatSymPosDefMatrix h = new floatLinalg.floatSymPosDefMatrix(x0.Length);

                //pnm.F(x, true, ref g, ref h);
                int iters;
                resp = UnconstrainedMinimization.Solve(x0, pnm.F, AeqConstr, bEqConstr, out iters);

                return resp;
            }

            /// <summary>Computes p-norm of a vector, sum(|xi|^p)</summary>
            public static CLCalc.Program.Kernel kernelpNorm;
            /// <summary>Computes gradients of p-norm</summary>
            public static CLCalc.Program.Kernel kerneldpNorm;

            private class PNormMinClass
            {
                private floatLinalg.floatMatrix A;
                private floatLinalg.floatVector b, w, lambda;
                private floatLinalg.floatVector CLp, CLq;
                private floatLinalg.floatDiag Diagw, Diaglambda;

                //temporary
                private floatLinalg.floatVector tempX, tempdX, tempd2x, tempAxmb, Axmb, tempAtz;
                private floatLinalg.floatVector dtempAtz, d2tempAtz;
                private floatLinalg.floatDiag Identity, Diagd2Atz;

                public PNormMinClass(floatLinalg.floatMatrix A, floatLinalg.floatVector b, floatLinalg.floatVector w,
                    floatLinalg.floatVector lambda, floatLinalg.floatVector CLp, floatLinalg.floatVector CLq)
                {
                    this.A = A;
                    this.b = b;
                    this.w = w;
                    this.lambda = lambda;
                    this.CLp = CLp;
                    this.CLq = CLq;

                    //Temporary buffers
                    tempX = new floatLinalg.floatVector(new float[A.Cols]);
                    tempdX = new floatLinalg.floatVector(new float[A.Cols]);

                    tempAxmb = new floatLinalg.floatVector(new float[A.Rows]);
                    Axmb = new floatLinalg.floatVector(new float[A.Rows]);

                    Diagw = new floatLinalg.floatDiag(w);
                    Diaglambda = new floatLinalg.floatDiag(lambda);

                    tempdX = new floatLinalg.floatVector(new float[lambda.Length]);
                    tempd2x = new floatLinalg.floatVector(new float[lambda.Length]);
                    tempAtz = new floatLinalg.floatVector(new float[lambda.Length]);

                    dtempAtz = new floatLinalg.floatVector(new float[w.Length]);
                    d2tempAtz = new floatLinalg.floatVector(new float[w.Length]);
                    Diagd2Atz = new floatLinalg.floatDiag(d2tempAtz);

                    float[] id = new float[A.Rows];
                    for (int i = 0; i < A.Rows; i++) id[i] = 1;
                    Identity = new floatLinalg.floatDiag(id);
                }


                /// <summary>Computes objective function, gradient and Hessian for p-norm minimization with q-norm regularization</summary>
                public float F(floatLinalg.floatVector x, bool ComputeGradHess, ref floatLinalg.floatVector gradF, ref floatLinalg.floatSymPosDefMatrix H)
                {
                    float resp;

                    //Computes Ax-b
                    floatLinalg.BLAS.MatrVecProdSumVec(A, x, 1, b, -1, ref Axmb);
                    floatLinalg.BLAS.CopyVector(Axmb, tempAxmb);
                    floatLinalg.BLAS.CopyVector(x, tempX);

                    if (floatLinalg.UseOpenCLIfAvailable && CLCalc.CLAcceleration == CLCalc.CLAccelerationType.UsingCL)
                    {
                        kernelpNorm.Execute(new CLCalc.Program.Variable[] { tempX.CLValues, CLq.CLValues, lambda.CLValues }, tempX.Length);
                        kernelpNorm.Execute(new CLCalc.Program.Variable[] { tempAxmb.CLValues, CLp.CLValues, w.CLValues }, tempAxmb.Length);
                    }
                    else
                    {
                        for (int i = 0; i < A.Rows; i++)
                        {
                            tempAxmb.Values[i] = (float)Math.Pow(Math.Abs(tempAxmb.Values[i]), CLp.Values[0]) * w.Values[i];
                        }
                        for (int i = 0; i < A.Cols; i++)
                        {
                            tempX.Values[i] = (float)Math.Pow(Math.Abs(tempX.Values[i]), CLq.Values[0]) * lambda.Values[i];
                        }
                    }

                    resp = tempX.Sum() + tempAxmb.Sum();

                    if (ComputeGradHess)
                    {
                        //Atz
                        if (floatLinalg.UseOpenCLIfAvailable && CLCalc.CLAcceleration == CLCalc.CLAccelerationType.UsingCL)
                            kerneldpNorm.Execute(new CLCalc.Program.Variable[] { Axmb.CLValues, CLp.CLValues, w.CLValues, dtempAtz.CLValues, d2tempAtz.CLValues }, tempAxmb.Length);
                        else
                        {
                            for (int i = 0; i < Axmb.Values.Length; i++)
                            {
                                float temp = Axmb.Values[i];

                                dtempAtz.Values[i] = (float)Math.Pow(Math.Abs(temp), CLp.Values[0] - 1.0f) * w.Values[i] * Math.Sign(temp) * CLp.Values[0];
                                d2tempAtz.Values[i] = (float)Math.Pow(Math.Abs(temp), CLp.Values[0] - 2.0f) * w.Values[i] * CLp.Values[0] * (CLp.Values[0] - 1.0f);
                            }
                        }

                        floatLinalg.BLAS.MatrTraspVecMult(A, Identity, dtempAtz, ref tempAtz);

                        //x
                        if (floatLinalg.UseOpenCLIfAvailable && CLCalc.CLAcceleration == CLCalc.CLAccelerationType.UsingCL)
                            kerneldpNorm.Execute(new CLCalc.Program.Variable[] { x.CLValues, CLq.CLValues, lambda.CLValues, tempdX.CLValues, tempd2x.CLValues }, x.Length);
                        else
                        {
                            for (int i = 0; i < x.Values.Length; i++)
                            {
                                float temp = x.Values[i];

                                tempdX.Values[i] = (float)Math.Pow(Math.Abs(temp), CLq.Values[0] - 1.0f) * lambda.Values[i] * Math.Sign(temp) * CLq.Values[0];
                                tempd2x.Values[i] = (float)Math.Pow(Math.Abs(temp), CLq.Values[0] - 2.0f) * lambda.Values[i] * CLq.Values[0] * (CLq.Values[0] - 1.0f);
                            }
                        }
                       
                        //Accumulates gradients
                        floatLinalg.BLAS.LinearCombination(1, tempAtz, 1, tempdX, ref gradF);

                        //d2tempAtz.ReadFromDevice();
                        //tempd2x.ReadFromDevice();

                        //Hessian

                        //A.Write("C:\\oct\\A.txt");
                        //d2tempAtz.Write("C:\\oct\\z2.txt");

                        floatLinalg.BLAS.MatrTranspMatrProd(A, Diagd2Atz, tempd2x, ref H);
                    }

                    return resp;
                }
            }

            #endregion

        }
        #endregion

        #region Quadratic programming

        /// <summary>Quadratic problem solving</summary>
        public class QuadraticProgramming
        {
            #region Solution log
            /// <summary>Log of various parameters of solution</summary>
            public static class SolutionLog
            {
                /// <summary>Keep log of operations?</summary>
                public static bool KeepLog = false;

                /// <summary>Internal variable to keep track if needs to log residuals</summary>
                public static bool LogResiduals = false;

                /// <summary>How many Newton directions were computed?</summary>
                public static int Iterations;
                /// <summary>How did the duality gaps evolve?</summary>
                public static List<float> SurrDualityGaps = new List<float>();
                
                /// <summary>Evolution of last variable in the computation of feasibility. Problem becomes feasible when the var is less than zero</summary>
                public static List<float> EvolutionOfT = new List<float>();

                /// <summary>Sequence of points in problem solution</summary>
                public static List<float[]> PtSequence = new List<float[]>();

                /// <summary>Primal dual step sizes</summary>
                public static List<float> StepSizes = new List<float>();


                /// <summary>Evolution of dual residuals</summary>
                public static List<float> DualResiduals = new List<float>();
                /// <summary>Evolution of centralization residuals</summary>
                public static List<float> CentResiduals = new List<float>();

                /// <summary>Clears log</summary>
                public static void Clear()
                {
                    StepSizes.Clear();
                    PtSequence.Clear();

                    CentResiduals.Clear();
                    DualResiduals.Clear();
                    SurrDualityGaps.Clear();
                    EvolutionOfT.Clear();
                }
            }
            #endregion

            #region Feasibility problem
            /// <summary>Checks if it's possible to satisfy Mx less than d and Ax = b. Returns a feasible point if so</summary>
            /// <param name="x0">Initial guess</param>
            /// <param name="M">Inequality constraint matrix</param>
            /// <param name="d">Inequality rhs</param>
            /// <param name="A">Equality constr matrix</param>
            /// <param name="b">Equality rhs</param>
            public static float[] CheckFeasibility(float[] x0, float[,] M, float[] d, float[,] A, float[] b)
            {
                //Computes Mx-d
                floatLinalg.floatMatrix CLM = new floatLinalg.floatMatrix(M);
                floatLinalg.floatVector CLd = new floatLinalg.floatVector(d);
                floatLinalg.floatVector CLx = new floatLinalg.floatVector(x0);

                floatLinalg.floatMatrix CLA = null;
                floatLinalg.floatVector CLb = null;

                if (A != null)
                {
                    CLA = new floatLinalg.floatMatrix(M);
                    CLb = new floatLinalg.floatVector(d);
                }
                floatLinalg.floatVector Mxd = new floatLinalg.floatVector(new float[M.GetLength(0)]);



                if (CheckFeasibility(CLx, CLM, CLd, CLA, CLb, Mxd))
                {
                    CLx.ReadFromDevice();
                    return (float[])CLx.Values.Clone();
                }
                else return null;
            }

            /// <summary>Checks if it's possible to satisfy Mx less than d and Ax = b. Returns a feasible point in x0 if so</summary>
            /// <param name="x0">Initial guess</param>
            /// <param name="M">Inequality constraint matrix</param>
            /// <param name="d">Inequality rhs</param>
            /// <param name="A">Equality constr matrix</param>
            /// <param name="b">Equality rhs</param>
            /// <param name="Mxd">Temporary vector to hold M*x - d</param>
            public static bool CheckFeasibility(floatLinalg.floatVector x0, floatLinalg.floatMatrix M, floatLinalg.floatVector d, floatLinalg.floatMatrix A,
                floatLinalg.floatVector b, floatLinalg.floatVector Mxd)
            {
                SolutionLog.Clear();

                //Computes Mx - d
                floatLinalg.BLAS.MatrVecProdSumVec(M, x0, 1, d, -1, ref Mxd);

                //Problem is already feasible
                if (!Mxd.HasPositiveEntries())
                {
                    return true;
                }

                //If problem is not feasible, computes largest term
                Mxd.ReadFromDevice();
                float max = 1.0f;
                for (int i = 0; i < Mxd.Values.Length; i++) if (max < Mxd.Values[i]) max = Mxd.Values[i];
                //Augments max to create a feasible point for the feasibility problem
                max *= 1.1f;


                //Constructs the instance of the feasibility problem.
                //Augments M and A by one. M will contain the -1s in the extra row; A has to contain an extra zero in the last additional column
                if (CLCalc.CLAcceleration==CLCalc.CLAccelerationType.UsingCL) M.CLValues.ReadFromDeviceTo(M.Values);

                float[,] Mfeas = new float[M.Rows, M.Cols + 1];
                for (int i = 0; i < M.Rows; i++)
                {
                    for (int j = 0; j < M.Cols; j++)
                    {
                        Mfeas[i, j] = M[i, j];
                    }
                    Mfeas[i, M.Cols] = -1;
                }

                float[,] Afeas = null;
                if (A != null)
                {
                    Afeas = new float[A.Rows, A.Cols + 1];
                    for (int i = 0; i < A.Rows; i++)
                    {
                        for (int j = 0; j < A.Cols; j++)
                        {
                            Afeas[i, j] = A[i, j];
                        }
                        Afeas[i, M.Cols] = 0;
                    }
                }

                //Feasibility problem feasible point
                float[] xFeas = new float[x0.Length + 1];
                x0.ReadFromDevice();
                for (int i = 0; i < x0.Length; i++) xFeas[i] = x0.Values[i];
                xFeas[x0.Length] = max;

                float[] lambda = new float[M.Rows];
                for (int i = 0; i < lambda.Length; i++) lambda[i] = 0.1f;
                float[] nu = null;
                if (A != null) nu = new float[A.Rows];

                //Objective is to minimize last variable
                float[] q = new float[x0.Length + 1];
                //for (int i = 0; i < x0.Length; i++) q[i] = 0.001f;
                q[x0.Length] = 10 * max * (float)lambda.Length * (float)lambda.Length;

                QuadraticProgramming qp = new QuadraticProgramming();

                d.ReadFromDevice();
                float[] bVals = null;
                if (A != null)
                {
                    b.ReadFromDevice();
                    bVals = b.Values;
                }
                
                float[] xF = qp.SolvePrimalDual(xFeas, lambda, nu, null, q, Mfeas, d.Values, Afeas, bVals, feasStopFunc);


                if (xF[xF.Length - 1] < 0) //feasible
                {
                    for (int i = 0; i < x0.Length; i++) x0.Values[i] = xF[i];
                    if (floatLinalg.UseOpenCLIfAvailable && CLCalc.CLAccelerationType.UsingCL == CLCalc.CLAcceleration)
                    {
                        x0.CLValues.WriteToDevice(x0.Values);
                    }
                    return true;
                }
                else //infeasible
                {
                    return false;
                }
            }

            /// <summary>Kernel to compute last element of a vector</summary>
            public static CLCalc.Program.Kernel kernelgetLast;

            static bool feasStopFunc(floatLinalg.floatVector x, floatLinalg.floatVector lambda, floatLinalg.floatVector nu)
            {
                float[] lastElem = new float[1];
                if (floatLinalg.UseOpenCLIfAvailable && CLCalc.CLAcceleration == CLCalc.CLAccelerationType.UsingCL)
                {
                    if (x.CLn == null) x.CLn = new CLCalc.Program.Variable(new int[] { x.Values.Length });
                    kernelgetLast.Execute(new CLCalc.Program.MemoryObject[] { x.CLValues, x.CLn, x.CLCoef }, 1);
                    x.CLCoef.ReadFromDeviceTo(lastElem);
                }
                else
                {
                    lastElem[0] = x.Values[x.Values.Length - 1];
                }

                if (SolutionLog.KeepLog) SolutionLog.EvolutionOfT.Add(lastElem[0]);

                //Stops if last element is less than zero
                return lastElem[0] < 0;
            }

            #endregion


            #region QP vars and its temporary variables
            //Problem vars
            floatLinalg.floatSymPosDefMatrix CLP;
            floatLinalg.floatMatrix CLM;


            //Equality constraint variables
            floatLinalg.floatMatrix CLA;
            floatLinalg.floatVector CLb;
            floatLinalg.floatVector CLnu, CLnuPlus;
            floatLinalg.floatVector CLDeltaNu;


            floatLinalg.floatVector CLx, CLxPlus;
            floatLinalg.floatVector CLlambda, CLlambdaPlus;
            floatLinalg.floatVector CLDeltaX;
            floatLinalg.floatVector CLDeltaLambda;

            floatLinalg.floatVector CLq;
            floatLinalg.floatVector CLd;


            //Auxiliary vars
            floatLinalg.floatVector zerosN;

            floatLinalg.floatVector onesM;
            floatLinalg.floatVector onesC;

            /// <summary>Primal dual Hessian n x n</summary>
            floatLinalg.floatSymPosDefMatrix Hpd;
            floatLinalg.floatSymPosDefMatrix MtDzM;

            floatLinalg.floatVector temp;
            floatLinalg.floatVector temp2, tempM, tempM2, tempC;
            floatLinalg.floatVector rhsXpd;

            /// <summary>Primal residual, c x 1</summary>
            floatLinalg.floatVector rPri;
            /// <summary>Centralization residual, m x 1</summary>
            floatLinalg.floatVector rCent;
            /// <summary>Dual residual, n x 1</summary>
            floatLinalg.floatVector rDual;

            floatLinalg.floatVector Mxd, z2, z;
            floatLinalg.floatVector CLPx;
            floatLinalg.floatVector Atnu;

            floatLinalg.floatSymPosDefMatrix AinvHAt;
            floatLinalg.floatVector AinvHrhs;
            floatLinalg.floatMatrix tempAinvMatrix;
            #endregion


            /// <summary>Delegate to stop execution if that's thhe case (as in the feasibility problem, when we can quit if t less than zero)</summary>
            public delegate bool StopFunc(floatLinalg.floatVector x, floatLinalg.floatVector lambda, floatLinalg.floatVector nu);


            /// <summary>DEBUG. Reads all vars from device</summary>
            private void ReadAll()
            {
                CLx.ReadFromDevice();
                CLDeltaX.ReadFromDevice();
                rCent.ReadFromDevice();
                Mxd.ReadFromDevice();

                tempM.ReadFromDevice();
                tempM2.ReadFromDevice();

                CLlambda.ReadFromDevice();
                z.ReadFromDevice();
                z2.ReadFromDevice();
                Mxd.ReadFromDevice();

                CLDeltaLambda.ReadFromDevice();

                MtDzM.CLValues.ReadFromDeviceTo(MtDzM.Values);

                Hpd.CLValues.ReadFromDeviceTo(Hpd.Values);
                temp.ReadFromDevice();
                temp2.ReadFromDevice();

                CLxPlus.ReadFromDevice();
                CLlambda.ReadFromDevice();
            }


            #region Primal-dual method
            /// <summary>Solves a quadratic programming problem 1/2 x'Px + q'x subject to Mx less or equal d and Ax = b</summary>
            /// <param name="x0">Start point x0</param>
            /// <param name="lambda0">Start dual point lambda0</param>
            /// <param name="nu0">Start nus (equality constraints)</param>
            /// <param name="P">Positive semidefinite quadratic objective matrix P</param>
            /// <param name="q">Linear objective q</param>
            /// <param name="M">Ineq constraint matrix M</param>
            /// <param name="d">Ineq constraint right hand side d</param>
            /// <param name="A">Constraint matrix A</param>
            /// <param name="b">Constraint right hand side b</param>
            /// <param name="sf">Stop function. The system won't check feasibility if this function is not null. Return true when you want the optimization to stop based on x, lambda and nu</param>
            public float[] SolvePrimalDual(float[] x0, float[] lambda0, float[] nu0, floatLinalg.floatSymPosDefMatrix P,
                float[] q, float[,] M, float[] d, float[,] A, float[] b, StopFunc sf)
            {
                //Number of primal vars
                int n = x0.Length;
                //Number of dual vars
                int m = lambda0.Length;
                //Constraint variables
                int c = nu0 == null ? 0 : nu0.Length;

                if (P!=null && P.getN != n) throw new Exception("Incompatible matrix P dimensions");
                if (M.GetLength(0) != m || M.GetLength(1) != n) throw new Exception("Incompatible matrix M dimensions");
                if (q.Length != n) throw new Exception("Incompatible vector q dimensions");
                if (d.Length != m) throw new Exception("Incompatible vector d dimensions");
                if (nu0 == null && A != null) throw new Exception("Equality constraint matrix was given. Please also give initial values for laplace multipliers nu0");

                CLP = P;
                if (CLP != null && !CLP.IsMatrixInClMemoryUpdated)
                {
                    CLP.CLValues.WriteToDevice(CLP.Values);
                    CLP.IsMatrixInClMemoryUpdated = true;
                }

                if (c > 0)
                {
                    if (b.Length != c) throw new Exception("Incompatible vector b dimensions");
                    if (A.GetLength(0) != c || A.GetLength(1) != n) throw new Exception("Incompatible matrix A dimensions");
                }

                CLM = new floatLinalg.floatMatrix(M);

                CLx = new floatLinalg.floatVector(x0);
                CLxPlus = new floatLinalg.floatVector(x0);
                CLlambda = new floatLinalg.floatVector(lambda0);
                CLlambdaPlus = new floatLinalg.floatVector(lambda0);
                CLDeltaX = new floatLinalg.floatVector(x0);
                CLDeltaLambda = new floatLinalg.floatVector(lambda0);

                CLq = new floatLinalg.floatVector(q);
                CLd = new floatLinalg.floatVector(d);

                //Auxiliary vars
                zerosN = new floatLinalg.floatVector(new float[n]);

                float[] fOnes = new float[m];
                for (int i = 0; i < fOnes.Length; i++) fOnes[i] = 1;
                onesM = new floatLinalg.floatVector(fOnes);

                Hpd = new floatLinalg.floatSymPosDefMatrix(n);
                MtDzM = new floatLinalg.floatSymPosDefMatrix(n);

                temp = new floatLinalg.floatVector(new float[n]);
                temp2 = new floatLinalg.floatVector(new float[n]);
                rhsXpd = new floatLinalg.floatVector(new float[n]);

                Mxd = new floatLinalg.floatVector(new float[m]);
                z2 = new floatLinalg.floatVector(new float[m]);
                z = new floatLinalg.floatVector(new float[m]);
                rCent = new floatLinalg.floatVector(new float[m]);
                tempM = new floatLinalg.floatVector(new float[m]);
                tempM2 = new floatLinalg.floatVector(new float[m]);

                CLPx = new floatLinalg.floatVector(new float[n]);

                //Equality constraint variables
                if (c > 0)
                {
                    CLA = new floatLinalg.floatMatrix(A);
                    CLb = new floatLinalg.floatVector(b);
                    CLnu = new floatLinalg.floatVector(nu0);
                    CLDeltaNu = new floatLinalg.floatVector(nu0);
                    rPri = new floatLinalg.floatVector(new float[c]);
                    Atnu = new floatLinalg.floatVector(new float[n]);
                    tempC = new floatLinalg.floatVector(new float[c]);
                    CLnuPlus = new floatLinalg.floatVector(new float[c]);

                    AinvHAt = new floatLinalg.floatSymPosDefMatrix(c);
                    AinvHrhs = new floatLinalg.floatVector(new float[c]);
                    tempAinvMatrix = new floatLinalg.floatMatrix(new float[n, c]);

                    fOnes = new float[c];
                    for (int i = 0; i < fOnes.Length; i++) fOnes[i] = 1;
                    onesC = new floatLinalg.floatVector(fOnes);
                }
                else
                {
                    CLA = null;
                    CLb = null;
                    CLnu = null;
                    CLnuPlus = null;
                }

                //Checks feasibility
                bool feasible = true;
                float[] xfeas;
                if (sf == null)
                {
                    xfeas = CheckFeasibility(x0, M, d, A, b);
                    feasible = (xfeas != null);

                    if (floatLinalg.UseOpenCLIfAvailable && CLCalc.CLAcceleration == CLCalc.CLAccelerationType.UsingCL) CLx.CLValues.WriteToDevice(xfeas);
                    else for (int i = 0; i < xfeas.Length; i++) CLx.Values[i] = xfeas[i];
                }

                if (SolutionLog.KeepLog)
                {
                    CLx.ReadFromDevice();
                    SolutionLog.PtSequence.Add((float[])CLx.Values.Clone());
                }

                if (feasible)
                {
                    float surrogDualityGap = 100;
                    float t = 0.1f;
                    int MAXITER = 60;
                    int curIter = 0;

                    while (surrogDualityGap > 5e-5f && curIter < MAXITER &&
                        (sf == null || (sf != null && !sf(CLx, CLlambda, CLnu)))
                        )
                    {


                        if (curIter == (MAXITER >> 1))
                        {
                            //DEBUG, taking too long to converge
                            for (int i = 0; i < CLlambda.Values.Length; i++) CLlambda.Values[i] = 0.1f;
                            if (floatLinalg.UseOpenCLIfAvailable && CLCalc.CLAcceleration == CLCalc.CLAccelerationType.UsingCL) CLlambda.CLValues.WriteToDevice(CLlambda.Values);
                            t *= 0.1f;
                        }




                        //PDResidual(CLx, CLlambda, CLnu, t);
                        ComputePrimalDualSearchDir(t);
                        PrimalDualLineSearch(CLx, Mxd, CLlambda, CLnu, CLDeltaX, CLDeltaLambda,
                            CLDeltaNu, CLxPlus, CLlambdaPlus, CLnuPlus, CompRestric, PDResidual, ref t, out surrogDualityGap);

                        curIter++;

                        if (SolutionLog.KeepLog)
                        {
                            SolutionLog.SurrDualityGaps.Add(surrogDualityGap);
                            CLx.ReadFromDevice();
                            SolutionLog.PtSequence.Add((float[])CLx.Values.Clone());
                        }

                    }

                    SolutionLog.Iterations = curIter;
                    //ReadAll();

                    //Copies lambdas and nus
                    CLlambda.ReadFromDevice();
                    for (int i = 0; i < m; i++) lambda0[i] = CLlambda.Values[i];

                    if (CLnu != null)
                    {
                        CLnu.ReadFromDevice();
                        for (int i = 0; i < c; i++) nu0[i] = CLnu.Values[i];
                    }


                    CLx.ReadFromDevice();
                    return CLx.Values;
                }
                else return null; //Not feasible
            }



            /// <summary>Computes primal dual search direction</summary>
            private void ComputePrimalDualSearchDir(float t)
            {
                //Computes Mx - d
                floatLinalg.BLAS.MatrVecProdSumVec(CLM, CLx, 1, CLd, -1, ref Mxd);

                //Computes z[i] = -lambda[i]/Mxd[i]. uses z2 to already compute 1/(m[i]'x - d[i]). 
                floatLinalg.BLAS.ElemWiseInv(Mxd, ref z2);


                //******************
                floatLinalg.BLAS.DiagVecProd(new floatLinalg.floatDiag(CLlambda), z2, -1, ref z);


                //Playing here. What if we tried shifting this a little bit towards the
                //barrier direction?
                //float tMod = 1 + (float)Math.Log(1 + t);
                //floatLinalg.BLAS.DiagVecProd(new floatLinalg.floatDiag(CLlambda), z2, -1/tMod, ref z);
                //******************

                //Computes MtD(z)M
                floatLinalg.BLAS.MatrTranspMatrProd(CLM, new floatLinalg.floatDiag(z), zerosN, ref MtDzM);

                floatLinalg.floatVector vecHpd = new floatLinalg.floatVector(Hpd);
                floatLinalg.floatVector vecMtDzM = new floatLinalg.floatVector(MtDzM);

                if (CLP != null)
                {
                    floatLinalg.floatVector vecP = new floatLinalg.floatVector(CLP);
                    floatLinalg.BLAS.LinearCombination(1, vecP, 1, vecMtDzM, ref vecHpd);
                }
                else
                {
                    floatLinalg.BLAS.CopyVector(vecMtDzM, vecHpd);
                }

                //computes right hand side rhsXpd = -(Px+q+M'(-z2)*1/t+A'nu)
                if (CLP != null) floatLinalg.BLAS.SymPosDefSymMatrVecMultiply(CLP, CLx, ref CLPx); //Px
                floatLinalg.BLAS.MatrTraspVecMult(CLM, new floatLinalg.floatDiag(onesM), z2, ref temp); //M'z2

                floatLinalg.BLAS.LinearCombination(-1, CLq, 1 / t, temp, ref temp2); // -q - 1/t*M'(-z2)
                if (CLP != null) floatLinalg.BLAS.LinearCombination(-1, CLPx, 1, temp2, ref temp); //-Px - q - 1/t*M'(-z2)
                else floatLinalg.BLAS.CopyVector(temp2, temp);

                if (CLA != null)
                {

                    floatLinalg.BLAS.MatrTraspVecMult(CLA, new floatLinalg.floatDiag(onesC), CLnu, ref Atnu);
                    floatLinalg.BLAS.LinearCombination(-1, Atnu, 1, temp, ref rhsXpd); //-Px - q - 1/t*M'(-z2) - A'*nu

                    //primal residual, -rPri
                    floatLinalg.BLAS.MatrVecProdSumVec(CLA, CLx, -1, CLb, 1, ref rPri);


                    //Solves KKT system [Hpd A'; A 0] = [rhsXpd; rPri]. Remember that rPri was computed as -Ax+b
                    
                    //A inv(H) A'
                    floatLinalg.BLAS.ComputeAinvHTranspA(CLA, Hpd, ref AinvHAt, ref tempAinvMatrix, false);
                    
                    Hpd.LinearSolve(rhsXpd, false, ref temp2);
                    floatLinalg.BLAS.MatrVecProd(CLA, temp2, 1, ref AinvHrhs);
                    
                    //Ainv(H)rhs - rpri
                    floatLinalg.BLAS.LinearCombination(1, AinvHrhs, -1, rPri, ref tempC);

                    //Solves for deltaNu
                    AinvHAt.LinearSolve(tempC, false, ref CLDeltaNu);

                    //Procceeds to deltaX = invH * (rhs - AtDeltanu)
                    floatLinalg.BLAS.MatrTraspVecMult(CLA, new floatLinalg.floatDiag(onesC), CLDeltaNu, ref Atnu);
                    floatLinalg.BLAS.LinearCombination(-1, Atnu, 1, rhsXpd, ref temp);

                    Hpd.LinearSolve(temp, false, ref CLDeltaX);
                }
                else
                {
                    //Solve for search direction deltaX
                    Hpd.LinearSolve(temp, false, ref CLDeltaX);

                    //ReadAll();
                }

                //Computes rCent
                floatLinalg.BLAS.DiagVecProd(new floatLinalg.floatDiag(CLlambda), Mxd, 1, ref tempM);
                floatLinalg.BLAS.LinearCombination(-1, tempM, -1.0f / t, onesM, ref rCent);

                //computes deltaLambda
                floatLinalg.BLAS.MatrVecProd(CLM, CLDeltaX, -1, ref tempM); //-M * DeltaX
                floatLinalg.BLAS.DiagVecProd(new floatLinalg.floatDiag(CLlambda), tempM, 1, ref tempM2); //-diag(lambda)*M*DeltaX
                floatLinalg.BLAS.DiagVecProd(new floatLinalg.floatDiag(z2), tempM2, 1, ref tempM); //-diag(1./f)*diag(lambda)*M*DeltaX
                floatLinalg.BLAS.DiagVecProd(new floatLinalg.floatDiag(z2), rCent, 1, ref tempM2); //diag(1./f)*rCent
                floatLinalg.BLAS.LinearCombination(1, tempM, 1, tempM2, ref CLDeltaLambda);
            }

            /// <summary>Computes the norm of the residuals in a given point</summary>
            /// <param name="x">Primal vars</param>
            /// <param name="lambda">Dual vars</param>
            /// <param name="nu">Equality constraint vars</param>
            /// <param name="t">Solution quuality parameter t</param>
            private float PDResidual(floatLinalg.floatVector x, floatLinalg.floatVector lambda, floatLinalg.floatVector nu, float t)
            {
                float resSquared = 0;

                //computes rDual = -(Px+q+M'*lambda+A'nu)
                if (CLP != null) floatLinalg.BLAS.SymPosDefSymMatrVecMultiply(CLP, x, ref CLPx); //P
                floatLinalg.BLAS.MatrTraspVecMult(CLM, new floatLinalg.floatDiag(onesM), lambda, ref temp); //M'*lambda
                floatLinalg.BLAS.LinearCombination(1, CLq, 1, temp, ref temp2); // q + M'*lambda
                if (CLP != null) floatLinalg.BLAS.LinearCombination(1, CLPx, 1, temp2, ref temp); //Px + q + M'*lambda
                else floatLinalg.BLAS.CopyVector(temp2, temp);

                float tempR;

                if (nu != null) //there are equality constraints
                {
                    floatLinalg.BLAS.MatrTraspVecMult(CLA, new floatLinalg.floatDiag(onesC), CLnu, ref Atnu);
                    floatLinalg.BLAS.LinearCombination(1, temp, 1, Atnu, ref rDual); //2Px + q + M'*lambda + A*nu
                    tempR = rDual.NormSquared(ref temp2);
                    resSquared += tempR;
                }
                else
                {
                    tempR = temp.NormSquared(ref temp2);
                    resSquared += tempR;
                }
                if (SolutionLog.KeepLog && SolutionLog.LogResiduals) 
                    SolutionLog.DualResiduals.Add(tempR);



                //Centralization residue -diag(lambda)f(x) - 1/t*ones(m)
                //Computes Mx - d = f(x)
                floatLinalg.BLAS.MatrVecProdSumVec(CLM, CLx, 1, CLd, -1, ref Mxd);
                floatLinalg.BLAS.DiagVecProd(new floatLinalg.floatDiag(lambda), Mxd, -1, ref tempM);

                floatLinalg.BLAS.LinearCombination(1, tempM, -1 / t, onesM, ref rCent);
                tempR = rCent.NormSquared(ref tempM);
                resSquared += tempR;

                if (SolutionLog.KeepLog && SolutionLog.LogResiduals) 
                    SolutionLog.CentResiduals.Add(tempR);

                if (nu != null)
                {
                    //primal residual, rPri
                    floatLinalg.BLAS.MatrVecProdSumVec(CLA, CLx, 1, CLb, -1, ref rPri);
                    resSquared += rPri.NormSquared(ref tempC);
                }


                return (float)Math.Sqrt(resSquared);
            }

            private void CompRestric(floatLinalg.floatVector x, ref floatLinalg.floatVector fx)
            {
                floatLinalg.BLAS.MatrVecProdSumVec(CLM, x, 1, CLd, -1, ref fx);
            }

            /// <summary>Computes values of the restrictions</summary>
            public delegate void ComputeRestrictions(floatLinalg.floatVector x, ref floatLinalg.floatVector fx);
            /// <summary>Computes norm of primal dual residuals vector</summary>
            public delegate float ComputePDResidualNorm(floatLinalg.floatVector x, floatLinalg.floatVector lambda, floatLinalg.floatVector nu, float t);

            private void PrimalDualLineSearch(floatLinalg.floatVector x, floatLinalg.floatVector fx, floatLinalg.floatVector lambda, floatLinalg.floatVector nu,
                floatLinalg.floatVector dx, floatLinalg.floatVector dlambda, floatLinalg.floatVector dnu,
                floatLinalg.floatVector xPlus, floatLinalg.floatVector lambdaPlus, floatLinalg.floatVector nuPlus, ComputeRestrictions F, ComputePDResidualNorm r, ref float t,
                out float surrogDualityGap)
            {

                float BETA = 0.6f;
                float ALPHA = 0.02f;

                //min(1, -lambda./dlambda)
                float s = 1;
                lambda.ReadFromDevice();
                dlambda.ReadFromDevice();

                for (int i = 0; i < lambda.Values.Length; i++)
                {
                    if (dlambda.Values[i] < 0) s = Math.Min(s, -lambda.Values[i] / dlambda.Values[i]);
                }

                s *= 0.99f;

                //fx can't have positive entries
                floatLinalg.BLAS.LinearCombination(1, x, s, dx, ref xPlus);
                floatLinalg.BLAS.LinearCombination(1, lambda, s, dlambda, ref lambdaPlus);
                if (nu != null) floatLinalg.BLAS.LinearCombination(1, nu, s, dnu, ref nuPlus);

                bool fxHasPositiveEntry = true;

                while (fxHasPositiveEntry && s > 0)
                {
                    F(xPlus, ref fx);
                    fxHasPositiveEntry = fx.HasPositiveEntries();
                    if (fx.HasPositiveEntries())
                    {
                        s *= BETA;
                        floatLinalg.BLAS.LinearCombination(1, x, s, dx, ref xPlus);
                        floatLinalg.BLAS.LinearCombination(1, lambda, s, dlambda, ref lambdaPlus);
                        if (nu != null) floatLinalg.BLAS.LinearCombination(1, nu, s, dnu, ref nuPlus);
                    }
                }


                //Save residuals at this point if necessary
                SolutionLog.LogResiduals = true;
                //Function decrease requirement
                float NormResPrev = r(x, lambda, nu, t);
                SolutionLog.LogResiduals = false;

                float NormPlus = r(xPlus, lambdaPlus, nuPlus, t);

                while (NormPlus > (1.0f - ALPHA * s) * NormResPrev)
                {
                    s *= BETA;
                    floatLinalg.BLAS.LinearCombination(1, x, s, dx, ref xPlus);
                    floatLinalg.BLAS.LinearCombination(1, lambda, s, dlambda, ref lambdaPlus);
                    if (nu != null) floatLinalg.BLAS.LinearCombination(1, nu, s, dnu, ref nuPlus);
                    NormPlus = r(xPlus, lambdaPlus, nuPlus, t);
                }

                if (!float.IsNaN(NormPlus))
                {
                    floatLinalg.BLAS.CopyVector(xPlus, x);
                    floatLinalg.BLAS.CopyVector(lambdaPlus, lambda);
                    if (nu != null) floatLinalg.BLAS.CopyVector(nuPlus, nu);

                    F(xPlus, ref fx);
                    surrogDualityGap = -floatLinalg.BLAS.Dot(fx, lambda, ref tempM);
                }
                else
                {
                    //Can't improve anymore
                    surrogDualityGap = 0.0f;
                }

                t = lambda.Values.Length * 10.0f / surrogDualityGap;

                if (SolutionLog.KeepLog)
                {
                    SolutionLog.StepSizes.Add(s);
                }

                //ReadAll();
            }

            #endregion

            #region Barrier method
            /// <summary>Solves a quadratic programming problem 1/2 x'Px + q'x subject to Mx less or equal d and Ax = b</summary>
            /// <param name="x0">Start point x0</param>
            /// <param name="P">Positive semidefinite quadratic objective matrix P</param>
            /// <param name="q">Linear objective q</param>
            /// <param name="M">Ineq constraint matrix M</param>
            /// <param name="d">Ineq constraint right hand side d</param>
            /// <param name="A">Constraint matrix A</param>
            /// <param name="b">Constraint right hand side b</param>
            /// <param name="sf">Stop function. The system won't check feasibility if this function is not null. Return true when you want the optimization to stop based on x, lambda and nu</param>
            private float[] SolveBarrier(float[] x0, floatLinalg.floatSymPosDefMatrix P,
                float[] q, float[,] M, float[] d, float[,] A, float[] b, StopFunc sf)
            {
                //Number of primal vars
                int n = x0.Length;
                //Number of dual vars
                int m = M.GetLength(0);

                //Constraint variables
                int c = A == null ? 0 : A.GetLength(0);

                if (P != null && P.getN != n) throw new Exception("Incompatible matrix P dimensions");
                if (M.GetLength(0) != m || M.GetLength(1) != n) throw new Exception("Incompatible matrix M dimensions");
                if (q.Length != n) throw new Exception("Incompatible vector q dimensions");
                if (d.Length != m) throw new Exception("Incompatible vector d dimensions");

                CLP = P;
                if (CLP != null && !CLP.IsMatrixInClMemoryUpdated)
                {
                    CLP.CLValues.WriteToDevice(CLP.Values);
                    CLP.IsMatrixInClMemoryUpdated = true;
                }

                if (c > 0)
                {
                    if (b.Length != c) throw new Exception("Incompatible vector b dimensions");
                    if (A.GetLength(0) != c || A.GetLength(1) != n) throw new Exception("Incompatible matrix A dimensions");
                }

                CLM = new floatLinalg.floatMatrix(M);

                CLx = new floatLinalg.floatVector(x0);

                return null;
            }
            #endregion
        }

        #endregion

        #region Logistic regression

        /// <summary>Creates a one-vs-all classification system using Logistic Regression</summary>
        public class LogisticRegression
        {
            /// <summary>Kernel to compute Logistic Regression cost function and gradient/Hessian parameters</summary>
            public static CLCalc.Program.Kernel kernelComputeLogistRegParams;
            /// <summary>Computes p-norm of a vector, sum(|xi|^p)</summary>
            public static CLCalc.Program.Kernel kernelpNorm;
            /// <summary>Computes gradients of p-norm</summary>
            public static CLCalc.Program.Kernel kerneldpNorm;

            #region Variables and linear algebra variables
            /// <summary>Total of categories</summary>
            private List<float> Categories = new List<float>();

            //Creates samples matrix and vectors

            /// <summary>Samples matrix</summary>
            public floatLinalg.floatMatrix CLX;
            /// <summary>Classifications vector to use in training</summary>
            public floatLinalg.floatVector y;
            /// <summary>Regularization parameter</summary>
            public floatLinalg.floatVector lambda;
            /// <summary>Classifications of each example</summary>
            public float[] _classifs;

            //Regularization
            floatLinalg.floatVector CLq;
            floatLinalg.floatVector tempX;
            floatLinalg.floatVector tempdX;
            floatLinalg.floatVector tempd2X;


            //Temporary buffers
            floatLinalg.floatVector CLTheta;
            floatLinalg.floatVector CLGrad;
            floatLinalg.floatVector CLtempGrad;
            floatLinalg.floatVector temp;
            floatLinalg.floatVector CLDeltaTheta;
            floatLinalg.floatSymPosDefMatrix Hess;

            //Temporary vectors
            floatLinalg.floatVector XTheta;
            floatLinalg.floatVector z1;
            floatLinalg.floatVector z2;
            floatLinalg.floatVector cost;
            floatLinalg.floatVector ones;


            //To output the classification

            /// <summary>Matrix containing classification coefficients [numCategories, Sample dimension + 1] (because of interceptor)</summary>
            public floatLinalg.floatMatrix CLM;
            floatLinalg.floatVector CLv;

            #endregion

            /// <summary>Constructor. Receives samples and trains classifier. Includes the intercept term automatically. Classifications that are equal to or
            /// less than zero are negative examples. Any non-zero classification will get its own logistic regression classifier</summary>
            /// <param name="Samples">Matrix containing one sample per line and n samples, [n, p]</param>
            /// <param name="Classifications">Vector of classifications, [n]</param>
            public LogisticRegression(float[,] Samples, float[] Classifications)
            {
                float[] regWeights = new float[Samples.GetLength(1)];
                Init(Samples, Classifications, regWeights, 2);
            }

            /// <summary>Constructor. Receives samples and trains classifier. Includes the intercept term automatically. Classifications that are equal to or
            /// less than zero are negative examples. Any non-zero classification will get its own logistic regression classifier</summary>
            /// <param name="Samples">Matrix containing one sample per line and n samples, [n, p]</param>
            /// <param name="Classifications">Vector of classifications, [n]</param>
            /// <param name="RegularizationWeights">Regularization weights, [p]</param>
            /// <param name="regularizationQ">Exponent of regularization term, sum (|beta|^q). Should be greater than 1</param>
            public LogisticRegression(float[,] Samples, float[] Classifications, float[] RegularizationWeights, float regularizationQ)
            {
                Init(Samples, Classifications, RegularizationWeights, regularizationQ);
            }

            /// <summary>Creates a new logistic regression classifier from already trained coefficients. 
            /// Note: GetInternalHitRate will not work! (because there are no internal samples)</summary>
            /// <param name="CLM">Coefficients, including interceptor coefficient</param>
            /// <param name="Categories">Classification categories</param>
            public LogisticRegression(floatLinalg.floatMatrix CLM, List<float> Categories)
            {
                int p = CLM.Cols - 1;
                int nCategs = CLM.Rows;

                if (Categories.Count != nCategs) throw new Exception("Number of categories and classification coefficients dimension not compatible");

                this.CLM = CLM;
                this.Categories = Categories;
            }

            private void Init(float[,] Samples, float[] Classifications, float[] RegularizationWeights, float regularizationQ)
            {
                int n = Samples.GetLength(0);
                int p = Samples.GetLength(1);

                if (Classifications.Length != n) throw new Exception("Incompatible Classifications vector dimension");
                if (RegularizationWeights.Length != p) throw new Exception("Incompatible RegularizationWeights vector dimension");
                if (regularizationQ <= 1) throw new Exception("Regularization term Q should be > 1");

                //Includes intercept term
                float[,] X = new float[n, p + 1];
                for (int i = 0; i < n; i++)
                {
                    X[i, 0] = 1;
                    for (int j = 0; j < p; j++)
                    {
                        X[i, j + 1] = Samples[i, j];
                    }
                }

                //Regularization terms
                float[] regTerms = new float[p + 1];
                regTerms[0] = 0;
                for (int i = 0; i < RegularizationWeights.Length; i++) regTerms[i + 1] = RegularizationWeights[i];

                //Retrieves categories
                for (int i = 0; i < Classifications.Length; i++)
                {
                    if (Classifications[i] > 0 && Categories.IndexOf(Classifications[i]) < 0)
                        Categories.Add(Classifications[i]);
                }
                if (Categories.Count == 0) throw new Exception("At least one category should be given");


                //Creates samples matrix and vectors
                CLX = new floatLinalg.floatMatrix(X);
                y = new floatLinalg.floatVector(Classifications);

                //Don't regularize first term
                float[] newLambs = new float[p + 1];
                newLambs[0] = 0;
                for (int i = 0; i < p; i++) newLambs[i + 1] = RegularizationWeights[i];
                lambda = new floatLinalg.floatVector(newLambs);


                //Initial guess
                float[] t0 = new float[p + 1];
                for (int i = 0; i < t0.Length; i++) t0[i] = 1E-3f;
                //t0[0] = 1; t0[1] = 0.2f; t0[2] = 0.3f;
                CLTheta = new floatLinalg.floatVector(t0);


                CLGrad = new floatLinalg.floatVector(new float[p + 1]);
                CLtempGrad = new floatLinalg.floatVector(new float[p + 1]);

                CLq = new floatLinalg.floatVector(new float[] { regularizationQ });
                tempX = new floatLinalg.floatVector(new float[p + 1]);
                tempdX = new floatLinalg.floatVector(new float[p + 1]);
                tempd2X = new floatLinalg.floatVector(new float[p + 1]);

                temp = new floatLinalg.floatVector(new float[p + 1]);
                CLDeltaTheta = new floatLinalg.floatVector(new float[p + 1]);
                Hess = new floatLinalg.floatSymPosDefMatrix(p + 1);



                XTheta = new floatLinalg.floatVector(new float[n]);
                z1 = new floatLinalg.floatVector(new float[n]);
                z2 = new floatLinalg.floatVector(new float[n]);
                cost = new floatLinalg.floatVector(new float[n]);

                float[] vones = new float[n];
                for (int i = 0; i < n; i++) vones[i] = 1;
                ones = new floatLinalg.floatVector(vones);


                _classifs = Classifications;

                //Trains classifier
                Train();
            }

            /// <summary>Trains this classifier using the samples Matrix and classifications vector</summary>
            public void Train()
            {
                int n = CLX.Rows;
                int p = CLX.Cols - 1;

                //Builds the matrix M that will be used in the classification, M[Categories.Count, p+1]
                float[,] M = new float[Categories.Count, p + 1];

                float[] x0 = new float[p + 1];
                for (int i = 0; i < p + 1; i++) x0[i] = 0.1f;

                //Classification part
                for (int i = 0; i < Categories.Count; i++)
                {
                    //Composes y for this classifier
                    for (int j = 0; j < n; j++)
                    {
                        y.Values[j] = _classifs[j] == Categories[i] ? 1 : 0;
                    }
                    if (floatLinalg.UseOpenCLIfAvailable && CLCalc.CLAcceleration == CLCalc.CLAccelerationType.UsingCL) y.CLValues.WriteToDevice(y.Values);

                    int iters;
                    if (floatLinalg.UseOpenCLIfAvailable && CLCalc.CLAcceleration == CLCalc.CLAccelerationType.UsingCL) CLTheta.CLValues.WriteToDevice(x0);
                    else for (int k = 0; k < CLTheta.Length; k++) CLTheta.Values[k] = x0[k];
                    float[] beta = floatOptimization.UnconstrainedMinimization.Solve(CLTheta, RegularizedLogistReg, out iters, Hess, CLGrad, temp, CLDeltaTheta);

                    //Copies result to classification matrix M
                    for (int j = 0; j < beta.Length; j++) M[i, j] = beta[j];
                }

                //Creates classification matrix
                if (CLM == null || CLM.Rows != M.GetLength(0) || CLM.Cols != M.GetLength(1)) CLM = new floatLinalg.floatMatrix(M);
                else CLM.SetValues(M);
            }


            #region Cost function, gradient, hessian, regularization
            /// <summary>Computes cost function for logistic regression at a given x</summary>
            /// <param name="CLTheta">Current function point</param>
            /// <param name="ComputeGradHess">Compute gradient and hessian?</param>
            /// <param name="Grad">Gradientof F, if requested. Should not be computed if not requested</param>
            /// <param name="Hess">Hessian of F, if requested. Should not be computed if not requested</param>
            public float RegularizedLogistReg(floatLinalg.floatVector CLTheta, bool ComputeGradHess, ref floatLinalg.floatVector Grad, ref floatLinalg.floatSymPosDefMatrix Hess)
            {
                //Computes x*Theta
                floatLinalg.BLAS.MatrVecProd(CLX, CLTheta, 1, ref XTheta);

                floatLinalg.BLAS.CopyVector(CLTheta, tempX);

                //Computes cost and gradient/hessian terms
                if (floatLinalg.UseOpenCLIfAvailable && CLCalc.CLAcceleration == CLCalc.CLAccelerationType.UsingCL)
                {
                    kernelComputeLogistRegParams.Execute(new CLCalc.Program.Variable[] { XTheta.CLValues, y.CLValues, z1.CLValues, z2.CLValues, cost.CLValues }, XTheta.Length);

                    //Regularization
                    kernelpNorm.Execute(new CLCalc.Program.Variable[] { tempX.CLValues, CLq.CLValues, lambda.CLValues }, tempX.Length);
                }
                else
                {
                    for (int i = 0; i < z1.Length; i++)
                    {
                        float eMz = (float)Math.Exp(-XTheta.Values[i]);

                        float hTheta = 1.0f / (1.0f + eMz);
                        z1.Values[i] = hTheta - y.Values[i];
                        z2.Values[i] = eMz * hTheta * hTheta;

                        cost.Values[i] = y.Values[i] == 0 ? -(float)Math.Log(1 - hTheta) : -(float)Math.Log(hTheta);
                    }

                    //Regularization cost
                    for (int i = 0; i < CLTheta.Length; i++)
                    {
                        tempX.Values[i] = (float)Math.Pow(Math.Abs(tempX.Values[i]), CLq.Values[0]) * lambda.Values[i];
                    }
                }

                if (ComputeGradHess)
                {
                    //Regularization
                    if (floatLinalg.UseOpenCLIfAvailable && CLCalc.CLAcceleration == CLCalc.CLAccelerationType.UsingCL)
                    {
                        kerneldpNorm.Execute(new CLCalc.Program.Variable[] { CLTheta.CLValues, CLq.CLValues, lambda.CLValues, tempdX.CLValues, tempd2X.CLValues }, CLTheta.Length);
                    }
                    else
                    {
                        for (int i = 0; i < CLTheta.Values.Length; i++)
                        {
                            float temp = CLTheta.Values[i];

                            tempdX.Values[i] = (float)Math.Pow(Math.Abs(temp), CLq.Values[0] - 1.0f) * lambda.Values[i] * Math.Sign(temp) * CLq.Values[0];
                            tempd2X.Values[i] = (float)Math.Pow(Math.Abs(temp), CLq.Values[0] - 2.0f) * lambda.Values[i] * CLq.Values[0] * (CLq.Values[0] - 1.0f);
                        }
                    }


                    floatLinalg.BLAS.MatrTraspVecMult(CLX, new floatLinalg.floatDiag(ones), z1, ref CLtempGrad);
                    floatLinalg.BLAS.LinearCombination(1, CLtempGrad, 1, tempdX, ref CLGrad);

                    floatLinalg.BLAS.MatrTranspMatrProd(CLX, new floatLinalg.floatDiag(z2), tempd2X, ref Hess);

                }

                return cost.Sum() + tempX.Sum();
            }
            #endregion

            #region Classification output
            /// <summary>Classifies a sample and returns classification values (the bigger, the most certain)
            /// Note: in the classical approach one would need to compute the 1/(1+exp(-Values[i])) to get the logistic rating</summary>
            /// <param name="Sample">Sample to be classified</param>
            /// <param name="Values">Classification values</param>
            public float Classify(float[] Sample, out float[] Values)
            {
                if (CLTheta.Length != Sample.Length + 1) throw new Exception("Incompatible Sample length");
                if (CLv == null || CLv.Length != Categories.Count) CLv = new floatLinalg.floatVector(new float[Categories.Count]);

                Values = CLv.Values;

                CLTheta.Values[0] = 1;
                for (int i = 0; i < Sample.Length; i++) CLTheta.Values[i + 1] = Sample[i];

                if (floatLinalg.UseOpenCLIfAvailable && CLCalc.CLAcceleration == CLCalc.CLAccelerationType.UsingCL) CLTheta.CLValues.WriteToDevice(CLTheta.Values);

                floatLinalg.BLAS.MatrVecProd(CLM, CLTheta, 1, ref CLv);

                CLv.ReadFromDevice();
                float max = CLv.Values[0];
                int indMax = 0;
                for (int i = 1; i < CLv.Length; i++)
                {
                    if (CLv.Values[i] > max)
                    {
                        max = CLv.Values[i];
                        indMax = i;
                    }
                }

                return Categories[indMax];
            }

            /// <summary>Classifies a Samples matrix, one sample per row. Note: the interceptor x[0] = 1 has to be included</summary>
            /// <param name="Samples">Samples matrix, [n x p+1], where p = original x dimension</param>
            /// <param name="Values">Classification values</param>
            /// <param name="maxVals">Maximum classification values. 
            /// Note: in the classical approach one would need to compute the 1/(1+exp(-Values[i])) to get the logistic rating</param>
            public float[] Classify(floatLinalg.floatMatrix Samples, ref floatLinalg.floatMatrix Values, out float[] maxVals)
            {
                int n = Samples.Rows;
                int p = CLM.Cols - 1;

                if (Samples.Cols != p + 1) throw new Exception("Incompatible Samples dimensions");

                floatLinalg.BLAS.MatrTranspMatrProd(CLM, Samples, ref Values);

                if (floatLinalg.UseOpenCLIfAvailable && CLCalc.CLAcceleration == CLCalc.CLAccelerationType.UsingCL) Values.CLValues.ReadFromDeviceTo(Values.Values);

                //Values dimensions: nSamples x nCategories
                maxVals = new float[n];
                int[] indMax = new int[n];
                for (int j = 0; j < n; j++) maxVals[j] = Values.Values[j];

                for (int i = 1; i < Categories.Count; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (maxVals[j] < Values.Values[j + n * i])
                        {
                            maxVals[j] = Values.Values[j + n * i];
                            indMax[j] = i;
                        }
                    }
                }


                float[] categories = new float[n];
                for (int i = 0; i < categories.Length; i++) categories[i] = Categories[indMax[i]];

                return categories;
            }

            /// <summary>Gets hit rate of a given set</summary>
            /// <param name="Samples">Samples to rate. One sample per row</param>
            /// <param name="Labels">Correct labels</param>
            public float GetHitRate(floatLinalg.floatMatrix Samples, float[] Labels)
            {
                floatLinalg.floatMatrix Values = null;
                float[] MaxVals;
                float[] TestLbls = Classify(Samples, ref Values, out MaxVals);

                float total = 0;
                float correct = 0;

                for (int i = 0; i < Samples.Rows; i++)
                {
                    total += 1;
                    if ((Labels[i] == TestLbls[i] && MaxVals[i] > 0) || (Labels[i] == 0 && MaxVals[i] <= 0))
                    {
                        correct += 1;
                    }
                }
                return correct / total;
            }

            /// <summary>Gets hit rate of a given set</summary>
            /// <param name="Samples">Samples to rate. One sample per row</param>
            /// <param name="Labels">Correct labels</param>
            public float GetHitRate(float[,] Samples, float[] Labels)
            {
                int n = Samples.GetLength(0);
                int p = Samples.GetLength(1);

                //Includes intercept term
                float[,] X = new float[n, p + 1];
                for (int i = 0; i < n; i++)
                {
                    X[i, 0] = 1;
                    for (int j = 0; j < p; j++)
                    {
                        X[i, j + 1] = Samples[i, j];
                    }
                }

                return GetHitRate(new floatLinalg.floatMatrix(X), Labels);
            }


            /// <summary>Retrieves hit rate in training set</summary>
            public float GetInternalHitRate()
            {
                return GetHitRate(CLX, _classifs);
            }

            #endregion
        }

        #endregion
    }
    

}
