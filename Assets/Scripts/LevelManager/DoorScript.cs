using System;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    private List<GameObject> players = new List<GameObject>();
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == 7)
        {
            players.Add(col.gameObject);
        }
    }
    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.layer == 7)
        {
            players.Remove(col.gameObject);
        }
    }

    private void NextLevel()
    {
        foreach (var player in players)
        {
            player.GetComponent<PlayerController>().Save();
        }
        
        LevelManager.NextLevel();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && players.Count > 0) NextLevel();
    }
}
