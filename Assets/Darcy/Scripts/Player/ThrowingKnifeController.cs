using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Author: Darcy Matheson
// Purpose: Controls the movement of the knife ultimate, as well as its other attributes like healing on kill

public class ThrowingKnifeController : MonoBehaviour
{
    #region Variables

    #region Internal

    private Vector3 lastBouncePosition;
    public static bool isAirborn { get; private set; }
    public static int maxBounces;
    private int bounceCount;
    private float bounceTimer;
    private float airtimeTimer;
    private float bounceDuration;
    private float halfTargetHeight;
    private float lingerTimer;
    private bool isStowing;
    private bool isDrawing;
    private bool isStowed;
    private bool isFullyDrawn;
    private float switchTimer;

    #endregion

    #region Parameters

    #region Lifetime
    [Header("Lifetime")]

    [Tooltip("The maximum number of enemies in the wave the knife is able to eliminate (scales with wave size)."), Range(0f, 1f)]
    public float waveKillPotential = 0.3f;
    [Tooltip("The maximum bounce distance between targets."), Range(20f, 70f)]
    public float bounceDetectionRadius = 30f;
    #endregion

    #region Speed
    [Header("Speed")]

    [Tooltip("How quickly the knife moves through the air."), Range(20f, 50f)]
    public float moveSpeed = 30f;
    [Tooltip("How quickly the knife rotates while flying through the air."), Range(2f, 6f)]
    public float spinSpeed = 4f;
    #endregion

    #region Other
    [Header("Other")]

    [Tooltip("The maximum distance the knife can lock on to a target when first thrown."), Range(20f, 50f)]
    public float throwDetectionRadius = 50f;
    [Tooltip("How closely you are required to aim at your throw target for a lock on."), Range(0.4f, 0.9f)]
    public float minimumThrowSimilarity = 0.8f;
    [Tooltip("The percentage of the player's full health which is restored with each knife kill."), Range(1, 10)]
    public int healthPercentRestorePerKill = 5;
    #endregion

    #region Draw / Stow
    [Header("Draw / Stow")]

    public Vector3 stowedPosition; // Default: (0f, -0.5f, -0.5f)
    [Tooltip("How long it takes to full draw/stow the knife."), Range(0.3f, 1f)]
    public float switchDuration = 0.3f;
    [Tooltip("How long the knife lingers to let the trail dissipate before returning to the player's hand."), Range(1f, 3f)]
    public float lingerDuration = 1f;
    #endregion

    #region Configuration
    [Header("Configuration")]

    [Tooltip("The layers which should block bouncing line of sight for the knife.")]
    public LayerMask wallLayers;
    [Tooltip("The layer which all enemies are on.")]
    public LayerMask enemyLayers;
    #endregion

    #endregion

    #region Components
    [Header("Components")]

    public Transform holderTransform;
    public Transform pivotTransform;
    public GameObject knifeModel;
    private Transform currentTargetTransform;
    private Enemy currentTargetEnemy;

    public TrailRenderer bladeTrail;
    public TrailRenderer handleTrail;
    public Transform parentHolsterTransform;

    private Transform mainCameraTransform;
    private Transform preferredThrowPoint;

    private PlayerStats playerStats;

    #endregion

    #endregion

    private void Awake()
    {
        // Static assignment
        isAirborn = false;
        maxBounces = 0;
    }

    private void Start()
    {
        #region Initialisation

        bounceCount = 0;

        mainCameraTransform = Camera.main.transform;

        isStowing = false;
        isDrawing = false;
        isFullyDrawn = false;
        isStowed = true;
        holderTransform.localPosition = stowedPosition;
        knifeModel.SetActive(false);

        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();

        #endregion
    }

