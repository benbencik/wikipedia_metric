using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace WikipediaMetric
{
    public class Benchmark
    {
        private readonly List<TimeSpan> executionTimes; // Stores the execution times for each iteration
        private readonly Stopwatch stopwatch; // Measures elapsed time

        public int Iterations { get; private set; } // Number of iterations performed
        public TimeSpan TotalElapsedTime { get; private set; } // Total elapsed time across all iterations
        public TimeSpan AverageTime { get; private set; } // Average execution time per iteration

        public Benchmark()
        {
            executionTimes = new List<TimeSpan>();
            stopwatch = new Stopwatch();
        }

        public void Measure(Action action, int iterations = 1)
        {
            Iterations = iterations;
            executionTimes.Clear();
            TotalElapsedTime = TimeSpan.Zero;

            for (int i = 0; i < iterations; i++)
            {
                stopwatch.Restart();
                action.Invoke();
                stopwatch.Stop();
                TimeSpan elapsed = stopwatch.Elapsed;
                executionTimes.Add(elapsed);
                TotalElapsedTime += elapsed;
            }

            AverageTime = TimeSpan.FromTicks(TotalElapsedTime.Ticks / iterations);
        }
    }
}
