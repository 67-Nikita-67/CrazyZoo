using CrazyZoo.Core.Interfaces;

namespace CrazyZoo.Core.Models;

public class Monkey : Animal, ISwimmable, IClimb
{
    public Monkey(string name)
        : base(name, 80, 70)
    {
    }

    public override string Species => "Monkey";
    public override string Emoji => "🐒";

    public override string MakeSound()
    {
        return $"{Name} says: Ooh-ooh-ah-ah!";
    }

    public string Swim()
    {
        if (!TrySpendEnergy(10))
            return $"{Name} is too tired to swim!";

        ChangeHappiness(5);
        return $"{Name} swims in the river!";
    }

    public string Climb()
    {
        if (!TrySpendEnergy(15))
            return $"{Name} is too tired to climb!";

        ChangeHappiness(10);
        return $"{Name} climbs a tall tree!";
    }

    public override string CrazyAction()
    {
        if (!TrySpendEnergy(20))
            return $"{Name} has no energy for banana madness!";

        ChangeHappiness(15);
        return $"{Name} throws bananas everywhere! BANANA PARTY!";
    }
}