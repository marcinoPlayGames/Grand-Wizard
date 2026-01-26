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
        public Image image;
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

        int indexInList = -1;

        for (int i = 0; i < objectsForPositions.Length; i++)
        {
            if (objectsForPositions[i].levelIndex == level)
            {
                indexInList = i;
                wizardHead.center = objectsForPositions[i].gameObject.transform.localPosition;
                break;
            }
        }

        if (indexInList == -1)
        {
            wizardHead.center = transform.localPosition;
            return;
        }

        for (int i = 0; i < objectsForPositions.Length; i++)
        {
            var image = objectsForPositions[i].image;

            if (i < indexInList)
                image.color = colorForFinishedLevels;
            else if (i == indexInList)
                image.color = colorForCurrentLevel;
            else if (i == indexInList + 1)
                image.color = colorForNextLevel;
            else
                image.color = colorForBlockedLevels;
        }
    }
}
