using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author: Darcy Matheson
// Purpose: Controls the homing projectiles shot by the strawberry enemy

public class HomingSeed : MonoBehaviour
{
    #region Variables

    #region Internal

    private float lifetimeTimer;
    private int contactDamage;
    private bool isDestroyed;

    #endregion

    #region Parameters

    #region Configuration
    [Header("Configuration")]

    public LayerMask environmentLayers;
    public LayerMask playerLayer;

    #endregion

    #region Speed
    [Header("Speed")]

    [Tooltip("How aggressively the seed turns towards the player while homing."), Range(1f, 10f)]
    public float steeringSpeed = 3f;
    [Tooltip("How quickly the seed moves forward."), Range(10f, 50f)]
    public float movementSpeed = 20f;
    [Tooltip("How quickly the seed twists as it flies (purely visual effect)."), Range(60f, 360f)]
    public float modelTwistSpeed = 100f;

    #endregion

    #region Distance
    [Header("Distance")]

    [Tooltip("The radius of the seed's hitbox when checking for environmental collisions."), Range(0.1f, 0.5f)]
    public float hitboxRadius = 0.2f;
    [Tooltip("The radius of the seed's hitbox when checking for collision with the player (attack range)."), Range(0.3f, 1f)]
    public float attackRadius = 0.5f;
    [Tooltip("The minimum distance the seed must be from the player before it is allowed to turn towards them (prevents orbiting and unavoidable behaviours)."), Range(3f, 8f)]
    public float homingMinDistance = 5f;

    #endregion

    #region Duration
    [Header("Duration")]

    [Tooltip("The lifetime of the seed, it expires after this time if it hasn't already collided with something."), Range(2f, 5f)]
    public float maxLifetime = 2f;

    #endregion

    #endregion

    #region Components
    [Header("Components")]

    public Transform modelPivotTransform;
    private Quaternion targetRotation;
    private Transform seedTransform;
    private Transform playerTransform;
    private CapsuleCollider playerCollider;
    private PlayerStats playerStats;
    public GameObject seedModel;

    #endregion

    #endregion

    // Update is called once per frame
    void Update()
    {
        // If the seed is alive
        if (lifetimeTimer > 0f)
        {
            lifetimeTimer -= Time.deltaTime;

            #region Homing Towards Player

            if ((seedTransform.position - playerTransform.position).magnitude > homingMinDistance)
            {
                Vector3 playerCentrePosition = playerTransform.position + (Vector3.up * (playerCollider.height / 2f));

                // Slowly rotate towards the player
                targetRotation = Quaternion.LookRotation(seedTransform.position - playerCentrePosition);
                seedTransform.rotation = Quaternion.Lerp(seedTransform.rotation, targetRotation, Time.deltaTime * steeringSpeed);
            }

            modelPivotTransform.Rotate(0f, 0f, Time.deltaTime * modelTwistSpeed, Space.Self);

            // Move in forward direction
            Vector3 potentialPosition = seedTransform.position - (seedTransform.forward * movementSpeed * Time.deltaTime);

            if (Physics.OverlapSphere(potentialPosition, attackRadius, playerLayer).Length <= 0)
            {
                // If the seed won't collide with the player next frame
                if (Physics.OverlapSphere(potentialPosition, hitboxRadius, environmentLayers).Length <= 0)
                {
                    // The seed hasn't collided with anything
                    seedTransform.position = potentialPosition;
                }
                else
                {
                    // The seed has collided with the environment
                    DestroySeed();
                }
            }
            else
            {
                // The seed has collided with the player
                playerStats.TakeDamage(contactDamage);
                DestroySeed();
            }

            #endregion
        }
        else
        {
            DestroySeed();
        }
    }

    // Called after the seed is instantiated
    public void SetupSeed(int damage)
    {
        // Initialisation
        seedTransform = GetComponent<Transform>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
        playerCollider = GameObject.FindGameObjectWithTag("Player").GetComponent<CapsuleCollider>();
        lifetimeTimer = maxLifetime;

        // Point at the player to start with
        Vector3 playerCentrePosition = playerTransform.position + (Vector3.up * (playerCollider.height / 2f));
        targetRotation = Quaternion.LookRotation(seedTransform.position - playerCentrePosition);
        seedTransform.rotation = targetRotation;

        // Set damage per hit
        contactDamage = damage;
        isDestroyed = false;
    }

    // Destroys the seed, first setting the model to inactive, waiting for the trail to end, then destroying the object
    void DestroySeed()
    {
        if (isDestroyed == false)
        {
            isDestroyed = true;

            lifetimeTimer = 0f;
            seedModel.SetActive(false);
            Destroy(gameObject, 1f);
        }
    }
}
