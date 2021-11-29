﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author: Darcy Matheson
// Purpose: Controls the movement and rotation of the camera at all times.

public class CameraController : MonoBehaviour
{
    #region Variables

    #region Parameters 

    #region Look Rotation
    [Header("Look Rotation")]
    
    [Tooltip("How sensitive the look/aim rotation is regarding the mouse input."), Range(0.1f, 10f)]
    public float mouseSensitivity = 3f;

    // The current XY rotation
    private Vector2 lookRotation = Vector2.zero;
    private Vector2 startingRotation = Vector2.zero;
    private Vector2 recoilOffset = Vector2.zero;
    #endregion

    #region Camera Follow
    [Header("Camera Follow")]

    [Tooltip("How closely the camera follows the player head position, rotation is not smoothed."), Range(0f, 50f)]
    public float followStrength = 25f;
    #endregion

    #region Head Bouncing
    [Header("Head Bouncing")]

    [Tooltip("How fast the head bounce animation is played."), Range(0f, 10f)]
    public float speedScale = 1f;

    [Tooltip("The maximum horizontal distance the head can bounce."), Range(0f, 1f)]
    public float maxHorizontalDistance = 0.5f;

    [Tooltip("The maximum vertical distance the head can bounce."), Range(0f, 1f)]
    public float maxVerticalDistance = 0.25f;

    [Tooltip("How much faster the head bounce animation plays when sprinting. Crouch scaling is automatic."), Range(1f, 3f)]
    public float headBounceSprintMultiplier = 1.5f;

    [Tooltip("How closely the smooth velocity resembles the actual velocity. Used for smoothing head bob animation when the walk velocity rapidly changes (ie. walking into a wall)."), Range(0f, 100f)]
    public float velocitySmoothing = 50f;
    
    // How fast the player is actually moving and their smoothed speed.
    private float walkSpeedSmooth;
    private float currentSpeedSmooth;

    // Proportional to the distance the player has moved so far, used an input for the head bounce functions
    private float moveTime = 0f;
    #endregion

    #region Screenshake

    private static float traumaLevel;
    public float traumaDrainRate;
    private float scaledStrength;
    public Vector3 rotationShakeMagnitude;
    public float shakeFrequency;

    #endregion

    #region Recoil
    [Header("Recoil")]

    [Tooltip("How fast the recoil drops off after being applied by the current weapon."), Range(1f, 100f)]
    public float recoilDampening = 2f;
    #endregion

    #region Sound

    public AudioClip[] footstepSounds;
    private float soundedMoveTime;
    private float defaultVolume;

    #endregion

    #endregion

    #region Options
    [Header("Options")]

    [Tooltip("Toggles head bounce animation on / off.")]
    public bool allowHeadBounce = true;

    [Tooltip("Scales the intensity of screenshake effects within the game."), Range(0f, 1f)]
    public float screenShakeIntensity = 1f;
    #endregion

    #region Components / References
    [Header("Components")]

    public Transform playerTransform;
    public Transform headTransform;
    public Transform cameraTransform;
    public Transform holderTransform;
    public Movement playerMovement;
    public Rigidbody playerRigidbody;
    public AudioSource footstepAudioSource;
    #endregion

    #endregion

    private void Awake()
    {
        // Static assignment
        traumaLevel = 0f;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Make the cursor invisible and stop it from leaving the window
        Cursor.lockState = CursorLockMode.Locked;
        soundedMoveTime = 0f - (Mathf.PI / (speedScale * 2f));
        defaultVolume = footstepAudioSource.volume;

        startingRotation = holderTransform.localRotation.eulerAngles;
        lookRotation = startingRotation;
    }

