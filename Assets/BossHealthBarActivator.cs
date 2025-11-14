using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHealthBarActivator : MonoBehaviour
{
    [SerializeField]
    GameObject bossHealthBarObject;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        bossHealthBarObject.SetActive(true);
    }
}
