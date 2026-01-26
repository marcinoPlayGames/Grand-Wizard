using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    public int objectMovingXDistance = -15;
    public int objectMovingYDistance = 0;
    public float objectMovingSpeed = 0.1f;
    Transform platform;
    private int moveCounter = 0;
    private bool doMove = false;
    private int finishCount = 0;

    private bool wasFirstTime = false;

    private bool activated = false;
    void Start()
    {
        platform = GetComponent<Transform>();
        platform.position = new Vector3(0, 0, 0);
        Debug.Log($"rect y is {platform.position.y}");

        if (objectMovingYDistance != 0  && objectMovingXDistance != 0)
        {
            finishCount = (int)Mathf.Abs((objectMovingXDistance * objectMovingYDistance) / objectMovingSpeed);
        }
        else if (objectMovingYDistance == 0)
        {
            finishCount = (int)Mathf.Abs(objectMovingXDistance / objectMovingSpeed);
        }
        else
        {
            finishCount = (int)Mathf.Abs(objectMovingYDistance / objectMovingSpeed);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(moveCounter);

        if (doMove)
        {
            if (moveCounter != finishCount)
            {
                moveCounter++;

                float newY = 0;
                float newX = 0;

                if (objectMovingYDistance != 0)
                {
                    newY = (float)platform.position.y + objectMovingSpeed * Mathf.Sign(objectMovingYDistance);
                }
                if (objectMovingXDistance != 0)
                {
                    newX = (float)platform.position.x + objectMovingSpeed * Mathf.Sign(objectMovingXDistance);
                }

                platform.position = new Vector3((float)newX, (float)newY, 0);
            }
        }
    }

    public void Activate()
    {
        doMove = true;

        moveCounter = 0;

        if (wasFirstTime)
        {
            objectMovingXDistance *= -1;
            objectMovingYDistance *= -1;
        }
    }

    public void Deactivate()
    {
        doMove = true;
        moveCounter = 0;

        objectMovingXDistance *= -1;
        objectMovingYDistance *= -1;

        wasFirstTime = true;
    }
}
