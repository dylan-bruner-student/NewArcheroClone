public class KnifeSkills : Upgrade
{
    public override string Name => "Knife Skills";
    public override string Description => "Decrease the delay between throws by 10%";
    public override int Rarity => 1;
    public override int MaxApplied => 5;

    protected override void ApplyUpgrade()
    {
        ThrowingKnifeManager.Instance.SpawnDelay *= 0.9f;
    }
}