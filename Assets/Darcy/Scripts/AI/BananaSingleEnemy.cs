using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author: Darcy Matheson
// Purpose: Controls the small banana enemies, which follow the player and deal contact damage on a short cooldown

public class BananaSingleEnemy : Enemy
{
    #region Variables

    #region Internal

    private float attackTimer;

    #endregion

    #region Parameters
    [Header("Parameters")]

    [Tooltip("The time between attacking the player (in seconds)."), Range(1f, 5f)]
    public float attackInterval = 1f;
    [Tooltip("How close to the player this enemy must be before attacking."), Range(1f, 3f)]
    public float attackRange = 2f;

    #endregion

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        #region Initialisation

        base.Configure();

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
                enemyAnimator.SetBool("IsMoving", false);

                #endregion
            }
            else
            {
                #region Navigate to Player

                // Move towards the player
                enemyAgent.SetDestination(playerTransform.position);
                enemyAgent.isStopped = false;
                enemyAnimator.SetBool("IsMoving", true);

                #endregion

                #region Start Attacking

                Vector3 directionToPlayer = playerTransform.position - enemyTransform.position;
                if (directionToPlayer.magnitude < attackRange && Physics.Linecast(enemyTransform.position + (Vector3.up * (enemyAgent.height - 0.2f)), mainCameraTransform.position, environmentLayers) == false)
                {
                    // Deal damage to the player
                    attackTimer = attackInterval;
                    playerStats.TakeDamage(scaledDamage);

                    // Set the damage direction indicator
                    UserInterfaceHUD.damageFlashTimer = UserInterfaceHUD.damageFlashDuration;
                    UserInterfaceHUD.damageOrigin = enemyTransform.position;

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

            if (isBurrowing)
            {
                // Burrowing animation
                float scale = enemyTransform.localScale.x;
                scale -= Time.deltaTime * 0.5f;
                scale = Mathf.Clamp01(scale);
                enemyTransform.localScale = Vector3.one * scale;
                enemyTransform.Rotate(Vector3.up * Time.deltaTime * 720f, Space.Self);

                // The animation has completed
                if (scale <= 0f)
                {
                    Configure();
                    this.gameObject.SetActive(false);
                    enemyTransform.localScale = Vector3.one;
                    enemyTransform.rotation = Quaternion.identity;
                    WaveManager.waveEnemies.Add(this.gameObject);
                }
            }
        }
    }
}
