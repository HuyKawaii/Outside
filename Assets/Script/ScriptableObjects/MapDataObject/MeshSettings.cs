using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Terrain Data", menuName = "Map Data/Terrain Data")]
public class MeshSettings : UpdatableData
{
    public int scale;

    public const int numberOfSupportedLod = 5;
    public const int numberOfSupportedChunkSizes = 9;
    public static readonly int[] supportedChunkSizes = { 48, 72, 96, 120, 144, 168, 192, 216, 240 };
    
    [Range(0, numberOfSupportedChunkSizes - 1)]
    public int chunkSizeIndex;
    //The number of vertices per line when rendering at LOD = 0. Including 2 extra vertices that excluded from final mesh but used for caculation
    public int verticesPerLine
    { get { return supportedChunkSizes[chunkSizeIndex] + 1; } }

    public int meshUnscaledSize
    { get { return verticesPerLine - 3; } }

    public float meshWorldSize 
    { get { return meshUnscaledSize * scale; } }
}
