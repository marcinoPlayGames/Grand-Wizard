using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndingCondition : MonoBehaviour
{
    // Reference to the terrain GameObject
    public GameObject terrainObject;

    // Number of NPC deaths required to hide the terrain
    public int deathsThreshold = 5;
    public TMP_Text NPCLeft;
    // Start is called before the first frame update
    void Start()
    {
        NPCLeft.text = $"Enemies left: {deathsThreshold}";
        CheckAndHideTerrain();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CheckAndHideTerrain()
    {
        NPCLeft.text = $"Enemies left: {deathsThreshold - NPCDeathCounter.TotalDeaths}";
        if (NPCDeathCounter.TotalDeaths >= deathsThreshold)
        {
            terrainObject.SetActive(false);
        }
        Debug.Log(NPCDeathCounter.TotalDeaths >= deathsThreshold);
    }
}
