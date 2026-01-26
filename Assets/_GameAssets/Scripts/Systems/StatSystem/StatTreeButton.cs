using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatTreeButton : MonoBehaviour
{
    public string statId = "Attack_Damage";

    public void OnClick()
    {
        StatUIController.Instance.ShowUpgradeWindow(statId);
    }
}
