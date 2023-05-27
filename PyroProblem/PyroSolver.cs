using System;
using System.Collections.Generic;
using static PyroProblem.Matrix;

namespace PyroProblem
{
    public static class PyroSolver
    {
        public static Matrix DerBlock { get; } = new Matrix(new[,]
        {
            { 7 / 3.0, -8 / 3.0, 1 / 3.0 },
            { -8 / 3.0, 16 / 3.0, -8 / 3.0 },
            { 1 / 3.0, -8 / 3.0, 7 / 3.0 }
        });

        public static Matrix DerFuncBlock { get; } = new Matrix(new[,]
        {
            { -1 / 2.0, 2 / 3.0, -1 / 6.0 },
            { -2 / 3.0, 0.0, 2 / 3.0 },
            { 1 / 6.0, -2 / 3.0, 1 / 2.0 }
        });
        
        public static Matrix FuncFuncBlock { get; } = new Matrix(new[,]
        {
            { 2 / 15.0, 1 / 15.0, -1 / 30.0 },
            { 1 / 15.0, 8 / 15.0, 1 / 15.0 },
            { -1 / 30.0, 1 / 15.0,  2 / 15.0 }
        });

        public static Vector LinearBlock { get; } = new Vector(new[] { 1 / 6.0, 2 / 3.0, 1 / 6.0 });
        private static void SetRow(Matrix phiMatrix, int i, double value1, double value2, double value3, double value4,
        double value5)
    {
        phiMatrix[i, 0] = value1;
        phiMatrix[i, 1] = value2;
        phiMatrix[i, 2] = value3;
        phiMatrix[i, 3] = value4;
        phiMatrix[i, 4] = value5;
    }

    public static Matrix GetInitialBiForm(Matrix toFillWith, int n)
    {
        Matrix phiMatrix = new Matrix(2 * n + 1, BlockOfFive);
        
        for (int i = 1; i < 2 * n; i++)
        {
            if (i % 2 != 0)
                SetRow(phiMatrix, i, 0, 0, toFillWith[1, 0], toFillWith[1, 1], toFillWith[1, 2]);
            if (i == 1 || i == 2 * n - 1)
                SetRow(phiMatrix, i, 0, toFillWith[1, 0], toFillWith[1, 1], toFillWith[1, 2], 0);
            else if (i % 2 is 0)
                SetRow(phiMatrix, i, toFillWith[2, 0], toFillWith[2, 1], toFillWith[2, 2] + toFillWith[0, 0],
                    toFillWith[0, 1], toFillWith[0, 2]);
        }

        SetRow(phiMatrix, 0, 0, 0, toFillWith[0, 0],
            toFillWith[0, 1], toFillWith[0, 2]);
        SetRow(phiMatrix, 2 * n, toFillWith[2, 0], toFillWith[2, 1], toFillWith[2, 2], 0, 0);
        
        return phiMatrix;
    }

    public static Vector GetInitialLinearFunc(Vector toFillWith, int n)
    {
        Vector phiVector = new Vector(2 * n + 1);
        for (int i = 1; i < 2 * n; i++)
        {
            if (i % 2 == 0) phiVector[i] = toFillWith[2] + toFillWith[0];
            else phiVector[i] = toFillWith[1];
        }

        phiVector[0] = toFillWith[0];
        phiVector[2 * n] = toFillWith[2];

        return phiVector;
    }

    public static Vector[] GetFinalVector(Vector lFunc, Vector rFunc, Vector muFunc, int n)
    {
        Vector[] finalForm = new Vector[2 * n + 1];

        for (int i = 0; i < 2 * n + 1; i++)
        {
            finalForm[i] = new Vector(BlockOfThree);
            finalForm[i][0] = lFunc[i];
            finalForm[i][1] = rFunc[i];
            finalForm[i][2] = muFunc[i];
        }

        return finalForm;
    }

