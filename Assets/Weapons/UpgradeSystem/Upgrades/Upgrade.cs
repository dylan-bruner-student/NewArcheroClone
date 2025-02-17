using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.EventSystems;

public abstract class Upgrade
{
    public abstract string Name { get;  }
    public abstract string Description { get;  }
    public abstract int Rarity { get; }
    public virtual int MaxApplied { get; } = 1;
    public virtual List<Upgrade> Requires { get; } = new List<Upgrade>();

    public virtual void SystemApplyUpgrade()
    {
        UpgradeController.Instance.AppliedUpgrades.Add(this);
        ApplyUpgrade();
    }

    protected abstract void ApplyUpgrade();
}