    void Update()
    {
        #region Input 

        if (isAirborn == false && lingerTimer <= 0f )
        {
            if (Input.GetKey(KeyCode.Q) && playerStats.GetSuperStatus() && PlayerStats.gamePaused == false)
            {
                if (isDrawing == false && isFullyDrawn == false)
                {
                    DrawKnife();
                }
            }
            else if (isFullyDrawn)
            {
                if (isFullyDrawn && preferredThrowPoint != null)
                {
                    Throw(preferredThrowPoint);
                }
                else if (isAirborn == false && isStowing == false && isStowed == false)
                {
                    StowKnife();
                }
            }
        }

        #endregion

        #region Stow / Draw Animation

        if (isFullyDrawn == false && isStowed == false)
        {
            // Animate the weapon stow/draw
            if (isStowing)
            {
                switchTimer -= Time.deltaTime;
                
                float progress = 1f - WeaponSwitchCurve(Mathf.Clamp01(switchTimer / switchDuration));
                Vector3 newPosition = Vector3.Lerp(Vector3.zero, stowedPosition, progress);
                holderTransform.localPosition = newPosition;

                // If the animation is completed
                if (progress >= 1f)
                {
                    isStowed = true;
                    isStowing = false;

                    knifeModel.SetActive(false);
                }
            }
            else if (isDrawing)
            {
                knifeModel.SetActive(true);
                switchTimer -= Time.deltaTime;

                float progress = 1f - WeaponSwitchCurve(Mathf.Clamp01(switchTimer / switchDuration));
                Vector3 newPosition = Vector3.Lerp(stowedPosition, Vector3.zero, progress);
                holderTransform.localPosition = newPosition;

                // If the animation is completed
                if (progress >= 1f)
                {
                    isFullyDrawn = true;
                    isDrawing = false;
                }
            }

        }

        #endregion

        #region Linger After Bounce End

        // The period in which the knife stays at the last target to let the trails catch up, before coming back to the player
        if (lingerTimer > 0f)
        {
            lingerTimer -= Time.deltaTime;
            knifeModel.SetActive(false);

            if (lingerTimer <= 0f)
            {
                // Turn trail effects off
                bladeTrail.emitting = false;
                handleTrail.emitting = false;

                // Reset the knife back to the player's hand in a stowed position
                isStowing = false;
                isDrawing = false;
                isFullyDrawn = false;
                isStowed = true;

                holderTransform.parent = parentHolsterTransform;
                holderTransform.localPosition = Vector3.zero;
                holderTransform.localRotation = Quaternion.identity;
                pivotTransform.localPosition = Vector3.zero;
                pivotTransform.localRotation = Quaternion.identity;
            }
        }

        #endregion
    }

    // Update is called once per frame
    void LateUpdate()
    {
        #region Enemy Lock-On (Pre-Bounce)

        if (isFullyDrawn)
        {
            // Aim at enemies
            Collider[] enemiesInRange = Physics.OverlapSphere(mainCameraTransform.position, throwDetectionRadius, enemyLayers);
            
            Transform closestPoint = null;
            float closestSimilarity = -1f;
            foreach (Collider col in enemiesInRange)
            {
                float enemyCentreHeight = col.gameObject.GetComponent<NavMeshAgent>().height / 2f;
                Vector3 enemyPosition = col.transform.position + (Vector3.up * halfTargetHeight);

                // Calculate vector from camera to enemy
                Vector3 directionToPoint = (enemyPosition - mainCameraTransform.position);

                // If there is a clear line of sight to the enemy
                if (Physics.Linecast(mainCameraTransform.position, enemyPosition, wallLayers) == false)
                {
                    float similarity = Vector3.Dot(directionToPoint.normalized, mainCameraTransform.forward);

                    // If this is the new closest enemy to the player's look vector
                    if (similarity > closestSimilarity)
                    {
                        halfTargetHeight = enemyCentreHeight;
                        closestPoint = col.transform;
                        closestSimilarity = similarity;
                    }
                }
            }

            // If there is a viable enemy target
            if (closestSimilarity >= minimumThrowSimilarity)
            {
                // Highlight the closest enemy with UI effect
                preferredThrowPoint = closestPoint;
            }
            else
            {
                // Stop highlighting the closest enemy
                preferredThrowPoint = null;
            }
        }

        #endregion

        #region Bounces

        if (isAirborn)
        {
            // Spin animation
            pivotTransform.localRotation = Quaternion.Euler(spinSpeed * 360f * (airtimeTimer), 0f, 0f);

            airtimeTimer += Time.deltaTime;

            if (currentTargetTransform == null || currentTargetEnemy == null || currentTargetEnemy.currentHealth <= 0f)
            {
                // Find a new target
                Collider[] enemiesWithinRange = Physics.OverlapSphere(holderTransform.position, bounceDetectionRadius, enemyLayers);

                if (enemiesWithinRange.Length > 0)
                {
                    Transform closestTransform = null;
                    float closestDistance = Mathf.Infinity;

                    // For each of the enemies within range
                    for (int i = 0; i < enemiesWithinRange.Length; i++)
                    {
                        float distanceToEnemy = Vector3.Distance(holderTransform.position, enemiesWithinRange[i].transform.position);

                        // If this is the closest transform so far
                        if (distanceToEnemy < closestDistance)
                        {
                            halfTargetHeight = enemiesWithinRange[i].GetComponent<NavMeshAgent>().height / 2f;
                            Vector3 enemyCentre = enemiesWithinRange[i].transform.position + (Vector3.up * halfTargetHeight);

                            // If the line of sight from the knife to the enemy is clear
                            if (Physics.Linecast(holderTransform.position, enemyCentre, wallLayers) == false)
                            {
                                // If the enemy has health
                                if (enemiesWithinRange[i].GetComponent<Enemy>().currentHealth > 0)
                                {
                                    // The most valid transform so far
                                    closestTransform = enemiesWithinRange[i].transform;
                                    closestDistance = distanceToEnemy;
                                }
                            }
                        }
                    }

                    // If there is a valid contender, assign it to the knife
                    currentTargetTransform = closestTransform;
                    if (currentTargetTransform != null)
                    {
                        currentTargetEnemy = currentTargetTransform.GetComponent<Enemy>();

                        lastBouncePosition = holderTransform.position;
                        bounceDuration = closestDistance;
                        bounceTimer = closestDistance;
                    }
                    else
                    {
                        StopBouncing();
                    }
                }
                else
                {
                    StopBouncing();
                }
            }

            if (isAirborn && currentTargetTransform != null && currentTargetEnemy != null && currentTargetEnemy.currentHealth > 0f)
            {
                // Move towards target
                float progress = BounceSmoothingCurve(Mathf.Clamp01(bounceTimer / bounceDuration));
                Vector3 knifePosition = Vector3.Lerp(lastBouncePosition, currentTargetTransform.position + (Vector3.up * halfTargetHeight), 1f - progress);
                holderTransform.position = knifePosition;
                bounceTimer -= Time.deltaTime * moveSpeed;

                // Rotate towards target
                if (Vector3.Distance(holderTransform.position, currentTargetTransform.position) > 1f)
                {
                    holderTransform.localRotation = Quaternion.LookRotation(currentTargetTransform.position - holderTransform.position);
                }

                // If target is reached
                if (progress <= 0f)
                {
                    // Execute target
                    bounceCount += 1;
                    int damageToDeal = currentTargetEnemy.currentHealth;
                    currentTargetEnemy.TakeDamage(damageToDeal, damageToDeal, currentTargetTransform.position + (Vector3.up * halfTargetHeight), true);
                    playerStats.currentHealth += Mathf.CeilToInt((playerStats.maxHealth / 100f) * healthPercentRestorePerKill);
                    playerStats.currentHealth = Mathf.Clamp(playerStats.currentHealth, 0, playerStats.maxHealth);

                    currentTargetTransform = null;
                    currentTargetEnemy = null;
                }
            }

            // Compare bounce count, if it exceeds the kill limit, end the throw
            if (bounceCount >= maxBounces)
            {
                StopBouncing();
            }
        }

        #endregion
    }

