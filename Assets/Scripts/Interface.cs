using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Interface : MonoBehaviour
{
    [Header("RunTime Test UI")]
    [SerializeField] private TextMeshProUGUI gridRunTimeText;
    [SerializeField] private TextMeshProUGUI roadRunTimeText;
    [SerializeField] private TextMeshProUGUI buildingRunTimeText;

    private (String, String, String) times;

    public void GenerateClicked()
    {
        times = CityManager.Instance.GenerateWithTests();
        gridRunTimeText.text = times.Item1;
        roadRunTimeText.text = times.Item2;
        buildingRunTimeText.text = times.Item3;
    }

    private void OnGenerated()
    {
        
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


}
