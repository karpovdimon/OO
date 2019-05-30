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
            lowerRandBounds = 1;
            maxRandBounds = 1000;
            normalizer = 1.0 / 100;
            Console.WriteLine("Starting TPL dependent flow");
            await TPLDependent();
            Console.WriteLine("TPL dependent flow END\n");
            Console.WriteLine("Starting thread native dependent flow");
            await ThreadDependent();
            Console.WriteLine("Thread native dependent flow END");
        }

        static Task ThreadDependent()
        {
            for (int n = 4; n <= 512; n *= 2)
            {
                double[,] A1 = { }, A2 = { }, B2 = { }, C2 = { }, B2_C2_Add = { }, Y3 = { }, y3power3 = { }, y3power2 = { };
                double[] b1 = { }, c1 = { }, b = { }, y1 = { }, y2 = { };
                double y2y2T = 0;
                var A1thread = new Thread(() => A1 = AllocateRandomSquareMatrix(n));
                var A2thread = new Thread(() => A2 = AllocateRandomSquareMatrix(n));
                var B2thread = new Thread(() => B2 = AllocateRandomSquareMatrix(n));
                var b1thread = new Thread(() => b1 = AllocateRandomVector(n));
                var c1thread = new Thread(() => c1 = AllocateRandomVector(n));

                Console.WriteLine($"Threads started.. Matrix size: {n}");
                Stopwatch s = new Stopwatch();
                s.Start();
                A1thread.Start();
                A2thread.Start();
                B2thread.Start();
                b1thread.Start();
                c1thread.Start();

                var K1 = _rand.Next(100) * .00001;
                var K2 = _rand.Next(100) * .00001;
                /*
                 b = even 3/(Math.Pow(i,2) +3)
                 b = odd 3/i  */
                var bthread = new Thread(() =>
                {
                    b = FindVectorB(n,
                        i => 3.0 / (Math.Pow(i, 2) + 3),
                        i => 3.0 / i);
                });
                bthread.Start();
                // Cij = 1/(i+j)*2;
                var C2thread = new Thread(() =>
                {
                    C2 = FindMatrixC(n, (i, j) => 1.0 / (i + j) * 2);
                });
                C2thread.Start();

                var y1thread = new Thread(() =>
                {
                    A1thread.Join();
                    bthread.Join();
                    y1 = Multiply(A1, b);
                });
                y1thread.Start();
                // y2 = A1*(3*b1+c1) 
                var y2thread = new Thread(() =>
                {
                    A1thread.Join();
                    b1thread.Join();
                    c1thread.Join();
                    y2 = Multiply(A1, Add(Multiply(3, b1), c1));
                });
                y2thread.Start();
                var B2_C2_Addthread = new Thread(() =>
                {
                    C2thread.Join();
                    B2thread.Join();
                    B2_C2_Add = Add(B2, C2);
                });
                B2_C2_Addthread.Start();
                var Y3thread = new Thread(() =>
                {
                    A2thread.Join();
                    B2_C2_Addthread.Join();
                    Y3 = Multiply(A2, B2_C2_Add);
                });
                Y3thread.Start();
                var y3power3thread = new Thread(() =>
                {
                    Y3thread.Join();
                    y3power3 = MatrixPower(Y3, 3);
                });
                y3power3thread.Start();
                var y3power2thread = new Thread(() =>
                {
                    Y3thread.Join();
                    y3power2 = MatrixPower(Y3, 2);
                });
                y3power2thread.Start();

                double[,] K1Y33 = { };
                var K1Y33thread = new Thread(() =>
                {
                    y3power3thread.Join();
                    K1Y33 = Multiply(y3power3, K1);
                });
                K1Y33thread.Start();
                var y2y2Tthread = new Thread(() =>
                {
                    y2thread.Join();
                    y2y2T = Multiply(y2, y2);
                });
                y2y2Tthread.Start();
                double[,] K1Y33y2y2T = { };
                var K1Y33y2y2Tthread = new Thread(() =>
                {
                    K1Y33thread.Join();
                    y2y2Tthread.Join();
                    K1Y33y2y2T = Multiply(K1Y33, y2y2T);
                });
                K1Y33y2y2Tthread.Start();

                double[,] _2phV = { };
                var _2ph = new Thread(() =>
                {
                    K1Y33y2y2Tthread.Join();
                    y3power3thread.Join();
                    _2phV = Add(K1Y33y2y2T, y3power3);
                });
                _2ph.Start();
                double[,] _3phV = { };
                var _3ph = new Thread(() =>
                {
                    _2ph.Join();
                    _3phV = Substract(_2phV, Y3);
                });
                _3ph.Start();
                double _4phV = 0;
                var _4ph = new Thread(() =>
                {
                    y2thread.Join();
                    y1thread.Join();
                    _4phV = Multiply(y2, y1);
                });
                _4ph.Start();
                double[,] _5phV = { };
                var _5ph = new Thread(() =>
                {
                    _4ph.Join();
                    _3ph.Join();
                    _5phV = Add(_3phV, _4phV);
                });
                _5ph.Start();
                double[] _6phV = { };
                var _6ph = new Thread(() =>
                {
                    y3power2thread.Join();
                    y1thread.Join();
                    _6phV = Multiply(Multiply(y3power2, K2), y1);
                });
                _6ph.Start();
                double[,] x = { };
                var _7ph = new Thread(() =>
                {
                    _5ph.Join();
                    _6ph.Join();
                    x = Add(_5phV, _6phV);
                });
                // x = (K1 * Y3 * y2 * y2t) + Math.Pow(Y3,3) - Y3 - (y2 * y1t) + (K2 * Math.Pow(Y3,2) * y1t)

                _7ph.Start();
                _7ph.Join();
                s.Stop();
                Console.WriteLine($"Calculations done for {n}x{n} in {s.Elapsed.TotalMilliseconds}ms!");
                // u.Print();
            }

            return Task.CompletedTask;
        }
        private static async Task TPLDependent()
        {
            var lcts = new LimitedConcurrencyLevelTaskScheduler(10);
            var factory = new TaskFactory(lcts);
            var cts = new CancellationTokenSource();
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
