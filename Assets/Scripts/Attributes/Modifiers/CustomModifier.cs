using System;

public class CustomModifier : AttributeModifier
{
    private Func<double, double> Modifier;

    public CustomModifier(Func<double, double> modifier)
    {
        Modifier = modifier;
    }

    public override double Apply(double value)
    {
        return Modifier(value);
    }
}