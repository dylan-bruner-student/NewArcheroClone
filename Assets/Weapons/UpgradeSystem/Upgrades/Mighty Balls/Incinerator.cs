using System.Collections.Generic;

public class Incinerator : Upgrade
{
    public override string Name => "Incinerator";
    public override string Description => "Allows your balls to hit Cacti";
    public override int Rarity => 1;
    public override int MaxApplied => 1;
    public override List<Upgrade> Requires => new List<Upgrade> { new MightyBalls() };

    protected override void ApplyUpgrade()
    {
        MightyBallsManager.Instance.Refresh();
    }
}