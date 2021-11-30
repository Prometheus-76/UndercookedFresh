using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author: Darcy Matheson
// Purpose: Flickers the intensity of an attached light source using procedural perlin noise lookup.

public class LightFlicker : MonoBehaviour
{
    public float effectIntensity;
    public float speed;

    private float seed;
    private float currentTime;
    private float baseIntensity;

    private Light lightComponent;

    // Start is called before the first frame update
    void Start()
    {
        lightComponent = GetComponent<Light>();

        seed = Random.Range(0f, 10f);
        baseIntensity = lightComponent.intensity;
        currentTime = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        currentTime = Time.time + seed;
        float newIntensity = (Mathf.PerlinNoise(currentTime * speed, seed) * effectIntensity) + baseIntensity;
        lightComponent.intensity = newIntensity;
    }
}
