using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author: Darcy Matheson
// Purpose: Controls the interaction system, player's health, OSP thresholds, ultimate charge, run duration and other related stats

public class PlayerStats : MonoBehaviour
{
    #region Variables

    #region Internal

    [HideInInspector]
    public float currentRunTime { get; private set; }
    [HideInInspector]
    public ulong currentScore { get; private set; }
    [HideInInspector]
    public ulong currentFibre;
    [HideInInspector]
    public int maxHealth;
    [HideInInspector]
    public int currentHealth;
    private float immuneRegenerationTimer;
    private float regenerationTimer;
    private float ultimateCharge;
    public float difficultyLevel { get; private set; }
    [HideInInspector]
    public int playerLevel;
    public static bool gamePaused;
    public static bool allowUltimateGeneration;
    private float interactTimer;
    private bool interactPresent;
    public static bool isAlive;
    [HideInInspector]
    public int attemptCount;
    [HideInInspector]
    public int enemiesKilled;
    [HideInInspector]
    public int damageDealt;
    [HideInInspector]
    public int highscore;

    #endregion

    #region Parameters

    #region Health and Regeneration
    [Header("Health and Regeneration")]

    [Tooltip("How much health the player starts off with."), Range(50, 200)]
    public int baseHealth = 100;
    [Tooltip("How much more health the player gains with each upgrade."), Range(1f, 2f)]
    public float healthMultiplierPerLevel = 1.2f;
    [Tooltip("How long the player must go without taking damage before health regeneration can begin."), Range(5f, 30f)]
    public float timeBeforeRegeneration = 10f;
    [Tooltip("How much health the player gains every second while regenerating."), Range(0.1f, 1f)]
    public float percentRegenerationPerSecond = 1f;
    [Tooltip("How much fibre each health point costs to restore."), Range(0.1f, 1f)]
    public float fibrePerHealthPoint = 0.2f;
    [Tooltip("How much the first health upgrade costs."), Range(10, 50)]
    public int healthUpgradeCostBase = 30;
    [Tooltip("How much more each health upgrade costs than the previous one."), Range(1f, 2f)]
    public float healthUpgradeCostMultiplierPerLevel = 1.3f;
    [Tooltip("The player must be above this percent health for one-shot protection to be active."), Range(70, 99)]
    public int maxOneShotProtectionThreshold = 90;
    [Tooltip("The current health percent the player will be set to when one-shot protection activates."), Range(5, 30)]
    public int minOneShotProtectionThreshold = 10;

    #endregion

    #region Difficulty
    [Header("Difficulty")]

    [Tooltip("How much the waves completed contributes to the increase of difficulty."), Range(0.1f, 2f)]
    public float difficultyWaveFactor = 0.5f;
    [Tooltip("How much the time duration contributes to the increase of difficulty."), Range(0.1f, 2f)]
    public float difficultyTimeFactor = 0.25f;

    #endregion

    #region Other
    [Header("Other")]

    [Tooltip("How quickly the ultimate charges up without dealing damage."), Range(0.001f, 0.01f)]
    public float passiveUltimateChargeRate = 0.002f;
    [Tooltip("How much screenshake is given based on the damage taken relative to maximum health (higher = more for less damage)."), Range(1f, 10f)]
    public float screenshakeDamageScaler = 3f;

    #endregion

    #region Interaction
    [Header("Interaction")]

    [Tooltip("How close the player must be to an interactive object to use it."), Range(2f, 6f)]
    public float interactRange = 4f;
    [Tooltip("The layer which interactive objects are on.")]
    public LayerMask interactiveLayer;
    [Tooltip("The layers which can interrupt the line of sight to an interactive object.")]
    public LayerMask environmentLayers;

    #endregion

    #endregion

    #region Components
    [Header("Components")]

    public Movement playerMovement;
    public GunController[] gunControllers;

    private Transform mainCameraTransform;
    private InteractiveObject currentInteraction;

    #endregion

    #endregion

    private void Awake()
    {
        // Static assignment
        gamePaused = false;
        allowUltimateGeneration = true;
        isAlive = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        #region Initialisation

        maxHealth = baseHealth;
        currentHealth = maxHealth;

        currentScore = 0;
        currentFibre = 0;

        playerLevel = 1;

        mainCameraTransform = Camera.main.GetComponent<Transform>();
        interactTimer = 0f;

        #endregion

        #region Get Stats

        attemptCount = PlayerPrefs.GetInt("AttemptCount", 0);
        highscore = PlayerPrefs.GetInt("Highscore", 0);

        #endregion
    }

