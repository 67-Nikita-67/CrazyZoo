using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

using CrazyZoo.Core.Interfaces;
using CrazyZoo.Core.Models;

namespace CrazyZoo.WPF;

public partial class MainWindow : Window
{
    private readonly ObservableCollection<Animal> _animals = new();

    private readonly ObservableCollection<string> _logs = new();

    public MainWindow()
    {
        InitializeComponent();

        AnimalList.ItemsSource = _animals;
        LogList.ItemsSource = _logs;

        _animals.Add(new Monkey("Bobby"));
        _animals.Add(new Penguin("Pingu"));
        _animals.Add(new Lion("Simba"));
        _animals.Add(new Parrot("Kesha"));

        AnimalList.SelectedIndex = 0;

        AddLog("Welcome to Crazy Zoo!");
    }

    private Animal? SelectedAnimal =>
        AnimalList.SelectedItem as Animal;

    private void AddLog(string message)
    {
        _logs.Add($"[{DateTime.Now:HH:mm:ss}] {message}");

        if (_logs.Count > 100)
            _logs.RemoveAt(0);

        LogList.ScrollIntoView(_logs[^1]);
    }

    private void RunAction(Func<Animal, string> action)
    {
        Animal? animal = SelectedAnimal;

        if (animal == null)
        {
            AddLog("Please select an animal first!");
            return;
        }

        AddLog(action(animal));
    }

    private void AnimalList_SelectionChanged(
        object sender, SelectionChangedEventArgs e)
    {
        if (SelectedAnimal != null)
            AddLog($"Selected: {SelectedAnimal.Name}");
    }

    private void Eat_Click(object sender, RoutedEventArgs e)
    {
        RunAction(animal => animal.Eat());
    }

    private void Rest_Click(object sender, RoutedEventArgs e)
    {
        RunAction(animal => animal.Rest());
    }

    private void Sound_Click(object sender, RoutedEventArgs e)
    {
        RunAction(animal => animal.MakeSound());
    }

    private void CrazyAction_Click(object sender, RoutedEventArgs e)
    {
        RunAction(animal => animal.CrazyAction());
    }

    private void Swim_Click(object sender, RoutedEventArgs e)
    {
        RunAction(animal =>
        {
            if (animal is ISwimmable swimmer)
                return swimmer.Swim();

            return $"{animal.Name} cannot swim!";
        });
    }

    private void Climb_Click(object sender, RoutedEventArgs e)
    {
        RunAction(animal =>
        {
            if (animal is IClimb climber)
                return climber.Climb();

            return $"{animal.Name} cannot climb!";
        });
    }

    private void Fly_Click(object sender, RoutedEventArgs e)
    {
        RunAction(animal =>
        {
            if (animal is IFly flyer)
                return flyer.Fly();

            return $"{animal.Name} cannot fly!";
        });
    }

    private void Talk_Click(object sender, RoutedEventArgs e)
    {
        RunAction(animal =>
        {
            if (animal is ITalk talker)
                return talker.Talk();

            return $"{animal.Name} cannot talk!";
        });
    }

    private void AddAnimal_Click(object sender, RoutedEventArgs e)
    {
        string name = NewAnimalName.Text.Trim();

        if (string.IsNullOrWhiteSpace(name))
        {
            AddLog("Error: Animal name cannot be empty!");
            return;
        }

        if (name.Length > 20)
        {
            AddLog("Error: Name is too long!");
            return;
        }

        if (SpeciesCombo.SelectedItem is not ComboBoxItem item)
        {
            AddLog("Please choose an animal species!");
            return;
        }

        string species = item.Content.ToString() ?? "";

        Animal animal;

        switch (species)
        {
            case "Monkey":
                animal = new Monkey(name);
                break;

            case "Penguin":
                animal = new Penguin(name);
                break;

            case "Lion":
                animal = new Lion(name);
                break;

            case "Parrot":
                animal = new Parrot(name);
                break;

            default:
                AddLog("Unknown animal species!");
                return;
        }

        _animals.Add(animal);
        AnimalList.SelectedItem = animal;

        NewAnimalName.Clear();

        AddLog($"New animal added: {animal.DisplayName}");
    }

    private void Rename_Click(object sender, RoutedEventArgs e)
    {
        Animal? animal = SelectedAnimal;

        if (animal == null)
        {
            AddLog("Please select an animal!");
            return;
        }

        string oldName = animal.Name;

        if (!animal.Rename(RenameTextBox.Text))
        {
            AddLog("Invalid name! Use 1-20 characters.");
            return;
        }

        RenameTextBox.Clear();

        AddLog($"{oldName} was renamed to {animal.Name}.");
    }

    private void ClearLog_Click(object sender, RoutedEventArgs e)
    {
        _logs.Clear();
        AddLog("Event log cleared.");
    }
}