using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat
{
    [SerializeField]
    float baseValue;
    [SerializeField]
    List<float> modifiers = new List<float>();
    [SerializeField]
    List<float> multiplierModifiers = new List<float>();

    public float GetValue()
    {
        float finalValue = baseValue;
        modifiers.ForEach(x => finalValue += x);
        float mutiplier = 0;
        multiplierModifiers.ForEach(x => mutiplier += x);
        return finalValue * (1 + mutiplier);
    }

    public void AddModifier(float modifier)
    {
        modifiers.Add(modifier);
    }

    public void RemoveModifier(float modifier)
    {
        modifiers.Remove(modifier);
    }

    public void AddMultiplierModifier(float modifier)
    {
        multiplierModifiers.Add(modifier);
    }

    public void RemoveMultiplierModifier(float modifier)
    {
        multiplierModifiers.Remove(modifier);
    }

    public void RemoveAllModifiers()
    {
        modifiers.Clear();
        multiplierModifiers.Clear();
    }
}
