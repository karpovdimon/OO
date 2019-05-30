using Extensions;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using static Extensions.ArrayExtensions;

namespace OA2
{
    class Program
    {
        private static readonly Random _rand = new Random();
        static async Task Main(string[] args)
        {
            var lcts = new LimitedConcurrencyLevelTaskScheduler(10);
            var factory = new TaskFactory(lcts);
            var cts = new CancellationTokenSource();
            lowerRandBounds = 1;
            maxRandBounds = 1000;
            normalizer = 1.0 / 100;
            for (int n = 4; n <= 512; n *= 2)
            {
                var A1task = factory.StartNew(() => AllocateRandomSquareMatrix(n), cts.Token);
                var A2task = factory.StartNew(() => AllocateRandomSquareMatrix(n), cts.Token);
                var B2task = factory.StartNew(() => AllocateRandomSquareMatrix(n), cts.Token);
                var b1task = factory.StartNew(() => AllocateRandomVector(n), cts.Token);
                var c1task = factory.StartNew(() => AllocateRandomVector(n), cts.Token);

                var K1 = _rand.Next(100) * .00001;
                var K2 = _rand.Next(100) * .00001;
                /*
                 b = even 3/(Math.Pow(i,2) +3)
                 b = odd 3/i  */
                var btask = factory.StartNew(() => FindVectorB(n,
                                        i => 3.0 / (Math.Pow(i, 2) + 3),
                                        i => 3.0 / i), cts.Token);

                // Cij = 1/(i+j)*2;
                var C2task = factory.StartNew(() => FindMatrixC(n, (i, j) => 1.0 / (i + j) * 2), cts.Token);

                var y1task = factory.StartNew(async () => Multiply(await A1task, await btask), cts.Token).Unwrap();
                // y2 = A1*(3*b1+c1) 
                var y2task = factory.StartNew(async () =>
                    Multiply(await A1task, Add(Multiply(3, await b1task), await c1task)), cts.Token).Unwrap();
                var B2_C2_Addtask = factory.StartNew(async () => Add(await B2task, await C2task), cts.Token).Unwrap();
                var Y3task = factory.StartNew(async () => Multiply(await A2task, await B2_C2_Addtask), cts.Token).Unwrap();
                var y3power3task = factory.StartNew(async () => MatrixPower(await Y3task, 3), cts.Token).Unwrap();
                var y3power2task = factory.StartNew(async () => MatrixPower(await Y3task, 2), cts.Token).Unwrap();

                var K1Y33task = factory.StartNew(async () => Multiply(await y3power3task, K1), cts.Token).Unwrap();
                var y2y2Ttask = factory.StartNew(async () => Multiply(await y2task, await y2task), cts.Token).Unwrap();
                var K1Y33y2y2T = factory.StartNew(async () => Multiply(await K1Y33task, await y2y2Ttask), cts.Token).Unwrap();

                var _2ph = factory.StartNew(async () => Add(await K1Y33y2y2T, await y3power3task), cts.Token).Unwrap();
                var _3ph = factory.StartNew(async () => Substract(await _2ph, await Y3task), cts.Token).Unwrap();
                var _4ph = factory.StartNew(async () => Multiply(await y2task, await y1task), cts.Token).Unwrap();
                var _5ph = factory.StartNew(async () => Add(await _3ph, await _4ph), cts.Token).Unwrap();
                var _6ph = factory.StartNew(async () => Multiply(Multiply(await y3power2task, K2), await y1task), cts.Token).Unwrap();
                var _7ph = factory.StartNew(async () => Add(await _5ph, await _6ph), cts.Token).Unwrap();
                // x = (K1 * Y3 * y2 * y2t) + Math.Pow(Y3,3) - Y3 - (y2 * y1t) + (K2 * Math.Pow(Y3,2) * y1t)
                Console.WriteLine($"Threads started.. Matrix size: {n}");
                Stopwatch s = new Stopwatch();
                s.Start();
                var u = await _7ph;
                s.Stop();
                Console.WriteLine($"Calculations done for {n}x{n} in {s.Elapsed.TotalMilliseconds}ms!");
                // u.Print();
            }
        }

        private static double[] FindVectorB(int size, Func<int, double> forEven, Func<int, double> forOdd)
        {
            var array = new double[size];
            for (int i = 0; i < size; i++)
            {
                if (i % 2 == 0) //even
                    array[i] = forEven(i);
                else
                    array[i] = forOdd(i);
            }
            return array;
        }

        private static double[,] FindMatrixC(int size, Func<int, int, double> expression)
        {
            var r = new double[size, size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    r[i, j] = expression(i, j);
                }
            }

            return r;
        }
    }
}
