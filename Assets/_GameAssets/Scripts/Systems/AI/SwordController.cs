using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordController : MonoBehaviour
{
    // Start is called before the first frame update
    private NPCAI npcAI;
    private NPCController npcController;
    private PlayerMove playerMove;
    private float swordDamage;
    private DamageType damageType;
    public void SetDamage(float damage, DamageType iDamageType)
    {
        swordDamage = damage;
        damageType = iDamageType;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMove playerMove = collision.gameObject.GetComponent<PlayerMove>();

            if (playerMove != null)
            {
                Debug.Log("Collided!");

                playerMove.DamagePlayer(swordDamage, damageType);
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
