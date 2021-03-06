using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

// Author: Darcy Matheson
// Purpose: Controls the score screen UI displayed to the player after death, at the end of a run

public class ScoreScreenHUD : MonoBehaviour
{
    #region Variables

    #region Internal

    public static bool showHUD;
    private PlayerStats playerStats;
    private SceneLoader sceneLoader;

    #endregion

    #region General

    public Canvas scoreScreenCanvas;
    public GameObject blurEffect;

    #endregion

    #region Stats

    public TextMeshProUGUI attemptsText;
    public TextMeshProUGUI timeAliveText;
    public TextMeshProUGUI wavesCompletedText;
    public TextMeshProUGUI enemiesKilledText;
    public TextMeshProUGUI damageDealtText;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI highScoreText;

    #endregion

    #endregion

    private void Awake()
    {
        // Static assignment
        showHUD = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        #region Intialisation

        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
        sceneLoader = GameObject.FindGameObjectWithTag("LoadingScreen").GetComponent<SceneLoader>();

        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        if (showHUD)
        {
            #region Format Time

            // The exact time minus the closest floored whole value
            int decimalSeconds = Mathf.FloorToInt((playerStats.currentRunTime - Mathf.Floor(playerStats.currentRunTime)) * 100f);

            // The floored excess leading up to 60
            int seconds = Mathf.FloorToInt(playerStats.currentRunTime % 60f);

            // The floored excess leading up to 3600, minus the second precision and scaled down to within 60 second range
            int minutes = (Mathf.FloorToInt(playerStats.currentRunTime % 3600f) - seconds) / 60;

            // The floored timer minus the second-values of seconds and minutes, scaled down to a 3600 second range
            int hours = (Mathf.FloorToInt(playerStats.currentRunTime) - (minutes * 60) - (seconds)) / 3600;

            #endregion

            #region Display Stats

            attemptsText.text = "<b>Attempt</b>\n#<size=150%>" + playerStats.attemptCount;

            timeAliveText.text = "<b>Time Alive:</b><size=130%> " + hours.ToString("D2") + ":" + minutes.ToString("D2") + ":" + seconds.ToString("D2") + "." + decimalSeconds.ToString("D2");
            wavesCompletedText.text = "<b>Waves Completed:</b><size=130%> " + (WaveManager.waveNumber - 1).ToString("N0");
            enemiesKilledText.text = "<b>Enemies Killed:</b><size=130%> " + playerStats.enemiesKilled.ToString("N0");
            damageDealtText.text = "<b>Damage Dealt:</b><size=130%> " + playerStats.damageDealt.ToString("N0");

            finalScoreText.text = "<b>Final Score:</b><size=120%> " + playerStats.currentScore.ToString("N0");
            highScoreText.text = "<b>High Score:</b><size=120%> " + playerStats.highscore.ToString("N0") + (((int)playerStats.currentScore == playerStats.highscore) ? " (NEW BEST!)" : "");

            #endregion
        }

        // Set score screen on/off
        scoreScreenCanvas.enabled = showHUD;
        blurEffect.SetActive(showHUD);
        
        if (showHUD)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    // Reload the current scene
    public void RestartStage()
    {
        sceneLoader.LoadSceneWithProgress(SceneManager.GetActiveScene().buildIndex);
    }

    // Return to the main menu
    public void ReturnToMenu()
    {
        sceneLoader.LoadSceneWithProgress(0);
    }
}
