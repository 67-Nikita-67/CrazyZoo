using System;
using System.Collections.Generic;
using CrazyZoo.Core.Models;

namespace CrazyZoo.Core.Interfaces;

public interface IRepository<T> where T : Animal
{
    void Add(T item);
    bool Remove(T item);
    IEnumerable<T> GetAll();
    IEnumerable<T> Find(Func<T, bool> predicate);
}
