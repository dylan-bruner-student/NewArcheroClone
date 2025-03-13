using System.Collections.Generic;

public class IncreaseShurikenDamage : Upgrade
{
    public override string Name => "+1 chain";
    public override string Description => "Increased shuriken chain length";
    public override int Rarity => 1;
    public override int MaxApplied => 5;

    public override List<Upgrade> Requires => new List<Upgrade>() { new UnlockShuriken() };

    protected override void ApplyUpgrade()
    {
        NinjaStarManager.Instance.damage += 10;
    }
}