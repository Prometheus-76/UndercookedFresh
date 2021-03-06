using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Author: Darcy Matheson
// Purpose: Onion specific enemy class that controls unique enemy behaviour and overrides taking damage with armour mechanic

public class OnionEnemy : Enemy
{
    #region Variables

    #region Internal

    private float chargeTimer;
    private float stunTimer;
    private float windupTimer;
    private Quaternion targetRotation;
    private int currentLayer;

    #endregion

    #region Parameters

    #region Distance
    [Header("Distance")]

    [Tooltip("How far away the onion can begin charging at the player."), Range(20f, 60f)]
    public float chargeRange = 60f;

    #endregion

    #region Duration
    [Header("Duration")]

    [Tooltip("How long the onion charges at the player for before stopping."), Range(2f, 10f)]
    public float chargeDuration = 3f;
    [Tooltip("How long the onion is stunned for after running into a wall."), Range(1f, 5f)]
    public float stunDuration = 3f;
    [Tooltip("How long it takes for the onion to begin a charge while winding up."), Range(1f, 3f)]
    public float windupDuration = 2f;

    #endregion

    #region Speed
    [Header("Speed")]

    [Tooltip("How fast the onion moves while charging at the player."), Range(10f, 30f)]
    public float chargeSpeed = 20f;
    [Tooltip("How fast the onion turns towards the player while charging."), Range(0.2f, 1f)]
    public float chargeTurningSpeed = 0.5f;
    [Tooltip("How fast the onion aims at the player while winding up."), Range(1f, 5f)]
    public float windupTurningSpeed = 3f;

    #endregion

    #region Armour
    [Header("Armour")]

    [Tooltip("How much damage is negated by the outer layer of armour."), Range(50, 90)]
    public int outerLayerDefense = 60;
    [Tooltip("How much damage is negated by the inner layer of armour."), Range(30, 50)]
    public int innerLayerDefense = 40;
    [Tooltip("Where on the health bar the outer layer of armour starts on the enemy."), Range(70, 90)]
    public int outerLayerPercentage = 80;
    [Tooltip("Where on the health bar the inner layer of armour starts on the enemy."), Range(20, 60)]
    public int innerLayerPercentage = 50;

    #endregion

    #endregion

    #region Components
    [Header("Components")]

    public GameObject model1;
    public Animator animator1;

    public GameObject model2;
    public Animator animator2;

    public GameObject model3;
    public Animator animator3;
    #endregion

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        base.Configure();
        currentLayer = 2;

