namespace CrazyZoo.Core.Models;

public class Kangaroo : Animal
{
    public Kangaroo(string name) : base(name, 95, 80)
    {
    }

    public override string Species => "Kangaroo";
    public override string Emoji => "🦘";

    public override string MakeSound()
    {
        return $"{Name} makes a clicking sound!";
    }

    public string Jump()
    {
        if (!TrySpendEnergy(10))
            return $"{Name} is too tired to jump!";

        ChangeHappiness(10);

        return $"{Name} jumps over a huge rock!";
    }

    public override string CrazyAction()
    {
        if (!TrySpendEnergy(25))
            return $"{Name} is too tired for a crazy jump!";

        ChangeHappiness(20);

        return $"{Name} makes an incredible triple backflip!";
    }
}
