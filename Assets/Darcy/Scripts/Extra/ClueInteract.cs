using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author: Darcy Matheson
// Purpose: Basic interaction interface that reports activation to the ClueBarrier for the ordered activation step.

public class ClueInteract : InteractiveObject
{
    public ClueBarrier clueBarrier;
    public int interactNumber;

    [HideInInspector]
    public bool activated;

    public void Start()
    {
        base.Configure();
        activated = false;
    }

    private void Update()
    {
        if (WaveManager.gameStarted)
        {
            this.gameObject.SetActive(false);
        }
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
        if (activated == false)
        {
            // Notify barrier that this object has been interacted with
            clueBarrier.StationInteracted(interactNumber);
            activated = true;
            interactDuration = 0f;
        }
    }
}
