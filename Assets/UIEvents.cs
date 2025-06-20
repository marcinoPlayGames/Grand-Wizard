using UnityEngine;

public class UIEvents : MonoBehaviour
{
    public void OnClick_SaveGame()
    {
        StatSystem.Instance.SaveGame();
    }

    public void OnClick_LoadGame()
    {
        StatSystem.Instance.LoadGameButton();
    }
}