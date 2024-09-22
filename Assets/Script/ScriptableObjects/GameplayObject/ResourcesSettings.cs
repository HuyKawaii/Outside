using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Resource Data", menuName = "Map Data/Resource Data")]
public class ResourcesSettings : ScriptableObject
{
    public GameObject prefab;
    public float minNoiseValue;
    public float maxNoiseValue;
    [Range(0f, 1f)]
    public float spawnChance;
    public int veinSize;
    public int minPerChunk;
    public int maxPerChunk;
    public bool isNoiseInRange(float noiseValue)
    {
        return noiseValue >= minNoiseValue && noiseValue <= maxNoiseValue;
    }

    
}
