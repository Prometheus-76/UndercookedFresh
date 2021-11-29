using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Author: Darcy Matheson
// Purpose: Controls the heads up display of the player with globally accessible static variables

public class UserInterfaceHUD : MonoBehaviour
{
    #region Variables

    #region General 
    [Header("General")]

    public Canvas HUDCanvas;

    public static bool showHUD;

    private Transform playerTransform;
    #endregion

    #region Reticles
    [Header("Reticles")]

    public Image microwaveGunReticleImage;
    public Image aSaltRifleReticleImage;
    public Image pepperShotgunReticleImage;
    public Image hitMarkerImage;

    public static float hitMarkerFadeTimer;
    public static float hitMarkerFadeDuration { get; private set; } // Assigned in Awake
    #endregion

    #region Reloading
    [Header("Reloading")]

    public Image reloadProgressImage;
    public Image reloadUnderlayImage;

    public static float reloadProgress; // The progress of the current weapon's reload animation
    #endregion

    #region Wave Counter and Progress
    [Header("Wave Counter and Progress")]

    public Image waveProgressImage;
    public TextMeshProUGUI waveStatusText;
    public TextMeshProUGUI waveProgressText;
    public TextMeshProUGUI skipPromptText;
    public TextMeshProUGUI waveTransitionText;

    public static float waveProgress; // The progress of the current wave
    public static float intermissionProgress; // The progress of the current intermission
    public static int waveNumber; // The number of the current wave
    public static int intermissionDuration; // The length in seconds of the intermission
    public static float waveChangeEffectDuration { get; private set; } // Set in Awake
    public static float waveChangeEffectTimer;
    #endregion

    #region Player Health
    [Header("Player Health")]

    public static int playerCurrentHealth;
    public static int playerMaxHealth;

    public Image healthProgressImage;
    public Image healthDelayedProgressImage;
    public TextMeshProUGUI healthText;
    private string colourCodeHealthBar;
    #endregion

    #region Weapon Details
    [Header("Weapon Details")]

    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI currentWeaponText;
    public Image weaponDividerImage;
    public Image weaponIconMicrowaveGun;
    public Image weaponIconASaltRifle;
    public Image weaponIconPepperShotgun;

    public static int ammoInMagazine;
    public static int ammoInReserves;
    public static int equippedWeapon;
    #endregion

    #region Ultimate Ability
    [Header("Ultimate Ability")]

    public GameObject superDisplayParent;
    public Image minorFillImage;
    public Image majorFillImage;
    public TextMeshProUGUI ultimateProgressText;

    public static float ultimateCharge;
    #endregion

    #region Run Timer
    [Header("Run Timer")]

    public TextMeshProUGUI runDurationText;

    public static float currentRunDuration;
    #endregion

    #region Score and Fibre
    [Header("Score and Fibre")]

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI fibreText;

    public static ulong currentScore;
    public static ulong currentFibre;
    #endregion

    #region Interaction
    [Header("Interaction")]

    public TextMeshProUGUI interactPromptText;
    public TextMeshProUGUI interactCostText;
    public Image interactUnderlayImage;
    public Image interactProgressImage;

    public static float interactProgress;
    public static int interactCost;
    public static string interactPrompt;
    public static bool interactPresent;
    public static bool interactHasDuration;
    public static bool displayInteractKeybind;

    #endregion

    #region Hazards
    [Header("Hazards")]

    public RectTransform directionalIndicatorTransform;
    public Image directionalIndicatorImage;

    public static Vector3 damageOrigin;
    public static float damageFlashDuration { get; private set; } // Assigned manually in awake
    public static float damageFlashTimer;

    private float smoothingSpeed;
    private Quaternion indicatorTargetRotation;

    #endregion

    #endregion

