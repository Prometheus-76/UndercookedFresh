using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author: Darcy Matheson
// Purpose: Raises the stones behind the window one at a time, used for accessing part of the easter egg.

public class SteppingStones : MonoBehaviour
{
    public GameObject[] stoneObjects;
    public GameObject triggerBox;
    public LayerMask playerLayer;
    private BoxCollider triggerCollider;

    public float riseDuration;
    private float riseTimer;
    public float maxHeight;
    private int stonesRisen;
    private bool riseTriggered;

    private Vector3 startingStonePosition;
    private Vector3 endingStonePosition;

    void Start()
    {
        stonesRisen = 0;
        riseTimer = riseDuration;
        triggerCollider = triggerBox.GetComponent<BoxCollider>();
        riseTriggered = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Trigger the rising sequence
        Vector3 triggerSize = triggerCollider.size;
        triggerSize.x /= 2f;
        triggerSize.y /= 2f;
        triggerSize.z /= 2f;
        if (WaveManager.gameStarted == false && riseTriggered == false && Physics.OverlapBox(triggerBox.transform.position, triggerCollider.size, Quaternion.identity, playerLayer).Length > 0)
        {
            riseTriggered = true;
        }

        // If we are raising stones and still have some left
        if (riseTriggered && stonesRisen < stoneObjects.Length)
        {
            // Assign a new stone
            if (riseTimer >= riseDuration)
            {
                startingStonePosition = stoneObjects[stonesRisen].transform.localPosition;
                endingStonePosition = startingStonePosition;
                endingStonePosition.y = StoneTargetHeight(stonesRisen);
            }

            // Raise the existing stone
            float raiseProgress = (riseTimer / riseDuration);
            stoneObjects[stonesRisen].transform.localPosition = Vector3.Lerp(startingStonePosition, endingStonePosition, (1f - StoneRaiseEaseOut(raiseProgress)));

            riseTimer -= Time.deltaTime;

            // Check if stone is completed
            if (raiseProgress <= 0f)
            {
                stonesRisen += 1;
                riseTimer = riseDuration;
            }
        }
    }

    // Stone number starts at 0
    float StoneTargetHeight(int stoneNumber)
    {
        float height = 0f;

        height = maxHeight * (stoneNumber / (float)(stoneObjects.Length - 1f));

        return height;
    }

    float StoneRaiseEaseOut(float progress)
    {
        float result = 0f;

        result = (Mathf.Cos(Mathf.PI * (progress + 1f)) + 1f) / 2f;

        return Mathf.Clamp01(result);
    }
}
