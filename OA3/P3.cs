using System;
using System.Diagnostics;
using static Extensions.ArrayExtensions;

namespace OA3
{
    public class Program
    {
        static void Main(string[] args)
        {
            var methods = new Func<double[,], double[,], double[,]>[]
            {
                MultiplyAssign,
                MultiplyRecursively
            };
            foreach (var method in methods)
            {
                double avgAcc = 0;
                Console.WriteLine($"Calculations with {method.Method.Name}..");

                for (int n = 5; n <= 500; n *= 2 + 1)
                {
                    Console.WriteLine($"Matrix size: {n}");
                    var s = new Stopwatch();
                    s.Start();
                    var _ = method(CreateAmatrix(n), CreateBmatrix(n));
                    s.Stop();
                    avgAcc += 1.0 / 5 * s.ElapsedMilliseconds;
                    Console.WriteLine($"Calculations done for {n}x{n} in {s.Elapsed.TotalMilliseconds}ms!");
                }
                Console.WriteLine($"Avg: {avgAcc}ms");
            }
        }

        private static double[,] CreateAmatrix(int size)
        {
            var r = new double[size, size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (i >= size / 2)
                    {
                        if (j <= i && size - j <= i + 1)
                            r[i, j] = 1;
                    }
                }
            }
            return r;
        }
        private static double[,] CreateBmatrix(int size)
        {
            var r = new double[size, size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {

                    if (i <= j && size - i <= j + 1)
                        r[i, j] = 1;
                    if (j <= i && j < size - i)
                        r[i, j] = 1;
                }
            }
            return r;
        }

    }
}