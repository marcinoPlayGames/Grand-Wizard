using Ink.Parsed;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OrbitMovement : MonoBehaviour
{
    [HideInInspector]
    public Vector2 center;

    [SerializeField]
    RectTransform rectTransform;

    private Vector2 pos;

    public float radius = 50f;
    public float speed = 1f;

    private float angle = 0f;

    void Update()
    {
        // Zwiększamy kąt w czasie
        angle += speed * Time.deltaTime;

        // Obliczamy pozycję
        pos.x = center.x + Mathf.Cos(angle) * radius;
        pos.y = center.y + Mathf.Sin(angle) * radius;

        rectTransform.anchoredPosition = pos;
    }
}