    // Take damage
    public void TakeDamage(int damage)
    {
        int damageTaken = damage;

        // If the player is above the OSP threshold
        if (((float)currentHealth / (float)maxHealth) >= (float)maxOneShotProtectionThreshold / 100f)
        {
            // If the damage brings the player below the minimum threshold
            if (currentHealth - damage < maxHealth * ((float)minOneShotProtectionThreshold / 100f))
            {
                damageTaken = currentHealth - Mathf.CeilToInt(maxHealth * ((float)minOneShotProtectionThreshold / 100f));
            }
        }

        // Take damage
        currentHealth -= damageTaken;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Refresh timer
        immuneRegenerationTimer = timeBeforeRegeneration;
        regenerationTimer = 0f;

        // If the player has run out of health
        if (currentHealth <= 0 && isAlive)
        {
            Die();
            CameraController.AddTrauma(1f);
        }

        // Screenshake if sufficient damage is dealt in an instance
        float shakeTrauma = ((float)damageTaken / maxHealth);
        CameraController.AddTrauma(shakeTrauma * screenshakeDamageScaler);
    }

    void Die()
    {
        isAlive = false;
        gamePaused = true;

        ScoreScreenHUD.showHUD = true;
        UserInterfaceHUD.showHUD = false;
        UpgradeStationHUD.showHUD = false;
        PauseMenuHUD.showHUD = false;

        Cursor.lockState = CursorLockMode.None;

        // Save stats
        attemptCount += 1;
        PlayerPrefs.SetInt("AttemptCount", attemptCount);
        highscore = Mathf.RoundToInt(Mathf.Max(currentScore, highscore));
        PlayerPrefs.SetInt("Highscore", highscore);
    }

    void Update()
    {
        // If the timer isn't paused (between rounds or when game is paused)
        if (gamePaused == false)
        {
            // If the game has started (due to picking up the first weapon)
            if (WaveManager.gameStarted)
            {
                if (WaveManager.waveActive)
                {
                    // Add unscaled time
                    currentRunTime += Time.deltaTime;
                    
                    // Allow passive ultimate charge
                    AddUltimateCharge(Time.deltaTime * passiveUltimateChargeRate * 100f);

                    // Passive health regeneration
                    if (currentHealth > 0)
                    {
                        if (immuneRegenerationTimer > 0f)
                        {
                            immuneRegenerationTimer -= Time.deltaTime;
                        }
                        else
                        {
                            immuneRegenerationTimer = 0f;

                            if (regenerationTimer < 0f)
                            {
                                // Add some health
                                int regeneratedHealth = Mathf.Min(Mathf.CeilToInt((percentRegenerationPerSecond / 100f) * maxHealth), maxHealth - currentHealth);
                                currentHealth += regeneratedHealth;
                                regenerationTimer = 1f;
                            }
                            else
                            {
                                regenerationTimer -= Time.deltaTime;
                            }
                        }
                    }
                }
                else
                {
                    currentRunTime += Time.deltaTime * (Input.GetKey(KeyCode.Tab) ? 20f : 1f);
                }
            }

            #region Item Interaction

            interactPresent = false;
            Collider[] interactsInProximity = Physics.OverlapSphere(mainCameraTransform.position, interactRange, interactiveLayer);
            if (interactsInProximity.Length > 0)
            {
                RaycastHit interactHit;
                Physics.Raycast(mainCameraTransform.position, mainCameraTransform.forward, out interactHit, 10f + interactRange, interactiveLayer);

                bool aimingAtColliderInProximity = false;
                for (int i = 0; i < interactsInProximity.Length; i++)
                {
                    if (interactsInProximity[i] == interactHit.collider)
                    {
                        aimingAtColliderInProximity = true;
                        break;
                    }
                }
                
                #region Raycast To Interact

                if (interactHit.transform != null && aimingAtColliderInProximity && Physics.Linecast(mainCameraTransform.position, interactHit.point, environmentLayers) == false)
                {
                    // Cache interactive object script
                    currentInteraction = interactHit.transform.gameObject.GetComponent<InteractiveObject>();

                    if (currentInteraction.isInteractable)
                    {
                        // Display interact info
                        interactPresent = true;

                        if (Input.GetKeyDown(KeyCode.E) && (ulong)currentInteraction.GetFibreCost() <= currentFibre)
                        {
                            // Start interaction timer
                            if (currentInteraction != null)
                            {
                                interactTimer = currentInteraction.interactDuration;

                                // If interact is instant
                                if (interactTimer <= 0f)
                                {
                                    currentInteraction.Interact();
                                    currentFibre -= (ulong)currentInteraction.GetFibreCost();
                                    interactTimer = 0f;
                                }
                            }
                        }                        
                    }
                }

                #endregion

                #region Overlap To Interact

                if (aimingAtColliderInProximity == false)
                {
                    Collider[] interactsOverlapping = Physics.OverlapSphere(mainCameraTransform.position, 0.4f, interactiveLayer);

                    if(interactsOverlapping.Length > 0)
                    {
                        // Cache interactive object script
                        currentInteraction = interactsOverlapping[0].transform.gameObject.GetComponent<InteractiveObject>();

                        if (currentInteraction.isInteractable)
                        {
                            // Display interact info
                            interactPresent = true;

                            if (Input.GetKeyDown(KeyCode.E) && (ulong)currentInteraction.GetFibreCost() <= currentFibre)
                            {
                                // Start interaction timer
                                if (currentInteraction != null)
                                {
                                    interactTimer = currentInteraction.interactDuration;

                                    // If interact is instant
                                    if (interactTimer <= 0f)
                                    {
                                        currentInteraction.Interact();
                                        currentFibre -= (ulong)currentInteraction.GetFibreCost();
                                        interactTimer = 0f;
                                    }
                                }
                            }
                        }
                    }
                }

                #endregion
            }

            // Progress timer
            if (Input.GetKey(KeyCode.E) && interactTimer > 0f && interactPresent)
            {
                interactTimer -= Time.deltaTime;

                // The wait timer has passed
                if (interactTimer <= 0f)
                {
                    currentInteraction.Interact();
                    currentFibre -= (ulong)currentInteraction.GetFibreCost();
                    interactTimer = 0f;
                }
            }
            else
            {
                // Interrupt wait timer
                interactTimer = 0f;
            }

            #endregion
        }

        #region Update UI

        UserInterfaceHUD.playerCurrentHealth = currentHealth;    
        UserInterfaceHUD.playerMaxHealth = maxHealth;

        UserInterfaceHUD.ultimateCharge = Mathf.Floor(Mathf.Clamp01(ultimateCharge) * 1000f) / 1000f;

        UserInterfaceHUD.currentRunDuration = currentRunTime;

        UserInterfaceHUD.currentScore = currentScore;
        UserInterfaceHUD.currentFibre = currentFibre;

        UserInterfaceHUD.interactPresent = interactPresent;
        UserInterfaceHUD.interactHasDuration = (currentInteraction != null && currentInteraction.interactDuration > 0f);
        
        if (interactPresent && currentInteraction != null)
        {
            UserInterfaceHUD.interactProgress = Mathf.Clamp01(1f - (interactTimer / currentInteraction.interactDuration));
            UserInterfaceHUD.interactPrompt = currentInteraction.interactPrompt;
            UserInterfaceHUD.interactCost = currentInteraction.GetFibreCost();
        }

        #endregion
    }

