using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdatableData : ScriptableObject
{
    public System.Action onValuesUpdated;
    public bool autoUpdate;

    protected virtual void OnValidate()
    {
        if (autoUpdate)
            NotifyUpdatedValue();
    }

    public void NotifyUpdatedValue()
    {
        if (onValuesUpdated != null)
            onValuesUpdated();
    }
}