        model2.SetActive(false);
        model3.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealth > 0)
        {
            base.CheckDistanceValidity();
        }

        if (currentHealth > 0 && isBurrowing == false && PlayerStats.isAlive)
        {
            // Calculate absolute and walking distances between enemy and player
            float absoluteDistanceToPlayer = Vector3.Distance(playerTransform.position, enemyTransform.position);
            enemyAgent.CalculatePath(playerTransform.position, enemyPath);
            float traversalDistanceToPlayer = CalculatePathLength(enemyPath);

            // Assign current armour layer
            currentLayer = RemainingArmourLayers();

            // Set model and animator
            switch (currentLayer)
            {
                case 2:
                    model1.SetActive(true);
                    model2.SetActive(false);
                    model3.SetActive(false);
                    enemyAnimator = animator1;
                    enemyCollider.radius = 1f;
                    enemyCollider.height = 2.5f;
                    enemyCollider.center = Vector3.up * 1f;
                    break;
                case 1:
                    model1.SetActive(false);
                    model2.SetActive(true);
                    model3.SetActive(false);
                    enemyAnimator = animator2;
                    enemyCollider.radius = 1f;
                    enemyCollider.height = 2f;
                    enemyCollider.center = Vector3.up * 0.75f;
                    break;
                case 0:
                    model1.SetActive(false);
                    model2.SetActive(false);
                    model3.SetActive(true);
                    enemyAnimator = animator3;
                    enemyCollider.radius = 0.75f;
                    enemyCollider.height = 1f;
                    enemyCollider.center = Vector3.up * 0.5f;
                    break;
            }

            #region Behaviour Tree

            // Determine next action
            if (stunTimer > 0f)
            {
                #region Stunned

                enemyAnimator.SetBool("IsStunned", true);
                enemyAnimator.SetBool("IsCharging", false);
                enemyAnimator.SetBool("IsAiming", false);
                movementParticles.SetActive(false);
                stunTimer -= Time.deltaTime;

                // Stop movement
                enemyAgent.isStopped = true;
                enemyAgent.ResetPath();
                chargeTimer = 0f;
                windupTimer = 0f;

                if (stunTimer < 0f)
                {
                    // Stop the stun
                    stunTimer = 0f;
                    enemyAnimator.SetBool("IsStunned", false);
                }

                #endregion
            }
            else if (chargeTimer > 0f)
            {
                #region Charging

                chargeTimer -= Time.deltaTime;

                if (chargeTimer > 0f)
                {
                    // Charge at the player
                    enemyAgent.isStopped = true;
                    enemyAgent.ResetPath();

                    // Assign the target position to charge towards
                    Vector3 chargeDirection = playerTransform.position - enemyTransform.position;
                    chargeDirection.y = 0f;

                    // Slowly rotate towards the player
                    targetRotation = Quaternion.LookRotation(chargeDirection);
                    enemyTransform.rotation = Quaternion.Lerp(enemyTransform.rotation, targetRotation, Time.deltaTime * chargeTurningSpeed);

                    Vector3 newPosition = enemyTransform.forward * chargeSpeed * Time.deltaTime;
                    newPosition.y = enemyAgent.height / 2f;

                    // The enemy hasn't collided with anything
                    enemyAgent.Move(newPosition);

                    enemyAnimator.SetBool("IsCharging", true);
                    enemyAnimator.SetBool("IsAiming", false);
                    movementParticles.SetActive(true);
                }
                else
                {
                    // Charge has timed out
                    chargeTimer = 0f;
                    enemyAnimator.SetBool("IsCharging", false);
                }

                #endregion
            }
            else if (windupTimer > 0f)
            {
                #region Aiming

                windupTimer -= Time.deltaTime;

                // Stop movement while aiming
                enemyAgent.isStopped = true;
                enemyAgent.ResetPath();

                // Assign the target position to aim towards
                Vector3 chargeDirection = playerTransform.position - enemyTransform.position;
                chargeDirection.y = 0f;

                // Slowly rotate towards the player
                targetRotation = Quaternion.LookRotation(chargeDirection);
                enemyTransform.rotation = Quaternion.Lerp(enemyTransform.rotation, targetRotation, Time.deltaTime * windupTurningSpeed);

                enemyAnimator.SetBool("IsAiming", true);
                movementParticles.SetActive(false);

                if (windupTimer < 0f)
                {
                    // The charge windup has completed, commence the charge
                    windupTimer = 0f;
                    chargeTimer = chargeDuration;

                    // Assign the target position to charge towards
                    chargeDirection = enemyTransform.forward;
                }

                #endregion
            }
            else
            {
                #region Navigation

                // Navigate to the player
                enemyAgent.SetDestination(playerTransform.position);

                enemyAgent.isStopped = false;
                chargeTimer = 0f;
                windupTimer = 0f;
                stunTimer = 0f;

                enemyAnimator.SetBool("IsCharging", false);
                enemyAnimator.SetBool("IsAiming", false);
                enemyAnimator.SetBool("IsStunned", false);
                movementParticles.SetActive(true);

                // Start aiming if within range and line of sight
                if (traversalDistanceToPlayer <= chargeRange)
                {
                    // If the player is off the navmesh, allow windup without strict projected line of sight
                    if (Mathf.Abs(absoluteDistanceToPlayer - traversalDistanceToPlayer) < 0.2f || absoluteDistanceToPlayer >= traversalDistanceToPlayer)
                    {
                        // Begin windup
                        windupTimer = windupDuration;
                    }
                }

                #endregion
            }

            #region Interrupt Aim

            // If the path to the player is not a straight line
            if (windupTimer > 0f)
            {
                // If the player is off the navmesh, allow windup without strict projected line of sight
                if (Mathf.Abs(absoluteDistanceToPlayer - traversalDistanceToPlayer) >= 0.3f && absoluteDistanceToPlayer < traversalDistanceToPlayer)
                {
                    windupTimer = 0f;
                }
            }

            #endregion

            #region Interrupt Charge

            // If currently charging
            if (chargeTimer > 0f)
            {
                // If the enemy hit the player this frame
                if (Physics.OverlapSphere(enemyTransform.position + (Vector3.up * (enemyAgent.height / 2f)), enemyAgent.radius + 0.1f, playerLayer).Length > 0)
                {
                    // Stop moving
                    enemyAgent.isStopped = true;
                    enemyAgent.ResetPath();
                    chargeTimer = 0f;

                    // Damage the player
                    playerStats.TakeDamage(scaledDamage);

                    // Set the damage direction indicator
                    UserInterfaceHUD.damageFlashTimer = UserInterfaceHUD.damageFlashDuration;
                    UserInterfaceHUD.damageOrigin = enemyTransform.position;
                }

                // Try to find the closest edge on the NavMesh
                if (enemyAgent.FindClosestEdge(out NavMeshHit closestNode))
                {
                    // If the edge is within range (geometry may make this difficult as sometimes the nearest edge is on uneven surface, making proximity difficult to facilitate)
                    if (closestNode.distance < 0.5f)
                    {
                        // If the enemy is facing the edge
                        Vector3 closestPoint = closestNode.position;
                        Vector3 directionToEdge = (closestPoint - enemyTransform.position).normalized;
                        if (Vector3.Dot(directionToEdge, enemyTransform.forward) > 0.9f)
                        {
                            Stun();
                        }
                    }
                }
            }

            #endregion

            #endregion
        }
        else
        {
            if (enemyAgent.enabled)
            {
                enemyAgent.ResetPath();
                enemyAgent.isStopped = true;
            }

            if (isBurrowing)
            {
                // Burrowing animation
                float scale = enemyTransform.localScale.x;
                scale -= Time.deltaTime * 0.5f;
                scale = Mathf.Clamp01(scale);
                enemyTransform.localScale = Vector3.one * scale;
                enemyTransform.Rotate(Vector3.up * Time.deltaTime * 360f, Space.Self);

                // The animation has completed
                if (scale <= 0f)
                {
                    Configure();
                    this.gameObject.SetActive(false);
                    enemyTransform.localScale = Vector3.one;
                    enemyTransform.rotation = Quaternion.identity;
                }
            }
        }
    }

    // Responsible for removing health from the enemy and spawning damage numbers when damage is dealt
    public override void TakeDamage(int damage, int expectedDamage, Vector3 position, bool ignoreArmour)
    {
        if (isBurrowing)
            return;

        int startingHealth = currentHealth;

        int damageTaken = 0;
        currentLayer = RemainingArmourLayers();

        // Determine level of armour for this layer
        int layerDefense = 0;
        switch (currentLayer)
        {
            case 2:
                layerDefense = outerLayerDefense;
                break;
            case 1:
                layerDefense = innerLayerDefense;
                break;
            case 0:
                layerDefense = 0;
                break;
        }

        if (ignoreArmour == false)
        {
            // Calculate the damage to deal with the current armour layer
            damageTaken = Mathf.CeilToInt((float)damage * ((100f - (float)layerDefense) / 100f));
        }
        else
        {
            damageTaken = damage;
        }

        // Determine the damage the enemy will take, cut it off if it brings them below 0
        damageTaken = Mathf.Clamp(damageTaken, 0, currentHealth);
        currentHealth -= damageTaken;

        // If the enemy has taken damage
        if (damageTaken > 0)
        {
            // Draw damage numbers
            GameObject damageNumberInstance = Instantiate<GameObject>(damageNumberPrefab, damageNumberParentTransform);
            damageNumberInstance.GetComponent<DamageNumber>().SetupDamageNumber(damageTaken.ToString(), position, (damage == expectedDamage));

            playerStats.damageDealt += damageTaken;

            // Show hit marker
            UserInterfaceHUD.hitMarkerFadeTimer = UserInterfaceHUD.hitMarkerFadeDuration;
        }

        // If the enemy has died
        if (startingHealth > 0 && currentHealth <= 0)
        {
            Die();
            model1.SetActive(false);
            model2.SetActive(false);
            model3.SetActive(true);
            enemyAnimator = animator3;
            enemyAnimator.SetBool("IsAlive", false);
        }
    }

    // Stuns the enemy
    void Stun()
    {
        // Stop the enemy, start their timer and stop pathfinding
        stunTimer = stunDuration;
        enemyAgent.ResetPath();
        enemyAgent.isStopped = true;
        enemyAgent.velocity = Vector3.zero;

        // Reset current actions
        chargeTimer = 0f;
        windupTimer = 0f;
    }

    // Returns how many armour layers remain, from 0-2
    int RemainingArmourLayers()
    {
        int result = 0;

        float healthPercentage = ((float)currentHealth / (float)baseMaxHealth) * 100f;
        if (healthPercentage > outerLayerPercentage)
        {
            // Outer layer
            result = 2;
        }
        else if (healthPercentage > innerLayerPercentage)
        {
            // Inner layer
            result = 1;
        }

        return result;
    }
}
