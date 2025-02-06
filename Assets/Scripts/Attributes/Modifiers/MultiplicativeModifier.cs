public class MultiplicativeModifier : AttributeModifier
{
    private double Modifier;

    public MultiplicativeModifier(double modifier)
    {
        Modifier = modifier;
    }

    public override double Apply(double value)
    {
        return value * Modifier;
    }
}