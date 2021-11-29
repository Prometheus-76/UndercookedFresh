using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//Script Author: Keira


/*
PlayerPref keys

Sliders - 

"MouseSensitivity"     - Sensitivity of mouse   - Type: Float | Range: 0.1 - 5  | Default: 2.5   | Displayed as: 0.1 - 5 float
"AudioLevelMaster"     - Master Audio           - Type: Float | Range: 0 - 1  | Default: 0.8 | Displayed as: 0 - 10 int
"AudioLevelMusic"      - Music Audio            - Type: Float | Range: 0 - 1  | Default: 1   | Displayed as: 0 - 10 int
"AudioLevelSfx"        - Sfx Audio              - Type: Float | Range: 0 - 1  | Default: 1   | Displayed as: 0 - 10 int
"ScreenshakeIntensity" - Screenshake            - Type: Int   | Range: 0 - 10 | Default: 10  | Displayed as: 0 - 10 int

Toggles: -                                      - Type: Int   | Range: 0 or 1 | Default: Varies 

"HeadBob"              - Head Bob effect        - Default: true  (1)
"CrouchToggle"         - Toggle Crouch          - Default: false (0)
"VSync"                - Use V-Sync             - Default: true  (1)

Presets: - 

"Resolution"  X/Y      - Resolution of game     - Type: Int   | Range: 0 - 5  | Default: 2   | Displayed (and stored seperately) as: below
                                                + 0 - 1366x768 | 1 - 1600x900 | 2 - 1920x1080 | 3 - 2560x1440 | 4 - 3440x1440 | 5 - 3840x2160

"RefreshRate"          - target framerate       - Type: Int   | Range: 0 - 6  | Default: 2   | Displayed (and stored) as: below
                                                + 0 - Unlimited | 1 - 30 | 2 - 60 | 3 - 75 | 4 - 120 | 5 - 144 | 6 - 240

"WindowMode"           - Window Mode            - Type: Int   | Range: 0 - 3  | Default: 2   | Displayed as: below
                                                + 0 - Exclusive Full Screen | 1 - Full Screen Window | 2 - Maximized Window | 3 - Windowed
*/

public class SettingsMenu : MonoBehaviour
{
    #region variables

    //All Default values are handles by "SetVarsToDefault"

    #region Video

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
    private int resolutionXTarget = 1920;
    private int resolutionYTarget = 1080;
    #endregion

    #region Framerate
    [Header("Framerate")]
    [Tooltip("Drop Down For Refreshrate")]
    public TMP_Dropdown fpsDropdown;
    private int fpsTarget = 60;
    #endregion

