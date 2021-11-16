using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author: Darcy Matheson
// Purpose: Interactive object that is destroyed when fibre is spent on it

public class RemovableWall : InteractiveObject
{
    public void Start()
    {
        #region Initialisation

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
        Destroy(gameObject);
    }
}
