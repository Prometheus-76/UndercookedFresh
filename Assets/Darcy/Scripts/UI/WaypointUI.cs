using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Author: Darcy Matheson
// Purpose: Generic waypoint controller, includes scaling with distance, rotation to face camera, line of sight fading and a distance readout on a TextMeshProUGUI component.

public class WaypointUI : MonoBehaviour
{
    #region Variables 

    #region Internal

    private float overallAlphaMultiplier;
    private float iconBaseAlpha;
    [HideInInspector]
    public bool isActivated;

    #endregion

    #region Configuration
    [Header("Configuration")]

    [Tooltip("How small the icon appears to be."), Range(10f, 30f)]
    public float scaleRate = 15f;
    [Tooltip("The distance to camera that starts fading in/out."), Range(5f, 20f)]
    public float fadeMinDistance = 10f;
    [Tooltip("The time it takes to fade in/out."), Range(0.1f, 0.5f)]
    public float overallFadeTime = 0.2f;
    [Tooltip("The layers which can block this waypoint, causing it to appear within min range.")]
    public LayerMask environmentLayers;

    #endregion

    #region Components

    private RectTransform canvasTransform;
    private Image iconImage;
    private TextMeshProUGUI distanceText;
    private Transform mainCameraTransform;

    #endregion

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        #region Initialisation

        canvasTransform = GetComponent<RectTransform>();
        iconImage = GetComponentInChildren<Image>();
        distanceText = GetComponentInChildren<TextMeshProUGUI>();
        mainCameraTransform = Camera.main.GetComponent<Transform>();
        overallAlphaMultiplier = 0f;
        iconBaseAlpha = iconImage.color.a;
        iconImage.color = Color.clear;
        distanceText.color = Color.clear;
        isActivated = true;

        #endregion
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // Calculate distance between the main camera and this waypoint
        float distanceToCamera = Vector3.Distance(canvasTransform.position, mainCameraTransform.position);
        bool isVisible = Physics.Linecast(canvasTransform.position, mainCameraTransform.position, environmentLayers) == false;

        #region Update Waypoint UI

        if ((distanceToCamera > fadeMinDistance || (distanceToCamera <= fadeMinDistance && isVisible == false)) && isActivated)
        {
            overallAlphaMultiplier += (Time.deltaTime / overallFadeTime);
        }
        else
        {
            overallAlphaMultiplier -= (Time.deltaTime / overallFadeTime);
        }

        // The overall alpha of the entire waypoint UI, 1 is full opacity and 0 is invisible
        overallAlphaMultiplier = Mathf.Clamp(overallAlphaMultiplier, 0f, 1f);

        // Calculate new colours with fade multiplier
        Color newColour = Color.white;

        newColour.a = iconBaseAlpha * overallAlphaMultiplier;
        iconImage.color = newColour;

        // Set text
        distanceText.text = Mathf.FloorToInt(distanceToCamera / 2f) + "m";
        distanceText.color = newColour;

        #endregion

        #region Orientation and Scaling Relative To Camera

        // Rotate towards camera
        Quaternion rotationToCamera = Quaternion.LookRotation(canvasTransform.position - mainCameraTransform.position);
        canvasTransform.rotation = rotationToCamera;

        // Scale with distance
        canvasTransform.localScale = Vector3.one * (Mathf.Pow(distanceToCamera, 1.1f) / scaleRate);

        #endregion
    }
}