    public static Matrix[,] GetFinalMatrix(Matrix cBiForm, Matrix ebiForm, Matrix yBiForm, Matrix gBiForm, 
        Matrix piBiForm, Matrix kBiForm, int n)
    {
        Matrix[,] finalForm = new Matrix[2 * n + 1, 5];
        for (int i = 0; i < 2 * n + 1; i++)
        {
            for (int j = 0; j < BlockOfFive; j++)
            {
                finalForm[i, j] = new Matrix(BlockOfThree, BlockOfThree);
                finalForm[i, j][0, 0] = cBiForm[i, j];
                finalForm[i, j][0, 1] = finalForm[i, j][1, 0] = ebiForm[i, j];
                finalForm[i, j][0, 2] = yBiForm[i, j];
                finalForm[i, j][1, 1] = gBiForm[i, j];
                finalForm[i, j][1, 2] = piBiForm[i, j];
                finalForm[i, j][2, 0] = finalForm[i, j][2, 1] = 0;
                finalForm[i, j][2, 2] = kBiForm[i, j];
            }
        }

        return finalForm;
    }

    public static void SetBoundaryValues(Matrix[,] finalMatrix, double toSet)
    {
        finalMatrix[0, 2][0, 0] = toSet;
        finalMatrix[0, 2][1, 1] = toSet;
        finalMatrix[0, 2][2, 2] = toSet;
    }
        
        public static Vector[] FiveDiagonalLowerUpperMethod(Matrix[,] a, Vector[] l)
        {
            int n = l.Length;

            Matrix[] alpha = new Matrix[n];
            Matrix[] betta = new Matrix[n];
            Matrix[] gamma = new Matrix[n];
            Matrix[] sigma = new Matrix[n];
            Matrix[] tetha = new Matrix[n];


            for (int i = 0; i < n; ++i)
            {
                if (i >= 2)
                    alpha[i] = a[i, 0];

                if (i >= 1)
                {
                    betta[i] = a[i, 1];
                    if (i >= 2)
                        betta[i] -= a[i, 0] * sigma[i - 2];
                }

                gamma[i] = a[i, 2];

                if (i >= 1)
                    gamma[i] -= betta[i] * sigma[i - 1];
                if (i >= 2)
                    gamma[i] -= a[i, 0] * tetha[i - 2];


                if (i <= n - 2)
                {
                    Matrix mult = a[i, 3];
                    if (i >= 1)
                        mult -= betta[i] * tetha[i - 1];
                    sigma[i] = InverseMatrix(gamma[i]) * mult;

                }

                if (i <= n - 3)
                {
                    tetha[i] = InverseMatrix(gamma[i]) * a[i, 4];
                }

            }

            Vector[] v = new Vector[n];

            for (int i = 0; i < n; ++i)
            {
                Vector mult = l[i];
                if (i >= 1)
                    mult -= betta[i] * v[i - 1];

                if (i >= 2)
                    mult -= a[i, 0] * v[i - 2];

                v[i] = InverseMatrix(gamma[i]) * mult;
            }

            Vector[] solution = new Vector[n];

            for (int i = n - 1; i >= 0; --i)
            {
                solution[i] = v[i];

                if (i < n - 1)
                    solution[i] -= sigma[i] * solution[i + 1];

                if (i < n - 2)
                    solution[i] -= tetha[i] * solution[i + 2];
            }

            return solution;
        }
        
        
        public static double GetNorm(List<double> vector, int n, double h)
        {
            double norm = 0;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < BlockOfThree; j++)
                {
                    for (int k = 0; k < BlockOfThree; k++)
                    {
                        norm += vector[i * (BlockOfThree - 1) + j] * vector[i * (BlockOfThree - 1) + k] * h * FuncFuncBlock[j,k];
                        norm += vector[i * (BlockOfThree - 1) + j] * vector[i * (BlockOfThree - 1) + k] * (1.0 / h) * DerBlock[j, k];
                    }

                }
            }

            return Math.Sqrt(norm);
        }
    }
}