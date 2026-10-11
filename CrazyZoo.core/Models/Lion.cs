using CrazyZoo.Core.Interfaces;

namespace CrazyZoo.Core.Models;

public class Lion : Animal, IClimb
{
    public Lion(string name)
        : base(name, 75, 50)
    {
    }

    public override string Species => "Lion";
    public override string Emoji => "🦁";

    public override string MakeSound()
    {
        return $"{Name} says: RRAAAAR!";
    }

    public string Climb()
    {
        if (!TrySpendEnergy(20))
            return $"{Name} is too tired to climb!";

        ChangeHappiness(5);
        return $"{Name} climbs onto a big rock!";
    }

    public override string CrazyAction()
    {
        if (!TrySpendEnergy(25))
            return $"{Name} is too tired for the royal roar!";

        ChangeHappiness(15);
        return $"{Name} roars so loudly that the zoo shakes!";
    }
}