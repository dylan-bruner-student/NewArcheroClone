public abstract class Upgrade
{
    public abstract string Name { get;  }
    public abstract string Description { get;  }
    public abstract int Rarity { get; }

    public abstract void ApplyUpgrade();
}