using CrazyZoo.Core.Interfaces;

namespace CrazyZoo.Core.Models;

public class Parrot : Animal, IFly, ITalk
{
    public Parrot(string name)
        : base(name, 85, 80)
    {
    }

    public override string Species => "Parrot";
    public override string Emoji => "🦜";

    public override string MakeSound()
    {
        return $"{Name} says: Squawk! Squawk!";
    }

    public string Fly()
    {
        if (!TrySpendEnergy(20))
            return $"{Name} is too tired to fly!";

        ChangeHappiness(10);
        return $"{Name} flies around the zoo!";
    }

    public string Talk()
    {
        if (!TrySpendEnergy(5))
            return $"{Name} is too tired to talk!";

        ChangeHappiness(5);
        return $"{Name} says: Hello! How are you?";
    }

    public override string CrazyAction()
    {
        if (!TrySpendEnergy(25))
            return $"{Name} is too tired for a crazy show!";

        ChangeHappiness(20);
        return $"{Name} imitates the zookeeper and shouts: EVERYBODY DANCE!";
    }
}