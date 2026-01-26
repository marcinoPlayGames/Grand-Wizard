using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSpikes : MonoBehaviour
{
    [SerializeField]
    private MovingObject movingObject;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (movingObject != null && other.CompareTag("Player"))
        {
            movingObject.Activate();
        }
    }
}
