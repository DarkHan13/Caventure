using System;
using UnityEngine;

public abstract class Weapon : Item
{
    public int damage;
    public Transform thing;
    private void Start()
    {
        SetParameters(5);
        thing = gameObject.transform.GetChild(0);
    }
    
    void SetParameters(int damage)
    {
        this.damage = damage;
    }
}