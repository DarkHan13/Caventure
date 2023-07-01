using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InverseScale : MonoBehaviour
{

    public PlayerController player;

    public bool isRight = true;
    void Update()
    {
        if (player.isRight != isRight)
        {
            isRight = player.isRight;
            if (isRight) transform.localScale = new Vector3(1, 1, 1);
            else transform.localScale = new Vector3(1, 1, -1);
        }    
    }
}
