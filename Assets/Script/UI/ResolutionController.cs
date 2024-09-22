using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResolutionController : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdown;

    private Resolution[] resolutions;
    private List<Resolution> filteredResolutionList;

    private float currentRefreshRate;
    private int currentResolutionIndex;

    void Start()
    {
        resolutions = Screen.resolutions;
        filteredResolutionList = new List<Resolution>();

        dropdown.ClearOptions();
        currentRefreshRate = Screen.currentResolution.refreshRate;

        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].refreshRate == currentRefreshRate)
            {
                filteredResolutionList.Add(resolutions[i]);
            }
        }

        List<string> options = new List<string>();
        for (int i = 0; i < filteredResolutionList.Count; i++)
        {
            string resolutionOption = filteredResolutionList[i].width + "x" + filteredResolutionList[i].height;
            options.Add(resolutionOption);
            if (filteredResolutionList[i].width == Screen.width && filteredResolutionList[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        dropdown.AddOptions(options);
        dropdown.value = currentResolutionIndex;
        dropdown.RefreshShownValue();
        dropdown.onValueChanged.AddListener(SetResolution);
    }


    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = filteredResolutionList[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, true);
    }
   
}
