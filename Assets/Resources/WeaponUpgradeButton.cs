using UnityEngine;

public class WeaponUpgradeButton : MonoBehaviour
{
    public string weaponId = "DefaultSword";

    public void OnClick()
    {
        WeaponUpgradeUIController.Instance.ShowPanel(weaponId);
    }
}