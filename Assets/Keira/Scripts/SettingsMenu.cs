using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//Script Author: Keira

/*
PlayerPref keys

Sliders - 

"MouseSensitivity"     - Sensitivity of mouse   - Type: Float | Range: 1 - 5  | Default: 3   | Displayed as: 1 - 5 float
"AudioLevelMaster"     - Master Audio           - Type: Float | Range: 0 - 1  | Default: 0.8 | Displayed as: 0 - 10 int
"AudioLevelMusic"      - Music Audio            - Type: Float | Range: 0 - 1  | Default: 1   | Displayed as: 0 - 10 int
"AudioLevelSfx"        - Sfx Audio              - Type: Float | Range: 0 - 1  | Default: 1   | Displayed as: 0 - 10 int

To add/edit

"ScreenshakeIntensity" - Screenshake            - Type: Int   | Range: 0 - 10 | Default: 10  | Displayed as: 0 - 10 int

Toggles: -                                      - Type: Int   | Range: 0 or 1 | Default: Varies 

"HeadBob"              - Head Bob effect        - Default: true  (1)
"CrouchToggle"         - Toggle Crouch          - Default: false (0)
"VSync"                - Use V-Sync             - Default: true  (1)

Presets: - 

"Resolution"           - Resolution of game     - Type: Int   | Range: 0 - 5  | Default: 2   | Displayed as: below
                                                + 0 - 1366x768 | 1 - 1600x900 | 2 - 1920x1080 | 3 - 2560x1440 | 4 - 3440x1440 | 5 - 3840x2160

"RefreshRate"          - target framerate       - Type: Int   | Range: 0 - 6  | Default: 2   | Displayed as: below
                                                + 0 - Unlimited | 1 - 30 | 2 - 60 | 3 - 75 | 4 - 120 | 5 - 144 | 6 - 240

"WindowMode"           - Window Mode            - Type: Int   | Range: 0 - 1  | Default: 1   | Displayed as: below
                                                + 0 - FullScreen | 1 - Windowed 
*/

public class SettingsMenu : MonoBehaviour
{
    #region variables

    #region Video

    #region Framerate
    [Header("Framerate")]
    [Tooltip("Drop Down For Refreshrate")]
    public TMP_Dropdown fpsDropdown;
    private int fpsTarget = 2;
    #endregion

    #region Window Mode
    [Header("Window Mode")]
    [Tooltip("Drop Down For Window Mode")]
    public TMP_Dropdown windowModeDropdown;
    private int windowModeTarget = 1;
    #endregion

    #region Resolution
    [Header("Resolution")]
    [Tooltip("Slider For Refreshrate")]
    public TMP_Dropdown resolutionDropdown;
    private int resolutionTarget = 2;
    #endregion

    #region V-Sync
    [Header("V-Sync")]
    [Tooltip("Drop Down For V-Sync False/True")]
    public TMP_Dropdown vSyncDropdown;
    private int vSyncTarget = 1;
    #endregion

    #endregion

    #region Audio

    #region Audio Master
    [Header("Audio Master")]
    [Tooltip("Master Audio Default Value"), Range(0, 10)]
    public int audioMasterTarget = 8;
    [Tooltip("Slider For Master Audio")]
    public Slider audioMasterSlider;
    [Tooltip("TextMeshPro Input Field For Master Audio")]
    public TMP_Text audioMasterTMPText;
    #endregion

    #region Audio Music
    [Header("Audio Music")]
    [Tooltip("Music Audio Default Value"), Range(0, 10)]
    public int audioMusicTarget = 10;
    [Tooltip("Slider For Music Audio")]
    public Slider audioMusicSlider;
    [Tooltip("TextMeshPro Input Field For Music Audio")]
    public TMP_Text audioMusicTMPText;
    #endregion

    #region Audio Sfx
    [Header("Audio SFX")]
    [Tooltip("Sfx Audio Default Value"), Range(0, 10)]
    public int audioSfxTarget = 10;
    [Tooltip("Slider For Sfx Audio")]
    public Slider audioSfxSlider;
    [Tooltip("TextMeshPro Input Field For Sfx Audio")]
    public TMP_Text audioSfxTMPText;
    #endregion

