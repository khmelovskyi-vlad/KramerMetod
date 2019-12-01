using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KramerMetod
{
    class KramerUsingThreads
    {
        public KramerUsingThreads()
        {
        }
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
        Semaphore semaphores;
        private int semaphoresCount;
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
        private void InitializeSemaphore()
        {
            semaphores = new Semaphore(numberOfLogicalProcessors, numberOfLogicalProcessors);
            semaphoresCount = numberOfLogicalProcessors - 1;
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
            return countFib*Fib(countFib-1);
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
            FindTwoMatrix();
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            InitializeSemaphore();
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
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds);
            Console.WriteLine("RunTime " + elapsedTime);
        }
        Mutex mutex = new Mutex();
        
        private int Determinant(int[,] matrix)
        {
            if (matrix.Length == 1)
            {
                return matrix[0, 0];
            }
            Cycle(matrix, 1);
            WaitHandle.WaitAll(manualResets);
            return determinant;
        }
        private void DeterminantInThread(object matrixInformation)
        {
            DeterminantWithoutThreads(matrixInformation);
            semaphoresCount++;
        }
        private void DeterminantWithoutThreads(object matrixInformation)
        {
            var (matrix, multiplyNumber, resetEvent) = ConvertInformationObject(matrixInformation);
            if (matrix.Length == 1)
            {
                AddDeterminanAndCountAction(matrix, multiplyNumber, resetEvent);
                return;
            }
            Cycle(matrix, multiplyNumber);
            resetEvent.Set();
        }
        private void Cycle(int[,] matrix, int multiplyNumber)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                informationAboutMatrix newInformation = CreateStrInformation(matrix, i, multiplyNumber);
                if (semaphoresCount != 0)
                {
                    CreateThread(newInformation);
                }
                else
                {
                    DeterminantWithoutThreads(newInformation);
                }
            }
        }
        private int countEvents = 0;
        private informationAboutMatrix CreateStrInformation(int[,] matrix, int i, int multiplyNumber)
        {
            informationAboutMatrix information = new informationAboutMatrix();
            information.multiplyNumber = (int)(matrix[0, i] * Math.Pow(-1, i)) * multiplyNumber;
            information.smallMatrix = SmallMatrix(matrix, 0, i);
            mutex.WaitOne();
            information.resetEvent = manualResets[countEvents];
            countEvents++;
            mutex.ReleaseMutex();
            return information;
        }
        private void CreateThread(informationAboutMatrix information)
        {
            semaphoresCount--;
            ThreadPool.QueueUserWorkItem((x) => DeterminantInThread(x), information);
        }
        private void AddDeterminanAndCountAction(int[,] matrix, int multiplyNumber, ManualResetEvent resetEvent)
        {
            determinant += matrix[0, 0] * multiplyNumber;
            resetEvent.Set();
            
        }
        private (int [,] matrix, int multiplyNumber, ManualResetEvent resetEvent) ConvertInformationObject(object matrixInformation)
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
