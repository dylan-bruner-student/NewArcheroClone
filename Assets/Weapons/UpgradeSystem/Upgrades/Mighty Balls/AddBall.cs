using System.Collections.Generic;

public class AddBall : Upgrade
{
    public override string Name => "Another Ball";
    public override string Description => "";
    public override int Rarity => 1;
    public override int MaxApplied => 7;
    public override List<Upgrade> Requires => new List<Upgrade> { new MightyBalls() };

    protected override void ApplyUpgrade()
    {
        MightyBallsManager.Instance.BallCount += 1;
        MightyBallsManager.Instance.Refresh();
    }
}