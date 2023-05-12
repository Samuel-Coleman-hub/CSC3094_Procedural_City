using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ZoneDropDown : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI numberText;
    [SerializeField] public Toggle generateRoadToggle;
    [SerializeField] public Slider roadLengthSlider;
    [SerializeField] public TMP_Dropdown roadPatternDropDown;
    [SerializeField] public Slider buildingDensitySlider;
    [SerializeField] public Slider buildingHeightSlider;
    [SerializeField] public Slider buildingWidthSlider;
    [SerializeField] public Toggle tallerCenterToggle;
    [SerializeField] public Toggle widerOutskirtToggle;
    [SerializeField] public Slider treeDensitySlider;
    [SerializeField] public Slider turbineDensity;
}
