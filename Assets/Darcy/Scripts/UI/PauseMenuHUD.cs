using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Author: Darcy Matheson
// Purpose: Controls the pause menu HUD, including saving and loading the mouse sensitivity from memory (PlayerPrefs)

public class PauseMenuHUD : MonoBehaviour
{
    #region Variables

    #region Internal

    private enum NavigationAction
    {
        None,
        RestartRun,
        ExitToMenu,
        QuitGame
    }

    private NavigationAction desiredAction;
    public static bool showHUD;

    #endregion

    #region General

    public Canvas pauseMenuCanvas;
    public GameObject blurEffect;

    public GameObject navigationScreen;
    public GameObject confirmationScreen;

    public Slider sensitivitySlider;

    private CameraController cameraController;
    private SceneLoader sceneLoader;

    #endregion

    #endregion

    void Start()
    {
        #region Initialisation

        // Display the saved sensitivity value on the slider, defaulting to 3.0
        sensitivitySlider.value = Mathf.RoundToInt(PlayerPrefs.GetFloat("MouseSensitivity", 3f) * 10f);
        
        desiredAction = NavigationAction.None;
        showHUD = false;

        cameraController = Camera.main.transform.parent.GetComponent<CameraController>();
        sceneLoader = GameObject.FindGameObjectWithTag("LoadingScreen").GetComponent<SceneLoader>();

        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        // Only allow access to the pause menu when the player is alive and not in the upgrade station
        if (UpgradeStationHUD.showHUD == false && PlayerStats.isAlive)
        {
            // Pauses the game when escape is pressed
            if (Input.GetKeyDown(KeyCode.Escape) && PlayerStats.gamePaused == false)
            {
                // Change paused state
                PlayerStats.gamePaused = true;
                showHUD = true;
            }

            // Adjust game behaviour accordingly
            Time.timeScale = (PlayerStats.gamePaused) ? 0f : 1f;
            UserInterfaceHUD.showHUD = !PlayerStats.gamePaused;
            Cursor.lockState = (PlayerStats.gamePaused) ? CursorLockMode.None : CursorLockMode.Locked;
            blurEffect.SetActive(PlayerStats.gamePaused);
        }

        pauseMenuCanvas.enabled = showHUD;
    }

    // Resume the game and close the pause menu
    public void Resume()
    {
        PlayerStats.gamePaused = false;

        // Adjust game behaviour accordingly
        Time.timeScale = (PlayerStats.gamePaused) ? 0f : 1f;
        pauseMenuCanvas.enabled = PlayerStats.gamePaused;
        UserInterfaceHUD.showHUD = !PlayerStats.gamePaused;
        Cursor.lockState = (PlayerStats.gamePaused) ? CursorLockMode.None : CursorLockMode.Locked;
        showHUD = false;
    }

    #region Navigation Buttons

    // Select restart option and navigate to confirmation menu
    public void AttemptRestartStage()
    {
        desiredAction = NavigationAction.RestartRun;

        navigationScreen.SetActive(false);
        confirmationScreen.SetActive(true);
    }

    // Select return to menu option and navigate to confirmation menu
    public void AttemptReturnToMenu()
    {
        desiredAction = NavigationAction.ExitToMenu;

        navigationScreen.SetActive(false);
        confirmationScreen.SetActive(true);
    }

    // Select quit option and navigate to confirmation menu
    public void AttemptQuit()
    {
        desiredAction = NavigationAction.QuitGame;

        navigationScreen.SetActive(false);
        confirmationScreen.SetActive(true);
    }

    #endregion

    #region Action Validation

    // Called when the player selects "Yes" in the confirmation screen of the pause menu
    public void ConfirmAction()
    {
        // Based on previous selection, perform action chosen
        switch (desiredAction)
        {
            case NavigationAction.RestartRun:
                RestartStage();
                break;
            case NavigationAction.ExitToMenu:
                ReturnToMenu();
                break;
            case NavigationAction.QuitGame:
                Quit();
                break;
        }
    }

    // Called when the player selects "No" in the confirmation screen of the pause menu
    public void DenyAction()
    {
        // Return to pause menu origin
        desiredAction = NavigationAction.None;
        navigationScreen.SetActive(true);
        confirmationScreen.SetActive(false);
    }

    #endregion

    #region Perform Action

    // Reload the current scene
    private void RestartStage()
    {
        sceneLoader.LoadSceneWithProgress(SceneManager.GetActiveScene().buildIndex);
    }

    // Return to the main menu
    private void ReturnToMenu()
    {

    }

    // Close the game
    private void Quit()
    {
        Application.Quit();
    }

    #endregion

    // Called on Slider.ValueChanged()
    public void UpdateSensitivity()
    {
        // Update the mouse sensitivity
        cameraController.mouseSensitivity = sensitivitySlider.value / 10f;
        PlayerPrefs.SetFloat("MouseSensitivity", sensitivitySlider.value / 10f);
    }
}
