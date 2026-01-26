using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatTreeButton : MonoBehaviour
{
    public string statId = "Attack_Damage";

    public void OnClick()
    {
        Debug.Log("Clicked!");
        StatUIController.Instance.ShowUpgradeWindow(statId);
    }
}
