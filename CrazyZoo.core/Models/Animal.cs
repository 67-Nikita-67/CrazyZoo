using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CrazyZoo.Core.Models;

public abstract class Animal : INotifyPropertyChanged
{
    private string _name;
    private int _energy;
    private int _happiness;

    public event PropertyChangedEventHandler? PropertyChanged;

    protected Animal(string name, int energy, int happiness)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Trim().Length > 20)
            throw new ArgumentException("Name must contain 1-20 characters.");

        if (energy < 0 || energy > 100)
            throw new ArgumentException("Energy must be between 0 and 100.");

        if (happiness < 0 || happiness > 100)
            throw new ArgumentException("Happiness must be between 0 and 100.");

        _name = name.Trim();
        _energy = energy;
        _happiness = happiness;
    }

    public string Name
    {
        get => _name;
        private set
        {
            _name = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(DisplayName));
        }
    }

    public int Energy
    {
        get => _energy;
        private set
        {
            _energy = Math.Clamp(value, 0, 100);
            OnPropertyChanged();
        }
    }

    public int Happiness
    {
        get => _happiness;
        private set
        {
            _happiness = Math.Clamp(value, 0, 100);
            OnPropertyChanged();
        }
    }

    public abstract string Species { get; }
    public abstract string Emoji { get; }

    public string DisplayName => $"{Emoji} {Name} ({Species})";

    protected bool TrySpendEnergy(int amount)
    {
        if (amount < 0 || Energy < amount)
            return false;

        Energy -= amount;
        return true;
    }

    protected void ChangeHappiness(int amount)
    {
        Happiness += amount;
    }

    public bool Rename(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            return false;

        if (newName.Trim().Length > 20)
            return false;

        Name = newName.Trim();
        return true;
    }

    public virtual string Eat()
    {
        Energy += 15;
        Happiness += 5;

        return $"{Name} ate some food! Energy: {Energy}.";
    }

    public virtual string Rest()
    {
        Energy += 25;

        return $"{Name} had a nice rest! Energy: {Energy}.";
    }

    public abstract string MakeSound();

    public abstract string CrazyAction();
    public virtual string ReactToAdded(Animal newcomer)
    {
        ChangeHappiness(5);
        return $"{Name} welcome {newcomer.Name}! Happiness: {Happiness}";
    }

    public virtual string ReactToRemoved(Animal leaving)
    {
        ChangeHappiness(-3);
        return $"{Name} says goodbye to {leaving.Name}! Happiness: {Happiness}";
    }

    public virtual string ReactToCrazyAction(Animal actor)
    {
        ChangeHappiness(2);
        return $"{Name} reacts to {actor.Name} s crazy action! Happiness: {Happiness}";
    }


    protected void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }
}