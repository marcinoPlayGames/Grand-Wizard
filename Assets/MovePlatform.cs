using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlatform : MonoBehaviour
{
    // Start is called before the first frame update
    public int platformMovingDistance = 3;
    public float platformMovingSpeed = 0.1f;
    Transform platform;
    private int moveCounter = 0;
    void Start()
    {
        platform = GetComponent<Transform>();
        platform.position = new Vector3(0, 0, 0);
        Debug.Log($"rect y is {platform.position.y}");
        
    }

    // Update is called once per frame
    void Update()
    {

        moveCounter++;

        Debug.Log(moveCounter);

        if (moveCounter >= 2 * platformMovingDistance * (1 / platformMovingSpeed))
        {
            moveCounter = 0;
        }

        if (moveCounter < platformMovingDistance * (1 / platformMovingSpeed))
        {
            float newY = (float)platform.position.y + platformMovingSpeed;
            platform.position = new Vector3(platform.position.x, (float)newY, 0);
        }

        if (moveCounter >= platformMovingDistance * (1 / platformMovingSpeed))
        {
            float newY = (float)platform.position.y - platformMovingSpeed;
            platform.position = new Vector3(platform.position.x, (float)newY, 0);
        }
    }
}
