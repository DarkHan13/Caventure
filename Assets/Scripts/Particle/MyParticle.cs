using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyParticle : MonoBehaviour
{
    public Rigidbody2D rb;
    private float _seconds = 0;
    private SpriteRenderer _spriteRenderer;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void DestroyParticle(float seconds)
    {
        _seconds = seconds;
        StartCoroutine(DestroyParticle());
    }
    private IEnumerator DestroyParticle()
    {
        yield return new WaitForSeconds(_seconds);
        float t = 1f;
        while (t > 0f)
        {
            t -= Time.deltaTime * 1.5f;
            _spriteRenderer.color = new Color (1f, 1f, 1f, t);
            yield return 0;
        }
        Debug.Log("DESTROY BEFORE");
        Destroy(gameObject);
        Debug.Log("DESTROY AFTER");

    }
    
}
