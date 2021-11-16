using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Author: Darcy Matheson
// Purpose: Displays the modified value of the sensitivity slider on the pause screen

public class SliderReadoutUI : MonoBehaviour
{
    #region Variables

    private TextMeshProUGUI dataLabel;
    private Slider dataSlider;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        #region Initialisation

        dataSlider = transform.parent.GetComponent<Slider>();
        dataLabel = GetComponent<TextMeshProUGUI>();

        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        // Assign text value
        dataLabel.text = (dataSlider.value / 10f).ToString("F1");
    }
}
