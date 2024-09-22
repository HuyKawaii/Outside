using QFSW.QC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DayNightManager : MonoBehaviour
{
    [SerializeField] 
    private Light sunLight;
    [SerializeField]
    private LightingPreset lightingPreset;
    [SerializeField]
    private Material skyblockMaterial;

    [SerializeField, Range(0, 24)]
    private float timeOfDay;
    public TimeOfDay timePeriod;
    private float startOfDayTime = 6.0f;
    private float endOfDayTime = 18.0f;
    private float timeMultiplier = 24f;
    [SerializeField] private float realTimeInDay;
    [SerializeField] private bool isPaused;

    private float transitionPeriod = 2.0f;
    private string transitionVarString = "_TimePercentage";
    #region Singleton
    public static DayNightManager instance;
    private void Awake()
    {
        instance = this;
    }
    #endregion

    private void Start()
    {
        if (!Application.isPlaying)
            return;
        if (GameManager.Instance.IsGameDataSet())
        {
            EnviromentalData data = GameManager.Instance.GetEnviromentalData();
            timeOfDay = data.timeOfDay;
        }

        timePeriod = TimeOfDay.DayTime;
    }

    private void Update()
    {
        if (sunLight == null)
            return;

        if (Application.isPlaying)
        {
            if (!isPaused)
            {
                timeOfDay += Time.deltaTime * timeMultiplier / realTimeInDay;
                timeOfDay %= 24;
            }
            
            SetDayNight();
            UpdateLightng(timeOfDay);
            TransitionSkyBox(timeOfDay);
        }else
        {
            UpdateLightng(timeOfDay);
            TransitionSkyBox(timeOfDay);
            SetDayNight();
        }
    }

    private void SetDayNight()
    {
        if (timeOfDay >= startOfDayTime && timeOfDay <= endOfDayTime)
        {
            if (timePeriod == TimeOfDay.NightTime)
            {
                timePeriod = TimeOfDay.DayTime;
            }
        }
        else if (timePeriod == TimeOfDay.DayTime)
        {
            timePeriod = TimeOfDay.NightTime;
        }
    }
    private void UpdateLightng(float timeOfDay)
    {
        float timePercentage = timeOfDay / 24;

        if (sunLight != null)
        {
            sunLight.color = lightingPreset.directionalLightColor.Evaluate(timePercentage);
            sunLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercentage * 360f) - 90f, 170f, 0));
        }

        RenderSettings.ambientLight = lightingPreset.ambientColor.Evaluate(timePercentage);
        RenderSettings.fogColor = lightingPreset.fogColor.Evaluate(timePercentage);
    }

    private void TransitionSkyBox(float timeOfDay)
    {
        float temp = Mathf.Min(1.0f, (timeOfDay - endOfDayTime + transitionPeriod / 2) / transitionPeriod);
        if (temp > 0)
        {
            if (skyblockMaterial.GetFloat(transitionVarString) != (1 - temp))
                skyblockMaterial.SetFloat(transitionVarString, 1 - temp);
            return;
        }

        temp = Mathf.Min(1.0f, (timeOfDay - startOfDayTime + transitionPeriod / 2) / transitionPeriod);
        if (temp > 0)
        {
            if (skyblockMaterial.GetFloat(transitionVarString) != temp)
                skyblockMaterial.SetFloat(transitionVarString, temp);
        }
    }

    public float GetTimeOfDay()
    {
        return timeOfDay;
    }

    [Command]
    public void SetTimeOfDay(float newTime)
    {
        timeOfDay = newTime;
    }

    [Command]
    public void PauseTime(bool isPause)
    {
        this.isPaused = isPause;
    }

    public enum TimeOfDay{ DayTime, NightTime};
}
