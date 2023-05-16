namespace Pyro;

public class PyroSolver
{
    public const int BlockOfThree = 3;
    public const int BlockOfFive = 5;
    public const double L = 0.01;
    public const int N = 10;
    private const double H = L / N;
    public Matrix PhiDerMatrix { get; set; }
    public Matrix PhiDerFuncMatrix { get; set; }
    public Vector LinearFuncVector { get; set; }

    public Matrix CBiForm { get; set; }
    public Matrix EBiForm { get; set; }
    public Matrix YBiForm { get; set; }
    public Matrix GBiForm { get; set; }
    public Matrix PiBiForm { get; set; }
    public Matrix KBiForm { get; set; }
    public Vector RFunc { get; set; }
    public Vector LFunc { get; set; }
    public Vector MuFunc { get; set; }

    public static Matrix DerBlock { get; } = 1 / H * new Matrix(new[,]
    {
        { 7 / 3.0, -8 / 3.0, 1 / 3.0 },
        { -8 / 3.0, 16 / 3.0, -8 / 3.0 },
        { 1 / 3.0, -8 / 3.0, 7 / 3.0 }
    });

    public static Matrix DerFuncBlock { get; } = new(new[,]
    {
        { -1 / 2.0, 2 / 3.0, -1 / 6.0 },
        { -2 / 3.0, 0.0, 2 / 3.0 },
        { 1 / 6.0, -2 / 3.0, 1 / 2.0 }
    });

    public static Vector LinearBlock { get; } =
        H * new Vector(new[] { 1 / 6.0, 2 / 3.0, 1 / 6.0 });

    private void SetRow(Matrix phiMatrix, int i, double value1, double value2, double value3, double value4,
        double value5)
    {
        phiMatrix[i, 0] = value1;
        phiMatrix[i, 1] = value2;
        phiMatrix[i, 2] = value3;
        phiMatrix[i, 3] = value4;
        phiMatrix[i, 4] = value5;
    }

    public void CalculateInitialBiForm(Matrix phiMatrix, Matrix toFill)
    {
        for (int i = 1; i < 2 * N; i++)
        {
            if (i % 2 is not 0)
                SetRow(phiMatrix, i, 0, 0, toFill[1, 0], toFill[1, 1], toFill[1, 2]);
            if (i is 1 or 2 * N - 1)
                SetRow(phiMatrix, i, 0, toFill[1, 0], toFill[1, 1], toFill[1, 2], 0);
            else if (i % 2 is 0)
                SetRow(phiMatrix, i, toFill[2, 0], toFill[2, 1], toFill[2, 2] + toFill[0, 0],
                    toFill[0, 1], toFill[0, 2]);
        }

        SetRow(phiMatrix, 0, 0, 0, toFill[0, 0],
            toFill[0, 1], toFill[0, 2]);
        SetRow(phiMatrix, 2 * N, toFill[2, 0], toFill[2, 1], toFill[2, 2], 0, 0);
    }

    public void CalculateInitialLinearFunc(Vector phiVector)
    {
        for (int i = 1; i < 2 * N; i++)
        {
            if (i % 2 == 0) phiVector[i] = LinearBlock[2] + LinearBlock[0];
            else phiVector[i] = LinearBlock[1];
        }

        phiVector[0] = LinearBlock[0];
        phiVector[2 * N] = LinearBlock[2];
    }

    public Vector[] FormFinalVector()
    {
        Vector[] finalForm = new Vector[2 * N + 1];

        for (int i = 0; i < 2 * N + 1; i++)
        {
            finalForm[i] = new Vector(BlockOfThree);
            finalForm[i][0] = LFunc[i];
            finalForm[i][1] = RFunc[i];
            finalForm[i][2] = MuFunc[i];
        }

        return finalForm;
    }

    public Matrix[,] FormFinalMatrix()
    {
        Matrix[,] finalForm = new Matrix[2 * N + 1, 5];
        for (int i = 0; i < 2 * N + 1; i++)
        {
            for (int j = 0; j < BlockOfFive; j++)
            {
                finalForm[i, j] = new Matrix(BlockOfThree, BlockOfThree);
                finalForm[i, j][0, 0] = CBiForm[i, j];
                finalForm[i, j][0, 1] = finalForm[i, j][1, 0] = EBiForm[i, j];
                finalForm[i, j][0, 2] = YBiForm[i, j];
                finalForm[i, j][1, 1] = GBiForm[i, j];
                finalForm[i, j][1, 2] = PiBiForm[i, j];
                finalForm[i, j][2, 0] = finalForm[i, j][2, 1] = 0;
                finalForm[i, j][2, 2] = KBiForm[i, j];
            }
        }

        return finalForm;
    }

    public static Vector[] FiveDiagonalLowerUpperMethod(Matrix[,] matrix, Vector[] vector)
    {
        int n = vector.Length;
        Matrix[] alpha = new Matrix[n];
        Matrix[] betta = new Matrix[n];
        Matrix[] gamma = new Matrix[n];
        Matrix[] sigma = new Matrix[n];
        Matrix[] etha = new Matrix[n];

        for (int i = 0; i < n; ++i)
        {
            if (i >= 2)
            {
                alpha[i] = matrix[i, 0];
            }

            if (i >= 1)
            {
                betta[i] = matrix[i, 1];
                if (i >= 2)
                {
                    betta[i] -= matrix[i, 0] * sigma[i - 2];
                }
            }

            gamma[i] = matrix[i, 2];

            if (i >= 1)
            {
                gamma[i] -= betta[i] * sigma[i - 1];
            }

            if (i >= 2)
            {
                gamma[i] -= matrix[i, 0] * etha[i - 2];
            }

            if (i <= n - 2)
            {
                Matrix mult = matrix[i, 3];
                if (i >= 1)
                    mult -= betta[i] * etha[i - 1];
                sigma[i] = Matrix.InverseMatrix(gamma[i]) * mult;
            }


            if (i <= n - 3)
            {
                etha[i] = Matrix.InverseMatrix(gamma[i]) * matrix[i, 4];
            }
        }

        Vector[] v = new Vector[n];

        for (int i = 0; i < n; ++i)
        {
            Vector mult = vector[i];

            if (i >= 1)
            {
                mult -= betta[i] * v[i - 1];
            }

            if (i >= 2)
            {
                mult -= matrix[i, 0] * v[i - 2];
            }

            v[i] = Matrix.InverseMatrix(gamma[i]) * mult;
        }

        Vector[] w = new Vector[n];


        for (int i = n - 1; i >= 0; --i)
        {
            w[i] = v[i];

            if (i < n - 1)
            {
                w[i] -= sigma[i] * w[i + 1];
            }

            if (i < n - 2)
            {
                w[i] -= etha[i] * w[i + 2];
            }
        }

        return w;
    }
}