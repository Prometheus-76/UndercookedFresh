using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author: Darcy Matheson
// Purpose: Basic interactive item which unlocks a weapon/ability for the player when picked up. Microwave gun starts the run.

public class UnlockItem : InteractiveObject
{
    #region Variables

    #region Internal

    public enum ItemType
    {
        MicrowaveGun,
        ASaltRifle,
        PepperShotgun,
        ThrowingKnife,
        GrappleAbility
    }

    private PlayerStats playerStats;

    #endregion

    #region Configuration
    [Header("Configuration")]

    public ItemType unlockItem;

    #endregion

    #region Components
    [Header("Components")]

    public WeaponCoordinator weaponCoordinator;
    private Movement playerMovement;
    private WaveManager waveManager;

    #endregion

    #endregion

    private void Start()
    {
        #region Initialisation

        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<Movement>();
        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
        waveManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<WaveManager>();

        base.Configure();

        #endregion
    }

    // Return the cost of interacting with this object
    public override int GetFibreCost()
    {
        // Calculate the cost with difficulty scaling here
        return baseCost;
    }

    // Interact with the object and perform its function
    public override void Interact()
    {
        #region Unlock Item Type

        switch (unlockItem)
        {
            case ItemType.MicrowaveGun:
                weaponCoordinator.UnlockWeapon(0);
                waveManager.StartWaveCycle();
                break;
            case ItemType.ASaltRifle:
                weaponCoordinator.UnlockWeapon(1);
                break;
            case ItemType.PepperShotgun:
                weaponCoordinator.UnlockWeapon(2);
                break;
            case ItemType.ThrowingKnife:
                weaponCoordinator.UnlockKnife();
                playerStats.AddUltimateCharge(100f);
                break;
            case ItemType.GrappleAbility:
                playerMovement.UnlockGrapple();
                break;
        }

        #endregion

        Destroy(gameObject);
    }
}
