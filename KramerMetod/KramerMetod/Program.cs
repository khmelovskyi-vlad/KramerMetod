using System;

namespace KramerMetod
{
    class Program
    {
        static void Main(string[] args)
        {
            int[,] mat =  { { 1,2,30},
                            {4,5,6 },
                             {7,8,9 } };
            var test = Determinant(mat);
            Console.WriteLine(test);
            Console.ReadKey();
        }
        static double Determinant(int[,] matrix)
        {
            if (matrix.Length == 1)
            {
                return matrix[0, 0];
            }
            var determinant = 0d;
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                determinant += matrix[0, i] * Math.Pow(-1, i) * Determinant(SmallMatrix(matrix, 0, i));
            }
            return determinant;
        }
        static int [,] SmallMatrix (int [,] bigMatrix, int i, int j)
        {
            int[,] smallMatrix = new int[bigMatrix.GetLength(0) - 1, bigMatrix.GetLength(0) - 1];
            for (int iBigMatrix = 0, iSmallMatrix = 0; iBigMatrix < bigMatrix.GetLength(0); iSmallMatrix += (i == iBigMatrix ? 0 : 1), iBigMatrix++)
            {
                if (iBigMatrix == i)
                {
                    continue;
                }
                for (int jBigMatrix = 0, jSmallMatrix = 0; jBigMatrix < bigMatrix.GetLength(1); jSmallMatrix += (j == jBigMatrix ? 0 : 1), jBigMatrix++)
                {
                    if (jBigMatrix == j)
                    {
                        continue;
                    }
                    smallMatrix[iSmallMatrix, jSmallMatrix] = bigMatrix[iBigMatrix, jBigMatrix];
                }
                    
            }
            return smallMatrix;
        }
        static void WriteMatrix(int[,] mat)
        {
            for (int i = 0; i < mat.GetLength(0); i++)
            {
                for (int j = 0; j < mat.GetLength(1); j++)
                {
                    Console.Write($"{mat[i, j]}\t");
                }
                Console.WriteLine();
            }
        }
    }
}
