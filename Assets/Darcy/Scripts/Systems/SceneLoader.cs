using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

// Author: Darcy Matheson
// Purpose: Responsible for loading different stages and controlling the display of progress

public class SceneLoader : MonoBehaviour
{
    #region Variables

    public Canvas loadingCanvas;
    public Image loadingBar;
    public TextMeshProUGUI loadingProgressText;

    #endregion

    // Start is called before the first frame update
    public void Start()
    {
        #region Initialisation

        // Disable load screen on scene load
        loadingCanvas.enabled = false;

        #endregion
    }

    // Starts the coroutine for loading the level in async and enables the progress UI
    public void LoadSceneWithProgress(int buildIndex)
    {
        loadingCanvas.enabled = true;
        StartCoroutine(LoadAsyncProgress(buildIndex));
    }

    // Display progress and load scene in async
    IEnumerator LoadAsyncProgress(int buildIndex)
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(buildIndex);
        loadOperation.allowSceneActivation = false;

        while (loadingBar.fillAmount < 1f)
        {
            #region Increment Progress to Match Operation State

            float loadProgress = Mathf.Clamp01(loadOperation.progress / 0.9f);

            // Smooth bar fill, at least 1 second to allow a smooth transition
            loadingBar.fillAmount += Mathf.Min(0.01f, loadProgress - loadingBar.fillAmount);

            loadingProgressText.text = Mathf.FloorToInt(loadingBar.fillAmount * 100f) + "<size=80%>%";

            yield return new WaitForSecondsRealtime(0.01f);

            #endregion
        }

        yield return new WaitForSecondsRealtime(0.5f);
        loadOperation.allowSceneActivation = true;

        yield return null;
    }
}
