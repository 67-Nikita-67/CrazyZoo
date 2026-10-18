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
    private readonly Repository<Animal> _repository = new();

    public ObservableCollection<Animal> Animals { get; } = new();
    public ObservableCollection<string> Logs { get; } = new();
    public ObservableCollection<string> LinqResults { get; } = new();

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
        _repository.Add(animal);
        Animals.Add(animal);
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
                "Monkey" => new Monkey(NewAnimalName),
                "Penguin" => new Penguin(NewAnimalName),
                "Lion" => new Lion(NewAnimalName),
                "Parrot" => new Parrot(NewAnimalName),
                "Elephant" => new Elephant(NewAnimalName),
                _ => throw new ArgumentException(
                    "Unknown species!")
            };

            int previousLogCount = _repository.Logs.Count();

            _repository.Add(animal);
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
        int previousLogCount = _repository.Logs.Count();

        if (_repository.Remove(animal))
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

        int previousLogCount = _repository.Logs.Count();

        _repository.PerformCrazyAction(SelectedAnimal);

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
        foreach (string message in _repository.Logs.Skip(startIndex))
            AddLog(message);
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

        var animals = _repository.FindByName(SearchText);

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

        var animals = _repository.SortByEnergy();

        foreach (var animal in animals)
        {
            LinqResults.Add(
                $"{animal.DisplayName} | Energy: {animal.Energy}");
        }
    }
    private void GroupAnimals()
    {
        LinqResults.Clear();

        var groups = _repository.GroupBySpecies();

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
