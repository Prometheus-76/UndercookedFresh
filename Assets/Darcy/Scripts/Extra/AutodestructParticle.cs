using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutodestructParticle : MonoBehaviour
{
    private ParticleSystem particles;

    // Start is called before the first frame update
    void Start()
    {
        particles = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (particles.IsAlive() == false)
        {
            Destroy(gameObject, 1f);
        }
    }
}
