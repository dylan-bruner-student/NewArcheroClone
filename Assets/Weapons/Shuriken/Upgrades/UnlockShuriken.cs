public class UnlockShuriken : Upgrade
{
    public override string Name => "Shuriken!";
    public override string Description => "Unlock the Shuriken";
    public override int Rarity => 1;
    public override int MaxApplied => 1;

    protected override void ApplyUpgrade()
    {
        PlayerController.Instance.GetComponent<NinjaStarManager>().enabled = true;
    }
}