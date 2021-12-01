using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author: Darcy Matheson
// Purpose: Basic class storing some generic UI sounds, which can be played by clicking on buttons.

public class SoundUI : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] audioClips;

    private float sceneTimer;

    private void Start()
    {
        sceneTimer = 0f;
    }

    public void Update()
    {
        sceneTimer += Time.deltaTime;
    }

    public void PlaySound(int index)
    {
        // If the sound index doesn't exist
        if (index >= audioClips.Length || sceneTimer < 0.2f)
            return;

        // Play the sound
        audioSource.PlayOneShot(audioClips[index]);
    }
}
