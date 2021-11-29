using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuScreenManager : MonoBehaviour
{
    public GameObject optionsScreen;
    public Image overlayPanel;

    private float overlayOpacity;

    // Start is called before the first frame update
    void Start()
    {
        DisableOptionsScreen();
        Time.timeScale = 1f;
        overlayOpacity = 1f;
    }

    private void Update()
    {
        // Fade overlay panel out as game starts
        if (overlayOpacity > 0f)
        {
            overlayOpacity -= Time.deltaTime * 0.8f;
        }

        Color newColour = Color.black;
        newColour.a = overlayOpacity * overlayOpacity;
        overlayPanel.color = newColour;
    }

    public void EnableOptionsScreen()
    {
        optionsScreen.SetActive(true);
    }

    public void DisableOptionsScreen()
    {
        optionsScreen.SetActive(false);
    }
}
