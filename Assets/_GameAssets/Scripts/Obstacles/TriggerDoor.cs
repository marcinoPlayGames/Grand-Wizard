using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDoor : MonoBehaviour
{
    public GameObject triggeredObject;

    private MovingObject movingObject;

    private MovingObject movingButton;

    public float activationTime;

    private bool isActivated = false;

    // Start is called before the first frame update
    void Start()
    {
        if (triggeredObject != null)
        {
            // Getting component of MovingSpikes from TriggeredObject
            movingObject = triggeredObject.GetComponent<MovingObject>();

            movingButton = this.GetComponent<MovingObject>();

            if (movingObject == null)
            {
                Debug.LogWarning("TriggeredObject don't have component MovingDoor!");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (movingObject != null && other.gameObject.CompareTag("Player") && !isActivated)
        {
            movingObject.Activate();
            movingButton.Activate();
            StartCoroutine(DeactivateButtonAndDoorTimer());
        }
    }

    IEnumerator DeactivateButtonAndDoorTimer()
    {
        isActivated = true;

        // Wait for the cooldown duration
        yield return new WaitForSeconds(activationTime);

        isActivated = false;

        movingObject.Deactivate();
        movingButton.Deactivate();
    }
}
