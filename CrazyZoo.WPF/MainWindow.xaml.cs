using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CrazyZoo.WPF.ViewModels;
using CrazyZoo.WPF.Views;

namespace CrazyZoo.WPF;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        var viewModel = new MainViewModel();
        DataContext = viewModel;

        Loaded += (sender, e) =>
        {
            TabControl? tabs = FindTabControl(this);

            if (tabs == null)
                return;

            foreach (var item in tabs.Items)
            {
                if (item is TabItem tab &&
                    tab.Header?.ToString() == "CPU Simulation")
                    return;
            }

            var simulationTab = new TabItem
            {
                Header = "CPU Simulation",
                Content = new SimulationView(viewModel)
            };

            tabs.Items.Add(simulationTab);
        };
    }

    private static TabControl? FindTabControl(
        DependencyObject parent)
    {
        if (parent is TabControl control)
            return control;

        for (int i = 0;
             i < VisualTreeHelper.GetChildrenCount(parent);
             i++)
        {
            DependencyObject child =
                VisualTreeHelper.GetChild(parent, i);

            TabControl? result = FindTabControl(child);

            if (result != null)
                return result;
        }

        return null;
    }
}
