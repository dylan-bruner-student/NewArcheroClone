using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdditiveModifier : AttributeModifier
{
    private double Modifier;

    public AdditiveModifier(double modifier) { 
        Modifier = modifier;
    }

    public override double Apply(double value)
    {
        return value + Modifier;
    }
}
