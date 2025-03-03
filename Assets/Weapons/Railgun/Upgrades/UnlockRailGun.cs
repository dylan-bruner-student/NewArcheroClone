public class UnlockRailGun : Upgrade
{
    public override string Name => "RailGun!";
    public override string Description => "Unlock the RailGun";
    public override int Rarity => 1;
    public override int MaxApplied => 1;

    protected override void ApplyUpgrade()
    {
        PlayerController.Instance.GetComponent<RailGunManager>().enabled = true;
    }
}