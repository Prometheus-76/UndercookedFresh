using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//Script Author: Keira

/*
PlayerPref keys

"RefreshRate"        - target framerate       - Type: Int   | Range: 30 - 300 | Default: 60  | Displayed as: 30 - 300 int
"MouseSensitivity"   - Sensitivity of mouse   - Type: Float | Range: 0.1 - 5 | Default: 3   | Displayed as: 0.1 - 5 float
"AudioLevelMaster"   - Master Audio           - Type: Float | Range: 0 - 1    | Default: 0.8 | Displayed as: 0 - 10 int
"AudioLevelMusic"    - Music Audio            - Type: Float | Range: 0 - 1    | Default: 1   | Displayed as: 0 - 10 int
"AudioLevelSFX"      - SFX Audio              - Type: Float | Range: 0 - 1    | Default: 1   | Displayed as: 0 - 10 int
*/

public class SettingsMenu : MonoBehaviour
{
    #region variables

    #region Framerate
    [Header("Framerate")]
    //Framerate variables - "RefreshRate"
    [Tooltip("Framerate Default Target Value"), Range(30, 200)]
    public int fpsTarget = 60;
    [Tooltip("Framerate Minimum Value")]
    public int fpsMinimum = 30;
    [Tooltip("Framerate Maximum Value")]
    public int fpsMaximum = 300;
    [Tooltip("Slider For Refreshrate")]
    public Slider fpsSlider;
    [Tooltip("TextMeshPro Input Field For Refreshrate")]
    public TMP_InputField fpsTMPInputField;
    #endregion

    #region Mouse Sensitivity
    [Header("Mouse Sensitivity")]
    //Mouse Sensitivity Variables - "MouseSensitivity"
    [Tooltip("Mouse Sensitivity Default Value"), Range(1f, 5f)]
    public float mouseSensitivityTarget = 3f;
    private float mouseSensitivityMinimum = 1f;
    private float mouseSensitivityMaximum = 5f;
    [Tooltip("Slider For Mouse Sensitivity")]
    public Slider mouseSensitivitySlider;
    [Tooltip("TextMeshPro Input Field For Mouse Sensitivity")]
    public TMP_InputField mouseSensitivityTMPInputField;
    #endregion

    #region Audio
    [Header("Audio")]
    //Audio variables - General
    private int audioMinimum = 0;
    private int audioMaximum = 10;

    #region Audio Master
    //AudioMaster - "AudioLevelMaster"
    [Header("Master Audio Default Value"), Range(0, 10)]
    public int audioMasterTarget = 8;
    [Tooltip("Slider For Master Audio")]
    public Slider audioMasterSlider;
    [Tooltip("TextMeshPro Input Field For Master Audio")]
    public TMP_InputField audioMasterTMPInputField;
    #endregion

    #region Audio Music
    //AudioMusic - "AudioLevelMusic"
    [Header("Music Audio Default Value"), Range(0, 10)]
    public int audioMusicTarget = 10;
    [Tooltip("Slider For Music Audio")]
    public Slider audioMusicSlider;
    [Tooltip("TextMeshPro Input Field For Music Audio")]
    public TMP_InputField audioMusicTMPInputField;
    #endregion

    #region Audio SFX
    //AudioSFX - "AudioSFX"
    [Header("SFX Audio Default Value"), Range(0, 10)]
    public int audioSFXTarget = 10;
    [Tooltip("Slider For SFX Audio")]
    public Slider audioSFXSlider;
    [Tooltip("TextMeshPro Input Field For SFX Audio")]
    public TMP_InputField audioSFXTMPInputField;
    #endregion

    #endregion

    #region Other
    
    #endregion

    #endregion

