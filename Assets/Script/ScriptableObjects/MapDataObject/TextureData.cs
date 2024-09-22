using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Texture Data", menuName = "Map Data/Texture Data")]
public class TextureData : UpdatableData
{
    readonly string[] colorVarNameArray = { "_Color", "_Color_1", "_Color_2", "_Color_3", "_Color_4" };
    readonly string[] heightVarNameArray = { "_TerrainHeight", "_TerrainHeight_1", "_TerrainHeight_2", "_TerrainHeight_3", "_TerrainHeight_4" };
    public terrainType[] terrains;
    public Texture2D grassTexture;
    public Texture2D sandTexture;
    public void ApplyToMaterial(Material material, float minHeight, float maxHeight)
    {
        material.SetFloat("_MaxHeight", maxHeight);
        material.SetFloat("_MinHeight", minHeight);

        for (int i = 0; i < terrains.Length; i++)
        {
            material.SetColor(colorVarNameArray[i], terrains[i].color);
            material.SetFloat(heightVarNameArray[i], terrains[i].maxHeight);
        }

        material.SetTexture("_GrassTexture", grassTexture);
        material.SetTexture("_SandTexture", sandTexture);
    }
    [Serializable]
    public struct terrainType
    {
        public Color color;
        public float maxHeight;
    }
}
