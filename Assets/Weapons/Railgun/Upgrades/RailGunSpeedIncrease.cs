using System.Collections.Generic;

public class RailGunSpeedIncrease : Upgrade
{
    public override string Name => "Speed railing";
    public override string Description => "+20% speed";
    public override int Rarity => 1;
    public override int MaxApplied => 5;
    public override List<Upgrade> Requires => new List<Upgrade>() { new UnlockRailGun() };

    protected override void ApplyUpgrade()
    {
        PlayerController.Instance.GetComponent<RailGunManager>().ShotDelay *= 0.8f;
    }
}