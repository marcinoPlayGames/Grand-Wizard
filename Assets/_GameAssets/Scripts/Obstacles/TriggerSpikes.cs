using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSpikes : MonoBehaviour
{
    public GameObject triggeredObject;

    private MovingObject movingObject;

    // Start is called before the first frame update
    void Start()
    {
        if (triggeredObject != null)
        {
            // Getting component of MovingObject from TriggeredObject
            movingObject = triggeredObject.GetComponent<MovingObject>();

            if (movingObject == null)
            {
                Debug.LogWarning("TriggeredObject don't have component MovingObject!");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (movingObject != null && other.CompareTag("Player"))
        {
            movingObject.Activate();
        }
    }
}
