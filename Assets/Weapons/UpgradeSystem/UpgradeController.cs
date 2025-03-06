
using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeController : MonoBehaviour
{
    public static UpgradeController Instance;


    [SerializeField] private GameObject UpgradeMenu;
    [SerializeField] private Text Option1;
    [SerializeField] private Text Option2;
    [SerializeField] private Text Option3;

    public Action Callback_Option1 = null;
    public Action Callback_Option2 = null;
    public Action Callback_Option3 = null;

    public static List<Upgrade> Upgrades = new List<Upgrade> { 
        new MightyBalls(), new AddBall(), new HeavyBalls(), new Incinerator(),
        new KnifeSkills(), new LongerKnifes(),
        new UnlockRailGun(), new RailGunSpeedIncrease(),
        new DoublePickupRadius(),
        new StaminaUpgrade(),
        new CritChance(),
        new HealthRegen(),
    };

    public List<Upgrade> AppliedUpgrades = new List<Upgrade>();

    private void Awake()
    {

    }

    private void Start()
    {
        Instance = this;
        UpgradeMenu.SetActive(false);
    }


    public void PromptRandomUpgrades()
    {
        var theUpgrade = Upgrades
            .Where(upgrade => CheckCriteria(upgrade)) // Make sure the precursor upgrades are met
            .Where(upgrade => AppliedUpgrades.Count(u => u.GetType() == upgrade.GetType()) < upgrade.MaxApplied) // Make sure there isn't too many applied
            .OrderBy(upgrade => Guid.NewGuid()) // Randomize the list
            .Take(3)
            .ToList();

        PromptUpgrade(theUpgrade[0], theUpgrade[1], theUpgrade[2]);
    }


    private void PromptUpgrade(Upgrade u1, Upgrade u2, Upgrade u3)
    {
        Callback_Option1 = u1.SystemApplyUpgrade;
        Callback_Option2 = u2.SystemApplyUpgrade;
        Callback_Option3 = u3.SystemApplyUpgrade;


        Option1.text = u1.Name;
        Option2.text = u2.Name;
        Option3.text = u3.Name;
        
        TimeSystem.Pause();
        UpgradeMenu.SetActive(true);
    }

    public void OnPickOption(int index) 
    {
        Debug.Log($"Selected option: {index}");
        UpgradeMenu.SetActive(false);
        TimeSystem.Resume();

        switch (index)
        {
            case 0:
                Callback_Option1(); break;
            case 1:
                Callback_Option2(); break;
            case 2:
                Callback_Option3(); break;
        }
    }

    private bool CheckCriteria(Upgrade upgrade)
    {
        foreach (var u in upgrade.Requires)
        {
            bool found = false;

            foreach (var v in AppliedUpgrades)
                if (v.GetType() == u.GetType())
                    found = true;

            if (!found)
                return false;
        }

        return true;
    }

    public bool HasUpgrade(Type upgrade)
    {
        foreach (var u in AppliedUpgrades)
            if (u.GetType() == upgrade) return true;
        return false;
    }
}