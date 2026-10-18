using System;
using CrazyZoo.Core.Models;

namespace CrazyZoo.Сore.Events
{
    public class ZooEventArgs : EventArgs
    {
        public Animal Actor { get; }
        
        public ZooEventArgs(Animal actor)
        {
            Actor = actor;
        }
    }
}