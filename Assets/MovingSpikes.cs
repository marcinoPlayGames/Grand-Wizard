using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingSpikes : MonoBehaviour
{
    public int trapMovingXDistance = -15;
    public int trapMovingYDistance = 0;
    public float trapMovingSpeed = 0.1f;
    Transform platform;
    private int moveCounter = 0;
    private bool doMove = false;
    private int finishCount = 0;
    void Start()
    {
        platform = GetComponent<Transform>();
        platform.position = new Vector3(0, 0, 0);
        Debug.Log($"rect y is {platform.position.y}");

        if (trapMovingYDistance != 0  && trapMovingXDistance != 0)
        {
            finishCount = (int)Mathf.Abs((trapMovingXDistance * trapMovingYDistance) / trapMovingSpeed);
        }
        else if (trapMovingYDistance == 0)
        {
            finishCount = (int)Mathf.Abs(trapMovingXDistance / trapMovingSpeed);
        }
        else
        {
            finishCount = (int)Mathf.Abs(trapMovingYDistance / trapMovingSpeed);
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

                if (trapMovingYDistance != 0)
                {
                    newY = (float)platform.position.y + trapMovingSpeed * Mathf.Sign(trapMovingYDistance);
                }
                if (trapMovingXDistance != 0)
                {
                    newX = (float)platform.position.x + trapMovingSpeed * Mathf.Sign(trapMovingXDistance);
                }

                platform.position = new Vector3((float)newX, (float)newY, 0);
            }
        }
    }

    public void Activate()
    {
        doMove = true;
    }
}
