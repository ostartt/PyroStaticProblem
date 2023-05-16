using System.Text;

namespace Pyro;

using System;
using System.IO;

public class Vector : ICloneable
{
    private readonly double[] _vector;

    private const int MinVal = 1;
    private const int MaxVal = 10;

    public int Size { get; }

    public Vector(double[] vector)
    {
        _vector = vector;
        Size = vector.Length;
    }

    public Vector(double value1, double value2, double value3)
    {
        _vector = new[] { value1, value2, value3 };
        Size = _vector.Length;
    }

    public Vector(int size, bool rand = false, int min = MinVal, int max = MaxVal)
    {
        if (size < 1)
        {
            throw new ArgumentException("Vector size should be greater or equal to 1");
        }

        Size = size;
        _vector = new double[Size];
        Fill(rand, min, max);
    }

    public Vector(string path)
    {
        Vector vector = ReadVector(path);
        Size = vector.Size;
        _vector = new double[vector.Size];

        for (int i = 0; i < Size; i++)
        {
            _vector[i] = vector[i];
        }
    }

    public Vector(Vector vector)
    {
        Size = vector.Size;
        _vector = new double[Size];

        for (int i = 0; i < Size; i++)
        {
            _vector[i] = vector[i];
        }
    }


    private void Fill(bool rand, int min, int max)
    {
        if (rand)
        {
            Random random = new Random();
            for (int i = 0; i < Size; i++)
            {
                _vector[i] = random.Next(min, max);
            }
        }
        else
        {
            for (int i = 0; i < Size; i++)
            {
                _vector[i] = 0;
            }
        }
    }

    public double this[int i]
    {
        get
        {
            if (i < 0 || i > Size - 1)
            {
                throw new ArgumentException("Invalid index");
            }

            return _vector[i];
        }
        set
        {
            if (i < 0 || i > Size - 1)
            {
                throw new ArgumentException("Invalid index");
            }

            _vector[i] = value;
        }
    }

    public override string ToString()
    {
        StringBuilder vectorToString = new StringBuilder();

        for (int i = 0; i < Size; i++)
        {
            vectorToString.Append(_vector[i] + "   ");
        }

        return vectorToString.ToString() + '\n';
    }

    public object Clone()
    {
        return new Vector(this);
    }

    public static Vector operator *(Vector vector, int number)
    {
        Vector result = new Vector(vector.Size);

        for (int i = 0; i < vector.Size; i++)
        {
            result[i] = vector[i] * number;
        }

        return result;
    }

    public static Vector operator *(int number, Vector vector)
    {
        Vector result = new Vector(vector.Size);

        for (int i = 0; i < vector.Size; i++)
        {
            result[i] = vector[i] * number;
        }

        return result;
    }


    public static Vector operator +(Vector vector1, Vector vector2)
    {
        if (vector1.Size != vector2.Size)
        {
            throw new ArgumentException("Elements number of each vector should be equal");
        }

        Vector result = new Vector(vector1.Size);

        for (int i = 0; i < vector1.Size; i++)
        {
            result[i] = vector1[i] + vector2[i];
        }

        return result;
    }

    public static Vector operator -(Vector vector1, Vector vector2)
    {
        if (vector1.Size != vector2.Size)
        {
            throw new ArgumentException("Elements number of each vector should be equal");
        }

        Vector result = new Vector(vector1.Size);

        for (int i = 0; i < vector1.Size; i++)
        {
            result[i] = vector1[i] - vector2[i];
        }

        return result;
    }

    public override bool Equals(object obj)
    {
        if (obj is Vector)
        {
            Vector otherVector = (Vector)obj;
            if (Size == otherVector.Size)
            {
                for (int i = 0; i < otherVector.Size; i++)
                {
                    if (Math.Abs(otherVector[i] - this[i]) > 1e-5)
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

    public static bool operator ==(Vector vector1, Vector vector2)
    {
        return vector1.Equals(vector2);
    }

    public static bool operator !=(Vector vector1, Vector vector2)
    {
        return !vector1.Equals(vector2);
    }

    public override int GetHashCode()
    {
        return ToString().GetHashCode();
    }

    public static Vector operator *(Matrix matrix, Vector vector)
    {
        if (matrix.Columns != vector.Size)
        {
            throw new ArgumentException("Rows number in the matrix should be equal to the vector size");
        }

        Vector result = new Vector(matrix.Rows);
        for (int i = 0; i < matrix.Rows; i++)
        {
            for (int j = 0; j < matrix.Columns; j++)
            {
                result[i] += matrix[i, j] * vector[j];
            }
        }

        return result;
    }

    public static Vector operator *(Vector vector, Matrix matrix)
    {
        if (matrix.Rows != vector.Size)
        {
            throw new ArgumentException("Column number in the matrix should be equal to the vector size");
        }

        Vector result = new Vector(matrix.Columns);
        for (int i = 0; i < matrix.Columns; i++)
        {
            for (int j = 0; j < matrix.Rows; j++)
            {
                result[i] += matrix[j, i] * vector[j];
            }
        }

        return result;
    }

    public static Vector operator *(Vector vector1, Vector vector2)
    {
        if (vector1.Size != vector2.Size)
        {
            throw new ArgumentException("Vector sizes must match");
        }

        Vector result = new Vector(vector1.Size);
        for (int i = 0; i < vector1.Size; i++)
        {
            result[i] = vector1[i] * vector2[i];
        }

        return result;
    }

    public static Vector operator *(Vector vector, double number)
    {
        Vector result = new Vector(vector.Size);
        for (int i = 0; i < vector.Size; i++)
        {
            result[i] = vector[i] * number;
        }

        return result;
    }

    public static Vector operator *(double number, Vector vector)
    {
        Vector result = new Vector(vector.Size);
        for (int i = 0; i < vector.Size; i++)
        {
            result[i] = vector[i] * number;
        }

        return result;
    }

    public static Vector ReadVector(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("File does not exist");
        }

        string[] lines = File.ReadAllLines(path);
        int[] numArray = Array.ConvertAll(lines[0].Split(' '), int.Parse);
        Vector vector = new Vector(numArray.Length);

        for (int i = 0; i < numArray.Length; i++)
        {
            vector[i] = numArray[i];
        }

        return vector;
    }

    public static void WriteVector(Vector vector, string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("File does not exist");
        }

        File.WriteAllText(path, string.Empty);
        File.WriteAllText(path, vector.ToString());
    }

    public static Vector[] CloneVectorArray(Vector[] toClone)
    {
        return toClone.Select(matrix => (Vector)matrix.Clone()).ToArray();
    }
}