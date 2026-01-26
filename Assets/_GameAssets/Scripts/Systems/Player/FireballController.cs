using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;

public class FireballController : MonoBehaviour
{
    private float fireballDamage;

    private Vector3 spawnPosition;
    private float maxDistance = 0f; // nadpisywana z zewnątrz

    bool hasHit = false;

    // Update is called once per frame
    void Update()
    {
        if (maxDistance == 0f) return;
        
        float traveledSqr = (transform.position - spawnPosition).sqrMagnitude;

        if (traveledSqr >= maxDistance * maxDistance)
        {
            Debug.Log($"Distance was {maxDistance}!");
            Debug.Log("Destroyed by distance!");
            Destroy(gameObject);
        }
    }

    public void SetTracking(Vector3 startPos, float range)
    {
        spawnPosition = startPos;
        maxDistance = range;
    }

    public void SetDamage(float damage)
    {
        fireballDamage = damage;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHit) return;

        hasHit = true;

        if (collision.TryGetComponent(out NPCController npc))
        {
            Debug.Log($"Destroyed by: NPC!");

            npc.DamageNPC(fireballDamage);
            Destroy(gameObject);
        }
        else
        {
            Debug.Log($"Destroyed by: {collision.gameObject.name}!");

            Destroy(gameObject);
        }
    }
}
