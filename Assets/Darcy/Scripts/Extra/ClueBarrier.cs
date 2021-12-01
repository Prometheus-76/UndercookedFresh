using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author: Darcy Matheson
// Purpose: Runs the easter egg riddle.

public class ClueBarrier : InteractiveObject
{
    public LayerMask playerLayer;
    public ClueInteract[] clueInteracts;

    private BoxCollider extendedInteractTrigger;
    private BoxCollider playerPositionCheckTrigger;
    private MeshRenderer barrierRenderer;
    private MeshCollider barrierCollider;
    private Transform cameraHolderTransform;
    private string prompt;

    private bool clickedOnMoon;
    private int correctInteractsFound;
    private bool interactMistakeMade;
    private bool playerPositioned;
    private bool spamCompleted;
    private float maxTimeBetweenInteracts;
    private float timeBetweenInteractsTimer;
    private float spamTotalTime;
    private float spamTimer;
    private bool allConditionsValid;
    private bool riddleSolved;

    public void Start()
    {
        #region Initialisation

        base.Configure();

        extendedInteractTrigger = GetComponent<BoxCollider>();
        barrierRenderer = GetComponent<MeshRenderer>();
        barrierCollider = GetComponent<MeshCollider>();
        playerPositionCheckTrigger = transform.GetChild(0).GetComponent<BoxCollider>();
        cameraHolderTransform = Camera.main.transform.parent.transform;

        prompt = "";
        extendedInteractTrigger.enabled = false;

        clickedOnMoon = false;
        correctInteractsFound = 0;
        interactMistakeMade = false;
        playerPositioned = false;
        spamCompleted = false;
        maxTimeBetweenInteracts = 0.5f;
        timeBetweenInteractsTimer = 0f;
        spamTotalTime = 3f;
        spamTimer = 0f;
        allConditionsValid = false;
        riddleSolved = false;

        #endregion
    }

