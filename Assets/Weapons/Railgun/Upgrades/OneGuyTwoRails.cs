using System.Collections.Generic;

public class OneGuyTwoRails : Upgrade
{
    public override string Name => "Two rails one guy";
    public override string Description => "Another railgun";
    public override int Rarity => 1;
    public override int MaxApplied => 1;
    public override List<Upgrade> Requires => new List<Upgrade>() { new UnlockRailGun() };

    protected override void ApplyUpgrade()
    {
        PlayerController.Instance.GetComponent<RailGunManager>().ToggleDualRailing();
    }
}