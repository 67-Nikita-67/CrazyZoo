using System;

namespace CrazyZoo.Core.Models;

public class Giraffe : Animal
{
    public Giraffe(string name) : base(name, 85, 75)
    {
    }

    public override string Species => "Giraffe";
    public override string Emoji => "🦒";

    public override string MakeSound()
    {
        return $"{Name} makes a gentle humming sound!";
    }

    public override string CrazyAction()
    {
        if (!TrySpendEnergy(15))
            return $"{Name} is too tired to reach the trees!";

        ChangeHappiness(20);

        return $"{Name} reaches the tallest tree and eats magical leaves!";
    }
}
