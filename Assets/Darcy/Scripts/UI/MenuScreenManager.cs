using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Author: Darcy Matheson
// Purpose: Manages navigation of the main menu, the tutorial and the options screen.

public class MenuScreenManager : MonoBehaviour
{
    public GameObject menuScreen;
    public GameObject optionsScreen;
    public GameObject tutorialScreen;
    public GameObject tutorialScreen1;
    public GameObject tutorialScreen2;
    public Image overlayPanel;
    public TextMeshProUGUI optionsStatusText;
    public TextMeshProUGUI transitionButtonText;

    private float overlayOpacity;
    private float optionsStatusTextOpacity;

    private int currentTutorialScreen;

    // Start is called before the first frame update
    void Start()
    {
        EnableMenuScreen();
        DisableOptionsScreen();
        DisableTutorialScreen();

        Time.timeScale = 1f;
        overlayOpacity = 1f;
        optionsStatusTextOpacity = 0f;
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

        // Fade overlay panel out as game starts
        if (optionsStatusTextOpacity > 0f)
        {
            optionsStatusTextOpacity -= Time.deltaTime * 0.7f;
        }

        newColour = Color.white;
        newColour.a = optionsStatusTextOpacity * optionsStatusTextOpacity;
        optionsStatusText.color = newColour;
    }

    // MAIN MENU
    public void EnableMenuScreen()
    {
        menuScreen.SetActive(true);
    }

    public void DisableMenuScreen()
    {
        menuScreen.SetActive(false);
    }

    // OPTIONS
    public void EnableOptionsScreen()
    {
        optionsScreen.SetActive(true);
        DisableMenuScreen();
    }

    public void DisableOptionsScreen()
    {
        optionsScreen.SetActive(false);
        EnableMenuScreen();
    }

    public void PulseOptionsText(string message)
    {
        optionsStatusTextOpacity = 1f;
        optionsStatusText.text = message;
    }

    // TUTORIAL
    public void EnableTutorialScreen()
    {
        tutorialScreen.SetActive(true);
        DisableMenuScreen();
    }

    public void DisableTutorialScreen()
    {
        tutorialScreen.SetActive(false);
        EnableMenuScreen();
    }

    public void TutorialScreenTransition()
    {
        // Flip between tutorial screens
        if (currentTutorialScreen == 0)
        {
            // Go to next
            tutorialScreen1.SetActive(false);
            tutorialScreen2.SetActive(true);

            // Change button
            transitionButtonText.text = "Previous";
            currentTutorialScreen = 1;
        }
        else
        {
            // Go to previous
            tutorialScreen1.SetActive(true);
            tutorialScreen2.SetActive(false);

            // Change button
            transitionButtonText.text = "Continue";
            currentTutorialScreen = 0;
        }
    }
}
