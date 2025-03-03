using System.Collections.Generic;

public class MightyBalls : Upgrade
{
    public override string Name => "Mighty Balls";
    public override string Description => "Rotating balls that deal MASSIVE damage!";
    public override int Rarity => 1;

    protected override void ApplyUpgrade()
    {
        MightyBallsManager.Instance.BallCount = 1;
        MightyBallsManager.Instance.Refresh();
    }
}