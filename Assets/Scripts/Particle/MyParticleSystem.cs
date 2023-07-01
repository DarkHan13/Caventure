using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Random = UnityEngine.Random;

public class MyParticleSystem : MonoBehaviour
{

    [SerializeField] private List<GameObject> particles;

    

    [ContextMenu("Generate")]
    public void GenerateParticles()
    {
        for (int i = 0; i < particles.Count; i++)
        {
            particles[i] = Instantiate(particles[i], transform.position, Quaternion.identity);
            particles[i].SetActive(false);
        }
        for (int i = 0; i < particles.Count; i++)
        {
            particles[i].SetActive(true);
            particles[i].transform.position = transform.position;
            MyParticle particle = particles[i].GetComponent<MyParticle>();
            float x = Random.Range(-3, 3);
            float y = Random.Range(1, 5);
            particle.rb.AddForce(new Vector2(x, y), ForceMode2D.Impulse);
            particle.DestroyParticle(1f);
        }

        Destroy(gameObject);
    }

}