    public void DrawKnife()
    {
        if (switchTimer > 0f)
        {
            // If a switch animation is already in progress
            switchTimer = switchDuration - switchTimer;
        }
        else
        {
            // Starting a switch from a resting position
            switchTimer = switchDuration;
        }

        isDrawing = true;
        isStowing = false;

        isFullyDrawn = false;
        isStowed = false;
    }

    public void StowKnife()
    {
        if (switchTimer > 0f)
        {
            // If a switch animation is already in progress
            switchTimer = switchDuration - switchTimer;
        }
        else
        {
            // Starting a switch from a resting position
            switchTimer = switchDuration;
        }

        isDrawing = false;
        isStowing = true;

        isFullyDrawn = false;
        isStowed = false;
    }

    public void Throw(Transform firstTarget)
    {
        // Only allow one throw at a time
        if (isAirborn)
        {
            return;
        }

        maxBounces = Mathf.RoundToInt((float)WaveManager.waveEnemyCount * waveKillPotential);
        holderTransform.SetParent(null);
        bounceCount = 0;
        airtimeTimer = 0f;

        // Switch from held to airborn
        isAirborn = true;

        // Assign the first target
        currentTargetTransform = firstTarget;
        currentTargetEnemy = currentTargetTransform.GetComponent<Enemy>();
        lastBouncePosition = holderTransform.position;

        bounceTimer = Vector3.Distance(lastBouncePosition, firstTarget.position);
        bounceDuration = bounceTimer;

        // Turn trail effects on
        bladeTrail.emitting = true;
        handleTrail.emitting = true;

        playerStats.ConsumeSuper();
        PlayerStats.allowUltimateGeneration = false;
    }

    public void StopBouncing()
    {
        if (isAirborn == false)
        {
            return;
        }

        isAirborn = false;
        bounceCount = 0;

        currentTargetTransform = null;
        currentTargetEnemy = null;
        airtimeTimer = 0f;

        lingerTimer = lingerDuration;

        isStowing = false;
        isDrawing = false;
        isFullyDrawn = false;
        isStowed = false;

        PlayerStats.allowUltimateGeneration = true;
    }

    public float BounceSmoothingCurve(float time)
    {
        float result = 0f;

        result = (Mathf.Cos(Mathf.PI * (time + 1f)) + 1f) / 2f;
        return result;
    }

    public float WeaponSwitchCurve(float time)
    {
        float result = 0f;

        result = (Mathf.Cos(Mathf.PI * (time + 1f)) + 1f) / 2f;
        return result;
    }
}