    public void Update()
    {
        Vector2 lookDirection = cameraHolderTransform.rotation.eulerAngles;
        bool validLookPitch = lookDirection.x < 335f && lookDirection.x > 325f;
        bool validLookYaw = lookDirection.y < 5f || lookDirection.y > 355f;

        if (validLookPitch && validLookYaw && Input.GetMouseButtonDown(0) && clickedOnMoon == false)
        {
            clickedOnMoon = true;

            for (int i = 0; i < clueInteracts.Length; i++)
            {
                clueInteracts[i].gameObject.SetActive(true);
            }
        }

        bool gameUnstarted = !WaveManager.gameStarted;
        playerPositioned = Physics.OverlapBox(playerPositionCheckTrigger.gameObject.transform.position + playerPositionCheckTrigger.center, playerPositionCheckTrigger.size / 2f, Quaternion.identity, playerLayer).Length > 0;
        allConditionsValid = (gameUnstarted && playerPositioned && correctInteractsFound == 3 && interactMistakeMade == false);

        extendedInteractTrigger.enabled = gameUnstarted && playerPositioned && clickedOnMoon && correctInteractsFound == 3 && interactMistakeMade == false && riddleSolved == false;

        if (interactMistakeMade)
        {
            // The riddle has been solved and game started
            displayKeybind = false;
            interactDuration = 0f;
            prompt = "Secrets remain hidden forever.";

            barrierRenderer.enabled = true;
            barrierCollider.enabled = true;
        }
        else if (riddleSolved && gameUnstarted == false)
        {
            // The riddle has been solved and game started
            displayKeybind = false;
            interactDuration = 0f;
            prompt = "Nothing remains.";

            barrierRenderer.enabled = true;
            barrierCollider.enabled = true;
        }
        else if (spamCompleted && gameUnstarted == false)
        {
            // The riddle is ready to be solved and the game has started
            displayKeybind = false;
            interactDuration = 0f;
            prompt = "Secrets remain hidden forever.";
        }
        else if (spamCompleted && gameUnstarted)
        {
            // The riddle is ready to be solved and the game has not started
            displayKeybind = true;
            interactDuration = 3f;
            prompt = "Proceed with caution...";
        }
        else if (spamCompleted == false || allConditionsValid == false)
        {
            // The riddle is unsolved and the game has not started
            displayKeybind = false;
            interactDuration = 0f;

            // I SEE YOU CHEATING THERE, WALLIAM ;)
            prompt = "Before the beginning.";

            if (gameUnstarted)
                prompt = "Shoot for the moon.";

            if (gameUnstarted && clickedOnMoon)
                prompt = "A glimpse at the stars.  (Activate 1 / 3)";

            if (gameUnstarted && clickedOnMoon && (correctInteractsFound == 1 && interactMistakeMade == false))
                prompt = "Organic alcove.  (Activate 2 / 3)";

            if (gameUnstarted && clickedOnMoon && (correctInteractsFound == 2 && interactMistakeMade == false))
                prompt = "Ceramic and stone.  (Activate 3 / 3)";

            if (gameUnstarted && clickedOnMoon && (correctInteractsFound == 3 && interactMistakeMade == false))
                prompt = "Windows to the stars and seas.";

            if (gameUnstarted && clickedOnMoon && (correctInteractsFound == 3 && interactMistakeMade == false) && playerPositioned)
                prompt = "Determination.";
        }

        if (timeBetweenInteractsTimer <= 0f)
        {
            // If it has taken too long to press the button
            spamTimer = spamTotalTime;
        }
        else
        {
            timeBetweenInteractsTimer -= Time.deltaTime;
            spamTimer -= Time.deltaTime;
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

        // After the determination step is completed
        if (spamCompleted)
        {
            riddleSolved = true;

            // Bring the barrier down
            barrierRenderer.enabled = false;
            barrierCollider.enabled = false;
            extendedInteractTrigger.enabled = false;
        }

        // Before the determination step is completed
        if (interactDuration <= 0f && allConditionsValid && playerPositioned && spamCompleted == false)
        {
            timeBetweenInteractsTimer = maxTimeBetweenInteracts;

            if (spamTimer <= 0f)
            {
                spamCompleted = true;
                interactDuration = 3f;
            }
        }
    }

    public void StationInteracted(int number)
    {
        if (correctInteractsFound == 0)
        {
            // The first one
            if (number == 0)
            {
                // Correct
                correctInteractsFound += 1;
                clueInteracts[number].displayKeybind = false;
                clueInteracts[number].interactPrompt = "Activated.";
            }
            else
            {
                // Incorrect
                for (int i = 0; i < clueInteracts.Length; i++)
                {
                    clueInteracts[i].displayKeybind = false;
                    clueInteracts[i].interactDuration = 0f;
                    clueInteracts[i].interactPrompt = "Sequence Failed.";
                    clueInteracts[i].activated = true;
                }

                interactMistakeMade = true;
            }
        }
        else if (correctInteractsFound == 1)
        {
            // The second one
            if (number == 1)
            {
                // Correct
                correctInteractsFound += 1;
                clueInteracts[number].displayKeybind = false;
                clueInteracts[number].interactPrompt = "Activated.";
            }
            else
            {
                // Incorrect
                for (int i = 0; i < clueInteracts.Length; i++)
                {
                    clueInteracts[i].displayKeybind = false;
                    clueInteracts[i].interactDuration = 0f;
                    clueInteracts[i].interactPrompt = "Sequence Failed.";
                    clueInteracts[i].activated = true;
                }

                interactMistakeMade = true;
            }
        }
        else if (correctInteractsFound == 2)
        {
            // The last one
            if (number == 2)
            {
                // Correct
                correctInteractsFound += 1;

                for (int i = 0; i < clueInteracts.Length; i++)
                {
                    clueInteracts[i].displayKeybind = false;
                    clueInteracts[i].interactDuration = 0f;
                    clueInteracts[i].interactPrompt = "Sequence Complete.";
                    clueInteracts[i].activated = true;
                }
            }
            else
            {
                // Incorrect
                for (int i = 0; i < clueInteracts.Length; i++)
                {
                    clueInteracts[i].displayKeybind = false;
                    clueInteracts[i].interactDuration = 0f;
                    clueInteracts[i].interactPrompt = "Sequence Failed.";
                    clueInteracts[i].activated = true;
                }

                interactMistakeMade = true;
            }
        }
    }
}