    public void UpgradeHealth()
    {
        ulong upgradeCost = (ulong)CalculateHealthUpgradeCost();
        if (upgradeCost <= currentFibre)
        {
            int healthMissing = maxHealth - currentHealth;

            maxHealth = CalculateHealthUpgrade();
            currentHealth = Mathf.CeilToInt(maxHealth - healthMissing);
        
            playerLevel += 1;
            currentFibre -= upgradeCost;
        }
    }

    public int CalculateHealthUpgrade()
    {
        float newMaxHealth = baseHealth;
        for (int i = 0; i < playerLevel; i++)
        {
            newMaxHealth *= healthMultiplierPerLevel;
        }

        return Mathf.CeilToInt(newMaxHealth);
    }

    public int CalculateHealthUpgradeCost()
    {
        float scaledCost = healthUpgradeCostBase;
        for (int i = 0; i < playerLevel - 1; i++)
        {
            scaledCost *= healthUpgradeCostMultiplierPerLevel;
        }

        return Mathf.CeilToInt(scaledCost);
    }

    public void AddUltimateCharge(float chargeAmount)
    {
        if (allowUltimateGeneration && WeaponCoordinator.knifeUnlocked)
        {
            ultimateCharge += chargeAmount / 100f;
            ultimateCharge = Mathf.Clamp01(ultimateCharge);
        }
    }

    public bool GetSuperStatus()
    {
        return ultimateCharge >= 1f;
    }

    public void ConsumeSuper()
    {
        ultimateCharge = 0f;
    }

    public void UpgradeWeapon(int weaponIndex)
    {
        gunControllers[weaponIndex].UpgradeWeapon();
    }

    public void CalculateDifficultyLevel()
    {
        if (currentRunTime >= 0f && WaveManager.waveNumber > 0)
        {
            float waveDifficulty = ((float)WaveManager.waveNumber * difficultyWaveFactor);
            float timeDifficulty = (currentRunTime * (difficultyTimeFactor / 60f));

            difficultyLevel = waveDifficulty + timeDifficulty;
        }
    }

    public void AddScore(int baseScore)
    {
        currentScore += (ulong)Mathf.CeilToInt(baseScore + (baseScore * difficultyLevel));
    }

    public void AddFibre(int baseFibre)
    {
        currentFibre += (ulong)Mathf.CeilToInt(baseFibre + (baseFibre * difficultyLevel * 0.1f));
    }
}
