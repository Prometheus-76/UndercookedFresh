using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Script Author: Keira

public class SceneController : MonoBehaviour
{
    [Header("Scene Numbers")]

    [Tooltip("Title Screen Scene Number")]
    public int titleScreenSceneNumber = 0;
    [Tooltip("Game Scene Number")]
    public int gameSceneNumber = 1;

    //Loads game scene
    //private SceneLoader sceneLoader;

    void Start()
    {
       //sceneLoader = GameObject.FindGameObjectWithTag("LoadingSceen").GetComponent<SceneLoader>;
    }

    //Go to title screen scene
    public void TitleScreenScene()
    {
        SceneManager.LoadScene(titleScreenSceneNumber);
        //sceneLoader.LoadSceneWithProgress(titleScreenSceneNumber);
    }
    //Go to game screen scene
    public void GameScene()
    {
        SceneManager.LoadScene(gameSceneNumber);
        //sceneLoader.LoadSceneWithProgress(gameSceneNumber);
    }
    //Go to next scene
    public void NextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    //Go to previous scene
    public void PreviousScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
    //Quit/Close game
    public void QuitGame()
    {
        Application.Quit();
    }
}
