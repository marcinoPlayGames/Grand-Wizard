using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlatform : MonoBehaviour
{
    [SerializeField] Transform platform;

    public float distance = 3f;
    public float speed = 1f;

    private Vector3 startPos;
    private float direction = 1f;
    private float moved = 0f;

    void Awake()
    {
        startPos = platform.position;
    }

    void Update()
    {
        float delta = speed * Time.deltaTime * direction;
        platform.position += Vector3.up * delta;
        moved += Mathf.Abs(delta);

        if (moved >= distance)
        {
            direction *= -1f;
            moved = 0f;
        }
    }
}
