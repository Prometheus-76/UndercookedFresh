using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author: Darcy Matheson
// Purpose: Basic class storing some generic UI sounds, which can be played by clicking on buttons.

public class SoundUI : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] audioClips;

    public void PlaySound(int index)
    {
        // If the sound index doesn't exist
        if (index >= audioClips.Length)
            return;

        // Play the sound
        audioSource.PlayOneShot(audioClips[index]);
    }
}
