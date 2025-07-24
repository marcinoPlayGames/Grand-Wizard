using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballController : MonoBehaviour
{
    // Start is called before the first frame update
    private PlayerAttacks playerAttacks;
    private NPCController npcController;
    private float fireballDamage;

    private Vector3 spawnPosition;
    private float maxDistance = 0f; // nadpisywana z zewnątrz

    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        float traveled = Vector3.Distance(transform.position, spawnPosition);

        //Debug.Log("traveled = " + traveled);
        Debug.Log("maxDistance = " + maxDistance);
        if (traveled >= maxDistance && maxDistance != 0 && maxDistance != null)
        {
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

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("NPC"))
        {
            NPCController npcController = collision.gameObject.GetComponent<NPCController>();

            if (npcController != null)
            {
                Debug.Log("Collided!");
                npcController.DamageNPC(fireballDamage);
            }

            Debug.Log(gameObject.name);
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Damage(float damage)
    {

    }
}
