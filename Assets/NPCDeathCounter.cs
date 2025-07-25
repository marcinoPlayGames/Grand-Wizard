using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NPCDeathCounter : MonoBehaviour
{
    // Static variable to keep track of the total number of NPC deaths
    public EndingCondition terrainObject;
    
    public static int TotalDeaths { get; private set; }

    public TMP_Text NPCLeft;
    private static int deathsThreshold = 0;

    void Start()
    {
        deathsThreshold = terrainObject.deathsThreshold;
        NPCLeft.text = $"Enemies left: {deathsThreshold}";
    }

    void Awake()
    {
        if (terrainObject == null)
        {
            GameObject terrainObj = GameObject.Find("Blokada");
            if (terrainObj != null)
                terrainObject = terrainObj.GetComponent<EndingCondition>();
        }
    }

    // Method to increment the death count
    public void IncrementDeathCount()
    {
        TotalDeaths++;
        NPCLeft.text = $"Enemies left: {deathsThreshold - NPCDeathCounter.TotalDeaths}";
        Debug.Log(TotalDeaths);
    }

    public void CheckEndingCondition()
    {
        terrainObject.CheckAndHideTerrain();
    }

    // Method to reset the death count
    public static void ResetDeathCount()
    {
        TotalDeaths = 0;
    }
}
