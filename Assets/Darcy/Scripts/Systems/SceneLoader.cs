using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SceneLoader : MonoBehaviour
{
    public Canvas loadingCanvas;
    public Image loadingBar;
    public TextMeshProUGUI loadingProgressText;

    public void Start()
    {
        loadingCanvas.enabled = false;
    }

    public void LoadSceneWithProgress(int buildIndex)
    {
        loadingCanvas.enabled = true;
        StartCoroutine(LoadAsyncProgress(buildIndex));
    }

    IEnumerator LoadAsyncProgress(int buildIndex)
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(buildIndex);
        loadOperation.allowSceneActivation = false;

        while (loadingBar.fillAmount < 1f)
        {
            float loadProgress = Mathf.Clamp01(loadOperation.progress / 0.9f);

            // Smooth bar fill, at least 1 second to allow a smooth transition
            loadingBar.fillAmount += Mathf.Min(0.01f, loadProgress - loadingBar.fillAmount);
            loadingProgressText.text = Mathf.FloorToInt(loadingBar.fillAmount * 100f) + "<size=80%>%";

            yield return new WaitForSecondsRealtime(0.01f);
        }

        yield return new WaitForSecondsRealtime(0.5f);
        loadOperation.allowSceneActivation = true;

        yield return null;
    }
}
