using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CrazyZoo.WPF.Services;
using CrazyZoo.WPF.ViewModels;

namespace CrazyZoo.WPF.Views;

public partial class SimulationView : UserControl
{
    private readonly MainViewModel _mainViewModel;

    private readonly List<AnimalOption> _options = new();

    private CancellationTokenSource? _cancellation;
    private bool _isRunning;

    public SimulationView(MainViewModel mainViewModel)
    {
        InitializeComponent();

        _mainViewModel = mainViewModel;
        RefreshAnimals();
    }

    private void RefreshAnimals()
    {
        _options.Clear();

        foreach (var animal in _mainViewModel.Animals)
        {
            _options.Add(new AnimalOption
            {
                Name = animal.DisplayName,
                Energy = animal.Energy,
                Happiness = animal.Happiness,
                IsSelected = true
            });
        }

        AnimalList.ItemsSource = null;
        AnimalList.ItemsSource = _options;
    }

    private void Refresh_Click(
        object sender, RoutedEventArgs e)
    {
        RefreshAnimals();
        StatusText.Text = "Animal list refreshed.";
    }

    private void Sequential_Click(
        object sender, RoutedEventArgs e)
    {
        _ = StartAsync("Sequential");
    }

    private void Parallel_Click(
        object sender, RoutedEventArgs e)
    {
        _ = StartAsync("Parallel");
    }

    private void Compare_Click(
        object sender, RoutedEventArgs e)
    {
        _ = StartAsync("Compare");
    }

    private void Cancel_Click(
        object sender, RoutedEventArgs e)
    {
        _cancellation?.Cancel();
        StatusText.Text = "Cancellation requested...";
    }

    private async Task StartAsync(string mode)
    {
        if (_isRunning)
            return;

        var selected = _options
            .Where(a => a.IsSelected)
            .Select(a => new AnimalSnapshot(
                a.Name,
                a.Energy,
                a.Happiness))
            .ToList();

        if (selected.Count < 3)
        {
            MessageBox.Show(
                "Please select at least 3 animals.");
            return;
        }

        if (!int.TryParse(
                IterationsBox.Text, out int iterations)
            || iterations <= 0)
        {
            MessageBox.Show(
                "Enter a positive number of iterations.");
            return;
        }

        bool forceError = ErrorCheckBox.IsChecked == true;

        _isRunning = true;
        SetButtons(false);

        ReportList.Items.Clear();
        TotalTimeText.Text = "Calculating...";

        _cancellation = new CancellationTokenSource();
        CancellationToken token = _cancellation.Token;

        try
        {
            if (mode == "Compare")
            {
                long sequential = await RunModeAsync(
                    "Sequential",
                    selected,
                    iterations,
                    token,
                    forceError);

                if (!token.IsCancellationRequested)
                {
                    long parallel = await RunModeAsync(
                        "Parallel",
                        selected,
                        iterations,
                        token,
                        forceError);

                    double speedup = parallel > 0
                        ? (double)sequential / parallel
                        : 0;

                    TotalTimeText.Text =
                        $"Sequential: {sequential} ms | " +
                        $"Parallel: {parallel} ms | " +
                        $"Speedup: {speedup:F2}x";

                    ReportList.Items.Add(
                        $"COMPARISON: {speedup:F2}x speedup");
                }
                else
                {
                    TotalTimeText.Text =
                        "Comparison was cancelled.";
                }
            }
            else
            {
                long duration = await RunModeAsync(
                    mode,
                    selected,
                    iterations,
                    token,
                    forceError);

                TotalTimeText.Text =
                    $"Total {mode} time: {duration} ms";
            }

            StatusText.Text = token.IsCancellationRequested
                ? "Cancelled."
                : "Finished.";
        }
        catch (Exception ex)
        {
            ReportList.Items.Add(
                $"Unexpected error: {ex.Message}");

            StatusText.Text = "Error.";
        }
        finally
        {
            _cancellation.Dispose();
            _cancellation = null;

            _isRunning = false;
            SetButtons(true);
        }
    }

    private async Task<long> RunModeAsync(
        string mode,
        List<AnimalSnapshot> animals,
        int iterations,
        CancellationToken token,
        bool forceError)
    {
        ReportList.Items.Add("");
        ReportList.Items.Add($"===== {mode} =====");

        StatusText.Text = $"Running {mode}...";

        var watch = Stopwatch.StartNew();

        if (mode == "Sequential")
        {
            for (int i = 0; i < animals.Count; i++)
            {
                int index = i;

                SimulationResult result = await Task.Run(
                    () => CpuSimulation.Calculate(
                        animals[index],
                        iterations,
                        token,
                        forceError && index == 0));

                AddResult(result);
            }
        }
        else
        {
            var tasks = animals.Select((animal, index) =>
                Task.Run(() => CpuSimulation.Calculate(
                    animal,
                    iterations,
                    token,
                    forceError && index == 0)))
                .ToArray();

            SimulationResult[] results =
                await Task.WhenAll(tasks);

            foreach (SimulationResult result in results)
                AddResult(result);
        }

        watch.Stop();

        ReportList.Items.Add(
            $"TOTAL {mode}: {watch.ElapsedMilliseconds} ms");

        return watch.ElapsedMilliseconds;
    }

    private void AddResult(SimulationResult result)
    {
        ReportList.Items.Add(
            $"{result.Name} | {result.Status} | " +
            $"{result.Details} | {result.DurationMs} ms");

        ReportList.ScrollIntoView(
            ReportList.Items[ReportList.Items.Count - 1]);
    }

    private void SetButtons(bool enabled)
    {
        SequentialButton.IsEnabled = enabled;
        ParallelButton.IsEnabled = enabled;
        CompareButton.IsEnabled = enabled;
        RefreshButtonSafe(enabled);

        CancelButton.IsEnabled = !enabled;
    }

    private void RefreshButtonSafe(bool enabled)
    {
        foreach (var child in FindButtons(this))
        {
            if (child.Content is string text &&
                text == "Refresh Animals")
            {
                child.IsEnabled = enabled;
            }
        }
    }

    private static IEnumerable<Button> FindButtons(
        DependencyObject parent)
    {
        for (int i = 0;
             i < System.Windows.Media.VisualTreeHelper
                 .GetChildrenCount(parent); i++)
        {
            var child =
                System.Windows.Media.VisualTreeHelper
                    .GetChild(parent, i);

            if (child is Button button)
                yield return button;

            foreach (var nested in FindButtons(child))
                yield return nested;
        }
    }
}

public class AnimalOption
{
    public string Name { get; set; } = "";
    public int Energy { get; set; }
    public int Happiness { get; set; }
    public bool IsSelected { get; set; }
}
