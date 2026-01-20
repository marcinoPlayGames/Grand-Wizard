using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatTreeButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string statId = "Attack_Damage";

    public void OnClick()
    {
        Debug.Log("Clicked!");
        StatUIController.Instance.ShowUpgradeWindow(statId);
    }
}