    #region V Sync
    [Header("V-Sync")]
    [Tooltip("Drop Down For V-Sync Toggle")]
    public Toggle vSyncToggle;
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
    public float mouseSensitivityTarget = 2.5f;
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
    [Tooltip("Drop Down For Head Bob Toggle")]
    public Toggle headbobToggle;
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
        SetVarsToDefault();
        LoadSettings();
    }

    //Loads all settings, applying defaults to prefabs if empty, and updating all settings options to display correctly
    public void LoadSettings()
    {
        SetVarsToDefault();

        #region Initialisation

        #region Video

        #region Window Mode
        windowModeTarget = PlayerPrefs.GetInt("WindowMode", windowModeTarget);
        windowModeDropdown.value = windowModeTarget;
        windowModeDropdown.RefreshShownValue();
        #endregion

        #region Resolution
        resolutionXTarget = PlayerPrefs.GetInt("ResolutionX", resolutionXTarget);
        resolutionYTarget = PlayerPrefs.GetInt("ResolutionY", resolutionYTarget);
        switch (resolutionXTarget)
        {
            case 1366:
                resolutionDropdown.value = 0;
                break;
            case 1600:
                resolutionDropdown.value = 1;
                break;
            case 1920:
                resolutionDropdown.value = 2;
                break;
            case 2560:
                resolutionDropdown.value = 3;
                break;
            case 3440:
                resolutionDropdown.value = 4;
                break;
            case 3480:
                resolutionDropdown.value = 5;
                break;
            default:
                resolutionDropdown.value = 2;
                break;
        }
        resolutionDropdown.RefreshShownValue();
        #endregion

        #region Framerate
        fpsTarget = PlayerPrefs.GetInt("RefreshRate", fpsTarget);
        switch (fpsTarget)
        {
            case 0:
                fpsDropdown.value = 0;
                break;
            case 30:
                fpsDropdown.value = 1;
                break;
            case 60:
                fpsDropdown.value = 2;
                break;
            case 75:
                fpsDropdown.value = 3;
                break;
            case 120:
                fpsDropdown.value = 4;
                break;
            case 144:
                fpsDropdown.value = 5;
                break;
            case 240:
                fpsDropdown.value = 5;
                break;
            default:
                fpsDropdown.value = 2;
                break;
        }
        fpsDropdown.RefreshShownValue();
        #endregion

        #region V-Sync
        vSyncTarget = PlayerPrefs.GetInt("VSync", vSyncTarget);
        vSyncToggle.isOn = (vSyncTarget == 1) ? true : false;
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
        headbobToggle.isOn = (headbobTarget == 1) ? true : false;
        #endregion

        #region Crouch Toggle
        crouchToggleTarget = PlayerPrefs.GetInt("CrouchToggle", crouchToggleTarget);
        crouchToggleDropdown.value = crouchToggleTarget;
        #endregion

        #endregion

        #endregion

        SetVideoSettings();


    }

    #region functions

    #region Other

    //Sets all variables to default values
    private void SetVarsToDefault()
    {
        windowModeTarget = 2;
        resolutionXTarget = 1920;
        resolutionYTarget = 1080;
        fpsTarget = 60;
        vSyncTarget = 1;

        audioMasterTarget = 8;
        audioMusicTarget = 10;
        audioSfxTarget = 10;

        mouseSensitivityTarget = 2.5f;
        screenshakeTarget = 10;
        headbobTarget = 1;
        crouchToggleTarget = 0;      
    }

    //Returns all settings to default, but keeps track of high score and attempt count as to not wipe them
    public void ReturnToDefault()
    {
        int SaveHighScore = PlayerPrefs.GetInt("HighScore", 0);
        int SaveAttemptCount = PlayerPrefs.GetInt("AttemptCount", 0);
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("HighScore", SaveHighScore);
        PlayerPrefs.SetInt("AttemptCount", SaveAttemptCount);
        SetVarsToDefault();
        LoadSettings();
    }

    //Applies all settings
    public void ApplySettings()
    {
        PlayerPrefs.SetInt("WindowMode", windowModeTarget);
        PlayerPrefs.SetInt("ResolutionX", resolutionXTarget);
        PlayerPrefs.SetInt("ResolutionY", resolutionYTarget);
        PlayerPrefs.SetInt("RefreshRate", fpsTarget);
        PlayerPrefs.SetInt("VSync", vSyncTarget);
        PlayerPrefs.SetFloat("AudioLevelMaster", audioMasterTarget / 10f);
        PlayerPrefs.SetFloat("AudioLevelMusic", audioMusicTarget / 10f);
        PlayerPrefs.SetFloat("AudioLevelSfx", audioSfxTarget / 10f);
        PlayerPrefs.SetFloat("MouseSensitivity", mouseSensitivityTarget);
        PlayerPrefs.SetInt("ScreenshakeIntensity", screenshakeTarget);
        PlayerPrefs.SetInt("HeadBob", headbobTarget);
        PlayerPrefs.SetInt("CrouchToggle", crouchToggleTarget);

        SetVideoSettings();
    }

    //Sets Resolution, Windowmode, Refreshrate and Vsync.
    public void SetVideoSettings()
    {
        Screen.SetResolution(
            PlayerPrefs.GetInt("ResolutionX", 1920),
            PlayerPrefs.GetInt("ResolutionY", 1080),
            (FullScreenMode)PlayerPrefs.GetInt("WindowMode", 2));

        Application.targetFrameRate = PlayerPrefs.GetInt("RefreshRate", 60);

        QualitySettings.vSyncCount = PlayerPrefs.GetInt("VSync", 1);
    }

    #endregion

    #region Video

    #region Window Mode
    public void WindowModeDropdownUpdate()
    {
        windowModeTarget = windowModeDropdown.value;
    }
    #endregion

    #region Resolution
    public void ResolutionDropdownUpdate()
    {
        SetResXY(resolutionDropdown.value);
    }

    private void SetResXY(int resNumber)
    {
        switch (resNumber)
        {
            case 0:
                resolutionXTarget = 1366;
                resolutionYTarget = 768;
                break;
            case 1:
                resolutionXTarget = 1600;
                resolutionYTarget = 900;
                break;
            case 2:
                resolutionXTarget = 1920;
                resolutionYTarget = 1080;
                break;
            case 3:
                resolutionXTarget = 2560;
                resolutionYTarget = 1440;
                break;
            case 4:
                resolutionXTarget = 3440;
                resolutionYTarget = 1440;
                break;
            case 5:
                resolutionXTarget = 3840;
                resolutionYTarget = 2160;
                break;
        }
    }
    #endregion

    #region Framerate
    public void FramerateDropdownUpdate()
    {
        fpsTarget = GetFramerate(fpsDropdown.value);
    }

    private int GetFramerate(int fpsNumber)
    {
        switch (fpsNumber)
        {
            case 0:
                return 0;
            case 1:
                return 30;
            case 2:
                return 60;
            case 3:
                return 75;
            case 4:
                return 120;
            case 5:
                return 144;
            case 6:
                return 240;
        }
        return 60;
    }
    #endregion

    #region V-Sync
    public void VSyncToggleUpdate()
    {
        vSyncTarget = vSyncToggle.isOn ? 1 : 0;
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
    }
    private void AudioMasterTextUpdate()
    {
        audioMasterTMPText.text = audioMasterTarget.ToString();
    }
    #endregion

    #region Audio Music
    //Audio Music
    public void AudioMusicSliderUpdate()
    {
        audioMusicTarget = (int)audioMusicSlider.value;
        AudioMusicTextUpdate();
    }
    private void AudioMusicTextUpdate()
    {
        audioMusicTMPText.text = audioMusicTarget.ToString();
    }
    #endregion

    #region Audio Sfx
    //Audio Sfx
    public void AudioSfxSliderUpdate()
    {
        audioSfxTarget = (int)audioSfxSlider.value;
        AudioSfxTextUpdate();
    }
    private void AudioSfxTextUpdate()
    {
        audioSfxTMPText.text = audioSfxTarget.ToString();
    }
    #endregion

    #endregion

    #region Game

    #region Mouse Sensitivity
    public void MouseSensitivitySliderUpdate()
    {
        mouseSensitivityTarget = Mathf.Round(mouseSensitivitySlider.value * 10f) / 10f;
        MouseSensitivityTextUpdate();
    }
    private void MouseSensitivityTextUpdate()
    {
        mouseSensitivityTMPText.text = mouseSensitivityTarget.ToString();
    }
    #endregion

    #region Screenshake
    public void ScreenshakeSliderUpdate()
    {
        screenshakeTarget = (int)screenshakeSlider.value;
        ScreenshakeTextUpdate();
    }
    public void ScreenshakeTextUpdate()
    {
        screenshakeTMPText.text = screenshakeTarget.ToString();
    }
    #endregion

    #region Head Bob
    public void HeadbobToggleUpdate()
    {
        headbobTarget = headbobToggle.isOn ? 1 : 0;
    }
    #endregion

    #region Crouch Toggle
    public void CrouchToggleDropdownUpdate()
    {
        crouchToggleTarget = crouchToggleDropdown.value;
    }
    #endregion

    #endregion

    #endregion
}