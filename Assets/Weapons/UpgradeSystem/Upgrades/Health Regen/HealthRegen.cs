public class HealthRegen : Upgrade
{
    public override string Name => "1% health per kill";
    public override string Description => "";
    public override int Rarity => 1;
    public override int MaxApplied => 5;

    protected override void ApplyUpgrade()
    {
        PlayerController.Instance.RegenPerKill += 0.01f;
    }
}