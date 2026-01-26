using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndingCondition : MonoBehaviour
{
    // Reference to the terrain GameObject
    public GameObject terrainObject;

    public void HideTerrain()
    {
        terrainObject.SetActive(false);
    }
}
