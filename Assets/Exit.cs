using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : MonoBehaviour
{
    public void OnClick()
    {
        Debug.Log("Exiting game...!");
        Application.Quit();
    }
}
