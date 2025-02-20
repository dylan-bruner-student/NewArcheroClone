public class FasterRegen : Upgrade
{
    public override string Name => "Faster shield regen";
    public override string Description => "";
    public override int Rarity => 1;
    public override int MaxApplied => 5;

    protected override void ApplyUpgrade()
    {
        PlayerController.Instance.ShieldRegenSpeed += 1;
    }
}