using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndingCondition : MonoBehaviour
{
    // Reference to the terrain GameObject
    public GameObject terrainObject;

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HideTerrain()
    {
        terrainObject.SetActive(false);
    }
}
