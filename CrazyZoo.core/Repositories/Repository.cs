using CrazyZoo.Core.Models;
using CrazyZoo.Сore.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CrazyZoo.Core.Repositories;

public class Repository<T> where T : Animal
{
    private readonly List<T> items = new();
    private readonly List<string> logs = new();

    public event EventHandler<ZooEventArgs>? AnimalAdded;
    public event EventHandler<ZooEventArgs>? AnimalRemoved;
    public event EventHandler<ZooEventArgs>? CrazyActionPerformed;

    public IEnumerable<string> Logs => logs.AsReadOnly();

    public void Add(T item)
    {
        ArgumentNullException.ThrowIfNull(item);

        if (items.Contains(item))
            return;

        items.Add(item);
        logs.Add($"{item.Name} joined the repository.");

        foreach (T animal in items)
        {
            if (ReferenceEquals(animal, item))
                continue;

            logs.Add(animal.ReactToAdded(item));
        }

        AnimalAdded?.Invoke(this, new ZooEventArgs(item));
    }

    public bool Remove(T item)
    {
        if (!items.Contains(item))
            return false;

        foreach (T animal in items)
        {
            if (ReferenceEquals(animal, item))
                continue;

            logs.Add(animal.ReactToRemoved(item));
        }

        items.Remove(item);
        logs.Add($"{item.Name} left the repository.");

        AnimalRemoved?.Invoke(this, new ZooEventArgs(item));
        return true;
    }

    public string PerformCrazyAction(T item)
    {
        if (!items.Contains(item))
            return "Animal not found.";

        string result = item.CrazyAction();
        logs.Add(result);

        foreach (T animal in items)
        {
            if (ReferenceEquals(animal, item))
                continue;

            logs.Add(animal.ReactToCrazyAction(item));
        }

        CrazyActionPerformed?.Invoke(
            this, new ZooEventArgs(item));

        return result;
    }

    public IEnumerable<T> GetAll()
    {
        return items.ToList();
    }

    public IEnumerable<T> Find(Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return items.Where(predicate).ToList();
    }

    public IEnumerable<T> FindByName(string name)
    {
        return items
            .Where(a => a.Name.Contains(
                name, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public IEnumerable<T> SortByEnergy()
    {
        return items
            .OrderByDescending(a => a.Energy)
            .ToList();
    }

    public IEnumerable<IGrouping<string, T>> GroupBySpecies()
    {
        return items
            .GroupBy(a => a.Species)
            .ToList();
    }

}
