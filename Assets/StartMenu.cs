using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Awake()
    {
        PlayerPrefs.DeleteKey("WeaponUpgrades");
        PlayerPrefs.DeleteKey("PlayerStats");
        GameManager.Instance.ResetCoins();
        GameManager.Instance.ResetUnlockedLevel();

        WeaponUpgradeSystem.Instance.ResetDictionaries();

        StatSystem.Instance.ResetDictionaries();
        StatSystem.Instance.ResetAllStatData();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
