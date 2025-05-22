using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSpikes : MonoBehaviour
{
    public GameObject triggeredObject;

    private MovingSpikes movingSpikes;

    // Start is called before the first frame update
    void Start()
    {
        if (triggeredObject != null)
        {
            // Getting component of MovingSpikes from TriggeredObject
            movingSpikes = triggeredObject.GetComponent<MovingSpikes>();

            if (movingSpikes == null)
            {
                Debug.LogWarning("TriggeredObject don't have component MovingSpikes!");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (movingSpikes != null && other.CompareTag("Player"))
        {
            movingSpikes.Activate();
        }
    }
}
