using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CrazyZoo.Core.Interfaces;
using CrazyZoo.Core.Models;
using CrazyZoo.Core.Repositories;
using CrazyZoo.WPF.Commands;

namespace CrazyZoo.WPF.ViewModels;

public class MainViewModel : BaseViewModel
{
    private readonly Repository<Animal> _zooA = new();
    private readonly Repository<Animal> _zooB = new();
    private readonly Repository<Animal> _zooC = new();

    public ObservableCollection<Animal> Animals { get; } = new();
    public ObservableCollection<string> Logs { get; } = new();
    public ObservableCollection<string> LinqResults { get; } = new();

    public ObservableCollection<string> RepositoryNames { get; } =
        new() { "Zoo A", "Zoo B", "Zoo C" };

    private string _selectedRepositoryName = "Zoo A";

    public string SelectedRepositoryName
    {
        get => _selectedRepositoryName;
        set
        {
            if (SetProperty(ref _selectedRepositoryName, value))
            {
                LoadSelectedRepository();
            }
        }
    }

    private Repository<Animal> CurrentRepository =>
        SelectedRepositoryName switch
        {
            "Zoo B" => _zooB,
            "Zoo C" => _zooC,
            _ => _zooA
        };

    private Animal? _selectedAnimal;

    public Animal? SelectedAnimal
    {
        get => _selectedAnimal;
        set
        {
            if (SetProperty(ref _selectedAnimal, value))
                RefreshCommands();
        }
    }

    private string _newAnimalName = "";

    public string NewAnimalName
    {
        get => _newAnimalName;
        set => SetProperty(ref _newAnimalName, value);
    }

    private string _renameText = "";

    public string RenameText
    {
        get => _renameText;
        set => SetProperty(ref _renameText, value);
    }

    private string _selectedSpecies = "Monkey";

    public string SelectedSpecies
    {
        get => _selectedSpecies;
        set => SetProperty(ref _selectedSpecies, value);
    }

    private string _searchText = "";

    public string SearchText
    {
        get => _searchText;
        set => SetProperty(ref _searchText, value);
    }
    public ICommand AddCommand { get; }
    public ICommand RemoveCommand { get; }
    public ICommand EatCommand { get; }
    public ICommand RestCommand { get; }
    public ICommand SoundCommand { get; }
    public ICommand CrazyCommand { get; }
    public ICommand SwimCommand { get; }
    public ICommand ClimbCommand { get; }
    public ICommand FlyCommand { get; }
    public ICommand TalkCommand { get; }
    public ICommand RenameCommand { get; }
    public ICommand ClearLogCommand { get; }

    public ICommand SearchCommand { get; }
    public ICommand SortCommand { get; }
    public ICommand GroupCommand { get; }

    public MainViewModel()
    {
        SearchCommand = new RelayCommand(_ => SearchAnimals());
        SortCommand = new RelayCommand(_ => SortAnimals());
        GroupCommand = new RelayCommand(_ => GroupAnimals());

        AddCommand = new RelayCommand(_ => AddAnimal());

        RemoveCommand = new RelayCommand(
            _ => RemoveAnimal(),
            _ => SelectedAnimal != null);

        EatCommand = CreateAnimalCommand(a => a.Eat());
        RestCommand = CreateAnimalCommand(a => a.Rest());
        SoundCommand = CreateAnimalCommand(a => a.MakeSound());

        CrazyCommand = new RelayCommand(
            _ => PerformCrazyAction(),
            _ => SelectedAnimal != null);

        SwimCommand = CreateAnimalCommand(a =>
            a is ISwimmable s
                ? s.Swim()
                : $"{a.Name} cannot swim!");

        ClimbCommand = CreateAnimalCommand(a =>
            a is IClimb c
                ? c.Climb()
                : $"{a.Name} cannot climb!");

        FlyCommand = CreateAnimalCommand(a =>
            a is IFly f
                ? f.Fly()
                : $"{a.Name} cannot fly!");

        TalkCommand = CreateAnimalCommand(a =>
            a is ITalk t
                ? t.Talk()
                : $"{a.Name} cannot talk!");

        RenameCommand = new RelayCommand(
            _ => RenameAnimal(),
            _ => SelectedAnimal != null);

        ClearLogCommand = new RelayCommand(_ =>
        {
            Logs.Clear();
            AddLog("Event log cleared.");
        });

        AddInitialAnimal(new Monkey("Bobby"));
        AddInitialAnimal(new Penguin("Pingu"));
        AddInitialAnimal(new Lion("Simba"));
        AddInitialAnimal(new Parrot("Kesha"));
        AddInitialAnimal(new Elephant("Dumbo"));

        SelectedAnimal = Animals.FirstOrDefault();

        AddLog("Welcome to Crazy Zoo!");
    }

