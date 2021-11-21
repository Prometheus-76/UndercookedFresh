using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author: Darcy Matheson
// Purpose: Controls the banana group enemy, splitting into numerous smaller bananas when killed

public class BananaBunchEnemy : Enemy
{
    #region Variables

    #region Internal 

    private float attackTimer;
    private GameObject[] bananas;

    #endregion

    #region Parameters

    #region Setup
    [Header("Setup")]

    public GameObject bananaPrefab;

    #endregion

    #region Behaviours
    [Header("Behaviours")]

    [Tooltip("How many bananas emerge from this enemy upon death."), Range(1, 5)]
    public int bananaCount = 3;
    [Tooltip("The time between attacking the player (in seconds)."), Range(1f, 5f)]
    public float attackInterval = 2f;
    [Tooltip("How close to the player this enemy must be before attacking."), Range(1f, 3f)]
    public float attackRange = 2f;

    #endregion

    #endregion

    #region Components

    private Transform enemyHolderTransform;

    #endregion

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        #region Initialisation

        base.Configure();
        enemyHolderTransform = enemyTransform.parent;

        // Mini enemy frontloaded pool
        bananas = new GameObject[bananaCount];
        for (int i = 0; i < bananaCount; i++)
        {
            // Create the bananas and disable them
            bananas[i] = Instantiate<GameObject>(bananaPrefab, enemyTransform.position, Quaternion.identity, enemyTransform);
            bananas[i].SetActive(false);
        }

        #endregion
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
            #region Behaviour Tree

            if (attackTimer > 0f)
            {
                #region Attack Cooldown

                // Attack is on cooldown
                attackTimer -= Time.deltaTime;

                enemyAgent.isStopped = true;
                enemyAgent.ResetPath();

                #endregion
            }
            else
            {
                #region Navigate to Player

                // Move towards the player
                enemyAgent.isStopped = false;
                enemyAgent.SetDestination(playerTransform.position);

                #endregion

                #region Start Attacking

                Vector3 directionToPlayer = playerTransform.position - enemyTransform.position;
                if (directionToPlayer.magnitude < attackRange && Physics.Linecast(enemyTransform.position + (Vector3.up * (enemyAgent.height - 0.2f)), mainCameraTransform.position, environmentLayers) == false)
                {
                    // Deal damage to the player
                    attackTimer = attackInterval;
                    playerStats.TakeDamage(scaledDamage);

                    enemyAgent.isStopped = true;
                    enemyAgent.ResetPath();
                }

                #endregion
            }

            #endregion
        }
        else
        {
            if (enemyAgent.enabled)
            {
                enemyAgent.ResetPath();
                enemyAgent.isStopped = true;
            }
        }
    }

    // Override the death and spawn several single banana enemies
    public override void Die()
    {
        // Spawn the children in and change their parent
        for (int i = 0; i < bananas.Length; i++)
        {
            bananas[i].SetActive(true);
            bananas[i].transform.parent = enemyHolderTransform;
        }

        // Kill the parent
        base.Die();
    }
}
