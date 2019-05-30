using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Extensions
{
    // Provides a task scheduler that ensures a maximum concurrency level while 
    // running on top of the thread pool.
    public class LimitedConcurrencyLevelTaskScheduler : TaskScheduler
    {
        // Indicates whether the current thread is processing work items.
        [ThreadStatic]
        private static bool _currentThreadIsProcessingItems;

        // The list of tasks to be executed 
        private readonly LinkedList<Task> _tasks = new LinkedList<Task>(); // protected by lock(_tasks)

        // The maximum concurrency level allowed by this scheduler. 
        private readonly int _maxDegreeOfParallelism;

        // Indicates whether the scheduler is currently processing work items. 
        private int _delegatesQueuedOrRunning;

        // Creates a new instance with the specified degree of parallelism. 
        public LimitedConcurrencyLevelTaskScheduler(int maxDegreeOfParallelism)
        {
            if (maxDegreeOfParallelism < 1) throw new ArgumentOutOfRangeException("maxDegreeOfParallelism");
            _maxDegreeOfParallelism = maxDegreeOfParallelism;
        }

        // Queues a task to the scheduler. 
        protected sealed override void QueueTask(Task task)
        {
            // Add the task to the list of tasks to be processed.  If there aren't enough 
            // delegates currently queued or running to process tasks, schedule another. 
            lock (_tasks)
            {
                _tasks.AddLast(task);
                if (_delegatesQueuedOrRunning < _maxDegreeOfParallelism)
                {
                    ++_delegatesQueuedOrRunning;
                    NotifyThreadPoolOfPendingWork();
                }
            }
        }

        // Inform the ThreadPool that there's work to be executed for this scheduler. 
        private void NotifyThreadPoolOfPendingWork()
        {
            ThreadPool.UnsafeQueueUserWorkItem(_ =>
            {
                // Note that the current thread is now processing work items.
                // This is necessary to enable inlining of tasks into this thread.
                _currentThreadIsProcessingItems = true;
                try
                {
                    // Process all available items in the queue.
                    while (true)
                    {
                        Task item;
                        lock (_tasks)
                        {
                            // When there are no more items to be processed,
                            // note that we're done processing, and get out.
                            if (_tasks.Count == 0)
                            {
                                --_delegatesQueuedOrRunning;
                                break;
                            }

                            // Get the next item from the queue
                            item = _tasks.First.Value;
                            _tasks.RemoveFirst();
                        }

                        // Execute the task we pulled out of the queue
                        TryExecuteTask(item);
                    }
                }
                // We're done processing items on the current thread
                finally { _currentThreadIsProcessingItems = false; }
            }, null);
        }

        // Attempts to execute the specified task on the current thread. 
        protected sealed override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            // If this thread isn't already processing a task, we don't support inlining
            if (!_currentThreadIsProcessingItems) return false;

            // If the task was previously queued, remove it from the queue
            if (taskWasPreviouslyQueued)
                // Try to run the task. 
                if (TryDequeue(task))
                    return TryExecuteTask(task);
                else
                    return false;
            else
                return TryExecuteTask(task);
        }

        // Attempt to remove a previously scheduled task from the scheduler. 
        protected sealed override bool TryDequeue(Task task)
        {
            lock (_tasks) return _tasks.Remove(task);
        }

        // Gets the maximum concurrency level supported by this scheduler. 
        public sealed override int MaximumConcurrencyLevel { get { return _maxDegreeOfParallelism; } }

        // Gets an enumerable of the tasks currently scheduled on this scheduler. 
        protected sealed override IEnumerable<Task> GetScheduledTasks()
        {
            bool lockTaken = false;
            try
            {
                Monitor.TryEnter(_tasks, ref lockTaken);
                if (lockTaken) return _tasks;
                else throw new NotSupportedException();
            }
            finally
            {
                if (lockTaken) Monitor.Exit(_tasks);
            }
        }
    }
    public static class ActionExtensions
    {
        /// <summary>
        /// This method implements <see cref="Console.WriteLine()"/>
        /// </summary>
        public static Action _ => Console.WriteLine;
    }
    public static class MatrixExtensions
    {
        public static double[,] MultiplyMatrix(this double[,] A, double[,] B)
        {
            int rA = A.GetLength(0);
            int cA = A.GetLength(1);
            int rB = B.GetLength(0);
            int cB = B.GetLength(1);

            double[,] kHasil = new double[rA, cB];
            if (cA != rB)
            {
                Console.WriteLine("matrices can't be multiplied !!");
            }
            else
            {
                for (int i = 0; i < rA; i++)
                {
                    for (int j = 0; j < cB; j++)
                    {
                        double temp = 0;
                        for (int k = 0; k < cA; k++)
                        {
                            temp += A[i, k] * B[k, j];
                        }
                        kHasil[i, j] = temp;
                    }
                }
                return kHasil;
            }

            return default;
        }
        /// <summary>
        /// Returns the row with number 'row' of this matrix as a 1D-Array.
        /// </summary>
        public static T[] GetRow<T>(this T[,] matrix, int row)
        {
            var rowLength = matrix.GetLength(1);
            var rowVector = new T[rowLength];

            for (var i = 0; i < rowLength; i++)
                rowVector[i] = matrix[row, i];

            return rowVector;
        }



        /// <summary>
        /// Sets the row with number 'row' of this 2D-matrix to the parameter 'rowVector'.
        /// </summary>
        public static void SetRow<T>(this T[,] matrix, int row, T[] rowVector)
        {
            var rowLength = matrix.GetLength(1);

            for (var i = 0; i < rowLength; i++)
                matrix[row, i] = rowVector[i];
        }



        /// <summary>
        /// Returns the column with number 'col' of this matrix as a 1D-Array.
        /// </summary>
        public static T[] GetCol<T>(this T[,] matrix, int col)
        {
            var colLength = matrix.GetLength(0);
            var colVector = new T[colLength];

            for (var i = 0; i < colLength; i++)
                colVector[i] = matrix[i, col];

            return colVector;
        }



        /// <summary>
        /// Sets the column with number 'col' of this 2D-matrix to the parameter 'colVector'.
        /// </summary>
        public static void SetCol<T>(this T[,] matrix, int col, T[] colVector)
        {
            var colLength = matrix.GetLength(0);

            for (var i = 0; i < colLength; i++)
                matrix[i, col] = colVector[i];
        }
    }
    public static class ArrayExtensions
    {
        public static int lowerRandBounds = 1;
        public static int maxRandBounds = 1000;
        /// <summary>
        /// Normalizes output of matrix randomizer
        /// </summary>
        public static double normalizer = 1.0 / 100.0;
        private static readonly Random _rand = new Random();
        public static void Print(this IEnumerable<double[]> arr, int tolerance = 5)
        {
            var asm = Assembly.GetEntryAssembly()?.DefinedTypes?.FirstOrDefault();
            Console.WriteLine($"Lab: {asm?.Namespace}");
            var arr0 = arr as double[][] ?? arr.ToArray();
            var a = arr0.Length;
            var b = arr0[0].Length;
            for (int i = 0; i < a; i++)
            {
                for (int j = 0; j < b; j++)
                {
                    var fx = arr0[i][j].ToString("f" + tolerance);
                    Console.Write((new string(' ', Math.Abs(tolerance * 2 - fx.Length)) + fx + ' '));
                }

                Console.WriteLine();
            }
            Console.WriteLine();
        }

        public static void Print(this double[,] arr, int tolerance = 5)
        {
            var a = arr.GetLength(0);
            var b = arr.GetLength(1);
            for (int i = 0; i < a; i++)
            {
                for (int j = 0; j < b; j++)
                {
                    var fx = arr[i, j].ToString("f" + tolerance);
                    Console.Write(fx);
                    Console.Write(new string(' ', 2));
                    // Console.Write((new string(' ', tolerance * 2 - fx.Length) + fx + ' '));
                }

                Console.WriteLine();
            }

        }
        public static void Print(this double[] arr, int tolerance = 5)
        {
            var a = arr.GetLength(0);
            for (int i = 0; i < a; i++)
            {
                var fx = arr[i].ToString("f" + tolerance);
                Console.Write((new string(' ', tolerance * 2 - fx.Length) + fx + ' '));
            }
            Console.WriteLine();
        }

        public static double[,] Add(double[,] array1, double[] array2)
        {
            var r = new double[array1.GetLength(0), array1.GetLength(1)];
            for (int i = 0; i < array1.GetLength(1); i++)
            {
                for (int j = 0; j < array1.GetLength(1); j++)
                {
                    r[i, j] = array1[i, j] + array2[i];
                }
            }

            return r;
        }

        public static double[] Add(double[] array1, double[] array2)
        {
            var r = new double[array1.Length];
            for (int i = 0; i < array1.GetLength(0); i++)
            {
                r[i] = array1[i] + array2[i];

            }
            return r;
        }

        public static double[,] Substract(double[,] array1, double[,] array2)
        {
            ThrowIfSizeNotEqual(array1, array2);
            var r = new double[array1.GetLength(0), array1.GetLength(1)];
            for (int i = 0; i < array1.GetLength(0); i++)
            {
                for (int j = 0; j < array1.GetLength(1); j++)
                {
                    r[i, j] = array1[i, j] - array2[i, j];
                }
            }

            return r;
        }

        public static double[,] Add(double[,] array1, double val)
        {
            var r = new double[array1.GetLength(0), array1.GetLength(1)];
            for (int i = 0; i < array1.GetLength(0); i++)
            {
                for (int j = 0; j < array1.GetLength(1); j++)
                {
                    r[i, j] = array1[i, j] + val;
                }
            }

            return r;
        }

        public static double[,] Add(double[,] array1, double[,] array2)
        {
            ThrowIfSizeNotEqual(array1, array2);
            var r = new double[array1.GetLength(0), array1.GetLength(1)];
            for (int i = 0; i < array1.GetLength(0); i++)
            {
                for (int j = 0; j < array1.GetLength(1); j++)
                {
                    r[i, j] = array1[i, j] + array2[i, j];
                }
            }

            return r;
        }

        public static double Multiply(double[] coll1, double[] coll2)
        {
            double temp = 0;
            for (int i = 0; i < coll1.Length; i++)
            {
                temp += coll1[i] * coll2[i];
            }

            return temp;
        }

        public static double[] Multiply(double val, double[] vector)
        {
            var r = new double[vector.Length];
            for (int i = 0; i < vector.GetLength(0); i++)
                r[i] = vector[i] * val;
            return r;
        }

        public static double[] Multiply(double[,] array1, double[] vector)
        {
            var r = new double[array1.GetLength(0)];
            for (int i = 0; i < array1.GetLength(0); i++)
            {
                double temp = 0;
                for (int j = 0; j < array1.GetLength(1); j++)
                {
                    temp += array1[i, j] * vector[j];
                }

                r[i] = temp;
            }

            return r;
        }

        public static double[,] Transpose(double[,] array)
        {
            var r = new double[array.GetLength(0), array.GetLength(1)];
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    r[j, i] = array[i, j];
                }
            }

            return r;
        }

        public static double[,] Multiply(double[,] array1, double x)
        {
            var r = new double[array1.GetLength(0), array1.GetLength(1)];
            for (int i = 0; i < array1.GetLength(0); i++)
            {
                for (int j = 0; j < array1.GetLength(1); j++)
                {
                    r[i, j] = array1[i, j] * x;
                }
            }

            return r;
        }

        public static double[,] MatrixPower(double[,] array1, int n)
        {
            if (n == 1) return array1;
            var r = Multiply(array1, array1);
            return MatrixPower(r, n - 1);
        }

        public static double[,] Multiply(double[,] array1, double[,] array2)
        {
            ThrowIfSizeNotEqual(array1, array2);
            var r = new double[array1.GetLength(0), array1.GetLength(1)];
            for (int i = 0; i < array1.GetLength(0); i++)
            {
                for (int j = 0; j < array1.GetLength(1); j++)
                {
                    for (int k = 0; k < array2.GetLength(0); k++)
                    {
                        r[i, j] += array1[i, k] * array1[k, j];
                    }
                }
            }

            return r;
        }

        public static double[,] MultiplyAssign(double[,] array1, double[,] array2)
        {
            var r = new double[array1.GetLength(0), array1.GetLength(1)];
            for (int i = 0; i < array1.GetLength(0); i++)
            {
                for (int j = 0; j < array1.GetLength(1); j++)
                {
                    for (int k = 0; k < array2.GetLength(0); k++)
                        r[i, j] = r[i, j] + array1[i, k] * array2[k, i];
                }
            }

            return r;
        }
        public static double[,] MultiplyRecursively(double[,] array1, double[,] array2)
        {
            var r = new double[array1.GetLength(0), array1.GetLength(1)];
            for (int i = 0; i < array1.GetLength(0); i++)
            {
                for (int j = 0; j < array1.GetLength(1); j++)
                {
                    r[i, j] = InnerRecursively(array1, array2, i, j, array2.GetLength(0) - 1);
                }
            }

            return r;
        }

        private static double InnerRecursively(double[,] array1, double[,] array2, int i, int j, int k)
        {
            if (k < 0)
                return 0;
            return array1[i, k] * array2[k, i] + InnerRecursively(array1, array2, i, j, k - 1);
        }

        public static double[] AllocateRandomVector(int size)
        {
            var array = new double[size];
            Randomize(array);
            return array;
        }

        public static double[,] AllocateRandomSquareMatrix(int size)
        {
            var array = new double[size, size];
            Randomize(array);
            return array;
        }

        public static void Randomize(double[] array)
        {
            for (int i = 0; i < array.GetLength(0); i++)
            {
                array[i] = _rand.Next(lowerRandBounds, maxRandBounds);
            }
        }

        public static void Randomize(double[,] array)
        {
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    array[i, j] = _rand.Next(lowerRandBounds, maxRandBounds) * normalizer;
                }
            }
        }

        private static void ThrowIfSizeNotEqual(double[,] array1, double[,] array2)
        {
            if (array1 == null) throw new ArgumentNullException(nameof(array1));
            if (array2 == null) throw new ArgumentNullException(nameof(array2));
            if (array1.GetLength(0) != array2.GetLength(0))
            {
                throw new ArgumentException("matrices sizes are not equal");
            }
        }
    }
}