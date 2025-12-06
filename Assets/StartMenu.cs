using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    [SerializeField]
    Image loadGameButtonImage;
    [SerializeField]
    TextMeshProUGUI loadGameButtonText;
    [SerializeField]
    Button loadGameButtonButton;
    void Start()
    {
        StartSession();

        ResetDataOnStart();
        TransformLoadGameButton();
    }

    void StartSession()
    {
        var data = new Dictionary<string, object>
        {
            { "device", Application.platform.ToString() },
            { "version", Application.version }
        };

        AnalyticsManager.Instance.SendEvent(AnalyticsEvents.SessionStart, data);
    }

    private void TransformLoadGameButton()
    {
        bool showLoadGameButton = SavingSystem.DoesSaveGameFileExist();

        if (showLoadGameButton)
        {
            SetAlpha(loadGameButtonImage, 1f);
            SetAlpha(loadGameButtonText, 1f);
            loadGameButtonButton.interactable = true;
        }
        else
        {
            SetAlpha(loadGameButtonImage, 0.5f);
            SetAlpha(loadGameButtonText, 0.5f);
            loadGameButtonButton.interactable = false;
        }
    }

    void SetAlpha(Graphic graphic, float alpha)
    {
        if (graphic == null) return;

        Color c = graphic.color;
        c.a = alpha;
        graphic.color = c;
    }

    private void ResetDataOnStart()
    {
        PlayerPrefs.DeleteKey("WeaponUpgrades");
        PlayerPrefs.DeleteKey("PlayerStats");
        GameManager.Instance.ResetCoins();
        GameManager.Instance.ResetUnlockedLevel();

        WeaponUpgradeSystem.Instance.ResetDictionaries();

        StatSystem.Instance.ResetDictionaries();
    }
}
