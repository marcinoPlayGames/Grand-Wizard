using UnityEngine;

public class UIEvents : MonoBehaviour
{
    [SerializeField]
    UIButtonInfoHandler UIButtonInfoHandler;
    public void OnClick_SaveGame()
    {
        try
        {
            StatSystem.Instance.SavePlayerGame();

            UIButtonInfoHandler.ShowInfoUI("Saving game was successfull!");
        }
        catch
        {
            UIButtonInfoHandler.ShowInfoUI("Something went wrong with saving game!", InfoType.Error);
            return;
        }  
    }

    public void OnClick_LoadGame()
    {
        StatSystem.Instance.LoadGameButton();
    }
}