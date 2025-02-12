public class AddBall : Upgrade
{
    public override string Name => "Another Ball";
    public override string Description => "";
    public override int Rarity => 1;

    public override void ApplyUpgrade()
    {
        MightyBallsManager.Instance.BallCount += 1;
        MightyBallsManager.Instance.Refresh();
    }
}