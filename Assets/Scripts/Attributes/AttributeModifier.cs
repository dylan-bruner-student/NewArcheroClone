using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributeModifier
{

    public virtual double Apply(double value) { return value; }
}