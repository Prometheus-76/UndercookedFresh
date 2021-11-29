using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class VolumeManager : MonoBehaviour
{
    public AudioMixer mixer;

    // Start is called before the first frame update
    void Start()
    {
        float masterLevel = PlayerPrefs.GetFloat("AudioLevelMaster", 0.8f);
        float environmentLevel = PlayerPrefs.GetFloat("AudioLevelMusic", 1f);
        float effectLevel = PlayerPrefs.GetFloat("AudioLevelSfx", 1f);

        // Scale logarithmically, decibel gain attenuation from -80db to 0
        masterLevel = (masterLevel > 0f) ? (Mathf.Log10(masterLevel) * 20f) : -80f;
        environmentLevel = (environmentLevel > 0f) ? (Mathf.Log10(environmentLevel) * 20f) : -80f;
        effectLevel = (effectLevel > 0f) ? (Mathf.Log10(effectLevel) * 20f) : -80f;

        mixer.SetFloat("MasterVolume", masterLevel);
        mixer.SetFloat("EnvironmentVolume", environmentLevel);
        mixer.SetFloat("EffectsVolume", effectLevel);
    }
}
