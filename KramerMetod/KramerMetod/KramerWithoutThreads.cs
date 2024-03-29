﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace KramerMetod
{
    class KramerWithoutThreads
    {
        public KramerWithoutThreads()
        {
        }
        private int[,] matrixEquation;
        private int[] matrixAnsver;
        private int numberOfEquations;
        int determinant;

        private void InitializeMatrix()
        {
            numberOfEquations = ReadInt("number of equation");
            matrixEquation = new int[numberOfEquations, numberOfEquations];
            matrixAnsver = new int[numberOfEquations];
        }
        public void FindDeterminantAndX()
        {
            InitializeMatrix();
            FindTwoMatrix();
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            var determinantBasicMatrix = Determinant(matrixEquation);
            Console.WriteLine($"Determinant = {determinantBasicMatrix}");
            if (determinantBasicMatrix == 0)
            {
                Console.WriteLine($"The system is unlimited");
            }
            else
            {
                int[] oldMatrix = new int[matrixEquation.GetLength(0)];
                var allDeterminantDivideRoBasicMatrix = 0d;
                for (int number = 0; number < matrixEquation.GetLength(0); number++)
                {
                    for (int i = 0; i < matrixEquation.GetLength(0); i++)
                    {
                        oldMatrix[i] = matrixEquation[i, number];
                        matrixEquation[i, number] = matrixAnsver[i];
                    }
                    allDeterminantDivideRoBasicMatrix = (double)Determinant(matrixEquation) / (double)determinantBasicMatrix;
                    Console.WriteLine($"x{number + 1} = {allDeterminantDivideRoBasicMatrix}");
                    for (int i = 0; i < matrixEquation.GetLength(0); i++)
                    {
                        matrixEquation[i, number] = oldMatrix[i];
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


        private int Determinant(int[,] matrix)
        {
            if (matrix.Length == 1)
            {
                return matrix[0, 0];
            }
            var determinant = 0;
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                var s = Determinant(SmallMatrix(matrix, 0, i));
                determinant += (int)(matrix[0, i] * Math.Pow(-1, i) * s);
            }
            return determinant;
        }
        private int[,] SmallMatrix(int[,] bigMatrix, int i, int j)
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
        private void FindTwoMatrix()
        {
            for (int i = 0; i < matrixEquation.GetLength(0); i++)
            {
                do
                {
                    var equation = ReadString($"{i + 1} equation");
                    var equationWithoutIndex = DeleteIndex(equation);
                    var splitEquation = Split(equationWithoutIndex);
                    try
                    {
                        if (splitEquation.Length == numberOfEquations + 1)
                        {
                            matrixAnsver[i] = Int32.Parse(splitEquation[splitEquation.Length - 1]);
                            for (int j = 0; j < splitEquation.Length - 1; j++)
                            {
                                matrixEquation[i, j] = Int32.Parse(splitEquation[j]);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Bed input, try again");
                            continue;
                        }
                        break;
                    }
                    catch (FormatException ex)
                    {
                        Console.WriteLine($"Bed input {ex.Message}, try gain or click Escape");
                    }
                } while (true);
            }
        }
        private string DeleteIndex(string equation)
        {
            StringBuilder equationWithoutIndex = new StringBuilder();
            for (int i = 0; i < equation.Length; i++)
            {
                if (equation[i] == 'x')
                {
                    equationWithoutIndex.Append(equation[i]);
                    i++;
                    for (int j = i; j < 10000; j++)
                    {
                        if (equation[j] >= '0' && equation[j] <= '9')
                        {
                            i++;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                equationWithoutIndex.Append(equation[i]);
            }
            return equationWithoutIndex.ToString();
        }
        private int ReadInt(string readString)
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
                catch (FormatException ex)
                {
                    Console.WriteLine($"Bed input {ex.Message}, try again or click Escape");
                }
            } while (true);
        }
        private string ReadString(string read)
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
        private string[] Split(string split)
        {
            return split.Split(new[] { ' ', 'x', '=' }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
