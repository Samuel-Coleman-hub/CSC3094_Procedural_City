using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Interface : MonoBehaviour
{
    [Header("RunTime Test UI")]
    [SerializeField] private TextMeshProUGUI gridRunTimeText;
    [SerializeField] private TextMeshProUGUI roadRunTimeText;
    [SerializeField] private TextMeshProUGUI buildingRunTimeText;

    [Header("GridSize UI")]
    [SerializeField] private Slider gridSizeSlider;

    [Header("Auto Rotate UI")]
    [SerializeField] private Toggle autoRotateToggle;

    [Header("Zone UI")]
    [SerializeField] private int maxZoneUI;
    [SerializeField] private GameObject zoneUIParent;
    [SerializeField] private GameObject dropDownButtonPrefab;
    [SerializeField] private GameObject dropDownPanelPrefab;

    [SerializeField] private List<GameObject> buttonObjs;
    [SerializeField] private float spaceBetweenButtons = 100f;

    private (String, String, String) times;

    private void Start()
    {
        SetDropDownValues();
    }

    public void GenerateClicked()
    {
        ApplyDropDownValues();

        times = CityManager.Instance.GenerateWithTests();
        gridRunTimeText.text = times.Item1;
        roadRunTimeText.text = times.Item2;
        buildingRunTimeText.text = times.Item3;
    }

    private void OnGenerated()
    {
        
    }

    private void ApplyDropDownValues()
    {
        List<CityZone> zones = CityManager.Instance.Zones;
        for (int i = 0; i < buttonObjs.Count; i++)
        {
            ZoneDropDown zoneDropDown = buttonObjs[i].GetComponent<ZoneDropDown>();

            Debug.Log("Apply for zone " + i);
            zones[i].generateRoadsForZone = zoneDropDown.generateRoadToggle.isOn;
            zones[i].roadLength = (int)zoneDropDown.roadLengthSlider.value;

            //zoneDropDown.roadPatternDropDown.value

            zones[i].chanceOfBuildingPlacement = zoneDropDown.buildingDensitySlider.value;
            zones[i].buildingHeightScaleMultiplier = zoneDropDown.buildingHeightSlider.value;
            zones[i].buildingWidthScaleMultiplier = zoneDropDown.buildingHeightSlider.value;
            //zoneDropDown.tallerCenterToggle.isOn = 
            //wider outskirt

            zones[i].treeDensity = zoneDropDown.treeDensitySlider.value;

            if (zones[i].miscDensity.Count > 0)
            {
                zones[i].miscDensity[0] = zoneDropDown.turbineDensity.value;
            }
        }
    }

    private void SetDropDownValues(int index = default)
    {
        List<CityZone> zones = CityManager.Instance.Zones;

        if(index == default)
        {
            for (int i = 0; i < buttonObjs.Count; i++)
            {
                SetUpDropDown(zones, i);
            }
        }
        else
        {
           SetUpDropDown(zones, index);
        }
    }

    private void SetUpDropDown(List<CityZone> zones, int i)
    {
        ZoneDropDown zoneDropDown = buttonObjs[i].GetComponent<ZoneDropDown>();

        zoneDropDown.generateRoadToggle.isOn = zones[i].generateRoadsForZone;
        zoneDropDown.roadLengthSlider.value = zones[i].roadLength;

        //zoneDropDown.roadPatternDropDown.value

        zoneDropDown.buildingDensitySlider.value = zones[i].chanceOfBuildingPlacement;
        zoneDropDown.buildingHeightSlider.value = zones[i].buildingHeightScaleMultiplier;
        zoneDropDown.buildingHeightSlider.value = zones[i].buildingWidthScaleMultiplier;
        //zoneDropDown.tallerCenterToggle.isOn = 
        //wider outskirt

        zoneDropDown.treeDensitySlider.value = zones[i].treeDensity;

        if (zones[i].miscDensity.Count > 0)
        {
            zoneDropDown.turbineDensity.value = zones[i].miscDensity[0];
        }
    }

    public void GridSizeSlider()
    {
        Debug.Log("Changing size " + (int)gridSizeSlider.value);
        CityManager.Instance.X = (int)gridSizeSlider.value;
        CityManager.Instance.Z = (int)gridSizeSlider.value;
    }

    public void AutoRotateToggle()
    {
        CameraRotator.Instance.autoRotate = autoRotateToggle.isOn;
    }

    public void ShowZoneDropDown()
    {
        GameObject currentButton = EventSystem.current.currentSelectedGameObject;
        GameObject dropDownObj = currentButton.transform.GetChild(0).gameObject;

        if (dropDownObj.activeInHierarchy)
        {
            dropDownObj.SetActive(false);
        }
        else
        {
            dropDownObj.SetActive(true);
        }
        Debug.Log(buttonObjs.Count);

        foreach(GameObject buttonObj in buttonObjs)
        {
            if (buttonObj != currentButton)
            {
                buttonObj.transform.GetChild(0).gameObject.SetActive(false);
            }
        }
    }

    public void AddZoneUI()
    {
        foreach(GameObject buttonObj in buttonObjs)
        {
            buttonObj.transform.GetChild(0).gameObject.SetActive(false);
        }

        if (buttonObjs.Count < maxZoneUI)
        {
            GameObject newZoneUI = Instantiate(dropDownButtonPrefab);
            newZoneUI.transform.SetParent(zoneUIParent.transform, false);

            newZoneUI.GetComponent<Button>().onClick.AddListener(() => ShowZoneDropDown());

            Vector3 previousLocation = buttonObjs.Last().transform.position;
            newZoneUI.transform.position = new Vector3(previousLocation.x + spaceBetweenButtons, previousLocation.y, previousLocation.z);

            buttonObjs.Add(newZoneUI);

            newZoneUI.GetComponent<ZoneDropDown>().numberText.text = buttonObjs.Count.ToString();

            CityZone newZone = new CityZone();
            newZone = CityManager.Instance.DefaultZones[buttonObjs.Count - 1];
            CityManager.Instance.Zones.Add(newZone);
            SetUpDropDown(CityManager.Instance.Zones, buttonObjs.Count - 1);
        }
       
    }

    public void SubtractZoneUI()
    {
        foreach (GameObject buttonObj in buttonObjs)
        {
            buttonObj.transform.GetChild(0).gameObject.SetActive(false);
        }

        if (buttonObjs.Count > 3)
        {
            GameObject.Destroy(buttonObjs[buttonObjs.Count - 1]);
            buttonObjs.RemoveAt(buttonObjs.Count - 1);
            CityManager.Instance.Zones.RemoveAt(CityManager.Instance.Zones.Count -1);
        }

        
    }

    private void Awake()
    {
        if (CityManager.Instance != null)
        {
            CityManager.Instance.cityGenerated += OnGenerated;
        }
    }

    private void OnDestroy()
    {
        if (CityManager.Instance != null)
        {
            CityManager.Instance.cityGenerated -= OnGenerated;
        }
    }

    [System.Serializable]
    public class UIZone
    {
        
        //[SerializeField] public int index;
        [SerializeField] public GameObject button;
        //[SerializeField] public GameObject dropDownPanel;
    }
}

