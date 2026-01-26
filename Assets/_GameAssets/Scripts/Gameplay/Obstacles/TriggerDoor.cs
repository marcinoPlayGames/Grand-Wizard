using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDoor : MonoBehaviour
{
    [SerializeField]
    private MovingObject movingObject;

    [SerializeField]
    private MovingObject movingButton;

    public float activationTime;

    private bool isActivated = false;

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
