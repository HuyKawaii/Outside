using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingMenuUI : MonoBehaviour
{
    [SerializeField] private RectTransform settingMenuPanel;
    [SerializeField] private Slider masterVolumnSlider;
    [SerializeField] private Button closeSettingButton;
    [SerializeField] private Button applySettingButton;

    private void Start()
    {
        settingMenuPanel.gameObject.SetActive(false);
        masterVolumnSlider.value = VolumnManager.Instance.masterVolumn;

        VolumnManager.Instance.onMasterVolumnChangeCallback += (float newValue) =>
        {
            masterVolumnSlider.value = newValue;
        };

        masterVolumnSlider.onValueChanged.AddListener((float value) => 
        {
            VolumnManager.Instance.SetMasterVolumn(value);
        });
        closeSettingButton.onClick.AddListener(() =>
        {
            VolumnManager.Instance.LoadSettingSave();
            gameObject.SetActive(false);
        });
        applySettingButton.onClick.AddListener(() =>
        {
            VolumnManager.Instance.SaveVolumnSetting();
            gameObject.SetActive(false);
        });
    }
}
