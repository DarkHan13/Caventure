using System;
using UnityEngine;

public class Item : MonoBehaviour
{
    private bool isReady;
    public SpriteRenderer spriteRenderer;
    public Collider2D collider;
    public Rigidbody2D rb;

    public void SetActivePhysics(bool enable)
    {
        
        spriteRenderer.enabled = enable;
        collider.enabled = enable;
        rb.isKinematic = !enable;
    }

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        Debug.Log(gameObject.name);
        if (collider == null) collider = GetComponentInChildren<Collider2D>();
        if (rb == null) rb = GetComponentInChildren<Rigidbody2D>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        isReady = true;
    }
}
