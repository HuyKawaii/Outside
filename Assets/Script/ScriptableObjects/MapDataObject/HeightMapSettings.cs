using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Noise Data", menuName = "Map Data/Noise Data")]
public class HeightMapSettings: UpdatableData
{
    public NoiseSettings noiseSettings;

    public AnimationCurve heightCurve;
    public int heightMultiplier;

    public bool enableFallOff;
    [Range(1, 10)]
    public float fallOffRange = 1;
    [Range(1, 10)]
    public float fallOffOffset = 1;

    public float minHeight
    { get { return heightCurve.Evaluate(0) * heightMultiplier; }}

    public float maxHeight
    {get { return heightCurve.Evaluate(1) * heightMultiplier; }}

    protected override void OnValidate()
    {
        base.OnValidate();
        noiseSettings.ValidateValues();
    }
}
