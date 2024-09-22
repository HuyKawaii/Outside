using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumnManager : MonoBehaviour
{
    public static VolumnManager Instance { private set; get; }
    public float masterVolumn { private set; get; }
    public delegate void MaseterVolumnChangeCallback(float newValue);
    public MaseterVolumnChangeCallback onMasterVolumnChangeCallback;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        LoadSettingSave();
    }

    public void SetMasterVolumn(float masterVolumn)
    {
        this.masterVolumn = masterVolumn;
        //Debug.Log(masterVolumn);

        if (onMasterVolumnChangeCallback != null )
            onMasterVolumnChangeCallback(masterVolumn);
    }

    public void LoadSettingSave()
    {
        Settings savedSettings = GameSavingManager.Instance.LoadSetting();
        if (savedSettings != null) 
            SetMasterVolumn(savedSettings.masterVolumn);
    }

    public void SaveVolumnSetting()
    {
        GameSavingManager.Instance.SaveSetting(masterVolumn);
    }
}
