using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class WorldMapPointersManager : MonoBehaviour
{
    [System.Serializable]
    public class GameObjectLevelIndex
    {
        public GameObject gameObject;
        public int levelIndex;
    }

    public Color colorForFinishedLevels;
    public Color colorForCurrentLevel;
    public Color colorForNextLevel;
    public Color colorForBlockedLevels;

    [SerializeField]
    public OrbitMovement wizardHead;

    [SerializeField]
    public GameObjectLevelIndex[] objectsForPositions;

    private void Start()
    {
        int level = GameManager.Instance.GetUnlockedLevel();

        var target = objectsForPositions
        .FirstOrDefault(o => o.levelIndex == level);

        if (target == null)
        {
            Debug.LogError("Nie znaleziono obiektu dla levelu " + level);
            wizardHead.center = transform.localPosition;
            return;
        }

        int indexInList = Array.IndexOf(objectsForPositions, target);

        wizardHead.center = target.gameObject.GetComponent<Transform>().localPosition;

        for (int i = 0; i < objectsForPositions.Length; i++)
        {
            if (i < indexInList)
            {
                objectsForPositions[i].gameObject.GetComponent<Image>().color = colorForFinishedLevels;
            }
            else if (i == indexInList)
            {
                objectsForPositions[i].gameObject.GetComponent<Image>().color = colorForCurrentLevel;
            }
            else if (i == indexInList + 1)
            {
                objectsForPositions[i].gameObject.GetComponent<Image>().color = colorForNextLevel;
            }
            else
            {
                objectsForPositions[i].gameObject.GetComponent<Image>().color = colorForBlockedLevels;
            }
        }
    }
}
