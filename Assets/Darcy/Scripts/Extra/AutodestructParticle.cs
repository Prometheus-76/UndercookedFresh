using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author: Darcy Matheson
// Purpose: Placed on prefabs with Particle System components to clean them up when they are no longer in use

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
