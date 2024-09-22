using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Resource Data", menuName = "Map Data/Structure Data")]
public class StructureSetting : ScriptableObject
{
    public float minSpawnRange; //How far away from the center of the map does it spawn
    public float maxSpawnRange;
    public GameObject structurePrefab;
    public int structureCount;
}
