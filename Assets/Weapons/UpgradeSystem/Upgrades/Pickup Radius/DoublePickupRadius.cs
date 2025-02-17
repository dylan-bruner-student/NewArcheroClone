public class DoublePickupRadius : Upgrade
{
    public override string Name => "2x pickup radius";
    public override string Description => "";
    public override int Rarity => 1;
    public override int MaxApplied => 5;

    protected override void ApplyUpgrade()
    {
        PlayerController.Instance.SetPickupRadius(PlayerController.Instance.m_PickupRadius * 2);
    }
}