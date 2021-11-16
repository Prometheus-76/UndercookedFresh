using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Author: Darcy Matheson
// Purpose: Controls the pause menu HUD, including saving and loading the mouse sensitivity from memory (PlayerPrefs)

public class PauseMenuHUD : MonoBehaviour
{
    private enum NavigationAction
    {
        None,
        RestartRun,
        ExitToMenu,
        QuitGame
    }

    private NavigationAction desiredAction;

    public static bool showHUD;

    public Canvas pauseMenuCanvas;
    public GameObject blurEffect;

    public GameObject navigationScreen;
    public GameObject confirmationScreen;

    public Slider sensitivitySlider;

    public CameraController cameraController;

    private SceneLoader sceneLoader;

    void Start()
    {
        // Display the saved sensitivity value on the slider, defaulting to 3.0
        sensitivitySlider.value = Mathf.RoundToInt(PlayerPrefs.GetFloat("MouseSensitivity", 3f) * 10f);
        
        desiredAction = NavigationAction.None;
        showHUD = false;

        sceneLoader = GameObject.FindGameObjectWithTag("LoadingScreen").GetComponent<SceneLoader>();
    }

    // Update is called once per frame
    void Update()
    {
        // Pauses the game when escape is pressed
        if (UpgradeStationHUD.showHUD == false && PlayerStats.isAlive)
        {
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

    public void AttemptRestartStage()
    {
        desiredAction = NavigationAction.RestartRun;

        navigationScreen.SetActive(false);
        confirmationScreen.SetActive(true);
    }

    public void AttemptReturnToMenu()
    {
        desiredAction = NavigationAction.ExitToMenu;

        navigationScreen.SetActive(false);
        confirmationScreen.SetActive(true);
    }

    public void AttemptQuit()
    {
        desiredAction = NavigationAction.QuitGame;

        navigationScreen.SetActive(false);
        confirmationScreen.SetActive(true);
    }

    // Called on Slider.ValueChanged()
    public void UpdateSensitivity()
    {
        // Update the mouse sensitivity
        cameraController.mouseSensitivity = sensitivitySlider.value / 10f;
        PlayerPrefs.SetFloat("MouseSensitivity", sensitivitySlider.value / 10f);
    }

    // When the player presses the confirm button
    public void ConfirmAction()
    {
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

    public void DenyAction()
    {
        desiredAction = NavigationAction.None;
        navigationScreen.SetActive(true);
        confirmationScreen.SetActive(false);
    }

    private void RestartStage()
    {
        sceneLoader.LoadSceneWithProgress(SceneManager.GetActiveScene().buildIndex);
    }

    private void ReturnToMenu()
    {

    }

    private void Quit()
    {
        Application.Quit();
    }
}
