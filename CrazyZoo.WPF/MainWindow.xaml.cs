using System.Windows;
using CrazyZoo.WPF.ViewModels;

namespace CrazyZoo.WPF;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }
}