    private void AddInitialAnimal(Animal animal)
    {
        _zooA.Add(animal);
        Animals.Add(animal);
    }

    private void LoadSelectedRepository()
    {
        Animals.Clear();

        foreach (Animal animal in CurrentRepository.GetAll())
            Animals.Add(animal);

        SelectedAnimal = Animals.FirstOrDefault();

        LinqResults.Clear();

        AddLog($"Switched to {SelectedRepositoryName}");
    }

    private ICommand CreateAnimalCommand(
        Func<Animal, string> action)
    {
        return new RelayCommand(
            _ =>
            {
                if (SelectedAnimal != null)
                    AddLog(action(SelectedAnimal));
            },
            _ => SelectedAnimal != null);
    }

    private void AddAnimal()
    {
        try
        {
            Animal animal = SelectedSpecies switch
            {
                "Giraffe" => new Giraffe(NewAnimalName),
                "Crocodile" => new Crocodile(NewAnimalName),
                "Kangaroo" => new Kangaroo(NewAnimalName),
                "Monkey" => new Monkey(NewAnimalName),
                "Penguin" => new Penguin(NewAnimalName),
                "Lion" => new Lion(NewAnimalName),
                "Parrot" => new Parrot(NewAnimalName),
                "Elephant" => new Elephant(NewAnimalName),
                _ => throw new ArgumentException(
                    "Unknown species!")
            };

            int previousLogCount = CurrentRepository.Logs.Count();

            CurrentRepository.Add(animal);
            Animals.Add(animal);

            CopyRepositoryLogs(previousLogCount);

            SelectedAnimal = animal;
            NewAnimalName = "";
        }
        catch (ArgumentException ex)
        {
            AddLog($"Error: {ex.Message}");
        }
    }
    private void RemoveAnimal()
    {
        if (SelectedAnimal == null)
            return;

        Animal animal = SelectedAnimal;

        int previousLogCount = CurrentRepository.Logs.Count();

        if (CurrentRepository.Remove(animal))
        {
            Animals.Remove(animal);

            CopyRepositoryLogs(previousLogCount);

            SelectedAnimal = Animals.FirstOrDefault();
        }
    }

    private void PerformCrazyAction()
    {
        if (SelectedAnimal == null)
            return;

        int previousLogCount = CurrentRepository.Logs.Count();

        CurrentRepository.PerformCrazyAction(SelectedAnimal);

        CopyRepositoryLogs(previousLogCount);
    }

    private void RenameAnimal()
    {
        if (SelectedAnimal == null)
            return;

        string oldName = SelectedAnimal.Name;

        if (SelectedAnimal.Rename(RenameText))
        {
            AddLog(
                $"{oldName} was renamed to {SelectedAnimal.Name}.");

            RenameText = "";
        }
        else
        {
            AddLog("Invalid name! Use 1-20 characters.");
        }
    }

    private void CopyRepositoryLogs(int startIndex)
    {
        foreach (string message in
                 CurrentRepository.Logs.Skip(startIndex))
        {
            AddLog(message);
        }
    }

    private void AddLog(string message)
    {
        Logs.Add($"[{DateTime.Now:HH:mm:ss}] {message}");

        if (Logs.Count > 100)
            Logs.RemoveAt(0);
    }

    private void RefreshCommands()
    {
        foreach (ICommand command in new[]
        {
            RemoveCommand,
            EatCommand,
            RestCommand,
            SoundCommand,
            CrazyCommand,
            SwimCommand,
            ClimbCommand,
            FlyCommand,
            TalkCommand,
            RenameCommand
        })
        {
            if (command is RelayCommand relay)
                relay.RaiseCanExecuteChanged();
        }
    }
    private void SearchAnimals()
    {
        LinqResults.Clear();

        var animals = CurrentRepository.FindByName(SearchText);

        foreach (var animal in animals)
        {
            LinqResults.Add(
                $"{animal.DisplayName} | Energy: {animal.Energy}");
        }

        if (LinqResults.Count == 0)
            LinqResults.Add("No animals found.");
    }
    private void SortAnimals()
    {
        LinqResults.Clear();

        var animals = CurrentRepository.SortByEnergy();

        foreach (var animal in animals)
        {
            LinqResults.Add(
                $"{animal.DisplayName} | Energy: {animal.Energy}");
        }
    }
    private void GroupAnimals()
    {
        LinqResults.Clear();

        var groups = CurrentRepository.GroupBySpecies();

        foreach (var group in groups)
        {
            LinqResults.Add(
                $"{group.Key} ({group.Count()}):");

            foreach (var animal in group)
            {
                LinqResults.Add(
                    $"    {animal.DisplayName}");
            }
        }
    }
}
