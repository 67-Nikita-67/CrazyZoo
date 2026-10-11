using CrazyZoo.Core.Interfaces;

namespace CrazyZoo.Core.Models;

public class Penguin : Animal, ISwimmable
{
    public Penguin(string name)
        : base(name, 90, 60)
    {
    }

    public override string Species => "Penguin";
    public override string Emoji => "🐧";

    public override string MakeSound()
    {
        return $"{Name} says: Honk honk!";
    }

    public string Swim()
    {
        if (!TrySpendEnergy(10))
            return $"{Name} is too tired to swim!";

        ChangeHappiness(10);
        return $"{Name} swims like a champion!";
    }

    public override string CrazyAction()
    {
        if (!TrySpendEnergy(15))
            return $"{Name} is too tired to dance!";

        ChangeHappiness(20);
        return $"{Name} starts a crazy ice disco!";
    }
}