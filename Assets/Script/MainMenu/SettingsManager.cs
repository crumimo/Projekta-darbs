using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class SettingsManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_Dropdown resolutionDropdown; 
    [SerializeField] private Toggle vSyncToggle;             

    private Resolution[] availableResolutions;

    private void Start()
    {
        availableResolutions = Screen.resolutions;
        
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();

        int currentResolutionIndex = 0;

        for (int i = 0; i < availableResolutions.Length; i++)
        {
            string option = availableResolutions[i].width + " x " + availableResolutions[i].height;
            options.Add(option);
            
            if (availableResolutions[i].width == Screen.currentResolution.width &&
                availableResolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        
        vSyncToggle.isOn = QualitySettings.vSyncCount > 0;
    }

    public void ApplySettings()
    {
        int resolutionIndex = resolutionDropdown.value;
        Resolution selectedResolution = availableResolutions[resolutionIndex];
        Screen.SetResolution(selectedResolution.width, selectedResolution.height, Screen.fullScreen);

        
        QualitySettings.vSyncCount = vSyncToggle.isOn ? 1 : 0;

        Debug.Log("Настройки применены: " + selectedResolution.width + "x" + selectedResolution.height + 
                  ", V-Sync: " + (vSyncToggle.isOn ? "Включен" : "Выключен"));
    }
}