﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClueBarrier : InteractiveObject
{
    public LayerMask playerLayer;

    private BoxCollider extendedInteractTrigger;
    private BoxCollider playerPositionCheckTrigger;
    private MeshRenderer barrierRenderer;
    private MeshCollider barrierCollider;
    private string prompt;

    private int interactCount;
    private bool allConditionsValid;
    private bool riddleSolved;
    private bool pressedTabAndShift;

    public void Start()
    {
        #region Initialisation

        base.Configure();

        extendedInteractTrigger = GetComponent<BoxCollider>();
        barrierRenderer = GetComponent<MeshRenderer>();
        barrierCollider = GetComponent<MeshCollider>();
        playerPositionCheckTrigger = transform.GetChild(0).GetComponent<BoxCollider>();

        prompt = "";
        extendedInteractTrigger.enabled = false;

        interactCount = 0;
        allConditionsValid = false;
        riddleSolved = false;
        pressedTabAndShift = false;

        #endregion
    }

    public void Update()
    {
        bool gameUnstarted = !WaveManager.gameStarted;
        bool playerPositioned = Physics.OverlapBox(playerPositionCheckTrigger.gameObject.transform.position + playerPositionCheckTrigger.center, playerPositionCheckTrigger.size / 2f, Quaternion.identity, playerLayer).Length > 0;
        if (pressedTabAndShift == false)
        {
            pressedTabAndShift = Input.GetKey(KeyCode.Tab) && Input.GetKey(KeyCode.LeftShift);
        }
        bool visitedConfirmationScreen = PauseMenuHUD.actionDenied;
        allConditionsValid = (gameUnstarted && playerPositioned && visitedConfirmationScreen && pressedTabAndShift);

        extendedInteractTrigger.enabled = gameUnstarted && playerPositioned && riddleSolved == false;

        if (riddleSolved && gameUnstarted == false)
        {
            // The riddle has been solved and game started
            displayKeybind = false;
            interactDuration = 0f;
            prompt = "Nothing remains.";

            barrierRenderer.enabled = true;
            barrierCollider.enabled = true;
        }
        else if (interactCount >= 5 && gameUnstarted == false)
        {
            // The riddle is ready to be solved and the game has started
            displayKeybind = false;
            interactDuration = 0f;
            prompt = "Secrets remain hidden forever.";
        }
        else if (interactCount >= 5 && gameUnstarted)
        {
            // The riddle is ready to be solved and the game has not started
            displayKeybind = true;
            interactDuration = 3f;
            prompt = "Proceed with caution...";
        }
        else if (interactCount < 5 || allConditionsValid == false)
        {
            // The riddle is unsolved and the game has not started
            displayKeybind = false;
            interactDuration = 0f;
            prompt = "Before the beginning.  ";
            prompt += (gameUnstarted ? "Elevation truncation.  " : "");
            prompt += (gameUnstarted && playerPositioned ? "Accelerate all stillness.  " : "");
            prompt += (gameUnstarted && playerPositioned && pressedTabAndShift ? "Any second thoughts.  " : "");
            prompt += (gameUnstarted && playerPositioned && pressedTabAndShift && visitedConfirmationScreen ? "Determination.  " : "");
        }

        interactPrompt = prompt;
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
        if (WaveManager.gameStarted)
            return;

        // Add to the total counter
        if (interactDuration <= 0f && allConditionsValid)
        {
            interactCount += 1;
        }
        else if (interactCount >= 5)
        {
            riddleSolved = true;

            // Bring the barrier down
            barrierRenderer.enabled = false;
            barrierCollider.enabled = false;
            extendedInteractTrigger.enabled = false;
        }
    }
}