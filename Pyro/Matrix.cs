using System.Text;

namespace Pyro;

using System;
using System.IO;

public class Matrix : ICloneable
{
    private readonly double[,] _matrix;

    private const int MinVal = 1;
    private const int MaxVal = 10;

    public int Rows { get; }

    public int Columns { get; }

    public Matrix(double[,] matrix)
    {
        _matrix = matrix;
        Rows = matrix.GetLength(0);
        Columns = matrix.GetLength(1);
    }

    public Matrix(int rows, int columns, bool rand = false, int min = MinVal, int max = MaxVal)
    {
        if (rows < 1 || columns < 1)
        {
            throw new ArgumentException("Wrong dimension of matrix");
        }

        Rows = rows;
        Columns = columns;
        _matrix = new double[rows, columns];
        Fill(rand, min, max);
    }

    public Matrix(String path)
    {
        Matrix matrix = ReadMatrix(path);
        Rows = matrix.Rows;
        Columns = matrix.Columns;
        _matrix = new double[Rows, Columns];

        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                _matrix[i, j] = matrix[i, j];
            }
        }
    }

    public Matrix(Matrix matrix)
    {
        Rows = matrix.Rows;
        Columns = matrix.Columns;
        _matrix = new double[Rows, Columns];

        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                _matrix[i, j] = matrix[i, j];
            }
        }
    }

    private void Fill(bool rand, int min, int max)
    {
        if (rand)
        {
            Random random = new Random();
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    _matrix[i, j] = random.Next(min, max);
                }
            }
        }
        else
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    _matrix[i, j] = 0;
                }
            }
        }
    }

    public double this[int i, int j]
    {
        get
        {
            if (i < 0 || j < 0 || i > Rows - 1 || j > Columns - 1)
            {
                throw new ArgumentException("Invalid indexes");
            }

            return _matrix[i, j];
        }
        set
        {
            if (i < 0 || j < 0 || i > Rows - 1 || j > Columns - 1)
            {
                throw new ArgumentException("Invalid indexes");
            }

            _matrix[i, j] = value;
        }
    }

    public override string ToString()
    {
        StringBuilder matrixToString = new StringBuilder();

        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                matrixToString.Append(_matrix[i, j] + "\t");
            }

            matrixToString.Append("\n");
        }

        return matrixToString + "\n";
    }

    public object Clone()
    {
        return new Matrix(this);
    }

    public static Matrix operator *(Matrix matrix, double number)
    {
        Matrix result = new Matrix(matrix.Rows, matrix.Columns);

        for (int i = 0; i < matrix.Rows; i++)
        {
            for (int j = 0; j < matrix.Columns; j++)
            {
                result[i, j] = matrix[i, j] * number;
            }
        }

        return result;
    }

    public static Matrix operator *(double number, Matrix matrix)
    {
        Matrix result = new Matrix(matrix.Rows, matrix.Columns);

        for (int i = 0; i < matrix.Rows; i++)
        {
            for (int j = 0; j < matrix.Columns; j++)
            {
                result[i, j] = matrix[i, j] * number;
            }
        }

        return result;
    }


    public static Matrix operator +(Matrix matrix1, Matrix matrix2)
    {
        if (matrix1.Rows != matrix2.Rows || matrix1.Columns != matrix2.Columns)
        {
            throw new ArgumentException("Column and row numbers should be equal in both matrices");
        }

        Matrix result = new Matrix(matrix1.Rows, matrix1.Columns);

        for (int i = 0; i < matrix1.Rows; i++)
        {
            for (int j = 0; j < matrix1.Columns; j++)
            {
                result[i, j] = matrix1[i, j] + matrix2[i, j];
            }
        }

        return result;
    }

    public static Matrix operator -(Matrix matrix1, Matrix matrix2)
    {
        if (matrix1.Rows != matrix2.Rows || matrix1.Columns != matrix2.Columns)
        {
            throw new ArgumentException("Column and row numbers should be equal in both matrices");
        }

        Matrix result = new Matrix(matrix1.Rows, matrix1.Columns);

        for (int i = 0; i < matrix1.Rows; i++)
        {
            for (int j = 0; j < matrix1.Columns; j++)
            {
                result[i, j] = matrix1[i, j] - matrix2[i, j];
            }
        }

        return result;
    }

    public static Matrix operator *(Matrix matrix1, Matrix matrix2)
    {
        if (matrix1.Columns != matrix2.Rows)
        {
            throw new ArgumentException(
                "Column number in the first matrix should be equal to row number in the second matrix");
        }

        Matrix result = new Matrix(matrix1.Rows, matrix2.Columns);
        for (int i = 0; i < result.Rows; i++)
        {
            for (int j = 0; j < result.Columns; j++)
            {
                double sum = 0;

                for (int k = 0; k < matrix1.Columns; k++)
                {
                    sum += matrix1[i, k] * matrix2[k, j];
                }

                result[i, j] = sum;
            }
        }

        return result;
    }

    public override bool Equals(object obj)
    {
        if (obj is Matrix)
        {
            Matrix otherMatrix = (Matrix)obj;
            if (Columns == otherMatrix.Columns && Rows == otherMatrix.Rows)
            {
                for (int i = 0; i < otherMatrix.Rows; i++)
                {
                    for (int j = 0; j < otherMatrix.Columns; j++)
                        if (Math.Abs(otherMatrix[i, j] - this[i, j]) > 1e-5)
                        {
                            return false;
                        }
                }

                return true;
            }

            return false;
        }

        return false;
    }

    public static bool operator ==(Matrix matrix1, Matrix matrix2)
    {
        return matrix1.Equals(matrix2);
    }

    public static bool operator !=(Matrix matrix1, Matrix matrix2)
    {
        return !matrix1.Equals(matrix2);
    }

    public override int GetHashCode()
    {
        return ToString().GetHashCode();
    }

    public static Matrix ReadMatrix(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("File does not exist");
        }

        string[] lines = File.ReadAllLines(path);
        int rows = lines.Length;
        int columns = lines[0].Split(' ').Length;

        Matrix matrix = new Matrix(rows, columns);
        for (int i = 0; i < rows; i++)
        {
            string[] values = lines[i].Split(' ');
            for (int j = 0; j < columns; j++)
            {
                matrix[i, j] = int.Parse(values[j]);
            }
        }

        return matrix;
    }

    public static void WriteMatrix(Matrix matrix, string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("File does not exist");
        }

        File.WriteAllText(path, string.Empty);
        File.WriteAllText(path, matrix.ToString());
    }

    public static double GetEuclideanNorm(Matrix matrix)
    {
        double sumOfSquares = 0;
        for (int i = 0; i < matrix.Rows; i++)
        {
            for (int j = 0; j < matrix.Columns; j++)
            {
                sumOfSquares += Math.Pow(matrix[i, j], 2);
            }
        }

        return Math.Sqrt(sumOfSquares);
    }

    public static double GetNorm(Matrix matrix)
    {
        double norm = 0;
        double maxNorm = 0;
        for (int i = 0; i < matrix.Rows; i++)
        {
            for (int j = 0; j < matrix.Columns; j++)
            {
                norm += Math.Abs(matrix[i, j]);
            }

            maxNorm = Math.Max(norm, maxNorm);
        }

        return maxNorm;
    }

    public static Matrix[] CloneMatrixArray(Matrix[] toClone)
    {
        return toClone.Select(matrix => (Matrix)matrix.Clone()).ToArray();
    }

    public static Matrix InverseMatrix(Matrix matrix)
    {
        if (matrix.Columns != PyroSolver.BlockOfThree || matrix.Rows != PyroSolver.BlockOfThree)
        {
            throw new ArgumentException("Matrix is not a block of 3");
        }

        double determinant = matrix[0, 0] * (matrix[1, 1] * matrix[2, 2] - matrix[2, 1] * matrix[1, 2])
                             - matrix[0, 1] * (matrix[1, 0] * matrix[2, 2] - matrix[2, 0] * matrix[1, 2])
                             + matrix[0, 2] * (matrix[1, 0] * matrix[2, 1] - matrix[2, 0] * matrix[1, 1]);

        if (determinant == 0)
        {
            throw new ArgumentException("Matrix has no inverse.");
        }

        Matrix inverse = new Matrix(3, 3);

        inverse[0, 0] = (matrix[1, 1] * matrix[2, 2] - matrix[2, 1] * matrix[1, 2]) / determinant;
        inverse[0, 1] = (matrix[0, 2] * matrix[2, 1] - matrix[0, 1] * matrix[2, 2]) / determinant;
        inverse[0, 2] = (matrix[0, 1] * matrix[1, 2] - matrix[0, 2] * matrix[1, 1]) / determinant;

        inverse[1, 0] = (matrix[1, 2] * matrix[2, 0] - matrix[1, 0] * matrix[2, 2]) / determinant;
        inverse[1, 1] = (matrix[0, 0] * matrix[2, 2] - matrix[0, 2] * matrix[2, 0]) / determinant;
        inverse[1, 2] = (matrix[1, 0] * matrix[0, 2] - matrix[0, 0] * matrix[1, 2]) / determinant;

        inverse[2, 0] = (matrix[1, 0] * matrix[2, 1] - matrix[2, 0] * matrix[1, 1]) / determinant;
        inverse[2, 1] = (matrix[2, 0] * matrix[0, 1] - matrix[0, 0] * matrix[2, 1]) / determinant;
        inverse[2, 2] = (matrix[0, 0] * matrix[1, 1] - matrix[1, 0] * matrix[0, 1]) / determinant;

        return inverse;
    }
}