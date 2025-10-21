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

    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitForSeconds(1f);

        CheckAndHideTerrain();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CheckAndHideTerrain()
    {
        if (NPCDeathCounter.TotalDeaths >= deathsThreshold)
        {
            terrainObject.SetActive(false);
        }
        Debug.Log(NPCDeathCounter.TotalDeaths >= deathsThreshold);
    }
}
