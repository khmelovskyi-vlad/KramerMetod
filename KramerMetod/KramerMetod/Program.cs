﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KramerMetod
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Kramer method");
            KramerWithThreadFinalVersion kramerWithThreadFinalVersion = new KramerWithThreadFinalVersion();
            kramerWithThreadFinalVersion.FindDeterminantAndX();
            //FindDeterminantAndX();
            //Task.Run(() => Console.ReadKey());
            //KramerUsingThreadsBad kramerUsingThreadsBad = new KramerUsingThreadsBad();
            //kramerUsingThreadsBad.FindDeterminantAndX();
            //KramerUsingThreads kramerUsingThreads = new KramerUsingThreads();
            //kramerUsingThreads.FindDeterminantAndX();
            //KramerWithoutThreads kramerWithoutThreads = new KramerWithoutThreads();
            //kramerWithoutThreads.FindDeterminantAndX();
            Console.ReadKey();
        }
        static void FindDeterminantAndX()
        {
            (int[,] matrix, int[,] ansverMatrix) = ReturnTwoMatrix();
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            var determinantBasicMatrix = Determinant(matrix);
            Console.WriteLine($"Determinant = {determinantBasicMatrix}");
            if (determinantBasicMatrix == 0)
            {
                Console.WriteLine($"The system is unlimited");
            }
            else
            {
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
                    Console.WriteLine($"x{number + 1} = {allDeterminantDivideRoBasicMatrix}");
                    for (int i = 0; i < matrix.GetLength(0); i++)
                    {
                        matrix[i, number] = oldMatrix[i];
                    }
                }
            }
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds);
            Console.WriteLine("RunTime " + elapsedTime);
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
        static (int[,], int[,]) ReturnTwoMatrix()
        {
            var n = ReadInt("number of equation");
            int[,] matrixEquation = new int[n, n];
            int[,] ansverMatrix = new int[n,1];
            for (int i = 0; i < matrixEquation.GetLength(0); i++)
            {
                do
                {
                    string stringEquation = ReadString($"{i + 1} equation without index x");
                    var printEquation = Split(stringEquation);
                    try
                    {
                        if (printEquation.Length == n + 1)
                        {
                            ansverMatrix[i, 0] = Int32.Parse(printEquation[printEquation.Length - 1]);
                            for (int j = 0; j < printEquation.Length - 1; j++)
                            {
                                matrixEquation[i, j] = Int32.Parse(printEquation[j]);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Bed input, try again");
                            continue;
                        }
                        break;
                    }
                    catch(FormatException ex)
                    {
                        Console.WriteLine($"Bed input {ex.Message}, try gain or click Escape");
                    }
                } while (true);
            }
            return (matrixEquation, ansverMatrix);
        }
        static int ReadInt(string  readString)
        {
            do
            {
                try
                {
                    Console.WriteLine($"Enter {readString}");
                    Console.Write($"{readString} = ");
                    var key = Console.ReadKey();
                    if (key.Key == ConsoleKey.Escape)
                    {
                        throw new OperationCanceledException();
                    }
                    var line = Console.ReadLine();
                    var keyLine = key.KeyChar + line;
                    return Int32.Parse(keyLine);
                }
                catch(FormatException ex)
                {
                    Console.WriteLine($"Bed input {ex.Message}, try again or click Escape");
                }
            } while (true);
        }
        static string ReadString(string read)
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
        }
        static string[] Split(string split)
        {
            return split.Split(new[] { ' ', 'x', '=' }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