    #endregion

    #region Game

    #region Mouse Sensitivity
    [Header("Mouse Sensitivity")]
    //Mouse Sensitivity Variables - "MouseSensitivity"
    [Tooltip("Mouse Sensitivity Default Value")]
    public float mouseSensitivityTarget = 3f;
    [Tooltip("Slider For Mouse Sensitivity")]
    public Slider mouseSensitivitySlider;
    [Tooltip("TextMeshPro Input Field For Mouse Sensitivity")]
    public TMP_Text mouseSensitivityTMPText;
    #endregion

    #region Screenshake
    [Header("Screenshake Intensity")]
    //Mouse Sensitivity Variables - "MouseSensitivity"
    [Tooltip("Screenshake Default Value"), Range(0, 10)]
    public int screenshakeTarget = 10;
    [Tooltip("Slider For Screenshake")]
    public Slider screenshakeSlider;
    [Tooltip("TextMeshPro Input Field For Mouse Sensitivity")]
    public TMP_Text screenshakeTMPText;
    #endregion

    #region Head Bob
    [Header("Head Bob")]
    [Tooltip("Drop Down For Head Bob False/True")]
    public TMP_Dropdown headbobDropdown;
    private int headbobTarget = 1;
    #endregion

    #region Crouch Toggle
    [Header("Crouch Toggle")]
    [Tooltip("Drop Down For Crouch Toggle False/True")]
    public TMP_Dropdown crouchToggleDropdown;
    private int crouchToggleTarget = 0;
    #endregion

    #endregion

    #endregion


    void Start()
    {

        #region Initialisation

        #region Video

        #region Framerate
        //fpsTarget = PlayerPrefs.GetInt("RefreshRate", fpsTarget);
        //fpsDropdown.value = fpsTarget;
        #endregion

        #region Window Mode
        windowModeTarget = PlayerPrefs.GetInt("WindowMode", windowModeTarget);
        windowModeDropdown.value = windowModeTarget;
        windowModeDropdown.RefreshShownValue();
        #endregion

        #region Resolution
        resolutionTarget = PlayerPrefs.GetInt("Resolution", resolutionTarget);
        resolutionDropdown.value = resolutionTarget;
        resolutionDropdown.RefreshShownValue();
        #endregion

        #region V-Sync
        vSyncTarget = PlayerPrefs.GetInt("VSync", vSyncTarget);
        vSyncDropdown.value = vSyncTarget;
        vSyncDropdown.RefreshShownValue();
        #endregion

        #endregion

        #region Audio

        #region Audio Master
        audioMasterTarget = (int)(PlayerPrefs.GetFloat("AudioLevelMaster", audioMasterTarget / 10f) * 10f);
        audioMasterSlider.value = (audioMasterTarget);
        AudioMasterTextUpdate();
        #endregion

        #region Audio Music
        audioMusicTarget = (int)(PlayerPrefs.GetFloat("AudioLevelMusic", audioMusicTarget / 10f) * 10f);
        audioMusicSlider.value = (audioMusicTarget);
        AudioMusicTextUpdate();
        #endregion

        #region Audio Sfx
        audioSfxTarget = (int)(PlayerPrefs.GetFloat("AudioLevelSfx", audioSfxTarget / 10f) * 10f);
        audioSfxSlider.value = audioSfxTarget;
        AudioSfxTextUpdate();
        #endregion

        #endregion

        #region Game

        #region Mouse Sensitivity
        mouseSensitivityTarget = PlayerPrefs.GetFloat("MouseSensitivity", mouseSensitivityTarget);
        mouseSensitivitySlider.value = mouseSensitivityTarget;
        MouseSensitivityTextUpdate();
        #endregion

        #region Screenshake
        screenshakeTarget = PlayerPrefs.GetInt("ScreenshakeIntensity", screenshakeTarget);
        screenshakeSlider.value = screenshakeTarget;
        ScreenshakeTextUpdate();
        #endregion

        #region Head Bob
        headbobTarget = PlayerPrefs.GetInt("HeadBob", headbobTarget);
        headbobDropdown.value = headbobTarget;
        #endregion

        #region Crouch Toggle
        crouchToggleTarget = PlayerPrefs.GetInt("CrouchToggle", crouchToggleTarget);
        crouchToggleDropdown.value = crouchToggleTarget;
        #endregion

        #endregion

        #endregion

    }

