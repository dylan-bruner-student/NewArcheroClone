using System.Collections.Generic;

public class HeavyBalls : Upgrade
{
    public override string Name => "Heavy Balls";
    public override string Description => "+1 attack damage";
    public override int Rarity => 1;
    public override int MaxApplied => 5;
    public override List<Upgrade> Requires => new List<Upgrade> { new MightyBalls() };

    protected override void ApplyUpgrade()
    {
        MightyBallsManager.Instance.BallDamage += 1;
        MightyBallsManager.Instance.Refresh();
    }
}