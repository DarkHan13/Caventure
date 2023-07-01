using System;
using UnityEngine;

public class Sword : Weapon
{


    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("entity"))
        {
            Destructable _destructable = col.gameObject.GetComponent<Destructable>();
            if (_destructable != null)
            {
                Debug.Log("NOT NULL");
                var d = gameObject.transform.position.x - col.transform.position.x;
                var dir = d > 0 ? 0 : 1;
                _destructable.TakeDamage(damage, dir);
            }
            else
            {
                Debug.Log(col.gameObject.name);
            }
        }
    }

}
