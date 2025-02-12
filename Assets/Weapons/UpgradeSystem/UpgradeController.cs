
using System;
using TMPro;
using UnityEngine;

public class UpgradeController : MonoBehaviour
{
    public static UpgradeController Instance;


    [SerializeField] private GameObject UpgradeMenu;
    [SerializeField] private GameObject Option1;
    [SerializeField] private GameObject Option2;
    [SerializeField] private GameObject Option3;

    public Action Callback_Option1 = null;
    public Action Callback_Option2 = null;
    public Action Callback_Option3 = null;

    private void Start()
    {
        Instance = this;
        UpgradeMenu.SetActive(false);
    }


    public void PromptUpgrade(Upgrade u1, Upgrade u2, Upgrade u3)
    {
        Callback_Option1 = u1.ApplyUpgrade;
        Callback_Option2 = u2.ApplyUpgrade;
        Callback_Option3 = u3.ApplyUpgrade;


        Option1.GetComponent<TextMeshProUGUI>().text = u1.Name;
        Option2.GetComponent<TextMeshProUGUI>().text = u2.Name;
        Option3.GetComponent<TextMeshProUGUI>().text = u3.Name;
        
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
}