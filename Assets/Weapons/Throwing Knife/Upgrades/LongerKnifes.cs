public class LongerKnifes : Upgrade
{
    public override string Name => "Longer Knifes";
    public override string Description => "+5 attack range";
    public override int Rarity => 1;
    public override int MaxApplied => 5;

    protected override void ApplyUpgrade()
    {
        ThrowingKnifeManager.Instance.AttackRange += 5;
    }
}