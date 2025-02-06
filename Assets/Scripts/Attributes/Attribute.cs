using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attribute
{
    public double Value
    {
        get
        {
            return _Value;
        }
    }
    
    private double _Value = 0;

    private double BaseValue;
    private List<AttributeModifier> modifiers = new List<AttributeModifier>();

    public Attribute(double baseValue)
    {
        this.BaseValue = baseValue;
    }

    public void AddModifier(AttributeModifier modifier) { 
        modifiers.Add(modifier);
        UpdateValue();
    }
    
    public bool RemoveModifier(AttributeModifier modifier) { 
        bool res = modifiers.Remove(modifier);
        UpdateValue();
        return res;
    }
    
    public List<AttributeModifier> GetModifiers() { return modifiers; }

    private void UpdateValue() {
        _Value = BaseValue;
        foreach (AttributeModifier modifier in modifiers)
            _Value = modifier.Apply(_Value);
    }
}