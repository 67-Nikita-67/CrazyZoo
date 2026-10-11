using CrazyZoo.Core.Interfaces;

namespace CrazyZoo.Core.Models;

public class Elephant : Animal, ISwimmable
{
    public Elephant(string name)
        : base(name, 95, 75)
    {
    }

    public override string Species => "Elephant";
    public override string Emoji => "🐘";

    public override string MakeSound()
    {
        return $"{Name} says: TRUUUUMP!";
    }

    public string Swim()
    {
        if (!TrySpendEnergy(15))
            return $"{Name} is too tired to swim!";

        ChangeHappiness(10);

        return $"{Name} swims and sprays water with its trunk!";
    }

    public override string CrazyAction()
    {
        if (!TrySpendEnergy(25))
            return $"{Name} is too tired for a water show!";

        ChangeHappiness(20);

        return $"{Name} sprays water everywhere! ELEPHANT WATER PARTY!";
    }
}