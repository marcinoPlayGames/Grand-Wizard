using Ink.Parsed;
using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OrbitMovement : MonoBehaviour
{
    [HideInInspector]
    public Vector2 center;

    [SerializeField]
    RectTransform rectTransform;

    public float radius = 50f;
    public float speed = 1f;

    private float angle = 0f;

    void Update()
    {
        // Zwiększamy kąt w czasie
        angle += speed * Time.deltaTime;

        // Obliczamy pozycję
        float x = center.x + Mathf.Cos(angle) * radius;
        float y = center.y + Mathf.Sin(angle) * radius;

        rectTransform.anchoredPosition = new Vector2(x, y);
    }
}