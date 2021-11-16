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
    [Tooltip("Options Scene Number")]
    public int optionsSceneNumber = 1;
    [Tooltip("Game Scene Number")]
    public int gameSceneNumber = 2;
    [Tooltip("End Scene Number")]
    public int endSceneNumber = 3;

    //Go to title screen scene
    public void TitleScreenScene()
    {
        SceneManager.LoadScene(titleScreenSceneNumber);
    }
    //Go to title screen scene
    public void OptionsScene()
    {
        SceneManager.LoadScene(optionsSceneNumber);
    }
    //Go to game screen scene
    public void GameScene()
    {
        SceneManager.LoadScene(gameSceneNumber);
    }
    //Go to end screen scene
    public void EndScene()
    {
        SceneManager.LoadScene(endSceneNumber);
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
