public class CritChance : Upgrade
{
    public override string Name => "Crit Chance";
    public override string Description => "+10% Crit Chance";
    public override int Rarity => 1;
    public override int MaxApplied => 4;

    protected override void ApplyUpgrade()
    {
        PlayerController.Instance.CritChance += 0.10f;
    }
}