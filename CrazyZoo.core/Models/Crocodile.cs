using CrazyZoo.Core.Interfaces;

namespace CrazyZoo.Core.Models;

public class Crocodile : Animal, ISwimmable
{
    public Crocodile(string name) : base(name, 90, 60)
    {
    }

    public override string Species => "Crocodile";
    public override string Emoji => "🐊";

    public override string MakeSound()
    {
        return $"{Name} makes a deep growling sound!";
    }

    public string Swim()
    {
        if (!TrySpendEnergy(10))
            return $"{Name} is too tired to swim!";

        ChangeHappiness(10);

        return $"{Name} swims silently through the river!";
    }

    public override string CrazyAction()
    {
        if (!TrySpendEnergy(20))
            return $"{Name} is too tired for a crazy action!";

        ChangeHappiness(15);

        return $"{Name} performs a spectacular crocodile spin!";
    }
}