    void Start()
    {
        
        #region Initialisation

        #region Framerate
        //Framerate
        fpsTarget = PlayerPrefs.GetInt("RefreshRate", 60);
        fpsSlider.maxValue = fpsMaximum;
        fpsSlider.value = (fpsTarget);
        fpsSlider.minValue = fpsMinimum;
        fpsTMPInputField.text = fpsTarget.ToString();
        #endregion

        #region Mouse Sensitivity
        //Mouse Sensitivity
        mouseSensitivityTarget = PlayerPrefs.GetFloat("MouseSensitivity", 3);
        mouseSensitivitySlider.maxValue = mouseSensitivityMaximum;
        mouseSensitivitySlider.value = (mouseSensitivityTarget);
        mouseSensitivitySlider.minValue = mouseSensitivityMinimum;
        mouseSensitivityTMPInputField.text = mouseSensitivityTarget.ToString();
        #endregion

        #region Audio

        #region Audio Master
        //AudioMaster
        audioMasterTarget = (int)(PlayerPrefs.GetFloat("AudioLevelMaster", 8f / 10f) * 10f);
        audioMasterSlider.maxValue = audioMaximum;
        audioMasterSlider.value = (audioMasterTarget);
        audioMasterSlider.minValue = audioMinimum;
        audioMasterTMPInputField.text = audioMasterTarget.ToString();
        #endregion

        #region Audio Music
        //AudioMusic
        audioMusicTarget = (int)(PlayerPrefs.GetFloat("AudioLevelMusic", 10f / 10f) * 10f);
        audioMusicSlider.maxValue = audioMaximum;
        audioMusicSlider.value = (audioMusicTarget);
        audioMusicSlider.minValue = audioMinimum;
        audioMusicTMPInputField.text = audioMusicTarget.ToString();
        #endregion

        #region Audio SFX
        //AudioSFX
        audioSFXTarget = (int)(PlayerPrefs.GetFloat("AudioLevelSFX", 10f / 10f) * 10f);
        audioSFXSlider.maxValue = audioMaximum;
        audioSFXSlider.value = audioSFXTarget;
        audioSFXSlider.minValue = audioMinimum;
        audioSFXTMPInputField.text = audioSFXTarget.ToString();
        #endregion

        #endregion

        #endregion
        
    }

    #region functions

    #region Refreshrate
    //Refresh Rate
    public void SetFramerate()
    {
        int inputfieldText;
        if ((int)fpsSlider.value != fpsTarget)
        {
            fpsTarget = (int)fpsSlider.value;
            fpsTMPInputField.text = fpsTarget.ToString();
        }
        else if (int.TryParse(fpsTMPInputField.text, out inputfieldText))
        {
            if (inputfieldText != fpsTarget)
            {
                fpsTarget = Mathf.Clamp(inputfieldText, fpsMinimum, fpsMaximum);
                fpsTMPInputField.text = fpsTarget.ToString();
                fpsSlider.SetValueWithoutNotify(fpsTarget);
            }
        }
        else
        {
            fpsTMPInputField.text = fpsTarget.ToString();
        }
        Application.targetFrameRate = fpsTarget;
        PlayerPrefs.SetInt("RefreshRate", fpsTarget);
    }
    public void ClampTextInputFramerate()
    {
        int inputfieldText;
        if (int.TryParse(fpsTMPInputField.text, out inputfieldText))
        {
            inputfieldText = Mathf.Clamp(inputfieldText, 0, 1000);
            if (inputfieldText == 1000)
            {
                inputfieldText = fpsMaximum;
            }
            fpsTMPInputField.text = inputfieldText.ToString();
        }
        else
        {
            fpsTMPInputField.text = "";
        }
    }
    #endregion

    #region Mouse Sensitivity
    //Mouse Sensitivity
    public void SetMouseSense()
    {
        float inputfieldText;

        if (mouseSensitivitySlider.value != mouseSensitivityTarget)
        {
            mouseSensitivityTarget = Mathf.Round(mouseSensitivitySlider.value * 10) / 10;
            mouseSensitivityTMPInputField.text = mouseSensitivityTarget.ToString();
        }
        else if (float.TryParse(mouseSensitivityTMPInputField.text, out inputfieldText))
        {
            if (inputfieldText != mouseSensitivityTarget)
            {
                mouseSensitivityTarget = Mathf.Clamp(inputfieldText, mouseSensitivityMinimum, mouseSensitivityMaximum);
                mouseSensitivityTMPInputField.text = mouseSensitivityTarget.ToString();
                mouseSensitivitySlider.SetValueWithoutNotify(mouseSensitivityTarget);
            }
        }
        else
        {
            mouseSensitivityTMPInputField.text = mouseSensitivityTarget.ToString();
        }
        PlayerPrefs.SetFloat("MouseSensitivity", mouseSensitivityTarget);
    }
    public void ClampTextInputMouseSense()
    {
        float inputfieldText;
        if (float.TryParse(mouseSensitivityTMPInputField.text, out inputfieldText))
        {
            inputfieldText = (float)(Mathf.Round(inputfieldText * 10) / 10);
            inputfieldText = Mathf.Clamp(inputfieldText, 0, 100);
            if (inputfieldText == 100)
            {
                inputfieldText = mouseSensitivityMaximum;
            }
            mouseSensitivityTMPInputField.text = inputfieldText.ToString();
        }
        else
        {
            mouseSensitivityTMPInputField.text = "";
        }
    }
    #endregion

