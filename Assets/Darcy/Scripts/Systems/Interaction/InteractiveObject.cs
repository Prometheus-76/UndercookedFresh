using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author: Darcy Matheson
// Purpose: Base class for all interactive objects, facilitates UI pop-up and fibre expenses

public class InteractiveObject : MonoBehaviour
{
    #region Variables

    #region Internal

    [HideInInspector]
    public bool isInteractable; // Only relevant on Upgrade Station script

    #endregion

    #region Configuration
    [Header("Configuration")]

    public int baseCost;
    public float difficultyScaling;
    public float interactDuration;
    public string interactPrompt;

    #endregion

    #endregion

    // Set up the object
    public void Configure()
    {
        isInteractable = true;
    }

    // Return the cost of interacting with this object
    public virtual int GetFibreCost()
    {
        return 0;
    }

    // Interact with the object and perform its function
    public virtual void Interact()
    {
        // Do some stuff here
    }
}
