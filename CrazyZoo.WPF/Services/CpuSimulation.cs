using System;
using System.Diagnostics;
using System.Threading;

namespace CrazyZoo.WPF.Services;

public record AnimalSnapshot(
    string Name,
    int Energy,
    int Happiness);

public record SimulationResult(
    string Name,
    string Status,
    string Details,
    long DurationMs);

public static class CpuSimulation
{
    public static SimulationResult Calculate(
        AnimalSnapshot animal,
        int iterations,
        CancellationToken token,
        bool forceError = false)
    {
        Stopwatch watch = Stopwatch.StartNew();

        try
        {
            if (forceError)
                throw new InvalidOperationException(
                    "Test error for this animal.");

            if (iterations <= 0)
                throw new ArgumentException(
                    "Iterations must be positive.");

            double result = 0;

            for (int i = 0; i < iterations; i++)
            {
                if (i % 4096 == 0)
                    token.ThrowIfCancellationRequested();

                double value =
                    animal.Energy * animal.Energy +
                    animal.Happiness * animal.Happiness +
                    i % 1000;

                result += Math.Sqrt(value);
            }

            token.ThrowIfCancellationRequested();
            watch.Stop();

            return new SimulationResult(
                animal.Name,
                "Success",
                $"Result: {result:F2}",
                watch.ElapsedMilliseconds);
        }
        catch (OperationCanceledException)
        {
            watch.Stop();

            return new SimulationResult(
                animal.Name,
                "Cancelled",
                "Calculation was cancelled.",
                watch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            watch.Stop();

            return new SimulationResult(
                animal.Name,
                "Error",
                ex.Message,
                watch.ElapsedMilliseconds);
        }
    }
}
