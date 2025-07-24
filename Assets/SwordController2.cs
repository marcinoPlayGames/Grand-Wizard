using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordController2 : MonoBehaviour
{
    // Start is called before the first frame update
    private NPCAI npcAI;
    private NPCController npcController;
    private PlayerMove playerMove;
    private float swordDamage;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetDamage(float damage)
    {
        swordDamage = damage;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("NPC"))
        {
            NPCController npcController = collision.gameObject.GetComponent<NPCController>();

            if (npcController != null)
            {
                Debug.Log("Collided!");
                npcController.DamageNPC(swordDamage);
            }

            Debug.Log(gameObject.name);
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMove playerMove = collision.gameObject.GetComponent<PlayerMove>();

            if (playerMove != null)
            {
                Debug.Log("Collided!");
                playerMove.DamagePlayer(swordDamage, DamageType.Physical);
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
