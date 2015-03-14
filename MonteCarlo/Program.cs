#define PARALLEL
// 4.9X Speedup on 8 Core i7 2600
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MonteCarlo
{
    class Program
    {
        const long _numIterations = 500000L;
        //const long _numIterations = 7L;
        const long _dartsPerIter = 10000L;
        //const long _dartsPerIter = 1000000000L;

        protected static long ThrowDarts(long Iterations, Random random)
        {
            long dartsInsideCircle = 0;
            for (long iteration = 0; iteration < Iterations; iteration++)
            {
                double pointX = random.NextDouble();
                double pointY = random.NextDouble();

                //double distanceFromOrigin = Math.Sqrt(pointX * pointX + pointY * pointY);
                double distanceFromOrigin = pointX * pointX + pointY * pointY;
                //bool pointInsideCircle = distanceFromOrigin <= 1.0;
                bool pointInsideCircle = (distanceFromOrigin * distanceFromOrigin) <= 1.0;

                if (pointInsideCircle)
                {
                    dartsInsideCircle++;
                }
            }
            return dartsInsideCircle;
        }
        public long CountlongerationsInsideCircle()
        {
            ConcurrentBag<long> results = new ConcurrentBag<long>();
#if PARALLEL
            Parallel.For(0, _numIterations,
                // initialise each thread by setting it's hit count to 0
                         () => new LoopThreadState(),
                // in the body, we throw one dart and see whether it hit or not
                         (iteration, _, localState) =>
                         {
                             localState.Count += ThrowDarts(_dartsPerIter, localState.RandomNumberGenerator);
                             
                             return localState;
                         },
                // finally, we sum (in a thread-safe way) all the hit counts of each thread together
                         result => results.Add(result.Count));
#else
            var localState = new LoopThreadState();
            for (long i = 0; i < _numIterations; i++)
            {
                results.Add(ThrowDarts(_dartsPerIter, localState.RandomNumberGenerator));
            }
#endif
            return results.Sum();
        }

        public static void Main(string[] args)
        {

            Console.WriteLine("Running the test");
            var stopwatch = Stopwatch.StartNew();
            var inside = new Program().CountlongerationsInsideCircle();
            stopwatch.Stop();
            Console.WriteLine("Approx: {0}/{1} => Pi: {2}, in {3} ms",
                               inside, _numIterations * _dartsPerIter,
                               (4.0 * inside) / (1.0 * _numIterations * _dartsPerIter),
                               stopwatch.ElapsedMilliseconds);

            Console.ReadKey();
        }
    }
}
