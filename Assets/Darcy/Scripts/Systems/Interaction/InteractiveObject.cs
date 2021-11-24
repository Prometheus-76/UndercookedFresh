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

    [Tooltip("The unscaled cost of interacting with this object."), Range(0, 100)]
    public int baseCost;
    [Tooltip("The time it takes to interact with this object (in seconds)."), Range(0f, 5f)]
    public float interactDuration;
    [Tooltip("The prompt that appears when looking at this object, automatically adds on \"(keybind)\".")]
    public string interactPrompt;
    [Tooltip("Whether to add on the keybind prompt for this object.")]
    public bool displayKeybind = true;

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
