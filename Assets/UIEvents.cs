using UnityEngine;

public class UIEvents : MonoBehaviour
{
    public void OnClick_SaveGame()
    {
        StatSystem.Instance.SavePlayerGame();
    }

    public void OnClick_LoadGame()
    {
        StatSystem.Instance.LoadGameButton();
    }
}