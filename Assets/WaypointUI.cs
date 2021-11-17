using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaypointUI : MonoBehaviour
{
    #region Variables 

    #region Internal

    private float overallAlphaMultiplier;
    private float iconBaseAlpha;

    #endregion

    #region Configuration
    [Header("Configuration")]

    [Tooltip("How small the icon appears to be."), Range(10f, 30f)]
    public float scaleRate = 15f;
    [Tooltip("The distance to camera that starts fading in/out."), Range(5f, 20f)]
    public float fadeMinDistance = 10f;
    [Tooltip("The time it takes to fade in/out."), Range(0.1f, 0.5f)]
    public float overallFadeTime = 0.2f;

    #endregion

    #region Components

    private RectTransform canvasTransform;
    private Image iconImage;
    private Transform mainCameraTransform;

    #endregion

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        #region Initialisation

        canvasTransform = GetComponent<RectTransform>();
        iconImage = GetComponentInChildren<Image>();
        mainCameraTransform = Camera.main.GetComponent<Transform>();
        overallAlphaMultiplier = 0f;
        iconBaseAlpha = iconImage.color.a;

        #endregion
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // Calculate distance between the main camera and this waypoint
        float distanceToCamera = Vector3.Distance(canvasTransform.position, mainCameraTransform.position);

        #region Fade All Waypoint UI

        if (distanceToCamera > fadeMinDistance)
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

        newColour = iconImage.color;
        newColour.a = iconBaseAlpha * overallAlphaMultiplier;
        iconImage.color = newColour;

        #endregion

        #region Orientation and Scaling Relative To Camera

        // Rotate towards camera
        Quaternion rotationToCamera = Quaternion.LookRotation(mainCameraTransform.position - canvasTransform.position);
        canvasTransform.rotation = rotationToCamera;

        // Scale with distance
        canvasTransform.localScale = Vector3.one * (distanceToCamera / scaleRate);

        #endregion
    }
}