    private void Awake()
    {
        // Static assignment
        showHUD = true;
        hitMarkerFadeDuration = 0.3f;
        hitMarkerFadeTimer = 0f;
        reloadProgress = 0f;
        equippedWeapon = -1;
        waveProgress = 0f;
        intermissionProgress = 0f;
        waveNumber = 1;
        intermissionDuration = 0;
        waveChangeEffectDuration = 3f;
        waveChangeEffectTimer = 0f;
        playerCurrentHealth = 100;
        playerMaxHealth = 100;
        ammoInMagazine = 0;
        ammoInMagazine = 0;
        equippedWeapon = -1;
        ultimateCharge = 0f;
        currentRunDuration = 0f;
        currentScore = 0;
        currentFibre = 0;
        interactProgress = 0f;
        interactCost = 0;
        interactPrompt = "";
        interactPresent = false;
        interactHasDuration = false;
        displayInteractKeybind = true;
        damageOrigin = Vector3.zero;
        damageFlashDuration = 1.5f;
        damageFlashTimer = 0f;
    }

    // Start is called before the first frame update
    void Start()
    {
        #region Initialisation

        // Grab the colour of the health bar and automatically use it for text
        colourCodeHealthBar = ColorUtility.ToHtmlStringRGBA(healthProgressImage.color);

        indicatorTargetRotation = Quaternion.identity;

        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        #region General

        HUDCanvas.enabled = showHUD;

        #endregion

        if (showHUD)
        {
            #region Hit Marker

            if (hitMarkerFadeTimer > 0f)
            {
                // Fade in/out
                float fadeProgress = (hitMarkerFadeTimer / hitMarkerFadeDuration);
                Color hitMarkerColour = Color.white;
                hitMarkerColour.a = HitMarkerFade(fadeProgress) * 0.7f;
                hitMarkerImage.color = hitMarkerColour;
                hitMarkerFadeTimer -= Time.deltaTime;

                // Turn the hit marker on
                hitMarkerImage.enabled = true;
            }
            else
            {
                // Turn the hit marker off as it isn't being used
                hitMarkerImage.enabled = false;
            }

            #endregion

            #region Reload Progress

            // If the weapon is reloading
            if (Mathf.Clamp01(reloadProgress) > 0f)
            {
                // Hide reticle
                microwaveGunReticleImage.enabled = false;
                aSaltRifleReticleImage.enabled = false;
                pepperShotgunReticleImage.enabled = false;
                hitMarkerFadeTimer = 0f;

                // Show reload progress
                reloadProgressImage.enabled = true;
                reloadUnderlayImage.enabled = true;

                // Update progress
                reloadProgressImage.fillAmount = Mathf.Clamp01(reloadProgress);
            }
            else
            {
                if (Movement.isGrappling == false)
                {
                    // Show reticle
                    switch (equippedWeapon)
                    {
                        case -1:
                            microwaveGunReticleImage.enabled = true;
                            aSaltRifleReticleImage.enabled = false;
                            pepperShotgunReticleImage.enabled = false;
                            break;
                        case 0:
                            microwaveGunReticleImage.enabled = true;
                            aSaltRifleReticleImage.enabled = false;
                            pepperShotgunReticleImage.enabled = false;
                            break;
                        case 1:
                            aSaltRifleReticleImage.enabled = true;
                            microwaveGunReticleImage.enabled = false;
                            pepperShotgunReticleImage.enabled = false;
                            break;
                        case 2:
                            pepperShotgunReticleImage.enabled = true;
                            microwaveGunReticleImage.enabled = false;
                            aSaltRifleReticleImage.enabled = false;
                            break;
                    }
                }
                else
                {
                    // Hide reticle
                    microwaveGunReticleImage.enabled = false;
                    aSaltRifleReticleImage.enabled = false;
                    pepperShotgunReticleImage.enabled = false;
                }

                // Hide reload progress
                reloadProgressImage.enabled = false;
                reloadUnderlayImage.enabled = false;
            }

            #endregion

            #region Wave Counter and Progress

            skipPromptText.enabled = (WaveManager.waveActive == false && WaveManager.gameStarted);

            // Set the wave complete / wave starting text prompts
            if (waveChangeEffectTimer > 0f)
            {
                // Calculate opacity
                float progress = Mathf.Clamp01(waveChangeEffectTimer / waveChangeEffectDuration);
                float textAlpha = WaveChangeEffectCurve(progress);
                Color textColour = Color.white;
                textColour.a = textAlpha;

                // Set text based on circumstance
                waveTransitionText.text = (WaveManager.waveActive == true) ? ("<b>Stage " + WaveManager.waveNumber) : ("<b>Stage " + WaveManager.waveNumber + " Complete!\n<size=40%></b>Upgrade Station Activated");
                waveTransitionText.color = textColour;

                // Turn the text on
                waveTransitionText.enabled = true;
                waveChangeEffectTimer -= Time.deltaTime;
            }
            else
            {
                // Turn the text off
                waveChangeEffectTimer = 0f;
                waveTransitionText.enabled = false;
            }

            // Set the wave progress bar and relevant text
            if (intermissionProgress <= 0)
            {
                waveProgressImage.fillAmount = Mathf.Clamp01(waveProgress);
                waveProgressText.text = (Mathf.Clamp01(waveProgress) * 100f).ToString("F1") + "%";
                waveStatusText.text = "Stage " + waveNumber;
            }
            else
            {
                waveProgressImage.fillAmount = Mathf.Clamp01(1f - intermissionProgress);
                waveProgressText.text = (Mathf.Clamp01(1f - intermissionProgress) * intermissionDuration).ToString("F1") + "s";
                waveStatusText.text = "Prepare";
            }

            #endregion

            #region Player Health

            healthText.text = "<color=#" + colourCodeHealthBar + ">" + playerCurrentHealth + "<size=50%><color=white> / " + playerMaxHealth;
            healthProgressImage.fillAmount = Mathf.Clamp01((float)playerCurrentHealth / (float)playerMaxHealth);

            // Update delayed fill
            if (healthProgressImage.fillAmount < healthDelayedProgressImage.fillAmount)
            {
                // Delay rate is faster when the difference between current and delayed is higher
                float newFillAmount = Mathf.Clamp01(healthDelayedProgressImage.fillAmount - ((healthDelayedProgressImage.fillAmount - healthProgressImage.fillAmount) * 5f * Time.deltaTime));
                healthDelayedProgressImage.fillAmount = (newFillAmount - healthProgressImage.fillAmount > 0.002f) ? newFillAmount : healthProgressImage.fillAmount;
            }
            else
            {
                // Reset delayed fill under current fill when regenerating health
                healthDelayedProgressImage.fillAmount = healthProgressImage.fillAmount;
            }

            #endregion

            #region Weapon Details

            // Ammo display
            if ((ammoInReserves > 0 && ammoInReserves != -1) || ammoInReserves == -1 || ammoInMagazine > 0)
            {
                // There is ammo in the weapon
                ammoText.text = ammoInMagazine + " <size=60%><color=#FFFFFFAA>/ " + (ammoInReserves == -1 ? "∞" : ammoInReserves.ToString());
            }
            else
            {
                // There is no ammo in the magazine or in reserves
                ammoText.text = "Empty";
            }

            // Weapon icon
            switch (equippedWeapon)
            {
                case -1:
                    ammoText.enabled = false;
                    weaponDividerImage.enabled = false;
                    currentWeaponText.enabled = false;
                    currentWeaponText.text = "None";
                    weaponIconMicrowaveGun.enabled = false;
                    weaponIconASaltRifle.enabled = false;
                    weaponIconPepperShotgun.enabled = false;
                    break;
                case 0:
                    ammoText.enabled = true;
                    weaponDividerImage.enabled = true;
                    currentWeaponText.enabled = true;
                    currentWeaponText.text = "Microwave Gun";
                    weaponIconMicrowaveGun.enabled = true;
                    weaponIconASaltRifle.enabled = false;
                    weaponIconPepperShotgun.enabled = false;
                    break;
                case 1:
                    ammoText.enabled = true;
                    weaponDividerImage.enabled = true;
                    currentWeaponText.enabled = true;
                    currentWeaponText.text = "A-Salt Rifle";
                    weaponIconMicrowaveGun.enabled = false;
                    weaponIconASaltRifle.enabled = true;
                    weaponIconPepperShotgun.enabled = false;
                    break;
                case 2:
                    ammoText.enabled = true;
                    weaponDividerImage.enabled = true;
                    currentWeaponText.enabled = true;
                    currentWeaponText.text = "Pepper Shotgun";
                    weaponIconMicrowaveGun.enabled = false;
                    weaponIconASaltRifle.enabled = false;
                    weaponIconPepperShotgun.enabled = true;
                    break;
            }

            #endregion

            #region Ultimate Ability

            superDisplayParent.SetActive(WeaponCoordinator.knifeUnlocked);

            float ultimateChargePercent = (ultimateCharge * 100f);

            minorFillImage.fillAmount = (ultimateCharge * (25f / 29f)) + (((Mathf.FloorToInt(ultimateChargePercent) - Mathf.FloorToInt(ultimateChargePercent) % 20f) / 20f) * (1f / 29f));
            float majorFillTarget = ((float)(Mathf.FloorToInt(ultimateChargePercent) - (Mathf.FloorToInt(ultimateChargePercent) % 20f)) / 100f);
        
            // Lerp between major fill targets
            if (majorFillImage.fillAmount < majorFillTarget)
            {
                majorFillImage.fillAmount += 0.6f * Time.deltaTime;
                if (majorFillImage.fillAmount > majorFillTarget)
                {
                    // Snap to major fill target
                    majorFillImage.fillAmount = majorFillTarget;
                }
            }

            if (majorFillImage.fillAmount > ultimateCharge)
            {
                majorFillImage.fillAmount = ultimateCharge;
            }

            ultimateProgressText.text = (ultimateCharge < 1f ? ultimateChargePercent.ToString("F1") + "<size=70%>" : "Charged!");

            #endregion

            #region Run Timer

            // The exact time minus the closest floored whole value
            int decimalSeconds = Mathf.FloorToInt((currentRunDuration - Mathf.Floor(currentRunDuration)) * 100f);

            // The floored excess leading up to 60
            int seconds = Mathf.FloorToInt(currentRunDuration % 60f);

            // The floored excess leading up to 3600, minus the second precision and scaled down to within 60 second range
            int minutes = (Mathf.FloorToInt(currentRunDuration % 3600f) - seconds) / 60;

            // The floored timer minus the second-values of seconds and minutes, scaled down to a 3600 second range
            int hours = (Mathf.FloorToInt(currentRunDuration) - (minutes * 60) - (seconds)) / 3600;

            // Format and display string
            runDurationText.text = hours.ToString("D2") + ":" + minutes.ToString("D2") + ":" + seconds.ToString("D2") + "<size=50%><voffset=0.7em><color=#FFFFFF64>." + decimalSeconds.ToString("D2");

            #endregion

            #region Score and Fibre

            string scoreString = currentScore.ToString("D7");
            string tempString = "Score: <b><color=#FFFFFF64>";        
        
            bool startFound = false;
            for (int i = 0; i < scoreString.Length; i++)
            {
                if (scoreString[i] == '0' && startFound == false)
                {
                    tempString += '0';
                }
                else
                {
                    if (startFound == false)
                    {
                        startFound = true;
                        tempString += "<color=white>";
                    }

                    tempString += scoreString[i];
                }
            }

            // Assign completed string
            scoreString = tempString;

            string fibreString = currentFibre.ToString("D5");
            tempString = "F<voffset=0.12em>ibre: </voffset><b><color=#FFFFFF64>";

            startFound = false;
            for (int i = 0; i < fibreString.Length; i++)
            {
                if (fibreString[i] == '0' && startFound == false)
                {
                    tempString += '0';
                }
                else
                {
                    if (startFound == false)
                    {
                        startFound = true;
                        tempString += "<color=white>";
                    }

                    tempString += fibreString[i];
                }
            }

            // Assign completed string
            fibreString = tempString;

            // Assign to text UI
            scoreText.text = scoreString;
            fibreText.text = fibreString;

            #endregion

            #region Interaction

            if (interactProgress > 0f && interactProgress < 1f)
            {
                // Show and update progress bar
                interactProgressImage.enabled = true;
                interactUnderlayImage.enabled = true;

                interactProgressImage.fillAmount = interactProgress;
            }
            else
            {
                interactProgressImage.enabled = false;
                interactUnderlayImage.enabled = false;
            }
        
            if (interactPresent)
            {
                // Show interact text
                interactPromptText.enabled = true;
                interactPromptText.text = interactPrompt + ((displayInteractKeybind) ? " (" + (interactHasDuration ? "Hold " : "") + KeyCode.E.ToString().ToLower() + ")" : "");

                if (interactCost > 0)
                {
                    interactCostText.enabled = true;
                    interactCostText.text = interactCost.ToString() + " Fibre";
                }
            }
            else
            {
                // Hide UI
                interactPromptText.enabled = false;
                interactCostText.enabled = false;

                interactProgressImage.enabled = false;
                interactUnderlayImage.enabled = false;
            }

            #endregion

            #region Hazards

            if (damageFlashTimer > 0f)
            {
                // Which way is the player facing?
                Vector3 playerForward = playerTransform.forward;
                playerForward.y = 0f;

                Vector3 playerRight = playerTransform.right;
                playerRight.y = 0f;
                
                // Where is the source of the damage?
                Vector3 globalDamageDirection = damageOrigin - playerTransform.position;
                globalDamageDirection.y = 0f;
                globalDamageDirection.Normalize();

                // How close is the player to looking at the damage?
                float lookHazardDot = Vector3.Dot(playerForward, globalDamageDirection);
                lookHazardDot = 1f - (2f * (Mathf.Acos(lookHazardDot)) / Mathf.PI); // Map circular Dot() to linear angle delta
                float rotationAngleDegrees = ((1f - lookHazardDot) / 2f) * 180f;

                // Account for left/right in dot product magnitude (1 is on the right (default), -1 is on the left)
                int hazardSide = (Vector3.Dot(playerRight, globalDamageDirection) > 0f) ? 1 : -1;
                rotationAngleDegrees *= -hazardSide;

                // Slowly rotate towards the hazard source
                rotationAngleDegrees = (float.IsNaN(rotationAngleDegrees)) ? 0f : rotationAngleDegrees; // Prevent stupid quaternion witchcraft
                indicatorTargetRotation = Mathf.Approximately(rotationAngleDegrees, 0f) ? Quaternion.identity : Quaternion.Euler(0f, 0f, rotationAngleDegrees);
                directionalIndicatorTransform.localRotation = Quaternion.Lerp(directionalIndicatorTransform.localRotation, indicatorTargetRotation, Time.deltaTime * 15f);

                // Fade in/out
                float fadeProgress = (damageFlashTimer / damageFlashDuration);
                Color indicatorColour = directionalIndicatorImage.color;
                indicatorColour.a = DamageIndicatorFade(fadeProgress);
                directionalIndicatorImage.color = indicatorColour;
                damageFlashTimer -= Time.deltaTime;

                // Turn the indicator on
                directionalIndicatorImage.enabled = true;
            }
            else
            {
                // Turn the indicator off as it isn't being used
                directionalIndicatorImage.enabled = false;
            }

            #endregion
        }
    }

    // Used by the damage indicator to control the opacity over time
    float DamageIndicatorFade(float progress)
    {
        float result = 0f;

        if (progress < 0.25f)
        {
            result = 16f * Mathf.Pow(progress, 2f);
        }
        else
        {
            result = 1f;
        }

        result = Mathf.Clamp01(result);
        return result;
    }

    // Used by the hit marker to control the opacity over time
    float HitMarkerFade(float progress)
    {
        float result = 0f;

        result = Mathf.Pow(progress, 2f);

        result = Mathf.Clamp01(result);
        return result;
    }

    // Ease in/out from 0 - 1 and back, linger for roughly half the total duration between transitions
    public float WaveChangeEffectCurve(float progress)
    {
        float result = 0f;

        if (progress > 0.8f)
        {
            result = 1f - (25f * Mathf.Pow(progress - 0.8f, 2f));
        }
        else if (progress < 0.25f)
        {
            result = 1f - (16f * Mathf.Pow(progress - 0.25f, 2f));
        }
        else
        {
            result = 1f;
        }

        return result;
    }
}
