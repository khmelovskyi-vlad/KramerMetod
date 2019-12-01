using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace KramerMetod
{
    class KramerWithThreadFinalVersion
    {
        private int[,] matrixEquation;
        private int[] matrixAnsver;
        private int numberOfEquations;
        private static int numberOfLogicalProcessors = Environment.ProcessorCount;
        int determinant;
        private void InitializeMatrix()
        {
            numberOfEquations = ReadInt("number of equation");
            matrixEquation = new int[numberOfEquations, numberOfEquations];
            matrixAnsver = new int[numberOfEquations];
        }
        private int maxUsingThreads;
        private int needCountAction;
        private int needMethods;
        struct informationAboutMatrix
        {
            public int multiplyNumber;
            public int[,] smallMatrix;
            public ManualResetEvent resetEvent;
        }
        private void InitializeNeedMethods()
        {
            if (matrixAnsver.Length > 2)
            {
                if (matrixAnsver.Length == 3)
                {
                    needMethods = 3;
                    return;
                }
                needMethods = matrixAnsver.Length;
                for (int i = matrixAnsver.Length - 1; i > 2; i--)
                {
                    needMethods += needMethods * i;
                }
            }
            else
            {
                needMethods = 0;
            }
        }
        private void InitializeMaxUsingThreads()
        {
            maxUsingThreads = numberOfLogicalProcessors - 1;
        }
        private int Fib(int countFib)
        {
            if (countFib == 0)
            {
                return 1;
            }
            if (countFib == 1)
            {
                return 1;
            }
            return countFib * Fib(countFib - 1);
        }
        private void CreateManualResets()
        {
            manualResets = new ManualResetEvent[needCountAction + needMethods];
            for (int i = 0; i < manualResets.Length; i++)
            {
                manualResets[i] = new ManualResetEvent(false);
            }
        }
        ManualResetEvent[] manualResets;
        public void FindDeterminantAndX()
        {
            InitializeMatrix();
            AddEqutionsAndAnswers();
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            InitializeMaxUsingThreads();
            needCountAction = Fib(numberOfEquations);
            InitializeNeedMethods();
            CreateManualResets();
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
                    CreateManualResets();
                    countEvents = 0;
                    determinant = 0;
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
            string elapsedTime = String.Format($"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds:00}");
            Console.WriteLine("RunTime " + elapsedTime);
        }
        Mutex mutexCountEvents = new Mutex();

        private int Determinant(int[,] matrix)
        {
            if (matrix.Length == 1)
            {
                return matrix[0, 0];
            }
            CycleWithOrWithoutThread(matrix, 1);
            try
            {
                WaitHandle.WaitAll(manualResets);
            }
            catch (Exception)
            {
                throw new OperationCanceledException();
            }
            return determinant;
        }
        private void FindDeterminantInThread(object matrixInformation)
        {
            FindDeterminantWithoutThreads(matrixInformation);
            maxUsingThreads++;
        }
        private void FindDeterminantWithoutThreads(object matrixInformation)
        {
            var (matrix, multiplyNumber, resetEvent) = ConvertInformationObject(matrixInformation);
            if (matrix.Length == 1)
            {
                AddDeterminanAndCountAction(matrix, multiplyNumber, resetEvent);
                return;
            }
            CycleWithOrWithoutThread(matrix, multiplyNumber);
            resetEvent.Set();
        }
        private void CycleWithOrWithoutThread(int[,] matrix, int multiplyNumber)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                informationAboutMatrix newInformation = CreateStrInformation(matrix, i, multiplyNumber);
                if (maxUsingThreads != 0)
                {
                    CreateThread(newInformation);
                }
                else
                {
                    FindDeterminantWithoutThreads(newInformation);
                }
            }
        }
        private int countEvents = 0;
        private informationAboutMatrix CreateStrInformation(int[,] matrix, int i, int multiplyNumber)
        {
            informationAboutMatrix information = new informationAboutMatrix();
            information.multiplyNumber = (int)(matrix[0, i] * Math.Pow(-1, i)) * multiplyNumber;
            information.smallMatrix = SmallMatrix(matrix, 0, i);
            mutexCountEvents.WaitOne();
            information.resetEvent = manualResets[countEvents];
            countEvents++;
            mutexCountEvents.ReleaseMutex();
            return information;
        }
        private void CreateThread(informationAboutMatrix information)
        {
            maxUsingThreads--;
            ThreadPool.QueueUserWorkItem((x) => FindDeterminantInThread(x), information);
        }
        private void AddDeterminanAndCountAction(int[,] matrix, int multiplyNumber, ManualResetEvent resetEvent)
        {
            determinant += matrix[0, 0] * multiplyNumber;
            resetEvent.Set();

        }
        private (int[,] matrix, int multiplyNumber, ManualResetEvent resetEvent) ConvertInformationObject(object matrixInformation)
        {
            var information = (informationAboutMatrix)matrixInformation;
            return (information.smallMatrix, information.multiplyNumber, information.resetEvent);
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
        private void AddEqutionsAndAnswers()
        {
            for (int i = 0; i < matrixEquation.GetLength(0); i++)
            {
                do
                {
                    var equation = ReadString($"{i + 1} equation");
                    try
                    {
                        var (arrayEquation, havingAnswer, answer) = FindArrayEqutionAndAnswer(equation);
                        if (havingAnswer)
                        {
                            matrixAnsver[i] = answer;
                            for (int j = 0; j < arrayEquation.Length; j++)
                            {
                                matrixEquation[i, j] = arrayEquation[j];
                            }
                        }
                        else
                        {
                            Console.WriteLine("Bed input, try again");
                            continue;
                        }
                        break;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Bed input {ex.Message}, try gain or click Escape");
                    }
                } while (true);
            }
        }
        private (int[] arrayEquation, bool havingAnswer, int answer) FindArrayEqutionAndAnswer(string equation)
        {
            var arrayEquation = new int[numberOfEquations];
            var havingAnswer = false;
            int answer = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append(equation[0]);
            for (int i = 1; i < equation.Length; i++)
            {
                if (equation[i] >= '0' && equation[i] <= '9' || equation[i] == 'x')
                {
                    sb.Append(equation[i]);
                }
                else
                {
                    var numAndIndex = Split(sb.ToString());
                    if (sb[0] == 'x')
                    {
                        arrayEquation[Convert.ToInt32(numAndIndex[0]) - 1] += 1;
                    }
                    else if (numAndIndex[0] == "-")
                    {
                        arrayEquation[Convert.ToInt32(numAndIndex[1]) - 1] += -1;
                    }
                    else
                    {
                        arrayEquation[Convert.ToInt32(numAndIndex[1]) - 1] += Convert.ToInt32(numAndIndex[0]);
                    }
                    if (equation[i] == '=')
                    {
                        StringBuilder ansverSb = new StringBuilder();
                        for (int j = i + 1; j < equation.Length; j++)
                        {
                            ansverSb.Append(equation[j]);
                        }
                        var ansverString = ansverSb.ToString();
                        answer = Convert.ToInt32(ansverString);
                        havingAnswer = true;
                        break;
                    }
                    sb.Clear();
                    sb.Append(equation[i]);
                }
            }
            return (arrayEquation, havingAnswer, answer);
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