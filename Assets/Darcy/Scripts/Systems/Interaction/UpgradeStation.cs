using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Author: Darcy Matheson
// Purpose: Interactive object that allows the player to interact, summoning the upgrade/refill menu

public class UpgradeStation : InteractiveObject
{
    private WaypointUI waypointScript;

    public GameObject inactiveModel;
    public GameObject activeModel;

    // Start is called before the first frame update
    public void Start()
    {
        #region Initialisation

        // Default upgrade station to being disabled
        isInteractable = false;
        waypointScript = GetComponentInChildren<WaypointUI>();

        #endregion
    }

    private void Update()
    {
        waypointScript.isActivated = isInteractable;
        inactiveModel.SetActive(!isInteractable);
        activeModel.SetActive(isInteractable);
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
        // Pause the game and show the upgrade HUD
        PlayerStats.gamePaused = true;
        UpgradeStationHUD.showHUD = true;
        UserInterfaceHUD.showHUD = false;

        // Make the cursor visible and confine it to the window
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
