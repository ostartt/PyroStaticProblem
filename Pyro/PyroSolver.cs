namespace Pyro;

public class PyroSolver
{
    public const double L = 1;
    public const int N = 8;
    private const double H = L / N;
    private const int Size = 2 * N + 1;
    public Matrix DerBlockMatrix;
    public Matrix DerFuncBlockMatrix;
    public Vector LinearFuncVector;
    
    private Matrix BiFormDerBLock  = 1 / H * new Matrix(new [,]
    {
        { 2.33333, -2.66667, 0.33333 },
        { -2.66667, 5.33333, -2.66667 },
        { 0.33333, -2.66667, 2.33333 }
    });

    private Matrix BiFormDerFuncBLock { get; } = 1 / H * new Matrix(new [,]
    {
        { -1.0, 1.33333, -0.33333 },
        { -1.33333, 0.0, 1.33333 },
        { 0.33333, -1.33333, 1.0 }
    });

    private Vector LinearFuncBlock { get; } =
        H / 2 * new Vector(new[] { 0.166667, 0.666667, 0.166667 });

    public void CalculateInitialBlockVector()
    {
        LinearFuncVector = new Vector(Size);
        for (int i = 0; i < Size; i++)
        {
            if (i % 2 == 0 && i != 0 && i != Size - 1)
            {
                LinearFuncVector[i] = LinearFuncBlock[2] + LinearFuncBlock[0];
            }
            else
            {
                LinearFuncVector[i] = LinearFuncBlock[1];
            }
        }

        LinearFuncVector[0] = LinearFuncBlock[0];
        LinearFuncVector[Size - 1] = LinearFuncBlock[2];
    }

    public void CalculateInitialBlockMatrices()
    {
        DerBlockMatrix = new Matrix(Size, Size);
        DerFuncBlockMatrix = new Matrix(Size, Size);

        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                if (i == j + 1 && j % 2 == 0)
                {
                    DerBlockMatrix[i, j] = BiFormDerBLock[1, 0];
                    DerFuncBlockMatrix[i, j] = BiFormDerFuncBLock[1, 0];
                }
                else if (i == j + 1 && j % 2 != 0)
                {
                    DerBlockMatrix[i, j] = BiFormDerBLock[2, 1];
                    DerFuncBlockMatrix[i, j] = BiFormDerFuncBLock[2, 1];
                }
                else if (i == j - 1 && j % 2 == 0)
                {
                    DerBlockMatrix[i, j] = BiFormDerBLock[1, 2];
                    DerFuncBlockMatrix[i, j] = BiFormDerFuncBLock[1, 2];
                }
                else if (i == j - 1 && j % 2 != 0)
                {
                    DerBlockMatrix[i, j] = BiFormDerBLock[0, 1];
                    DerFuncBlockMatrix[i, j] = BiFormDerFuncBLock[0, 1];
                }
                else if (i == j && j % 2 == 0)
                {
                    DerBlockMatrix[i, j] = BiFormDerBLock[0, 0];
                    DerFuncBlockMatrix[i, j] = BiFormDerFuncBLock[0, 0];
                }
                else if (i == j && j % 2 != 0)
                {
                    DerBlockMatrix[i, j] = BiFormDerBLock[1, 1];
                    DerFuncBlockMatrix[i, j] = BiFormDerFuncBLock[1, 1];
                }
                else if (i == j + 2 && j % 2 == 0)
                {
                    DerBlockMatrix[i, j] = BiFormDerBLock[2, 0];
                    DerFuncBlockMatrix[i, j] = BiFormDerFuncBLock[2, 0];
                }
                else if (i == j - 2 && j % 2 == 0)
                {
                    DerBlockMatrix[i, j] = BiFormDerBLock[0, 2];
                    DerFuncBlockMatrix[i, j] = BiFormDerFuncBLock[0, 2];
                }
            }
        }
    }
}