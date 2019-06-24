using System;
using System.Linq;
using System.Text;

namespace KramerMetod
{
    class Program
    {
        static void Main(string[] args)
        {
            (int[,] matrix, int[,] ansverMatrix) = ReadString();
            var determinantBasicMatrix = Determinant(matrix);
            Console.WriteLine($"Det = {determinantBasicMatrix}");
            int[] oldMatrix = new int[matrix.GetLength(0)];
            var allDeterminantDivideRoBasicMatrix = 0d;
            for (int number = 0; number < matrix.GetLength(0); number++)
            {
                for (int i = 0; i < matrix.GetLength(0); i++)
                {
                    oldMatrix[i] = matrix[i, number];
                    matrix[i, number] = ansverMatrix[i, 0];
                }
                allDeterminantDivideRoBasicMatrix = (double)Determinant(matrix) / (double)determinantBasicMatrix;
                Console.WriteLine($"x{number+1} = {allDeterminantDivideRoBasicMatrix}");
                for (int i = 0; i < matrix.GetLength(0); i++)
                {
                    matrix[i, number] = oldMatrix[i];
                }
            }
            Console.ReadKey();
        }
        static int Determinant(int[,] matrix)
        {
            if (matrix.Length == 1)
            {
                return matrix[0, 0];
            }
            var determinant = 0;
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                determinant += (int)(matrix[0, i] * Math.Pow(-1, i) * Determinant(SmallMatrix(matrix, 0, i)));
            }
            return determinant;
        }
        static int[,] SmallMatrix(int[,] bigMatrix, int i, int j)
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
            Console.WriteLine();
            for (int i = 0; i < mat.GetLength(0); i++)
            {
                for (int j = 0; j < mat.GetLength(1); j++)
                {
                    Console.Write($"{mat[i, j]}\t");
                }
                Console.WriteLine();
            }
        }
        static int[,] ReadMatrix()
        {
            var (n, m) = ReadBorder();
            int[,] matrix = new int[n, m];
            Console.WriteLine("Write matrix");
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    matrix[i, j] = ReadIntUnit(' ');
                }
                Console.WriteLine();
            }
            return matrix;

        }
        static int ReadIntUnit(params char[] unitSymbol)
        {
            StringBuilder sb = new StringBuilder();
            while (true)
            {
                var key = Console.ReadKey();
                if (unitSymbol.Contains(key.KeyChar))
                {
                    return Int32.Parse(sb.ToString());
                }
                else
                {
                    sb.Append(key.KeyChar);
                }
            }
        }
        static (int n, int m) ReadBorder()
        {
            Console.WriteLine("Enter the number of equation");
            Console.Write("Number of equation=");
            var n = ReadInt();
            Console.WriteLine("Enter the number of variables");
            Console.Write("Number of variables=");
            var m = ReadInt();
            return (n, m);
        }
        static int ReadInt()
        {
            var key = Console.ReadKey();
            if (key.Key == ConsoleKey.Escape)
            {
                throw new OperationCanceledException();
            }
            var line = Console.ReadLine();
            var keyLine = key.KeyChar + line;
            return Int32.Parse(keyLine);
        }
        static (int[,], int[,]) ReadString()
        {
            var (n, m) = ReadBorder();
            int[,] matrixEquation = new int[n,m];
            int[,] ansverMatrix = new int[n,1];
            var numEquation = 1;
            while (true)
            {
                string stringEquation = Read($"{numEquation} equation");
                var printEquation = Split(stringEquation);
                ansverMatrix[numEquation-1,0] = Int32.Parse(printEquation[printEquation.Length-1]);
                for(int j = 0; j < printEquation.Length-1; j++)
                {
                    matrixEquation[numEquation-1, j] = Int32.Parse(printEquation[j]);
                }
                numEquation++;
                var key = Console.ReadKey();
                if(key.Key == ConsoleKey.Tab)
                {
                    return (matrixEquation, ansverMatrix);
                }
            }
        }
        static string[] Split(string split)
        {
            return split.Split(new[] { ' ', 'x', '=' }, StringSplitOptions.RemoveEmptyEntries);
        }
        static string Read(string read)
        {
            do
            {
                Console.WriteLine($"Enter {read}");
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.Escape)
                {
                    throw new OperationCanceledException();
                }
                var line = Console.ReadLine();
                var keylin = $"{key.KeyChar}{line}";
                return Convert.ToString(keylin);
            } while (true);
        }
    }
}