    #region functions

    #region Video

    #region Framerate
    public void FramerateDropdownUpdate()
    {
        fpsTarget = fpsDropdown.value;
        PlayerPrefs.SetInt("RefreshRate", fpsTarget);
    }
    #endregion

    #region Window Mode
    public void WindowModeDropdownUpdate()
    {
        windowModeTarget = windowModeDropdown.value;
        PlayerPrefs.SetInt("WindowMode", windowModeTarget);
    }
    #endregion

    #region Resolution
    public void ResolutionDropdownUpdate()
    {
        resolutionTarget = resolutionDropdown.value;
        PlayerPrefs.SetInt("Resolution", resolutionTarget);
    }
    #endregion

    #region V-Sync
    public void VSyncDropdownUpdate()
    {
        vSyncTarget = vSyncDropdown.value;
        PlayerPrefs.SetInt("VSync", vSyncTarget);
    }
    #endregion

    #endregion

    #region Audio

    #region Audio Master
    //Audio Master
    public void AudioMasterSliderUpdate()
    {
        audioMasterTarget = (int)audioMasterSlider.value;
        AudioMasterTextUpdate();
        PlayerPrefs.SetFloat("AudioLevelMaster", audioMasterTarget / 10f);
    }
    private void AudioMasterTextUpdate()
    {
        //audioMasterTMPText.text = audioMasterTarget.ToString();
    }
    #endregion

    #region Audio Music
    //Audio Music
    public void AudioMusicSliderUpdate()
    {
        audioMusicTarget = (int)audioMusicSlider.value;
        AudioMusicTextUpdate();
        PlayerPrefs.SetFloat("AudioLevelMusic", audioMusicTarget / 10f);
    }
    private void AudioMusicTextUpdate()
    {
        //audioMusicTMPText.text = audioMusicTarget.ToString();
    }
    #endregion

    #region Audio Sfx
    //Audio Sfx
    public void AudioSfxSliderUpdate()
    {
        audioSfxTarget = (int)audioSfxSlider.value;
        AudioSfxTextUpdate();
        PlayerPrefs.SetFloat("AudioLevelSfx", audioSfxTarget / 10f);
    }
    private void AudioSfxTextUpdate()
    {
        //audioSfxTMPText.text = audioSfxTarget.ToString();
    }
    #endregion

    #endregion

    #region Game

    #region Mouse Sensitivity
    public void MouseSensitivitySliderUpdate()
    {
        mouseSensitivityTarget = Mathf.Round(mouseSensitivitySlider.value * 10f) / 10f;
        MouseSensitivityTextUpdate();
        PlayerPrefs.SetFloat("MouseSensitivity", mouseSensitivityTarget);
    }
    private void MouseSensitivityTextUpdate()
    {
        //mouseSensitivityTMPText.text = mouseSensitivityTarget.ToString();
    }
    #endregion

    #region Screenshake
    public void ScreenshakeSliderUpdate()
    {
        screenshakeTarget = (int)screenshakeSlider.value;
        ScreenshakeTextUpdate();
        PlayerPrefs.SetInt("ScreenshakeIntensity", screenshakeTarget);
    }
    public void ScreenshakeTextUpdate()
    {
        //screenshakeTMPText.text = mouseSensitivityTarget.ToString();
    }
    #endregion

    #region Head Bob
    public void HeadbobUpdate()
    {
        headbobTarget = headbobDropdown.value;
        PlayerPrefs.SetInt("HeadBob", headbobTarget);
    }
    #endregion

    #region Crouch Toggle
    public void CrouchToggleDropdownUpdate()
    {
        crouchToggleTarget = crouchToggleDropdown.value;
        PlayerPrefs.SetInt("CrouchToggle", crouchToggleTarget);
    }
    #endregion

    #endregion

    #endregion
}