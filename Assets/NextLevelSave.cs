using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextLevelSave : MonoBehaviour
{
    private void OnEnable()
    {
        StatSystem.Instance.SaveGameRuntime();
    }
}
