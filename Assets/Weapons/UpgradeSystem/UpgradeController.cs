
using System;
using UnityEngine;

public class UpgradeController : MonoBehaviour
{
    public static UpgradeController Instance;


    [SerializeField] private GameObject UpgradeMenu;

    public Action Callback_Option1 = null;
    public Action Callback_Option2 = null;
    public Action Callback_Option3 = null;

    private void Start()
    {
        Instance = this;
    }


    public void OnPickOption(int index) 
    {
        Debug.Log($"Selected option: {index}");

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