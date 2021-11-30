using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author: Darcy Matheson
// Purpose: Uses an eased lerp to move the background inverse to mouse movement, creating a parallax effect.

public class ParallaxBackground : MonoBehaviour
{
    [Range(0f, 1f)]
    public float effectStrength;
    public float followStrength;

    private Camera mainCamera;
    private RectTransform backgroundTransform;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        backgroundTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePosition = mainCamera.ScreenToViewportPoint(Input.mousePosition);
        mousePosition.x -= 0.5f;
        mousePosition.y -= 0.5f;
        mousePosition *= 2f;
        mousePosition.x = Mathf.Clamp(mousePosition.x, -1f, 1f);
        mousePosition.y = Mathf.Clamp(mousePosition.y, -1f, 1f);

        Vector3 offsetPosition = Vector3.zero;
        offsetPosition.x = -mousePosition.x * (1920f / 2f) * effectStrength;
        offsetPosition.y = -mousePosition.y * (1080f / 2f) * effectStrength;

        Vector3 smoothOffset = Vector3.Lerp(backgroundTransform.anchoredPosition, offsetPosition, Time.deltaTime * followStrength);

        backgroundTransform.anchoredPosition = smoothOffset;
    }
}