    #region Audio

    #region Audio Master
    //Audio Master
    public void SetAudioMaster()
    {
        int inputfieldText;

        if ((int)audioMasterSlider.value != audioMasterTarget)
        {
            audioMasterTarget = (int)audioMasterSlider.value;
            audioMasterTMPInputField.text = audioMasterTarget.ToString();
        }
        else if (int.TryParse(audioMasterTMPInputField.text, out inputfieldText))

        {
            if (inputfieldText != audioMasterTarget)
            {
                audioMasterTarget = Mathf.Clamp(inputfieldText, audioMinimum, audioMaximum);
                audioMasterTMPInputField.text = audioMasterTarget.ToString();
                audioMasterSlider.SetValueWithoutNotify(audioMasterTarget);
            }
        }
        else
        {
            audioMasterTMPInputField.text = audioMasterTarget.ToString();
        }
        PlayerPrefs.SetFloat("AudioLevelMaster", audioMasterTarget / 10f);
    }
    public void ClampTextInputAudioMaster()
    {
        int inputfieldText;
        if (int.TryParse(audioMasterTMPInputField.text, out inputfieldText))
        {
            audioMasterTMPInputField.text = Mathf.Clamp(inputfieldText, 0, 99).ToString();
        }
        else
        {
            audioMasterTMPInputField.text = "";
        }
    }
    #endregion

    #region Audio Music
    //Audio Music
    public void SetAudioMusic()
    {
        int inputfieldText;
        if ((int)audioMusicSlider.value != audioMusicTarget)
        {
            audioMusicTarget = (int)audioMusicSlider.value;
            audioMusicTMPInputField.text = audioMusicTarget.ToString();
        }
        else if (int.TryParse(audioMusicTMPInputField.text, out inputfieldText))

        {
            if (inputfieldText != audioMusicTarget)
            {
                audioMusicTarget = Mathf.Clamp(inputfieldText, audioMinimum, audioMaximum);
                audioMusicTMPInputField.text = audioMusicTarget.ToString();
                audioMusicSlider.SetValueWithoutNotify(audioMusicTarget);
            }
        }
        else
        {
            audioMusicTMPInputField.text = audioMusicTarget.ToString();
        }
        PlayerPrefs.SetFloat("AudioLevelMusic", audioMusicTarget / 10f);
    }
    public void ClampTextInputAudioMusic()
    {
        int inputfieldText;
        if (int.TryParse(audioMusicTMPInputField.text, out inputfieldText))
        {
            audioMusicTMPInputField.text = Mathf.Clamp(inputfieldText, 0, 99).ToString();
        }
        else
        {
            audioMusicTMPInputField.text = "";
        }
    }
    #endregion

    #region Audio SFX
    //Audio SFX
    public void SetAudioSFX()
    {
        int inputfieldText;
        if ((int)audioSFXSlider.value != audioSFXTarget)
        {
            audioSFXTarget = (int)audioSFXSlider.value;
            audioSFXTMPInputField.text = audioSFXTarget.ToString();
        }
        else if (int.TryParse(audioSFXTMPInputField.text, out inputfieldText))

        {
            if (inputfieldText != audioSFXTarget)
            {
                audioSFXTarget = Mathf.Clamp(inputfieldText, audioMinimum, audioMaximum);
                audioSFXTMPInputField.text = audioSFXTarget.ToString();
                audioSFXSlider.SetValueWithoutNotify(audioSFXTarget);
            }
        }
        else
        {
            audioSFXTMPInputField.text = audioSFXTarget.ToString();
        }
        PlayerPrefs.SetFloat("AudioLevelSFX", audioSFXTarget / 10f);
    }
    public void ClampTextInputAudioSFX()
    {
        int inputfieldText;
        if (int.TryParse(audioSFXTMPInputField.text, out inputfieldText))
        {
            audioSFXTMPInputField.text = Mathf.Clamp(inputfieldText, 0, 99).ToString();
        }
        else
        {
            audioSFXTMPInputField.text = "";
        }
    }
    #endregion

    #endregion

    #endregion
}

