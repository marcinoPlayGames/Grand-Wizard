using UnityEngine;
using UnityEngine.SceneManagement;

public class WeaponUpgradeButton : MonoBehaviour
{
    public string weaponId = "DefaultSword";

    public void OnClick()
    {
        WeaponUpgradeUIController.Instance.ShowPanel(weaponId);
    }

    public void OnClickDetailsCurrent()
    {
        WeaponUpgradeUIController.Instance.ShowDetails(weaponId, false); // current level
    }

    public void OnClickDetailsNext()
    {
        WeaponUpgradeUIController.Instance.ShowDetails(weaponId, true); // next level
    }

    public void LoadNextLevelScene()
    {
        SceneManager.LoadScene("NextLevel"); // lub inna nazwa Twojej sceny
    }

    public void OnClickBackToMainPanel()
    {
        WeaponUpgradeUIController.Instance.BackToMainPanel();
    }

    public void OnClickBackToUpgradePanel()
    {
        WeaponUpgradeUIController.Instance.BackToUpgrade();
    }
}