using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;

public class SwordController : MonoBehaviour
{
    // Start is called before the first frame update
    private float swordDamage;
    private DamageType damageType;

    bool hasHit = false;
    public void SetDamage(float damage, DamageType iDamageType)
    {
        swordDamage = damage;
        damageType = iDamageType;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasHit) return;
        
        hasHit = true;

        if (collision.gameObject.TryGetComponent(out PlayerMove player))
        {
            player.DamagePlayer(swordDamage, damageType);

            SwordPool.Instance.ReturnSword(gameObject);
        }
        else
        {
            SwordPool.Instance.ReturnSword(gameObject);
        }
    }

    public void ResetSword()
    {
        hasHit = false;
    }
}
