using UnityEngine;
using UnityEngine.UI;

public class NPCDeathCounter : MonoBehaviour
{
    // Static variable to keep track of the total number of NPC deaths
    public EndingCondition terrainObject;
    
    public static int TotalDeaths { get; private set; }

    void Start()
    {
        
    }

    // Method to increment the death count
    public static void IncrementDeathCount()
    {
        TotalDeaths++;
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
