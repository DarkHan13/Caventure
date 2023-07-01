using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class DestructObject : MonoBehaviour, Destructable
{
    private MyParticleSystem _myParticleSystem;
    private bool _isHaveMyParticle;
    [SerializeField] private int _durability = 1;
    [SerializeField] public GameObject hitParticle;

    private void Start()
    {
        _myParticleSystem = GetComponent<MyParticleSystem>();
        if (_myParticleSystem) _isHaveMyParticle = true;
    }


    public void TakeDamage(int damage, int direction = 0)
    {
        
        _durability -= damage;
        Instantiate(hitParticle, transform.position, Quaternion.Euler(0f, 0f, Random.Range(0, 360)));

        if (_durability <= 0) SelfDestruction();
    }
    
    private void SelfDestruction()
    {
        if (_isHaveMyParticle) _myParticleSystem.GenerateParticles();
        else Destroy(gameObject);
    }
}