    // Update is called once per frame
    void Update()
    {
        #region Look/Aim Rotation

        if (PlayerStats.gamePaused == false)
        {
            //Note: deltaTime is NOT required for this, as GetAxis refers to the distance moved this frame
            //Update Rotation (Left/Right - Turret)
            float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity;
            lookRotation.y += (mouseX + (recoilOffset.x));

            //Update Rotation (Up/Down - Gun)
            float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
            lookRotation.x -= (mouseY + (recoilOffset.y));
        }
        
        lookRotation.x = Mathf.Clamp(lookRotation.x, -89f, 89f);

        // Reset recoil
        recoilOffset -= (recoilOffset * recoilDampening * Time.deltaTime);

        //Set Rotation (All)
        playerTransform.localEulerAngles = new Vector3(0f, lookRotation.y, 0f);
        holderTransform.localEulerAngles = new Vector3(lookRotation.x, lookRotation.y, 0f);

        #endregion

        // Calculate position to follow player from
        Vector3 followPosition = (headTransform.position - holderTransform.position) * followStrength * Time.deltaTime;

        // If the follow would overshoot the target on this frame (occurs at low FPS)
        if (followPosition.magnitude > Vector3.Distance(headTransform.position, holderTransform.position))
        {
            followPosition = headTransform.position;
        }
        else
        {
            followPosition += holderTransform.position;
        }

        #region Camera Motion Effects

        #region Head Bounce

        Vector3 headBounceOffset = Vector3.zero;

        // 0 to 1 representing our % of the max speed, smoothed over time
        float velocityScale = walkSpeedSmooth / (playerMovement.baseMoveSpeed * headBounceSprintMultiplier);
        velocityScale = Mathf.Clamp(velocityScale, 0f, 1f);

        // Set the volume based on the speed of the player with max volume at sprinting speed or above
        footstepAudioSource.volume = Mathf.Round(defaultVolume * velocityScale * 100f) / 100f;

        // Only animate when grounded and not sliding, otherwise pause
        if (Movement.isSliding == false && Movement.isGrounded)
        {
            moveTime += velocityScale * Time.deltaTime;
        }

        float bounceHeight = (Mathf.Cos(2f * moveTime * speedScale) - 1f) * maxVerticalDistance * velocityScale;
        float bounceLength = Mathf.Sin(moveTime * speedScale) * maxHorizontalDistance * velocityScale;

        if (moveTime > soundedMoveTime + (Mathf.PI / speedScale))
        {
            // Play sound
            int footstepSoundIndex = Random.Range(0, footstepSounds.Length);
            footstepAudioSource.PlayOneShot(footstepSounds[footstepSoundIndex]);

            // Update soundedMoveTime
            soundedMoveTime += (Mathf.PI / speedScale);
        }

        // If the option for head bounce effects is enabled
        if (allowHeadBounce)
        {
            headBounceOffset = (Vector3.up * bounceHeight) + (holderTransform.right * bounceLength);
        }

        #endregion

        #region Screenshake

        // Adjust trauma level and calculate strength
        traumaLevel -= Time.deltaTime * traumaDrainRate;
        traumaLevel = Mathf.Clamp01(traumaLevel);
        scaledStrength = TraumaCurve(traumaLevel);

        float time = (Time.time * shakeFrequency);
        float pitch = (Mathf.PerlinNoise(time + 1f, time + 1f) - 0.5f) * scaledStrength * screenShakeIntensity * rotationShakeMagnitude.x;
        float yaw = (Mathf.PerlinNoise(time + 2f, time + 2f) - 0.5f) * scaledStrength * screenShakeIntensity * rotationShakeMagnitude.y;
        float roll = (Mathf.PerlinNoise(time + 3f, time + 3f) - 0.5f) * scaledStrength * screenShakeIntensity * rotationShakeMagnitude.z;
        Quaternion newShakeRotation = Quaternion.Euler(pitch, yaw, roll);

        #endregion

        // Apply combined effects to camera
        Vector3 effectPosition = headBounceOffset;
        Quaternion effectRotation = newShakeRotation;
        cameraTransform.localPosition = effectPosition;
        cameraTransform.localRotation = effectRotation;

        #endregion

        // The holder follows the player
        holderTransform.position = followPosition;
    }

    // FixedUpdate is called every physics iteration
    void FixedUpdate()
    {
        #region Smooth Speed Readouts

        // Increase/Decrease by limited amount, forcefully linearly smoothing results
        float actualSpeed = playerRigidbody.velocity.magnitude;
        float addition = Mathf.Min(Mathf.Abs(actualSpeed - currentSpeedSmooth), velocitySmoothing * Time.fixedDeltaTime);
        currentSpeedSmooth += (actualSpeed > currentSpeedSmooth) ? addition : -addition;

        // Velocity without y component
        Vector3 walkVelocity = playerRigidbody.velocity;
        walkVelocity.y = 0f;

        float actualWalkSpeed = walkVelocity.magnitude;
        float walkAddition = Mathf.Min(Mathf.Abs(actualWalkSpeed - walkSpeedSmooth), velocitySmoothing * Time.fixedDeltaTime);
        walkSpeedSmooth += (actualWalkSpeed > walkSpeedSmooth) ? walkAddition : -walkAddition;

        #endregion
    }

    // Reset the rotation of the camera to it's neutral default
    public void ResetRotation()
    {
        lookRotation = startingRotation;
    }

    // Sets up a new recoil for the camera
    public void AddRecoil(float maxX, float maxY)
    {
        float x = RecoilDistribution(Random.Range(0.1f, 1f)) * maxX;
        float y = RecoilDistribution(Random.Range(0.2f, 1f)) * maxY;

        recoilOffset.x = x * (Random.Range(0, 2) == 1 ? 1f : -1f);
        recoilOffset.y = y;
    }

    // Scales random recoil values to be biased towards higher end while still feeling random
    float RecoilDistribution(float input)
    {
        float output = 0f;
        output = 1f - Mathf.Pow(input - 1f, 4f);

        return output;
    }

    // Square relationship for determine shake strength from trauma level
    float TraumaCurve(float input)
    {
        float output = 0f;

        output = input * input;
        output = Mathf.Clamp01(output);
        return output;
    }

    public static void AddTrauma(float amount)
    {
        traumaLevel += amount;
        traumaLevel = Mathf.Clamp01(traumaLevel);
    }
}
